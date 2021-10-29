using BIT.SOLUTION.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BIT.SOLUTION.Lib.Enum
{
    public static class Enumeration
    {
        public enum LayoutCode
        {
            [TableName("Accout")]
            Acount =0
        }
        public enum BITEntityState
        {
            /// <summary>
            /// bat buoc
            /// </summary>
            None = 0,
            /// <summary>
            /// khoa ngoai khong dung
            /// </summary>
            Add = 1,
            /// <summary>
            /// vuot qua length
            /// </summary>
            Edit = 2,
            /// <summary>
            /// chua du min lenth
            /// </summary>
            Delete = 3,
            /// <summary>
            /// dinh dang email
            /// </summary>
            Duplicate = 4,
            /// <summary>
            ///  kieu taxcode
            /// </summary>
            AddOrEdit = 5,
            UpdateInsert = 6,
        }
        public enum ValidateType
        {
            /// <summary>
            /// bat buoc
            /// </summary>
            Required =0,
            /// <summary>
            /// khoa ngoai khong dung
            /// </summary>
            Unique =1,
            /// <summary>
            /// vuot qua length
            /// </summary>
            MaxLength =2,
            /// <summary>
            /// chua du min lenth
            /// </summary>
            MinLength =3,
            /// <summary>
            /// dinh dang email
            /// </summary>
            Email = 4,
            /// <summary>
            ///  kieu taxcode
            /// </summary>
            TaxCode = 5,
        }
        public enum ServiceResultType
        {
            /// <summary>
            /// Khong loi
            /// </summary>
            None =0,
            /// <summary>
            /// dang bao tri
            /// </summary>
            SystemIsMaintain =1,
            /// <summary>
            /// token khong hop le
            /// </summary>
            TokenInvalid =2,
            /// <summary>
            /// trung khoa
            /// </summary>
            Duplicate =3,
            /// <summary>
            /// phat sinh data
            /// </summary>
            Contraint =4,
            /// <summary>
            /// gia tri vuot qua quy dinh
            /// </summary>
            OutOfRange =5,
        }
        public enum ServiceErrorType
        {
            /// <summary>
            /// Khong loi
            /// </summary>
            None = 0,
            /// <summary>
            /// dang bao tri
            /// </summary>
            SystemIsMaintain = 1,
            /// <summary>
            /// token khong hop le
            /// </summary>
            TokenInvalid = 2,
            /// <summary>
            /// trung khoa
            /// </summary>
            Duplicate = 3,
            /// <summary>
            /// phat sinh data
            /// </summary>
            Contraint = 4,
            /// <summary>
            /// gia tri vuot qua quy dinh
            /// </summary>
            OutOfRange = 5,
        }
    }
}
