using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ValuesController : ControllerBase
    {
        private readonly ILogger<ValuesController> _logger;

        public ValuesController(ILogger<ValuesController> logger)
        {
            _logger = logger;
        }

        private static readonly DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static DateTime FromUnixTime(long unixTime)
        {
            return epoch.AddSeconds(unixTime);
        }
        string Value(Claim claim)
        {
            if (claim.Type == "iat" || claim.Type == "exp")
            {
                return FromUnixTime(int.Parse(claim.Value)).ToString();
            }
            else
            {
                return claim.Value;
            }
        }

        [HttpGet]
        [Authorize(Policy = "GetScope")]
        public IEnumerable<string> Get()
        {
            return Request.HttpContext.User.Claims.Select(claim =>
                $"{claim.Type} = '{Value(claim)}' ({claim.ValueType})")
                .ToArray();
        }
    }
}
