using System.ComponentModel.DataAnnotations;

namespace Survoicerium.Backend.ApiModels
{
    public class GameInfo
    {
        [Required]
        public string Hash { get; set; }

        [Required]
        public string HardwareId { get; set; }

        [Required]
        public string Token { get; set; }
    }
}
