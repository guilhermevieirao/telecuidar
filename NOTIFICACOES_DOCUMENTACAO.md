# 📢 Sistema de Notificações - TeleCuidar

## Visão Geral

O sistema de notificações permite enviar alertas em tempo real para os usuários da plataforma. As notificações aparecem em um dropdown no header com badge contador de não lidas.

## Características

### ✨ Funcionalidades
- ✅ Dropdown com lista de notificações
- ✅ Badge com contador de não lidas
- ✅ Polling automático a cada 30 segundos
- ✅ Filtro "Todas" / "Não lidas"
- ✅ Marcar como lida ao clicar
- ✅ Links de ação clicáveis
- ✅ Paginação com "Carregar mais"
- ✅ 4 tipos visuais (info, success, warning, error)
- ✅ Formatação de tempo relativo (TimeAgo)

### 🎨 Tipos de Notificação

| Tipo | Ícone | Cor | Uso |
|------|-------|-----|-----|
| `info` | 📢 | Azul | Informações gerais |
| `success` | ✅ | Verde | Ações bem-sucedidas |
| `warning` | ⚠️ | Amarelo | Avisos importantes |
| `error` | ❌ | Vermelho | Erros e problemas |

---

## Backend API

### Endpoints Disponíveis

#### 1. Listar Notificações
```http
GET /api/notifications?pageNumber=1&pageSize=10&onlyUnread=false
Authorization: Bearer {token}
```

**Resposta:**
```json
{
  "data": [
    {
      "id": 1,
      "title": "Bem-vindo ao TeleCuidar! 🎉",
      "message": "Olá João! Sua conta foi criada com sucesso.",
      "type": "success",
      "actionUrl": "/dashboard",
      "actionText": "Ir para o painel",
      "isRead": false,
      "timeAgo": "5m atrás",
      "createdAt": "2025-11-26T10:30:00"
    }
  ],
  "pageNumber": 1,
  "pageSize": 10,
  "totalPages": 3,
  "totalCount": 25
}
```

#### 2. Contador de Não Lidas
```http
GET /api/notifications/unread-count
Authorization: Bearer {token}
```

**Resposta:**
```json
{
  "count": 5
}
```

#### 3. Criar Notificação (Admin Only)
```http
POST /api/notifications
Authorization: Bearer {token}
Content-Type: application/json

{
  "title": "Nova mensagem",
  "message": "Você recebeu uma nova mensagem de Dr. Silva.",
  "type": "info",
  "userId": 10,
  "actionUrl": "/mensagens",
  "actionText": "Ver mensagem",
  "relatedEntityType": "Message",
  "relatedEntityId": 42
}
```

#### 4. Marcar como Lida
```http
PATCH /api/notifications/1/mark-as-read
Authorization: Bearer {token}
```

---

## Frontend - Como Usar

### 1. Importar o Componente

```typescript
import { NotificationsComponent } from '../notifications/notifications.component';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, NotificationsComponent],
  // ...
})
```

### 2. Adicionar no Template

```html
<header>
  <nav>
    <app-notifications></app-notifications>
    <div class="user-info">...</div>
  </nav>
</header>
```

### 3. Usar o NotificationService

```typescript
import { NotificationService } from '../../core/services/notification.service';

export class MyComponent {
  constructor(private notificationService: NotificationService) {}

  // Obter notificações
  loadNotifications() {
    this.notificationService.getMyNotifications(1, false).subscribe({
      next: (response) => console.log(response.data),
      error: (error) => console.error(error)
    });
  }

  // Obter contador
  getUnreadCount() {
    this.notificationService.getUnreadCount().subscribe({
      next: (response) => console.log(`Não lidas: ${response.count}`),
      error: (error) => console.error(error)
    });
  }

  // Marcar como lida
  markAsRead(notificationId: number) {
    this.notificationService.markAsRead(notificationId).subscribe({
      next: () => console.log('Marcada como lida'),
      error: (error) => console.error(error)
    });
  }
}
```

---

## Criando Notificações (Backend)

### Usando MediatR Commands

```csharp
// No seu Handler ou Controller
var command = new CreateNotificationCommand
{
    Title = "Consulta agendada",
    Message = "Sua consulta foi agendada para 30/11/2025 às 14:00.",
    Type = "success",
    UserId = 10,
    ActionUrl = "/consultas",
    ActionText = "Ver detalhes",
    RelatedEntityType = "Appointment",
    RelatedEntityId = 123
};

var result = await _mediator.Send(command);
```

