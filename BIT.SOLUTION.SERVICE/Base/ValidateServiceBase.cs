using BIT.SOLUTION.Lib;
using BIT.SOLUTION.MODEL;
using System;
using System.Collections.Generic;
using System.Linq;
namespace BIT.SOLUTION.SERVICE
{
    public partial class ServiceBitBase : ServiceBase, IServiceBitBase
    {
        public virtual List<ValidateResult> ValidateSaveData(BaseEntity entity)
        {
            List<ValidateResult> lstResult = new List<ValidateResult>();
            List<BITField> fields = entity.Fields;
            List<BITField> customFields = entity.FieldsCustom;
            if (fields != null)
            {
                if (fields.FirstOrDefault(f => f.IsUnique == true) != null)
                {
                    lstResult = ValidateDuplicateData(entity);
                }
            }
            return lstResult;
        }
        public virtual List<ValidateResult> ValidateDuplicateData(BaseEntity entity)
        {
            List<ValidateResult> lstResult = new List<ValidateResult>();
            List<BITField> fields = new List<BITField>();
            fields.AddRange(entity.Fields);
            if (fields.FirstOrDefault(f => f.IsUnique == true) != null)
            {
                if (entity.FieldsCustom != null)
                {
                    fields.AddRange(entity.FieldsCustom);
                }
                List<BITField> uniqueFields = fields.FindAll(f => f.IsUnique == true);
                for (int i = 0; i < uniqueFields.Count; i++)
                {
                    BITField f = uniqueFields[i];
                    this.ValidateDuplicateDataByField(entity, f, ref lstResult);
                }
            }
            return lstResult;
        }
        public virtual void ValidateDuplicateDataByField(BaseEntity entity, BITField f, ref List<ValidateResult> lstResult)
        {
            try
            {

            }
            catch (FormatException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

       
    }
}
