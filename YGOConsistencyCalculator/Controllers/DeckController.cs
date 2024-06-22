using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Microsoft.Owin.Security;
using YGOConsistencyCalculator.Models;
using Microsoft.AspNet.Identity.Owin;
using System.Diagnostics;
namespace YGOConsistencyCalculator.Controllers
{
    public class DeckController : Controller
    {
        private static readonly HttpClient client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();

        static DeckController()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:44386/api/DeckData/");
        }

        // GET: Deck/New
        [Authorize]
        public ActionResult New()
        {
            GetApplicationCookie();
            return View();
        }

        // POST: Deck/Create
        [HttpPost]
        [Authorize]
        public ActionResult Create(Deck deck)
        {
            GetApplicationCookie();
            string url = "AddDeck";

            string jsonpayload = jss.Serialize(deck);

            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";

            HttpResponseMessage response = client.PostAsync(url, content).Result;

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("List");
            }
            else
            {
                return Error();
            }
        }

        // GET: Deck/List
        [Authorize]
        public ActionResult List()
        {
            GetApplicationCookie();
            string url = "ListDecks";
            HttpResponseMessage response = client.GetAsync(url).Result;
            Debug.WriteLine(response.Content.ReadAsStringAsync().Result.ToString());

            IEnumerable<DeckDto> decks = response.Content.ReadAsAsync<IEnumerable<DeckDto>>().Result;

            return View(decks);
        }

        //GET: Deck/Details/1
        [Authorize]
        public ActionResult Details(int id)
        {
            GetApplicationCookie();
            string url = "FindDeck/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            DeckDto selectedDeck = response.Content.ReadAsAsync<DeckDto>().Result;

            return View(selectedDeck);
        }

        //GET: Deck/MyDecks
        [Authorize]
        public ActionResult MyDecks()
        {
            GetApplicationCookie();
            string url = "UserDecks/" + User.Identity.GetUserId();
            HttpResponseMessage response = client.GetAsync(url).Result;
            Debug.WriteLine(response.Content.ReadAsStringAsync().Result);
            Debug.WriteLine(response.Content.ReadAsStringAsync().Result);
            Debug.WriteLine(response.Content.ReadAsStringAsync().Result);


            IEnumerable<DeckDto> decks = response.Content.ReadAsAsync<IEnumerable<DeckDto>>().Result;

            return View((object)User.Identity.GetUserId());
        }

        //GET: Deck/Edit/1
        [Authorize]
        public ActionResult Edit(int id)
        {
            GetApplicationCookie();
            string url = "FindDeck/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            DeckDto selectedDeck = response.Content.ReadAsAsync<DeckDto>().Result;

            return View(selectedDeck);
        }

        //GET: Deck/EditDeck/1
        [Authorize]
        public ActionResult EditDeck(int id)
        {
            GetApplicationCookie();
            string url = "FindDeck/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            DeckDto selectedDeck = response.Content.ReadAsAsync<DeckDto>().Result;

            return View(selectedDeck);
        }

        //POST: Deck/Update/1
        [HttpPost]
        [Authorize]
        public ActionResult Update(int id, Deck deck)
        {
            GetApplicationCookie();
            string url = "UpdateDeck/" + id;
            string jsonpayload = jss.Serialize(deck);
            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Details/" + id);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // GET: Deck/Delete/1
        [Authorize]
        public ActionResult DeleteConfirm(int id)
        {
            GetApplicationCookie();
            string url = "FindDeck/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            DeckDto selectedDeck = response.Content.ReadAsAsync<DeckDto>().Result;
            return View(selectedDeck);

        }

        // POST: Deck/Delete/1
        [HttpPost]
        [Authorize]
        public ActionResult Delete(int id)
        {
            GetApplicationCookie();
            string url = "DeleteDeck/" + id;
            HttpContent content = new StringContent("");
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("List");
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        public ActionResult Error()
        {
            return View();
        }

        /// <summary>
        /// Grabs the authentication cookie sent to this controller.
        /// </summary>
        private void GetApplicationCookie()
        {
            string token = "";
            //HTTP client is set up to be reused, otherwise it will exhaust server resources.
            //This is a bit dangerous because a previously authenticated cookie could be cached for
            //a follow-up request from someone else. Reset cookies in HTTP client before grabbing a new one.
            client.DefaultRequestHeaders.Remove("Cookie");
            if (!User.Identity.IsAuthenticated) return;

            HttpCookie cookie = System.Web.HttpContext.Current.Request.Cookies.Get(".AspNet.ApplicationCookie");
            if (cookie != null) token = cookie.Value;

            //collect token as it is submitted to the controller
            //use it to pass along to the WebAPI.
            Debug.WriteLine("Token Submitted is : " + token);
            if (token != "") client.DefaultRequestHeaders.Add("Cookie", ".AspNet.ApplicationCookie=" + token);

            return;
        }

    }
}
