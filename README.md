# GrailJobApi

GrailJobApi is the backend API for GrailJob, a private job-search assistant designed to help a candidate structure their search, analyze their CV, define search criteria, generate company opportunities, and manage a curated company workspace.

The API is built with ASP.NET Core and follows a modular architecture organized around business domains such as candidate profile analysis, job search, company workspace management, and user access.

## Overview

GrailJobApi provides:

- User authentication
- Private access request workflow
- Admin user management
- PDF CV upload and analysis
- Search criteria management
- AI-assisted job/company search workflow
- Company workspace with saved and excluded companies
- Comments and status management on companies
- PostgreSQL persistence
- Swagger/OpenAPI documentation in development

## Main modules

The project is organized into domain-oriented modules:

```txt
GrailJobApi/
├── Modules/
│   ├── CandidateProfile/
│   │   ├── Application/
│   │   ├── Domain/
│   │   ├── Infrastructure/
│   │   └── Presentation/
│   │
│   ├── CompanyWorkspace/
│   │   ├── Application/
│   │   ├── Domain/
│   │   ├── Infrastructure/
│   │   └── Presentation/
│   │
│   ├── JobSearch/
│   │   ├── Application/
│   │   ├── Domain/
│   │   ├── Infrastructure/
│   │   └── Presentation/
│   │
│   └── UserAccess/
│       ├── Application/
│       ├── Domain/
│       ├── Infrastructure/
│       └── Presentation/
│
├── Shared/
├── wwwroot/
├── Program.cs
├── GrailJobApi.csproj
├── appsettings.json
├── appsettings.Development.json
└── Dockerfile
