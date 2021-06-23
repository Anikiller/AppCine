using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AppCine.Models;

namespace Cine.Data
{
    public class AppCineContext : DbContext
    {
        public AppCineContext (DbContextOptions<AppCineContext> options)
            : base(options)
        {
        }

        public DbSet<AppCine.Models.Pelicula> Pelicula { get; set; }
    }
}
