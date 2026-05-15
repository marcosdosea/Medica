using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Core;

public partial class MedicaContext : DbContext
{
    public MedicaContext()
    {
    }

    public MedicaContext(DbContextOptions<MedicaContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Alergium> Alergia { get; set; }

    public virtual DbSet<Cuidador> Cuidadors { get; set; }

    public virtual DbSet<Deficiencium> Deficiencia { get; set; }

    public virtual DbSet<Medicamento> Medicamentos { get; set; }

    public virtual DbSet<Paciente> Pacientes { get; set; }

    public virtual DbSet<Planejamento> Planejamentos { get; set; }

    public virtual DbSet<Prescricao> Prescricaos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySQL("server=localhost;port=3306;user=root;password=root;database=Medica");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Alergium>(entity =>
        {
            entity.HasKey(e => new { e.IdPaciente, e.IdCuidador }).HasName("PRIMARY");

            entity.ToTable("alergia");

            entity.HasIndex(e => e.IdCuidador, "fk_Paciente_has_Cuidador_Cuidador1_idx");

            entity.HasIndex(e => e.IdPaciente, "fk_Paciente_has_Cuidador_Paciente_idx");

            entity.Property(e => e.IdPaciente).HasColumnName("idPaciente");
            entity.Property(e => e.IdCuidador).HasColumnName("idCuidador");
            entity.Property(e => e.Parentesco)
                .HasMaxLength(30)
                .HasColumnName("parentesco");

            entity.HasOne(d => d.IdCuidadorNavigation).WithMany(p => p.Alergia)
                .HasForeignKey(d => d.IdCuidador)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_Paciente_has_Cuidador_Cuidador1");

            entity.HasOne(d => d.IdPacienteNavigation).WithMany(p => p.Alergia)
                .HasForeignKey(d => d.IdPaciente)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_Paciente_has_Cuidador_Paciente");
        });

        modelBuilder.Entity<Cuidador>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("cuidador");

            entity.HasIndex(e => e.Cpf, "cpf_UNIQUE").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Cpf)
                .HasMaxLength(11)
                .HasColumnName("cpf");
            entity.Property(e => e.Foto)
                .HasColumnType("blob")
                .HasColumnName("foto");
            entity.Property(e => e.Nome)
                .HasMaxLength(60)
                .HasColumnName("nome");
        });

        modelBuilder.Entity<Deficiencium>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("deficiencia");

            entity.HasIndex(e => e.IdPaciente, "fk_Deficiencia_Paciente1_idx");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Descricao)
                .HasMaxLength(200)
                .HasColumnName("descricao");
            entity.Property(e => e.IdPaciente).HasColumnName("idPaciente");

            entity.HasOne(d => d.IdPacienteNavigation).WithMany(p => p.Deficiencia)
                .HasForeignKey(d => d.IdPaciente)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_Deficiencia_Paciente1");
        });

        modelBuilder.Entity<Medicamento>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("medicamento");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Apelido)
                .HasMaxLength(60)
                .HasColumnName("apelido");
            entity.Property(e => e.FormaFarmaceutica)
                .HasColumnType("enum('Inalante','Injetável','Creme','Líquido','Supositório','Ingerir')")
                .HasColumnName("formaFarmaceutica");
            entity.Property(e => e.Foto)
                .HasColumnType("blob")
                .HasColumnName("foto");
            entity.Property(e => e.Nome)
                .HasMaxLength(60)
                .HasColumnName("nome");
        });

        modelBuilder.Entity<Paciente>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("paciente");

            entity.HasIndex(e => e.CartaoSus, "cartaoSus_UNIQUE").IsUnique();

            entity.HasIndex(e => e.Cpf, "cpf_UNIQUE").IsUnique();

            entity.HasIndex(e => e.Nome, "idx_nome");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AlergiaMedicamento).HasColumnName("alergiaMedicamento");
            entity.Property(e => e.Altura).HasColumnName("altura");
            entity.Property(e => e.Apelido)
                .HasMaxLength(60)
                .HasColumnName("apelido");
            entity.Property(e => e.Bairro)
                .HasMaxLength(60)
                .HasColumnName("bairro");
            entity.Property(e => e.CartaoSus)
                .HasMaxLength(15)
                .HasColumnName("cartaoSus");
            entity.Property(e => e.Cep)
                .HasMaxLength(8)
                .HasColumnName("cep");
            entity.Property(e => e.Cidade)
                .HasMaxLength(30)
                .HasColumnName("cidade");
            entity.Property(e => e.Complemento)
                .HasMaxLength(60)
                .HasColumnName("complemento");
            entity.Property(e => e.Cpf)
                .HasMaxLength(11)
                .HasColumnName("cpf");
            entity.Property(e => e.Ddd)
                .HasMaxLength(2)
                .HasColumnName("ddd");
            entity.Property(e => e.DddResponsavel)
                .HasMaxLength(2)
                .HasColumnName("dddResponsavel");
            entity.Property(e => e.Escolaridade)
                .HasColumnType("enum('Analfabeto','FundamentalIncompleto','FundamentalCompleto','MedioIncompleto','MedioCompleto','SuperiorIncompleto','SuperiorCompleto','PosGraduacao','Mestrado','Doutorado')")
                .HasColumnName("escolaridade");
            entity.Property(e => e.Estado)
                .HasMaxLength(2)
                .HasColumnName("estado");
            entity.Property(e => e.Foto)
                .HasColumnType("blob")
                .HasColumnName("foto");
            entity.Property(e => e.Identificador)
                .HasMaxLength(30)
                .HasColumnName("identificador");
            entity.Property(e => e.Nome)
                .HasMaxLength(60)
                .HasColumnName("nome");
            entity.Property(e => e.NomeTelefoneResponsavel)
                .HasMaxLength(60)
                .HasColumnName("nomeTelefoneResponsavel");
            entity.Property(e => e.Peso).HasColumnName("peso");
            entity.Property(e => e.PossuiDeficiencia).HasColumnName("possuiDeficiencia");
            entity.Property(e => e.Rua)
                .HasMaxLength(60)
                .HasColumnName("rua");
            entity.Property(e => e.Sexo)
                .HasComment("'M' = Masculino;\n'F' = Feminino.")
                .HasColumnType("enum('M','F')")
                .HasColumnName("sexo");
            entity.Property(e => e.Telefone)
                .HasMaxLength(9)
                .HasColumnName("telefone");
            entity.Property(e => e.TelefoneResponsavel)
                .HasMaxLength(9)
                .HasColumnName("telefoneResponsavel");
            entity.Property(e => e.TipoSanguineo)
                .HasColumnType("enum('A+','A-','B+','B-','AB+','AB-','O+','O-')")
                .HasColumnName("tipoSanguineo");
        });

        modelBuilder.Entity<Planejamento>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("planejamento");

            entity.HasIndex(e => new { e.IdPaciente, e.IdMedicamento }, "fk_Planejamento_Planejamento1_idx");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DataConfirmacao)
                .HasColumnType("date")
                .HasColumnName("dataConfirmacao");
            entity.Property(e => e.DataFim)
                .HasColumnType("date")
                .HasColumnName("dataFim");
            entity.Property(e => e.DataInicio)
                .HasColumnType("date")
                .HasColumnName("dataInicio");
            entity.Property(e => e.Dosagem)
                .HasMaxLength(30)
                .HasColumnName("dosagem");
            entity.Property(e => e.Foto)
                .HasColumnType("blob")
                .HasColumnName("foto");
            entity.Property(e => e.Frequencia)
                .HasColumnType("time")
                .HasColumnName("frequencia");
            entity.Property(e => e.HoraAtual)
                .HasColumnType("time")
                .HasColumnName("horaAtual");
            entity.Property(e => e.HoraConfirmacao)
                .HasColumnType("time")
                .HasColumnName("horaConfirmacao");
            entity.Property(e => e.HoraInicio)
                .HasColumnType("time")
                .HasColumnName("horaInicio");
            entity.Property(e => e.IdMedicamento).HasColumnName("idMedicamento");
            entity.Property(e => e.IdPaciente).HasColumnName("idPaciente");
            entity.Property(e => e.Latitude)
                .HasPrecision(10, 8)
                .HasColumnName("latitude");
            entity.Property(e => e.Longitude)
                .HasPrecision(11, 8)
                .HasColumnName("longitude");

            entity.HasOne(d => d.Prescricao).WithMany(p => p.Planejamentos)
                .HasForeignKey(d => new { d.IdPaciente, d.IdMedicamento })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_Planejamento_Planejamento1");
        });

        modelBuilder.Entity<Prescricao>(entity =>
        {
            entity.HasKey(e => new { e.IdPaciente, e.IdMedicamento }).HasName("PRIMARY");

            entity.ToTable("prescricao");

            entity.HasIndex(e => e.IdMedicamento, "fk_Paciente_has_Medicamento_Medicamento1_idx");

            entity.HasIndex(e => e.IdPaciente, "fk_Paciente_has_Medicamento_Paciente1_idx");

            entity.Property(e => e.IdPaciente).HasColumnName("idPaciente");
            entity.Property(e => e.IdMedicamento).HasColumnName("idMedicamento");

            entity.HasOne(d => d.IdMedicamentoNavigation).WithMany(p => p.Prescricaos)
                .HasForeignKey(d => d.IdMedicamento)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_Paciente_has_Medicamento_Medicamento1");

            entity.HasOne(d => d.IdPacienteNavigation).WithMany(p => p.Prescricaos)
                .HasForeignKey(d => d.IdPaciente)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_Paciente_has_Medicamento_Paciente1");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
