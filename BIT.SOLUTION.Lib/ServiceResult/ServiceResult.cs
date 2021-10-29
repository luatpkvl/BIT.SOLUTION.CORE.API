
using BIT.SOLUTION.Common;
using BIT.SOLUTION.Lib;
using BIT.SOLUTION.Lib.Enum;
using System;
using System.Collections.Generic;
using System.Net;

namespace BIT.SOLUTION.Lib
{
    public class ServiceResult
    {
        private bool _success;
        private List<ValidateResult> _validateInfo;
        private HttpStatusCode? m_Code;
        public bool Success
        {
            get
            {
                return _success;
            }
            set
            {
                if(!value)
                {
                    m_Code = HttpStatusCode.InternalServerError;
                }
                _success = value;

            }
        }
        public HttpStatusCode? Code
        {
            get
            {
                return m_Code;
            }
            set
            {
                m_Code = value;
                if(m_Code != HttpStatusCode.OK)
                {
                    Success = false;
                }
                else
                {
                    Success = true;
                }
            }
        }

        public string Error { set; get; }
        public string ErrorCode { set; get; }
        public Enumeration.ServiceErrorType? ErrorType { set; get; }
        /// <summary>
        /// kiem tra thuc hien luu
        /// </summary>

        public List<ValidateResult> ValidateInfo
        {
            get
            {
                if(this._validateInfo == null)
                {
                    this._validateInfo = new List<ValidateResult>();
                }
                return _validateInfo;
            }
            set
            {
                _validateInfo = value;
            }
        }
        /// <summary>
        ///  tong ban ghi thanh cong
        /// </summary>
        public long Total { set; get; } = 0;
        /// <summary>
        /// thong bao
        /// </summary>
        public long SubTotal { set; get; } = 0;
        /// <summary>
        /// du lieu tra ve
        /// </summary>
        public object Data { set; get; }
        /// <summary>
        /// du lieu tra ve tuw ELTS hay khong
        /// </summary>
        public bool FromELTS { set; get; }
        /// <summary>
        /// ma loi kieu int
        /// </summary>
        public int? Status { set; get; }
        /// <summary>
        /// gio he thong
        /// </summary>
        public DateTime ServerDateTime { set; get; } = DateTime.Now;
        /// <summary>
        /// co bao he thong dang Maintence
        /// </summary>
        public bool? IsMaintenace { set; get; }
        /// <summary>
        ///  thong bao co quyen
        /// </summary>

        public bool? IsPermission { set; get; }
        public string Env
        {
            get
            {
                return "DEV";
            }
        }

        public ServiceResult()
        {
            Success = true;
            Code = HttpStatusCode.OK;
            IsMaintenace = false;
        }
        public override string ToString()
        {
            string strDescription = string.Empty;
            if(this.Success)
            {
                strDescription = string.Format("Success: {0} - Code: {1}", Success, Code);
            }
            else
            {
                if(ValidateInfo != null)
                {
                    strDescription = string.Format("Failed - Code: {0} - Message:{1}", Code, CommonFnConvert.SerializableObject(ValidateInfo));
                }
            }
            return strDescription;
        }
        public void SetError(Exception e, string sInfo ="")
        {
            this.Code = HttpStatusCode.InternalServerError;
            this.Error = e.Message;
            this.Success = false;
            /// GHi logger
        }
        public void SetValue(bool success, HttpStatusCode code, string errorMesage ="", object data = null, Enumeration.ServiceErrorType ErrorType  =  Enumeration.ServiceErrorType.None)
        {
            this.Success = success;
            this.Code = code;
            this.Data = data;
            this.ErrorType = ErrorType;
        }

    }
}
