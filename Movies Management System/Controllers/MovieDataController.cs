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
    public class MovieDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/MovieData
        /// <summary>
        /// Returns all Movies in the system.
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: all Movies in the database, including their associated Genre.
        /// </returns>
        /// <example>
        /// GET: api/MovieData/ListMovies
        /// </example>
        [HttpGet]
        [ResponseType(typeof(MovieDto))]
        public IHttpActionResult ListMovies()
        {
            List<Movie> Movies = db.Movies.ToList();
            List<MovieDto> MovieDtos = new List<MovieDto>();

            Movies.ForEach(a => MovieDtos.Add(new MovieDto()
            {
                MovieID = a.MovieID,
                MovieTitle = a.MovieTitle,
                PubYear = a.PubYear,
                Director = a.Director,
                Ratings = a.Ratings,
                GenreID = a.Genre.GenreID,
                GenreName = a.Genre.GenreName
            }));

            return Ok(MovieDtos);
        }

        /// <summary>
        /// Gathers information about all Movies related to a particular genre ID
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: all Movies in the database, including their associated genre matched with a particular genre ID
        /// </returns>
        /// <param name="id">Genre ID.</param>
        /// <example>
        /// GET: api/MovieData/ListMoviesForGenre/3
        /// </example>
        [HttpGet]
        [ResponseType(typeof(MovieDto))]
        public IHttpActionResult ListMoviesForGenre(int id)
        {
            List<Movie> Movies = db.Movies.Where(a => a.GenreID == id).ToList();
            List<MovieDto> MovieDtos = new List<MovieDto>();

            Movies.ForEach(a => MovieDtos.Add(new MovieDto()
            {
                MovieID = a.MovieID,
                MovieTitle = a.MovieTitle,
                PubYear = a.PubYear,
                Director = a.Director,
                Ratings = a.Ratings,
                GenreID = a.Genre.GenreID,
                GenreName = a.Genre.GenreName
            }));

            return Ok(MovieDtos);
        }

        /// <summary>
        /// Gathers information about Movies related to a particular Client
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: all Movies in the database, including their associated Genre that match to a particular Client id
        /// </returns>
        /// <param name="id">Client ID.</param>
        /// <example>
        /// GET: api/MovieData/ListMoviesForClient/1
        /// </example>
        [HttpGet]
        [ResponseType(typeof(MovieDto))]
        public IHttpActionResult ListMoviesForClient(int id)
        {
            //all Movies that have Clients which match with our ID
            List<Movie> Movies = db.Movies.Where(
                a => a.Clients.Any(
                    k => k.ClientID == id
                )).ToList();
            List<MovieDto> MovieDtos = new List<MovieDto>();

            Movies.ForEach(a => MovieDtos.Add(new MovieDto()
            {
                MovieID = a.MovieID,
                MovieTitle = a.MovieTitle,
                PubYear = a.PubYear,
                Director = a.Director,
                Ratings = a.Ratings,
                GenreID = a.Genre.GenreID,
                GenreName = a.Genre.GenreName
            }));

            return Ok(MovieDtos);
        }


        /// <summary>
        /// Associates a particular Client with a particular Movie
        /// </summary>
        /// <param name="Movieid">The Movie ID primary key</param>
        /// <param name="Clientid">The Client ID primary key</param>
        /// <returns>
        /// HEADER: 200 (OK)
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <example>
        /// POST api/MovieData/AssociateMovieWithClient/9/1
        /// </example>
        [HttpPost]
        [Route("api/MovieData/AssociateMovieWithClient/{Movieid}/{Clientid}")]
        public IHttpActionResult AssociateMovieWithClient(int Movieid, int Clientid)
        {

            Movie SelectedMovie = db.Movies.Include(a => a.Clients).Where(a => a.MovieID == Movieid).FirstOrDefault();
            Client SelectedClient = db.Clients.Find(Clientid);

            if (SelectedMovie == null || SelectedClient == null)
            {
                return NotFound();
            }

            //Debug.WriteLine("input Movie id is: " + Movieid);
            //Debug.WriteLine("selected Movie name is: " + SelectedMovie.MovieTitle);
            //Debug.WriteLine("input Client id is: " + Clientid);
            //Debug.WriteLine("selected Client name is: " + SelectedClient.ClientName);


            SelectedMovie.Clients.Add(SelectedClient);
            db.SaveChanges();

            return Ok();
        }

        /// <summary>
        /// Removes an association between a particular Client and a particular Movie
        /// </summary>
        /// <param name="Movieid">The Movie ID primary key</param>
        /// <param name="Clientid">The Client ID primary key</param>
        /// <returns>
        /// HEADER: 200 (OK)
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <example>
        /// POST api/MovieData/AssociateMovieWithClient/9/1
        /// </example>
        [HttpPost]
        [Route("api/MovieData/UnAssociateMovieWithClient/{Movieid}/{Clientid}")]
        public IHttpActionResult UnAssociateMovieWithClient(int Movieid, int Clientid)
        {

            Movie SelectedMovie = db.Movies.Include(a => a.Clients).Where(a => a.MovieID == Movieid).FirstOrDefault();
            Client SelectedClient = db.Clients.Find(Clientid);

            if (SelectedMovie == null || SelectedClient == null)
            {
                return NotFound();
            }

            //Debug.WriteLine("input Movie id is: " + Movieid);
            //Debug.WriteLine("selected Movie name is: " + SelectedMovie.MovieTitle);
            //Debug.WriteLine("input Client id is: " + Clientid);
            //Debug.WriteLine("selected Client name is: " + SelectedClient.ClientName);


            SelectedMovie.Clients.Remove(SelectedClient);
            db.SaveChanges();

            return Ok();
        }

        /// <summary>
        /// Returns all Movies in the system.
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: An Movie in the system matching up to the Movie ID primary key
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <param name="id">The primary key of the Movie</param>
        /// <example>
        /// GET: api/MovieData/FindMovie/5
        /// </example>
        [ResponseType(typeof(MovieDto))]
        [HttpGet]
        public IHttpActionResult FindMovie(int id)
        {
            Movie Movie = db.Movies.Find(id);
            MovieDto MovieDto = new MovieDto()
            {
                MovieID = Movie.MovieID,
                MovieTitle = Movie.MovieTitle,
                PubYear = Movie.PubYear,
                Director = Movie.Director,
                Ratings = Movie.Ratings,
                GenreID = Movie.Genre.GenreID,
                GenreName = Movie.Genre.GenreName
            };
            if (Movie == null)
            {
                return NotFound();
            }

            return Ok(MovieDto);
        }

        /// <summary>
        /// Updates a particular Movie in the system with POST Data input
        /// </summary>
        /// <param name="id">Represents the Movie ID primary key</param>
        /// <param name="Movie">JSON FORM DATA of an Movie</param>
        /// <returns>
        /// HEADER: 204 (Success, No Content Response)
        /// or
        /// HEADER: 400 (Bad Request)
        /// or
        /// HEADER: 404 (Not Found)
        /// </returns>
        /// <example>
        /// POST: api/MovieData/UpdateMovie/5
        /// FORM DATA: Movie JSON Object
        /// </example>
        [ResponseType(typeof(void))]
        [HttpPost]
        public IHttpActionResult UpdateMovie(int id, Movie Movie)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != Movie.MovieID)
            {

                return BadRequest();
            }

            db.Entry(Movie).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MovieExists(id))
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
        /// Adds an Movie to the system
        /// </summary>
        /// <param name="Movie">JSON FORM DATA of an Movie</param>
        /// <returns>
        /// HEADER: 201 (Created)
        /// CONTENT: Movie ID, Movie Data
        /// or
        /// HEADER: 400 (Bad Request)
        /// </returns>
        /// <example>
        /// POST: api/MovieData/AddMovie
        /// FORM DATA: Movie JSON Object
        /// </example>
        [ResponseType(typeof(Movie))]
        [HttpPost]
        public IHttpActionResult AddMovie(Movie Movie)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Movies.Add(Movie);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = Movie.MovieID }, Movie);
        }

        /// <summary>
        /// Deletes an Movie from the system by it's ID.
        /// </summary>
        /// <param name="id">The primary key of the Movie</param>
        /// <returns>
        /// HEADER: 200 (OK)
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <example>
        /// POST: api/MovieData/DeleteMovie/5
        /// FORM DATA: (empty)
        /// </example>
        [ResponseType(typeof(Movie))]
        [HttpPost]
        public IHttpActionResult DeleteMovie(int id)
        {
            Movie Movie = db.Movies.Find(id);
            if (Movie == null)
            {
                return NotFound();
            }

            db.Movies.Remove(Movie);
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

        private bool MovieExists(int id)
        {
            return db.Movies.Count(e => e.MovieID == id) > 0;
        }
    }
}