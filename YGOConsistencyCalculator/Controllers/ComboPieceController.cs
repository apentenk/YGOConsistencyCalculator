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
    public class ComboPieceController : Controller
    {
        private static readonly HttpClient client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();

        static ComboPieceController()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:44386/api/ComboPieceData/");
        }
        
        // GET: ComboPiece/New
        public ActionResult New()
        {
            return View();
        }

        // POST: ComboPiece/Create
        [HttpPost]
        public ActionResult Create(ComboPiece comboPiece)
        {
            string url = "AddComboPiece";

            string jsonpayload = jss.Serialize(comboPiece);

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

        // GET: ComboPiece/List
        public ActionResult List()
        {
            string url = "ListComboPieces";
            HttpResponseMessage response = client.GetAsync(url).Result;

            IEnumerable<ComboPieceDto> comboPieces = response.Content.ReadAsAsync<IEnumerable<ComboPieceDto>>().Result;

            return View(comboPieces);
        }

        // GET: ComboPiece/Details/1
        public ActionResult Details(int id)
        {
            string url = "FindComboPiece/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            ComboPieceDto selectedComboPiece = response.Content.ReadAsAsync<ComboPieceDto>().Result;

            return View(selectedComboPiece);
        }

        // GET: ComboPiece/Edit/1
        public ActionResult Edit(int id)
        {
            string url = "FindComboPiece/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            ComboPieceDto selectedComboPiece = response.Content.ReadAsAsync<ComboPieceDto>().Result;
            return View(selectedComboPiece);
        }

        // POST: Animal/Update/5
        [HttpPost]
        public ActionResult Update(int id, ComboPiece comboPiece)
        {

            string url = "UpdateComboPiece/" + id;
            string jsonpayload = jss.Serialize(comboPiece);
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

        // GET: ComboPiece/Delete/1
        public ActionResult DeleteConfirm(int id)
        {
            string url = "FindComboPiece/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            ComboPieceDto selectedComboPiece = response.Content.ReadAsAsync<ComboPieceDto>().Result;
            return View(selectedComboPiece);

        }

        // POST: ComboPiece/Delete/1
        [HttpPost]
        public ActionResult Delete(int id)
        {
            string url = "DeleteComboPiece/" + id;
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
