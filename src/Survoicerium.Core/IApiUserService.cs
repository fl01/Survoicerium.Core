using System.Threading.Tasks;
using Survoicerium.Core.Dto;

namespace Survoicerium.Core
{
    public interface IApiUserService
    {
        Task<IApiUser> GetOrAddAsync(AddUserDto addUserDto);

        Task<IApiUser> GetUserByHardwareIdAsync(string hardwareId);

        Task<IApiUser> GetUserByApiKeyAsync(string apiKey);

        Task<bool> IsValidApiKeyAsync(string apiKey);
    }
}
