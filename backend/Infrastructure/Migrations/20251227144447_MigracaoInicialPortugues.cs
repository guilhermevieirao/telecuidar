using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MigracaoInicialPortugues : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "especialidades",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Nome = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Descricao = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    CamposPersonalizadosJson = table.Column<string>(type: "TEXT", nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_especialidades", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "usuarios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    SenhaHash = table.Column<string>(type: "TEXT", nullable: false),
                    Nome = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Sobrenome = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Cpf = table.Column<string>(type: "TEXT", maxLength: 14, nullable: false),
                    Telefone = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    Avatar = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Tipo = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    EmailVerificado = table.Column<bool>(type: "INTEGER", nullable: false),
                    TokenVerificacaoEmail = table.Column<string>(type: "TEXT", nullable: true),
                    ExpiracaoTokenVerificacaoEmail = table.Column<DateTime>(type: "TEXT", nullable: true),
                    TokenVerificacaoEmailExpira = table.Column<DateTime>(type: "TEXT", nullable: true),
                    EmailPendente = table.Column<string>(type: "TEXT", nullable: true),
                    TokenEmailPendente = table.Column<string>(type: "TEXT", nullable: true),
                    ExpiracaoTokenEmailPendente = table.Column<DateTime>(type: "TEXT", nullable: true),
                    NovoEmailPendente = table.Column<string>(type: "TEXT", nullable: true),
                    TokenTrocaEmail = table.Column<string>(type: "TEXT", nullable: true),
                    TokenTrocaEmailExpira = table.Column<DateTime>(type: "TEXT", nullable: true),
                    TokenResetSenha = table.Column<string>(type: "TEXT", nullable: true),
                    ExpiracaoTokenResetSenha = table.Column<DateTime>(type: "TEXT", nullable: true),
                    TokenRedefinicaoSenha = table.Column<string>(type: "TEXT", nullable: true),
                    TokenRedefinicaoSenhaExpira = table.Column<DateTime>(type: "TEXT", nullable: true),
                    RefreshToken = table.Column<string>(type: "TEXT", nullable: true),
                    ExpiracaoRefreshToken = table.Column<DateTime>(type: "TEXT", nullable: true),
                    RefreshTokenExpira = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "agendas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ProfissionalId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ConfiguracaoGlobalJson = table.Column<string>(type: "TEXT", nullable: false),
                    ConfiguracaoDiasJson = table.Column<string>(type: "TEXT", nullable: false),
                    DataInicioVigencia = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DataFimVigencia = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DataInicioValidade = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DataFimValidade = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Ativa = table.Column<bool>(type: "INTEGER", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_agendas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_agendas_usuarios_ProfissionalId",
                        column: x => x.ProfissionalId,
                        principalTable: "usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "bloqueios_agenda",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ProfissionalId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Tipo = table.Column<int>(type: "INTEGER", nullable: false),
                    Data = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DataInicio = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DataFim = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Motivo = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    AprovadoPorId = table.Column<Guid>(type: "TEXT", nullable: true),
                    AprovadoEm = table.Column<DateTime>(type: "TEXT", nullable: true),
                    MotivoRejeicao = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_bloqueios_agenda", x => x.Id);
                    table.ForeignKey(
                        name: "FK_bloqueios_agenda_usuarios_AprovadoPorId",
                        column: x => x.AprovadoPorId,
                        principalTable: "usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_bloqueios_agenda_usuarios_ProfissionalId",
                        column: x => x.ProfissionalId,
                        principalTable: "usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "certificados_salvos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Nome = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    ProfissionalId = table.Column<Guid>(type: "TEXT", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "TEXT", nullable: false),
                    NomeSujeito = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    NomeEmissor = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    ValidoDe = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ValidoAte = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ImpressaoDigital = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    DadosPfxCriptografados = table.Column<string>(type: "TEXT", nullable: false),
                    RequerSenhaAoUsar = table.Column<bool>(type: "INTEGER", nullable: false),
                    SenhaCriptografada = table.Column<string>(type: "TEXT", nullable: true),
                    CaminhoArquivo = table.Column<string>(type: "TEXT", nullable: true),
                    Apelido = table.Column<string>(type: "TEXT", nullable: false),
                    NomeProprietario = table.Column<string>(type: "TEXT", nullable: true),
                    CpfCnpj = table.Column<string>(type: "TEXT", nullable: true),
                    DataValidade = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Thumbprint = table.Column<string>(type: "TEXT", nullable: true),
                    HashSenha = table.Column<string>(type: "TEXT", nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_certificados_salvos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_certificados_salvos_usuarios_ProfissionalId",
                        column: x => x.ProfissionalId,
                        principalTable: "usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "consultas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    PacienteId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ProfissionalId = table.Column<Guid>(type: "TEXT", nullable: false),
                    EspecialidadeId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Data = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Horario = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    HorarioFim = table.Column<TimeSpan>(type: "TEXT", nullable: true),
                    Tipo = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    Observacao = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    LinkVideo = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    CamposEspecialidadeJson = table.Column<string>(type: "TEXT", nullable: true),
                    ResumoIA = table.Column<string>(type: "TEXT", nullable: true),
                    ResumoIAGeradoEm = table.Column<DateTime>(type: "TEXT", nullable: true),
                    HipoteseDiagnosticaIA = table.Column<string>(type: "TEXT", nullable: true),
                    HipoteseDiagnosticaIAGeradaEm = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_consultas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_consultas_especialidades_EspecialidadeId",
                        column: x => x.EspecialidadeId,
                        principalTable: "especialidades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_consultas_usuarios_PacienteId",
                        column: x => x.PacienteId,
                        principalTable: "usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_consultas_usuarios_ProfissionalId",
                        column: x => x.ProfissionalId,
                        principalTable: "usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "convites",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    TipoUsuario = table.Column<int>(type: "INTEGER", nullable: false),
                    EspecialidadeId = table.Column<Guid>(type: "TEXT", nullable: true),
                    Token = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    ExpiraEm = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CriadoPorId = table.Column<Guid>(type: "TEXT", nullable: false),
                    AceitoEm = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_convites", x => x.Id);
                    table.ForeignKey(
                        name: "FK_convites_especialidades_EspecialidadeId",
                        column: x => x.EspecialidadeId,
                        principalTable: "especialidades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_convites_usuarios_CriadoPorId",
                        column: x => x.CriadoPorId,
                        principalTable: "usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "historicos_clinicos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    PacienteId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CondicoesCronicas = table.Column<string>(type: "TEXT", nullable: true),
                    Alergias = table.Column<string>(type: "TEXT", nullable: true),
                    MedicamentosUsoContinuo = table.Column<string>(type: "TEXT", nullable: true),
                    CirurgiasAnteriores = table.Column<string>(type: "TEXT", nullable: true),
                    HistoricoFamiliar = table.Column<string>(type: "TEXT", nullable: true),
                    Observacoes = table.Column<string>(type: "TEXT", nullable: true),
                    TipoSanguineo = table.Column<string>(type: "TEXT", nullable: true),
                    AlergiasJson = table.Column<string>(type: "TEXT", nullable: true),
                    MedicamentosEmUsoJson = table.Column<string>(type: "TEXT", nullable: true),
                    DoencasCronicasJson = table.Column<string>(type: "TEXT", nullable: true),
                    CirurgiasAnterioresJson = table.Column<string>(type: "TEXT", nullable: true),
                    HistoricoFamiliarJson = table.Column<string>(type: "TEXT", nullable: true),
                    VacinacoesJson = table.Column<string>(type: "TEXT", nullable: true),
                    HabitosSociaisJson = table.Column<string>(type: "TEXT", nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_historicos_clinicos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_historicos_clinicos_usuarios_PacienteId",
                        column: x => x.PacienteId,
                        principalTable: "usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "logs_auditoria",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "TEXT", nullable: true),
                    Acao = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    TipoEntidade = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Entidade = table.Column<string>(type: "TEXT", nullable: false),
                    EntidadeId = table.Column<string>(type: "TEXT", nullable: true),
                    ValoresAntigos = table.Column<string>(type: "TEXT", nullable: true),
                    ValoresNovos = table.Column<string>(type: "TEXT", nullable: true),
                    DadosAntigos = table.Column<string>(type: "TEXT", nullable: true),
                    DadosNovos = table.Column<string>(type: "TEXT", nullable: true),
                    EnderecoIp = table.Column<string>(type: "TEXT", maxLength: 45, nullable: true),
                    UserAgent = table.Column<string>(type: "TEXT", nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_logs_auditoria", x => x.Id);
                    table.ForeignKey(
                        name: "FK_logs_auditoria_usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "notificacoes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Titulo = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Mensagem = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    Tipo = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Lida = table.Column<bool>(type: "INTEGER", nullable: false),
                    Link = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notificacoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_notificacoes_usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "perfis_paciente",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Cns = table.Column<string>(type: "TEXT", maxLength: 15, nullable: true),
                    NomeSocial = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Sexo = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    DataNascimento = table.Column<DateTime>(type: "TEXT", nullable: true),
                    NomeMae = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    NomePai = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Nacionalidade = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Cep = table.Column<string>(type: "TEXT", maxLength: 10, nullable: true),
                    Endereco = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Cidade = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Estado = table.Column<string>(type: "TEXT", maxLength: 2, nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_perfis_paciente", x => x.Id);
                    table.ForeignKey(
                        name: "FK_perfis_paciente_usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "perfis_profissional",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Crm = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    Cbo = table.Column<string>(type: "TEXT", maxLength: 10, nullable: true),
                    EspecialidadeId = table.Column<Guid>(type: "TEXT", nullable: true),
                    TipoRegistro = table.Column<string>(type: "TEXT", nullable: true),
                    NumeroRegistro = table.Column<string>(type: "TEXT", nullable: true),
                    Sexo = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    DataNascimento = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Nacionalidade = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Cep = table.Column<string>(type: "TEXT", maxLength: 10, nullable: true),
                    Endereco = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Cidade = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Estado = table.Column<string>(type: "TEXT", maxLength: 2, nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_perfis_profissional", x => x.Id);
                    table.ForeignKey(
                        name: "FK_perfis_profissional_especialidades_EspecialidadeId",
                        column: x => x.EspecialidadeId,
                        principalTable: "especialidades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_perfis_profissional_usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "verificacoes_email",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Token = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Tipo = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    ExpiraEm = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Utilizado = table.Column<bool>(type: "INTEGER", nullable: false),
                    UtilizadoEm = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_verificacoes_email", x => x.Id);
                    table.ForeignKey(
                        name: "FK_verificacoes_email_usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "anamneses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ConsultaId = table.Column<Guid>(type: "TEXT", nullable: false),
                    QueixaPrincipal = table.Column<string>(type: "TEXT", nullable: true),
                    HistoriaDoencaAtual = table.Column<string>(type: "TEXT", nullable: true),
                    HistoriaPatologicaPregressa = table.Column<string>(type: "TEXT", nullable: true),
                    HistoriaFamiliar = table.Column<string>(type: "TEXT", nullable: true),
                    HabitosVida = table.Column<string>(type: "TEXT", nullable: true),
                    RevisaoSistemas = table.Column<string>(type: "TEXT", nullable: true),
                    MedicamentosEmUso = table.Column<string>(type: "TEXT", nullable: true),
                    Alergias = table.Column<string>(type: "TEXT", nullable: true),
                    Observacoes = table.Column<string>(type: "TEXT", nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_anamneses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_anamneses_consultas_ConsultaId",
                        column: x => x.ConsultaId,
                        principalTable: "consultas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "anexos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ConsultaId = table.Column<Guid>(type: "TEXT", nullable: false),
                    EnviadoPorId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Titulo = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    NomeArquivo = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    NomeOriginal = table.Column<string>(type: "TEXT", nullable: false),
                    CaminhoArquivo = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    TipoArquivo = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    TipoMime = table.Column<string>(type: "TEXT", nullable: false),
                    TamanhoArquivo = table.Column<long>(type: "INTEGER", nullable: false),
                    TamanhoBytes = table.Column<long>(type: "INTEGER", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_anexos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_anexos_consultas_ConsultaId",
                        column: x => x.ConsultaId,
                        principalTable: "consultas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_anexos_usuarios_EnviadoPorId",
                        column: x => x.EnviadoPorId,
                        principalTable: "usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "anexos_chat",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ConsultaId = table.Column<Guid>(type: "TEXT", nullable: false),
                    RemetenteId = table.Column<Guid>(type: "TEXT", nullable: false),
                    EnviadoPorId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Mensagem = table.Column<string>(type: "TEXT", nullable: true),
                    NomeArquivo = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    NomeOriginal = table.Column<string>(type: "TEXT", nullable: false),
                    CaminhoArquivo = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    TipoArquivo = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    TipoMime = table.Column<string>(type: "TEXT", nullable: false),
                    TamanhoArquivo = table.Column<long>(type: "INTEGER", nullable: true),
                    TamanhoBytes = table.Column<long>(type: "INTEGER", nullable: false),
                    EnviadoEm = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_anexos_chat", x => x.Id);
                    table.ForeignKey(
                        name: "FK_anexos_chat_consultas_ConsultaId",
                        column: x => x.ConsultaId,
                        principalTable: "consultas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_anexos_chat_usuarios_EnviadoPorId",
                        column: x => x.EnviadoPorId,
                        principalTable: "usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_anexos_chat_usuarios_RemetenteId",
                        column: x => x.RemetenteId,
                        principalTable: "usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "atestados_medicos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ConsultaId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ProfissionalId = table.Column<Guid>(type: "TEXT", nullable: false),
                    PacienteId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Tipo = table.Column<int>(type: "INTEGER", nullable: false),
                    DataEmissao = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DataInicio = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DataFim = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DiasAfastamento = table.Column<int>(type: "INTEGER", nullable: true),
                    DiasTotais = table.Column<int>(type: "INTEGER", nullable: false),
                    AssinadoDigitalmente = table.Column<bool>(type: "INTEGER", nullable: false),
                    CertificadoId = table.Column<Guid>(type: "TEXT", nullable: true),
                    DataAssinatura = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Cid = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    Conteudo = table.Column<string>(type: "TEXT", nullable: false),
                    Observacoes = table.Column<string>(type: "TEXT", nullable: true),
                    AssinaturaDigital = table.Column<string>(type: "TEXT", maxLength: 10000, nullable: true),
                    ImpressaoDigitalCertificado = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    SubjetoCertificado = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    AssinadoEm = table.Column<DateTime>(type: "TEXT", nullable: true),
                    HashDocumento = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    PdfAssinadoBase64 = table.Column<string>(type: "TEXT", nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_atestados_medicos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_atestados_medicos_consultas_ConsultaId",
                        column: x => x.ConsultaId,
                        principalTable: "consultas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_atestados_medicos_usuarios_PacienteId",
                        column: x => x.PacienteId,
                        principalTable: "usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_atestados_medicos_usuarios_ProfissionalId",
                        column: x => x.ProfissionalId,
                        principalTable: "usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "dados_biometricos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ConsultaId = table.Column<Guid>(type: "TEXT", nullable: false),
                    PressaoArterial = table.Column<string>(type: "TEXT", nullable: true),
                    FrequenciaCardiaca = table.Column<string>(type: "TEXT", nullable: true),
                    FrequenciaRespiratoria = table.Column<string>(type: "TEXT", nullable: true),
                    Temperatura = table.Column<string>(type: "TEXT", nullable: true),
                    SaturacaoOxigenio = table.Column<string>(type: "TEXT", nullable: true),
                    Peso = table.Column<string>(type: "TEXT", nullable: true),
                    Altura = table.Column<string>(type: "TEXT", nullable: true),
                    Imc = table.Column<string>(type: "TEXT", nullable: true),
                    CircunferenciaAbdominal = table.Column<string>(type: "TEXT", nullable: true),
                    Glicemia = table.Column<string>(type: "TEXT", nullable: true),
                    TipoGlicemia = table.Column<string>(type: "TEXT", nullable: true),
                    Observacoes = table.Column<string>(type: "TEXT", nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dados_biometricos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dados_biometricos_consultas_ConsultaId",
                        column: x => x.ConsultaId,
                        principalTable: "consultas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "pre_consultas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ConsultaId = table.Column<Guid>(type: "TEXT", nullable: false),
                    NomeCompleto = table.Column<string>(type: "TEXT", nullable: true),
                    DataNascimento = table.Column<string>(type: "TEXT", nullable: true),
                    Peso = table.Column<string>(type: "TEXT", nullable: true),
                    Altura = table.Column<string>(type: "TEXT", nullable: true),
                    CondicoesCronicas = table.Column<string>(type: "TEXT", nullable: true),
                    Medicamentos = table.Column<string>(type: "TEXT", nullable: true),
                    Alergias = table.Column<string>(type: "TEXT", nullable: true),
                    Cirurgias = table.Column<string>(type: "TEXT", nullable: true),
                    ObservacoesHistorico = table.Column<string>(type: "TEXT", nullable: true),
                    Tabagismo = table.Column<string>(type: "TEXT", nullable: true),
                    ConsumoAlcool = table.Column<string>(type: "TEXT", nullable: true),
                    AtividadeFisica = table.Column<string>(type: "TEXT", nullable: true),
                    ObservacoesHabitos = table.Column<string>(type: "TEXT", nullable: true),
                    PressaoArterial = table.Column<string>(type: "TEXT", nullable: true),
                    FrequenciaCardiaca = table.Column<string>(type: "TEXT", nullable: true),
                    Temperatura = table.Column<string>(type: "TEXT", nullable: true),
                    SaturacaoOxigenio = table.Column<string>(type: "TEXT", nullable: true),
                    ObservacoesSinaisVitais = table.Column<string>(type: "TEXT", nullable: true),
                    SintomasPrincipais = table.Column<string>(type: "TEXT", nullable: true),
                    InicioSintomas = table.Column<string>(type: "TEXT", nullable: true),
                    IntensidadeDor = table.Column<int>(type: "INTEGER", nullable: true),
                    ObservacoesSintomas = table.Column<string>(type: "TEXT", nullable: true),
                    ObservacoesAdicionais = table.Column<string>(type: "TEXT", nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pre_consultas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_pre_consultas_consultas_ConsultaId",
                        column: x => x.ConsultaId,
                        principalTable: "consultas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "prescricoes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ConsultaId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ProfissionalId = table.Column<Guid>(type: "TEXT", nullable: false),
                    PacienteId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Tipo = table.Column<string>(type: "TEXT", nullable: true),
                    Observacoes = table.Column<string>(type: "TEXT", nullable: true),
                    ValidadeEmDias = table.Column<int>(type: "INTEGER", nullable: true),
                    AssinadoDigitalmente = table.Column<bool>(type: "INTEGER", nullable: false),
                    CertificadoId = table.Column<Guid>(type: "TEXT", nullable: true),
                    DataAssinatura = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ItensJson = table.Column<string>(type: "TEXT", nullable: false),
                    AssinaturaDigital = table.Column<string>(type: "TEXT", maxLength: 10000, nullable: true),
                    ImpressaoDigitalCertificado = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    SubjetoCertificado = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    AssinadoEm = table.Column<DateTime>(type: "TEXT", nullable: true),
                    HashDocumento = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    PdfAssinadoBase64 = table.Column<string>(type: "TEXT", nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_prescricoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_prescricoes_consultas_ConsultaId",
                        column: x => x.ConsultaId,
                        principalTable: "consultas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_prescricoes_usuarios_PacienteId",
                        column: x => x.PacienteId,
                        principalTable: "usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_prescricoes_usuarios_ProfissionalId",
                        column: x => x.ProfissionalId,
                        principalTable: "usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "registros_soap",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ConsultaId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Subjetivo = table.Column<string>(type: "TEXT", nullable: true),
                    Objetivo = table.Column<string>(type: "TEXT", nullable: true),
                    Avaliacao = table.Column<string>(type: "TEXT", nullable: true),
                    Plano = table.Column<string>(type: "TEXT", nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_registros_soap", x => x.Id);
                    table.ForeignKey(
                        name: "FK_registros_soap_consultas_ConsultaId",
                        column: x => x.ConsultaId,
                        principalTable: "consultas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "uploads_mobile",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Token = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    UsuarioId = table.Column<Guid>(type: "TEXT", nullable: true),
                    ConsultaId = table.Column<Guid>(type: "TEXT", nullable: true),
                    Origem = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    NomeArquivo = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    CaminhoArquivo = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    TipoArquivo = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    TamanhoArquivo = table.Column<long>(type: "INTEGER", nullable: false),
                    Processado = table.Column<bool>(type: "INTEGER", nullable: false),
                    ExpiraEm = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_uploads_mobile", x => x.Id);
                    table.ForeignKey(
                        name: "FK_uploads_mobile_consultas_ConsultaId",
                        column: x => x.ConsultaId,
                        principalTable: "consultas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_uploads_mobile_usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_agendas_ProfissionalId",
                table: "agendas",
                column: "ProfissionalId");

            migrationBuilder.CreateIndex(
                name: "IX_anamneses_ConsultaId",
                table: "anamneses",
                column: "ConsultaId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_anexos_ConsultaId",
                table: "anexos",
                column: "ConsultaId");

            migrationBuilder.CreateIndex(
                name: "IX_anexos_EnviadoPorId",
                table: "anexos",
                column: "EnviadoPorId");

            migrationBuilder.CreateIndex(
                name: "IX_anexos_chat_ConsultaId",
                table: "anexos_chat",
                column: "ConsultaId");

            migrationBuilder.CreateIndex(
                name: "IX_anexos_chat_EnviadoPorId",
                table: "anexos_chat",
                column: "EnviadoPorId");

            migrationBuilder.CreateIndex(
                name: "IX_anexos_chat_RemetenteId",
                table: "anexos_chat",
                column: "RemetenteId");

            migrationBuilder.CreateIndex(
                name: "IX_atestados_medicos_ConsultaId",
                table: "atestados_medicos",
                column: "ConsultaId");

            migrationBuilder.CreateIndex(
                name: "IX_atestados_medicos_HashDocumento",
                table: "atestados_medicos",
                column: "HashDocumento");

            migrationBuilder.CreateIndex(
                name: "IX_atestados_medicos_PacienteId",
                table: "atestados_medicos",
                column: "PacienteId");

            migrationBuilder.CreateIndex(
                name: "IX_atestados_medicos_ProfissionalId",
                table: "atestados_medicos",
                column: "ProfissionalId");

            migrationBuilder.CreateIndex(
                name: "IX_bloqueios_agenda_AprovadoPorId",
                table: "bloqueios_agenda",
                column: "AprovadoPorId");

            migrationBuilder.CreateIndex(
                name: "IX_bloqueios_agenda_ProfissionalId",
                table: "bloqueios_agenda",
                column: "ProfissionalId");

            migrationBuilder.CreateIndex(
                name: "IX_bloqueios_agenda_Status",
                table: "bloqueios_agenda",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_certificados_salvos_ImpressaoDigital",
                table: "certificados_salvos",
                column: "ImpressaoDigital");

            migrationBuilder.CreateIndex(
                name: "IX_certificados_salvos_ProfissionalId",
                table: "certificados_salvos",
                column: "ProfissionalId");

            migrationBuilder.CreateIndex(
                name: "IX_consultas_Data",
                table: "consultas",
                column: "Data");

            migrationBuilder.CreateIndex(
                name: "IX_consultas_EspecialidadeId",
                table: "consultas",
                column: "EspecialidadeId");

            migrationBuilder.CreateIndex(
                name: "IX_consultas_PacienteId",
                table: "consultas",
                column: "PacienteId");

            migrationBuilder.CreateIndex(
                name: "IX_consultas_ProfissionalId",
                table: "consultas",
                column: "ProfissionalId");

            migrationBuilder.CreateIndex(
                name: "IX_consultas_Status",
                table: "consultas",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_convites_CriadoPorId",
                table: "convites",
                column: "CriadoPorId");

            migrationBuilder.CreateIndex(
                name: "IX_convites_Email",
                table: "convites",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_convites_EspecialidadeId",
                table: "convites",
                column: "EspecialidadeId");

            migrationBuilder.CreateIndex(
                name: "IX_convites_Token",
                table: "convites",
                column: "Token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_dados_biometricos_ConsultaId",
                table: "dados_biometricos",
                column: "ConsultaId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_especialidades_Nome",
                table: "especialidades",
                column: "Nome");

            migrationBuilder.CreateIndex(
                name: "IX_historicos_clinicos_PacienteId",
                table: "historicos_clinicos",
                column: "PacienteId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_logs_auditoria_Acao",
                table: "logs_auditoria",
                column: "Acao");

            migrationBuilder.CreateIndex(
                name: "IX_logs_auditoria_CriadoEm",
                table: "logs_auditoria",
                column: "CriadoEm");

            migrationBuilder.CreateIndex(
                name: "IX_logs_auditoria_UsuarioId",
                table: "logs_auditoria",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_notificacoes_Lida",
                table: "notificacoes",
                column: "Lida");

            migrationBuilder.CreateIndex(
                name: "IX_notificacoes_UsuarioId",
                table: "notificacoes",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_perfis_paciente_Cns",
                table: "perfis_paciente",
                column: "Cns");

            migrationBuilder.CreateIndex(
                name: "IX_perfis_paciente_UsuarioId",
                table: "perfis_paciente",
                column: "UsuarioId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_perfis_profissional_Crm",
                table: "perfis_profissional",
                column: "Crm");

            migrationBuilder.CreateIndex(
                name: "IX_perfis_profissional_EspecialidadeId",
                table: "perfis_profissional",
                column: "EspecialidadeId");

            migrationBuilder.CreateIndex(
                name: "IX_perfis_profissional_UsuarioId",
                table: "perfis_profissional",
                column: "UsuarioId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_pre_consultas_ConsultaId",
                table: "pre_consultas",
                column: "ConsultaId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_prescricoes_ConsultaId",
                table: "prescricoes",
                column: "ConsultaId");

            migrationBuilder.CreateIndex(
                name: "IX_prescricoes_HashDocumento",
                table: "prescricoes",
                column: "HashDocumento");

            migrationBuilder.CreateIndex(
                name: "IX_prescricoes_PacienteId",
                table: "prescricoes",
                column: "PacienteId");

            migrationBuilder.CreateIndex(
                name: "IX_prescricoes_ProfissionalId",
                table: "prescricoes",
                column: "ProfissionalId");

            migrationBuilder.CreateIndex(
                name: "IX_registros_soap_ConsultaId",
                table: "registros_soap",
                column: "ConsultaId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_uploads_mobile_ConsultaId",
                table: "uploads_mobile",
                column: "ConsultaId");

            migrationBuilder.CreateIndex(
                name: "IX_uploads_mobile_ExpiraEm",
                table: "uploads_mobile",
                column: "ExpiraEm");

            migrationBuilder.CreateIndex(
                name: "IX_uploads_mobile_Token",
                table: "uploads_mobile",
                column: "Token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_uploads_mobile_UsuarioId",
                table: "uploads_mobile",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_Cpf",
                table: "usuarios",
                column: "Cpf",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_Email",
                table: "usuarios",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_Telefone",
                table: "usuarios",
                column: "Telefone");

            migrationBuilder.CreateIndex(
                name: "IX_verificacoes_email_Token",
                table: "verificacoes_email",
                column: "Token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_verificacoes_email_UsuarioId",
                table: "verificacoes_email",
                column: "UsuarioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "agendas");

            migrationBuilder.DropTable(
                name: "anamneses");

            migrationBuilder.DropTable(
                name: "anexos");

            migrationBuilder.DropTable(
                name: "anexos_chat");

            migrationBuilder.DropTable(
                name: "atestados_medicos");

            migrationBuilder.DropTable(
                name: "bloqueios_agenda");

            migrationBuilder.DropTable(
                name: "certificados_salvos");

            migrationBuilder.DropTable(
                name: "convites");

            migrationBuilder.DropTable(
                name: "dados_biometricos");

            migrationBuilder.DropTable(
                name: "historicos_clinicos");

            migrationBuilder.DropTable(
                name: "logs_auditoria");

            migrationBuilder.DropTable(
                name: "notificacoes");

            migrationBuilder.DropTable(
                name: "perfis_paciente");

            migrationBuilder.DropTable(
                name: "perfis_profissional");

            migrationBuilder.DropTable(
                name: "pre_consultas");

            migrationBuilder.DropTable(
                name: "prescricoes");

            migrationBuilder.DropTable(
                name: "registros_soap");

            migrationBuilder.DropTable(
                name: "uploads_mobile");

            migrationBuilder.DropTable(
                name: "verificacoes_email");

            migrationBuilder.DropTable(
                name: "consultas");

            migrationBuilder.DropTable(
                name: "especialidades");

            migrationBuilder.DropTable(
                name: "usuarios");
        }
    }
}
