# Contact-page Application

This repository contains a complete full-stack application consisting of:

- **Backend**: ASP.NET Core Web API
- **Frontend**: React.js SPA
- **Database**: SQL Server (running inside a Docker container)

The project demonstrates authentication using JWT tokens, CRUD operations on contacts, and categorization of contacts with categories and subcategories.


---

## Table of Contents

- [Application Features](#application-features)
- [Backend Overview](#backend-overview)
- [Frontend Overview](#frontend-overview)
- [Docker Setup](#docker-setup)
- [How to Run the Application](#how-to-run-the-application)
- [Project Structure](#project-structure)
- [API Endpoints](#api-endpoints)
- [Future Improvements](#future-improvements)

---

## Application Features

![image](https://github.com/user-attachments/assets/c0940ab2-2052-4e2d-866f-84b623a1535b)


- Displays a list of contacts fetched from the database.
- Allows users to click on a contact to view its detailed information.
- Includes a login panel on the main page.
- Only after a successful login, users can:
  - Edit an existing contact's information.
  - Edit the contact's category or assign a new one (including creating new subcategories if needed).
  - Create new contacts.
  - Delete existing contacts.

---

## Backend Overview

### Technologies Used

- ASP.NET Core (.NET 8 or .NET 7)
- Entity Framework Core
- SQL Server
- JWT (JSON Web Tokens) for authentication
- REST API
- CORS for frontend connection

### Main Components

- **Controllers**:
  - `ContactsController`: Manages CRUD operations for contacts.
  - `CategoryController`: Handles categories and subcategories.
  - `AuthController`: Handles user login and JWT generation.
- **Data**:
  - `AppDbContext`: Defines database context.
- **Models**:
  - `User`, `Contact`, `Category`, `Subcategory`
- **DTOs**:
  - `ContactDto`, `UpdateContactDto`, `SubcategoryDto`, `LoginDto`

### Features

- JWT-based Authentication (`/api/auth/login`)
- Full CRUD for Contacts (`/api/contacts`)
- Fetching Categories and Subcategories (`/api/categories`, `/api/categories/{id}/subcategories`)
- Seeding:
  - Default user: `testuser / testpass`
  - Default categories and subcategories
  - Sample contacts
- CORS enabled for `http://localhost:3000`
- Basic validation for categories and subcategories
- DTO usage for safer API exposure

---

## Frontend Overview

### Technologies Used

- React.js
- Fetch API for HTTP requests
- LocalStorage for token storage
- (Optional) Basic CSS inline styling

### Key Components

- **App**:
  - Manages login state and routing.
- **Login**:
  - Handles user login and token management.
- **Contacts**:
  - Displays list of contacts
  - Add, edit, delete contacts
  - Handles categories and subcategories
- **Methods**:
  - `fetchContacts`, `fetchCategories`, `handleEditClick`, `handleSave`, `handleDelete`, `handleAddNew`, `handleCreate`, `fetchSubcategories`

### Features

- JWT Authentication with backend
- CRUD operations on contacts
- Selecting categories and subcategories
- Simple and clean UI without external libraries like Material-UI or Bootstrap

---

## Docker Setup

The SQL Server database runs in a Docker container defined by the `docker-compose.yml` file. Default SQL Server credentials:

- User: sa
- Password: Your_password123

## How to Run the Application

### Prerequisites

* Docker
* .NET SDK (8 or 7)
* Node.js (recommended version 18+)

### Steps

1. **Clone the Repository**

   ```bash
   git clone https://github.com/your-username/your-repo-name.git
   cd your-repo-name
   ```

2. **Start SQL Server using Docker Compose**

   ```bash
   docker-compose up -d
   ```

3. **Run the Backend**

   Navigate to the backend directory and run:

   ```bash
   cd Backend-core
   dotnet run
   ```

   Backend will be available at:

   * [https://localhost:7094](https://localhost:7094)

4. **Run the Frontend**

   Navigate to the frontend directory and install dependencies:

   ```bash
   cd frontend-directory-name
   npm install
   ```

   Then start the frontend:

   ```bash
   npm start
   ```

   Frontend should open at:

   * [http://localhost:3000](http://localhost:3000)

### Authentication

Log in with:

* **Username**: `testuser`
* **Password**: `testpass`

JWT Token will be stored in localStorage.

---

## Project Structure

```
/Backend-core
/docker-compose.yml
/frontend-directory-name
/.gitignore
/README.md
```

---

## API Endpoints

| Method | Endpoint                           | Description          | 
| :----: | :--------------------------------- | :------------------- | 
|  POST  | /api/auth/login                    | User login           | 
|   GET  | /api/contacts                      | List all contacts    |
|  POST  | /api/contacts                      | Create a new contact |
|   GET  | /api/contacts/{id}                 | Get contact details  |
|   PUT  | /api/contacts/{id}                 | Update a contact     |
| DELETE | /api/contacts/{id}                 | Delete a contact     |
|   GET  | /api/categories                    | List all categories  |
|   GET  | /api/categories/{id}/subcategories | List subcategories   |

---

## Future Improvements

* Add Dockerfile for backend containerization.
* Add automatic database migration and seeding during container startup.
* Improve security: password hashing, secrets management.
* Add persistent Docker volumes for database storage if needed.
* Improve frontend validation and error handling.
