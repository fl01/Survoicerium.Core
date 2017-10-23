using System;
using Survoicerium.Core;
using Survoicerium.Core.Dto;

namespace Survoicerium.Infrastructure.Mongo
{
    public class ApiUserService : IApiUserService
    {
        public IApiUser Add(AddUserDto addUserDto)
        {
            throw new NotImplementedException();
        }

        public IApiUser GetUserByHardwareId(string hardwareId)
        {
            throw new NotImplementedException();
        }

        public bool IsValidApiKey(string apiKey)
        {
            return true;
        }
    }
}
