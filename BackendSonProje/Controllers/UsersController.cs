using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BackendSonProje.Models;
using BackendSonProje.Models.Entites;
using BackendSonProje.Models.ViewModels;
using NuGet.Versioning;

namespace BackendSonProje.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserDbContext _context;

        public UsersController(UserDbContext context)
        {
            _context = context;
        }


        #region DEFAULT CRUD

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int? id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // PUT: api/Users/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int? id, User user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Users
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.Id }, user);
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int? id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        #endregion

        [Route("register")]
        [HttpPost]
        public async Task<ActionResult> RegisterUser(UserRegisterVM user)
        {
            // ViewModel dönüşümü Manuel Metod
            User addUser = new()
            {
                Email= user.Email,
                Name= user.Name,
                Surname= user.Surname,
                Password= user.Password,
                PhoneNumber= user.PhoneNumber
            };
            // Kayıtlı Email var mı kontrolü
            User? kontUser = await _context.Users.FirstOrDefaultAsync(x => x.Email == user.Email);

            // Email kayıtlıysa hata döndür değilse kaydet
            if (kontUser == null)
            {
                _context.Users.Add(addUser);
                await _context.SaveChangesAsync();
                return Ok("Kayıt Yapıldı");

            }
            else
            {
                throw new Exception("E-mail Zaten Kayıtlı. ");

            }

        }


        [Route("loginuser")]
        [HttpPost()]
        public async Task<ActionResult> LoginUser(UserLoginVM user)
        {
            // Şifre - Email Uyuşuyor mu
            User? kontUser = await _context.Users.FirstOrDefaultAsync(x => x.Email == user.Email && x.Password == user.Password);
            if (kontUser == null)
            {
                throw new Exception("Şifre veya Email Hatalı");
            }
            else
            {
                return Ok("Giriş Yapıldı");
            }
        }
        [Route("reservation")]
        [HttpPost]
        public void reservation(ReservationDataVM res)
        {
            string serviceName = "";

            // Telefon numarası üzerinden User seçimi
            var userId = _context.Users.FirstOrDefault(u => u.PhoneNumber == res.PhoneNumber);

            // Yönetilebilirlik arttırmak amacıyla sevice nin başlangıç ve bitiş id sini otomatik bulma
            List<Services> services = _context.Services.ToList();
            switch (res.Service)
            {
                case 1: serviceName = "PC"; break;
                case 2: serviceName = "PS"; break;
                case 3: serviceName = "LT" ; break;
            }

            var service = services.Where(u => u.ServiceName.Contains(serviceName)).OrderBy(p => p.ServicesId);
            var serviceFirst = service.First().ServicesId;
            var serviceLast = service.Last().ServicesId;


            // PC-PS-LT eğer doluysa sonraki numaralı servise e geçen algoritma
            while (true)
            {
                
                var serviceId = _context.Services.FirstOrDefault(u => u.ServicesId == serviceFirst);

                if (null == _context.Reservations.FirstOrDefault(u => u.Service == serviceId && u.Clock == res.Clock &&  u.Day == res.Day))
                {
                    Reservation reservation = new()
                    {
                        Day = res.Day,
                        Service = serviceId,
                        User = userId,
                        Clock = res.Clock
                    };
                    _context.Add(reservation);
                    _context.SaveChanges();
                    break;
                }
                else
                {
                    serviceFirst += 1;
                    if (serviceFirst > serviceLast)
                    {
                        break;
                    }
                    continue;
                }
            }
            
        }
        [Route("getreservations")]
        [HttpGet]
        // Dolu olan reservation ları döndüren fonksiyon
        public List<ResList> GetReservations()
        {
            int day = 7;
            int clock = 18;
            int service = 0;
            int serviceSon = 0;
            int serviceCount = 0;
            List<ResList> resList = new();

            // Yönetilebilirlik arttırmak amacıyla sevice nin başlangıç ve bitiş id sini otomatik bulma
            List<Services> services = _context.Services.ToList();


            var servicesPc = services.Where(u => u.ServiceName.Contains("PC")).OrderBy(p => p.ServicesId);
            var serviceFirstPc = servicesPc.First().ServicesId;
            var serviceLastPc = servicesPc.Last().ServicesId;
            var serviceCountPc = servicesPc.Count();

            var servicesPs = services.Where(u => u.ServiceName.Contains("PS")).OrderBy(p => p.ServicesId);
            var serviceFirstPs = servicesPs.First().ServicesId;
            var serviceLastPs = servicesPs.Last().ServicesId;
            var serviceCountPs = servicesPs.Count();

            var servicesLt = services.Where(u => u.ServiceName.Contains("LT")).OrderBy(p => p.ServicesId);
            var serviceFirstLt = servicesLt.First().ServicesId;
            var serviceLastLt = servicesLt.Last().ServicesId;
            var serviceCountLt = servicesLt.Count();

            // rezervasyon listesi
            List<Reservation> res = _context.Reservations.Include(u => u.Service).ToList();

            for (int s = 1; s <= 3; s++)
            {
                for (int i = 1; i < day; i++)
                {
                    for (int j = 10; j < clock; j++)
                    {
                        
                        switch (s)
                        {
                            case 1:  service = Convert.ToInt32(serviceFirstPc);  serviceSon = Convert.ToInt32(serviceLastPc); serviceCount = Convert.ToInt32(serviceCountPc); break;
                            case 2:  service = Convert.ToInt32(serviceFirstPs); serviceSon = Convert.ToInt32(serviceLastPs); serviceCount = Convert.ToInt32(serviceCountPs); break;
                            case 3:  service = Convert.ToInt32(serviceFirstLt); serviceSon = Convert.ToInt32(serviceLastLt); serviceCount = Convert.ToInt32(serviceCountLt); break;
                        }
                        var count = res.Where(c => c.Service.ServicesId >= service && c.Service.ServicesId <= serviceSon).Count(u => u.Day == i && u.Clock == j);
                        if (count == serviceCount)
                        {
                            ResList res1 = new ResList
                            {
                                service = s,
                                clock= j,
                                day= i,
                            };
                            resList.Add(res1);

                        }
                    }
                }
            }
            
            return resList;
            

        }
        [Route("resclear")]
        [HttpGet]
        public void resClear()
        {         
                List<Reservation> res = _context.Reservations.ToList();
                _context.Reservations.RemoveRange(res);
                _context.SaveChanges();       
        }
        private bool UserExists(int? id)
        {
            return _context.Users.Any(e => e.Id == id);
        }

    }
}
