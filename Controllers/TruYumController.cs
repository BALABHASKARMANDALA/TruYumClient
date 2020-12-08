using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using TruYumClientApplication.Models;

namespace TruYumClientApplication.Controllers
{
    public class TruYumController : Controller
    {
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(TruYumController));

        // GET: TruYum
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("token") == null)
            {
                _log4net.Info("token not found");

                return RedirectToAction("Login");

            }
            else
            {
                _log4net.Info("MenuItemlist getting Displayed");

                List<MenuItem> ItemList = new List<MenuItem>();
                using (var client = new HttpClient())
                {
                    var contentType = new MediaTypeWithQualityHeaderValue("application/json");
                    client.DefaultRequestHeaders.Accept.Add(contentType);

                    client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("token"));

                    using (var response = await client.GetAsync("http://40.88.232.219/api/MenuItem"))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        ItemList = JsonConvert.DeserializeObject<List<MenuItem>>(apiResponse);
                    }
                }
                return View(ItemList);
            }
        }

        public async Task<IActionResult> PlaceOrder(int id)
        {
            _log4net.Info("Placing order in progess");
            if (HttpContext.Session.GetString("token") == null)
            {
                return RedirectToAction("Login", "Login");
            }
            else
            {
                MenuItem item = new MenuItem();
                Cart cartobj = new Cart();
                using (var client = new HttpClient())
                {
                    var contentType = new MediaTypeWithQualityHeaderValue("application/json");
                    client.DefaultRequestHeaders.Accept.Add(contentType);

                    client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("token"));
                    using (var response = await client.GetAsync("http://40.88.232.219/api/MenuItem/" + id))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        item = JsonConvert.DeserializeObject<MenuItem>(apiResponse);
                    }
                    cartobj.ItemId = item.ItemId;
                    cartobj.Quantity = 0;
                    cartobj.TotalPrice = 0;
                }
                return View(cartobj);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder(Cart cartobj)
        {
            _log4net.Info("Order placed");
            if (HttpContext.Session.GetString("token") == null)
            {
                return RedirectToAction("Login", "Login");
            }
            else
            {
                MenuItem _item = new MenuItem();
                using (var client = new HttpClient())
                {
                    var contentType = new MediaTypeWithQualityHeaderValue("application/json");
                    client.DefaultRequestHeaders.Accept.Add(contentType);

                    client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("token"));

                    using (var response = await client.GetAsync("http://40.88.232.219/api/MenuItem/" + cartobj.ItemId))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        _item = JsonConvert.DeserializeObject<MenuItem>(apiResponse);
                    }
                    cartobj.TotalPrice = cartobj.Quantity * _item.Price;
                    var content = new StringContent(JsonConvert.SerializeObject(cartobj), Encoding.UTF8, "application/json");

                    using (var response = await client.PostAsync("http://52.151.204.49/api/Cart/", content))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        cartobj = JsonConvert.DeserializeObject<Cart>(apiResponse);
                    }
                }
                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> GetOrderDetails(int id)
        {
            _log4net.Info("Getting order details");
            if (HttpContext.Session.GetString("token") == null)
            {
                return RedirectToAction("Login", "Login");
            }
            else
            {
                List<Cart> item = new List<Cart>();
                ViewBag.Username = HttpContext.Session.GetString("UserName");
                using (var client = new HttpClient())
                {
                    var contentType = new MediaTypeWithQualityHeaderValue("application/json");
                    client.DefaultRequestHeaders.Accept.Add(contentType);
                    if (HttpContext.Session.GetInt32("UserId") != null)
                    {
                        id = Convert.ToInt32(HttpContext.Session.GetInt32("UserId"));
                    }
                    client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("token"));

                    using (var response = await client.GetAsync("http://52.151.204.49/api/Cart/" + id))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        item = JsonConvert.DeserializeObject<List<Cart>>(apiResponse);
                    }
                }
                return View(item);
            }
        }
    }
}
