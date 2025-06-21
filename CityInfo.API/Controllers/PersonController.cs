using CityInfo.API.Models;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PersonController : ControllerBase
    {
        private readonly IValidator<Person> _personValidator;
        public PersonController(IValidator<Person> personValidator)
        {
            _personValidator = personValidator;
        }
        [HttpPost]
        public IActionResult CreatePerson([FromBody] Person person)
        {
            var result = _personValidator.Validate(person);
            if (!result.IsValid)
            {
                return BadRequest(result.Errors);
            }
            return Ok("Person is valid!");
        }
    }
}
