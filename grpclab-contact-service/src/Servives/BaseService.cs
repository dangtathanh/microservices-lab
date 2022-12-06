using Microsoft.Extensions.Options;

namespace GRPCLab.ContactService.Servives
{
    public class BaseService<T>
    {
        protected readonly AppSettings _setting;
        protected readonly ILogger<T> _logger;
        public BaseService(ILoggerFactory loggerFactory,
                            IOptionsSnapshot<AppSettings> settings)
        {
            _logger = loggerFactory.CreateLogger<T>();
            _setting = settings.Value;
        }
    }
}
