using Survoicerium.Core.Dto;
using System.Threading.Tasks;

namespace Survoicerium.Core.Abstractions
{
    public interface IGameService
    {
       Task JoinGameAsync(GameInfoDto game);
    }
}
