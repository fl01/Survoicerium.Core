using System.Threading.Tasks;
using Survoicerium.Core.Dto;

namespace Survoicerium.Core
{
    public interface IApiUserService
    {
        Task<IApiUser> AddAsync(AddUserDto addUserDto);

        IApiUser GetUserByHardwareId(string hardwareId);

        bool IsValidApiKey(string apiKey);
    }
}
