using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Harpocrates.Api.Host.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class ApiControllerBase : ControllerBase
    {
    }
}
