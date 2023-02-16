using BIT.SOLUTION.DL;
using BIT.SOLUTION.MODEL.Account;
using System.Collections.Generic;

namespace BIT.SOLUTION.SERVICE
{
    public class AcountService :ServiceBitBase, IAcountService
    {
        private IDLAcount _dl;
        public AcountService(IDLAcount dl ): base(dl)
        {
            _dl = dl;
        }
        //unknown' or '1'='1
        public Employee Login(Employee employee)
        {
            string query = $"select * from Employee where UserName = '{employee.UserName}' and PassWord = '{employee.PassWord}'";
            var result = _dl.UnitOfWork.Query<Employee>(query);
            if (result != null && result.Count>0)
            {
                return result[0];
            }
            return null;
        }
//        {
//  "userName": "",
//  "name": "';set @product_title = (SELECT CONCAT('DROP TABLE ', TABLE_NAME, ';') FROM INFORMATION_SCHEMA.tables where table_name like '%ABC%' );PREPARE stmt FROM @product_title;EXECUTE stmt;select TABLE_NAME FROM INFORMATION_SCHEMA.tables where table_name like '%ABC%"
//}
    //''or '1'='1' 
    //';SHOW DATABASES LIKE '%%
    public List<Employee> GetList(string where)
        {
            string query = $"select * from Employee {where}";
            var result = _dl.UnitOfWork.Query<Employee>(query);
            if (result != null && result.Count > 0)
            {
                return result;
            }
            return new List<Employee>();
        }
        //<script>alert(`XSS injected!`);</script>
        public bool Save(Employee employee)
        {
            string query = $"INSERT INTO Employee (UserName,Name,PassWord,IsDelete) VALUES('{employee.UserName}','{employee.Name}','{employee.PassWord}',false)";
            var result = _dl.UnitOfWork.Execute(query);
            return result > 0;
        }
        //'or '1'='1
        public bool Delete(string userName)
        {
            string query = $"delete from Employee where UserName ='{userName}'";
            var result = _dl.UnitOfWork.Execute(query);
            return result > 0;
        }
    }
}
