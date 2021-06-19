using Core.Interfaces;
using Core.Poco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infraestructure.Data
{
    public class ClientRepository : IClientRepository
    {
        private RAFContext context;
        private readonly int SIZE = 228;

        public ClientRepository()
        {

        }
        #region Metodos
        public void Create(Cliente t)
        {
            context.Create<Cliente>(t);
        }

        public int Update(Cliente t)
        {
            throw new NotImplementedException();
        }

        public bool Delete(Cliente t)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Cliente> GetAll()
        {
            return context.GetAll<Cliente>();
        }

        public IEnumerable<Cliente> Find(Expression<Func<Cliente, bool>> where)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
