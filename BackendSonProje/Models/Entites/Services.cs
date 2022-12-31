namespace BackendSonProje.Models.Entites
{
    public class Services
    {
        public int? ServicesId { get; set; }
        public string? ServiceName { get; set; }
        public ICollection<Reservation>? Reservation { get; set; }

    }
}
