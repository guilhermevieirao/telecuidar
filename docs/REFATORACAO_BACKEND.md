# Refatoração do Backend - TeleCuidar

## Data: 27/12/2025

## Resumo

Refatoração completa do backend para usar nomes em português em todas as tabelas, colunas, rotas e entidades. Dados clínicos que estavam em JSON foram extraídos para tabelas relacionais próprias.

## Mudanças Realizadas

### 1. Entidades (Domain/Entities/) - 23 arquivos

Todas as entidades foram renomeadas para português:

| Entidade Antiga | Entidade Nova |
|-----------------|---------------|
| User | Usuario |
| PatientProfile | PerfilPaciente |
| ProfessionalProfile | PerfilProfissional |
| Specialty | Especialidade |
| Appointment | Consulta |
| Schedule | Agenda |
| ScheduleBlock | BloqueioAgenda |
| Notification | Notificacao |
| Invite | Convite |
| Prescription | Prescricao |
| MedicalCertificate | AtestadoMedico |
| Attachment | Anexo |
| AttachmentChat | AnexoChat |
| SavedCertificate | CertificadoSalvo |
| AuditLog | LogAuditoria |
| UploadMobile | UploadMobile |
| EmailVerification | VerificacaoEmail |
| ClinicalHistory | HistoricoClinico |

**Novas entidades extraídas de JSON:**
- PreConsulta (dados de pré-consulta)
- Anamnese (anamnese médica)
- RegistroSoap (registro SOAP - Subjetivo, Objetivo, Avaliação, Plano)
- DadosBiometricos (sinais vitais e medidas)

### 2. Enums (Domain/Enums/) - 6 arquivos

- `UsuarioEnums.cs` (TipoUsuario, StatusUsuario)
- `ConsultaEnums.cs` (TipoConsulta, StatusConsulta)
- `EspecialidadeEnums.cs` (StatusEspecialidade)
- `AgendaEnums.cs` (StatusAgenda, TipoBloqueio, StatusBloqueioAgenda)
- `ConviteEnums.cs` (StatusConvite)
- `AtestadoEnums.cs` (TipoAtestado)

### 3. Tabelas do Banco de Dados

Todas as tabelas foram renomeadas para português:

```
usuarios
perfis_paciente
perfis_profissional
especialidades
consultas
pre_consultas
anamneses
registros_soap
dados_biometricos
agendas
bloqueios_agenda
prescricoes
atestados_medicos
notificacoes
convites
anexos
anexos_chat
certificados_salvos
logs_auditoria
historicos_clinicos
verificacoes_email
uploads_mobile
```

### 4. Rotas da API

| Rota Antiga | Rota Nova |
|-------------|-----------|
| /api/auth | /api/autenticacao |
| /api/users | /api/usuarios |
| /api/appointments | /api/consultas |
| /api/specialties | /api/especialidades |
| /api/schedules | /api/agendas |
| /api/schedule-blocks | /api/bloqueios-agenda |
| /api/notifications | /api/notificacoes |
| /api/prescriptions | /api/prescricoes |
| /api/medical-certificates | /api/atestados |
| /api/invites | /api/convites |
| /api/audit-logs | /api/logs-auditoria |
| /api/reports | /api/relatorios |
| /api/ai | /api/ia |
| /api/attachments | /api/anexos |
| /api/certificates | /api/certificados |
| /api/health | /api/saude |
| /api/cns | /api/cns |
| /api/jitsi | /api/jitsi |
| /api/medications | /api/medicamentos |

### 5. Relacionamentos de Dados Clínicos

A Consulta agora tem relacionamentos 1:1 com:

```
Consulta (1) ←→ (1) PreConsulta
Consulta (1) ←→ (1) Anamnese
Consulta (1) ←→ (1) RegistroSoap
Consulta (1) ←→ (1) DadosBiometricos
```

### 6. Frontend

Todos os serviços Angular foram atualizados para usar as novas rotas em português.

## Arquivos Removidos

- Migrations antigas (backend/Infrastructure/Migrations/*)
- Testes antigos (backend/Tests/Unit/*, backend/Tests/Integration/*, backend/Tests/Helpers/*)
- Controllers antigos em inglês (backend/WebAPI/Controllers/*Controller.cs antigos)
- Entidades antigas em inglês

## Migration

A migration `MigracaoInicialPortugues` foi criada e aplicada com sucesso.

## Como Executar

```bash
# Backend
cd backend
dotnet build
dotnet run --project WebAPI

# Frontend
cd frontend
npm install
npm run build
npm start
```

## Notas

- Os testes precisam ser reescritos para usar as novas entidades
- O banco de dados foi recriado do zero
- Dados de seed são inseridos automaticamente na primeira execução
