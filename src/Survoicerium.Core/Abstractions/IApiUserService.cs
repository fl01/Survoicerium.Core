using System.Threading.Tasks;
using Survoicerium.Core.Dto;

namespace Survoicerium.Core.Abstractions
{
    public interface IApiUserService
    {
        Task<ApiUser> GetOrAddAsync(AddUserDto addUserDto);

        Task<ApiUser> GetUserByApiKeyAsync(string apiKey);
    }
}
