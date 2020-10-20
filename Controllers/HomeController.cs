using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WeddingPlanner.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;

using Microsoft.EntityFrameworkCore;

namespace WeddingPlanner.Controllers
{
    public class HomeController : Controller
    {
        private MyContext dbContext;
        public HomeController(MyContext context)
        {
            dbContext = context;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("add")]
        public IActionResult RegisterUser(User fromForm)
        {
            
            if(ModelState.IsValid)
            {
                if(dbContext.Users.Any(u => u.Email == fromForm.Email))
                {
                    ModelState.AddModelError("Email", "This Email is already in use!");
                    return RedirectToAction("Index");
                }
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                fromForm.Password = Hasher.HashPassword(fromForm, fromForm.Password);
                dbContext.Add(fromForm);
                dbContext.SaveChanges();
                //sets this new user into session
                var userInDb = dbContext.Users.FirstOrDefault(u => u.Email == fromForm.Email);
                HttpContext.Session.SetInt32("UserId", userInDb.UserId);

                return RedirectToAction("Dashboard");
            }
            else
            {
                return View("Index");
            }

        }
        
        [HttpPost("login")]
        public IActionResult LoginUser(LoginUser fromForm)
        {
            if(ModelState.IsValid)
            {
                var userInDb = dbContext.Users.FirstOrDefault(u => u.Email == fromForm.LoginEmail);

                if(userInDb == null)
                {
                    ModelState.AddModelError("Email", "Invalid Email/Password");
                    return View("Index");
                }
                var hasher = new PasswordHasher<LoginUser>();
                var result = hasher.VerifyHashedPassword(fromForm, userInDb.Password, fromForm.LoginPassword);
                if (result == 0)
                {
                    ModelState.AddModelError("Password", "Invalid Email/Password");
                    return View("Index");                    
                }

                //Sets this user into Session
                HttpContext.Session.SetInt32("UserId", userInDb.UserId);
                return RedirectToAction ("Dashboard");
            }
            else
            {
                
                return View("Index");
            }
        }

        [HttpGet("dashboard")]
        public IActionResult Dashboard()
        {
            int? user = HttpContext.Session.GetInt32("UserId");
            if(user == null) 
            { 
                return RedirectToAction("Index");
            }

            Wrapper Wrapper = new Wrapper();
            // Wrapper.WeddingList = WeddingList;

            Wrapper.WeddingList  = dbContext.Weddings
                    .Include(r => r.RSVPList)
                    .ThenInclude(r => r.User).ToList();  

            Wrapper.User = dbContext.Users
                .Include(u => u.RSVPList)
                .ThenInclude(u => u.Wedding)
                .FirstOrDefault(u => u.UserId == user);

            return View(Wrapper);
        }

        [HttpGet("logOut")]
        public IActionResult LogOutUser()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        [HttpGet("newWedding")]
        public IActionResult NewWeddingForm()
        {
            int? user = HttpContext.Session.GetInt32("UserId");
            if(user == null) 
            { 
                return RedirectToAction("Index");
            }

            ViewBag.UserId = user;

            return View();
        }

        [HttpPost("addWedding")]
        public IActionResult AddWedding(Wedding fromForm)
        {           

            if(ModelState.IsValid)
            {
                dbContext.Add(fromForm);
                dbContext.SaveChanges();
                
                Wedding Wedding  = dbContext.Weddings
                    .Include(r => r.RSVPList)
                    .ThenInclude(r => r.User)
                    .FirstOrDefault(w => w.WedderOne == fromForm.WedderOne && w.WedderTwo == fromForm.WedderTwo);

                Wrapper Wrapper = new Wrapper();
                Wrapper.Wedding = Wedding;
                
                // System.Console.WriteLine(WeddingId);
                ViewBag.Address = Wedding.WeddingAddress;

                return View("WeddingDetails", Wrapper);
            }
            else
            {
                return View("NewWeddingForm");
            }

        }

        [HttpGet("/{WeddingId}")]
        public IActionResult WeddingDetails(int WeddingId)
        {
            System.Console.WriteLine(WeddingId);
            int? user = HttpContext.Session.GetInt32("UserId");
            if(user == null) 
            { 

                return RedirectToAction("Index");
            }

            Wedding ThisWedding = dbContext.Weddings.Include(w => w.RSVPList).ThenInclude(w => w.User).FirstOrDefault(w => w.WeddingId == WeddingId); 
            Wrapper Wrapper = new Wrapper();
            Wrapper.Wedding = ThisWedding;

            ViewBag.Address = ThisWedding.WeddingAddress;
                
            return View(Wrapper);
        }

        [HttpPost("delete")]
        public IActionResult DeleteWedding(Wrapper fromForm)
        {
            Wedding ToDelete = dbContext.Weddings
                .FirstOrDefault(w => w.WeddingId == fromForm.Wedding.WeddingId);
            dbContext.Remove(ToDelete);
            dbContext.SaveChanges();
            return RedirectToAction("Dashboard");
        }

        [HttpPost("rsvpWedding")]
        public IActionResult RSVPWedding(Wrapper fromForm, int UserId)
        {
            RSVP rsvp = new RSVP();
            rsvp.WeddingId = fromForm.Wedding.WeddingId;
            rsvp.UserId = UserId;

            dbContext.Add(rsvp);
            dbContext.SaveChanges();
            return RedirectToAction("Dashboard");           

        }
        [HttpPost("UnrsvpWedding")]
        public IActionResult UnRSVPWedding(Wrapper fromForm, int UserId)
        {
            RSVP ToDelete = dbContext.RSVPs                
                .FirstOrDefault(w => w.WeddingId == fromForm.Wedding.WeddingId && w.UserId == UserId);
                
            dbContext.Remove(ToDelete);
            dbContext.SaveChanges();
            return RedirectToAction("Dashboard");

        }

    }
}
