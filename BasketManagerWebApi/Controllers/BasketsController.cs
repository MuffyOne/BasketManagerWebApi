using BasketManagerWebApi.Common.Models;
using BasketManagerWebApi.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BasketManagerWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BasketsController : ControllerBase
    {
        private readonly BasketContext _context;

        public BasketsController(BasketContext context)
        {
            _context = context;
        }

        // GET: api/Baskets
        /// <summary>
        /// Gets the list of all baskets.
        /// Returns empty if no basket is present
        /// Accepts HTTP Get requests
        /// </summary>
        /// <returns>IEnumerable&lt;Basket&gt;.</returns>
        [HttpGet]
        public IEnumerable<Basket> GetBaskets()
        {
            return _context.Baskets;
        }

        // GET: api/Baskets/5
        /// <summary>
        /// Gets the basket with the specified Id coming from routing.
        /// Returns 404 Not Found in case the basket is not found
        /// Accepts HTTP Get requests
        /// </summary>
        /// <param name="id">The identifier of the basket you want to get.</param>
        /// <returns>Task&lt;IActionResult&gt;.</returns>
        [HttpGet("{id}")]
        public IActionResult GetBasket([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var basket = _context.Baskets.FirstOrDefault(i => i.BasketId == id);

            if (basket == null)
            {
                return NotFound();
            }

            return Ok(basket);
        }

        // DELETE: api/Baskets/5
        /// <summary>
        /// Deletes the basket and all elements inside it. The basket Id is coming from routing
        /// Returns 404 Not Found if the basket is not found
        /// Accepts HTTP Delete requests
        /// </summary>
        /// <param name="id">The identifier of the basket you want to delete.</param>
        /// <returns>IActionResult.</returns>
        [HttpDelete("{id}")]
        public IActionResult DeleteBasket([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = _context.DeleteBasketAndAllElements(id);

            if (result == Enums.BasketDeleteResult.NotFound)
            {
                return NotFound(string.Format("basket with id {0} not found", id));
            }
            return Ok();
        }

        private bool BasketExists(int id)
        {
            return _context.Baskets.Any(e => e.Id == id);
        }
    }
}