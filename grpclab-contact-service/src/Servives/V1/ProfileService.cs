using GRPCLab.ContactService.Models;
using GRPCLab.ContactService.Models.V1;
using GRPCLab.ContactService.Servives;
using GrpcProfileClient;
using Microsoft.Extensions.Options;

namespace GRPCLab.ContactService.Servives.V1
{
    public class ProfileService : BaseService<ProfileService>, IProfileService<ProfileService>
    {
        private readonly Profile.ProfileClient _client;

        public ProfileService(Profile.ProfileClient client,
                                ILoggerFactory loggerFactory,
                                IOptionsSnapshot<AppSettings> settings) : base(loggerFactory, settings)
        {
            _client = client;
        }

        public async Task<BaseGetProfilesResult> GetManyAsync(BaseGetProfilesRequest request)
        {
            var _request = request as GetProfilesRequest;
            if(_request == null
                || (_request?.Ids?.Count() ?? 0) == 0)
            {
                throw new ArgumentException("Param value is null", nameof(GetProfilesRequest));
            }

            var _rq = new ProfilesRequest();
            IEnumerable<int>? ids = _request!.Ids;
            _rq.Ids.AddRange(ids);
            // Forward the call on to the greeter service
            var call = await _client.GetProfilesAsync(_rq);
            return new GetProfilesResult
            {
                Result = call
            };
        }
    }
}
