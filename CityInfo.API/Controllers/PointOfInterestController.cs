// Ignore Spelling: Dto validator

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
        private readonly LocalMailService _localMailService;

        public PointOfInterestController(ILogger<PointOfInterestController> logger,LocalMailService localMailService)
        {
            _logger = logger ?? throw new ArgumentException(null, nameof(logger));
            _localMailService = localMailService ?? throw new ArgumentException(null, nameof(localMailService));
        }


        [HttpGet]
        public ActionResult<PointOfInterestDto> GetPointOfInterests(int cityId)
        {
            //throw new Exception("A sample exception");

            try
            {
                throw new Exception("A sample exception");
                var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
                if (city == null)
                {
                    _logger.LogInformation($"City with id {cityId} wasn't found when accessing points of interest");
                    return NotFound();
                }
                return Ok(city.PointOfInterest);
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"An exception while getting point of interest for the city with id {cityId}", ex.Message);

                return StatusCode(StatusCodes.Status500InternalServerError, "A problem happened while handling your request");
            }
        }

        [HttpGet("{pointInterestId}", Name = "GetPointOfInterest")]
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

            var pointOfInterestList = city.PointOfInterest.ToList();

            var maxPoi = pointOfInterestList.Count > 0 ? pointOfInterestList.Max(p => p.Id) : 0;

            PointOfInterestDto poi = new()
            {
                Id = maxPoi + 1,
                Name = pointOfInterestCreateDto.Name,
                Description = pointOfInterestCreateDto.Description
            };

            pointOfInterestList.Add(poi);

            city.PointOfInterest = pointOfInterestList;

            return CreatedAtRoute("GetPointOfInterest", new { cityId, pointInterestId = poi.Id }, poi);
        }

        [HttpPut("{pointInterestId}")]
        public IActionResult UpdatePointOfInterest(int cityId, int pointInterestId, PointOfInterestDto pointOfInterestDto)
        {
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);

            if (city is null) return NotFound();

            var pointOfInterestList = city.PointOfInterest.ToList();

            var poi = pointOfInterestList.FirstOrDefault(p => p.Id == pointInterestId);

            if (poi is null) return NotFound();

            poi.Name = pointOfInterestDto.Name;
            poi.Description = pointOfInterestDto.Description;


            city.PointOfInterest = pointOfInterestList;

            return NoContent();
        }

        [HttpPatch("{pointInterestId}")]
        public IActionResult PatchPointOfInterest(int cityId, int pointInterestId, JsonPatchDocument<PointOfInterestUpdateDto> patchDocument)
        {
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);

            if (city is null) return NotFound();

            var pointOfInterestList = city.PointOfInterest.ToList();

            var poiFromStore = pointOfInterestList.FirstOrDefault(p => p.Id == pointInterestId);

            if (poiFromStore is null) return NotFound();

            var poiToPatch = new PointOfInterestUpdateDto()
            {
                Name = poiFromStore.Name,
                Description = poiFromStore.Description,
            };

            patchDocument.ApplyTo(poiToPatch, ModelState);


            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (!TryValidateModel(poiToPatch)) return BadRequest(ModelState);

            poiFromStore.Name = poiToPatch.Name;
            poiFromStore.Description = poiToPatch.Description;


            city.PointOfInterest = pointOfInterestList;

            return NoContent();
        }

        [HttpDelete("{pointInterestId}")]
        public IActionResult DeletePointOfInterest(int cityId, int pointInterestId)
        {
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);

            if (city is null) return NotFound();

            var pointOfInterestList = city.PointOfInterest.ToList();

            var poi = pointOfInterestList.FirstOrDefault(p => p.Id == pointInterestId);

            if (poi is null) return NotFound();

            pointOfInterestList.Remove(poi);
            city.PointOfInterest = pointOfInterestList;
            string message = $"City with {cityId} has been deleted";
            _logger.LogInformation(message);
            _localMailService.SenMail(message);

            return NoContent();
        }
    }
}
