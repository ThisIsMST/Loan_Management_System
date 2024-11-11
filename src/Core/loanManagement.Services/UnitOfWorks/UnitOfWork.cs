using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace loanManagement.Services.UnitOfWorks
{
    public interface UnitOfWork
    {
        void Save();
        void Begin();
        void Rollback();
        void Commit();
    }
}
