using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Movies_Management_System.Models.ViewModels
{
    public class DetailsGenre
    {
        //the Genre itself that we want to display
        public GenreDto SelectedGenre { get; set; }

        //all of the related movies to that particular genre
        public IEnumerable<MovieDto> RelatedMovies { get; set; }
    }
}