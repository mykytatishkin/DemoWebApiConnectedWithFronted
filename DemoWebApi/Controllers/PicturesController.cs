using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DemoWebApi.Data;
using DemoWebApi.Models;
using System.Drawing.Imaging;
using System.Drawing;

namespace DemoWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PicturesController : ControllerBase
    {
        private readonly BookContext _context;
        private readonly IWebHostEnvironment _env;

        public PicturesController(BookContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: api/Pictures
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Picture>>> GetPicture()
        {
          if (_context.Picture == null)
          {
              return NotFound();
          }
            return await _context.Picture.ToListAsync();
        }

        // GET: api/Pictures/5
        [HttpGet("{id}")]
        public async Task<ActionResult<List<Picture>>> GetPicture(int id)
        {
          if (_context.Picture == null)
          {
              return NotFound();
          }
            var pictures = await _context.Picture.Where(p => p.BookId == id).ToListAsync();


            if (pictures == null)
            {
                return NotFound();
            }

            return pictures;
        }

        // PUT: api/Pictures/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPicture(int id, Picture picture)
        {
            if (id != picture.Id)
            {
                return BadRequest();
            }

            _context.Entry(picture).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PictureExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Pictures
        [HttpPost]
        public async Task<ActionResult<Picture>> PostPicture(Picture picture)
        {
            if (_context.Picture == null)
            {
                return Problem("Entity set 'BookContext.Picture'  is null.");
            }
            //1. copy uploaded file to /images
            string fileName = DateTimeOffset.Now.Ticks.ToString() + ".jpg";
            string filePath = Path.Combine(_env.ContentRootPath, "images", fileName);
            byte[] imageBytes = Convert.FromBase64String(picture.ImageUrl);
            using (Image image = Image.FromStream(new MemoryStream(imageBytes)) )
            {
                image.Save(filePath, ImageFormat.Jpeg);
            }

            //2. set picture.ImageUrl = path to the directory images
            //picture.ImageUrl = "/images/" + fileName;
            _context.Picture.Add(picture);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPicture", new { id = picture.Id }, picture);
        }

        // DELETE: api/Pictures/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePicture(int id)
        {
            if (_context.Picture == null)
            {
                return NotFound();
            }
            var picture = await _context.Picture.FindAsync(id);
            if (picture == null)
            {
                return NotFound();
            }

            _context.Picture.Remove(picture);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PictureExists(int id)
        {
            return (_context.Picture?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
