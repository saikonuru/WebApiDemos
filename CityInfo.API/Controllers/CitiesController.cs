using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CitiesController : ControllerBase
    {
        private readonly CitiesDataStore _citiesDataStore;
        public CitiesController(CitiesDataStore citiesDataStore)
        {
           _citiesDataStore = citiesDataStore ?? throw new ArgumentException(null, nameof(citiesDataStore));

        }

        //[HttpGet(Name ="mycities")] Not working
        //[HttpGet("mycities")]
        //[HttpGet]
        //public JsonResult GetCities() => new JsonResult(_citiesDataStore.Cities);

        [HttpGet]
        public ActionResult<IEnumerable<CityDto>> GetCities() => Ok(_citiesDataStore.Cities);

        // [HttpGet("{id}")]
        // public JsonResult GetCity(int id)
        // {
        //     return new JsonResult(_citiesDataStore.Cities.FirstOrDefault(c => c.Id == id));
        // }

        [HttpGet("{id}")]
        public ActionResult<CityDto> GetCity(int id)
        {
            var city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == id);
            if (city is null) return NotFound();

            return Ok(city);
        }
    }
}
