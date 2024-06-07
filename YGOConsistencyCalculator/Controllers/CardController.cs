using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using YGOConsistencyCalculator.Models;

namespace YGOConsistencyCalculator.Controllers
{
    public class CardController : Controller
    {
        private static readonly HttpClient client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();

        static CardController()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:44386/api/CardData/");
        }
        
        // GET: Card/New
        public ActionResult New()
        {
            return View();
        }

        // POST: Card/Create
        [HttpPost]
        public ActionResult Create(Card card)
        {
            string url = "AddCard";

            string jsonpayload = jss.Serialize(card);

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

        // GET: Card/List
        public ActionResult List()
        {
            string url = "ListCards";
            HttpResponseMessage response = client.GetAsync(url).Result;

            IEnumerable<CardDto> cards = response.Content.ReadAsAsync<IEnumerable<CardDto>>().Result;

            return View(cards);
        }

        // GET: Card/Details/1
        public ActionResult Details(int id)
        {
            string url = "FindCard/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            CardDto selectedCard = response.Content.ReadAsAsync<CardDto>().Result;

            return View(selectedCard);
        }

        // GET: Card/Edit/1
        public ActionResult Edit(int id)
        {
            string url = "FindCard/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            CardDto selectedCard = response.Content.ReadAsAsync<CardDto>().Result;
            return View(selectedCard);
        }

        // POST: Animal/Update/5
        [HttpPost]
        public ActionResult Update(int id, Card card)
        {

            string url = "UpdateCard/" + id;
            string jsonpayload = jss.Serialize(card);
            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Details/" +id);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // GET: Card/DeleteConfirm/1
        public ActionResult DeleteConfirm(int id)
        {
            string url = "FindCard/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            CardDto selectedCard = response.Content.ReadAsAsync<CardDto>().Result;
            return View(selectedCard);

        }

        // POST: Card/Delete/1
        [HttpPost]
        public ActionResult Delete(int id)
        {
            string url = "DeleteCard/" + id;
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
