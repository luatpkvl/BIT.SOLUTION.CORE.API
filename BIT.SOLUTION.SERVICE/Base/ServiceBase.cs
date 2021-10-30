using BIT.SOLUTION.DL;

namespace BIT.SOLUTION.SERVICE
{
    public class ServiceBase : IServiceBase
    {
        private readonly IDLBase DL;
        public ServiceBase(IDLBase dl)
        {
            this.DL = dl;
        }
        public void Dispose()
        {
            this.DL.UnitOfWork.Dispose();
        }
    }
}
