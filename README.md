# InventoryShop

A backend simulation of a game item marketplace, built as a hands-on deep dive into production-grade ASP.NET Core development — Domain-Driven Design, authentication/authorization, caching, and testing.

https://github.com/user-attachments/assets/5803374c-4b10-43e7-ac3b-94a92e70c568

## Overview

InventoryShop models a marketplace where players can manage and trade in-game items. The project intentionally goes beyond a CRUD demo to cover patterns and concerns you'd expect in a real production backend: domain modeling with aggregates and value objects, a clear error-handling strategy, secure authentication, and a tested, cache-aware API.

## Tech Stack

| Area | Technology |
|---|---|
| Runtime | .NET 10 |
| API | ASP.NET Core (Controllers) |
| Architecture | Clean Architecture, Domain-Driven Design (DDD) |
| Database | PostgreSQL 16, EF Core |
| Caching | Hybrid Cache (Redis-backed) |
| Auth | JWT authentication, policy- and role-based authorization |
| Validation | FluentValidation |
| Mapping | AutoMapper |
| Testing | xUnit, Moq |
| API Docs | Scalar UI |
| Tooling | DBeaver (schema inspection / manual SQL), Docker Compose |
| Functional helpers | CSharpFunctionalExtensions (`Result<T, Error>`) |

## Architecture & Patterns

- **Clean Architecture** with strict boundaries between Domain, Application, Infrastructure, and API layers
- **DDD building blocks** — aggregates and value objects used to encapsulate domain invariants
- **Result pattern** for expected business failures, with **thrown exceptions** reserved for invariant violations, handled centrally via a **GlobalExceptionHandler**
- **Specification pattern** for composable, reusable query logic
- **Hybrid Cache** to reduce database load on hot read paths, backed by Redis

## Authentication & Security

- JWT-based player authentication with password hashing and a symmetric signing key
- Endpoint access controlled via `[AllowAnonymous]` opt-outs against a global fallback authorization policy
- Fine-grained authorization via policies and roles
- Unit tests cover domain logic and application-layer behavior using xUnit and Moq.
  
## Project Structure

```
InventoryShop.Domain          # Entities, specifications, domain errors
InventoryShop.Application     # Use cases / application logic
InventoryShop.Infrastructure  # EF Core DbContext, repositories, transaction manager
InventoryShop.Web             # ASP.NET Controllers API, entry point
InventoryShop.xxx.Tests       # Tests of specific project
InventoryShop.Tests.Common    # Common tests code pieces
```