### Helpers do NotificationService (Frontend - Admin)

```typescript
// Apenas administradores podem criar notificações via frontend
import { NotificationService } from '../../core/services/notification.service';

export class AdminComponent {
  constructor(private notificationService: NotificationService) {}

  // Boas-vindas
  sendWelcome(userId: number, userName: string) {
    this.notificationService.notifyWelcome(userId, userName).subscribe({
      next: () => console.log('Notificação enviada'),
      error: (error) => console.error(error)
    });
  }

  // Arquivo enviado
  notifyFileUpload(userId: number, fileName: string, fileId: number) {
    this.notificationService.notifyFileUploaded(userId, fileName, fileId)
      .subscribe();
  }

  // Lembrete de consulta
  sendAppointmentReminder(userId: number, date: string) {
    this.notificationService.notifyAppointmentReminder(userId, date)
      .subscribe();
  }

  // Nova mensagem
  notifyNewMessage(userId: number, senderName: string) {
    this.notificationService.notifyNewMessage(userId, senderName)
      .subscribe();
  }

  // Status da conta
  notifyAccountStatus(userId: number, isActive: boolean) {
    this.notificationService.notifyAccountStatus(userId, isActive)
      .subscribe();
  }

  // Manutenção
  notifyMaintenance(userId: number, date: string) {
    this.notificationService.notifySystemMaintenance(userId, date)
      .subscribe();
  }

  // Erro
  notifyError(userId: number, message: string) {
    this.notificationService.notifyError(userId, message)
      .subscribe();
  }
}
```

---

## Formatação TimeAgo

O backend retorna automaticamente o campo `timeAgo` formatado:

| Tempo Decorrido | Formato |
|----------------|---------|
| < 1 minuto | "Agora" |
| 1-59 minutos | "5m atrás" |
| 1-23 horas | "2h atrás" |
| 1-6 dias | "3d atrás" |
| 1-4 semanas | "2sem atrás" |
| > 4 semanas | "26/11/2025" |

---

## Exemplos de Uso Práticos

### 1. Notificar Upload de Arquivo

```csharp
// Backend: FilesController.cs
[HttpPost("upload")]
public async Task<IActionResult> Upload([FromForm] UploadFileCommand command)
{
    var result = await _mediator.Send(command);
    
    if (result.IsSuccess)
    {
        // Criar notificação
        await _mediator.Send(new CreateNotificationCommand
        {
            Title = "Arquivo enviado com sucesso",
            Message = $"O arquivo '{command.File.FileName}' foi enviado.",
            Type = "success",
            UserId = GetCurrentUserId(),
            ActionUrl = "/arquivos",
            ActionText = "Ver arquivos",
            RelatedEntityType = "FileUpload",
            RelatedEntityId = result.Data.Id
        });
    }
    
    return Ok(result);
}
```

### 2. Notificar Mudança de Status

```csharp
// Backend: UsersController.cs
[HttpPatch("{id}/toggle-status")]
public async Task<IActionResult> ToggleStatus(int id)
{
    var result = await _mediator.Send(new ToggleUserStatusCommand { UserId = id });
    
    if (result.IsSuccess)
    {
        var user = result.Data;
        
        // Notificar o usuário
        await _mediator.Send(new CreateNotificationCommand
        {
            Title = user.IsActive ? "Conta ativada" : "Conta desativada",
            Message = user.IsActive 
                ? "Sua conta foi reativada." 
                : "Sua conta foi temporariamente desativada.",
            Type = user.IsActive ? "success" : "warning",
            UserId = id,
            ActionUrl = "/perfil",
            ActionText = "Ver perfil"
        });
    }
    
    return Ok(result);
}
```

### 3. Broadcast para Todos os Usuários

```csharp
// Backend: AdminController.cs
[HttpPost("broadcast-notification")]
[Authorize(Roles = "Administrador")]
public async Task<IActionResult> BroadcastNotification([FromBody] BroadcastRequest request)
{
    // Buscar todos os usuários ativos
    var users = await _unitOfWork.Repository<User>()
        .GetAllAsync(u => u.IsActive);
    
    // Criar notificação para cada um
    foreach (var user in users)
    {
        await _mediator.Send(new CreateNotificationCommand
        {
            Title = request.Title,
            Message = request.Message,
            Type = request.Type,
            UserId = user.Id
        });
    }
    
    return Ok(new { message = $"Notificação enviada para {users.Count()} usuários" });
}
```

