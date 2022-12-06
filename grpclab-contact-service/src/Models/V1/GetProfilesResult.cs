using GrpcProfileClient;

namespace GRPCLab.ContactService.Models.V1
{
    public class GetProfilesResult : BaseGetProfilesResult
    {
        public ProfilesResponse? Result { set; get; } 
    }
}
