# Car Auction Management System

## Introduction
The Car Auction Management System facilitates the management of vehicle auctions. It provides a comprehensive platform for listing vehicles, managing auctions, and handling bidding processes. The system supports various vehicle types including Sedans, SUVs, Trucks, and Hatchbacks, making it versatile for different automotive auction needs.

## Running the Application
The application can be easily run using the provided Makefile:

There are two versions of the application: Console and WebApi versions

```bash
# Run the Console
make  run-console

# Run the web api
make run-api
http://localhost:5022/swagger

# Run tests
make test
```

## Design Decisions

### Domain-Driven Design (DDD)
The project follows DDD principles with a clear separation of concerns:
- **Domain Layer**: Contains core business logic and entities (Vehicle, Auction, Bid)
- **Application Layer**: Handles use cases and orchestration
- **Infrastructure Layer**: Manages technical concerns (persistence, external services)
- **API Layer**: Exposes REST endpoints for client interaction

### DTOs and Data Transfer
- DTOs are used to decouple the domain model from client-facing contracts
- Mapping between domain entities and DTOs is handled by dedicated factories
- This approach ensures domain model encapsulation and API stability

### Concurrency Patterns
- Optimistic concurrency control for auction operations
- Thread-safe bid processing using async/await patterns
- In-memory repository implementation with thread-safe collections

### Clean Architecture
- Dependencies flow inwards, with the domain layer having no external dependencies
- Clear boundaries between layers enforced through interfaces
- Dependency injection used throughout for loose coupling

## TODOs
1. Enums for Car Types to provide extensibility
2. Graceful Shutdowns
3. Caching & High DB Writes
4. Probes

