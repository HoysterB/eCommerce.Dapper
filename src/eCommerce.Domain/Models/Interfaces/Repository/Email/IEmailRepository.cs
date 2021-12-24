using eCommerce.Domain.Models.Entities;

namespace eCommerce.Domain.Models.Interfaces.Repository.Email
{
    public interface IEmailRepository
    {
        void SendEmail(Usuario entity);
    }
}