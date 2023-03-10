using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Movies_Management_System.Models;
using System.Diagnostics;

namespace Movies_Management_System.Controllers
{
    public class GenreDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// Return all Genres in the system
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: all genres in the database
        /// </returns>
        /// <example>
        /// GET: api/GenreData/ListGenre
        /// </example>
        [HttpGet]
        [ResponseType(typeof(GenreDto))]
        public IHttpActionResult ListGenre()
        {
            List<Genre> Genres = db.Genres.ToList();
            List<GenreDto> GenreDtos = new List<GenreDto>();

            Genres.ForEach(s => GenreDtos.Add(new GenreDto()
            {
                GenreID = s.GenreID,
                GenreName = s.GenreName,
            }));

            return Ok(GenreDtos);
        }

        /// <summary>
        /// Return all Genres in the system.
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: An Genre in the system matching up to the Genre ID primary key
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <param name="id">The primary key of the Genre</param>
        /// <example>
        /// GET: api/GenreData/FindGenre/6
        /// </example>
        [ResponseType(typeof(GenreDto))]
        [HttpGet]
        public IHttpActionResult FindGenre(int id)
        {
            // instantiate
            Genre Genre = db.Genres.Find(id);
            GenreDto GenreDto = new GenreDto()
            {
                GenreID = Genre.GenreID,
                GenreName = Genre.GenreName,
            };
            if (Genre == null)
            {
                return NotFound();
            }

            return Ok(GenreDto);
        }

        /// <summary>
        /// Update a particular Genre in the system via POST
        /// </summary>
        /// <param name="id">Genre ID primary key</param>
        /// <param name="Genre">Genre json format data</param>
        /// <returns>
        /// HEADER: 204 (Success, No Content Response) or 400 (Bad Request) or 404 (Not Found)
        /// </returns>
        /// <example>
        /// POST: api/GenreData/UpdateGenre/3
        /// </example>
        [ResponseType(typeof(void))]
        [HttpPost]
        public IHttpActionResult UpdateGenre(int id, Genre Genre)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != Genre.GenreID)
            {

                return BadRequest();
            }

            db.Entry(Genre).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GenreExists(id))
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
        /// Add an new Genre to the system via POST
        /// </summary>
        /// <param name="Genre">Genre json format data</param>
        /// <returns>
        /// HEADER: 201 (Created)
        /// CONTENT: Genre ID, Genre Data
        /// or
        /// HEADER: 400 (Bad Request)
        /// </returns>
        /// <example>
        /// POST: api/GenreData/AddGenre
        /// </example>
        [ResponseType(typeof(Genre))]
        [HttpPost]
        public IHttpActionResult AddGenre(Genre Genre)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Genres.Add(Genre);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = Genre.GenreID }, Genre);
        }

        /// <summary>
        /// Delete a Genre from the system via POST
        /// </summary>
        /// <param name="id">primary key of the Genre</param>
        /// <returns>
        /// HEADER: 200 (OK) or 404 (NOT FOUND)
        /// </returns>
        /// <example>
        /// POST: api/GenreData/DeleteGenre/1
        /// </example>
        [ResponseType(typeof(Genre))]
        [HttpPost]
        public IHttpActionResult DeleteGenre(int id)
        {
            Genre Genre = db.Genres.Find(id);
            if (Genre == null)
            {
                return NotFound();
            }

            db.Genres.Remove(Genre);
            db.SaveChanges();

            return Ok();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool GenreExists(int id)
        {
            return db.Genres.Count(e => e.GenreID == id) > 0;
        }
    }
}