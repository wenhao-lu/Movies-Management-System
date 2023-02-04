using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Movies_Management_System.Models
{
    public class Genre
    {
        [Key]
        public int GenreID { get; set; }

        public string GenreName { get; set; }

    }
    public class GenreDto
    {
        public int GenreID { get; set; }
        public string GenreName { get; set; }
    }
}