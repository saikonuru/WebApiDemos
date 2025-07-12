using CityInfo.API.Entities;

namespace CityInfo.API.Services
{
    public interface ICityRepository
    {
        Task<(IEnumerable<City>, PaginationMetaData)> GetCitiesAsync(string? name, string? searchQuery, int? pageNumber, int? pageSize);
        Task<City?> GetCityAsync(int cityId,bool includePointsOfInterest = false);

        Task<IEnumerable<PointOfInterest>> GetPointOfInterestsAsync(int cityId);

        Task<PointOfInterest?> GetPointOfInterestAsync(int cityId, int pointOfInterestId);
        Task<bool> CityExistsAsync(int cityId);

        Task AddPointsOfInterest(int cityId,PointOfInterest pointOfInterest);
        void DeletePointOfInterest(PointOfInterest pointOfInterest);
        Task<bool> SaveChangesAsync();
    }
}
