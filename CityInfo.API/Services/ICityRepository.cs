using CityInfo.API.Entities;

namespace CityInfo.API.Services
{
    public interface ICityRepository
    {
        Task<IEnumerable<City>> GetCitiesAsync();
        Task<City?> GetCityAsync(int cityId,bool includePointsOfInterest = false);

        Task<IEnumerable<PointOfInterest>> GetPointOfInterestsAsync(int cityId);

        Task<PointOfInterest?> GetPointOfInterestAsync(int cityId, int pointOfInterestId);
        Task<bool> CityExists(int cityId);
    }
}
