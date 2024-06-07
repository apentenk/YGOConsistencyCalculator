using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using YGOConsistencyCalculator.Models;

namespace YGOConsistencyCalculator.Controllers
{
    public class CardDataController : ApiController
    {
        private ApplicationDbContext  db = new ApplicationDbContext();

        /// <summary>
        /// Adds a card to the system
        /// </summary>
        /// <param name="card">JSON FORM DATA of a card</param>
        /// <returns>
        /// HEADER: 201 (Created)
        /// CONTENT: Card ID, Card Data
        /// or
        /// HEADER: 400 (Bad Request)
        /// </returns>
        /// <example>
        /// POST: api/CardData/AddCard
        /// FORM DATA: Card JSON Object
        /// </example>
        [ResponseType(typeof(Card))]
        [HttpPost]
        [Route("api/CardData/AddCard/")]
        public IHttpActionResult AddCard(Card NewCard)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            db.Cards.Add(NewCard);
            db.SaveChanges();
            return StatusCode(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Returns all cards in the system.
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: all cards in the database.
        /// </returns>
        /// <example>
        /// GET: api/CardData/ListCards
        /// </example>
        [HttpGet]
        [Route("api/CardData/ListCards")]
        public IEnumerable<CardDto> ListCards()
        {
            List<Card> CardList = db.Cards.ToList();
            List<CardDto> CardDtoList = new List<CardDto>();

            CardList.ForEach(c => CardDtoList.Add(new CardDto()
            {
                Id = c.Id,
                Number = c.Number,
                Name = c.Name,
                CardFrame = c.CardFrame,
                CardType = c.CardType,
                Attribute = c.Attribute,
                Level = c.Level,
                Scale = c.Scale,
                Attack = c.Attack,
                Defense = c.Defense,
            }));
            return CardDtoList;
        }

        /// <summary>
        /// Returns a specific card in the system.
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: A card in the system matching up to the card ID primary key
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <param name="id">The primary key of the card</param>
        /// <example>
        /// GET: api/CardData/FindCard/1
        /// </example>
        [HttpGet]
        [Route("api/CardData/FindCard/{id}")]
        [ResponseType(typeof(CardDto))]
        public IHttpActionResult FindCard(int id)
        {
            Card card = db.Cards.Find(id);
            if (card == null)
            {
                return NotFound();
            }
            CardDto cardDto = new CardDto()
            {
                Id = card.Id,
                Number = card.Number,
                Name = card.Name,
                CardFrame = card.CardFrame,
                CardType = card.CardType,
                Level = card.Level,
                Attribute = card.Attribute,
                Scale = card.Scale,
                Attack = card.Attack,
                Defense = card.Defense
            };
            return Ok(cardDto);
        }

        /// <summary>
        /// Returns all card in the system that have a name containing a search key.
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: Card in the system with a card name that cointains the search value
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <param name="search">The search key of the card name</param>
        /// <example>
        /// GET: api/CardData/NameSearch/abyss
        /// </example>
        [HttpGet]
        [Route("api/CardData/NameSearch/{search}")]
        public IEnumerable<CardDto> NameSearch(string search)
        {
            List<Card> Cards = db.Cards.Where(c => c.Name.Contains(search)).ToList();
            List<CardDto> CardDtos = new List<CardDto>();
            Cards.ForEach(c => CardDtos.Add(new CardDto()
            {
                Id = c.Id,
                Number = c.Number,
                Name = c.Name,
                CardFrame = c.CardFrame,
                CardType = c.CardType,
                Level = c.Level,
                Attribute = c.Attribute,
                Scale = c.Scale,
                Attack = c.Attack,
                Defense = c.Defense
            }));
            return CardDtos;
        }

        /// <summary>
        /// Updates a particular card in the system with POST Data input
        /// </summary>
        /// <param name="id">Represents the Card ID primary key</param>
        /// <param name="card">JSON FORM DATA of an card</param>
        /// <returns>
        /// HEADER: 204 (Success, No Content Response)
        /// or
        /// HEADER: 400 (Bad Request)
        /// or
        /// HEADER: 404 (Not Found)
        /// </returns>
        /// <example>
        /// POST: api/CardData/UpdateCard/1
        /// FORM DATA: Card JSON Object
        /// </example>
        [ResponseType(typeof(Card))]
        [HttpPost]
        [Route("api/CardData/UpdateCard/{id}")]
        public IHttpActionResult UpdateCard(int id, [FromBody] Card card)
        {
            Debug.WriteLine("here?");
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            if (id != card.Id)
            {
                return BadRequest();
            }

            db.Entry(card).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!(db.Cards.Count(e => e.Id == id) > 0))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return StatusCode(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Deletes a card from the system by it's ID.
        /// </summary>
        /// <param name="id">The primary key of the card</param>
        /// <returns>
        /// HEADER: 200 (OK)
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <example>
        /// POST: api/CardData/DeleteCard/1
        /// FORM DATA: (empty)
        /// </example>
        [ResponseType(typeof(Card))]
        [HttpPost]
        [Route("api/CardData/DeleteCard/{id}")]
        public IHttpActionResult DeleteCard(int id)
        {
            Card card = db.Cards.Find(id);
            if (card == null)
            {
                return NotFound();
            }
            db.Cards.Remove(card);
            db.SaveChanges();
            return Ok();
        }
    }
}
