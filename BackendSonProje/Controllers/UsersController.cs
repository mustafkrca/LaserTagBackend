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
        public async Task<ActionResult> RegisterUser(User user)
        {
            User? kontUser = await _context.Users.FirstOrDefaultAsync(x => x.Email == user.Email);
            if (kontUser == null)
            {
                _context.Users.Add(user);
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

            var userId = _context.Users.FirstOrDefault(u => u.PhoneNumber == res.PhoneNumber);

            // PC-PS-LT eğer doluysa sonraki numaralı servise e geçen algoritma
            while (true)
            {
                int sonService =0;
                switch (res.Service)
                {
                    case 1: sonService = 6; break;
                    case 2: res.Service = 7; sonService = 12; break;
                    case 3: res.Service = 12; sonService = 13; break;
                }
                var serviceId = _context.Services.FirstOrDefault(u => u.ServicesId == res.Service);

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
                    res.Service += 1;
                    if (res.Service == sonService)
                    {
                        break;
                    }
                    continue;
                }
            }
            
        }
        [Route("getreservations")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Reservation>>> GetReservations()
        {
            return await _context.Reservations.ToListAsync();
        }

        private bool UserExists(int? id)
        {
            return _context.Users.Any(e => e.Id == id);
        }

    }
}
