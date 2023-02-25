using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Diagnostics;
using System.Net.Http;
using Movies_Management_System.Models;
using Movies_Management_System.Models.ViewModels;
using System.Web.Script.Serialization;

namespace Movies_Management_System.Controllers
{
    // set up CRUD functions (and others) for Movie  
    public class MovieController : Controller
    {
        private static readonly HttpClient client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();

        static MovieController()
        {
            // set up the base url address
            client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:44387/api/");
        }

        // GET: Movie/List
        public ActionResult List()
        {
            //communicate with ClientData api to retrieve a list of Clients
            //curl https://localhost:44387/api/Moviedata/listMovies


            string url = "Moviedata/listMovies";
            HttpResponseMessage response = client.GetAsync(url).Result;

            //Debug.WriteLine(response.StatusCode);

            IEnumerable<MovieDto> Movies = response.Content.ReadAsAsync<IEnumerable<MovieDto>>().Result;

            //Debug.WriteLine(Movies.Count());

            // return to the 'Movies' view page
            return View(Movies);
        }

        // GET: Movie/Details/5
        public ActionResult Details(int id)
        {
            DetailsMovie ViewModel = new DetailsMovie();

            //communicate with Moviedata api to retrieve one specific Movie
            //curl https://localhost:44387/api/Moviedata/findMovie/{id}

            string url = "Moviedata/findMovie/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            //Debug.WriteLine("The response code is ");
            //Debug.WriteLine(response.StatusCode);

            MovieDto SelectedMovie = response.Content.ReadAsAsync<MovieDto>().Result;
            //Debug.WriteLine("Movie received : ");
            //Debug.WriteLine(SelectedMovie.MovieTitle);

            // set up a ViewModel to show the relationship between movies and clients
            ViewModel.SelectedMovie = SelectedMovie;

            //show associated Clients with this Movie via ViewModel
            url = "clientdata/listclientsformovie/" + id;
            response = client.GetAsync(url).Result;
            IEnumerable<ClientDto> LinkedClients = response.Content.ReadAsAsync<IEnumerable<ClientDto>>().Result;

            ViewModel.LinkedClients = LinkedClients;

            // show unlinked Clients to this movie
            url = "clientdata/listclientsnotinterestedmovie/" + id;
            response = client.GetAsync(url).Result;
            IEnumerable<ClientDto> AvailableClients = response.Content.ReadAsAsync<IEnumerable<ClientDto>>().Result;

            ViewModel.AvailableClients = AvailableClients;

            return View(ViewModel);
        }


        //POST: Movie/Associate/{Movieid}
        [HttpPost]
        public ActionResult Associate(int id, int ClientID)
        {
            //Debug.WriteLine("Associate Movie :" + id + " with Client " + ClientID);

            //call api to link Movie with Client
            string url = "moviedata/associatemoviewithclient/" + id + "/" + ClientID;
            HttpContent content = new StringContent("");
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            return RedirectToAction("Details/" + id);
        }


        //Get: Movie/UnAssociate/{id}?ClientID={ClientID}
        [HttpGet]
        public ActionResult UnAssociate(int id, int ClientID)
        {
            //Debug.WriteLine("Unassociate Movie :" + id + " with Client: " + ClientID);

            //call api to link Movie with Client
            string url = "moviedata/unassociatemoviewithclient/" + id + "/" + ClientID;
            HttpContent content = new StringContent("");
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            return RedirectToAction("Details/" + id);
        }


        public ActionResult Error()
        {
            // error view page
            return View();
        }

        
        public ActionResult New()
        {
            //show a list of genres
            //GET api/Genredata/listGenre

            string url = "genredata/listgenre";
            HttpResponseMessage response = client.GetAsync(url).Result;
            IEnumerable<GenreDto> GenreOptions = response.Content.ReadAsAsync<IEnumerable<GenreDto>>().Result;

            return View(GenreOptions);
        }

        // POST: Movie/Create
        [HttpPost]
        public ActionResult Create(Movie Movie)
        {
            //Debug.WriteLine("json payload:");
            //Debug.WriteLine(Movie.MovieName);
            //add a new movie
            //e.g. curl -H "Content-Type:application/json" -d @Movie.json https://localhost:44387/api/Moviedata/addMovie 
            string url = "moviedata/addmovie";

            // json
            string jsonpayload = jss.Serialize(Movie);
            //Debug.WriteLine(jsonpayload);

            HttpContent content = new StringContent(jsonpayload);
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

        // GET: Movie/Edit/5
        public ActionResult Edit(int id)
        {
            UpdateMovie ViewModel = new UpdateMovie();

            //the existing Movie information
            string url = "moviedata/findmovie/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            MovieDto SelectedMovie = response.Content.ReadAsAsync<MovieDto>().Result;
            ViewModel.SelectedMovie = SelectedMovie;

            // all genres to choose from when updating this Movie
            // the existing Movie information
            url = "genredata/listgenre/";
            response = client.GetAsync(url).Result;
            IEnumerable<GenreDto> GenreOptions = response.Content.ReadAsAsync<IEnumerable<GenreDto>>().Result;

            ViewModel.GenreOptions = GenreOptions;

            return View(ViewModel);
        }


        // POST: Movie/Update/5
        /// <summary>
        /// Add upload movie picture funtion
        /// </summary>
        /// <param name="id"></param>
        /// <param name="Movie"></param>
        /// <param name="MoviePic"></param>
        /// <returns>
        /// Updated movies information and redirect to the Movie List page
        /// User can update the movie without uploading a picture (optional)
        /// </returns>
        [HttpPost]
        public ActionResult Update(int id, Movie Movie, HttpPostedFileBase MoviePic)
        {
            // upload movie pictures method
            // add a feature to uplaod image file to the server using POST request(in the Update function)
            string url = "moviedata/updatemovie/" + id;
            string jsonpayload = jss.Serialize(Movie);
            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;
            Debug.WriteLine(content);


            //server response is OK, and we have movie picture data(file exists)
            if (response.IsSuccessStatusCode && MoviePic != null)
            {
                //Seperate request for updating the movie picture (when user update movies without providing pictures) 
                //Debug.WriteLine("Update picture");

                //set up picture url
                url = "MovieData/UploadMoviePic/" + id;
                Debug.WriteLine("Received picture "+ MoviePic.FileName);

                MultipartFormDataContent requestcontent = new MultipartFormDataContent();
                HttpContent imagecontent = new StreamContent(MoviePic.InputStream);
                requestcontent.Add(imagecontent, "MoviePic", MoviePic.FileName);
                response = client.PostAsync(url, requestcontent).Result;

                return RedirectToAction("List");
            }
            else if (response.IsSuccessStatusCode)
            {
                //server response is OK, but no picture uploaded(upload picture is a seperate add-on feature)
                return RedirectToAction("List");
            }
            else
            {
                
                return RedirectToAction("Error");
            }
        }

        // GET: Movie/Delete/5
        public ActionResult DeleteConfirm(int id)
        {
            string url = "moviedata/findmovie/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            MovieDto selectedMovie = response.Content.ReadAsAsync<MovieDto>().Result;
            return View(selectedMovie);
        }

        // POST: Movie/Delete/5
        [HttpPost]
        public ActionResult Delete(int id)
        {
            string url = "moviedata/deletemovie/" + id;
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
    }
}
