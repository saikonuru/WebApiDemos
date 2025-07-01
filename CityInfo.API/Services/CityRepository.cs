using CityInfo.API.DbContexts;
using CityInfo.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace CityInfo.API.Services
{
    public class CityRepository(CityInfoContext cityInfoContext) : ICityRepository
    {
        private readonly CityInfoContext cityInfoContext = cityInfoContext ?? throw new ArgumentNullException(nameof(cityInfoContext));

        public async Task<IEnumerable<City>> GetCitiesAsync() => await cityInfoContext.Cities.ToListAsync();

        public async Task<City?> GetCityAsync(int cityId) => await cityInfoContext.Cities.OrderBy(c => c.Id).FirstOrDefaultAsync(c => c.Id == cityId);

        public async Task<City?> GetCityAsync(int cityId, bool includePointOfInterest)
        {
            var query = cityInfoContext.Cities.AsQueryable();

            if (includePointOfInterest)
                query = query.Include(c => c.PointOfInterests);

            return await query.FirstOrDefaultAsync(c => c.Id == cityId);
        }

        public async Task<PointOfInterest?> GetPointOfInterestAsync(int cityId, int pointOfInterestId)
        {
            return await cityInfoContext.PointOfInterest
                .FirstOrDefaultAsync(p => p.CityId == cityId && p.Id == pointOfInterestId);
        }

        public async Task<IEnumerable<PointOfInterest>> GetPointOfInterestsAsync(int cityId)
        {
            return await cityInfoContext.PointOfInterest
                .Where(p => p.CityId == cityId).ToListAsync();
        }
    }
}
