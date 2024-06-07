using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Web.Http;
using System.Web.Http.Description;
using YGOConsistencyCalculator.Models;


namespace YGOConsistencyCalculator.Controllers
{
    public class DeckDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// Adds a Deck to the system
        /// </summary>
        /// <param name="deck">JSON FORM DATA of a deck</param>
        /// <returns>
        /// HEADER: 201 (Created)
        /// CONTENT: Deck ID, Deck Data
        /// or
        /// HEADER: 400 (Bad Request)
        /// </returns>
        /// <example>
        /// POST: api/DeckData/AddDeck
        /// FORM DATA: Deck JSON Object
        /// </example>
        [ResponseType(typeof(Deck))]
        [HttpPost]
        [Route("api/DeckData/AddDeck/")]
        public IHttpActionResult AddDeck(Deck NewDeck)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            db.Decks.Add(NewDeck);
            db.SaveChanges();
            return StatusCode(HttpStatusCode.NoContent);
        }

        //POST: /api/DeckData/CreateDeck
        [ResponseType(typeof(Deck))]
        [HttpPost]
        [Route("api/DeckData/CreateDeck/")]
        public int CreateDeck(Deck NewDeck)
        {
            if (!ModelState.IsValid)
            {
                return -1;
            }
            db.Decks.Add(NewDeck);
            db.SaveChanges();
            return NewDeck.DeckId;
        }

        /// <summary>
        /// Returns all decks in the system.
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: all decks in the database.
        /// </returns>
        /// <example>
        /// GET: api/DeckData/ListDecks
        /// </example>
        [HttpGet]
        [Route("api/DeckData/ListDecks")]
        public IEnumerable<DeckDto> ListDecks()
        {
            List<Deck> DeckList = db.Decks.ToList();
            List<DeckDto> DeckDtoList = new List<DeckDto>();

            DeckList.ForEach(d => DeckDtoList.Add(new DeckDto()
            {
                DeckId = d.DeckId,
                UserId = d.UserId,
                DeckName = d.DeckName,
                StarterChance = d.StarterChance,
                OneCardChance = d.OneCardChance,
                TwoCardChance = d.TwoCardChance,
                ExtenderChance = d.ExtenderChance,
                BrickChance = d.BrickChance,
                EngineReqChance = d.EngineReqChance
            }));
            return DeckDtoList;
        }

        /// <summary>
        /// Returns a specific deck in the system.
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: A deck in the system matching up to the deck ID primary key
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <param name="id">The primary key of the deck</param>
        /// <example>
        /// GET: api/DeckData/FindDeck/1
        /// </example>
        [HttpGet]
        [Route("api/DeckData/FindDeck/{id}")]
        [ResponseType(typeof(DeckDto))]
        public IHttpActionResult FindDeck(int id)
        {
            Deck deck = db.Decks.Find(id);
            DeckDto deckDto = new DeckDto()
            {
                DeckId = deck.DeckId,
                UserId = deck.UserId,
                DeckName = deck.DeckName,
                DeckList = deck.DeckList,
                StarterChance = deck.StarterChance,
                OneCardChance = deck.OneCardChance,
                TwoCardChance = deck.TwoCardChance,
                ExtenderChance = deck.ExtenderChance,
                BrickChance = deck.BrickChance,
                EngineReqChance = deck.EngineReqChance

            };
            if (deck == null)
            {
                return NotFound();
            }
            return Ok(deckDto);
        }

        /// <summary>
        /// Returns all deck in the system that belong to a specific user.
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: Deck in the system with a that belong to a specific user
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <param name="id">The id of the owner of the deck</param>
        /// <example>
        /// GET: api/DeckData/UserDecks/1231as-asdas546
        /// </example>
        [HttpGet]
        [Route("api/DeckData/UserDecks/{id}")]
        public IEnumerable<DeckDto> UserDecks(string id)
        {
            List<Deck> Decks = db.Decks.Where(d => d.UserId.Equals(id)).ToList();
            List<DeckDto> DeckDtos = new List<DeckDto>();
            Decks.ForEach(d => DeckDtos.Add(new DeckDto()
            {
                DeckId = d.DeckId,
                UserId = d.UserId,
                DeckName = d.DeckName,
                StarterChance = d.StarterChance,
                OneCardChance = d.OneCardChance,
                TwoCardChance = d.TwoCardChance,
                ExtenderChance = d.ExtenderChance,
                BrickChance = d.BrickChance,
                EngineReqChance = d.EngineReqChance
            }));
            return DeckDtos;
        }

        /// <summary>
        /// Updates a particular deck in the system with POST Data input
        /// </summary>
        /// <param name="id">Represents the Deck ID primary key</param>
        /// <param name="deck">JSON FORM DATA of an deck</param>
        /// <returns>
        /// HEADER: 204 (Success, No Content Response)
        /// or
        /// HEADER: 400 (Bad Request)
        /// or
        /// HEADER: 404 (Not Found)
        /// </returns>
        /// <example>
        /// POST: api/DeckData/UpdateDeck/1
        /// FORM DATA: Deck JSON Object
        /// </example>
        [ResponseType(typeof(Deck))]
        [HttpPost]
        [Route("api/DeckData/UpdateDeck/{id}")]
        public IHttpActionResult UpdateDeck(int id, [FromBody] Deck deck)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != deck.DeckId)
            {
                return BadRequest();
            }

            db.Entry(deck).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!(db.Decks.Count(e => e.DeckId == id) > 0))
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
        /// Deletes a deck from the system by it's ID.
        /// </summary>
        /// <param name="id">The primary key of the deck</param>
        /// <returns>
        /// HEADER: 200 (OK)
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <example>
        /// POST: api/DeckData/DeleteDeck/1
        /// FORM DATA: (empty)
        /// </example>
        [HttpPost]
        [Route("api/DeckData/DeleteDeck/{id}")]
        public IHttpActionResult DeleteDeck(int id)
        {
            Deck deck = db.Decks.Find(id);
            if (deck == null)
            {
                return NotFound();
            }
            db.Decks.Remove(deck);
            db.SaveChanges();
            return Ok();
        }
    }
}