namespace AgriStreamFunctions.Models
{
    public class IrrigationCommandMessage
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
            "Azure Function";

        public DateTimeOffset RequestedAt { get; set; } =
            DateTimeOffset.UtcNow;
    }
}