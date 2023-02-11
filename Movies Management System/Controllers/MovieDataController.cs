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
        /// Return all Movies in the system
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: all Movies in the database
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

            // loop through the database movie table to get all information
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
        /// Get information about all movies related to a particular genre ID
        /// For simple design, one movie only has one genre type, but one genre can have many movies.  1--M relationship
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: all movies in the database, including their associated genre matched with a particular genre ID
        /// </returns>
        /// <param name="id">Genre ID.</param>
        /// <example>
        /// GET: api/MovieData/ListMoviesForGenre/2
        /// Click on genre name (e.g. Drama) can direct to the '/genre/details' page, which lists this genre with associated movies 
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
        /// Get information about Movies related to a particular Client
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: all Movies in the database, including their associated Genre that match to a particular Client id
        /// </returns>
        /// <param name="id">Client ID.</param>
        /// <example>
        /// GET: api/MovieData/ListMoviesForClient/3
        /// Clients WatchList
        /// Click on client name will direct to the '/client/details' page, which lists this client with associated movies(WatchList) 
        /// </example>
        [HttpGet]
        [ResponseType(typeof(MovieDto))]
        public IHttpActionResult ListMoviesForClient(int id)
        {
            //all Movies that have Clients which match with ID
            List<Movie> Movies = db.Movies.Where(
                a => a.Clients.Any(
                    k => k.ClientID == id)).ToList();
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
        /// HEADER: 200 (OK) or 404 (NOT FOUND)
        /// </returns>
        /// <example>
        /// POST api/MovieData/AssociateMovieWithClient/9/3
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

            //Debug.WriteLine("Movie id: " + Movieid);
            //Debug.WriteLine("Selected Movie: " + SelectedMovie.MovieTitle);

            //Debug.WriteLine("Client id: " + Clientid);
            //Debug.WriteLine("Selected Client: " + SelectedClient.ClientName);

            SelectedMovie.Clients.Add(SelectedClient);
            db.SaveChanges();

            return Ok();
        }

        /// <summary>
        /// Unlink a particular Client with a particular Movie
        /// </summary>
        /// <param name="Movieid">The Movie ID primary key</param>
        /// <param name="Clientid">The Client ID primary key</param>
        /// <returns>
        /// HEADER: 200 (OK) or 404 (NOT FOUND)
        /// </returns>
        /// <example>
        /// POST api/MovieData/AssociateMovieWithClient/9/5
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

            //Debug.WriteLine("Movie id: " + Movieid);
            //Debug.WriteLine("Selected Movie: " + SelectedMovie.MovieTitle);

            //Debug.WriteLine("Client id: " + Clientid);
            //Debug.WriteLine("Selected Client: " + SelectedClient.ClientName);

            SelectedMovie.Clients.Remove(SelectedClient);
            db.SaveChanges();

            return Ok();
        }

        /// <summary>
        /// Get all Movies in the system.
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: An Movie in the system matching up to the Movie ID primary key
        /// or HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <param name="id">The primary key of the Movie</param>
        /// <example>
        /// GET: api/MovieData/FindMovie/2
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
        /// Update a particular Movie
        /// </summary>
        /// <param name="id">Movie ID primary key</param>
        /// <param name="Movie">Movie json data</param>
        /// <returns>
        /// HEADER: 204 (Success, No Content Response) or 400 (Bad Request) or 404 (Not Found)
        /// </returns>
        /// <example>
        /// POST: api/MovieData/UpdateMovie/1
        /// Form data: Movie JSON Object
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
        /// Add a new Movie to the system
        /// </summary>
        /// <param name="Movie">Movie json data</param>
        /// <returns>
        /// HEADER: 201 (Created)
        /// CONTENT: Movie ID, Movie Data
        /// or HEADER: 400 (Bad Request)
        /// </returns>
        /// <example>
        /// POST: api/MovieData/AddMovie
        /// Form data: Movie JSON Object
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
        /// Delete a Movie from the system
        /// </summary>
        /// <param name="id">Movie primary key</param>
        /// <returns>
        /// HEADER: 200 (OK) or 404 (NOT FOUND)
        /// </returns>
        /// <example>
        /// POST: api/MovieData/DeleteMovie/3
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