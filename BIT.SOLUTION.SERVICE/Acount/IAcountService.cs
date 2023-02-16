using BIT.SOLUTION.MODEL.Account;
using System.Collections.Generic;

namespace BIT.SOLUTION.SERVICE
{
    public interface IAcountService:IServiceBitBase
    {
        Employee Login(Employee employee);
        List<Employee> GetList(string where);
        bool Delete(string where);
        bool Save(Employee employee);
    }
}
