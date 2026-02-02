# Chili Clothes Backend 🌶️

Este repositorio contiene la API REST completa para el sistema de gestión de pedidos de **Chili Clothes**.  
El sistema está construido con **.NET 8** y arquitectura limpia, diseñado para gestionar usuarios, productos y pedidos con roles diferenciados (Admin y User).

## 🚀 Tecnologías Principales

*   **Framework**: .NET 8 (ASP.NET Core Web API)
*   **Base de Datos**: MySQL (Hospedado en Clever Cloud)
*   **ORM**: Entity Framework Core 8 (Pomelo MySQL)
*   **Autenticación**: JWT (JSON Web Tokens)
*   **Seguridad**: BCrypt para hash de contraseñas
*   **Documentación**: Swagger UI / OpenAPI

---

## 🛠️ Configuración e Instalación

### 1. Prerrequisitos
*   [.NET SDK 8.0](https://dotnet.microsoft.com/download/dotnet/8.0)
*   Git

### 2. Clonar el repositorio
```bash
git clone https://github.com/ChiliClothes/chili-clothes-backend.git
cd chili-clothes-backend
```

### 3. Instalar Dependencias
Si acabas de clonar el proyecto, ejecuta el siguiente comando para restaurar los paquetes NuGet necesarios (Entity Framework, JWT, BCrypt, etc.):

```bash
dotnet restore
```

Si necesitas agregar los paquetes manualmente en un entorno nuevo, estos son los comandos utilizados:
```bash
dotnet add package Pomelo.EntityFrameworkCore.MySql
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add package BCrypt.Net-Next
dotnet add package System.IdentityModel.Tokens.Jwt
dotnet add package Swashbuckle.AspNetCore
```

### 4. Configuración de Base de Datos
El proyecto ya está pre-configurado para conectarse a la base de datos de producción en `appsettings.json`.
No es necesario configurar nada adicional para probarlo localmente.

### 4. Ejecutar la Aplicación
```bash
dotnet restore
dotnet run
```
La aplicación se iniciará en `http://localhost:5003`.
Al abrir esta dirección en tu navegador, serás redirigido automáticamente a la documentación interactiva en **Swagger UI**.

---

## 🔑 Características y Endpoints

### 🔐 Autenticación (`Auth`)
El sistema utiliza JWT Bearer Tokens.
*   **POST** `/api/Auth/register`: Registrar nuevo usuario (Rol predeterminado: USER).
*   **POST** `/api/Auth/login`: Iniciar sesión y obtener Token (incluye `role`, `email`, `sub`).

### 📦 Productos (`Products`)
*   **GET** `/api/Products`: Listar productos activos (Público/User).
*   **GET** `/api/Products/admin/all`: Listar TODOS los productos (Solo Admin).
*   **GET** `/api/Products/{id}`: Detalle de producto.
*   **POST** `/api/Products`: Crear producto (Solo Admin).
*   **PUT** `/api/Products/{id}`: Actualizar producto (Solo Admin).
*   **DELETE** `/api/Products/{id}`: Desactivar producto (Solo Admin).

### 🛒 Pedidos (`Orders`)
Gestión completa con validación de stock y cálculo de totales en backend.
*   **POST** `/api/Orders`: Crear pedido (Solo User). _Valida stock e items._
*   **GET** `/api/Orders`:
    *   **User**: Ve historial de sus propios pedidos.
    *   **Admin**: Ve todos los pedidos del sistema.
*   **PUT** `/api/Orders/{id}/status`: Cambiar estado (Solo Admin). _Estados: PENDING, PREPARING, DELIVERED, CANCELLED_.
*   **PUT** `/api/Orders/{id}/cancel`: Cancelar pedido (Solo User, si estado es PENDING).

---

## 👤 Gestión de Usuarios y Roles

### Roles Disponibles
1.  **USER**: Clientes que pueden ver productos y realizar pedidos.
2.  **ADMIN**: Administradores con acceso total a gestión de productos y pedidos.

### Crear un Administrador
Por defecto, el registro crea usuarios `USER`. Para promover un usuario a `ADMIN`:
1.  Regístrate desde `/api/Auth/register` o Swagger.
2.  Accede a la base de datos y ejecuta:
    ```sql
    UPDATE Users SET Role = 'ADMIN' WHERE Email = 'tu_email@ejemplo.com';
    ```

---

## 🧪 Pruebas con Swagger
1.  Ve a `http://localhost:5003/swagger`
2.  Usa `/api/Auth/login` para obtener un token.
3.  Copia el token (sin comillas).
4.  Haz clic en el botón verde **Authorize** arriba a la derecha.
5.  Escribe `Bearer TU_TOKEN_AQUI` y confirma.
6.  ¡Ahora puedes probar los endpoints protegidos!

---

## 📂 Estructura del Proyecto

```
/ChiliClothes
├── Controllers/      # Controladores de la API
├── Data/            # Contexto de BD y configuración EF Core
├── Models/          # Entidades de Dominio (User, Product, Order)
├── DTOs/            # Data Transfer Objects (Request/Response models)
├── Migrations/      # Migraciones de Base de Datos
├── appsettings.json # Configuración y Strings de Conexión
└── Program.cs       # Punto de entrada y configuración de servicios
```

---

Hecho por Chili Clothes.
