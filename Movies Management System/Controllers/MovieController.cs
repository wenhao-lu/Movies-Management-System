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
    public class MovieController : Controller
    {
        private static readonly HttpClient client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();

        static MovieController()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:44387/api/");
        }

        // GET: Movie/List
        public ActionResult List()
        {
            //objective: communicate with our Movie data api to retrieve a list of Movies
            //curl https://localhost:44387/api/Moviedata/listMovies


            string url = "Moviedata/listMovies";
            HttpResponseMessage response = client.GetAsync(url).Result;

            //Debug.WriteLine("The response code is ");
            //Debug.WriteLine(response.StatusCode);

            IEnumerable<MovieDto> Movies = response.Content.ReadAsAsync<IEnumerable<MovieDto>>().Result;
            //Debug.WriteLine("Number of Movies received : ");
            //Debug.WriteLine(Movies.Count());


            return View(Movies);
        }

        // GET: Movie/Details/5
        public ActionResult Details(int id)
        {
            DetailsMovie ViewModel = new DetailsMovie();

            //objective: communicate with our Movie data api to retrieve one Movie
            //curl https://localhost:44387/api/Moviedata/findMovie/{id}

            string url = "Moviedata/findMovie/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            Debug.WriteLine("The response code is ");
            Debug.WriteLine(response.StatusCode);

            MovieDto SelectedMovie = response.Content.ReadAsAsync<MovieDto>().Result;
            Debug.WriteLine("Movie received : ");
            Debug.WriteLine(SelectedMovie.MovieTitle);

            ViewModel.SelectedMovie = SelectedMovie;

            //show associated Clients with this Movie
            url = "clientdata/listclientsformovie/" + id;
            response = client.GetAsync(url).Result;
            IEnumerable<ClientDto> ResponsibleClients = response.Content.ReadAsAsync<IEnumerable<ClientDto>>().Result;

            ViewModel.ResponsibleClients = ResponsibleClients;

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
            Debug.WriteLine("Attempting to associate Movie :" + id + " with Client " + ClientID);

            //call our api to associate Movie with Client
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
            Debug.WriteLine("Attempting to unassociate Movie :" + id + " with Client: " + ClientID);

            //call our api to associate Movie with Client
            string url = "moviedata/unassociatemoviewithclient/" + id + "/" + ClientID;
            HttpContent content = new StringContent("");
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            return RedirectToAction("Details/" + id);
        }


        public ActionResult Error()
        {

            return View();
        }

        // GET: Movie/New
        public ActionResult New()
        {
            //information about all Genre in the system.
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
            Debug.WriteLine("the json payload is :");
            //Debug.WriteLine(Movie.MovieName);
            //objective: add a new Movie into our system using the API
            //curl -H "Content-Type:application/json" -d @Movie.json https://localhost:44387/api/Moviedata/addMovie 
            string url = "moviedata/addmovie";


            string jsonpayload = jss.Serialize(Movie);
            Debug.WriteLine(jsonpayload);

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

            // all Genre to choose from when updating this Movie
            //the existing Movie information
            url = "genredata/listgenre/";
            response = client.GetAsync(url).Result;
            IEnumerable<GenreDto> GenreOptions = response.Content.ReadAsAsync<IEnumerable<GenreDto>>().Result;

            ViewModel.GenreOptions = GenreOptions;

            return View(ViewModel);
        }

        // POST: Movie/Update/5
        [HttpPost]
        public ActionResult Update(int id, Movie Movie)
        {

            string url = "moviedata/updatemovie/" + id;
            string jsonpayload = jss.Serialize(Movie);
            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;
            Debug.WriteLine(content);
            if (response.IsSuccessStatusCode)
            {
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
