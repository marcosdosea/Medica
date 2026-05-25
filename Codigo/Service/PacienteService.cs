using Core;
using Core.Dto;
using Core.Service;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Service
{
    public class PacienteService : IPacienteService
    {
        private readonly MedicaContext context;

        public PacienteService(MedicaContext context)
        {
            this.context = context;
        }

        private void SanitizarDados(PacienteDetailsDto dto)
        {
            if (dto == null) return;

            if (!string.IsNullOrEmpty(dto.Cpf))
            {
                dto.Cpf = dto.Cpf.Replace(".", "").Replace("-", "").Trim();
            }

            if (!string.IsNullOrEmpty(dto.Cep))
            {
                dto.Cep = dto.Cep.Replace("-", "").Replace(".", "").Trim();
            }

            if (!string.IsNullOrEmpty(dto.Telefone))
            {
                dto.Telefone = dto.Telefone.Replace("(", "")
                                           .Replace(")", "")
                                           .Replace("-", "")
                                           .Replace(" ", "")
                                           .Trim();
            }

            if (!string.IsNullOrEmpty(dto.TelefoneResponsavel))
            {
                dto.TelefoneResponsavel = dto.TelefoneResponsavel.Replace("(", "")
                                                                .Replace(")", "")
                                                                .Replace("-", "")
                                                                .Replace(" ", "")
                                                                .Trim();
            }
        }

        public async Task<uint> CreateAsync(PacienteDetailsDto dto)
        {
            SanitizarDados(dto);

            var paciente = new Paciente
            {
                Nome = dto.Nome,
                Cpf = dto.Cpf,
                CartaoSus = dto.CartaoSus,
                TipoSanguineo = dto.TipoSanguineo,
                Peso = dto.Peso,
                Altura = dto.Altura,
                Sexo = dto.Sexo,
                Apelido = dto.Apelido,
                AlergiaMedicamento = dto.AlergiaMedicamento,
                Escolaridade = dto.Escolaridade,
                PossuiDeficiencia = dto.PossuiDeficiencia,
                Cep = dto.Cep,
                Rua = dto.Rua,
                Bairro = dto.Bairro,
                Identificador = dto.Identificador,
                Cidade = dto.Cidade,
                Estado = dto.Estado,
                Complemento = dto.Complemento,
                Ddd = dto.Ddd,
                Telefone = dto.Telefone,
                DddResponsavel = dto.DddResponsavel,
                TelefoneResponsavel = dto.TelefoneResponsavel,
                NomeTelefoneResponsavel = dto.NomeTelefoneResponsavel,
                Foto = dto.Foto,
                DataNascimento = dto.DataNascimento
            };

            await context.Pacientes.AddAsync(paciente);
            await context.SaveChangesAsync();

            return paciente.Id;
        }

        public async Task EditAsync(PacienteDetailsDto dto)
        {
            SanitizarDados(dto);

            var paciente = await context.Pacientes.FindAsync(dto.Id);
            if (paciente != null)
            {
                paciente.Nome = dto.Nome;
                paciente.Cpf = dto.Cpf;
                paciente.CartaoSus = dto.CartaoSus;
                paciente.TipoSanguineo = dto.TipoSanguineo;
                paciente.Peso = dto.Peso;
                paciente.Altura = dto.Altura;
                paciente.Sexo = dto.Sexo;
                paciente.Apelido = dto.Apelido;
                paciente.AlergiaMedicamento = dto.AlergiaMedicamento;
                paciente.Escolaridade = dto.Escolaridade;
                paciente.PossuiDeficiencia = dto.PossuiDeficiencia;
                paciente.Cep = dto.Cep;
                paciente.Rua = dto.Rua;
                paciente.Bairro = dto.Bairro;
                paciente.Identificador = dto.Identificador;
                paciente.Cidade = dto.Cidade;
                paciente.Estado = dto.Estado;
                paciente.Complemento = dto.Complemento;
                paciente.Ddd = dto.Ddd;
                paciente.Telefone = dto.Telefone;
                paciente.DddResponsavel = dto.DddResponsavel;
                paciente.TelefoneResponsavel = dto.TelefoneResponsavel;
                paciente.NomeTelefoneResponsavel = dto.NomeTelefoneResponsavel;
                if (dto.Foto != null) paciente.Foto = dto.Foto; // Mantém a foto anterior caso não envie uma nova

                context.Pacientes.Update(paciente);
                await context.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(uint id)
        {
            var paciente = await context.Pacientes.FindAsync(id);
            if (paciente != null)
            {
                context.Pacientes.Remove(paciente);
                await context.SaveChangesAsync();
            }
        }

        public async Task<PacienteDetailsDto?> GetAsync(uint id)
        {
            var p = await context.Pacientes.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
            if (p == null) return null;

            return new PacienteDetailsDto
            {
                Id = p.Id,
                Nome = p.Nome,
                Cpf = p.Cpf,
                CartaoSus = p.CartaoSus,
                TipoSanguineo = p.TipoSanguineo,
                Peso = p.Peso,
                Altura = p.Altura,
                Sexo = p.Sexo,
                Apelido = p.Apelido,
                AlergiaMedicamento = p.AlergiaMedicamento,
                Escolaridade = p.Escolaridade,
                PossuiDeficiencia = p.PossuiDeficiencia,
                Cep = p.Cep,
                Rua = p.Rua,
                Bairro = p.Bairro,
                Identificador = p.Identificador,
                Cidade = p.Cidade,
                Estado = p.Estado,
                Complemento = p.Complemento,
                Ddd = p.Ddd,
                Telefone = p.Telefone,
                DddResponsavel = p.DddResponsavel,
                TelefoneResponsavel = p.TelefoneResponsavel,
                NomeTelefoneResponsavel = p.NomeTelefoneResponsavel,
                Foto = p.Foto,
                DataNascimento = p.DataNascimento
            };
        }

        public async Task<IEnumerable<PacienteDto>> GetByMedicamentoAsync(uint idMedicamento)
        {
            return await context.Planejamentos
                .AsNoTracking()
                .Where(p => p.IdMedicamento == idMedicamento)
                .Select(p => p.IdPaciente)
                .Distinct()
                .Join(
                    context.Pacientes.AsNoTracking(),
                    idPaciente => idPaciente,
                    paciente => paciente.Id,
                    (idPaciente, paciente) => paciente
                )
                .OrderBy(p => p.Nome)
                .Select(p => new PacienteDto
                {
                    Id = p.Id,
                    Nome = p.Nome,
                    Cpf = p.Cpf,
                    Sexo = p.Sexo,
                    DataNascimento = p.DataNascimento,
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<PacienteDto>> GetAsync(string searchTerm = "")
        {
            var query = context.Pacientes.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(p => p.Nome.Contains(searchTerm) || p.Cpf.Contains(searchTerm));
            }

            return await query
                .OrderBy(p => p.Nome)
                .Select(p => new PacienteDto
                {
                    Id = p.Id,
                    Nome = p.Nome,
                    Cpf = p.Cpf,
                    Sexo = p.Sexo,
                    DataNascimento = p.DataNascimento,
                })
                .ToListAsync();
        }
    }
}