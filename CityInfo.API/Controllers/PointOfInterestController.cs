// Ignore Spelling: Dto validator

using AutoMapper;
using CityInfo.API.Models;
using CityInfo.API.Services;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;

namespace CityInfo.API.Controllers
{
    [Route("api/cities/{cityId}/pointofinterest")]
    [ApiController]
    public class PointOfInterestController : ControllerBase
    {
        private readonly ILogger<PointOfInterestController> _logger;
        private readonly IMailService _mailService;
        private readonly ICityRepository cityRepository;
        private readonly IMapper mapper;

        public PointOfInterestController(ILogger<PointOfInterestController> logger, IMailService mailService,  ICityRepository  cityRepository, IMapper mapper)
        {
            _logger = logger ?? throw new ArgumentException(null, nameof(logger));
            _mailService = mailService ?? throw new ArgumentException(null, nameof(mailService));
      
            this.cityRepository = cityRepository ?? throw new ArgumentNullException(nameof(cityRepository));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<PointOfInterestDto>>> GetPointOfInterests(int cityId)
        {
            try
            {
                if (!await cityRepository.CityExists(cityId))
                {
                    _logger.LogInformation($"City with id {cityId} wasn't found when accessing points of interest");
                    return NotFound();
                }
                var pointOfInterestsForCity = await cityRepository.GetPointOfInterestsAsync(cityId);
                return Ok(mapper.Map<IEnumerable<PointOfInterestDto>>(pointOfInterestsForCity));
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"An exception while getting point of interest for the city with id {cityId}", ex.Message);

                return StatusCode(StatusCodes.Status500InternalServerError, "A problem happened while handling your request");
            }
        }

        [HttpGet("{pointInterestId}", Name = "GetPointOfInterest")]
        public async Task<ActionResult<PointOfInterestDto>> GetPointOfInterestAsync(int cityId, int pointInterestId)
        {
            var pointOfInterest = await cityRepository.GetPointOfInterestAsync(cityId,pointInterestId);
            return (pointOfInterest is null) ? NotFound() : Ok(mapper.Map<PointOfInterestDto>(pointOfInterest));
        }

        //[HttpPost]
        //public IActionResult CreatePointOfInterest(int cityId, PointOfInterestCreateDto pointOfInterestCreateDto)
        //{
        //    var pointsOfInterests = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);

        //    if (pointsOfInterests is null) return NotFound();

        //    var pointOfInterestList = pointsOfInterests.PointOfInterests.ToList();

        //    var maxPoi = pointOfInterestList.Count > 0 ? pointOfInterestList.Max(p => p.Id) : 0;

        //    PointOfInterestDto pointOfInterestsForCity = new()
        //    {
        //        Id = maxPoi + 1,
        //        Name = pointOfInterestCreateDto.Name,
        //        Description = pointOfInterestCreateDto.Description
        //    };

        //    pointOfInterestList.Add(pointOfInterestsForCity);

        //    pointsOfInterests.PointOfInterests = pointOfInterestList;

        //    return CreatedAtRoute("GetPointOfInterest", new { cityId, pointInterestId = pointOfInterestsForCity.Id }, pointOfInterestsForCity);
        //}

        //[HttpPut("{pointInterestId}")]
        //public IActionResult UpdatePointOfInterest(int cityId, int pointInterestId, PointOfInterestDto pointOfInterestDto)
        //{
        //    var pointsOfInterests = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);

        //    if (pointsOfInterests is null) return NotFound();

        //    var pointOfInterestList = pointsOfInterests.PointOfInterests.ToList();

        //    var pointOfInterestsForCity = pointOfInterestList.FirstOrDefault(p => p.Id == pointInterestId);

        //    if (pointOfInterestsForCity is null) return NotFound();

        //    pointOfInterestsForCity.Name = pointOfInterestDto.Name;
        //    pointOfInterestsForCity.Description = pointOfInterestDto.Description;


        //    pointsOfInterests.PointOfInterests = pointOfInterestList;

        //    return NoContent();
        //}

        //[HttpPatch("{pointInterestId}")]
        //public IActionResult PatchPointOfInterest(int cityId, int pointInterestId, JsonPatchDocument<PointOfInterestUpdateDto> patchDocument)
        //{
        //    var pointsOfInterests = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);

        //    if (pointsOfInterests is null) return NotFound();

        //    var pointOfInterestList = pointsOfInterests.PointOfInterests.ToList();

        //    var poiFromStore = pointOfInterestList.FirstOrDefault(p => p.Id == pointInterestId);

        //    if (poiFromStore is null) return NotFound();

        //    var poiToPatch = new PointOfInterestUpdateDto()
        //    {
        //        Name = poiFromStore.Name,
        //        Description = poiFromStore.Description,
        //    };

        //    patchDocument.ApplyTo(poiToPatch, ModelState);


        //    if (!ModelState.IsValid) return BadRequest(ModelState);
        //    if (!TryValidateModel(poiToPatch)) return BadRequest(ModelState);

        //    poiFromStore.Name = poiToPatch.Name;
        //    poiFromStore.Description = poiToPatch.Description;


        //    pointsOfInterests.PointOfInterests = pointOfInterestList;

        //    return NoContent();
        //}

        //[HttpDelete("{pointInterestId}")]
        //public IActionResult DeletePointOfInterest(int cityId, int pointInterestId)
        //{
        //    var pointsOfInterests = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);

        //    if (pointsOfInterests is null) return NotFound();

        //    var pointOfInterestList = pointsOfInterests.PointOfInterests.ToList();

        //    var pointOfInterestsForCity = pointOfInterestList.FirstOrDefault(p => p.Id == pointInterestId);

        //    if (pointOfInterestsForCity is null) return NotFound();

        //    pointOfInterestList.Remove(pointOfInterestsForCity);
        //    pointsOfInterests.PointOfInterests = pointOfInterestList;
        //    string message = $"City with {cityId} has been deleted";
        //    _logger.LogInformation(message);
        //    _mailService.SenMail(message);

        //    return NoContent();
        //}
    }
}
