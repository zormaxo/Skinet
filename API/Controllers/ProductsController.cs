using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Dtos;
using API.Errors;
using API.Helpers;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
  public class ProductsController : BaseApiController
  {
    private readonly IGenericRepository<ProductBrand> _productBrandRepo;
    private readonly IGenericRepository<ProductType> _productTypeRepo;
    private readonly IGenericRepository<Product> _productsRepo;
    private readonly IMapper _mapper;
    private readonly StoreContext context;

    public ProductsController(IGenericRepository<Product> productsRepo,
            IGenericRepository<ProductType> productTypeRepo,
            IGenericRepository<ProductBrand> productBrandRepo,
            IMapper mapper, StoreContext context)
    {
      _mapper = mapper;
      this.context = context;
      _productsRepo = productsRepo;
      _productTypeRepo = productTypeRepo;
      _productBrandRepo = productBrandRepo;
    }

    [HttpGet]
    [HttpGet]
    public async Task<ActionResult<Pagination<ProductToReturnDto>>> GetProducts(
            [FromQuery] ProductSpecParams productParams)
    {
      var spec = new ProductsWithTypesAndBrandsSpecification(productParams);
      var countSpec = new ProductsWithFiltersForCountSpecification(productParams);

      var totalItems = await _productsRepo.CountAsync(countSpec);

      var products = await _productsRepo.ListAsync(spec);

      var data = _mapper.Map<IReadOnlyList<ProductToReturnDto>>(products);

      return Ok(new Pagination<ProductToReturnDto>(productParams.PageIndex,
          productParams.PageSize, totalItems, data));
    }


    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]   //swagger hints
    public async Task<ActionResult<ProductToReturnDto>> GetProduct(int id)
    {

      var omer1 = context.Products;
      var omer2 = context.Products.ToList();
      var omer3 = context.Products.Include(x => x.ProductBrand);
      var omer4 = context.Products.Include(x => x.ProductBrand).ToList(); ;



      var spec = new ProductsWithTypesAndBrandsSpecification(id);

      var product = await _productsRepo.GetEntityWithSpec(spec);

      if (product == null) return NotFound(new ApiResponse(404));

      return _mapper.Map<ProductToReturnDto>(product);
    }

    [HttpGet("brands")]
    public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetBrands()
    {
      return Ok(await _productBrandRepo.ListAllAsync());
    }

    [HttpGet("types")]
    public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetTypes()
    {
      return Ok(await _productTypeRepo.ListAllAsync());
    }
  }
}
