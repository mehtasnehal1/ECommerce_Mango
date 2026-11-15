using AutoMapper;
using Mango.Services.CouponAPI.Data;
using Mango.Services.CouponAPI.Models;
using Mango.Services.CouponAPI.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;


namespace Mango.Services.CouponAPI.Controllers
{
    [Route("api/coupon")]
    [ApiController]
    [Authorize]
    public class CouponAPIController : ControllerBase
    {

        private readonly AppDbContext _db;
        private readonly ResponseDto _response;
        private readonly IMapper _mapper;

        public CouponAPIController(AppDbContext db, IMapper mapper)
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
                IEnumerable<Coupon> objListcoupons = _db.Coupons.ToList();
                _response.Result = _mapper.Map<IEnumerable<CouponDto>>(objListcoupons);
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
                Coupon objCoupons = _db.Coupons.First( c => c.CouponId == id);
                _response.Result = _mapper.Map<CouponDto>(objCoupons);
            }
            catch (Exception ex)
            {
                _response.IsSuccess=false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpGet]
        [Route("GetByCode/{code}")]
        public ResponseDto GetByCode(string code)
        {
            try
            {
                Coupon objCoupons = _db.Coupons.FirstOrDefault(c => c.CouponCode == code);

                if(objCoupons == null)
                {
                    _response.IsSuccess = false;
                }

                _response.Result = _mapper.Map<CouponDto>(objCoupons);
            }
            catch (Exception ex)
            { 
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpPost]
        [Authorize (Roles ="ADMIN")]
        public ResponseDto AddNewCoupon([FromBody] CouponDto couponDto)
        {
            try
            {
                Coupon objCoupon = _mapper.Map<Coupon>(couponDto);
                _db.Coupons.Add(objCoupon);
                _db.SaveChanges();

                var options = new Stripe.CouponCreateOptions
                {
                    AmountOff = (long)(couponDto.DiscountAmount * 100), // Convert to cents
                    Name = couponDto.CouponCode,
                    Currency = "usd",
                    Id = couponDto.CouponCode,
                };
                
                var service = new Stripe.CouponService();
                service.Create(options);
                _response.Result = _mapper.Map<CouponDto>(objCoupon);
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
        public ResponseDto UpdateCoupon([FromBody] CouponDto couponDto)
        {
            try
            {
                Coupon objCoupon = _mapper.Map<Coupon>(couponDto);
                _db.Coupons.Update(objCoupon);
                _db.SaveChanges();

                _response.Result = _mapper.Map<CouponDto>(objCoupon);
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
        public ResponseDto DeleteCoupon(int id)
        {
            try
            {
                Coupon objCoupon = _db.Coupons.First(c => c.CouponId == id);    
                _db.Coupons.Remove(objCoupon);
                _db.SaveChanges();

                var service = new Stripe.CouponService();
                service.Delete(objCoupon.CouponCode);
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
