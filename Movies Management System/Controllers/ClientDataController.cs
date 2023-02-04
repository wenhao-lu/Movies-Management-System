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
    public class ClientDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// Returns all Clients in the system.
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: all Clients in the database, including their associated genre.
        /// </returns>
        /// <example>
        /// GET: api/ClientData/ListClients
        /// </example>
        [HttpGet]
        [ResponseType(typeof(ClientDto))]
        public IHttpActionResult ListClients()
        {
            List<Client> Clients = db.Clients.ToList();
            List<ClientDto> ClientDtos = new List<ClientDto>();

            Clients.ForEach(k => ClientDtos.Add(new ClientDto()
            {
                ClientID = k.ClientID,
                ClientName = k.ClientName,
                ClientLocation = k.ClientLocation
            }));

            return Ok(ClientDtos);
        }

        /// <summary>
        /// Returns all Clients in the system associated with a particular movie.
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: all Clients in the database taking care of a particular movie
        /// </returns>
        /// <param name="id">Movie Primary Key</param>
        /// <example>
        /// GET: api/ClientData/ListClientsForMovie/1
        /// </example>
        [HttpGet]
        [ResponseType(typeof(ClientDto))]
        public IHttpActionResult ListClientsForMovie(int id)
        {
            List<Client> Clients = db.Clients.Where(
                k => k.Movies.Any(
                    a => a.MovieID == id)
                ).ToList();
            List<ClientDto> ClientDtos = new List<ClientDto>();

            Clients.ForEach(k => ClientDtos.Add(new ClientDto()
            {
                ClientID = k.ClientID,
                ClientName = k.ClientName,
                ClientLocation = k.ClientLocation
            }));

            return Ok(ClientDtos);
        }


        /// <summary>
        /// Returns Clients in the system not caring for a particular Movie.
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: all Clients in the database not taking care of a particular Movie
        /// </returns>
        /// <param name="id">Movie Primary Key</param>
        /// <example>
        /// GET: api/ClientData/ListClientsNotCaringForMovie/1
        /// </example>
        [HttpGet]
        [ResponseType(typeof(ClientDto))]
        public IHttpActionResult ListClientsNotInterestedMovie(int id)
        {
            List<Client> Clients = db.Clients.Where(
                k => !k.Movies.Any(
                    a => a.MovieID == id)
                ).ToList();
            List<ClientDto> ClientDtos = new List<ClientDto>();

            Clients.ForEach(k => ClientDtos.Add(new ClientDto()
            {
                ClientID = k.ClientID,
                ClientName = k.ClientName,
                ClientLocation = k.ClientLocation
            }));

            return Ok(ClientDtos);
        }

        /// <summary>
        /// Returns all Clients in the system.
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: An Client in the system matching up to the Client ID primary key
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <param name="id">The primary key of the Client</param>
        /// <example>
        /// GET: api/ClientData/FindClient/5
        /// </example>
        [ResponseType(typeof(ClientDto))]
        [HttpGet]
        public IHttpActionResult FindClient(int id)
        {
            Client Client = db.Clients.Find(id);
            ClientDto ClientDto = new ClientDto()
            {
                ClientID = Client.ClientID,
                ClientName = Client.ClientName,
                ClientLocation = Client.ClientLocation
            };
            if (Client == null)
            {
                return NotFound();
            }

            return Ok(ClientDto);
        }

        /// <summary>
        /// Updates a particular Client in the system with POST Data input
        /// </summary>
        /// <param name="id">Represents the Client ID primary key</param>
        /// <param name="Client">JSON FORM DATA of an Client</param>
        /// <returns>
        /// HEADER: 204 (Success, No Content Response)
        /// or
        /// HEADER: 400 (Bad Request)
        /// or
        /// HEADER: 404 (Not Found)
        /// </returns>
        /// <example>
        /// POST: api/ClientData/UpdateClient/5
        /// FORM DATA: Client JSON Object
        /// </example>
        [ResponseType(typeof(void))]
        [HttpPost]
        public IHttpActionResult UpdateClient(int id, Client Client)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != Client.ClientID)
            {

                return BadRequest();
            }

            db.Entry(Client).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClientExists(id))
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
        /// Adds an Client to the system
        /// </summary>
        /// <param name="Client">JSON FORM DATA of an Client</param>
        /// <returns>
        /// HEADER: 201 (Created)
        /// CONTENT: Client ID, Client Data
        /// or
        /// HEADER: 400 (Bad Request)
        /// </returns>
        /// <example>
        /// POST: api/ClientData/AddClient
        /// FORM DATA: Client JSON Object
        /// </example>
        [ResponseType(typeof(Client))]
        [HttpPost]
        public IHttpActionResult AddClient(Client Client)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Clients.Add(Client);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = Client.ClientID }, Client);
        }

        /// <summary>
        /// Deletes an Client from the system by it's ID.
        /// </summary>
        /// <param name="id">The primary key of the Client</param>
        /// <returns>
        /// HEADER: 200 (OK)
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <example>
        /// POST: api/ClientData/DeleteClient/5
        /// FORM DATA: (empty)
        /// </example>
        [ResponseType(typeof(Client))]
        [HttpPost]
        public IHttpActionResult DeleteClient(int id)
        {
            Client Client = db.Clients.Find(id);
            if (Client == null)
            {
                return NotFound();
            }

            db.Clients.Remove(Client);
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

        private bool ClientExists(int id)
        {
            return db.Clients.Count(e => e.ClientID == id) > 0;
        }
    }
}