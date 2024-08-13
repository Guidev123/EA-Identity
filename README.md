# CustomIdentity 🔒

 <h1>API de Autenticação de Usuários</h1>
    <p>Esta é uma API de autenticação de usuários desenvolvida utilizando <strong>Identity</strong> e <strong>JSON Web Token (JWT)</strong>. Esta API permite a criação de contas de usuários, login, e proteção de rotas através de tokens JWT. A API é projetada para ser utilizada em aplicativos que requerem autenticação segura e gerenciamento de usuários. Eu utilizo está API em todos meus projetos que necessitam de autenticação de usuarios.</p>
    
  <h2>Tecnologias Utilizadas 💻</h2>
    <ul>
        <li><strong>C#⚙️</strong></li>
        <li><strong>.NET⚙️</strong></li>
        <li><strong>ASP.NET⚙️</strong></li>
        <li><strong>SQL Server⚙️</strong></li>
        <li><strong>Identity⚙️</strong></li>
        <li><strong>JSON Web Token (JWT)⚙️</strong></li>
    </ul>

https://github.com/Guidev123/CustomIdentity/assets/155389912/4db42260-ee20-466e-9084-4aa0f45be8c5

https://github.com/Guidev123/CustomIdentity/assets/155389912/fe74e45d-3508-4f63-b602-e1220b1c0e4c




<h1>Tutorial para Rodar o Projeto Localmente</h1>
<p>Este guia irá orientá-lo a configurar e rodar o projeto localmente. Certifique-se de seguir cada etapa cuidadosamente.</p>

  <h2>Pré-requisitos</h2>
    <ul>
        <li>.NET Core SDK</li>
              <li>SQL Server</li>
    </ul>

  <h2>Passo a Passo</h2>

  <h3>1. Clonar o Repositório</h3>
    <p>Primeiro, clone o repositório do projeto para o seu ambiente local:</p>
    <pre><code>git clone 
    </code></pre>

  <h3>2. Criar o Banco de Dados</h3>
    <p>Use o comando <code>update-database</code> para criar o banco de dados:</p>
    <pre><code>dotnet ef database update
    </code></pre>

  <h3>3. Ajustar as Configurações de Inicialização</h3>
    <p>Certifique-se de que todos os projetos estão configurados para inicializar no ambiente de desenvolvimento (DEV). Abra o arquivo de configuração de inicialização e ajuste as configurações conforme necessário. No arquivo <code>launchSettings.json</code> de cada projeto, verifique se o ambiente está definido como <code>Development</code>.</p>
    <pre><code>{
    "IIS Express DEV": {
      "commandName": "IISExpress",
      "launchBrowser": true,
      "launchUrl": "swagger",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    },
    "IIS Express PROD": {
      "commandName": "IISExpress",
      "launchBrowser": true,
      "launchUrl": "swagger",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Production"
      }
    },
    </code></pre>


  <h3>5. Inicializar o Projeto</h3>
    <p>Por fim, inicialize o projeto no ambiente de desenvolvimento. No terminal, navegue até a pasta de cada projeto e execute:</p>
    <pre><code>dotnet run
    </code></pre>
