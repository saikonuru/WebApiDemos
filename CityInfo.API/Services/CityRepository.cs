using CityInfo.API.DbContexts;
using CityInfo.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace CityInfo.API.Services
{
    public class CityRepository(CityInfoContext cityInfoContext) : ICityRepository
    {
        private readonly CityInfoContext cityInfoContext = cityInfoContext ?? throw new ArgumentNullException(nameof(cityInfoContext));

        public async Task<IEnumerable<City>> GetCitiesAsync() => await cityInfoContext.Cities.ToListAsync();


        public async Task<City?> GetCityAsync(int cityId, bool includePointOfInterest = false)
        {
            var query = cityInfoContext.Cities.AsQueryable();

            if (includePointOfInterest)
                query = query.Include(c => c.PointOfInterests);

            return await query.FirstOrDefaultAsync(c => c.Id == cityId);
        }

        public async Task<bool> CityExists(int cityId) =>
            await cityInfoContext.Cities.AnyAsync(c => c.Id == cityId);

        public async Task<PointOfInterest?> GetPointOfInterestAsync(int cityId, int pointOfInterestId) =>
            await cityInfoContext.PointOfInterest.FirstOrDefaultAsync(p => p.CityId == cityId && p.Id == pointOfInterestId);

        public async Task<IEnumerable<PointOfInterest>> GetPointOfInterestsAsync(int cityId) =>
            await cityInfoContext.PointOfInterest.Where(p => p.CityId == cityId).ToListAsync();
    }
}
