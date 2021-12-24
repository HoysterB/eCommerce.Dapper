using System.Collections.Generic;

namespace eCommerce.Domain.Models.Interfaces.Repository.Base
{
    public interface IBaseRepository<T> where T : class
    {
        public List<T> GetAll();
        public T Get(int id);
        public void Insert(T entity);
        public void Update(T entity);
        public void Delete(int id);
    }
}