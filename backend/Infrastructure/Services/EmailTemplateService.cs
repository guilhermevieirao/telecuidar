namespace Infrastructure.Services;

/// <summary>
/// Serviço para geração de templates de e-mail
/// </summary>
public static class EmailTemplateService
{
    private const string LogoUrl = "https://telecuidar.com/logo.png";
    private const string CorPrimaria = "#4F46E5";
    private const string CorSecundaria = "#7C3AED";

    public static string GerarTemplateBase(string titulo, string conteudo)
    {
        return $@"
<!DOCTYPE html>
<html lang=""pt-BR"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>{titulo}</title>
</head>
<body style=""margin: 0; padding: 0; font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; background-color: #f3f4f6;"">
    <table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""max-width: 600px; margin: 0 auto; background-color: #ffffff;"">
        <tr>
            <td style=""background: linear-gradient(135deg, {CorPrimaria}, {CorSecundaria}); padding: 30px; text-align: center;"">
                <h1 style=""color: #ffffff; margin: 0; font-size: 28px;"">TeleCuidar</h1>
                <p style=""color: rgba(255,255,255,0.9); margin: 10px 0 0 0; font-size: 14px;"">Cuidando de você, onde estiver</p>
            </td>
        </tr>
        <tr>
            <td style=""padding: 40px 30px;"">
                {conteudo}
            </td>
        </tr>
        <tr>
            <td style=""background-color: #f9fafb; padding: 20px 30px; text-align: center; border-top: 1px solid #e5e7eb;"">
                <p style=""color: #6b7280; font-size: 12px; margin: 0;"">
                    Este é um e-mail automático. Por favor, não responda.
                </p>
                <p style=""color: #9ca3af; font-size: 11px; margin: 10px 0 0 0;"">
                    © {DateTime.Now.Year} TeleCuidar. Todos os direitos reservados.
                </p>
            </td>
        </tr>
    </table>
</body>
</html>";
    }

    public static string GerarEmailVerificacaoHtml(string nomeUsuario, string token, string frontendUrl)
    {
        var link = $"{frontendUrl}/verificar-email?token={token}";
        
        var conteudo = $@"
<h2 style=""color: #1f2937; margin: 0 0 20px 0;"">Olá, {nomeUsuario}!</h2>
<p style=""color: #4b5563; line-height: 1.6;"">
    Bem-vindo(a) ao TeleCuidar! Para concluir seu cadastro e acessar a plataforma, 
    por favor confirme seu e-mail clicando no botão abaixo:
</p>
<div style=""text-align: center; margin: 30px 0;"">
    <a href=""{link}"" style=""background: linear-gradient(135deg, {CorPrimaria}, {CorSecundaria}); color: #ffffff; padding: 14px 32px; text-decoration: none; border-radius: 8px; font-weight: 600; display: inline-block;"">
        Confirmar E-mail
    </a>
</div>
<p style=""color: #6b7280; font-size: 14px;"">
    Se o botão não funcionar, copie e cole o link abaixo no seu navegador:
</p>
<p style=""color: #4F46E5; font-size: 13px; word-break: break-all;"">{link}</p>
<p style=""color: #9ca3af; font-size: 12px; margin-top: 30px;"">
    Este link expira em 24 horas. Se você não solicitou este cadastro, ignore este e-mail.
</p>";

        return GerarTemplateBase("Confirme seu E-mail", conteudo);
    }

    public static string GerarEmailRedefinicaoSenhaHtml(string nomeUsuario, string token, string frontendUrl)
    {
        var link = $"{frontendUrl}/redefinir-senha?token={token}";
        
        var conteudo = $@"
<h2 style=""color: #1f2937; margin: 0 0 20px 0;"">Olá, {nomeUsuario}!</h2>
<p style=""color: #4b5563; line-height: 1.6;"">
    Recebemos uma solicitação para redefinir a senha da sua conta no TeleCuidar. 
    Clique no botão abaixo para criar uma nova senha:
</p>
<div style=""text-align: center; margin: 30px 0;"">
    <a href=""{link}"" style=""background: linear-gradient(135deg, {CorPrimaria}, {CorSecundaria}); color: #ffffff; padding: 14px 32px; text-decoration: none; border-radius: 8px; font-weight: 600; display: inline-block;"">
        Redefinir Senha
    </a>
</div>
<p style=""color: #6b7280; font-size: 14px;"">
    Se o botão não funcionar, copie e cole o link abaixo no seu navegador:
</p>
<p style=""color: #4F46E5; font-size: 13px; word-break: break-all;"">{link}</p>
<p style=""color: #dc2626; font-size: 12px; margin-top: 30px;"">
    ⚠️ Este link expira em 1 hora. Se você não solicitou a redefinição de senha, ignore este e-mail.
</p>";

        return GerarTemplateBase("Redefinição de Senha", conteudo);
    }

