# PRN232_Assignment_01

# FUNewsManagementSystem

## Overview
**FUNewsManagementSystem** is a news management system designed for universities or educational institutions to manage and publish news content. The project consists of two main components:
- **Backend**: An ASP.NET Core Web API with OData support, using Entity Framework Core to interact with a SQL Server database.
- **Frontend**: An ASP.NET Core MVC web application providing a user interface for news and account management.

The system supports CRUD (Create, Read, Update, Delete) operations and search functionality for accounts, news categories, articles, and tags.

## Key Features
- **Account Management (Admin)**:
  - Create, view, update, and delete accounts.
  - Generate statistical reports for news articles within a date range (StartDate to EndDate), sorted by creation date in descending order.
  - **Note**: Cannot delete accounts associated with news articles.
- **Category Management (Staff)**:
  - Create, view, update, and delete news categories.
  - **Note**: Cannot delete categories used by news articles.
- **News Article Management (Staff)**:
  - Create, view, update, and delete articles, including tag assignment.
  - View history of articles created by the staff.
- **Profile Management (Staff)**: Edit personal information (name, email, password).
- **News Viewing**: Public access to view active news articles without authentication.
- **Authentication**: Login using email and password, with a default admin account:
  - Email: `admin@FUNewsManagementSystem.org`
  - Password: `@@abc123@@`

## Technologies Used
- **Backend**: ASP.NET Core 8.0, Entity Framework Core, OData, SQL Server.
- **Frontend**: ASP.NET Core MVC, Bootstrap (for UI and popup dialogs).
- **Architecture**: 3-layer architecture (Presentation, Business Logic, Data Access).
- **Design Patterns**: Repository Pattern, Singleton Pattern.
- **Database**: SQL Server 2012 or higher.

## Project Structure
The project is divided into two solutions:
1. **StudentName_ClassCode_A01_BE.sln**: Backend (Web API).
2. **StudentName_ClassCode_A01_FE.sln**: Frontend (Web Application).

### Backend Directory
FUNewsManagementSystem.WebAPI
├── Controllers/              # API controllers handling HTTP requests

├── Data/                     # DbContext for Entity Framework Core

├── Models/                   # Data models (Category, NewsArticle, SystemAccount, Tag, NewsTag)

├── Repositories/             # Repository Pattern for business logic

├── appsettings.json          # Database connection and admin account configuration

└── Program.cs                # Service and OData configuration

### Frontend Directory
FUNewsManagementSystem.WebApp
├── Controllers/              # MVC controllers for UI logic

├── Models/                   # ViewModels and API call models

├── Views/                    # Razor Views for the UI

├── wwwroot/                  # Static resources (CSS, JS, Bootstrap)

└── Program.cs                # Service configuration

## System Requirements
- Visual Studio 2019 or later (recommended: Visual Studio 2022 with .NET 8).
- SQL Server 2012 or later (or SQL Server Express).
- SQL Server Management Studio (SSMS) for running the database script.
- .NET 8.0 SDK.

## Setup Instructions

### 1. Set Up the Database
- Open SQL Server Management Studio (SSMS).
- Run the provided `FUNewsManagement.sql` script to create the `FUNewsManagement` database and sample data.

### 2. Configure the Backend
- Open the `StudentName_ClassCode_A01_BE.sln` solution in Visual Studio.
- Update the connection string in `appsettings.json`:
  ```json
  {
    "ConnectionStrings": {
      "DefaultConnection": "Server=your_server_name;Database=FUNewsManagement;Trusted_Connection=True;"
    },
    "AdminAccount": {
      "Email": "admin@FUNewsManagementSystem.org",
      "Password": "@@abc123@@",
      "Role": "Admin"
    }
  }


## Contact
For questions, feedback, or support:
- **Email**: [trandinhhung717@gmail.com]
- **GitHub Issues**: Open an issue at [github.com/hungtabe/PRN232_Assignment_01-/issues]


Thank you for using the PRN232_Assignment_01!







