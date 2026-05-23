using System.ComponentModel.DataAnnotations;

namespace MedicaWeb.Models
{

    public enum TipoSanguineoEnum
    {
        [Display(Name = "A+")]
        APositivo,

        [Display(Name = "A-")]
        ANegativo,

        [Display(Name = "B+")]
        BPositivo,

        [Display(Name = "B-")]
        BNegativo,

        [Display(Name = "AB+")]
        ABPositivo,

        [Display(Name = "AB-")]
        ABNegativo,

        [Display(Name = "O+")]
        OPositivo,

        [Display(Name = "O-")]
        ONegativo
    }

    public enum SexoEnum
    {
        [Display(Name = "Masculino")]
        M,

        [Display(Name = "Feminino")]
        F
    }

    public enum EscolaridadeEnum
    {
        [Display(Name = "Analfabeto")]
        Analfabeto,

        [Display(Name = "Fundamental Incompleto")]
        FundamentalIncompleto,

        [Display(Name = "Fundamental Completo")]
        FundamentalCompleto,

        [Display(Name = "Médio Incompleto")]
        MedioIncompleto,

        [Display(Name = "Médio Completo")]
        MedioCompleto,

        [Display(Name = "Superior Incompleto")]
        SuperiorIncompleto,

        [Display(Name = "Superior Completo")]
        SuperiorCompleto,

        [Display(Name = "Pós-Graduação")]
        PosGraduacao,

        [Display(Name = "Mestrado")]
        Mestrado,

        [Display(Name = "Doutorado")]
        Doutorado
    }

    public class PacienteViewModel
    {
        public uint Id { get; set; }

        [Display(Name = "Nome")]
        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(60, ErrorMessage = "O nome deve ter no máximo 60 caracteres.")]
        public string Nome { get; set; } = null!;

        [Display(Name = "CPF")]
        [Required(ErrorMessage = "O CPF é obrigatório.")]
        [StringLength(11, ErrorMessage = "O CPF deve ter no máximo 11 caracteres.")]
        public string Cpf { get; set; } = null!;

        [Display(Name = "Cartão SUS")]
        [StringLength(15, ErrorMessage = "O Cartão SUS deve ter no máximo 15 caracteres.")]
        public string? CartaoSus { get; set; }

        [Display(Name = "Tipo Sanguíneo")]
        [Required(ErrorMessage = "O tipo sanguíneo é obrigatório.")]
        public TipoSanguineoEnum? TipoSanguineo { get; set; }

        [Display(Name = "Peso")]
        [Range(0.0, 500.0, ErrorMessage = "Informe um peso válido.")]
        public float? Peso { get; set; }

        [Display(Name = "Altura")]
        [Range(0.0, 3.0, ErrorMessage = "Informe uma altura válida.")]
        public float? Altura { get; set; }

        /// <summary>
        /// 'M' = Masculino;
        /// 'F' = Feminino.
        /// </summary>
        [Display(Name = "Sexo")]
        [Required(ErrorMessage = "O sexo é obrigatório.")]
        public SexoEnum? Sexo { get; set; }

        [Display(Name = "Apelido")]
        [StringLength(60, ErrorMessage = "O apelido deve ter no máximo 60 caracteres.")]
        public string? Apelido { get; set; }

        [Display(Name = "Possui alergia a medicamento")]
        [Required(ErrorMessage = "Informe se possui alergia a medicamento.")]
        public sbyte AlergiaMedicamento { get; set; }

        [Display(Name = "Escolaridade")]
        [Required(ErrorMessage = "A escolaridade é obrigatória.")]
        public EscolaridadeEnum? Escolaridade { get; set; }

        [Display(Name = "Possui deficiência")]
        [Required(ErrorMessage = "Informe se possui deficiência.")]
        public sbyte PossuiDeficiencia { get; set; }

        [Display(Name = "CEP")]
        [Required(ErrorMessage = "O CEP é obrigatório.")]
        [StringLength(8, ErrorMessage = "O CEP deve ter no máximo 8 caracteres.")]
        public string Cep { get; set; } = null!;

        [Display(Name = "Rua")]
        [Required(ErrorMessage = "A rua é obrigatória.")]
        [StringLength(60, ErrorMessage = "A rua deve ter no máximo 60 caracteres.")]
        public string Rua { get; set; } = null!;

        [Display(Name = "Bairro")]
        [Required(ErrorMessage = "O bairro é obrigatório.")]
        [StringLength(60, ErrorMessage = "O bairro deve ter no máximo 60 caracteres.")]
        public string Bairro { get; set; } = null!;

        [Display(Name = "Identificador")]
        [Required(ErrorMessage = "O identificador é obrigatório.")]
        [StringLength(30, ErrorMessage = "O identificador deve ter no máximo 30 caracteres.")]
        public string Identificador { get; set; } = null!;

        [Display(Name = "Cidade")]
        [Required(ErrorMessage = "A cidade é obrigatória.")]
        [StringLength(30, ErrorMessage = "A cidade deve ter no máximo 30 caracteres.")]
        public string Cidade { get; set; } = null!;

        [Display(Name = "Estado")]
        [Required(ErrorMessage = "O estado é obrigatório.")]
        [StringLength(2, ErrorMessage = "O estado deve ter no máximo 2 caracteres.")]
        public string Estado { get; set; } = null!;

        [Display(Name = "Complemento")]
        [Required(ErrorMessage = "O complemento é obrigatório.")]
        [StringLength(60, ErrorMessage = "O complemento deve ter no máximo 60 caracteres.")]
        public string Complemento { get; set; } = null!;

        [Display(Name = "DDD")]
        [Required(ErrorMessage = "O DDD é obrigatório.")]
        [StringLength(2, ErrorMessage = "O DDD deve ter no máximo 2 caracteres.")]
        public string Ddd { get; set; } = null!;

        [Display(Name = "Telefone")]
        [Required(ErrorMessage = "O telefone é obrigatório.")]
        [StringLength(9, ErrorMessage = "O telefone deve ter no máximo 9 caracteres.")]
        public string Telefone { get; set; } = null!;

        [Display(Name = "DDD do Responsável")]
        [StringLength(2, ErrorMessage = "O DDD do responsável deve ter no máximo 2 caracteres.")]
        public string? DddResponsavel { get; set; }

        [Display(Name = "Telefone do Responsável")]
        [StringLength(9, ErrorMessage = "O telefone do responsável deve ter no máximo 9 caracteres.")]
        public string? TelefoneResponsavel { get; set; }

        [Display(Name = "Nome do Responsável")]
        [StringLength(60, ErrorMessage = "O nome do responsável deve ter no máximo 60 caracteres.")]
        public string? NomeTelefoneResponsavel { get; set; }

        [Display(Name = "Foto")]
        public byte[]? Foto { get; set; }
    }
}
