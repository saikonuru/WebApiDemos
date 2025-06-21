// Ignore Spelling: Dto

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

        [HttpGet("{pointInterestId}",Name = "GetPointOfInterest")]
        public ActionResult<PointOfInterestDto> GetPointOfInterest(int cityId, int pointInterestId)
        {
            var pointOfInterest = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId)?.PointOfInterest.FirstOrDefault(p => p.Id == pointInterestId);
            return (pointOfInterest is null) ? NotFound() : Ok(pointOfInterest);
        }

        [HttpPost]
        public IActionResult CreatePointOfInterest(int cityId, PointOfInterestCreateDto pointOfInterestCreateDto)
        {
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);

            if (city is null) return NotFound();
           
            var maxPoi = CitiesDataStore.Current.Cities.SelectMany(p=>p.PointOfInterest).Max(p=>p.Id);

            PointOfInterestDto poi = new()
            { Id = maxPoi + 1, Name = pointOfInterestCreateDto.Name, Description = pointOfInterestCreateDto.Description };
            city.PointOfInterest.ToList().Add(poi);

            return CreatedAtRoute("GetPointOfInterest",new { cityId, pointInterestId = poi.Id },poi);
        }

    }
}
