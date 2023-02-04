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
    public class ClientController : Controller
    {
        private static readonly HttpClient client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();

        static ClientController()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:44387/api/");
        }

        // GET: Client/List
        public ActionResult List()
        {
            //objective: communicate with our Client data api to retrieve a list of Clients
            //curl https://localhost:44387/api/clientdata/listclients


            string url = "clientdata/listclients";
            HttpResponseMessage response = client.GetAsync(url).Result;

            //Debug.WriteLine("The response code is ");
            //Debug.WriteLine(response.StatusCode);

            IEnumerable<ClientDto> Clients = response.Content.ReadAsAsync<IEnumerable<ClientDto>>().Result;
            //Debug.WriteLine("Number of Clients received : ");
            //Debug.WriteLine(Clients.Count());


            return View(Clients);
        }

        // GET: Client/Details/5
        public ActionResult Details(int id)
        {
            DetailsClient ViewModel = new DetailsClient();

            //objective: communicate with our Client data api to retrieve one Client
            //curl https://localhost:44387/api/Clientdata/findClient/{id}

            string url = "clientdata/findclient/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            Debug.WriteLine("The response code is ");
            Debug.WriteLine(response.StatusCode);

            ClientDto SelectedClient = response.Content.ReadAsAsync<ClientDto>().Result;
            Debug.WriteLine("Client received : ");
            Debug.WriteLine(SelectedClient.ClientName);

            ViewModel.SelectedClient = SelectedClient;

            //show all movies under the care of this Client
            url = "moviedata/listmoviesforclient/" + id;
            response = client.GetAsync(url).Result;
            IEnumerable<MovieDto> KeptMovies = response.Content.ReadAsAsync<IEnumerable<MovieDto>>().Result;

            ViewModel.KeptMovies = KeptMovies;


            return View(ViewModel);
        }

        public ActionResult Error()
        {

            return View();
        }

        // GET: Client/New
        public ActionResult New()
        {
            return View();
        }

        // POST: Client/Create
        [HttpPost]
        public ActionResult Create(Client Client)
        {
            Debug.WriteLine("the json payload is :");
            //Debug.WriteLine(Client.ClientName);
            //objective: add a new Client into our system using the API
            //curl -H "Content-Type:application/json" -d @Client.json https://localhost:44387/api/clientdata/addclient 
            string url = "clientdata/addclient";


            string jsonpayload = jss.Serialize(Client);
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

        // GET: Client/Edit/5
        public ActionResult Edit(int id)
        {
            string url = "clientdata/findclient/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            ClientDto selectedClient = response.Content.ReadAsAsync<ClientDto>().Result;
            return View(selectedClient);
        }

        // POST: Client/Update/5
        [HttpPost]
        public ActionResult Update(int id, Client Client)
        {

            string url = "clientdata/updateclient/" + id;
            string jsonpayload = jss.Serialize(Client);
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

        // GET: Client/Delete/5
        public ActionResult DeleteConfirm(int id)
        {
            string url = "clientdata/findclient/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            ClientDto selectedClient = response.Content.ReadAsAsync<ClientDto>().Result;
            return View(selectedClient);
        }

        // POST: Client/Delete/5
        [HttpPost]
        public ActionResult Delete(int id)
        {
            string url = "clientdata/deleteclient/" + id;
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
