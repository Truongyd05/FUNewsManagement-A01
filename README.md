# FUNewsManagement — Assignment 01 (ASP.NET Core MVC)

A university news management system built with **ASP.NET Core 8 MVC**, **Entity Framework Core**, and a **3-layer architecture** using the **Repository** and **Singleton** patterns.

> PRN222 — Assignment 01 · ASP.NET Core Web App (Model-View-Controller)

## Overview

FUNewsManagementSystem lets an institution publish news to the public while giving staff and admins the tools to manage content, categories, accounts and reports.

- Visitors and lecturers can browse **active** news without signing in.
- Staff manage categories, news articles (with tags), their own profile and the news they created.
- Admin manages accounts and produces statistics for a date range.

## Tech stack

| Concern | Choice |
|---|---|
| Framework | ASP.NET Core 8 MVC (`net8.0`) |
| ORM | Entity Framework Core 8 (SQL Server provider) |
| Database | Microsoft SQL Server 2012+ |
| Authentication | Cookie authentication + role-based authorization |
| UI | Razor views, Bootstrap 5, custom `site.css` |

## Solution structure

```
PhungDangTruong_A01/
├─ BusinessObjects/       Entity classes + EF Core DbContext
├─ DataAccessObjects/     DAOs (Singleton) that query the DbContext
├─ Repositories/          Repository interfaces + implementations
├─ Services/              Business logic + service interfaces
├─ PhungDangTruongMVC/    ASP.NET Core MVC web app
├─ FUNewsManagement.sql   Database creation script
└─ PhungDangTruong_ClassCode_A01.sln
```

Dependencies flow one way:

```
Controller → Service → Repository → DAO → DbContext
```

Controllers never talk to EF Core directly; every database operation goes through the service/repository/DAO layers.

## Features

### Public / Lecturer
- Browse active news, filter by category and search by keyword or tag.
- Read the full article on its details page.

### Staff — `AccountRole = 1`
- **Categories:** list, search, create, edit, delete. A category can only be deleted when no news article references it.
- **News articles:** list, search, create, edit, delete, including tag assignment.
- **History:** view the news articles created by the signed-in staff member.
- **Profile:** view and update their own account information.

### Admin — configured in `appsettings.json`
- **Accounts:** list, search, create, edit, delete system accounts.
- **Reports:** statistics for the period from `StartDate` to `EndDate` (based on the article's created date), sorted in descending order.

Create and update actions are performed in **popup dialogs**, and every delete action asks for **confirmation**.

## Getting started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- SQL Server 2012 or later (Express / Developer / LocalDB)

### 1. Create the database
Run `FUNewsManagement.sql` in SQL Server Management Studio (or with `sqlcmd`). The script creates the `FUNewsManagement` database and the `Category`, `NewsArticle`, `NewsTag`, `SystemAccount` and `Tag` tables.

### 2. Configure the connection string
Edit `PhungDangTruongMVC/appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=.\\TRUONG;Database=FUNewsManagement;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

Change `Server=.\\TRUONG` to match your SQL Server instance.

### 3. Run

```bash
dotnet restore
dotnet run --project PhungDangTruongMVC
```

Or open `PhungDangTruong_ClassCode_A01.sln` in Visual Studio and press **F5**.

The **Login page is the default route**. Default URLs: `http://localhost:5112` (HTTP) and `https://localhost:7144` (HTTPS).

## Default admin account

| Email | Password |
|---|---|
| `admin@FUNewsManagementSystem.org` | `@@abc123@@` |

These values live in the `AdminAccount` section of `appsettings.json` and are not stored in the database.

## Roles

| Role | Value | Source |
|---|---|---|
| Admin | `0` | `appsettings.json` (`AdminAccount`) |
| Staff | `1` | `SystemAccount.AccountRole` |
| Lecturer | `2` | `SystemAccount.AccountRole` |

## Notes

- Only news with `NewsStatus = 1` (active) is visible to the public.
- `Category` and `NewsArticle` both use an active/inactive status flag.
- The layout, popup dialogs and sidebar partials live under `PhungDangTruongMVC/Views/`.
