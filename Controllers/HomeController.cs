using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using MvcApp.Models;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Authorization;
using MvcApp.Services;


namespace MvcApp.Controllers;

public class HomeController : Controller
{
    SqlConnection conn;
    SqlCommand cmd;
    SqlDataReader dr;
    private readonly string connectionString;
    private readonly NewServices _newsServices;

    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger, NewServices newServices)
    {
        _logger = logger;
        connectionString = "Server=localhost;Database=IronAndSteel;Trusted_Connection=True;TrustServerCertificate=True";
        _newsServices = newServices;
    }

    public IActionResult Index()
    {
        List<Iron> irons = new List<Iron>();
         try{
            conn = new SqlConnection(connectionString);
            cmd = new SqlCommand($"Select * from Iron", conn);
            conn.Open();  
            dr = cmd.ExecuteReader();
            while (dr != null && dr.Read())
            {
                irons.Add(new Iron
                {
                    Id = (int)dr["Id"],
                    IronName = dr["IronName"].ToString(),
                    IronDescription = dr["IronDescription"].ToString(),
                    Price = (decimal)dr["Price"],
                    Brand = dr["Brand"].ToString(),
                    ImageData = dr["ImageData"] != DBNull.Value ? dr["ImageData"].ToString() : null
                });
            }   
        }
        finally
        {
            conn.Close();
        }
        
        return View(irons);
    }
    
    [HttpGet]
    public IActionResult Registration()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Registration(Registeration model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            conn = new SqlConnection(connectionString);
            cmd = new SqlCommand($"insert into Users (Name, Email, Password) Values (@Name, @Email, @Password)", conn);
            cmd.Parameters.AddWithValue("@Name", model.Name);
            cmd.Parameters.AddWithValue("@Email", model.Email);
            cmd.Parameters.AddWithValue("@Password", model.Password);
            conn.Open();
            int i = cmd.ExecuteNonQuery();
        }catch(SqlException ex)
        {
            if(ex.Number == 2627 || ex.Number == 2602)
            {
                ModelState.AddModelError("Email", "Email Already Exist");
                return View(model);
            }
        }
        finally
        {
            conn.Close();
        }

        return RedirectToAction("Login");
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(Login model)
    { 
        try{
            conn = new SqlConnection(connectionString);
            cmd = new SqlCommand($"select Name, Email from Users where Email = '{model.Email}' and Password = '{model.Password}'", conn);
            conn.Open();  
            dr = cmd.ExecuteReader();
            if(!dr.Read()){
                return View(model);
            }
            string name = dr["Name"].ToString()?? "";
            string email = dr["Email"].ToString() ?? "";

            string role = "User";

            if(dr["Email"].ToString() == "prabal@gmail.com")
            {
                role = "Admin";
            }
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, name),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, role)  
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
            return RedirectToAction("Index");
                
        }
        finally
        {
            conn.Close();
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
        List<Blog> blogs = new List<Blog>();
        try{
            conn = new SqlConnection(connectionString);
            cmd = new SqlCommand($"Select * from Blogs order by createdAt desc", conn);
            conn.Open();  
            dr = cmd.ExecuteReader();
            while (dr != null && dr.Read())
            {
                blogs.Add(new Blog
                {
                    Id = (int)dr["Id"],
                    Title = dr["Title"].ToString(),
                    Content = dr["Content"].ToString(),
                    Email = dr["Email"].ToString(),
                    CreatedAt = Convert.ToDateTime(dr["CreatedAt"])
                });
            }   
        }
        finally
        {
            conn.Close();
        }
        
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
        try{
            conn = new SqlConnection(connectionString);
            cmd = new SqlCommand($"insert into Blogs (Title, Content, Email) values ('{model.Title}', '{model.Content}', '{email}')", conn);
            conn.Open();  
            cmd.ExecuteNonQuery();
        }
        finally
        {
            conn.Close();
        }
        return RedirectToAction("Blog");
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
       Iron iron = null;
        try
        {
            conn = new SqlConnection(connectionString);
            cmd = new SqlCommand($"select * from Iron where Id = @Id", conn);
            cmd.Parameters.AddWithValue("@Id", Id);
            conn.Open();
            dr = cmd.ExecuteReader();
            if (dr != null && dr.Read())
            {
                iron = new Iron(){
                
                Id = (int)dr["Id"],
                IronName = dr["IronName"].ToString(),
                IronDescription = dr["IronDescription"].ToString(),
                Price = (decimal)dr["Price"],
                Brand = dr["Brand"].ToString(),
                ImageData = dr["ImageData"] != DBNull.Value ? dr["ImageData"].ToString() : null,
                };
            } 
        }
        finally
        {
            conn.Close();
        }

        return View(iron);
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