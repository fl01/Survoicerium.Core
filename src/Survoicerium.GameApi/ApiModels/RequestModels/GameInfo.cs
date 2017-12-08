using System.ComponentModel.DataAnnotations;

namespace Survoicerium.GameApi.ApiModels.RequestModels
{
    public class GameInfo
    {
        [Required]
        public string Hash { get; set; }
    }
}
