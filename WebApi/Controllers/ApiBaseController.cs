using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace WebApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ApiBaseController : ControllerBase
    {
        protected ILogger logger { get; }

        public ApiBaseController(ILogger<ApiBaseController> logger) : base()
        {
            this.logger = logger;
        }
    }
}
