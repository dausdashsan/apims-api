using APIMS_Api.Models;
using APIMS_Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace APIMS_Api.Controllers
{
    /// <summary>
    /// Controller for retrieving air quality station information.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class StationsController : ControllerBase
    {
        private readonly IAqiService _aqiService;

        public StationsController(IAqiService aqiService)
        {
            _aqiService = aqiService;
        }

        /// <summary>
        /// Get all air quality monitoring stations.
        /// </summary>
        /// <returns>A list of all 70 stations.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StationFeature>>> GetAll()
        {
            var stations = await _aqiService.GetAllAsync();
            return Ok(stations);
        }

        /// <summary>
        /// Filter stations by state name.
        /// </summary>
        /// <param name="stateName">Example: Selangor, Johor, WP Kuala Lumpur.</param>
        /// <returns>Stations in the specified state.</returns>
        [HttpGet("by-state")]
        public async Task<ActionResult<IEnumerable<StationFeature>>> GetByState([FromQuery] string stateName)
        {
            var stations = await _aqiService.GetByStateAsync(stateName);
            if (!stations.Any()) return NotFound();
            return Ok(stations);
        }

        /// <summary>
        /// Filter stations by region name.
        /// </summary>
        /// <param name="regionName">Example: Northern, Central, Southern.</param>
        /// <returns>Stations in the specified region.</returns>
        [HttpGet("by-region")]
        public async Task<ActionResult<IEnumerable<StationFeature>>> GetByRegion([FromQuery] string regionName)
        {
            var stations = await _aqiService.GetByRegionAsync(regionName);
            if (!stations.Any()) return NotFound();
            return Ok(stations);
        }

        /// <summary>
        /// Filter stations by air quality class.
        /// </summary>
        /// <param name="className">Example: Good, Moderate, Unhealthy.</param>
        /// <returns>Stations with the specified air quality class.</returns>
        [HttpGet("by-class")]
        public async Task<ActionResult<IEnumerable<StationFeature>>> GetByClass([FromQuery] string className)
        {
            var stations = await _aqiService.GetByClassAsync(className);
            if (!stations.Any()) return NotFound();
            return Ok(stations);
        }

        /// <summary>
        /// Filter stations by unique station ID.
        /// </summary>
        /// <param name="stationId">Example: CA01R, CA02K.</param>
        /// <returns>The specified station.</returns>
        [HttpGet("by-station")]
        public async Task<ActionResult<IEnumerable<StationFeature>>> GetByStation([FromQuery] string stationId)
        {
            var stations = await _aqiService.GetByStationIdAsync(stationId);
            if (!stations.Any()) return NotFound();
            return Ok(stations);
        }
    }
}
