using AutoMapper;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CitiesController(ICityRepository cityRepository,IMapper mapper) : ControllerBase
    {
        private readonly ICityRepository cityRepository = cityRepository ?? throw new ArgumentNullException(nameof(cityRepository));
        private readonly IMapper mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

        //[HttpGet(Name ="mycities")] Not working
        //[HttpGet("mycities")]
        //[HttpGet]
        //public JsonResult GetCities() => new JsonResult(_citiesDataStore.Cities);

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CityDto>>> GetCities()
        {

            var cityEntities = await cityRepository.GetCitiesAsync();

            return Ok(mapper.Map<IEnumerable<CityWithoutPointOfInterestsDto>>(cityEntities));
        }

        // [HttpGet("{id}")]
        // public JsonResult GetCity(int id)
        // {
        //     return new JsonResult(_citiesDataStore.Cities.FirstOrDefault(c => c.Id == id));
        // }

        [HttpGet("{id}")]
        public async Task<ActionResult<CityDto>> GetCity(int id)
        {
            var city = await cityRepository.GetCityAsync(id);
            if (city is null) return NotFound();

            return Ok(mapper.Map<CityWithoutPointOfInterestsDto>(city));
        }
    }
}
