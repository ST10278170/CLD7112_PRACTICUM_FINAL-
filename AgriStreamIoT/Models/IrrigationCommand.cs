namespace AgriStreamIoT.Models
{
    public class IrrigationCommand
    {
        public Guid CommandId { get; set; } =
            Guid.NewGuid();

        public int DeviceId { get; set; }

        public string DeviceName { get; set; } =
            string.Empty;

        public string FieldLocation { get; set; } =
            string.Empty;

        public string Command { get; set; } =
            "StartIrrigation";

        public string RequestedBy { get; set; } =
            "AgriStream MVC";

        public DateTimeOffset RequestedAt { get; set; } =
            DateTimeOffset.UtcNow;
    }
}