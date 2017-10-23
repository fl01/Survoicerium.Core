using Survoicerium.Core.Dto;

namespace Survoicerium.Core
{
    public interface IApiUserService
    {
        IApiUser Add(AddUserDto addUserDto);
        
        IApiUser GetUserByHardwareId(string hardwareId);

        bool IsValidApiKey(string apiKey);
    }
}
