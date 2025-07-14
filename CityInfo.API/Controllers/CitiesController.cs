using AutoMapper;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CityInfo.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "MustBeFromLondon")]
    public class CitiesController(ICityRepository cityRepository, IMapper mapper) : ControllerBase
    {
        private readonly ICityRepository cityRepository = cityRepository ?? throw new ArgumentNullException(nameof(cityRepository));
        private readonly IMapper mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        private const int MaxPageSize = 20;
        private const string PaginationHeader = "X-Pagination";

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CityWithoutPointOfInterestsDto>>> GetCities(
            string? name, string? searchQuery, int pageNumber = 1, int pageSize = 10)
        {

            pageSize = pageSize > MaxPageSize ? MaxPageSize : pageSize;

            var (cityEntities, paginationMetaData) = await cityRepository.GetCitiesAsync(name, searchQuery, pageNumber, pageSize);

            Response.Headers.Append(PaginationHeader, JsonConvert.SerializeObject(paginationMetaData));

            return Ok(mapper.Map<IEnumerable<CityWithoutPointOfInterestsDto>>(cityEntities));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCity(int id, bool includePointOfInterests)
        {
            var city = await cityRepository.GetCityAsync(id, includePointOfInterests);
            if (city is null) return NotFound();

            return includePointOfInterests
                ? Ok(mapper.Map<CityDto>(city))
                : Ok(mapper.Map<CityWithoutPointOfInterestsDto>(city));
        }
    }
}
