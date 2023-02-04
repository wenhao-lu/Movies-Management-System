using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Movies_Management_System.Models
{
    public class Client
    {
        [Key]
        public int ClientID { get; set; }
        public string ClientName { get; set; }
        public string ClientLocation { get; set; }

        // <Movie>-<Client>  ==  M-M 
        public ICollection<Movie> Movies { get; set; }

    }

    public class ClientDto
    {
        public int ClientID { get; set; }
        public string ClientName { get; set; }
        public string ClientLocation { get; set; }


    }
}