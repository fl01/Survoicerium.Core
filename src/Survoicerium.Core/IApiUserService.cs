using System.Threading.Tasks;
using Survoicerium.Core.Dto;

namespace Survoicerium.Core
{
    public interface IApiUserService
    {
        Task<IApiUser> AddAsync(AddUserDto addUserDto);

        Task<IApiUser> GetUserByHardwareIdAsync(string hardwareId);

        Task<IApiUser> GetUserByApiKeyAsync(string apiKey);

        bool IsValidApiKey(string apiKey);
    }
}
