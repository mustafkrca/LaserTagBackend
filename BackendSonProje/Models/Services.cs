namespace BackendSonProje.Models
{
    public class Services
    {
        public int? ServicesId { get; set; }  
        public string? ServiceName { get; set; }
        public ICollection<Reservation>? Reservation { get; set; }

    }
}
