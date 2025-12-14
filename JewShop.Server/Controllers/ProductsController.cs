using JewShop.Server.Services.Interfaces;
using JewShop.Shared.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace JewShop.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll()
        {
            var products = await _productService.GetAllProductsAsync();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetById(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
                return NotFound();

            return Ok(product);
        }

        [HttpPost]
        public async Task<ActionResult<ProductDto>> Create([FromBody] CreateProductDto dto)
        {
            var product = await _productService.CreateProductAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ProductDto>> Update(int id, [FromBody] UpdateProductDto dto)
        {
            var product = await _productService.UpdateProductAsync(id, dto);
            if (product == null)
                return NotFound();

            return Ok(product);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var result = await _productService.DeleteProductAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }

        // Image Endpoints
        [HttpPost("{id}/images")]
        public async Task<ActionResult<ProductImageDto>> AddImage(int id, [FromBody] CreateProductImageDto dto)
        {
            var image = await _productService.AddProductImageAsync(id, dto);
            return Ok(image);
        }

        [HttpDelete("images/{imageId}")]
        public async Task<ActionResult> DeleteImage(int imageId)
        {
            var result = await _productService.DeleteProductImageAsync(imageId);
            if (!result)
                return NotFound();

            return NoContent();
        }

        // Variant Endpoints
        [HttpPost("{id}/variants")]
        public async Task<ActionResult<ProductVariantDto>> AddVariant(int id, [FromBody] CreateProductVariantDto dto)
        {
            var variant = await _productService.AddProductVariantAsync(id, dto);
            return Ok(variant);
        }

        [HttpPut("variants/{variantId}")]
        public async Task<ActionResult<ProductVariantDto>> UpdateVariant(int variantId, [FromBody] UpdateProductVariantDto dto)
        {
            var variant = await _productService.UpdateProductVariantAsync(variantId, dto);
            if (variant == null)
                return NotFound();

            return Ok(variant);
        }

        [HttpDelete("variants/{variantId}")]
        public async Task<ActionResult> DeleteVariant(int variantId)
        {
            var result = await _productService.DeleteProductVariantAsync(variantId);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}
