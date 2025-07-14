using Asp.Versioning;
using AutoMapper;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CityInfo.API.Controllers
{
    /// <summary>
    /// City Controller
    /// </summary>
    /// <param name="cityRepository"></param>
    /// <param name="mapper"></param>
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion(1.0)] // Optional as default version is mentioned in Program.cs 
    [Authorize(Policy = "MustBeFromLondon")]

    public class CitiesController(ICityRepository cityRepository, IMapper mapper) : ControllerBase
    {
        private readonly ICityRepository cityRepository = cityRepository ?? throw new ArgumentNullException(nameof(cityRepository));
        private readonly IMapper mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        private const int MaxPageSize = 20;
        private const string PaginationHeader = "X-Pagination";

        /// <summary>
        ///  Get All cities
        /// </summary>
        /// <param name="name"></param>
        /// <param name="searchQuery"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CityWithoutPointOfInterestsDto>>> GetCities(
            string? name, string? searchQuery, int pageNumber = 1, int pageSize = 10)
        {

            pageSize = pageSize > MaxPageSize ? MaxPageSize : pageSize;

            var (cityEntities, paginationMetaData) = await cityRepository.GetCitiesAsync(name, searchQuery, pageNumber, pageSize);

            Response.Headers.Append(PaginationHeader, JsonConvert.SerializeObject(paginationMetaData));

            return Ok(mapper.Map<IEnumerable<CityWithoutPointOfInterestsDto>>(cityEntities));
        }

        /// <summary>
        /// Get city info by Id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="includePointOfInterests"></param>
        /// <returns></returns>
        /// <response code="200">Returns the requested city</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
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
