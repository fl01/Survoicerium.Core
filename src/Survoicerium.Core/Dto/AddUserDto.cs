namespace Survoicerium.Core.Dto
{
    public class AddUserDto
    {
        public string HardwareId { get; set; }

        public string Code { get; set; }

        public ulong DiscordUserId { get; set; }
    }
}
