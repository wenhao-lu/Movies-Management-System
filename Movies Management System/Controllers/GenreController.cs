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
            //objective: communicate with our Genre data api to retrieve a list of Genres
            //curl https://localhost:44387/api/genredata/listgenres


            string url = "genredata/listgenre";
            HttpResponseMessage response = client.GetAsync(url).Result;

            //Debug.WriteLine("The response code is ");
            //Debug.WriteLine(response.StatusCode);

            IEnumerable<GenreDto> Genre = response.Content.ReadAsAsync<IEnumerable<GenreDto>>().Result;
            //Debug.WriteLine("Number of Genres received : ");
            //Debug.WriteLine(Genres.Count());


            return View(Genre);
        }

        // GET: Genre/Details/5
        public ActionResult Details(int id)
        {
            //objective: communicate with our genre data api to retrieve one genre
            //curl https://localhost:44387/api/genredata/findgenre/{id}

            DetailsGenre ViewModel = new DetailsGenre();

            string url = "genredata/findgenre/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            Debug.WriteLine("The response code is ");
            Debug.WriteLine(response.StatusCode);

            GenreDto SelectedGenre = response.Content.ReadAsAsync<GenreDto>().Result;
            Debug.WriteLine("Genre received : ");
            Debug.WriteLine(SelectedGenre.GenreName);

            ViewModel.SelectedGenre = SelectedGenre;

            //showcase information about movies related to this Genre
            //send a request to gather information about movies related to a particular genre ID
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
            Debug.WriteLine("the json payload is :");
            //Debug.WriteLine(Genre.GenreName);
            //objective: add a new Genre into our system using the API
            //curl -H "Content-Type:application/json" -d @Genre.json https://localhost:44387/api/genredata/addgenre 
            string url = "genredata/addgenre";


            string jsonpayload = jss.Serialize(Genre);
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

            string url = "genredata/updategenre/" + id;
            string jsonpayload = jss.Serialize(Genre);
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

        // GET: Genre/Delete/5
        public ActionResult DeleteConfirm(int id)
        {
            string url = "genredata/findgenre/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            GenreDto selectedGenre = response.Content.ReadAsAsync<GenreDto>().Result;
            return View(selectedGenre);
        }

        // POST: Genre/Delete/5
        [HttpPost]
        public ActionResult Delete(int id)
        {
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
