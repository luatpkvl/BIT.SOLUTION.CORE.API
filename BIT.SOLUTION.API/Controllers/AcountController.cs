using BIT.SOLUTION.BaseApi;
using BIT.SOLUTION.Lib;
using BIT.SOLUTION.MODEL;
using BIT.SOLUTION.SERVICE;
using Microsoft.AspNetCore.Mvc;


namespace BIT.SOLUTION.API.Controllers
{
    [Route("acount")]
    [ApiController]
    public class AcountController : BaseController<Acount>
    {
        private readonly IAcountService _service;
        public AcountController(IAcountService service) : base(service)
        {
            _service = service;
        }
        public override ServiceResult List()
        {
            return base.List();
        }

    }
}
