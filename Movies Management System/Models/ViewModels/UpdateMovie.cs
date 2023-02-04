using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Movies_Management_System.Models.ViewModels
{
    public class UpdateMovie
    {
        //This viewmodel is a class which stores information that we need to present to /Movie/Update/{}

        //the existing movie information

        public MovieDto SelectedMovie { get; set; }

        // all genres to choose from when updating this movie

        public IEnumerable<GenreDto> GenreOptions { get; set; }
    }
}