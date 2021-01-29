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
    public class TransactionController : ApiController
    {
        private readonly ApplicationDbContext _context = new ApplicationDbContext();

        //Create
        //api/transaction
        [HttpPost]

        public async Task<IHttpActionResult> CreateTransaction([FromBody] Transaction model)
        {
            Product product = await _context.Products.FindAsync(model.ProductSKU);
            Customer customer = await _context.Customers.FindAsync(model.CustomerId);

            if (model is null)
            {
                return BadRequest("Your request body cannot be empty.");

            }
            if (ModelState.IsValid)
            {
                //verify if product is in stock
                //verify number in inventory matches transaction
                if (product.NumberInInventory >= model.ItemCount)
                {

                    if (model.CustomerId == customer.Id & model.ProductSKU == product.SKU)
                    {
                        _context.Transactions.Add(model);

                        
                      
                        //remove product from inventory after completion
                        product.NumberInInventory = product.NumberInInventory - model.ItemCount;

                        int changeCount = await _context.SaveChangesAsync();


                        return Ok("Your Transaction was completed.");

                    }
                    return BadRequest("Customer or Product Id's were not found.");

                }
            }
            return BadRequest(ModelState);
  
        }

        //Get all
        //api/transaction
        [HttpGet]
        public async Task<IHttpActionResult> GetAllTransactions()
        {
            List<Transaction> transactions = await _context.Transactions.ToListAsync();

            return Ok(transactions);

        }

        //Get Transactions by Customer Id
        //api/transaction/{id}
        [HttpGet]
        [Route ("api/Transaction/Customer/{id}")]
        public async Task<IHttpActionResult> GetTransacitonByCustId([FromUri] int id, Transaction transaction)
        {
            Customer customer = await _context.Customers.FindAsync(id);
            //Transaction transactionId = await _context.Transactions.FindAsync(customer.Id);

            List<Transaction> transactions = await _context.Transactions.ToListAsync();
            List<Transaction> tranList = new List<Transaction>();
            if (customer != null)
            {
                foreach (var item in transactions)
                {
                    if (item.CustomerId == id)
                    {
                        tranList.Add(item);
                    }
                    // return Ok(tranList);
                }
                if (customer.Id == id)
                {
                    return Ok(tranList);
                }
            }
            return NotFound();
        }

        //Get Transaction by Transaction Id
        //api/transaction/{id}

        [HttpGet]
        public async Task<IHttpActionResult> GetTransactionByTransId([FromUri] int id)
        {
            Transaction transaction = await _context.Transactions.FindAsync(id);

            if(transaction != null)
            {
                return Ok(transaction);
            }
            return NotFound();
        }

        //Update Transaction
        //api/transaction/{id}

        [HttpPut]
        public async Task<IHttpActionResult> UpdateTransaction([FromUri] int id, [FromBody] Transaction updatedTransaction)
        {
            //check if id's match

            if(id != updatedTransaction?.Id)
            {
                return BadRequest("Id's do not match.");
            }
            //check modelstate
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);

            }

            //find transaction
            Transaction transaction = await _context.Transactions.FindAsync(id);
            Product product = await _context.Products.FindAsync(transaction.ProductSKU);

            //transaction doesn't exist
            if(transaction is null)
            {
                return NotFound();
            }
            if (updatedTransaction.CustomerId != null)
            {
                transaction.CustomerId = updatedTransaction.CustomerId;
            }
            if (updatedTransaction.DateOfTransaction != null)
            {
                transaction.DateOfTransaction = updatedTransaction.DateOfTransaction;
            }
           
           
            if (transaction.ProductSKU == updatedTransaction.ProductSKU & updatedTransaction.ProductSKU!=null)
            {
                if (transaction.ItemCount > updatedTransaction.ItemCount)
                {
                    product.NumberInInventory = product.NumberInInventory + (transaction.ItemCount - updatedTransaction.ItemCount);
                }
                else product.NumberInInventory = product.NumberInInventory - (updatedTransaction.ItemCount - transaction.ItemCount);
            }
            if(transaction.ProductSKU != updatedTransaction.ProductSKU & updatedTransaction.ProductSKU!=null)
            {
                Product updatedProduct = await _context.Products.FindAsync(updatedTransaction.ProductSKU);
                if (transaction.ItemCount > updatedTransaction.ItemCount)
                {
                    updatedProduct.NumberInInventory = updatedProduct.NumberInInventory + (transaction.ItemCount - updatedTransaction.ItemCount);
                    product.NumberInInventory = product.NumberInInventory - (transaction.ItemCount - updatedTransaction.ItemCount);
                }
                else updatedProduct.NumberInInventory = updatedProduct.NumberInInventory - (updatedTransaction.ItemCount - transaction.ItemCount); product.NumberInInventory = product.NumberInInventory + (updatedTransaction.ItemCount - transaction.ItemCount);
                      

            }
            if (updatedTransaction.ItemCount != null)
            {
                transaction.ItemCount = updatedTransaction.ItemCount;
            }
            if (updatedTransaction.ProductSKU != null)
            {
                transaction.ProductSKU = updatedTransaction.ProductSKU;
            }
           
            await _context.SaveChangesAsync();
            return Ok("Transaction was updated.");


        }

        //delete
        //api/transaction/{id}
        [HttpDelete]

        public async Task<IHttpActionResult> DeleteTransaction([FromUri] int id)
        {
            Transaction transaction = await _context.Transactions.FindAsync(id);
            Product product =  await _context.Products.FindAsync(transaction.ProductSKU);

            if (transaction is null)
                return NotFound();

           
            product.NumberInInventory = transaction.ItemCount + product.NumberInInventory;

            _context.Transactions.Remove(transaction);

            if(await _context.SaveChangesAsync()== 1)
            {
                return Ok("The Transaction successfully deleted.");

            }
            return InternalServerError();
        }
    }
}
