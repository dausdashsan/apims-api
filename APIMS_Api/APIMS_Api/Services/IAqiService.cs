using APIMS_Api.Models;

namespace APIMS_Api.Services
{
    public interface IAqiService
    {
        Task<List<StationFeature>> GetAllAsync();
        Task<List<StationFeature>> GetByStateAsync(string stateName);
        Task<List<StationFeature>> GetByRegionAsync(string regionName);
        Task<List<StationFeature>> GetByClassAsync(string className);
        Task<List<StationFeature>> GetByStationIdAsync(string stationId);
    }
}
