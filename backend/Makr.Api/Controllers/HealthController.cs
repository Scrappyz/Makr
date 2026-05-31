using Microsoft.AspNetCore.Mvc;

namespace Makr.Api.Controllers
{
    [ApiController]
    public class HealthController : BaseController
    {
        [HttpGet]
        public String Get()
        {
            return "Healthy";
        }
    }
}
