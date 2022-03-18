using GOES_I;
using GOES_I.Animator;
using Microsoft.AspNetCore.Mvc;

namespace GOES_X.Controllers
{

    [ApiController]
    public class EndUserProductsController : Controller
    {
        private IQueryService _QueryService;
        private IUserService _UserService;

        public EndUserProductsController(IQueryService queryService, IUserService userService)
        {
            _QueryService = queryService;
            _UserService = userService; 
        }

        [HttpGet]
        [Route("api/eup")]
        public IActionResult Get([FromQuery(Name = "eupName")] string eupName, [FromQuery(Name = "key")] string key, [FromQuery(Name = "timestamp")] long timestamp)
        {
            try
            {
                // TODO: Add timestamp support FromQuery
                // TODO: Add Exists check
                var product = _UserService.GetEndUserProduct(eupName, new DateTime(timestamp));

                // TODO: This should move to dedicated endpoints for different eups etc...
                if (!product.ContainsKey(key))
                    return NotFound("Key was not present in EUP");
                if (!System.IO.File.Exists((string)product[key]))
                    return NotFound("GOES-X did not cache this product");
                byte[] data = System.IO.File.ReadAllBytes((string)product[key]);

                return File(data, "image/png");
            }
            catch (Exception)
            {
                return Unauthorized("File is being built.");
            }
            
        }

        [HttpOptions]
        [Route("api/eup")]
        public IActionResult Options([FromQuery(Name = "eupName")] string eupName, [FromQuery(Name = "key")] string key, [FromQuery(Name = "timestamp")] long timestamp)
        {
            try
            {
                var product = _UserService.GetEndUserProduct(eupName, new DateTime(timestamp));

                if (!product.ContainsKey(key))
                    return NotFound("Key was not present in EUP");
                if (!System.IO.File.Exists((string)product[key]))
                    return NotFound("GOES-X did not cache this product");
                return Ok();
            }
            catch (Exception)
            {
                return NotFound("GOES-X did not cache this product");
            }
        }

        [HttpGet]
        [Route("api/animate")]
        public IActionResult GetAnimation(
            [FromQuery(Name = "productName")] string productName, 
            [FromQuery(Name = "productIndex")] string productIndex,
            [FromQuery(Name = "startTimestamp")] long startTimestamp,
            [FromQuery(Name = "endTimestamp")] long endTimestamp
        )
        {
            if (startTimestamp >= endTimestamp)
                return Problem("Invalid range");
            TimeSpan length = TimeSpan.FromTicks(endTimestamp - startTimestamp); 
            string animationPath = EupAnimator.AnimateEup(productName, productIndex, new DateTime(startTimestamp), length);
            byte[] data = System.IO.File.ReadAllBytes(animationPath);

            return File(data, "video/mp4");
        }

        /*[HttpGet]
        [Route("api/eup/availability")]
        public IActionResult GetAvailability([FromQuery(Name = "start")] long startTicks, [FromQuery(Name = "end")] long endTicks)
        {

        }*/
    }
}
