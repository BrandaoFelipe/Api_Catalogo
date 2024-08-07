﻿using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APICatalogo.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("2.0", Deprecated = false)]
    [ApiConventionType(typeof(DefaultApiConventions))]
    public class TesteV2Controller : ControllerBase
    {
        [HttpGet]
        public string GetVersion()
        {
            return "TesteV1 - GET - Api version 2.0";
        }
    }
}
