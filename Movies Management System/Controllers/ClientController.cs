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

        // set up CRUD functions (and others) for Client  
        static ClientController()
        {
            // set up the base url address
            client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:44387/api/");
        }

        // GET: Client/List
        public ActionResult List()
        {
            // communicate with ClientData api to retrieve a list of Clients
            //e.g. curl https://localhost:44387/api/clientdata/listclients

            string url = "clientdata/listclients";
            HttpResponseMessage response = client.GetAsync(url).Result;

            //Debug.WriteLine(response.StatusCode);

            IEnumerable<ClientDto> Clients = response.Content.ReadAsAsync<IEnumerable<ClientDto>>().Result;
            //Debug.WriteLine(Clients.Count());

            // return to the 'Clients' view page
            return View(Clients);
        }

        // GET: Client/Details/5
        public ActionResult Details(int id)
        {
            DetailsClient ViewModel = new DetailsClient();

            //communicate with ClientData api to retrieve one Client
            //e.g. curl https://localhost:44387/api/Clientdata/findClient/{id}

            string url = "clientdata/findclient/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            //Debug.WriteLine(response.StatusCode);

            ClientDto SelectedClient = response.Content.ReadAsAsync<ClientDto>().Result;

            //Debug.WriteLine(SelectedClient.ClientName);

            ViewModel.SelectedClient = SelectedClient;

            //show all movies loved by this Client (client's watchlist)
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
            // add a new client via POST
            //Debug.WriteLine("the json payload is :");
            //Debug.WriteLine(Client.ClientName);
            //curl -H "Content-Type:application/json" -d @Client.json https://localhost:44387/api/clientdata/addclient 
            string url = "clientdata/addclient";

            string jsonpayload = jss.Serialize(Client);
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

        // GET: Client/Edit/5
        public ActionResult Edit(int id)
        {
            // choose a specific client ready to edit
            string url = "clientdata/findclient/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            ClientDto selectedClient = response.Content.ReadAsAsync<ClientDto>().Result;
            return View(selectedClient);
        }

        // POST: Client/Update/5
        [HttpPost]
        public ActionResult Update(int id, Client Client)
        {
            // update a specific client
            string url = "clientdata/updateclient/" + id;
            string jsonpayload = jss.Serialize(Client);

            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;
            //Debug.WriteLine(content);

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
            // client delete confirm
            string url = "clientdata/findclient/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            ClientDto selectedClient = response.Content.ReadAsAsync<ClientDto>().Result;

            return View(selectedClient);
        }

        // POST: Client/Delete/5
        [HttpPost]
        public ActionResult Delete(int id)
        {
            // delete a specific client
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
