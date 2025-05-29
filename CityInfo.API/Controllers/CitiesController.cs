using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CitiesController : ControllerBase
    {

        //[HttpGet(Name ="mycities")] Not working
        //[HttpGet("mycities")]
        [HttpGet]
        public JsonResult GetCities()
        {
            return new JsonResult(

                new List<Object>
                {
                    new {id=1, Name="Hyderabad"},
                    new {id=2, Name="Guntur"},
                }
                );
        }

    }
}
