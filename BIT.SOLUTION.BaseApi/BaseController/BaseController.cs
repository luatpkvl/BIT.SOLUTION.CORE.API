using BIT.SOLUTION.Common;
using BIT.SOLUTION.Lib;
using BIT.SOLUTION.MODEL;
using BIT.SOLUTION.SERVICE;
using Microsoft.AspNetCore.Mvc;
using System;

namespace BIT.SOLUTION.BaseApi
{
    public class BaseController<T> : ControllerBase where T: BaseEntity
    {
        private IServiceBitBase _service;
        public BaseController(IServiceBitBase service)
        {
            _service = service;
        }
        [HttpGet]
        [Route("list")]
        public ServiceResult List()
        {
            ServiceResult result = new ServiceResult()
            {
                Code = System.Net.HttpStatusCode.OK,
                Status = 200,
                Success = true,
                Total = 0,
            };
             var rs = _service.GetDataAll<T>("*");
          
            result.Data = rs;
            if (rs.Count != 0)
            {
                result.Total = rs.Count ;
            }
            return result;
        }
        [HttpGet]
        [Route("get")]
        public ServiceResult Get()
        {
            ServiceResult result = new ServiceResult()
            {
                Code = System.Net.HttpStatusCode.OK,
                Status = 200,
                Success = true,
                Total = 0,
            };
            result.Data = _service.GetDataByID(Guid.Parse("a98fabf5-988c-47a4-b187-272865e1f824"), Utility.GetTableName<T>(), "*");
            if(result.Data != null)
            {
                result.Total = 1;
            } 
            return result;
        }
        [HttpPost]
        public ServiceResult Post(T  entity)
        {
            return _service.SaveData<T>(entity);
        }
        [HttpPut]
        public ServiceResult Put(T entity)
        {
            entity.UserProc = false;
            entity.BITEntityState = Lib.Enum.Enumeration.BITEntityState.Edit;
            ServiceResult result = new ServiceResult();
            result.Data = _service.SaveData<T>(entity);
            return result;
        }
        [HttpDelete]
        public ServiceResult Delete(T entity)
        {
            entity.UserProc = false;
            entity.BITEntityState = Lib.Enum.Enumeration.BITEntityState.Delete;
            ServiceResult result = new ServiceResult();
            result.Data = _service.Delete<T>(entity);
            return result;
        }
    }
}
