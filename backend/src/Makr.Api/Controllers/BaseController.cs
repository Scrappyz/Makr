using Microsoft.AspNetCore.Mvc;

namespace Makr.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public abstract class BaseController : ControllerBase
    {
    }
}
