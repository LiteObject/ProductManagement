using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ProductManagement.App.DTOs;
using ProductManagement.App.Services;

namespace ProductManagement.API.Controllers
{
    /// <summary>
    /// Controller for managing products.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public sealed class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IMapper _mapper;
        private readonly ILogger<ProductsController> _logger;

        private static readonly Action<ILogger, string, Exception?> _logInstantiated =
           LoggerMessage.Define<string>(LogLevel.Information, new EventId(1, nameof(ProductsController)), "Instantiated {ControllerName}");
        private static readonly Action<ILogger, string, Exception?> _logInvoked =
            LoggerMessage.Define<string>(LogLevel.Information, new EventId(2, nameof(GetAsync)), "Invoked {MethodName}");
        private static readonly Action<ILogger, string, int, Exception?> _logInvokedWithId =
            LoggerMessage.Define<string, int>(LogLevel.Information, new EventId(3, nameof(GetProductByIdAsync)), "Invoked {MethodName} with ID: {Id}");

        /// <summary>
        /// Constructor for ProductsController.
        /// </summary>
        /// <param name="productService"></param>
        /// <param name="mapper"></param>
        /// <param name="logger"></param>
        public ProductsController(IProductService productService, IMapper mapper, ILogger<ProductsController> logger)
        {
            _productService = productService;
            _mapper = mapper;
            _logger = logger;
            _logInstantiated(_logger, nameof(ProductsController), null);
        }

        /// <summary>
        /// Get all products.
        /// </summary>
        /// <returns></returns>
        [HttpGet(Name = "GetProducts")]
        public async Task<IActionResult> GetAsync()
        {
            _logInvoked(_logger, nameof(GetAsync), null);
            var products = await _productService.GetAllProductsAsync().ConfigureAwait(false);
            return Ok(products);
        }

        /// <summary>
        /// Get product by ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}", Name = "GetProductById")]
        public async Task<IActionResult> GetProductByIdAsync(int id)
        {
            _logInvokedWithId(_logger, nameof(GetProductByIdAsync), id, null);
            var product = await _productService.GetProductByIdAsync(id).ConfigureAwait(false);

            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        /// <summary>
        /// Add a new product.
        /// </summary>
        /// <param name="productDto">The product DTO.</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> PostAsync(CreateProductDto productDto)
        {
            _logInvoked(_logger, nameof(PostAsync), null);
            int newProductId = await _productService.AddProductAsync(productDto).ConfigureAwait(false);
            return CreatedAtRoute("GetProductById", new { id = newProductId }, productDto);
        }

        /// <summary>
        /// Update a product.
        /// </summary>
        /// <param name="id">The product id.</param>
        /// <param name="productDto"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAsync(int id, UpdateProductDto productDto)
        {
            if (id != productDto?.Id)
            {
                return BadRequest();
            }

            _logInvokedWithId(_logger, nameof(PutAsync), id, null);
            await _productService.UpdateProductAsync(productDto).ConfigureAwait(false);
            return NoContent();
        }

        /// <summary>
        /// Test method to produce an exception for testing global exception handling.
        /// </summary>
        /// <returns></returns>
        [HttpGet("throw")]
        public IActionResult ThrowException()
        {
            // Throw an exception for testing purposes
            throw new InvalidOperationException("This is a test exception for global exception handling.");
        }
    }
}
