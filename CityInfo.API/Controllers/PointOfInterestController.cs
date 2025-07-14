// Ignore Spelling: Dto validator

using Asp.Versioning;
using AutoMapper;
using CityInfo.API.Models;
using CityInfo.API.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;

namespace CityInfo.API.Controllers
{
    [Route("api/v{version:apiVersion}/cities/{cityId}/pointofinterest")]
    [ApiController]
    [Authorize(Policy = "MustBeFromLondon")]
    [ApiVersion(1)]
    [ApiVersion(2)]
    public class PointOfInterestController(ILogger<PointOfInterestController> logger, IMailService mailService, ICityRepository cityRepository, IMapper mapper) : ControllerBase
    {
        private readonly ILogger<PointOfInterestController> _logger = logger ?? throw new ArgumentException(null, nameof(logger));
        private readonly IMailService _mailService = mailService ?? throw new ArgumentException(null, nameof(mailService));
        private readonly ICityRepository cityRepository = cityRepository ?? throw new ArgumentNullException(nameof(cityRepository));
        private readonly IMapper mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PointOfInterestDto>>> GetPointOfInterests(int cityId)
        {
            try
            {
                var cityName = User.Claims.FirstOrDefault(c => c.Type == "city")?.Value;

                if (string.IsNullOrEmpty(cityName) || !await cityRepository.CityNameMatchWithIdAsync(cityId, cityName))
                {
                    return Forbid();
                }


                if (!await cityRepository.CityExistsAsync(cityId))
                {
                    _logger.LogInformation($"City with id {cityId} wasn't found when accessing points of interest");
                    return NotFound();
                }
                var pointOfInterestsForCity = await cityRepository.GetPointOfInterestsAsync(cityId);
                IEnumerable<PointOfInterestDto> result = mapper.Map<IEnumerable<PointOfInterestDto>>(pointOfInterestsForCity);
                return Ok(result);
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
            Entities.PointOfInterest? pointOfInterest = await cityRepository.GetPointOfInterestAsync(cityId, pointInterestId);
            PointOfInterestDto result = mapper.Map<PointOfInterestDto>(pointOfInterest);
            return (pointOfInterest is null) ? NotFound() : Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePointOfInterestAsync(int cityId, PointOfInterestCreateDto pointOfInterestDto)
        {
            if (!await cityRepository.CityExistsAsync(cityId)) return NotFound();

            Entities.PointOfInterest pointOfInterestEntity = mapper.Map<Entities.PointOfInterest>(pointOfInterestDto);

            await cityRepository.AddPointsOfInterest(cityId, pointOfInterestEntity);

            await cityRepository.SaveChangesAsync();

            PointOfInterestDto pointOfInterestsForCity = mapper.Map<PointOfInterestDto>(pointOfInterestEntity);


            return CreatedAtRoute("GetPointOfInterest", new { cityId, pointInterestId = pointOfInterestsForCity.Id }, pointOfInterestsForCity);
        }

        [HttpPut("{pointInterestId}")]
        public async Task<IActionResult> UpdatePointOfInterestAsync(int cityId, int pointInterestId, PointOfInterestUpdateDto pointOfInterestDto)
        {

            if (!await cityRepository.CityExistsAsync(cityId)) return NotFound();

            var pointOfInterestEntity = await cityRepository.GetPointOfInterestAsync(cityId, pointInterestId);


            if (pointOfInterestEntity is null) return NotFound();
            mapper.Map(pointOfInterestDto, pointOfInterestEntity);
            await cityRepository.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("{pointInterestId}")]
        public async Task<IActionResult> PatchPointOfInterestAsync(int cityId, int pointInterestId, JsonPatchDocument<PointOfInterestUpdateDto> patchDocument)
        {

            if (!await cityRepository.CityExistsAsync(cityId)) return NotFound();

            var pointOfInterestEntity = await cityRepository.GetPointOfInterestAsync(cityId, pointInterestId);

            if (pointOfInterestEntity is null) return NotFound();

            PointOfInterestUpdateDto poiToPatch = mapper.Map<PointOfInterestUpdateDto>(pointOfInterestEntity);

            patchDocument.ApplyTo(poiToPatch, ModelState);

            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (!TryValidateModel(poiToPatch)) return BadRequest(ModelState);

            mapper.Map(poiToPatch, pointOfInterestEntity);
            await cityRepository.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{pointInterestId}")]
        public async Task<IActionResult> DeletePointOfInterestAsync(int cityId, int pointInterestId)
        {
            var pointOfInterestsForCity = await cityRepository.GetPointOfInterestAsync(cityId, pointInterestId);

            if (pointOfInterestsForCity is null) return NotFound();

            cityRepository.DeletePointOfInterest(pointOfInterestsForCity);
            await cityRepository.SaveChangesAsync();
            string message = $"PointOfInterests with {pointInterestId} has been deleted";
            _logger.LogInformation(message);
            _mailService.SenMail(message);

            return NoContent();
        }
    }
}
