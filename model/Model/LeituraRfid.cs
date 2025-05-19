using System.ComponentModel.DataAnnotations;

namespace library.Model
{
    public class LeituraRfid
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid RfidId { get; set; }

        [Required]
        public Guid SensorId { get; set; }

        [Required]
        public DateTime Timestamp { get; set; }

    }
}