---

## Estrutura de Dados

### Notification Entity

```csharp
public class Notification : BaseEntity
{
    public string Title { get; set; }
    public string Message { get; set; }
    public string Type { get; set; } // info, success, warning, error
    public string? ActionUrl { get; set; }
    public string? ActionText { get; set; }
    
    // Relacionamentos
    public int UserId { get; set; }
    public User User { get; set; }
    
    public int? CreatedByUserId { get; set; }
    public User? CreatedByUser { get; set; }
    
    // Tracking
    public bool IsRead { get; set; }
    public DateTime? ReadAt { get; set; }
    
    // Contexto
    public string? RelatedEntityType { get; set; }
    public int? RelatedEntityId { get; set; }
}
```

### Índices para Performance

```csharp
// Composite index para queries rápidas
builder.HasIndex(n => new { n.UserId, n.IsRead });

// Índices individuais
builder.HasIndex(n => n.UserId);
builder.HasIndex(n => n.IsRead);
builder.HasIndex(n => n.CreatedAt);
```

---

## Melhorias Futuras

### 🔮 Roadmap

1. **WebSockets / SignalR**
   - Notificações em tempo real sem polling
   - Reduzir carga no servidor

2. **Preferências do Usuário**
   - Desabilitar tipos específicos
   - Configurar frequência de notificações
   - Email/SMS para notificações importantes

3. **Templates de Notificação**
   - Templates reutilizáveis
   - Suporte a variáveis dinâmicas
   - Multi-idioma

4. **Notificações Agrupadas**
   - Agrupar notificações similares
   - "Você tem 5 novas mensagens"

5. **Push Notifications**
   - Notificações no navegador (Web Push API)
   - Notificações mobile (PWA)

6. **Analytics**
   - Taxa de abertura
   - Taxa de clique em ações
   - Métricas de engajamento

---

## Troubleshooting

### Notificações não aparecem?

1. Verificar autenticação (token JWT válido)
2. Verificar role do usuário (apenas admin cria)
3. Checar console do navegador para erros
4. Verificar network tab (requisições 200 OK?)

### Badge não atualiza?

- O polling automático funciona a cada 30s
- Ao abrir o dropdown, atualiza imediatamente
- Verificar se há erros no console

### Dropdown não fecha?

- Clicar no overlay (fundo escuro) fecha
- Clicar em uma notificação fecha automaticamente

---

## Testes

### Criar Notificação de Teste (Postman/Insomnia)

```http
POST http://localhost:5000/api/notifications
Authorization: Bearer {admin-token}
Content-Type: application/json

{
  "title": "🎉 Teste de Notificação",
  "message": "Esta é uma notificação de teste para validar o sistema.",
  "type": "info",
  "userId": 1,
  "actionUrl": "/dashboard",
  "actionText": "Ir para dashboard"
}
```

### Validar Contador

```http
GET http://localhost:5000/api/notifications/unread-count
Authorization: Bearer {user-token}
```

---

## Considerações de Performance

### Otimizações Implementadas

1. **Índices de Banco de Dados**
   - Composite index `(UserId, IsRead)` para queries rápidas
   - Índice em `CreatedAt` para ordenação

2. **Paginação**
   - Carrega apenas 10 notificações por vez
   - Botão "Carregar mais" para demanda

3. **Polling Inteligente**
   - Atualiza apenas contador a cada 30s
   - Lista completa só ao abrir dropdown

4. **Filtros no Backend**
   - Query `onlyUnread` reduz payload
   - SQL otimizado com LINQ

---

## Segurança

### Validações Implementadas

- ✅ JWT Bearer Authentication em todos os endpoints
- ✅ Role-based authorization (`[Authorize(Roles="Administrador")]`)
- ✅ User isolation (apenas vê suas próprias notificações)
- ✅ Validação de UserId no backend
- ✅ Sanitização de inputs

### Boas Práticas

- Nunca confiar em dados do frontend
- Sempre validar permissões no backend
- Logs de auditoria para criação de notificações
- Rate limiting para prevenir spam

---

## Conclusão

O sistema de notificações está completo e pronto para uso! 🎉

**Status:** ✅ 100% Implementado (Backend + Frontend + Migration)

**Próximos passos:**
1. Testar criação de notificações via Postman
2. Integrar com outros módulos (uploads, consultas, mensagens)
3. Considerar migração para SignalR no futuro
