using AutoMapper;
using Survoicerium.Frontend.Configuration;
using Survoicerium.InternalConfigurationApi.Contracts.Models;

namespace Survoicerium.Frontend
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<DiscordOAuthConfiguration, DiscordOAuth>();
        }
    }
}
