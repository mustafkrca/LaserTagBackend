namespace BackendSonProje.Models.ViewModels
{
    public class UserRegisterVM
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }

        public string? Email { get; set; }
        public string? Password { get; set; }
        public long? PhoneNumber { get; set; }
    }
}
