using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AppCine.Models;
using Cine.Data;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace AppCine.Controllers
{
    public class PeliculasController : Controller
    {
        private readonly AppCineContext _context;
        private static string fotoAnterior = "";

        public PeliculasController(AppCineContext context)
        {
            _context = context;
        }

        // GET: Peliculas
        public async Task<IActionResult> Index()
        {
            return View(await _context.Pelicula.ToListAsync());
        }

        // GET: Peliculas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pelicula = await _context.Pelicula
                .FirstOrDefaultAsync(m => m.id == id);
            if (pelicula == null)
            {
                return NotFound();
            }

            return View(pelicula);
        }
        public async Task<IActionResult> ConsultaCine()
        {
            return View(await _context.Pelicula.ToListAsync());
        }//fin del método de consulta pelicula

        // GET: Peliculas/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Peliculas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(List<IFormFile>files,[Bind("id,nomPelicula,tipoPelicula,reserva,foto,cupos,nombreCine")] Pelicula pelicula)
        {

            if (ModelState.IsValid)
            {
                //variable para definir la ruta fisica donde se almacenan las fotos
                //se toma el directorio base donde se encuentra la aplicacion Web
                string rutaFisica = AppDomain.CurrentDomain.BaseDirectory;

                //se toma la ruta de la carpeta del proyecto
                string filePath = rutaFisica.Substring(0, rutaFisica.Length - 24);

                //indicar al filePath el folder de donde se guardará la foto
                filePath += @"wwwroot\imagenes\";

                //almacenar el nombre del archivo que escogio
                string fileName = "";

                //recorrer los archivos adjuntos dentro del formulario
                foreach (var formFile in files)
                {
                    //validar el tamaño del archivo, porque puede venir vacío
                    if (formFile.Length > 0)
                    {
                        //construir el nombre de la foto con el id especifico del producto  agregar
                        fileName = pelicula.id + "-" + formFile.FileName;
                        //aqui elimino los espacio en blanco dentro deol nombre de la foto
                        fileName = fileName.Replace(" ", "_");
                        //en la ruta fisica del proyecto agrego el nombre de la foto
                        filePath += fileName;

                        //habilitar la acción para copiar el archivo
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            //le indicamos en nuestra BD donde esta la foto
                            await formFile.CopyToAsync(stream);
                            //agrego al context mi carpeta dentro del proyecto(donde esta la foto) más el nombre de la foto
                            pelicula.foto = "/imagenes/" + fileName;
                        }// fin del using
                    }//fin del if Length
                }// fin del ciclo
                //datos por defecto que se van a almacenar
        
                //guardar el objeto producto dentro de mi contexto(Context-->conexion hacia la BD)
                _context.Add(pelicula);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));

            }// fin del ModelState

            return View(pelicula);
        }

        // GET: Peliculas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pelicula = await _context.Pelicula.FindAsync(id);
            if (pelicula == null)
            {
                return NotFound();
            }
            fotoAnterior = pelicula.foto;
            return View(pelicula);
        }

        // POST: Peliculas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,nomPelicula,tipoPelicula,reserva,foto,cupos,nombreCine")] Pelicula pelicula, List<IFormFile> files)
        {
            if (id != pelicula.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //Agregamos los datos por default al producto

                    //borro la ruta física
                    //borrado fisico
                    string filePath = this.borrarRutaFisica();
                    //indicamos el nombre de la foto anterior a eliminar
                    filePath += fotoAnterior;
                    //se borra la foto anterior
                    this.borrarFoto(filePath);
                    //indicamos la ruta donde se va aguardar nueva foto
                    filePath = this.rutaFisicaGuardar();
                    //variable que sirva para almacenar el nombre de la foto
                    string fileName = "";
                    //vamos a revisar si el formulario que esta en la web tiene fotos adjuntas
                    foreach (var item in files)
                    {
                        if (item.Length > 0)
                        {
                            //construir el nombre de la foto con el id especifico del producto  agregar
                            fileName = pelicula.id + "-" + item.FileName;
                            //aqui elimino los espacio en blanco dentro del nombre de la foto
                            fileName = fileName.Replace(" ", "_");
                            //en la ruta fisica del proyecto agrego el nombre de la foto
                            filePath += fileName;

                            //habilitar la acción para copiar el archivo
                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                //le indicamos en nuestra BD donde esta la foto
                                await item.CopyToAsync(stream);
                                //agrego al context mi carpeta dentro del proyecto(donde esta la foto) más el nombre de la foto
                                pelicula.foto = "/imagenes/" + fileName;
                            }// fin del using
                        }//fin del Length
                    }//fin del foreach
                    //guardar pelicula con todos los cambios
                    _context.Update(pelicula);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PeliculaExists(pelicula.id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                //name of
                return RedirectToAction(nameof(Index));
            }
            return View(pelicula);
        }
        private string borrarRutaFisica()
        {
            //variavle apra definir la ruta fisica
            string rutaFisica = AppDomain.CurrentDomain.BaseDirectory;
            //se toma la carpeta del proyecto
            string filePath = rutaFisica.Substring(0, rutaFisica.Length - 24);
            //como borramos debemos remplazar los caracteres \ oir /
            filePath = filePath.Replace(@"\", "/");
            //Indicar el  folder de productos donde se debe de borrar la foto
            filePath += @"wwwroot";
            //devolvemos la ruta donde se borra
            return filePath;
        }//fin borrar ruta fisica
        /// <summary>
        /// Método que borra el archivo de la foto
        /// </summary>
        /// <param name="pFileName"></param>
        private void borrarFoto(string pFileName)
        {
            System.IO.File.Delete(pFileName);
        }//fin del método borrar foto
         // GET: Productos/Delete/5
        /// <summary>
        /// este método guarda la ruta física de la foto
        /// </summary>
        /// <returns></returns>
        private string rutaFisicaGuardar()
        {
            //variable para definir la ruta fisica donde se almacenan las fotos
            //se toma el directorio base donde se encuentra la aplicacion Web
            string rutaFisica = AppDomain.CurrentDomain.BaseDirectory;

            //se toma la ruta de la carpeta del proyecto
            string filePath = rutaFisica.Substring(0, rutaFisica.Length - 24);

            //indicar al filePath el folder de donde se guardará la foto
            filePath = @"wwwroot\imagenes\";
            //indicamos la ruta donde se va aguardar nueva foto
            return filePath;

        }
        // POST: Peliculas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pelicula = await _context.Pelicula.FindAsync(id);
            _context.Pelicula.Remove(pelicula);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PeliculaExists(int id)
        {
            return _context.Pelicula.Any(e => e.id == id);
        }

        public async Task<IActionResult> Reservaciones()
        {
            return View(await _context.Pelicula.ToListAsync());
        }//fin del método de consulta Pelicula

        public async Task<IActionResult> EditarReservaciones(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pelicula = await _context.Pelicula.FindAsync(id);
            if (pelicula == null)
            {
                return NotFound();
            }
            fotoAnterior = pelicula.foto;
            return View(pelicula);
        }

        // POST: Peliculas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarReservaciones(int id, [Bind("id,nomPelicula,tipoPelicula,reserva,foto,cupos,nombreCine")] Pelicula pelicula, List<IFormFile> files)
        {
            if (id != pelicula.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(pelicula);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PeliculaExists(pelicula.id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                //name of
                return RedirectToAction(nameof(Index));
            }
            return View(pelicula);
        }
    }
}
