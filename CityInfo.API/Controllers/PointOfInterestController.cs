using CityInfo.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [Route("api/cities/{cityId}/pointofinterest")]
    [ApiController]
    public class PointOfInterestController : ControllerBase
    {
        [HttpGet]
        public ActionResult<PointOfInterestDto> GetPointOfInterests(int cityId)
        {
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            return (city is null) ? NotFound() : Ok(city.PointOfInterest);
        }

        [HttpGet("{pointInterestId}")]
        public ActionResult<PointOfInterestDto> GetPointOfInterests(int cityId, int pointInterestId)
        {
            var pointOfInterest = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId)?.PointOfInterest.FirstOrDefault(p => p.Id == pointInterestId);
            return (pointOfInterest is null) ? NotFound() : Ok(pointOfInterest);
        }
    }
}
