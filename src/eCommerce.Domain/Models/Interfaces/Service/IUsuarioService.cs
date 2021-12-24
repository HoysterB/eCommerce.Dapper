using System.Collections.Generic;
using eCommerce.Domain.Models.Entities;
using eCommerce.Domain.Models.Responses;

namespace eCommerce.Domain.Models.Interfaces.Service
{
    public interface IUsuarioService
    {
        public List<Usuario> GetAll();
        public Usuario Get(int id);
        public void Insert(Usuario entity);
        public void Update(Usuario entity);
        public void Delete(int id);
    }
}