## Reservation System
Reservation System is a backend-only built as part of a self backend learning.

## Overview
This project implements a fixed-capacity event reservation system designed to prevent overselling under concurrent requests.

The system focuses on:
- Concurrency-safe seat allocation
- Reservation lifecycle management
- Expiration handling

## Core Concepts
Reservation Lifecycle :
  - Active > Confirmed
  - Active > Expired
  - Active > Cancelled

Oversell Prevention :
Overselling problem are prevented using atomical conditional update:
  Query (Theoritically) ->
    UPDATE SeatCategory
    SET RemainingSeats = RemainingSeats - :quantity
    WHERE Id = :categoryId
    AND RemainingSeats >= :quantity;

Concurrency Boundary :
The SeatCategory row acts as shared resource. All seat allocation contention are happening in this row  (hot row contention)

Expiration Worker :
Reservations expire asynchronously using a background worker.

Derived State:
RemainingSeats = cached derived value. Reservation rows = source of truth

## Architecture
This system will follow this architecture (Onion):
  1. Api -> HTTP endpoints
  2. Application -> Use cases & logic
  3. Domain -> Use cases and orchestration
  4. Infrastructure -> Database, persistence and implementation

## Tech Stack
.NET
EF Core
PostgreSQL

## Learning goals
This project explores:
  - Database concurrency control
  - Worker patterns
  - Transactional boundaries

## Note
This is a learning project, so please expect some bug, minor changes, and inconsistent.
