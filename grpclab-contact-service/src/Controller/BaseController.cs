using Microsoft.AspNetCore.Mvc;

namespace GRPCLab.ContactService.Controller
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    public class BaseController<T> : ControllerBase
    {
        protected readonly ILogger<T> _logger;
        public BaseController(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<T>();
        }
    }
}
