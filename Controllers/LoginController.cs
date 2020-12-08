using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TruYumClientApplication.Models;

namespace TruYumClientApplication.Controllers
{
    public class LoginController : Controller
    {
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(LoginController));
        // GET: LoginController
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(UserDetails user)
        {
            _log4net.Info("User Login");
            UserDetails Item = new UserDetails();
            using (var httpClient = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync("http://20.62.142.99/api/Auth/User", content);
                string apiResponse = await response.Content.ReadAsStringAsync();
                Item = JsonConvert.DeserializeObject<UserDetails>(apiResponse);
                StringContent content1 = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
                using (var response1 = await httpClient.PostAsync("http://20.62.142.99/api/Auth/Login", content1))
                {
                    if (!response1.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Login");
                    }

                    string apiResponse1 = await response1.Content.ReadAsStringAsync();

                    string stringJWT = response1.Content.ReadAsStringAsync().Result;

                    JWT jwt = JsonConvert.DeserializeObject<JWT>(stringJWT);

                    HttpContext.Session.SetString("token", jwt.Token);
                    HttpContext.Session.SetString("user", JsonConvert.SerializeObject(Item));
                    HttpContext.Session.SetInt32("UserId", Item.UserId);
                    HttpContext.Session.SetString("UserName", Item.UserName);
                    ViewBag.Message = "User logged in successfully!";

                    return RedirectToAction("Index", "TruYum");
                }
            }
        }

        public ActionResult Logout()
        {
            _log4net.Info("User Log Out");
            HttpContext.Session.Remove("token");
            // HttpContext.Session.SetString("user", null);

            return View("Login");
        }
    }
}
