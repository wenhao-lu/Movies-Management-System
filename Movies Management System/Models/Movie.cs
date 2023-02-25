using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Movies_Management_System.Models
{
    public class Movie
    {
        [Key]
        public int MovieID { get; set; }
        public string MovieTitle { get; set; }
        public int PubYear { get; set; }
        public string Director { get; set; }
        public int Ratings { get; set; }

        //movie images uploaded data for tracking
        //images will be stored into /Content/Images/Movies/{id}.{extension}
        public bool MovieHasPic { get; set; }
        public string PicExtension { get; set; }


        // <Genre>-<Movie>  ==  1-M 
        [ForeignKey("Genre")]
        public int GenreID { get; set; }
        public virtual Genre Genre { get; set; }


        // <Movie>-<Client>  ==  M-M 
        public ICollection<Client> Clients { get; set; }

    }

    public class MovieDto
    {
        public int MovieID { get; set; }
        public string MovieTitle { get; set; }
        public int PubYear { get; set; }
        public string Director { get; set; }
        public int Ratings { get; set; }
        public int GenreID { get; set; }
        public string GenreName { get; set; }

        //movie images uploaded data for tracking
        //images will be stored into /Content/Images/Movies/{id}.{extension}
        public bool MovieHasPic { get; set; }
        public string PicExtension { get; set; }

    }
}