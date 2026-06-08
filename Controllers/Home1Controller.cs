using ArvindamIrons.Models;
using ArvindamIrons.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Diagnostics;
using System.Security.Claims;

namespace ArvindamIrons.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DAL _dal;
        private readonly NewServices _newsServices;

        public HomeController(ILogger<HomeController> logger, NewServices newServices, DAL dal)
        {
            _logger = logger;
            _newsServices = newServices;
            _dal = dal;
        }

        public IActionResult Index()
        {
            List<Iron> irons = _dal.GetIronList();
            return View(irons);
        }

        [HttpGet]
        public IActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Registration(Registration model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                bool status = _dal.RegisterUser(model);

                if (status)
                {
                    return RedirectToAction("Login");
                }

                ModelState.AddModelError("", "Registration failed. Please try again.");
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627 || ex.Number == 2602)
                {
                    ModelState.AddModelError("Email", "Email Already Exist");
                    return View(model);
                }
                ModelState.AddModelError("", "A database error occurred: " + ex.Message);
            }


            return View(model);
        }


        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(Login model)
        {

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {

                Dictionary<string, string> UserCread = _dal.LoginUser(model);

                if (UserCread == null || UserCread.Count == 0)
                {
                    ModelState.AddModelError("", "Invalid email or password.");
                    return View(model);
                }


                string role = "User";

                if (UserCread["Name"].ToString() == "prabal@gmail.com")
                {
                    role = "Admin";
                }
                var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, UserCread["Name"]),
                new Claim(ClaimTypes.Email, UserCread["Email"]),
                new Claim(ClaimTypes.Role, role)
            };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                return RedirectToAction("Index");

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Un Unexpected Login error occured: " + ex.Message);
                return View(model);
            }
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        public IActionResult RND()
        {
            return View();
        }


        [Authorize]
        public IActionResult Blog()
        {

            List<Blog> blogs = _dal.GetAllComments();
            return View(blogs);
        }

        [HttpGet]
        [Authorize]
        public IActionResult AddBlog()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public IActionResult AddBlog(Blog model)
        {
            string email = User.FindFirst(ClaimTypes.Email)?.Value ?? "";

            bool status = _dal.AddNewBlog(model, email);
            if (status)
            {
                return RedirectToAction("Blog");
            }
            ModelState.AddModelError("", "Data is not Add in DB");
            return View();
        }

        [Authorize]
        public async Task<IActionResult> News()
        {
            var result = await _newsServices.GetNews("Iron and Steel");
            return View(result);
        }

        public IActionResult AboutUs()
        {
            return View();
        }

        public IActionResult View_Details(int Id)
        {
            Iron ironDetails = null;
            try
            {
                ironDetails = _dal.GetIronDetails(Id);
                return View(ironDetails);
            }
            catch (SqlException ex)
            {
                ModelState.AddModelError("", "Some Database Exception");
                return RedirectToAction("Index");
            }
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
