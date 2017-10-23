using System.ComponentModel.DataAnnotations;

namespace Survoicerium.GameApi.ApiModels
{
    public class GameInfo
    {
        [Required]
        public string Hash { get; set; }

        [Required]
        public string HardwareId { get; set; }

        [Required]
        public string ApiKey { get; set; }
    }
}
