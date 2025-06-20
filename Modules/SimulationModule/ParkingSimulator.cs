using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace System_Parkingowy.Modules.SimulationModule
{
    public class ParkingSimulator
    {
        private readonly Random _random = new Random();
        private Timer _timer;
        private bool _running = false;
        private int _numSpots;
        private double _lambda; // parametr Poissona (średnia liczba zdarzeń na jednostkę czasu)
        public event Action<int, bool, string> OnSpotStatusChanged; // miejsce, zajęte/zwolnione, typ czujnika
        private readonly string[] _sensorTypes = new[] { "ultrasonic", "magnetic", "camera" };

        public ParkingSimulator(int numSpots = 10, double lambda = 2.0)
        {
            _numSpots = numSpots;
            _lambda = lambda;
        }

        public void Start(int intervalMs = 1000)
        {
            if (_running) return;
            _running = true;
            _timer = new Timer(Simulate, null, 0, intervalMs);
        }

        public void Stop()
        {
            _running = false;
            _timer?.Dispose();
        }

        private void Simulate(object state)
        {
            if (!_running) return;
            // Liczba zdarzeń w tej iteracji (rozkład Poissona)
            int events = Poisson(_lambda);
            for (int i = 0; i < events; i++)
            {
                int spotId = _random.Next(1, _numSpots + 1);
                bool occupied = _random.NextDouble() < 0.5;
                string sensorType = _sensorTypes[_random.Next(_sensorTypes.Length)];
                // Symulacja błędu czujnika (np. 5% szansy na błąd)
                if (_random.NextDouble() < 0.05)
                {
                    occupied = !occupied; // fałszywy odczyt
                }
                OnSpotStatusChanged?.Invoke(spotId, occupied, sensorType);
            }
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