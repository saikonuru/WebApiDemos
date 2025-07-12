using AutoMapper;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace CityInfo.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CitiesController(ICityRepository cityRepository,IMapper mapper) : ControllerBase
    {
        private readonly ICityRepository cityRepository = cityRepository ?? throw new ArgumentNullException(nameof(cityRepository));
        private readonly IMapper mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        private const int maxPageSize = 20;

        //[HttpGet(Name ="mycities")] Not working
        //[HttpGet("mycities")]
        //[HttpGet]
        //public JsonResult GetCities() => new JsonResult(_citiesDataStore.Cities);

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CityDto>>> GetCities(string? name, string? searchQuery, int? pageNumber, int? pageSize)
        {

            pageSize = pageSize > maxPageSize ? maxPageSize : pageSize;

            var (cityEntities,paginationMetaData) = await cityRepository.GetCitiesAsync(name,searchQuery,pageNumber,pageSize);


            Response.Headers["X-Pagination"] = JsonSerializer.Serialize(value: paginationMetaData);


            return Ok(mapper.Map<IEnumerable<CityWithoutPointOfInterestsDto>>(cityEntities));
        }

        // [HttpGet("{id}")]
        // public JsonResult GetCity(int id)
        // {
        //     return new JsonResult(_citiesDataStore.Cities.FirstOrDefault(c => c.Id == id));
        // }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCity(int id, bool includePointOfInterests)
        {
            var city = await cityRepository.GetCityAsync(id, includePointOfInterests);
            if (city is null) return NotFound();

            if (includePointOfInterests)
            {
                var result = mapper.Map<CityDto>(city);
                return Ok(result);

            }
                
            else
                return Ok(mapper.Map<CityWithoutPointOfInterestsDto>(city));
        }
    }
}
