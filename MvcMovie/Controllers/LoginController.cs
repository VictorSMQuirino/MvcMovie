using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcMovie.Data;
using MvcMovie.Data.Security;
using MvcMovie.Data.Security.Interface;
using MvcMovie.Models;

namespace MvcMovie.Controllers;
public class LoginController : Controller
{
   private readonly MvcMovieContext _context;
   private readonly IUtils _utils;

    public LoginController(MvcMovieContext context, IUtils utils)
    {
        _context = context;
        _utils = utils;
    }

    // GET: Login/Index
    public IActionResult Index()
   {
      return View();
   }

   // POST: Login/Authenticate
   // To protect from overposting attacks, enable the specific properties you want to bind to.
   // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
   [HttpPost]
   [ValidateAntiForgeryToken]
   public async Task<IActionResult> Login([Bind("Email,Password")] Login user)
   {
      if (ModelState.IsValid)
      {
         user.Password = _utils.HashPassword(user.Password ?? "");
         var userInDb = await _context.User.FirstOrDefaultAsync(u => u.Email == user.Email && u.Password == user.Password);
        //  if (userInDb.Email == user.Email && userInDb.Password == user.Password)
        if(userInDb != default)
        {
            var role = userInDb.Email.Contains("_") ? "Admin" : "User";
            var token = _utils.GenerateJwtToken(userInDb.Email, role);
            Response.Cookies.Append("token","Bearer " + token, new CookieOptions 
            { 
                HttpOnly = true,
                Expires = DateTime.Now.AddMinutes(30)
            });
         return RedirectToAction("Index", "Home");
        }
      }
      return RedirectToAction("Login");
   }
}
