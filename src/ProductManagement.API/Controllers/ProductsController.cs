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
            LoggerMessage.Define<string>(LogLevel.Information, new EventId(2, nameof(Get)), "Invoked {MethodName}");
        private static readonly Action<ILogger, string, int, Exception?> _logInvokedWithId =
            LoggerMessage.Define<string, int>(LogLevel.Information, new EventId(3, nameof(GetProductById)), "Invoked {MethodName} with ID: {Id}");


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
        public IActionResult Get()
        {
            _logInvoked(_logger, nameof(Get), null);
            var products = _productService.GetAllProducts();
            return Ok(products);
        }

        /// <summary>
        /// Get product by ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}", Name = "GetProductById")]
        public IActionResult GetProductById(int id)
        {
            _logInvokedWithId(_logger, nameof(GetProductById), id, null);
            var product = _productService.GetProductById(id);

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
        public IActionResult Post(ProductDto productDto)
        {
            int newProductId = _productService.AddProduct(productDto);
            return CreatedAtRoute("GetProductById", new { id = newProductId }, productDto);
        }
    }
}
