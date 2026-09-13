ServiceDesk Engine API
Motor backend para la gestión de mesas de ayuda y tickets de soporte. Construido con .NET 10 Minimal APIs, PostgreSQL y containerizado con Docker Compose.

Tecnologías Utilizadas
Framework: .NET 10 (Minimal APIs)

Base de Datos: PostgreSQL 16

ORM: Entity Framework Core 10 (Npgsql)

Autenticación & Autorización: JWT con RBAC (Role-Based Access Control)

Seguridad: BCrypt.Net-Next

Validación: FluentValidation

Documentación: Scalar API Reference (/scalar/v1)

Infraestructura: Docker & Docker Compose

Control de Acceso (RBAC)
Client: Crea tickets, lee únicamente sus propios tickets y publica comentarios públicos.

Agent: Gestiona el estado y la prioridad de todos los tickets, se asigna casos y publica notas internas.

Admin: Acceso total sobre usuarios, tickets, comentarios y configuración del sistema.

Requisitos Previos
Docker Desktop (con Docker Compose activo)

.NET 10 SDK (opcional, para ejecución fuera de contenedores)

Inicio Rápido
Clonar el repositorio y navegar a la carpeta raíz:

PowerShell
git clone https://github.com/tu-usuario/ServiceDeskEngine.git
cd ServiceDeskEngine
Levantar los servicios con Docker Compose:

PowerShell
docker compose up --build -d
Acceder a la documentación interactiva en el navegador:
http://localhost:5253/scalar/v1

Endpoints Principales
Autenticación (/auth)
POST /auth/register — Registro de usuarios (Client, Agent, Admin).

POST /auth/login — Autenticación y obtención de Bearer JWT Token.

Tickets (/tickets)
GET /tickets — Listar tickets (filtrado automático según el rol JWT).

POST /tickets — Crear un ticket de soporte.

PUT /tickets/{id}/status — Actualizar estado, prioridad o asignación.

Comentarios (/tickets/{ticketId}/comments)
GET /tickets/{id}/comments — Obtener historial de comentarios (oculta notas internas a clientes).

POST /tickets/{id}/comments — Publicar un comentario público o nota interna.

Estructura del Proyecto
Plaintext
ServiceDeskEngine/
├── docker-compose.yml
├── README.md
└── ServiceDesk.Api/
    ├── Data/          # DbContext y Migraciones EF Core
    ├── Dtos/          # Data Transfer Objects (Records)
    ├── Endpoints/     # Definición de Minimal APIs
    ├── Models/        # Entidades de Dominio
    ├── Services/      # Servicio de Tokens JWT
    ├── Validators/    # Reglas con FluentValidation
    └── Dockerfile     # Build multicapa de .NET 10
Comandos Útiles
Ver logs: docker compose logs -f servicedesk-api

Detener servicios: docker compose down

Reiniciar limpiando volumen de datos: docker compose down -v