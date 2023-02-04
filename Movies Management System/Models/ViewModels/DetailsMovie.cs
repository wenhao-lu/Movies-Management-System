using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Movies_Management_System.Models.ViewModels
{
    public class DetailsMovie
    {
        public MovieDto SelectedMovie { get; set; }
        public IEnumerable<ClientDto> ResponsibleClients { get; set; }

        public IEnumerable<ClientDto> AvailableClients { get; set; }
    }
}