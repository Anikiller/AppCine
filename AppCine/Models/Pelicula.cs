using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AppCine.Models
{
    public class Pelicula
    {
        [Required]
        [Display(Name ="Código")]
        public int id { get; set; }
        [Required]
        [Display(Name = "Nombre")]
        public string nomPelicula { get; set; }
        [Required]
        [Display(Name = "Tipo")]
        public string tipoPelicula { get; set; }
        [Required]
        [Display(Name = "Reserva")]
        public int reserva { get; set; }
        public string foto { get; set; }
        [Required]
        [Display(Name = "Cupos")]
        public int cupos { get; set; }
        [Required]
        [Display(Name = " ")]
        public string nombreCine { get; set; }
    }
}
