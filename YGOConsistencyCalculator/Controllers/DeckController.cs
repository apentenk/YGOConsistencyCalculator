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
        public ActionResult New()
        {
            return View();
        }

        // POST: Deck/Create
        [HttpPost]
        public ActionResult Create(Deck deck)
        {
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
        public ActionResult List()
        {
            string url = "ListDecks";
            HttpResponseMessage response = client.GetAsync(url).Result;

            IEnumerable<DeckDto> decks = response.Content.ReadAsAsync<IEnumerable<DeckDto>>().Result;

            return View(decks);
        }

        //GET: Deck/Details/1
        public ActionResult Details(int id)
        {
            string url = "FindDeck/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            DeckDto selectedDeck = response.Content.ReadAsAsync<DeckDto>().Result;

            return View(selectedDeck);
        }

        //GET: Deck/MyDecks
        public ActionResult MyDecks()
        {

            string url = "UserDecks/" + User.Identity.GetUserId();
            HttpResponseMessage response = client.GetAsync(url).Result;
            Debug.WriteLine(response.Content.ReadAsStringAsync().Result);
            Debug.WriteLine(response.Content.ReadAsStringAsync().Result);
            Debug.WriteLine(response.Content.ReadAsStringAsync().Result);


            IEnumerable<DeckDto> decks = response.Content.ReadAsAsync<IEnumerable<DeckDto>>().Result;

            return View((object)User.Identity.GetUserId());
        }

        //GET: Deck/Edit/1
        public ActionResult Edit(int id)
        {
            string url = "FindDeck/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            DeckDto selectedDeck = response.Content.ReadAsAsync<DeckDto>().Result;

            return View(selectedDeck);
        }

        //GET: Deck/EditDeck/1
        public ActionResult EditDeck(int id)
        {
            string url = "FindDeck/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            DeckDto selectedDeck = response.Content.ReadAsAsync<DeckDto>().Result;

            return View(selectedDeck);
        }

        //POST: Deck/Update/1
        [HttpPost]
        public ActionResult Update(int id, Deck deck)
        {
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
        public ActionResult DeleteConfirm(int id)
        {
            string url = "FindDeck/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            DeckDto selectedDeck = response.Content.ReadAsAsync<DeckDto>().Result;
            return View(selectedDeck);

        }

        // POST: Deck/Delete/1
        [HttpPost]
        public ActionResult Delete(int id)
        {
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

    }
}
