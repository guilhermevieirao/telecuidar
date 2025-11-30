# Sistema de Agendamento de Consultas - TeleCuidar

## Resumo da Implementação

Foi implementado um sistema completo de agendamento de consultas para pacientes, integrado ao painel do paciente.

## Backend (.NET)

### 1. Entidade Appointment
- **Localização:** `backend/domain/Entities/Appointment.cs`
- **Campos principais:**
  - PatientId, ProfessionalId, SpecialtyId
  - AppointmentDate, AppointmentTime, DurationMinutes
  - Status (Agendado, Confirmado, Cancelado, Concluído)
  - Notes, CancellationReason

### 2. Queries Implementadas

#### GetAvailableSpecialtiesQuery
- **Localização:** `backend/application/Appointments/Queries/`
- **Funcionalidade:** Busca especialidades que possuem profissionais com agendas ativas
- **Retorno:** Lista de especialidades com contador de profissionais disponíveis

#### GetAvailableDatesQuery
- **Localização:** `backend/application/Appointments/Queries/`
- **Funcionalidade:** Busca datas disponíveis para uma especialidade nos próximos 30 dias
- **Validações:**
  - Verifica agendas ativas dos profissionais
  - Considera horários já agendados
  - Calcula slots disponíveis por dia

#### GetAvailableTimeSlotsQuery
- **Localização:** `backend/application/Appointments/Queries/`
- **Funcionalidade:** Busca horários disponíveis para uma especialidade em uma data específica
- **Validações:**
  - Considera horário de expediente
  - Respeita pausas configuradas
  - Verifica disponibilidade por profissional
  - Lista profissionais disponíveis para cada horário

#### GetPatientAppointmentsQuery
- **Localização:** `backend/application/Appointments/Queries/`
- **Funcionalidade:** Busca agendamentos do paciente
- **Filtros:** Opção de incluir consultas passadas

### 3. Commands Implementados

#### CreateAppointmentCommand
- **Localização:** `backend/application/Appointments/Commands/`
- **Validações implementadas:**
  - ✅ Paciente existe
  - ✅ Profissional existe e possui a especialidade
  - ✅ Profissional tem agenda ativa para o dia
  - ✅ Horário está dentro do expediente
  - ✅ Horário não está no período de pausa
  - ✅ Horário não está ocupado
  - ✅ Data e horário são válidos

### 4. API Controller
- **Localização:** `backend/api/Controllers/AppointmentsController.cs`
- **Endpoints:**
  - `GET /api/appointments/available-specialties` - Lista especialidades disponíveis
  - `GET /api/appointments/available-dates?specialtyId={id}&daysAhead={days}` - Busca datas
  - `GET /api/appointments/available-time-slots?specialtyId={id}&date={date}` - Busca horários
  - `POST /api/appointments` - Criar agendamento (apenas Paciente)
  - `GET /api/appointments/my-appointments?includePast={bool}` - Buscar agendamentos do paciente

## Frontend (Angular)

### 1. Service
- **Localização:** `frontend/src/app/core/services/appointment.service.ts`
- **Métodos:**
  - `getAvailableSpecialties()`
  - `getAvailableDates(specialtyId, daysAhead)`
  - `getAvailableTimeSlots(specialtyId, date)`
  - `createAppointment(request)`
  - `getMyAppointments(includePast)`

### 2. Componente de Agendamento
- **Localização:** `frontend/src/app/features/booking/`
- **Fluxo de Etapas:**

#### Etapa 1: Selecionar Especialidade
- Grid com cards de especialidades
- Mostra ícone, nome, descrição
- Contador de profissionais disponíveis

#### Etapa 2: Selecionar Data
- Grid com datas disponíveis
- Formato dd/MM/yyyy
- Nome do dia da semana
- Contador de horários disponíveis

#### Etapa 3: Selecionar Horário
- Grid com horários disponíveis
- Formato HH:mm
- Contador de profissionais disponíveis

#### Etapa 4: Selecionar Profissional (condicional)
- Aparece APENAS se houver múltiplos profissionais no horário
- Grid com cards de profissionais
- Foto, nome do profissional

#### Etapa 5: Confirmar
- Resumo das informações selecionadas
- Campo de observações (opcional)
- Botões: Cancelar / Confirmar Agendamento

### 3. Indicador de Progresso
- Mostra visualmente em qual etapa o usuário está
- Marca etapas concluídas
- Adapta-se dinamicamente (4 ou 5 etapas dependendo se há seleção de profissional)

### 4. Funcionalidades Adicionais
- ✅ Botão "Voltar" em todas as etapas
- ✅ Mensagens de erro/sucesso
- ✅ Loading states
- ✅ Validações de campos
- ✅ Design responsivo
- ✅ Redirecionamento automático após sucesso

### 5. Integração com Dashboard
- **Localização:** `frontend/src/app/features/dashboard/dashboard.component.html`
- Novo card "Agendar Consulta" adicionado
- Navegação para `/agendar` ao clicar
- Ícone laranja 🗓️ para destaque

### 6. Rota
- **Localização:** `frontend/src/app/app.routes.ts`
- **Path:** `/agendar`
- **Guard:** authGuard
- **Roles:** [1] (Paciente)

## Banco de Dados

### Migration Criada
- **Nome:** `AddAppointments`
- **Tabela:** `Appointments`
- **Relacionamentos:**
  - FK para Users (Patient)
  - FK para Users (Professional)
  - FK para Specialties

## Como Usar

### Para Pacientes:

1. **Acesse o painel:**
   - Login como paciente
   - No dashboard, clique em "Agendar Consulta"

2. **Selecione a especialidade:**
   - Escolha entre as especialidades com profissionais disponíveis

3. **Escolha a data:**
   - Selecione uma data disponível nos próximos 30 dias

4. **Escolha o horário:**
   - Selecione um dos horários disponíveis

5. **Escolha o profissional (se necessário):**
   - Se houver múltiplos profissionais, selecione o de sua preferência

6. **Confirme:**
   - Revise as informações
   - Adicione observações (opcional)
   - Confirme o agendamento

7. **Sucesso:**
   - Receberá mensagem de confirmação
   - Será redirecionado ao painel

### Para Administradores:

O sistema de agendas e especialidades já existente continua funcionando:
- Gerenciar especialidades
- Atribuir especialidades a profissionais
- Configurar agendas dos profissionais
- Definir horários, pausas e durações de consulta

## Próximos Passos Sugeridos

1. **Notificações:**
   - Enviar email/SMS de confirmação
   - Lembrete 24h antes da consulta

2. **Cancelamento:**
   - Permitir paciente cancelar agendamento
   - Implementar política de cancelamento

3. **Reagendamento:**
   - Permitir remarcar consulta

4. **Histórico:**
   - Visualizar consultas passadas
   - Adicionar avaliações/feedback

5. **Dashboard aprimorado:**
   - Mostrar próximas consultas no dashboard
   - Contador real de consultas agendadas

6. **Integração com Videochamada:**
   - Botão para iniciar consulta no horário agendado
   - Link de acesso direto à teleconsulta

## Observações Técnicas

- ✅ Todas as validações de negócio implementadas no backend
- ✅ Frontend com UX/UI intuitiva e responsiva
- ✅ Separação clara de responsabilidades (CQRS)
- ✅ Autenticação e autorização implementadas
- ✅ Tratamento de erros adequado
- ✅ Código documentado e organizado
