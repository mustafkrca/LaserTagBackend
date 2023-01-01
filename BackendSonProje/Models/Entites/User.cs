using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace BackendSonProje.Models.Entites
{
    public class User
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public long? PhoneNumber { get; set; }
        public ICollection<Reservation>? Reservations { get; set; }

    }
}
