using System;
using System.IO;    // needed for updating pictures 
using System.Web;   // needed for updating pictures 
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
        /// Update a particular Movie (with a picture uploaded)
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
            // contact database for picture upload feature
            db.Entry(Movie).Property(a => a.MovieHasPic).IsModified = false;
            db.Entry(Movie).Property(a => a.PicExtension).IsModified = false;

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
        /// Receive movie picture data, upload to the server, set MovieHasPic status
        /// </summary>
        /// <param name="id">the movie id</param>
        /// <returns>
        /// Successful status code 200
        /// </returns>
        /// <example>
        /// curl -F moviepic=@file.jpg "https://localhost:44387/api/moviedata/uploadmoviepic/1"
        /// POST: api/MovieData/UpdateMoviePic/1
        /// Form data: Movie Image
        /// </example>
        [HttpPost]
        public IHttpActionResult UploadMoviePic(int id)
        {

            bool movieHaspic = false;
            string picextension;
            if (Request.Content.IsMimeMultipartContent())
            {
                // check the number of image files received 
                int numfiles = HttpContext.Current.Request.Files.Count;
                Debug.WriteLine("Received files: " + numfiles);

                //Check if a file is posted
                if (numfiles == 1 && HttpContext.Current.Request.Files[0] != null)
                {
                    var moviePic = HttpContext.Current.Request.Files[0];
                    //Check if the file is empty
                    if (moviePic.ContentLength > 0)
                    {
                        //set up valid image file types (file extensions)
                        var imgTypes = new[] { "jpeg", "jpg", "png", "gif", "jfif", "webp" };
                        var extension = Path.GetExtension(moviePic.FileName).Substring(1);
                        //Check the extension of the file
                        if (imgTypes.Contains(extension))
                        {
                            try
                            {
                                string fileName = id + "." + extension;

                                //file path to ~/Content/Images/Movies/{id}.{extension}
                                string path = Path.Combine(HttpContext.Current.Server.MapPath("~/Content/Images/Movies/"), fileName);

                                //save the file to the path with 'fileName'
                                moviePic.SaveAs(path);

                                //movie picture was uploaded correctly
                                movieHaspic = true;
                                picextension = extension;

                                //update the movie picture related data
                                Movie Selectedmovie = db.Movies.Find(id);
                                Selectedmovie.MovieHasPic = movieHaspic;
                                Selectedmovie.PicExtension = picextension;
                                db.Entry(Selectedmovie).State = EntityState.Modified;

                                db.SaveChanges();

                            }
                            catch (Exception ex)
                            {
                                // check for error
                                Debug.WriteLine("Picture save failed");
                                Debug.WriteLine("Exception:" + ex);
                                return BadRequest();
                            }
                        }
                    }

                }

                return Ok();
            }
            else
            {
                //not multipart form data
                return BadRequest();

            }

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
        /// Delete a Movie from the system (including the movie picture)
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

            if (Movie.MovieHasPic && Movie.PicExtension != "")
            {
                //delete movie picture from path
                string path = HttpContext.Current.Server.MapPath("~/Content/Images/Movies/" + id + "." + Movie.PicExtension);
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
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