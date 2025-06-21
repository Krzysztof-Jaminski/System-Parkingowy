using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace System_Parkingowy.Modules.SimulationModule
{
    /// <summary>
    /// Symulator miejsc parkingowych generujący zdarzenia zajęcia/zwolnienia miejsc w oparciu o rozkład Poissona i różne typy czujników.
    /// Przechowuje historię zmian zajętości dla każdego miejsca.
    /// </summary>
    public class ParkingSimulator
    {
        private readonly Random _random = new Random();
        private Timer _timer;
        private bool _running = false;
        private int _numSpots;
        private double _lambda; // parametr Poissona (średnia liczba zdarzeń na jednostkę czasu)
        public event Action<int, bool, string> OnSpotStatusChanged; // miejsce, zajęte/zwolnione, typ czujnika
        private readonly string[] _sensorTypes = new[] { "ultrasonic", "magnetic", "camera" };

        // Historia zmian zajętości: spotId -> lista zmian (timestamp, zajętość, typ czujnika)
        private readonly Dictionary<int, List<(DateTime timestamp, bool occupied, string sensorType)>> _history = new();

        /// <summary>
        /// Tworzy nowy symulator parkingu.
        /// </summary>
        public ParkingSimulator(int numSpots = 10, double lambda = 2.0)
        {
            _numSpots = numSpots;
            _lambda = lambda;
            for (int i = 1; i <= numSpots; i++)
                _history[i] = new List<(DateTime, bool, string)>();
        }

        /// <summary>
        /// Rozpoczyna automatyczną symulację.
        /// </summary>
        public void Start(int intervalMs = 1000)
        {
            if (_running) return;
            _running = true;
            _timer = new Timer(Simulate, null, 0, intervalMs);
        }

        /// <summary>
        /// Zatrzymuje symulację.
        /// </summary>
        public void Stop()
        {
            _running = false;
            _timer?.Dispose();
        }

        /// <summary>
        /// Ręcznie wywołuje zdarzenie zmiany statusu miejsca (np. do testów API).
        /// </summary>
        public void TriggerEvent(int spotId, bool occupied, string sensorType)
        {
            AddHistory(spotId, occupied, sensorType);
            OnSpotStatusChanged?.Invoke(spotId, occupied, sensorType);
        }

        private void Simulate(object state)
        {
            if (!_running) return;
            int events = Poisson(_lambda);
            for (int i = 0; i < events; i++)
            {
                int spotId = _random.Next(1, _numSpots + 1);
                bool occupied = _random.NextDouble() < 0.5;
                string sensorType = _sensorTypes[_random.Next(_sensorTypes.Length)];
                if (_random.NextDouble() < 0.05)
                {
                    occupied = !occupied; // fałszywy odczyt
                }
                AddHistory(spotId, occupied, sensorType);
                OnSpotStatusChanged?.Invoke(spotId, occupied, sensorType);
            }
        }

        private void AddHistory(int spotId, bool occupied, string sensorType)
        {
            if (!_history.ContainsKey(spotId))
                _history[spotId] = new List<(DateTime, bool, string)>();
            _history[spotId].Add((DateTime.UtcNow, occupied, sensorType));
            if (_history[spotId].Count > 1000)
                _history[spotId].RemoveAt(0); // ogranicz historię
        }

        /// <summary>
        /// Zwraca historię zajętości danego miejsca parkingowego.
        /// </summary>
        public List<(DateTime timestamp, bool occupied, string sensorType)> GetHistory(int spotId)
        {
            return _history.ContainsKey(spotId) ? new List<(DateTime, bool, string)>(_history[spotId]) : new List<(DateTime, bool, string)>();
        }

        /// <summary>
        /// Zwraca historię zajętości wszystkich miejsc parkingowych (do predykcji).
        /// </summary>
        public Dictionary<int, List<(DateTime timestamp, bool occupied, string sensorType)>> GetHistoryForPrediction()
        {
            // Zwraca kopię historii dla wszystkich miejsc
            var result = new Dictionary<int, List<(DateTime, bool, string)>>();
            foreach (var kv in _history)
                result[kv.Key] = new List<(DateTime, bool, string)>(kv.Value);
            return result;
        }

        // Generator liczb z rozkładu Poissona
        private int Poisson(double lambda)
        {
            double L = Math.Exp(-lambda);
            int k = 0;
            double p = 1.0;
            do
            {
                k++;
                p *= _random.NextDouble();
            } while (p > L);
            return k - 1;
        }
    }
} 