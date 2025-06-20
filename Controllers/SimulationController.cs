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
        /// </summary>
        [HttpPost("start")]
        public IActionResult StartSimulation()
        {
            _simulator.Start();
            return Ok("Symulacja uruchomiona");
        }

        /// <summary>
        /// [Admin] Zatrzymuje symulację.
        /// </summary>
        [HttpPost("stop")]
        public IActionResult StopSimulation()
        {
            _simulator.Stop();
            return Ok("Symulacja zatrzymana");
        }

        /// <summary>
        /// [Admin] Zwraca aktualny stan miejsc parkingowych (symulacja).
        /// </summary>
        [HttpGet("status")]
        public IActionResult GetSimulationStatus()
        {
            return Ok(_spotStates.Select(kv => new { SpotId = kv.Key, kv.Value.occupied, kv.Value.sensorType }));
        }
    }
} 