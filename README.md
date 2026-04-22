# Bluewave - Inventory & Sales Microservices

Welcome to the **Bluewave** repository! This is a backend system designed to manage products, warehouses, and sales orders. 

The project is built using a modern **Microservices Architecture** to ensure it is scalable, easy to maintain, and resilient. It uses message brokers to keep different parts of the system communicating asynchronously.

## ✨ Key Features

* **Inventory Management:** Create and manage measurement units, product categories, suppliers, warehouses, and rich product profiles.
* **Sales Management:** Create sales orders with multiple items. 
* **Automated Stock Updates:** When a new order is created in the Sales API, the Inventory API automatically deducts the products from the warehouse using RabbitMQ events.
* **Ledger System:** Inventory transactions cannot be deleted or updated, acting as a secure and reliable ledger (similar to real-world accounting).

## 🚀 Technologies Used

This project was built with the latest tools in the .NET ecosystem:

* **.NET 9 & C#**
* **Entity Framework Core** (Code-first approach)
* **MediatR** (For the CQRS pattern)
* **MassTransit & RabbitMQ** (For message brokering and event-driven communication)
* **SQL Server** (Relational Database)
* **Docker** (For containerization of services like RabbitMQ)

## 🏗️ Architecture & Design Patterns

We focused heavily on writing clean, enterprise-level code. The main patterns used are:

* **Microservices Architecture:** Divided into `Inventory API` and `Sales API`.
* **Domain-Driven Design (DDD):** Rich domain models. Entities protect their own data and business rules (e.g., no negative quantities allowed).
* **CQRS (Command Query Responsibility Segregation):** Separating read operations (Queries) from write operations (Commands) for better performance and organization.
* **Vertical Slicing:** Features are grouped by use cases (e.g., `CreateOrder`, `GetProductById`) rather than technical layers, making the code much easier to navigate.

## 📁 Project Structure

```text
src/
 ├── Bluewave.Core/                # Shared logic, Base Entities, and Integration Events
 ├── Services/
 │    ├── Inventory/               # Inventory Microservice
 │    │    ├── Api/                # Controllers and API config
 │    │    ├── Application/        # CQRS Handlers and Interfaces
 │    │    ├── Domain/             # Rich Entities and Enums
 │    │    └── Infrastructure/     # EF Core DbContext and Migrations
 │    │
 │    └── Sales/                   # Sales Microservice
 │         ├── Api/
 │         ├── Application/
 │         ├── Domain/
 │         └── Infrastructure/
