using GRPCLab.ContactService.Models;

namespace GRPCLab.ContactService.Servives
{
    public interface IProfileService<T>
    {
        Task<BaseGetProfilesResult> GetManyAsync(BaseGetProfilesRequest request);
    }
}
