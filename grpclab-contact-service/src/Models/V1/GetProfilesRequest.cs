namespace GRPCLab.ContactService.Models.V1
{
    public class GetProfilesRequest : BaseGetProfilesRequest
    {
        public IEnumerable<int>? Ids { get; set; }
    }
}
