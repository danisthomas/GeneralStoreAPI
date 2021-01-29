using GeneralStoreAPI.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace GeneralStoreAPI.Controllers
{
    public class ProductController : ApiController
    {
        private readonly ApplicationDbContext _context = new ApplicationDbContext();

        //Post
        //api/Product
        [HttpPost]
        public async Task<IHttpActionResult> CreateProduct([FromBody] Product model)
        {
            if (model is null)
            {
                return BadRequest("Your request body cannot be empty.");

            }

            if (ModelState.IsValid)
            {
                _context.Products.Add(model);

                int changeCount = await _context.SaveChangesAsync();

                return Ok("Your Product was created.");
            }
            return BadRequest(ModelState);
        }

        //Get all
        //api/product
        [HttpGet]

        public async Task<IHttpActionResult> GetAllProducts()
        {
            List<Product> products = await _context.Products.ToListAsync();
            return Ok(products);
        }

        //Get by Id
        //api/products/{id}
        [HttpGet]

        public async Task<IHttpActionResult> GetProductById([FromUri] int id)
        {
            Product product = await _context.Products.FindAsync(id);

            if(product != null)
            {
                return Ok(product);
            }
            return NotFound();
        }

        //Put
        //api/product/{id}
        [HttpPut]
         
        public async Task<IHttpActionResult> UpdateProductById([FromUri] string id, [FromBody] Product updatedProduct)
        {
            //check if Id's match
            if(id != updatedProduct.SKU)
            {
                return BadRequest("SKU's don't match.");

            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            //find product in database
            Product product = await _context.Products.FindAsync(id);

            //if product doesn't exist
            if(product is null)
            {
                return NotFound();
            }

            //update
            product.ProductName = updatedProduct.ProductName;
            product.Cost = updatedProduct.Cost;
            product.NumberInInventory = updatedProduct.NumberInInventory;

            await _context.SaveChangesAsync();
            return Ok("Product was update.");
        }

        public async Task<IHttpActionResult> DeleteProduct([FromUri] string id)
        {
            Product product = await _context.Products.FindAsync(id);

            if(product is null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);

            if(await _context.SaveChangesAsync() == 1)
            {
                return Ok("Product was deleted.");
            }

            return InternalServerError();
        }




    }
}