    public static string GerarEmailConviteHtml(string tipoUsuario, string token, string nomeConvidador, string frontendUrl)
    {
        var link = $"{frontendUrl}/cadastrar?convite={token}";
        var tipoTexto = tipoUsuario == "Profissional" ? "profissional de saúde" : "usuário";
        
        var conteudo = $@"
<h2 style=""color: #1f2937; margin: 0 0 20px 0;"">Você foi convidado!</h2>
<p style=""color: #4b5563; line-height: 1.6;"">
    {(string.IsNullOrEmpty(nomeConvidador) ? "Você" : nomeConvidador)} convidou você para se juntar ao TeleCuidar como {tipoTexto}.
</p>
<p style=""color: #4b5563; line-height: 1.6;"">
    Clique no botão abaixo para criar sua conta e começar a usar a plataforma:
</p>
<div style=""text-align: center; margin: 30px 0;"">
    <a href=""{link}"" style=""background: linear-gradient(135deg, {CorPrimaria}, {CorSecundaria}); color: #ffffff; padding: 14px 32px; text-decoration: none; border-radius: 8px; font-weight: 600; display: inline-block;"">
        Aceitar Convite
    </a>
</div>
<p style=""color: #9ca3af; font-size: 12px; margin-top: 30px;"">
    Este convite expira em 7 dias.
</p>";

        return GerarTemplateBase("Convite TeleCuidar", conteudo);
    }

    public static string GerarEmailConfirmacaoConsultaHtml(string nomePaciente, string nomeProfissional, string especialidade, DateTime dataConsulta, string horario)
    {
        var dataFormatada = dataConsulta.ToString("dd 'de' MMMM 'de' yyyy", new System.Globalization.CultureInfo("pt-BR"));
        
        var conteudo = $@"
<h2 style=""color: #1f2937; margin: 0 0 20px 0;"">Consulta Confirmada!</h2>
<p style=""color: #4b5563; line-height: 1.6;"">
    Olá, {nomePaciente}! Sua consulta foi agendada com sucesso.
</p>
<div style=""background-color: #f3f4f6; border-radius: 8px; padding: 20px; margin: 20px 0;"">
    <p style=""color: #374151; margin: 5px 0;""><strong>Profissional:</strong> {nomeProfissional}</p>
    <p style=""color: #374151; margin: 5px 0;""><strong>Especialidade:</strong> {especialidade}</p>
    <p style=""color: #374151; margin: 5px 0;""><strong>Data:</strong> {dataFormatada}</p>
    <p style=""color: #374151; margin: 5px 0;""><strong>Horário:</strong> {horario}</p>
</div>
<p style=""color: #6b7280; font-size: 14px;"">
    Você receberá um lembrete antes da consulta. Certifique-se de ter uma boa conexão de internet.
</p>";

        return GerarTemplateBase("Consulta Confirmada", conteudo);
    }

    public static string GerarEmailLembreteConsultaHtml(string nome, string nomeProfissional, DateTime dataConsulta, string horario, string? linkConsulta)
    {
        var dataFormatada = dataConsulta.ToString("dd 'de' MMMM 'de' yyyy", new System.Globalization.CultureInfo("pt-BR"));
        
        var botaoEntrar = string.IsNullOrEmpty(linkConsulta) ? "" : $@"
<div style=""text-align: center; margin: 30px 0;"">
    <a href=""{linkConsulta}"" style=""background: linear-gradient(135deg, {CorPrimaria}, {CorSecundaria}); color: #ffffff; padding: 14px 32px; text-decoration: none; border-radius: 8px; font-weight: 600; display: inline-block;"">
        Entrar na Consulta
    </a>
</div>";

        var conteudo = $@"
<h2 style=""color: #1f2937; margin: 0 0 20px 0;"">Lembrete de Consulta</h2>
<p style=""color: #4b5563; line-height: 1.6;"">
    Olá, {nome}! Este é um lembrete da sua consulta que acontecerá em breve.
</p>
<div style=""background-color: #fef3c7; border-left: 4px solid #f59e0b; border-radius: 4px; padding: 15px; margin: 20px 0;"">
    <p style=""color: #92400e; margin: 5px 0;""><strong>Profissional:</strong> {nomeProfissional}</p>
    <p style=""color: #92400e; margin: 5px 0;""><strong>Data:</strong> {dataFormatada}</p>
    <p style=""color: #92400e; margin: 5px 0;""><strong>Horário:</strong> {horario}</p>
</div>
{botaoEntrar}
<p style=""color: #6b7280; font-size: 14px;"">
    Prepare-se com antecedência e certifique-se de estar em um local tranquilo com boa conexão de internet.
</p>";

        return GerarTemplateBase("Lembrete de Consulta", conteudo);
    }

    public static string GerarEmailTrocaEmailHtml(string nome, string token, string frontendUrl)
    {
        var link = $"{frontendUrl}/verificar-troca-email?token={token}";
        
        var conteudo = $@"
<h2 style=""color: #1f2937; margin: 0 0 20px 0;"">Confirme seu novo e-mail</h2>
<p style=""color: #4b5563; line-height: 1.6;"">
    Olá, {nome}! Você solicitou a troca do e-mail da sua conta no TeleCuidar.
    Clique no botão abaixo para confirmar este novo endereço de e-mail:
</p>
<div style=""text-align: center; margin: 30px 0;"">
    <a href=""{link}"" style=""background: linear-gradient(135deg, {CorPrimaria}, {CorSecundaria}); color: #ffffff; padding: 14px 32px; text-decoration: none; border-radius: 8px; font-weight: 600; display: inline-block;"">
        Confirmar Novo E-mail
    </a>
</div>
<p style=""color: #dc2626; font-size: 12px; margin-top: 30px;"">
    ⚠️ Se você não solicitou esta alteração, ignore este e-mail. Sua conta permanecerá segura.
</p>";

        return GerarTemplateBase("Confirme seu Novo E-mail", conteudo);
    }
}
