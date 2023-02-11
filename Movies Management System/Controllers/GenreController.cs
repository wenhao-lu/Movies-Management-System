using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using System.Diagnostics;
using Movies_Management_System.Models;
using Movies_Management_System.Models.ViewModels;
using System.Web.Script.Serialization;

namespace Movies_Management_System.Controllers
{
    public class GenreController : Controller
    {
        private static readonly HttpClient client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();

        static GenreController()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:44387/api/");
        }

        // GET: Genre/List
        public ActionResult List()
        {
            //communicate with GenreData api to retrieve a list of Genres
            //e.g. curl https://localhost:44387/api/genredata/listgenres

            string url = "genredata/listgenre";
            HttpResponseMessage response = client.GetAsync(url).Result;

            //Debug.WriteLine(response.StatusCode);

            IEnumerable<GenreDto> Genres = response.Content.ReadAsAsync<IEnumerable<GenreDto>>().Result;

            //Debug.WriteLine(Genres.Count());

            // return to the view page of /genre/list  
            return View(Genres);
        }

        // GET: Genre/Details/5
        public ActionResult Details(int id)
        {
            //communicate with GenreData api to retrieve one specific genre
            //e.g. curl https://localhost:44387/api/genredata/findgenre/{id}
            // create a ViewModel to show the relationship between Genres and Movies
            DetailsGenre ViewModel = new DetailsGenre();

            string url = "genredata/findgenre/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            //Debug.WriteLine(response.StatusCode);

            GenreDto SelectedGenre = response.Content.ReadAsAsync<GenreDto>().Result;

            //Debug.WriteLine(SelectedGenre.GenreName);

            ViewModel.SelectedGenre = SelectedGenre;

            //showcase information about movies related to this perticular genre
            //many movies can be related to one particular genre, 1--M relationship
            url = "moviedata/listmoviesforgenre/" + id;
            response = client.GetAsync(url).Result;
            IEnumerable<MovieDto> RelatedMovies = response.Content.ReadAsAsync<IEnumerable<MovieDto>>().Result;

            ViewModel.RelatedMovies = RelatedMovies;

            return View(ViewModel);
        }

        public ActionResult Error()
        {

            return View();
        }

        // GET: Genre/New
        public ActionResult New()
        {
            return View();
        }

        // POST: Genre/Create
        [HttpPost]
        public ActionResult Create(Genre Genre)
        {
            //Debug.WriteLine("the json payload is :");
            //Debug.WriteLine(Genre.GenreName);

            //add a new Genre into our system
            //e.g. curl -H "Content-Type:application/json" -d @Genre.json https://localhost:44387/api/genredata/addgenre 
            string url = "genredata/addgenre";

            string jsonpayload = jss.Serialize(Genre);
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

        // GET: Genre/Edit/5
        public ActionResult Edit(int id)
        {
            string url = "genredata/findgenre/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            GenreDto selectedGenre = response.Content.ReadAsAsync<GenreDto>().Result;
            return View(selectedGenre);
        }

        // POST: Genre/Update/5
        [HttpPost]
        public ActionResult Update(int id, Genre Genre)
        {
            // update a specific genre
            string url = "genredata/updategenre/" + id;
            string jsonpayload = jss.Serialize(Genre);

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

        // GET: Genre/Delete/5
        public ActionResult DeleteConfirm(int id)
        {
            // confirmation of a genre deletion
            string url = "genredata/findgenre/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            GenreDto selectedGenre = response.Content.ReadAsAsync<GenreDto>().Result;
            return View(selectedGenre);
        }

        // POST: Genre/Delete/5
        [HttpPost]
        public ActionResult Delete(int id)
        {   
            // remove a specific genre from the system via POST
            string url = "genredata/deletegenre/" + id;
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
