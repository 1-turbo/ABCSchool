# ABCSchool (.NET 8 Core Web API)

## Overview
The Mini School System is a web-based application built using .NET 8 Core Web API and C#. This project focuses on managing users, roles, tenants, and school-related resources. It implements authentication and authorization mechanisms using JWT tokens to control access to various system functionalities.

## Key Features
### 1. User Management
- Create, update, delete, and retrieve user details.
- Assign roles and permissions to users.

### 2. Role-Based Access Control (RBAC)
- Define roles with specific permissions.
- Restrict access to API endpoints based on assigned roles.

### 3. JWT Authentication & Authorization
- Secure API endpoints with JWT authentication.
- Authenticate users and generate JWT tokens upon login.
- Validate tokens to determine user access to resources.

### 4. Tenant Management (Multi-Tenancy)
- Register and manage tenants.
- Assign users to specific tenants.
- Activate or deactivate tenants based on subscription status.

### 5. School Resource Management
- Manage school-related data (students, teachers, courses, etc.).
- Implement CRUD operations for school resources.

### 6. Permissions Management
- Assign specific permissions to users.
- Retrieve and update permissions dynamically.

### 7. API Endpoints Implementation
- RESTful API endpoints for all functionalities.
- Secure endpoints with role-based authentication.

## Technology Stack
- **Backend:** .NET 8 Core Web API, C#
- **Database:** Microsoft SQL Server
- **Authentication:** JWT Token-based authentication

