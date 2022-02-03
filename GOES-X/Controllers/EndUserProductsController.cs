﻿using GOES_I;
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
    }
}
