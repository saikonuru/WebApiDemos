using CityInfo.API.DbContexts;
using CityInfo.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace CityInfo.API.Services
{
    public class CityRepository(CityInfoContext cityInfoContext) : ICityRepository
    {
        private readonly CityInfoContext cityInfoContext = cityInfoContext ?? throw new ArgumentNullException(nameof(cityInfoContext));

        public async Task<(IEnumerable<City>, PaginationMetaData)> GetCitiesAsync(string? name, string? searchQuery, int? pageNumber, int? pageSize)
        {
            // Start with all cities as a queryable collection
            var collection = cityInfoContext.Cities.AsQueryable();

            // Filter by exact name if provided
            if (!string.IsNullOrWhiteSpace(name))
            {
                name = name.Trim();
                collection = collection.Where(x => x.Name == name);
            }

            // Filter by search query if provided (in name or description)
            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                searchQuery = searchQuery.Trim();
                collection = collection.Where(a =>
                    a.Name.Contains(searchQuery) ||
                    (a.Description != null && a.Description.Contains(searchQuery)));
            }

            // Ensure pageNumber and pageSize are not null and provide default values if necessary
            var pageNumberValue = pageNumber ?? 1;
            var pageSizeValue = pageSize ?? 10;
            var totalItemCount = await collection.CountAsync();

            var paginationMetaData = new PaginationMetaData(totalItemCount, pageSizeValue, pageNumberValue);

            // Order by name and return as a list
            var collectionToRuturn = await collection.OrderBy(c => c.Name)
               .Skip(pageSizeValue * (pageNumberValue - 1))
               .Take(pageSizeValue)
               .ToListAsync();

            return (collectionToRuturn, paginationMetaData);
        }

        public async Task<City?> GetCityAsync(int cityId, bool includePointOfInterest = false)
        {
            var query = cityInfoContext.Cities.AsQueryable();

            if (includePointOfInterest)
                query = query.Include(c => c.PointOfInterests);

            return await query.FirstOrDefaultAsync(c => c.Id == cityId);
        }

        public async Task<bool> CityNameMatchWithIdAsync(int cityId,string cityName) =>
                    await cityInfoContext.Cities.AnyAsync(c => (c.Id == cityId && c.Name == cityName));

        public async Task<bool> CityExistsAsync(int cityId) =>
            await cityInfoContext.Cities.AnyAsync(c => c.Id == cityId);

        public async Task<PointOfInterest?> GetPointOfInterestAsync(int cityId, int pointOfInterestId) =>
            await cityInfoContext.PointOfInterest.FirstOrDefaultAsync(p => p.CityId == cityId && p.Id == pointOfInterestId);

        public async Task<IEnumerable<PointOfInterest>> GetPointOfInterestsAsync(int cityId) =>
            await cityInfoContext.PointOfInterest.Where(p => p.CityId == cityId).ToListAsync();

        public async Task AddPointsOfInterest(int cityId, PointOfInterest pointOfInterest)
        {
            var city = await GetCityAsync(cityId);
            city?.PointOfInterests.Add(pointOfInterest);
        }


        public void DeletePointOfInterest(PointOfInterest pointOfInterest) => cityInfoContext.PointOfInterest.Remove(pointOfInterest);


        public async Task<bool> SaveChangesAsync()
        {
            return (await cityInfoContext.SaveChangesAsync() > 0);
        }
    }
}
