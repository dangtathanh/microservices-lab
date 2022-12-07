using Google.Protobuf.Collections;
using Grpc.Core;
using GrpcProfile;

namespace GRPCLab.ProfileService.Services
{
    public class ProfileService : Profile.ProfileBase
    {

        public override Task<ProfilesResponse> GetProfiles(ProfilesRequest request, ServerCallContext context)
        {
            var result = new ProfilesResponse();
            result.Results.AddRange(new RepeatedField<ProfilesResponse.Types.Result> {
                new ProfilesResponse.Types.Result
                {
                    Id = 1,
                    Name = "Alex"
                },
                new ProfilesResponse.Types.Result
                {
                    Id = 2,
                    Name = "Jan"
                }
            });

            return Task.FromResult(result);
        }
    }
}
