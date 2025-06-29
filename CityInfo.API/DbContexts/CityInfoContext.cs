using CityInfo.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace CityInfo.API.DbContexts
{
    public class CityInfoContext(DbContextOptions<CityInfoContext> options) : DbContext(options)
    {
        public required DbSet<City> Cities { get; set; }
        public required DbSet<PointOfInterest> PointOfInterest { get; set; }
    }
}
