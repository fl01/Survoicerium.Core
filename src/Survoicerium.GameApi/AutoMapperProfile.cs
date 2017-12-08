using AutoMapper;
using Survoicerium.Core;
using Survoicerium.GameApi.ApiModels.ResponseModels;

namespace Survoicerium.GameApi
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<IApiUser, User>();
            CreateMap<IDiscordAccount, DiscordUser>();
        }
    }
}
