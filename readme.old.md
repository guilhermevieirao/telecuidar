# 🏥 SusAtende - Sistema de Telecuidado

**Cuidado de Saúde Digital Inteligente**

O SusAtende é uma plataforma integrada de telecuidado que conecta pacientes, profissionais e administradores em um ambiente digital seguro e eficiente. O sistema foi desenvolvido para modernizar e facilitar o acesso aos cuidados de saúde, oferecendo ferramentas completas para gestão médica e acompanhamento de pacientes.

## 📋 Índice

-   [Sobre o Projeto](#sobre-o-projeto)
-   [Funcionalidades Implementadas](#funcionalidades-implementadas)
-   [Funcionalidades Futuras](#funcionalidades-futuras)
-   [Tecnologias Utilizadas](#tecnologias-utilizadas)
-   [Estrutura do Sistema](#estrutura-do-sistema)
-   [Instalação](#instalação)
-   [Uso](#uso)
-   [Contribuição](#contribuição)
-   [Licença](#licença)

## 🎯 Sobre o Projeto

O SusAtende nasceu da necessidade de conectar pacientes, profissionais e administradores em uma plataforma única, segura e eficiente. Nossa missão é tornar o cuidado em saúde mais acessível, organizado e humanizado através da tecnologia.

Com foco na experiência do usuário e na segurança dos dados médicos, o sistema oferece ferramentas que simplificam processos, melhoram a comunicação e garantem que cada pessoa receba o cuidado que merece.

## ✅ Funcionalidades Implementadas

### 👤 Sistema de Usuários

-   **Autenticação segura** com diferentes tipos de usuário
- **Três perfis distintos**: Paciente, Profissional e Administrador
-   **Gestão de perfis** com dados específicos para cada tipo de usuário
-   **Sistema de permissões** baseado em roles
-   **Painel Unificado**: Interface única para todos os tipos de usuários

### 🏥 Área do Paciente

-   **Painel personalizado** com visão geral dos dados de saúde
-   **Histórico clínico completo** com consultas, diagnósticos e tratamentos
-   **Resultados de exames** organizados por data e tipo
-   **Prescrições médicas** com dosagens, instruções e observações
-   **Busca de profissionais** por nome ou especialidade
-   **Sistema de agendamento** de consultas por especialidade
-   **Navegação intuitiva** com botões de retorno ao painel

### 👨‍⚕️ Área do profissional

-   **Painel profissional** com estatísticas e informações relevantes
-   **Gestão de agenda** com horários flexíveis e configurações personalizadas
-   **Especialidades médicas** categorizadas e organizadas
-   **Histórico de atendimentos** e pacientes
-   **Emissão de prescrições** e solicitação de exames

### 🔧 Área Administrativa

-   **Painel administrativo completo** com estatísticas do sistema
-   **Gestão de usuários** (pacientes, profissionais e administradores)
-   **Controle de especialidades médicas**
-   **Sistema de agendas** para profissionais
-   **Relatórios e métricas** do sistema
-   **Configurações avançadas** da plataforma

### 📊 Sistema de Dados Médicos

-   **Histórico clínico detalhado** com:

    -   Data e tipo de consulta
    -   Diagnósticos e observações
    -   Medicamentos prescritos
    -   Exames solicitados
    -   Procedimentos realizados
    -   Sinais vitais (peso, altura, pressão arterial, temperatura)

-   **Resultados de exames** com:

    -   Tipos variados (laboratorial, imagem, cardiológico)
    -   Valores de referência
    -   Observações médicas
    -   Anexos de arquivos

-   **Prescrições médicas** incluindo:
    -   Medicamento e dosagem
    -   Frequência e duração do tratamento
    -   Instruções de uso
    -   Observações especiais

### 📅 Sistema de Agendamento

-   **Agendas flexíveis** para profissionais
-   **Configuração de horários** de trabalho e pausas
-   **Duração personalizada** de consultas
-   **Intervalos entre consultas** configuráveis
-   **Validação de conflitos** de horários
-   **Períodos de validade** das agendas

## 🚀 Funcionalidades Futuras

### 📹 Videochamadas

-   **Consultas virtuais** em tempo real
-   **Interface intuitiva** para chamadas de vídeo
-   **Gravação de sessões** (quando autorizado)
-   **Chat integrado** durante as consultas
-   **Compartilhamento de tela** para visualização de exames

### 🌐 Integração IoT - Parâmetros Biométricos

-   **Monitoramento em tempo real** de sinais vitais
-   **Dispositivos conectados** para captura automática de dados:
    -   Pressão arterial
    -   Frequência cardíaca
    -   Temperatura corporal
    -   Saturação de oxigênio
    -   Glicemia
    -   Peso e IMC
-   **Alertas automáticos** para valores fora dos parâmetros normais
-   **Histórico contínuo** de monitoramento
-   **Painel em tempo real** para profissionais

### 🤖 Anamnese Inteligente com IA

-   **Questionário dinâmico** adaptado ao perfil do paciente
-   **Inteligência artificial** para:
    -   Personalização de perguntas baseada no histórico
    -   Análise de respostas em tempo real
    -   Sugestões de investigações adicionais
    -   Identificação de padrões e riscos
-   **Processamento de linguagem natural** para respostas abertas
-   **Relatório automático** para o profissional
-   **Aprendizado contínuo** do sistema baseado nos casos

## 🛠️ Tecnologias Utilizadas

### Backend

-   **Laravel 12.25.0** - Framework PHP moderno e robusto
-   **PHP 8.2+** - Linguagem de programação
-   **MySQL/MariaDB** - Sistema de gerenciamento de banco de dados
-   **Eloquent ORM** - Mapeamento objeto-relacional

### Frontend

-   **Blade Templates** - Sistema de templates do Laravel
-   **TailwindCSS 4.0** - Framework CSS utilitário
-   **Vite 7.0** - Build tool e bundler
-   **JavaScript ES6+** - Interatividade e funcionalidades dinâmicas
-   **Axios** - Cliente HTTP para requisições AJAX

### Ferramentas de Desenvolvimento

-   **Composer** - Gerenciador de dependências PHP
-   **NPM** - Gerenciador de pacotes Node.js
-   **Laravel Pint** - Code style fixer
-   **PHPUnit** - Framework de testes
-   **Laravel Sail** - Ambiente de desenvolvimento Docker

## 🏗️ Estrutura do Sistema

### Modelos de Dados Principais

-   **User** - Usuários do sistema (base para todos os tipos)
-   **Paciente** - Dados específicos dos pacientes
-   **Profissional** - Informações dos profissionais médicos
-   **Especialidade** - Especialidades médicas disponíveis
-   **HistoricoClinico** - Registros de consultas e atendimentos
-   **ResultadoExame** - Resultados de exames médicos
-   **PrescricaoMedicamento** - Prescrições e medicamentos
-   **Consulta** - Agendamentos e consultas
-   **Agenda** - Horários de trabalho dos profissionais

### Arquitetura MVC

-   **Models** - Lógica de negócio e acesso aos dados
-   **Views** - Interface do usuário com Blade templates
-   **Controllers** - Lógica de controle e fluxo da aplicação
-   **Middlewares** - Autenticação e autorização
-   **Routes** - Definição de rotas e endpoints

## 🚀 Instalação

### Pré-requisitos

-   PHP 8.2 ou superior
-   Composer
-   Node.js e NPM
-   MySQL ou MariaDB
-   Servidor web (Apache/Nginx) ou usar o servidor embutido do Laravel

### Passos de Instalação

1. **Clone o repositório**

    ```bash
    git clone https://github.com/seu-usuario/susatende.git
    cd susatende
    ```

2. **Instale as dependências PHP**

    ```bash
    composer install
    ```

3. **Instale as dependências Node.js**

    ```bash
    npm install
    ```

4. **Configure o ambiente**

    ```bash
    cp .env.example .env
    php artisan key:generate
    ```

5. **Configure o banco de dados**

    - Edite o arquivo `.env` com suas credenciais de banco
    - Execute as migrations:

    ```bash
    php artisan migrate
    ```

6. **Execute os seeders (opcional)**

    ```bash
    php artisan db:seed
    ```

7. **Compile os assets**

    ```bash
    npm run build
    ```

8. **Inicie o servidor**
    ```bash
    php artisan serve
    ```

## 💻 Uso

### Acesso ao Sistema

-   Acesse `http://localhost:8000` em seu navegador
-   Registre-se como paciente ou faça login com credenciais existentes
-   Administradores podem gerenciar o sistema através do painel administrativo

### Tipos de Usuário

#### 👤 Paciente

-   Visualizar histórico clínico
-   Acessar resultados de exames
-   Consultar prescrições médicas
-   Buscar profissionais
-   Agendar consultas

#### 👨‍⚕️ profissional

-   Gerenciar agenda de atendimentos
-   Visualizar pacientes
-   Emitir prescrições
-   Registrar consultas
-   Solicitar exames

#### 🔧 Administrador

-   Gerenciar usuários do sistema
-   Configurar especialidades
-   Controlar agendas dos profissionais
-   Visualizar estatísticas do sistema
-   Administrar configurações gerais

## 🤝 Contribuição

Contribuições são bem-vindas! Para contribuir:

1. Faça um fork do projeto
2. Crie uma branch para sua feature (`git checkout -b feature/AmazingFeature`)
3. Commit suas mudanças (`git commit -m 'Add some AmazingFeature'`)
4. Push para a branch (`git push origin feature/AmazingFeature`)
5. Abra um Pull Request

### Diretrizes de Contribuição

-   Siga os padrões de código do Laravel
-   Escreva testes para novas funcionalidades
-   Documente mudanças significativas
-   Mantenha commits claros e descritivos

## 📄 Licença

Este projeto está licenciado sob a Licença MIT - veja o arquivo [LICENSE](LICENSE) para detalhes.

## 📞 Contato

-   **Email**: contato@susatende.com.br
-   **Telefone**: (11) 9999-9999
-   **Website**: [www.susatende.com.br](http://www.susatende.com.br)

---

**SusAtende** - Transformando o cuidado em saúde através da tecnologia 💚

_Versão 1.0.0 - Sistema em desenvolvimento ativo_