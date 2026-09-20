# ServiceDesk Engine

API RESTful para la gestión centralizada de tickets de soporte técnico e incidentes IT construida con **.NET 10** y **PostgreSQL 16**.

El proyecto implementa una arquitectura limpia con separación clara de responsabilidades, seguridad orientada a roles (RBAC) mediante JWT, validación estricta de payloads y persistencia aislada en contenedores Docker.

---

## 🛠️ Stack y Tecnologías

* **Runtime & Framework:** .NET 10 (Web API / Controllers)
* **Seguridad:** JWT (JSON Web Tokens) & RBAC (Role-Based Access Control)
* **Persistencia:** Entity Framework Core 10 (PostgreSQL 16)
* **Validación:** FluentValidation
* **Infraestructura:** Docker & Docker Compose

---

## 📂 Estructura del Proyecto

```text
ServiceDeskEngine/
├── docker-compose.yml
├── README.md
├── .gitignore
└── ServiceDeskEngine.Api/
    ├── Controllers/          # Endpoints HTTP (Auth, Tickets, Users)
    ├── Data/                 # DbContext y Migraciones EF Core
    ├── Dtos/                 # Data Transfer Objects
    ├── Models/               # Entidades de Dominio
    ├── Services/             # Lógica de negocio y Autenticación JWT
    ├── Validators/           # Reglas de validación con FluentValidation
    ├── Dockerfile            # Compilación multicapa de .NET 10
    └── .dockerignore         # Exclusión de binarios y temporales

Detener servicios: docker compose down

Reiniciar limpiando volumen de datos: docker compose down -v
