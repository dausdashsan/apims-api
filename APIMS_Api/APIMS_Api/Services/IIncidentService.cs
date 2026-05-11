using APIMS_Api.Models;

namespace APIMS_Api.Services
{
    public interface IIncidentService
    {
        List<Incident> GetAll();
        Incident? GetById(Guid id);
        Incident Create(CreateIncidentRequest request);
        Incident? Update(Guid id, UpdateIncidentRequest request);
        bool Delete(Guid id);
    }
}
