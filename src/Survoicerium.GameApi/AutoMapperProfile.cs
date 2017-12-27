using System;
using AutoMapper;
using Survoicerium.Core;
using Survoicerium.GameApi.ApiModels.ResponseModels;

namespace Survoicerium.GameApi
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<ApiUser, User>();
            CreateMap<DiscordAccount, DiscordUser>();
            CreateMap<Channel, Survoicerium.Infrastructure.Mongo.Models.Channel>()
                .ForMember<Guid>(f => f._id, f => f.ResolveUsing(x => x.Id));
            CreateMap<Survoicerium.Infrastructure.Mongo.Models.Channel, Channel>()
                 .ForMember<Guid>(f => f.Id, f => f.ResolveUsing(x => x._id));
        }
    }
}
