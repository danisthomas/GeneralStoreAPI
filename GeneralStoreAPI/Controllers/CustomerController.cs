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
    public class CustomerController : ApiController
    {
        private readonly ApplicationDbContext _context = new ApplicationDbContext();

        //Post(Create)

        //api/customer
        [HttpPost]
        public async Task<IHttpActionResult> CreateCustomer ([FromBody] Customer model)
        {
            if(model is null)
            {
                return BadRequest("Your request body cannot be empty.");
            }
            //model is valid
            if (ModelState.IsValid)
            {
                //store model in database
                _context.Customers.Add(model);

                //save changes

                int changeCount = await _context.SaveChangesAsync();
                return Ok("Your Customer was created!");
            }
            //model is not valid
            return BadRequest(ModelState);

        }

        //Get All Customers
        //api/Customer
        [HttpGet]
        public async Task<IHttpActionResult> GetAllCustomers()
        {
            List<Customer> customers = await _context.Customers.ToListAsync();
            return Ok(customers);
        }

        //Get Customer by Id
        //api/customer/{id}
        [HttpGet]
        public async Task<IHttpActionResult> GetCustomerById ([FromUri] int id)
        {
            Customer customer = await _context.Customers.FindAsync(id);
             if(customer != null)
            {
                return Ok(customer);
            }
            return NotFound();
        }

        //Put(Update)
        //api/customer/{id}
        [HttpPut]
        public async Task<IHttpActionResult> UpdateCustomer ([FromUri] int id, Customer updatedCustomer)
        {
            //check if id's match
            if(id != updatedCustomer?.Id)
            {
                return BadRequest("Id's do not match.");

            }
            //check modelstate
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //find customer in database
            Customer customer = await _context.Customers.FindAsync(id);

            //if customer does not exist
            if(customer is null)
            {
                return NotFound();

            }

            //if customer exists, update
            customer.FirstName = updatedCustomer.FirstName;
            customer.LastName = updatedCustomer.LastName;


            //save changes
            await _context.SaveChangesAsync();
            return Ok("The Customer has been updated.");
        }

        //Delete(delete)
        [HttpDelete]

        public async Task<IHttpActionResult> DeleteCustomer([FromUri] int id)
        {
            Customer customer = await _context.Customers.FindAsync(id);

            if(customer is null)
            {
                return NotFound();
            }

            _context.Customers.Remove(customer);

            if(await _context.SaveChangesAsync() == 1)
            {
                return Ok("The Customer was deleted.");
            }

            return InternalServerError();
        }
    }
}
