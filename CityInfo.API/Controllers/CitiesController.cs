using CityInfo.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CitiesController : ControllerBase
    {
        //[HttpGet(Name ="mycities")] Not working
        //[HttpGet("mycities")]
        //[HttpGet]
        //public JsonResult GetCities() => new JsonResult(CitiesDataStore.Current.Cities);

        [HttpGet]
        public ActionResult<IEnumerable<CityDto>> GetCities() => Ok(CitiesDataStore.Current.Cities);

        // [HttpGet("{id}")]
        // public JsonResult GetCity(int id)
        // {
        //     return new JsonResult(CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == id));
        // }

        [HttpGet("{id}")]
        public ActionResult<CityDto> GetCity(int id)
        {
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == id);
            if (city is null) return NotFound();

            return Ok(city);
        }
    }
}
