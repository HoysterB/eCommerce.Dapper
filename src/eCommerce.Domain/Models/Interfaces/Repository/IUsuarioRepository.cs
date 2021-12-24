using System.Collections.Generic;
using eCommerce.Domain.Models.Entities;
using eCommerce.Domain.Models.Interfaces.Repository.Base;

namespace eCommerce.Domain.Models.Interfaces.Repository
{
    public interface IUsuarioRepository : IBaseRepository<Usuario>
    {
        Usuario pegarDiferente(int id);
    }
}