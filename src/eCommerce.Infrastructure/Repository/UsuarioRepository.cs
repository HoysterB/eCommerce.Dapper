using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using eCommerce.Domain.Models.Entities;
using eCommerce.Domain.Models.Interfaces.Repository;

namespace eCommerce.Infrastructure.Repository
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly IDbConnection _connection;

        public UsuarioRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public List<Usuario> GetAll()
        {
            try
            {
                string sql =
                    $"SELECT U.*, C.*, EE.*, D.* FROM Usuarios AS U " +
                    $"LEFT JOIN " +
                    $"Contatos C ON C.UsuarioId = U.Id " +
                    $"LEFT JOIN " +
                    $"EnderecosEntrega EE ON EE.UsuarioId = U.Id " +
                    $"LEFT JOIN " +
                    $"UsuariosDepartamentos UD ON UD.UsuarioId = U.Id " +
                    $"LEFT JOIN " +
                    $"Departamentos D ON UD.DepartamentoId = D.Id";

                List<Usuario> usuarios = new List<Usuario>();

                _connection.Query<Usuario, Contato, EnderecoEntrega, Departamento, Usuario>(sql,
                    (usuario, contato, enderecoEntrega, departamento) =>
                    {
                        //Verificação do usuário
                        if (usuarios.SingleOrDefault(x => x.Id == usuario.Id) == null)
                        {
                            usuario.EnderecosEntrega = new List<EnderecoEntrega>();
                            usuario.Departamentos = new List<Departamento>();
                            usuario.Contato = contato;
                            usuarios.Add(usuario);
                        }
                        else
                        {
                            usuario = usuarios.SingleOrDefault(x => x.Id == usuario.Id);
                        }

                        //Verificação do endereço de entrega
                        if (usuario.EnderecosEntrega.SingleOrDefault(a => a.Id == enderecoEntrega.Id) == null)
                        {
                            usuario.EnderecosEntrega.Add(enderecoEntrega);
                        }

                        //Verificação de departamento
                        if (usuario.Departamentos.SingleOrDefault(a => a.Id == departamento.Id) == null)
                        {
                            usuario.Departamentos.Add(departamento);
                        }

                        return
                            usuario; //nesta query de 1:n o retorno deste callback não importa, pois estamos populando a lista 
                    });

                return usuarios;
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
                string sql =
                    $"SELECT U.*, C.*, EE.*, D.* FROM Usuarios AS U " +
                    $"LEFT JOIN " +
                    $"Contatos C ON C.UsuarioId = U.Id " +
                    $"LEFT JOIN " +
                    $"EnderecosEntrega EE ON EE.UsuarioId = U.Id " +
                    $"LEFT JOIN " +
                    $"UsuariosDepartamentos UD ON UD.UsuarioId = U.Id " +
                    $"LEFT JOIN " +
                    $"Departamentos D ON UD.DepartamentoId = D.Id " +
                    $"WHERE U.Id = @Id";

                List<Usuario> usuarios = new List<Usuario>();

                _connection.Query<Usuario, Contato, EnderecoEntrega, Departamento, Usuario>(sql,
                    (usuario, contato, enderecoEntrega, departamento) =>
                    {
                        //Verificação do usuário
                        if (usuarios.SingleOrDefault(x => x.Id == usuario.Id) == null)
                        {
                            usuario.EnderecosEntrega = new List<EnderecoEntrega>();
                            usuario.Departamentos = new List<Departamento>();
                            usuario.Contato = contato;
                            usuarios.Add(usuario);
                        }
                        else
                        {
                            usuario = usuarios.SingleOrDefault(x => x.Id == usuario.Id);
                        }

                        //Verificação do endereço de entrega
                        if (usuario.EnderecosEntrega.SingleOrDefault(a => a.Id == enderecoEntrega.Id) == null)
                        {
                            usuario.EnderecosEntrega.Add(enderecoEntrega);
                        }

                        //Verificação de departamento
                        if (usuario.Departamentos.SingleOrDefault(a => a.Id == departamento.Id) == null)
                        {
                            usuario.Departamentos.Add(departamento);
                        }

                        return
                            usuario;
                    }, new {Id = id});

                return usuarios.Single();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public void Insert(Usuario entity)
        {
            _connection.Open();
            var transaction = _connection.BeginTransaction();
            try
            {
                string sql = $"INSERT INTO Usuarios " +
                             $"(Nome, Email, Sexo, RG, CPF, NomeMae, SituacaoCadastro, DataCadastro) " +
                             $"VALUES " +
                             $"(@Nome, @Email, @Sexo, @RG, @CPF, @NomeMae, @SituacaoCadastro, @DataCadastro); " +
                             $"SELECT CAST(SCOPE_IDENTITY() AS INT) ";

                entity.Id = _connection.Query<int>(sql, entity, transaction).Single();

                if (entity.Contato != null)
                {
                    entity.Contato.UsuarioId = entity.Id;

                    string sqlContato = $"INSERT INTO Contatos " +
                                        $"(UsuarioId, Telefone, Celular) " +
                                        $"VALUES " +
                                        $"(@UsuarioId, @Telefone, @Celular); " +
                                        $"SELECT CAST(SCOPE_IDENTITY() AS INT) ";

                    entity.Contato.Id = _connection.Query<int>(sqlContato, entity.Contato, transaction).Single();
                }

                if (entity.EnderecosEntrega != null && entity.EnderecosEntrega.Count > 0)
                {
                    foreach (var enderecoEntrega in entity.EnderecosEntrega)
                    {
                        enderecoEntrega.UsuarioId = entity.Id;

                        string sqlEnderecoEntrega = $"INSERT INTO EnderecosEntrega " +
                                                    $"(UsuarioId, NomeEndereco, CEP, Estado, Cidade, Bairro, Endereco, Numero, Complemento) " +
                                                    $"VALUES " +
                                                    $"(@UsuarioId, @NomeEndereco, @CEP, @Estado, @Cidade, @Bairro, @Endereco, @Numero, @Complemento); " +
                                                    $"SELECT CAST (SCOPE_IDENTITY() AS INT)";

                        enderecoEntrega.Id = _connection.Query<int>(sqlEnderecoEntrega, enderecoEntrega, transaction)
                            .Single();
                    }
                }

                if (entity.Departamentos != null && entity.Departamentos.Count > 0)
                {
                    foreach (var departamento in entity.Departamentos)
                    {
                        string sqlUsuarioDepartamentos = $"INSERT INTO UsuariosDepartamentos " +
                                                         $"(UsuarioId, DepartamentoId) " +
                                                         $"VALUES " +
                                                         $"(@UsuarioId, @DepartamentoId); ";

                        _connection.Execute(sqlUsuarioDepartamentos, new
                            {
                                UsuarioId = entity.Id,
                                DepartamentoId = departamento.Id
                            },
                            transaction);
                    }
                }

                transaction.Commit();
            }
            catch (Exception e)
            {
                try
                {
                    transaction.Rollback();
                }
                catch (Exception ex)
                {
                    throw new Exception(e.Message + " " + ex.Message);
                }
            }
            finally
            {
                _connection.Close();
            }
        }

        public void Update(Usuario entity)
        {
            _connection.Open();
            var transaction = _connection.BeginTransaction();
            try
            {
                string sql = $"UPDATE Usuarios SET " +
                             $"Nome = @Nome, Email = @Email, Sexo = @Sexo, RG = @RG, CPF = @CPF, NomeMae = @NomeMae, SituacaoCadastro = @SituacaoCadastro, DataCadastro = @DataCadastro " +
                             $"WHERE Id = @Id";
                _connection.Execute(sql, entity, transaction);

                //Contato
                if (entity.Contato != null)
                {
                    string sqlContato =
                        $"UPDATE Contatos SET UsuarioId = @UsuarioId, Telefone = @Telefone, Celular = @Celular WHERE Id = @Id";
                    _connection.Execute(sqlContato, entity.Contato, transaction);
                }

                //Enderecos de entrega
                string sqlDeleteEnderecosEntrega = $"DELETE FROM EnderecosEntrega WHERE UsuarioId = @Id";
                _connection.Execute(sqlDeleteEnderecosEntrega, new {Id = entity.Id}, transaction);

                if (entity.Departamentos != null && entity.Departamentos.Count > 0)
                {
                    foreach (var enderecoEntrega in entity.EnderecosEntrega)
                    {
                        enderecoEntrega.UsuarioId = entity.Id;

                        string sqlEnderecoEntrega = $"INSERT INTO EnderecosEntrega " +
                                                    $"(UsuarioId, NomeEndereco, CEP, Estado, Cidade, Bairro, Endereco, Numero, Complemento) " +
                                                    $"VALUES " +
                                                    $"(@UsuarioId, @NomeEndereco, @CEP, @Estado, @Cidade, @Bairro, @Endereco, @Numero, @Complemento); " +
                                                    $"SELECT CAST (SCOPE_IDENTITY() AS INT)";

                        enderecoEntrega.Id = _connection.Query<int>(sqlEnderecoEntrega, enderecoEntrega, transaction)
                            .Single();
                    }
                }


                //Departamentos
                string sqlDeleteUsuariosDepartamentos = $"DELETE FROM UsuariosDepartamentos WHERE UsuarioId = @Id";
                _connection.Execute(sqlDeleteEnderecosEntrega, new {Id = entity.Id}, transaction);

                if (entity.EnderecosEntrega != null && entity.EnderecosEntrega.Count > 0)
                {
                    foreach (var departamento in entity.Departamentos)
                    {
                        string sqlUsuarioDepartamentos = $"INSERT INTO UsuariosDepartamentos " +
                                                         $"(UsuarioId, DepartamentoId) " +
                                                         $"VALUES " +
                                                         $"(@UsuarioId, @DepartamentoId); ";

                        _connection.Execute(sqlUsuarioDepartamentos, new
                            {
                                UsuarioId = entity.Id,
                                DepartamentoId = departamento.Id
                            },
                            transaction);
                    }
                }

                transaction.Commit();
            }
            catch (Exception e)
            {
                try
                {
                    transaction.Rollback();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
            finally
            {
                _connection.Close();
            }
        }

        public void Delete(int id)
        {
            try
            {
                string SQL = $"DELETE FROM Usuarios WHERE Id = @Id";
                _connection.Execute(SQL, new {Id = id});
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public Usuario pegarDiferente(int id) //Modelo menos performatico
        {
            string SQL = $"SELECT * FROM Usuarios WHERE Id = @Id;" +
                         $"SELECT * FROM Contatos WHERE UsuarioId = @Id;" +
                         $"SELECT * FROM EnderecosEntrega WHERE UsuarioId = @Id;" +
                         $"SELECT D.* FROM UsuariosDepartamentos UD INNER JOIN Departamentos D ON UD.DepartamentoId = D.Id WHERE UD.UsuarioId = @Id";

            var multipleResultSets = _connection.QueryMultiple(SQL, new {Id = id});
            Usuario usuario = multipleResultSets.Read<Usuario>().SingleOrDefault();
            Contato contato = multipleResultSets.Read<Contato>().SingleOrDefault();
            List<EnderecoEntrega> enderecos = multipleResultSets.Read<EnderecoEntrega>().ToList();
            List<Departamento> departamentos = multipleResultSets.Read<Departamento>().ToList();

            if (usuario != null)
            {
                usuario.Contato = contato;
                usuario.EnderecosEntrega = enderecos;
                usuario.Departamentos = departamentos;

                return usuario;
            }

            return null;
        }
    }
}