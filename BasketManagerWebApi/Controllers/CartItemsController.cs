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
        /// <summary>
        /// Returns all the products in all Baskets. If the BasketID is set returns all the products in that basket.
        /// </summary>
        /// <param name="basketId">The basket identifier of which you want to get the products. Optional</param>
        /// <returns>IEnumerable&lt;BasketItem&gt;.</returns>
        [HttpGet]
        public IEnumerable<BasketItem> GetBasketProducts([FromQuery] int basketId = -1)
        {
            return _context.GetBasketItems(basketId);
        }

        // GET: api/CartItems/5
        /// <summary>
        /// Gets the basket item with the specified Id.
        /// The id is coming from route
        /// </summary>
        /// <param name="id">The identifier of the item you want to retrieve.</param>
        /// <returns>Task&lt;IActionResult&gt;.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBasketItem([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var basketItem = await _context.BasketProducts.FindAsync(id);

            if (basketItem == null)
            {
                return NotFound();
            }

            return Ok(basketItem);
        }

        // PUT: api/CartItems/5
        /// <summary>
        /// Modify the passed basket item with the specified id.
        /// The basketItem is coming from body and the ID from route
        /// </summary>
        /// <param name="id">The identifier of the item you want to modify.</param>
        /// <param name="basketItem">The basket updated basket item.</param>
        /// <returns>IActionResult.</returns>
        [HttpPut("{id}")]
        public IActionResult PutBasketItem([FromRoute] int id, [FromBody] BasketItem basketItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != basketItem.Id)
            {
                return BadRequest();
            }

            var result = _context.ModifyCartItem(id, basketItem);

            if (result == ProductInjuryResult.NotFound)
            {
                return NotFound(string.Format("The product id {0} was not found!", basketItem.ProductId));
            }
            else if (result == ProductInjuryResult.QuantityNotAvailable)
            {
                return NotFound(string.Format("{0} items of product {1} were not found in stock!", basketItem.Quantity, basketItem.ProductId));
            }

            return NoContent();
        }

        // POST: api/CartItems
        /// <summary>
        /// Insert the passed basket item in the specified basket. 
        /// The basket item is coming from body and the basket id from query
        /// </summary>
        /// <param name="basketItem">The basket item you want to insert into the basket.</param>
        /// <param name="basketId">The basket identifier.</param>
        /// <returns>IActionResult.</returns>
        [HttpPost]
        public IActionResult PostBasketItem([FromBody] BasketItem basketItem, [BindRequired][FromQuery] int basketId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ProductInjuryResult productInjuryResult = _context.CheckProduct(basketItem.ProductId, basketItem.Quantity);

            if (productInjuryResult == ProductInjuryResult.NotFound)
            {
                return NotFound(string.Format("The product id {0} was not found!", basketItem.ProductId));
            }
            else if (productInjuryResult == ProductInjuryResult.QuantityNotAvailable)
            {
                return NotFound(string.Format("{0} items of product {1} were not found in stock!", basketItem.Quantity, basketItem.ProductId));
            }
            _context.AddProduct(basketItem, basketId);

            return CreatedAtAction("GetCartItem", new { id = basketItem.Id }, basketItem);
        }

        // DELETE: api/CartItems/5
        /// <summary>
        /// Deletes the basket item with the passed ID. The id is coming from route
        /// </summary>
        /// <param name="basketItemId">The basket item identifier you wanto to delete.</param>
        /// <returns>Task&lt;IActionResult&gt;.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBasketItem([FromRoute] int basketItemId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var cartItem = await _context.BasketProducts.FindAsync(basketItemId);
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