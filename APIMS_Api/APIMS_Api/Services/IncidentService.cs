using APIMS_Api.Models;
using Microsoft.Extensions.Caching.Memory;

namespace APIMS_Api.Services
{
    public class IncidentService : IIncidentService
    {
        private readonly IMemoryCache _cache;
        private const string CacheKey = "incidents";

        public IncidentService(IMemoryCache cache)
        {
            _cache = cache;
        }

        private List<Incident> GetStore()
        {
            return _cache.GetOrCreate(CacheKey, entry =>
            {
                entry.Priority = CacheItemPriority.NeverRemove;
                return new List<Incident>();
            })!;
        }

        public List<Incident> GetAll() => GetStore();

        public Incident? GetById(Guid id) => GetStore().FirstOrDefault(i => i.Id == id);

        public Incident Create(CreateIncidentRequest request)
        {
            var incidents = GetStore();
            var incident = new Incident
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                Description = request.Description,
                Location = request.Location,
                ReportedBy = request.ReportedBy,
                Status = "Open",
                ReportedAt = DateTime.UtcNow
            };
            incidents.Add(incident);
            return incident;
        }

        public Incident? Update(Guid id, UpdateIncidentRequest request)
        {
            var incident = GetStore().FirstOrDefault(i => i.Id == id);
            if (incident is null) return null;

            if (request.Title is not null) incident.Title = request.Title;
            if (request.Description is not null) incident.Description = request.Description;
            if (request.Location is not null) incident.Location = request.Location;
            if (request.Status is not null) incident.Status = request.Status;

            return incident;
        }

        public bool Delete(Guid id)
        {
            var incidents = GetStore();
            var incident = incidents.FirstOrDefault(i => i.Id == id);
            if (incident is null) return false;

            incidents.Remove(incident);
            return true;
        }
    }
}
