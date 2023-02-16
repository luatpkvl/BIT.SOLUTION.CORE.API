using BIT.SOLUTION.BaseApi;
using BIT.SOLUTION.Lib;
using BIT.SOLUTION.MODEL;
using BIT.SOLUTION.MODEL.Account;
using BIT.SOLUTION.SERVICE;
using Google.Protobuf.Collections;
using Microsoft.AspNetCore.Mvc;
using System;

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
       
        [HttpPost("Login")]
        public  ServiceResult Login(Employee employee)
        {
            ServiceResult rs = new ServiceResult();

            try
            {
                rs.Success = true;
                rs.Data = _service.Login(employee);
            }
            catch(Exception ex)
            {
                rs.Error = ex.Message;
            }
            return rs;


        }
        [HttpPost("GetList")]
        public ServiceResult GetList(Filler  filler)
        {
            ServiceResult rs = new ServiceResult();

            try
            {
                string where = " where IsDelete != true ";
                if(filler != null)
                {
                    if(!string.IsNullOrEmpty(filler.Name))
                    {
                        where = where + $" and UserName = '{filler.UserName}' ";
                    }
                    if (!string.IsNullOrEmpty(filler.Name))
                    {
                        where = where + $" or Name = '{filler.Name}' ";
                    }    
                }    
                rs.Success = true;
                rs.Data = _service.GetList(where);
            }
            catch (Exception ex)
            {
                rs.Error = ex.Message;
            }
            return rs;


        }
        [HttpPost("Save")]
        public ServiceResult Save(Employee employee)
        {
            ServiceResult rs = new ServiceResult();

            try
            {
                rs.Success = true;
                rs.Data = _service.Save(employee);
            }
            catch (Exception ex)
            {
                rs.Error = ex.Message;
            }
            return rs;


        }
        [HttpPost("Delete")]
        public ServiceResult Delete(string UserName)
        {
            ServiceResult rs = new ServiceResult();

            try
            {
                rs.Success = true;
                rs.Data = _service.Delete(UserName);
            }
            catch (Exception ex)
            {
                rs.Error = ex.Message;
            }
            return rs;


        }

    }
    public class Filler
    {
        public string UserName { set; get; }
        public string Name { set; get; }
    }
}
