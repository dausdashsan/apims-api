using APIMS_Api.Models;
using APIMS_Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace APIMS_Api.Controllers
{
    /// <summary>
    /// Controller for reporting and managing incidents.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class IncidentsController(IIncidentService incidentService) : ControllerBase
    {
        /// <summary>
        /// Get all reported incidents.
        /// </summary>
        [HttpGet]
        public ActionResult<IEnumerable<Incident>> GetAll()
        {
            return Ok(incidentService.GetAll());
        }

        /// <summary>
        /// Get an incident by ID.
        /// </summary>
        /// <param name="id">The incident's GUID.</param>
        [HttpGet("{id:guid}")]
        public ActionResult<Incident> GetById(Guid id)
        {
            var incident = incidentService.GetById(id);
            if (incident is null) return NotFound();
            return Ok(incident);
        }

        /// <summary>
        /// Report a new incident via JSON body.
        /// </summary>
        /// <param name="request">Incident details as JSON.</param>
        [HttpPost("json")]
        [Consumes("application/json")]
        public ActionResult<Incident> CreateJson([FromBody] CreateIncidentRequest request)
        {
            var incident = incidentService.Create(request);
            return CreatedAtAction(nameof(GetById), new { id = incident.Id }, incident);
        }

        /// <summary>
        /// Report a new incident via multipart/form-data.
        /// </summary>
        /// <param name="request">Incident details as form fields.</param>
        [HttpPost("form-data")]
        [Consumes("multipart/form-data")]
        public ActionResult<Incident> CreateFormData([FromForm] CreateIncidentRequest request)
        {
            var incident = incidentService.Create(request);
            return CreatedAtAction(nameof(GetById), new { id = incident.Id }, incident);
        }

        /// <summary>
        /// Report a new incident via application/x-www-form-urlencoded.
        /// </summary>
        /// <param name="request">Incident details as URL-encoded form fields.</param>
        [HttpPost("form-urlencoded")]
        [Consumes("application/x-www-form-urlencoded")]
        public ActionResult<Incident> CreateFormUrlEncoded([FromForm] CreateIncidentRequest request)
        {
            var incident = incidentService.Create(request);
            return CreatedAtAction(nameof(GetById), new { id = incident.Id }, incident);
        }

        /// <summary>
        /// Update an existing incident.
        /// </summary>
        /// <param name="id">The incident's GUID.</param>
        /// <param name="request">Fields to update. Status can be: Open, InProgress, Resolved, Closed.</param>
        [HttpPut("{id:guid}")]
        public ActionResult<Incident> Update(Guid id, [FromBody] UpdateIncidentRequest request)
        {
            var incident = incidentService.Update(id, request);
            if (incident is null) return NotFound();
            return Ok(incident);
        }

        /// <summary>
        /// Delete an incident by ID.
        /// </summary>
        /// <param name="id">The incident's GUID.</param>
        [HttpDelete("{id:guid}")]
        public IActionResult Delete(Guid id)
        {
            if (!incidentService.Delete(id)) return NotFound();
            return NoContent();
        }
    }
}
