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
    public class ComboPieceDataController : ApiController
    {
        private ApplicationDbContext  db = new ApplicationDbContext();

        /// <summary>
        /// Adds a Combo Piece to the system
        /// </summary>
        /// <param name="NewComboPiece">JSON FORM DATA of a combo piece definition</param>
        /// <returns>
        /// HEADER: 201 (Created)
        /// CONTENT: Combo piece definition ID, Combo piece definition Data
        /// or
        /// HEADER: 400 (Bad Request)
        /// </returns>
        /// <example>
        /// POST: api/ComboPieceData/AddComboPiece
        /// FORM DATA: Combo piece definition JSON Object
        /// </example>
        [ResponseType(typeof(ComboPiece))]
        [HttpPost]
        [Route("api/ComboPieceData/AddComboPiece/")]
        public IHttpActionResult AddComboPiece(ComboPiece NewComboPiece)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            db.ComboPieces.Add(NewComboPiece);
            db.SaveChanges();
            return StatusCode(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Returns all Combo Pieces in the system.
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: all Combo Pieces in the database.
        /// </returns>
        /// <example>
        /// GET: api/ComboPieceData/ListComboPieces
        /// </example>
        [HttpGet]
        [Route("api/ComboPieceData/ListComboPieces")]
        public IEnumerable<ComboPieceDto> ListComboPieces()
        {
            List<ComboPiece> ComboPieceList = db.ComboPieces.ToList();
            List<ComboPieceDto> ComboPieceDtosList = new List<ComboPieceDto>();

            ComboPieceList.ForEach(c => ComboPieceDtosList.Add(new ComboPieceDto()
            {
                ComboPieceId = c.ComboPieceId,
                CardNumber = c.CardNumber,
                DeckId = c.DeckId,
                Category = c.Category
            }));
            return ComboPieceDtosList;
        }

        /// <summary>
        /// Returns a specific Combo Piece in the system.
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: A Combo Piece in the system matching up to the Combo Piece ID primary key
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <param name="id">The primary key of the Combo Piece</param>
        /// <example>
        /// GET: api/ComboPieceData/FindComboPiece/1
        /// </example>
        [HttpGet]
        [Route("api/ComboPieceData/FindComboPiece/{id}")]
        [ResponseType(typeof(ComboPieceDto))]
        public IHttpActionResult FindComboPiece(int id)
        {
            ComboPiece ComboPiece = db.ComboPieces.Find(id);
            ComboPieceDto ComboPieceDto = new ComboPieceDto()
            {
                ComboPieceId = ComboPiece.ComboPieceId,
                CardNumber = ComboPiece.CardNumber,
                DeckId = ComboPiece.DeckId,
                Category = ComboPiece.Category
            };
            if (ComboPiece == null)
            {
                return NotFound();
            }
            return Ok(ComboPieceDto);
        }

        /// <summary>
        /// Returns all combo pieces in the system that belong to a specific deck.
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: Combo pieces in the system with a that belong to a specific deck
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <param name="id">The id of the deck containing the combo pieces</param>
        /// <example>
        /// GET: api/ComboData/GetComboPieces/1
        /// </example>
        [HttpGet]
        [Route("api/ComboData/GetComboPieces/{id}")]
        public IEnumerable<ComboPieceDto> GetComboPieces(int id)
        {
            List<ComboPiece> ComboPieces = db.ComboPieces.Where(c => c.DeckId == id).ToList();
            List<ComboPieceDto> ComboPiecesDtos = new List<ComboPieceDto>();
            ComboPieces.ForEach(c => ComboPiecesDtos.Add(new ComboPieceDto()
            {
                ComboPieceId = c.ComboPieceId,
                CardNumber = c.CardNumber,
                DeckId = c.DeckId,
                Category = c.Category
            }));
            return ComboPiecesDtos;
        }

        /// <summary>
        /// Updates a particular combo piece in the system with POST Data input
        /// </summary>
        /// <param name="id">Represents the combo piece ID primary key</param>
        /// <param name="deck">JSON FORM DATA of an combo piece</param>
        /// <returns>
        /// HEADER: 204 (Success, No Content Response)
        /// or
        /// HEADER: 400 (Bad Request)
        /// or
        /// HEADER: 404 (Not Found)
        /// </returns>
        /// <example>
        /// POST: api/ComboPieceData/UpdateComboPiece/1
        /// FORM DATA: Deck JSON Object
        /// </example>
        [ResponseType(typeof(ComboPiece))]
        [HttpPost]
        [Route("api/ComboPieceData/UpdateComboPiece/{id}")]
        public IHttpActionResult UpdateComboPiece(int id, [FromBody] ComboPiece ComboPiece)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != ComboPiece.ComboPieceId)
            {
                return BadRequest();
            }

            db.Entry(ComboPiece).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!(db.ComboPieces.Count(e => e.ComboPieceId == id) > 0))
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
        /// Updates all the combo pieces for a specific deck. Adds new combo pieces, updates modified combo pieces and removes old combo pieces
        /// </summary>
        /// <param name="id">Represents the Deck ID</param>
        /// <param name="NewComboPieces"> A list of new combo pieces to be added</param>
        /// <returns>
        /// HEADER: 204 (Success, No Content Response)
        /// or
        /// HEADER: 400 (Bad Request)
        /// </returns>
        /// <example>
        /// POST: api/ComboPieceData/UpdateComboPieces/1
        /// FORM DATA: List Combo piece JSON Object
        /// </example>
        [ResponseType(typeof(ComboPiece))]
        [HttpPost]
        [Route("api/ComboPieceData/UpdateComboPieces/{id}")]
        public IHttpActionResult UpdateComboPieces(int id, [FromBody] List<ComboPiece> NewComboPieces)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            List<ComboPiece> OldComboPieceList = db.ComboPieces.Where(c => c.DeckId.Equals(id)).ToList();
            List<int> oldComboPieceNumbers = new List<int>();
            List<int> newComboPieceNumbers = new List<int>();
            OldComboPieceList.ForEach(c => oldComboPieceNumbers.Add(c.CardNumber));
            NewComboPieces.ForEach(c => newComboPieceNumbers.Add(c.CardNumber));

            foreach (ComboPiece NewComboPiece in NewComboPieces)
            {
                if (!oldComboPieceNumbers.Contains(NewComboPiece.CardNumber))
                {
                    db.ComboPieces.Add(NewComboPiece);
                }
                else
                {
                    var OldComboPiece = db.ComboPieces.Where(c => c.CardNumber == NewComboPiece.CardNumber && c.DeckId == id).First();
                    db.Entry(OldComboPiece).Property("Category").CurrentValue = NewComboPiece.Category;
                }

            }
            foreach (ComboPiece OldComboPiece in OldComboPieceList)
            {
                if (!newComboPieceNumbers.Contains(OldComboPiece.CardNumber))
                {
                    db.ComboPieces.Remove(OldComboPiece);
                }
            }
            db.SaveChanges();
            return StatusCode(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Deletes a combo piece from the system by it's ID.
        /// </summary>
        /// <param name="id">The primary key of the combo piece</param>
        /// <returns>
        /// HEADER: 200 (OK)
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <example>
        /// POST: api/ComboPieceData/DeleteComboPiece/1
        /// FORM DATA: (empty)
        /// </example>
        [ResponseType(typeof(ComboPiece))]
        [HttpPost]
        [Route("api/ComboPieceData/DeleteComboPiece/{id}")]
        public IHttpActionResult DeleteComboPiece(int id)
        {
            ComboPiece ComboPiece = db.ComboPieces.Find(id);
            if (ComboPiece == null)
            {
                return NotFound();
            }
            db.ComboPieces.Remove(ComboPiece);
            db.SaveChanges();
            return Ok();
        }

    }
}
