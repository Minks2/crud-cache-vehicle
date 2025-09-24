# API de Gerenciamento de Veículos com Cache em Redis

### Descrição
API REST desenvolvida em .NET 8 para realizar um CRUD de veículos, utilizando MySQL como banco de dados e Redis como camada de cache para otimizar a performance das consultas. Este projeto foi criado como parte do estudo sobre performance de microsserviços e boas práticas de desenvolvimento.

---

## 🚀 Tecnologias Utilizadas
-   **.NET 8** (SDK)
-   **ASP.NET Core** (Web API)
-   **C#**
-   **Docker**
-   **MySQL** (Banco de dados relacional via Docker)
-   **Redis** (Banco de dados em memória para cache via Docker)
-   **Dapper** (Micro-ORM para acesso a dados)
-   **Swagger/OpenAPI** (Documentação da API)

---

## 📋 Pré-requisitos
Antes de começar, você vai precisar ter instalado em sua máquina:

-   [.NET 8 SDK](https://dotnet.microsoft.com/pt-br/download/dotnet/8.0)
-   [Docker Desktop](https://www.docker.com/products/docker-desktop/)
-   Um cliente de API, como [Postman](https://www.postman.com/) ou [Insomnia](https://insomnia.rest/download).

---

## ⚙️ Como Rodar o Projeto

Siga os passos abaixo para executar a aplicação localmente.

**1. Clone o Repositório**
```bash
git clone [https://github.com/Minks2/crud-cache-vehicle.git](https://github.com/Minks2/crud-cache-vehicle.git)
cd crud-cache-vehicle
```

**2. Inicie as Dependências com Docker**
Abra um terminal e execute os comandos abaixo para iniciar os contêineres do MySQL e do Redis.
```bash
# Iniciar o MySQL
docker run --name database-mysql -e MYSQL_ROOT_PASSWORD=123 -p 3306:3306 -d mysql

# Iniciar o Redis
docker run --name redis -p 6379:6379 -d redis
```

**3. Configure o Banco de Dados**
Conecte-se ao banco de dados MySQL (via DBeaver, MySQL Workbench, etc.) e execute o script abaixo.
```sql
CREATE DATABASE IF NOT EXISTS vehicle_db;
USE vehicle_db;
CREATE TABLE IF NOT EXISTS vehicles (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Brand VARCHAR(100) NOT NULL,
    Model VARCHAR(100) NOT NULL,
    Year INT NOT NULL,
    Plate VARCHAR(10) NOT NULL UNIQUE
);
```

**4. Confie no Certificado de Desenvolvimento**
Para evitar erros de certificado HTTPS no ambiente local, execute este comando em um terminal **como Administrador**:
```bash
dotnet dev-certs https --trust
```

**5. Execute a Aplicação**
Você pode rodar o projeto de duas formas:
-   **Pelo Visual Studio 2022:** Abra o arquivo `.sln` e pressione `F5`.
-   **Pelo terminal:** Navegue até a pasta do projeto (`crud-cache-vehicle/crud-cache-vehicle`) e execute:
    ```bash
    dotnet run
    ```
A documentação interativa do Swagger estará disponível em `https://localhost:<PORTA>/swagger`.

---

## Endpoints da API

-   `GET /api/vehicles`: Retorna uma lista de todos os veículos. (A resposta é cacheada por 10 minutos).
-   `POST /api/vehicles`: Cria um novo veículo. (Invalida o cache).
-   `PUT /api/vehicles/{id}`: Atualiza um veículo existente. (Invalida o cache).
-   `DELETE /api/vehicles/{id}`: Deleta um veículo. (Invalida o cache).

**Exemplo de corpo (body) para `POST` e `PUT`:**
```json
{
  "brand": "Fiat",
  "model": "Toro",
  "year": 2023,
  "plate": "BRA2E19"
}
```
