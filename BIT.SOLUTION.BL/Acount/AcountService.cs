using BIT.SOLUTION.DL;
namespace BIT.SOLUTION.SERVICE
{
    public class AcountService :ServiceBitBase, IAcountService
    {
        private IDLAcount dl;
        public AcountService(IDLAcount dl ): base(dl)
        {

        } 
    }
}
