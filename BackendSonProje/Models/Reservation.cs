using System.ComponentModel.DataAnnotations.Schema;

namespace BackendSonProje.Models
{
    public class Reservation
    {
        public int ReservationId { get; set; }
        public User? User { get; set; }
        public Services? Service { get; set; }
        public int Day { get; set; }
        public int Clock { get; set; }

    }
}
