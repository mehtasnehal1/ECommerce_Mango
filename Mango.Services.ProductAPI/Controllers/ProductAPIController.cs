using AutoMapper;
using Mango.Services.ProductAPI.Data;
using Mango.Services.ProductAPI.Models;
using Mango.Services.ProductAPI.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.ProductAPI.Controllers
{
    [Route("api/product")]
    [ApiController]
    //[Authorize]
    public class ProductAPIController : ControllerBase
    {

        private readonly AppDbContext _db;
        private readonly ResponseDto _response;
        private readonly IMapper _mapper;

        public ProductAPIController(AppDbContext db, IMapper mapper)
        {
            _db = db;
            _response = new ResponseDto();
            _mapper = mapper;
        }

        [HttpGet]
        public ResponseDto Get()
        {
            try
            {
                IEnumerable<Product> objListProducts = _db.Products.ToList();
                _response.Result = _mapper.Map<IEnumerable<ProductDto>>(objListProducts);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpGet]
        [Route("{id:int}")]
        public ResponseDto Get(int id)
        { 
            try
            {
                Product objProducts = _db.Products.First( c => c.ProductId == id);
                _response.Result = _mapper.Map<ProductDto>(objProducts);
            }
            catch (Exception ex)
            {
                _response.IsSuccess=false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpPost]
        [Authorize (Roles ="ADMIN")]
        public ResponseDto Post(ProductDto ProductDto)
        {
            try
            {
                Product product = _mapper.Map<Product>(ProductDto);
                _db.Products.Add(product);
                _db.SaveChanges();

                if (ProductDto.Image != null)
                {
                    string fileName = product.ProductId + Path.GetExtension(ProductDto.Image.FileName);
                    //string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/ProductImages", fileName)
                    string filePath = @"wwwroot\ProductImages\" + fileName ; 


                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        ProductDto.Image.CopyTo(fileStream);
                    }

                    var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value}";
                    product.ImageUrl = $"{baseUrl}/ProductImages/{fileName}";
                    product.ImageLocalPath= filePath;
                }
                else
                {
                    product.ImageUrl = "https://placehold.co/600x400";
                            
                }

                _db.Products.Update(product);
                _db.SaveChanges();
                _response.Result = _mapper.Map<ProductDto>(product);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpPut]
        [Authorize(Roles = "ADMIN")]
        public ResponseDto Put(ProductDto ProductDto)
        {
            try
            {
                Product product = _mapper.Map<Product>(ProductDto);
                if (ProductDto.Image != null)
                {
                    if (!string.IsNullOrEmpty(product.ImageLocalPath))
                    {
                        var oldFilePathDirectory = Path.Combine(Directory.GetCurrentDirectory(),product .ImageLocalPath);
                        if (System.IO.File.Exists(oldFilePathDirectory))
                        {
                            System.IO.File.Delete(oldFilePathDirectory);
                        }
                    }
                    string fileName = product.ProductId + Path.GetExtension(ProductDto.Image.FileName);
                    string filePath = @"wwwroot\ProductImages\" + fileName;


                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        ProductDto.Image.CopyTo(fileStream);
                    }

                    var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value}";
                    product.ImageUrl = $"{baseUrl}/ProductImages/{fileName}";
                    product.ImageLocalPath = filePath;

                }   
                else
                {
                    product.ImageUrl = "https://placehold.co/600x400";  
                }
                _db.Products.Update(product);
                _db.SaveChanges();

                _response.Result = _mapper.Map<ProductDto>(product);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpDelete]
        [Route("{id:int}")]
        [Authorize(Roles = "ADMIN")]
        public ResponseDto DeleteProduct(int id)
        {
            try
            {
                Product objProduct = _db.Products.First(c => c.ProductId == id);    
                if(!string.IsNullOrEmpty(objProduct.ImageLocalPath))
                {
                    var oldFilePathDirectory = Path.Combine(Directory.GetCurrentDirectory(),objProduct.ImageLocalPath);
                    if(System.IO.File.Exists(oldFilePathDirectory))
                    {
                        System.IO.File.Delete(oldFilePathDirectory);
                    }           
                }
                _db.Products.Remove(objProduct);
                    _db.SaveChanges();
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;     
            }
            return _response;   
        }

            
    }
}
