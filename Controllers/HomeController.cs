using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using ActivityCenter.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace ActivityCenter.Controllers
{
    public class HomeController : Controller
    {
        private MyContext _context;
        public HomeController(MyContext context)
        {
            _context = context;
        }
        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost("register")]
        public IActionResult Register(User newUser)
        {
            if(ModelState.IsValid)
            {
                if(_context.Users.Any(u => u.Email == newUser.Email))
                {
                    ModelState.AddModelError("Email", "Email Already Exist");
                    return View("Index");
                }
                else
                {
                    PasswordHasher<User> Hasher = new PasswordHasher<User>();
                    newUser.Password = Hasher.HashPassword(newUser, newUser.Password);
                    _context.Users.Add(newUser);
                    _context.SaveChanges();
                    HttpContext.Session.SetInt32("userinSession", newUser.UserId);
                    return Redirect("/dashboard");
                }
            }
            else
            {
                return View("Index");
            }
        }

        [HttpPost("login")]
        public IActionResult Login(LoginUser userIn)
        {
            if(ModelState.IsValid)
            {
                var userInDb = _context.Users.FirstOrDefault(u => u.Email == userIn.LoginEmail);
                if(userInDb == null){
                    ModelState.AddModelError("LoginEmail","Invalid Email Address");
                    return View("Index");
                }
                else
                {
                    var hash = new PasswordHasher<LoginUser>();
                    var result = hash.VerifyHashedPassword(userIn, userInDb.Password, userIn.LoginPassword);
                    if(result == 0)
                    {
                        ModelState.AddModelError("LoginPassword","Invalid Password");
                        return View("Index");
                    }
                    else
                    {
                        HttpContext.Session.SetInt32("userinSession", userInDb.UserId);
                        return Redirect("/dashboard");
                    }
                }
            }
            else
            {
                return View("Index");
            }
        }

        [HttpGet("dashboard")]
        public IActionResult Dashbaord()
        {
            int? ID = HttpContext.Session.GetInt32("userinSession");
            User userInDB = _context.Users.FirstOrDefault(u=>u.UserId==ID);
            List<Activitys> allActivities = _context.Activities.Include(r=>r.Creator)
                                                   .Include(r=>r.Guests)
                                                   .ThenInclude(a=>a.Activity)
                                                   .OrderBy(a => a.Date)
                                                   .ThenBy(a => a.Time)
                                                   .ToList();
            if(userInDB == null)
            {
                return RedirectToAction("Logout");
            }
            else
            {
                ViewBag.User = userInDB;
                return View("Dashboard",allActivities);
            }
        }
        [HttpGet("Activitys")]
        public IActionResult Activitys()
        {
            int? ID = HttpContext.Session.GetInt32("userinSession");
            ViewBag.getUser = _context.Users.FirstOrDefault(u=>u.UserId==ID);
            return View("Activitys");
        }
        [HttpPost("Activitys")]
        public IActionResult Create(Activitys newActivity)
        {
            int? ID = HttpContext.Session.GetInt32("userinSession");
            ViewBag.getUser = _context.Users.FirstOrDefault(u=>u.UserId==ID);
            if(ModelState.IsValid)
            {
                newActivity.UserId = (int)ID; 
                _context.Activities.Add(newActivity);
                _context.SaveChanges();
                return Redirect("/dashboard");
            }
            else
            {
                
                return View("Activitys");
            }
        }
        [HttpGet("/{actiID}")]
        public IActionResult ActivityInfo(int actiID)
        {
            int? ID = HttpContext.Session.GetInt32("userinSession");
            ViewBag.getUser = _context.Users.FirstOrDefault(u=>u.UserId==ID);
            ViewBag.getOne = _context.Activities.FirstOrDefault(e => e.ActivityId == actiID);
            List<Association> Guests = _context.Associations.Where(a => a.Activity.ActivityId == actiID)
                                                            .Include(r => r.Guest) 
                                                            .ToList();
            return View("ActivityInfo", Guests);
        }

        [HttpGet("/attend/{actiID}/{userID}")]
        public IActionResult Attend(int actiID, int userID)
        {
            Association attend = new Association();
            attend.UserId = userID;
            attend.ActivityId = actiID;
            _context.Associations.Add(attend);
            _context.SaveChanges();
            return Redirect("/dashboard");
        }
        [HttpGet("/leave/{actiID}/{userID}")]
        public IActionResult Leave(int actiID, int userID)
        {
            Association leave = _context.Associations.FirstOrDefault(a=> a.UserId == userID && a.ActivityId == actiID);
            _context.Associations.Remove(leave);
            _context.SaveChanges();
            return Redirect("/dashboard");
        }

        [HttpGet("/cancel/{actiID}")]
        public IActionResult Cancel(int actiID)
        {
            Activitys cancel = _context.Activities.FirstOrDefault(r=>r.ActivityId == actiID);
            _context.Activities.Remove(cancel);
            _context.SaveChanges();
            return Redirect("/dashboard");
        }






        [HttpGet("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("userinSession");
            HttpContext.Session.Clear();
            return Redirect("/");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}