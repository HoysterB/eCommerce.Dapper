using System;
using System.Collections.Generic;
using eCommerce.Domain.Models.Entities;
using eCommerce.Domain.Models.Interfaces.Repository;
using eCommerce.Domain.Models.Interfaces.Repository.Email;
using eCommerce.Domain.Models.Interfaces.Service;
using eCommerce.Domain.Models.Responses;

namespace eCommerce.Service.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IEmailRepository _emailRepository;

        public UsuarioService(
            IUsuarioRepository usuarioRepository,
            IEmailRepository emailRepository)
        {
            _usuarioRepository = usuarioRepository;
            _emailRepository = emailRepository;
        }

        public List<Usuario> GetAll()
        {
            try
            {
                var listUsers = _usuarioRepository.GetAll();


                return listUsers;


                // return new GetAllUsuariosResponse()
                // {
                //     Data = null,
                //     IsSuccess = true,
                //     Message = "Nenhum usuário encontrado."
                // };
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public Usuario Get(int id)
        {
            try
            {
                return _usuarioRepository.pegarDiferente(id);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public void Insert(Usuario entity)
        {
            try
            {
                _usuarioRepository.Insert(entity);

                try
                {
                    _emailRepository.SendEmail(entity);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public void Update(Usuario entity)
        {
            try
            {
                _usuarioRepository.Update(entity);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public void Delete(int id)
        {
            try
            {
                _usuarioRepository.Delete(id);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}