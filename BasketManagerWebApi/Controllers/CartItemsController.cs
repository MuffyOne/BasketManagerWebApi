using BasketManagerWebApi.Common.Models;
using BasketManagerWebApi.Enums;
using BasketManagerWebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BasketManagerWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartItemsController : ControllerBase
    {
        private readonly BasketContext _context;

        public CartItemsController(BasketContext context)
        {
            _context = context;
        }

        // GET: api/CartItems
        [HttpGet]
        public IEnumerable<BasketItem> GetCartProducts([FromQuery] int cartId = -1)
        {
            return _context.GetBasketItems(cartId);
        }

        // GET: api/CartItems/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCartItem([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var cartItem = await _context.BasketProducts.FindAsync(id);

            if (cartItem == null)
            {
                return NotFound();
            }

            return Ok(cartItem);
        }

        // PUT: api/CartItems/5
        [HttpPut("{id}")]
        public IActionResult PutCartItem([FromRoute] int id, [FromBody] BasketItem cartItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != cartItem.Id)
            {
                return BadRequest();
            }

            var result = _context.ModifyCartItem(id, cartItem);

            if (result == ProductInjuryResult.NotFound)
            {
                return NotFound(string.Format("The product id {0} was not found!", cartItem.ProductId));
            }
            else if (result == ProductInjuryResult.QuantityNotAvailable)
            {
                return NotFound(string.Format("{0} items of product {1} were not found in stock!", cartItem.Quantity, cartItem.ProductId));
            }

            return NoContent();
        }

        // POST: api/CartItems
        [HttpPost]
        public IActionResult PostCartItem([FromBody] BasketItem cartItem, [BindRequired][FromQuery] int cartId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ProductInjuryResult productInjuryResult = _context.CheckProduct(cartItem.ProductId, cartItem.Quantity);

            if (productInjuryResult == ProductInjuryResult.NotFound)
            {
                return NotFound(string.Format("The product id {0} was not found!", cartItem.ProductId));
            }
            else if (productInjuryResult == ProductInjuryResult.QuantityNotAvailable)
            {
                return NotFound(string.Format("{0} items of product {1} were not found in stock!", cartItem.Quantity, cartItem.ProductId));
            }
            _context.AddProduct(cartItem, cartId);

            return CreatedAtAction("GetCartItem", new { id = cartItem.Id }, cartItem);
        }

        // DELETE: api/CartItems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCartItem([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var cartItem = await _context.BasketProducts.FindAsync(id);
            if (cartItem == null)
            {
                return NotFound();
            }

            _context.BasketProducts.Remove(cartItem);
            await _context.SaveChangesAsync();

            return Ok(cartItem);
        }

        private bool CartItemExists(int id)
        {
            return _context.BasketProducts.Any(e => e.Id == id);
        }
    }
}