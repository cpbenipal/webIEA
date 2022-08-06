
using System.Collections.Generic;
using System.Threading.Tasks;
using webIEA.Contracts;

namespace webIEA.Contracts
{
    public interface IUnitOfWork
    {

        IRepositoryBase<T> GetRepository<T>()
            where T : class;
        
    }
}
