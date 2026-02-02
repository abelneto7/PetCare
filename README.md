# PetCare+

Sistema completo para gestão de pets, tutores e controle de vacinação em clínicas veterinárias. Desenvolvido seguindo os princípios de **Clean Architecture** para garantir escalabilidade, testabilidade e separação de responsabilidades.

## 🏗 Arquitetura

O projeto está estruturado em 4 camadas principais:

1.  **PetCare.Domain**: Núcleo do sistema. Contém Entidades, Enums e Interfaces. Não possui dependências externas.
2.  **PetCare.Application**: Camada de lógica de negócio e orquestração. Contém os Serviços (`AuthService`, `VacinaService`).
3.  **PetCare.Infrastructure**: Implementação de acesso a dados (EF Core, SQL Server) e Repositórios.
4.  **PetCare.Web**: Interface do usuário (MVC), responsável apenas por exibir dados e receber input.

---

## 🚦 Regras de Negócio e Funcionalidades

### 1. Autenticação e Segurança
*   **Cadastro**: Usuários devem informar Nome, Email e Senha. O email deve ser único no sistema.
*   **Login**: Acesso via email e senha.
*   **Segurança**: Senhas são criptografadas (hash) antes de serem salvas no banco. O sistema utiliza Cookies para gerenciar a sessão do usuário.

### 2. Gestão de Vacinas
O sistema calcula automaticamente o status da vacina com base na data da próxima dose:

*   🔴 **Atrasada**: Se a data da próxima dose for **anterior** à data de hoje.
*   🟡 **Vence em Breve**: Se a data da próxima dose for **nos próximos 7 dias** (inclusive hoje).
*   🟢 **Em Dia**: Se a data da próxima dose for posterior a 7 dias a partir de hoje.

*Cálculo da Próxima Dose*: Data da Aplicação + Intervalo (em dias).

### 3. Dashboard
A tela inicial exibe um resumo da clínica:
*   Total de Tutores cadastrados.
*   Total de Pets cadastrados.
*   Contador e listagem das **Vacinas Atrasadas**.
*   ⚠️ Alertas visuais para vacinas vencidas.

### 4. Cadastros (CRUDs)
*   **Tutores**: Nome, telefone, email e endereço.
*   **Pets**: Nome, espécie (Cão, Gato, Outros), raça e vinculação obrigatória a um Tutor existente.
*   **Vacinas**: Registro histórico de vacinas aplicadas, vinculado a um Pet.

---

## 🛠 Tecnologias Utilizadas

*   **.NET 8.0**
*   **ASP.NET Core MVC**
*   **Entity Framework Core** (SQL Server)
*   **Dependency Injection** (Nativo do .NET)
*   **Bootstrap 5** (Interface Visual)

---

## 🚀 Como Rodar o Projeto

### Pré-requisitos
*   [.NET SDK 8.0](https://dotnet.microsoft.com/download) instalado.
*   SQL Server (LocalDB ou instância dedicada).

### Passo a Passo

1.  **Clonar o repositório**
    ```bash
    git clone https://github.com/seu-usuario/petcare.git
    cd PetCare
    ```

2.  **Configurar Banco de Dados**
    Verifique a string de conexão em `PetCare.Web/appsettings.json`. O padrão é usar o LocalDB:
    ```json
    "ConnectionStrings": {
      "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=PetCareDb;Trusted_Connection=True;MultipleActiveResultSets=true"
    }
    ```

3.  **Aplicar Migrations**
    Execute o comando abaixo na raiz da solução para criar o banco de dados:
    ```bash
    dotnet ef database update --project PetCare.Infrastructure --startup-project PetCare.Web
    ```

4.  **Executar a Aplicação**
    ```bash
    cd PetCare.Web
    dotnet run
    ```

5.  **Acessar**
    Abra o navegador em `http://localhost:5122` (ou na porta indicada no terminal).

---

## 💡 Estrutura de Pastas

*   `/PetCare.Domain`: Entidades (`Pet`, `Tutor`...), Interfaces (`IPetRepository`...).
*   `/PetCare.Application`: Serviços (`VacinaService`...), DTOs (se houver).
*   `/PetCare.Infrastructure`: `AppDbContext`, Implementação dos Repositórios.
*   `/PetCare.Web`: Controllers, Views, wwwroot (CSS/JS).

---
*Desenvolvido com foco em Clean Code e SOLID.*
