using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // ============================================
    // DbSets - Tabelas do Banco de Dados
    // ============================================
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<PerfilPaciente> PerfisPaciente { get; set; }
    public DbSet<PerfilProfissional> PerfisProfissional { get; set; }
    public DbSet<Especialidade> Especialidades { get; set; }
    public DbSet<Consulta> Consultas { get; set; }
    public DbSet<PreConsulta> PreConsultas { get; set; }
    public DbSet<Anamnese> Anamneses { get; set; }
    public DbSet<RegistroSoap> RegistrosSoap { get; set; }
    public DbSet<DadosBiometricos> DadosBiometricos { get; set; }
    public DbSet<Anexo> Anexos { get; set; }
    public DbSet<AnexoChat> AnexosChat { get; set; }
    public DbSet<Agenda> Agendas { get; set; }
    public DbSet<BloqueioAgenda> BloqueiosAgenda { get; set; }
    public DbSet<Notificacao> Notificacoes { get; set; }
    public DbSet<Convite> Convites { get; set; }
    public DbSet<LogAuditoria> LogsAuditoria { get; set; }
    public DbSet<Prescricao> Prescricoes { get; set; }
    public DbSet<AtestadoMedico> AtestadosMedicos { get; set; }
    public DbSet<CertificadoSalvo> CertificadosSalvos { get; set; }
    public DbSet<UploadMobile> UploadsMobile { get; set; }
    public DbSet<VerificacaoEmail> VerificacoesEmail { get; set; }
    public DbSet<HistoricoClinico> HistoricosClinicos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ============================================
        // USUARIO
        // ============================================
        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.ToTable("usuarios");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.Cpf).IsUnique();
            entity.HasIndex(e => e.Telefone);
            
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.Property(e => e.SenhaHash).IsRequired();
            entity.Property(e => e.Nome).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Sobrenome).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Cpf).IsRequired().HasMaxLength(14);
            entity.Property(e => e.Telefone).HasMaxLength(20);
            entity.Property(e => e.Avatar).HasMaxLength(500);

            // Relacionamento 1:1 com PerfilPaciente
            entity.HasOne(e => e.PerfilPaciente)
                .WithOne(p => p.Usuario)
                .HasForeignKey<PerfilPaciente>(p => p.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);
                
            // Relacionamento 1:1 com PerfilProfissional
            entity.HasOne(e => e.PerfilProfissional)
                .WithOne(p => p.Usuario)
                .HasForeignKey<PerfilProfissional>(p => p.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ============================================
        // PERFIL PACIENTE
        // ============================================
        modelBuilder.Entity<PerfilPaciente>(entity =>
        {
            entity.ToTable("perfis_paciente");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.UsuarioId).IsUnique();
            entity.HasIndex(e => e.Cns);
            
            entity.Property(e => e.Cns).HasMaxLength(15);
            entity.Property(e => e.NomeSocial).HasMaxLength(200);
            entity.Property(e => e.Sexo).HasMaxLength(20);
            entity.Property(e => e.NomeMae).HasMaxLength(200);
            entity.Property(e => e.NomePai).HasMaxLength(200);
            entity.Property(e => e.Nacionalidade).HasMaxLength(100);
            entity.Property(e => e.Cep).HasMaxLength(10);
            entity.Property(e => e.Endereco).HasMaxLength(500);
            entity.Property(e => e.Cidade).HasMaxLength(100);
            entity.Property(e => e.Estado).HasMaxLength(2);
        });

        // ============================================
        // PERFIL PROFISSIONAL
        // ============================================
        modelBuilder.Entity<PerfilProfissional>(entity =>
        {
            entity.ToTable("perfis_profissional");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.UsuarioId).IsUnique();
            entity.HasIndex(e => e.Crm);
            
            entity.Property(e => e.Crm).HasMaxLength(20);
            entity.Property(e => e.Cbo).HasMaxLength(10);
            entity.Property(e => e.Sexo).HasMaxLength(20);
            entity.Property(e => e.Nacionalidade).HasMaxLength(100);
            entity.Property(e => e.Cep).HasMaxLength(10);
            entity.Property(e => e.Endereco).HasMaxLength(500);
            entity.Property(e => e.Cidade).HasMaxLength(100);
            entity.Property(e => e.Estado).HasMaxLength(2);
            
            entity.HasOne(e => e.Especialidade)
                .WithMany(s => s.Profissionais)
                .HasForeignKey(e => e.EspecialidadeId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // ============================================
        // ESPECIALIDADE
        // ============================================
        modelBuilder.Entity<Especialidade>(entity =>
        {
            entity.ToTable("especialidades");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Nome);
            
            entity.Property(e => e.Nome).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Descricao).HasMaxLength(1000);
        });

        // ============================================
        // CONSULTA
        // ============================================
        modelBuilder.Entity<Consulta>(entity =>
        {
            entity.ToTable("consultas");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Data);
            entity.HasIndex(e => e.Status);
            
            entity.Property(e => e.Observacao).HasMaxLength(2000);
            entity.Property(e => e.LinkVideo).HasMaxLength(500);
            
            // Ignorar propriedades alias
            entity.Ignore(e => e.RegistroSoap);
            entity.Ignore(e => e.HoraInicio);
            entity.Ignore(e => e.HoraFim);

            entity.HasOne(e => e.Paciente)
                .WithMany(u => u.ConsultasComoPaciente)
                .HasForeignKey(e => e.PacienteId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Profissional)
                .WithMany(u => u.ConsultasComoProfissional)
                .HasForeignKey(e => e.ProfissionalId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Especialidade)
                .WithMany(s => s.Consultas)
                .HasForeignKey(e => e.EspecialidadeId)
                .OnDelete(DeleteBehavior.Restrict);
                
            // Relacionamento 1:1 com PreConsulta
            entity.HasOne(e => e.PreConsulta)
                .WithOne(p => p.Consulta)
                .HasForeignKey<PreConsulta>(p => p.ConsultaId)
                .OnDelete(DeleteBehavior.Cascade);
                
            // Relacionamento 1:1 com Anamnese
            entity.HasOne(e => e.Anamnese)
                .WithOne(a => a.Consulta)
                .HasForeignKey<Anamnese>(a => a.ConsultaId)
                .OnDelete(DeleteBehavior.Cascade);
                
            // Relacionamento 1:1 com Soap
            entity.HasOne(e => e.Soap)
                .WithOne(s => s.Consulta)
                .HasForeignKey<RegistroSoap>(s => s.ConsultaId)
                .OnDelete(DeleteBehavior.Cascade);
                
            // Relacionamento 1:1 com DadosBiometricos
            entity.HasOne(e => e.DadosBiometricos)
                .WithOne(d => d.Consulta)
                .HasForeignKey<DadosBiometricos>(d => d.ConsultaId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ============================================
        // PRE-CONSULTA
        // ============================================
        modelBuilder.Entity<PreConsulta>(entity =>
        {
            entity.ToTable("pre_consultas");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.ConsultaId).IsUnique();
        });

        // ============================================
        // ANAMNESE
        // ============================================
        modelBuilder.Entity<Anamnese>(entity =>
        {
            entity.ToTable("anamneses");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.ConsultaId).IsUnique();
        });

        // ============================================
        // REGISTRO SOAP
        // ============================================
        modelBuilder.Entity<RegistroSoap>(entity =>
        {
            entity.ToTable("registros_soap");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.ConsultaId).IsUnique();
        });

        // ============================================
        // DADOS BIOMETRICOS
        // ============================================
        modelBuilder.Entity<DadosBiometricos>(entity =>
        {
            entity.ToTable("dados_biometricos");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.ConsultaId).IsUnique();
        });

        // ============================================
        // ANEXO
        // ============================================
        modelBuilder.Entity<Anexo>(entity =>
        {
            entity.ToTable("anexos");
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Titulo).IsRequired().HasMaxLength(200);
            entity.Property(e => e.NomeArquivo).IsRequired().HasMaxLength(255);
            entity.Property(e => e.CaminhoArquivo).IsRequired().HasMaxLength(1000);
            entity.Property(e => e.TipoArquivo).IsRequired().HasMaxLength(100);

            entity.HasOne(e => e.Consulta)
                .WithMany(c => c.Anexos)
                .HasForeignKey(e => e.ConsultaId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ============================================
        // ANEXO CHAT
        // ============================================
        modelBuilder.Entity<AnexoChat>(entity =>
        {
            entity.ToTable("anexos_chat");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.ConsultaId);
            
            entity.Property(e => e.NomeArquivo).HasMaxLength(255);
            entity.Property(e => e.CaminhoArquivo).HasMaxLength(1000);
            entity.Property(e => e.TipoArquivo).HasMaxLength(100);

            entity.HasOne(e => e.Consulta)
                .WithMany(c => c.AnexosChat)
                .HasForeignKey(e => e.ConsultaId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasOne(e => e.Remetente)
                .WithMany()
                .HasForeignKey(e => e.RemetenteId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ============================================
        // AGENDA
        // ============================================
        modelBuilder.Entity<Agenda>(entity =>
        {
            entity.ToTable("agendas");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.ProfissionalId);

            entity.HasOne(e => e.Profissional)
                .WithMany(u => u.Agendas)
                .HasForeignKey(e => e.ProfissionalId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ============================================
        // BLOQUEIO AGENDA
        // ============================================
        modelBuilder.Entity<BloqueioAgenda>(entity =>
        {
            entity.ToTable("bloqueios_agenda");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.ProfissionalId);
            entity.HasIndex(e => e.Status);
            
            entity.Property(e => e.Motivo).IsRequired().HasMaxLength(500);
            entity.Property(e => e.MotivoRejeicao).HasMaxLength(500);

            entity.HasOne(e => e.Profissional)
                .WithMany(u => u.BloqueiosAgenda)
                .HasForeignKey(e => e.ProfissionalId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasOne(e => e.AprovadoPor)
                .WithMany()
                .HasForeignKey(e => e.AprovadoPorId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // ============================================
        // NOTIFICACAO
        // ============================================
        modelBuilder.Entity<Notificacao>(entity =>
        {
            entity.ToTable("notificacoes");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.UsuarioId);
            entity.HasIndex(e => e.Lida);
            
            entity.Property(e => e.Titulo).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Mensagem).IsRequired().HasMaxLength(1000);
            entity.Property(e => e.Tipo).HasMaxLength(50);
            entity.Property(e => e.Link).HasMaxLength(500);

            entity.HasOne(e => e.Usuario)
                .WithMany(u => u.Notificacoes)
                .HasForeignKey(e => e.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ============================================
        // CONVITE
        // ============================================
        modelBuilder.Entity<Convite>(entity =>
        {
            entity.ToTable("convites");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Token).IsUnique();
            entity.HasIndex(e => e.Email);
            
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.Token).IsRequired().HasMaxLength(50);

            entity.HasOne(e => e.CriadoPor)
                .WithMany()
                .HasForeignKey(e => e.CriadoPorId)
                .OnDelete(DeleteBehavior.Restrict);
                
            entity.HasOne(e => e.Especialidade)
                .WithMany()
                .HasForeignKey(e => e.EspecialidadeId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // ============================================
        // LOG AUDITORIA
        // ============================================
        modelBuilder.Entity<LogAuditoria>(entity =>
        {
            entity.ToTable("logs_auditoria");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.UsuarioId);
            entity.HasIndex(e => e.CriadoEm);
            entity.HasIndex(e => e.Acao);
            
            entity.Property(e => e.Acao).IsRequired().HasMaxLength(100);
            entity.Property(e => e.TipoEntidade).IsRequired().HasMaxLength(100);
            entity.Property(e => e.EnderecoIp).HasMaxLength(45);

            entity.HasOne(e => e.Usuario)
                .WithMany(u => u.LogsAuditoria)
                .HasForeignKey(e => e.UsuarioId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // ============================================
        // PRESCRICAO
        // ============================================
        modelBuilder.Entity<Prescricao>(entity =>
        {
            entity.ToTable("prescricoes");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.ConsultaId);
            entity.HasIndex(e => e.HashDocumento);
            
            entity.Property(e => e.AssinaturaDigital).HasMaxLength(10000);
            entity.Property(e => e.ImpressaoDigitalCertificado).HasMaxLength(100);
            entity.Property(e => e.SubjetoCertificado).HasMaxLength(500);
            entity.Property(e => e.HashDocumento).HasMaxLength(100);

            entity.HasOne(e => e.Consulta)
                .WithMany(c => c.Prescricoes)
                .HasForeignKey(e => e.ConsultaId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasOne(e => e.Profissional)
                .WithMany()
                .HasForeignKey(e => e.ProfissionalId)
                .OnDelete(DeleteBehavior.Restrict);
                
            entity.HasOne(e => e.Paciente)
                .WithMany()
                .HasForeignKey(e => e.PacienteId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ============================================
        // ATESTADO MEDICO
        // ============================================
        modelBuilder.Entity<AtestadoMedico>(entity =>
        {
            entity.ToTable("atestados_medicos");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.ConsultaId);
            entity.HasIndex(e => e.HashDocumento);
            
            entity.Property(e => e.Cid).HasMaxLength(20);
            entity.Property(e => e.Conteudo).IsRequired();
            entity.Property(e => e.AssinaturaDigital).HasMaxLength(10000);
            entity.Property(e => e.ImpressaoDigitalCertificado).HasMaxLength(100);
            entity.Property(e => e.SubjetoCertificado).HasMaxLength(500);
            entity.Property(e => e.HashDocumento).HasMaxLength(100);

            entity.HasOne(e => e.Consulta)
                .WithMany(c => c.Atestados)
                .HasForeignKey(e => e.ConsultaId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasOne(e => e.Profissional)
                .WithMany()
                .HasForeignKey(e => e.ProfissionalId)
                .OnDelete(DeleteBehavior.Restrict);
                
            entity.HasOne(e => e.Paciente)
                .WithMany()
                .HasForeignKey(e => e.PacienteId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ============================================
        // CERTIFICADO SALVO
        // ============================================
        modelBuilder.Entity<CertificadoSalvo>(entity =>
        {
            entity.ToTable("certificados_salvos");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.ProfissionalId);
            entity.HasIndex(e => e.ImpressaoDigital);
            
            entity.Property(e => e.Nome).IsRequired().HasMaxLength(200);
            entity.Property(e => e.NomeSujeito).IsRequired().HasMaxLength(500);
            entity.Property(e => e.NomeEmissor).IsRequired().HasMaxLength(500);
            entity.Property(e => e.ImpressaoDigital).IsRequired().HasMaxLength(100);
            entity.Property(e => e.DadosPfxCriptografados).IsRequired();

            entity.HasOne(e => e.Profissional)
                .WithMany()
                .HasForeignKey(e => e.ProfissionalId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ============================================
        // UPLOAD MOBILE
        // ============================================
        modelBuilder.Entity<UploadMobile>(entity =>
        {
            entity.ToTable("uploads_mobile");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Token).IsUnique();
            entity.HasIndex(e => e.ExpiraEm);
            
            entity.Property(e => e.Token).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Origem).IsRequired().HasMaxLength(50);
            entity.Property(e => e.NomeArquivo).IsRequired().HasMaxLength(255);
            entity.Property(e => e.CaminhoArquivo).IsRequired().HasMaxLength(1000);
            entity.Property(e => e.TipoArquivo).IsRequired().HasMaxLength(100);

            entity.HasOne(e => e.Usuario)
                .WithMany()
                .HasForeignKey(e => e.UsuarioId)
                .OnDelete(DeleteBehavior.SetNull);
                
            entity.HasOne(e => e.Consulta)
                .WithMany()
                .HasForeignKey(e => e.ConsultaId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // ============================================
        // VERIFICACAO EMAIL
        // ============================================
        modelBuilder.Entity<VerificacaoEmail>(entity =>
        {
            entity.ToTable("verificacoes_email");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Token).IsUnique();
            entity.HasIndex(e => e.UsuarioId);
            
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Token).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Tipo).IsRequired().HasMaxLength(50);

            entity.HasOne(e => e.Usuario)
                .WithMany()
                .HasForeignKey(e => e.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ============================================
        // HISTORICO CLINICO
        // ============================================
        modelBuilder.Entity<HistoricoClinico>(entity =>
        {
            entity.ToTable("historicos_clinicos");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.PacienteId).IsUnique();

            entity.HasOne(e => e.Paciente)
                .WithMany()
                .HasForeignKey(e => e.PacienteId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
