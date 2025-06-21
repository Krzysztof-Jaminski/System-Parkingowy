using Microsoft.AspNetCore.Mvc;
using System_Parkingowy.Modules.SimulationModule;

namespace System_Parkingowy.Controllers
{
    [ApiController]
    [Route("api/simulation")]
    public class SimulationController : ControllerBase
    {
        private readonly ParkingSimulator _simulator;
        private static Dictionary<int, (bool occupied, string sensorType)> _spotStates = new();

        public SimulationController(ParkingSimulator simulator)
        {
            _simulator = simulator;
            _simulator.OnSpotStatusChanged += (spotId, occupied, sensorType) =>
            {
                _spotStates[spotId] = (occupied, sensorType);
            };
        }

        /// <summary>
        /// [Admin] Startuje symulację zajętości miejsc parkingowych.
        /// Funkcjonalność: Moduł sensorowy (symulacja) – uruchamia generowanie zdarzeń zajęcia/zwolnienia miejsc na podstawie rozkładu Poissona i typów czujników.
        /// </summary>
        [HttpPost("start")]
        public IActionResult StartSimulation()
        {
            _simulator.Start();
            return Ok("Symulacja uruchomiona");
        }

        /// <summary>
        /// [Admin] Zatrzymuje symulację.
        /// Funkcjonalność: Moduł sensorowy (symulacja) – zatrzymuje generowanie zdarzeń przez symulator.
        /// </summary>
        [HttpPost("stop")]
        public IActionResult StopSimulation()
        {
            _simulator.Stop();
            return Ok("Symulacja zatrzymana");
        }

        /// <summary>
        /// [Admin] Zwraca aktualny stan miejsc parkingowych (symulacja).
        /// Funkcjonalność: Monitoring zajętości w czasie rzeczywistym – pozwala na podgląd aktualnego stanu miejsc na podstawie najnowszych zdarzeń z symulatora.
        /// </summary>
        [HttpGet("status")]
        public IActionResult GetSimulationStatus()
        {
            return Ok(_spotStates.Select(kv => new { SpotId = kv.Key, kv.Value.occupied, kv.Value.sensorType }));
        }

        /// <summary>
        /// [Admin] Zwraca historię zajętości danego miejsca parkingowego (symulacja).
        /// Funkcjonalność: Historia zajętości – umożliwia pobranie historii zmian statusu danego miejsca (timestamp, zajętość, typ czujnika) do analizy lub predykcji.
        /// </summary>
        [HttpGet("history/{spotId}")]
        public IActionResult GetSpotHistory(int spotId)
        {
            var history = _simulator.GetHistory(spotId)
                .Select(h => new { h.timestamp, h.occupied, h.sensorType });
            return Ok(history);
        }

        /// <summary>
        /// [Admin] Zwraca historię zajętości wszystkich miejsc parkingowych (do predykcji).
        /// Funkcjonalność: Historia zajętości, predykcja – umożliwia pobranie historii wszystkich miejsc do budowy modeli predykcyjnych.
        /// </summary>
        [HttpGet("history")]
        public IActionResult GetAllHistory()
        {
            var all = _simulator.GetHistoryForPrediction()
                .ToDictionary(
                    kv => kv.Key,
                    kv => kv.Value.Select(h => new { h.timestamp, h.occupied, h.sensorType })
                );
            return Ok(all);
        }

        /// <summary>
        /// [Admin] Ręcznie wywołuje zdarzenie zmiany statusu miejsca (np. do testów API).
        /// Funkcjonalność: Testowanie, symulacja – pozwala ręcznie zasymulować zdarzenie zajęcia/zwolnienia miejsca przez wybrany typ czujnika.
        /// </summary>
        [HttpPost("trigger")]
        public IActionResult TriggerEvent([FromBody] TriggerEventRequest req)
        {
            _simulator.TriggerEvent(req.SpotId, req.Occupied, req.SensorType);
            return Ok("Zdarzenie wywołane");
        }

        public class TriggerEventRequest
        {
            /// <summary>Identyfikator miejsca parkingowego</summary>
            public int SpotId { get; set; }
            /// <summary>Czy miejsce zajęte</summary>
            public bool Occupied { get; set; }
            /// <summary>Typ czujnika (ultrasonic, magnetic, camera)</summary>
            public string SensorType { get; set; }
        }
    }
} 