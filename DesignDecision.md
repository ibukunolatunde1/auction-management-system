# Key Design Decisions

# API Layer Overview

1. Data Seeding
   - Development-only seed data
   - Realistic vehicle and auction data
   - Automatic seeding on application startup
   - Helps with testing and development

# Domain Layer Overview

The domain layer represents the core business logic and we have the following

1. Entities
- Vehicle.cs : Base abstract class for the vehicles
    DerivedClasses: Sedan.cs, SUV.cs etc

2. Value Objects
- Defines the structure of some needed objects and their properties
    Money.cs: All operations involving currencies and their format
    Bid.cs: Bid Information
    VehicleId.cs: Since its the primary key

3. Exceptions: Custom exceptions for domain-specific errors

TODO: Add Enums to the Vehicle class which can create extensibility for other types of cars

# Application Layer Overview
It's the layer between the API and Domain Layers.

1. DTOs serve as the request/response formatting. 

# Infrastructure Layer

1. ConcurrentDictionary for Thread safety. If multiple users are adding vehicles simultaneously
2. Async for non thread blocking operations
3. Cancellation tokens for graceful shutdowns