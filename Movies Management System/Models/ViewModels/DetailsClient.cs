using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Movies_Management_System.Models.ViewModels
{
    public class DetailsClient
    {
        public ClientDto SelectedClient { get; set; }
        public IEnumerable<MovieDto> KeptMovies { get; set; }
    }
}