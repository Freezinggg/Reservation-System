# Reservation System

Backend-only reservation system built as part of a self backend engineering learning journey.

## Overview

This project implements a fixed-capacity event reservation/ticketing system designed to prevent overselling under concurrent traffic.

The system focuses on:

* Concurrency-safe seat allocation
* Reservation lifecycle management
* Background expiration handling
* Admission control and traffic shaping
* Observability under load and contention

## Core Concepts

### Reservation Lifecycle

```text
Active -> Confirmed
Active -> Expired
Active -> Cancelled
```

### Oversell Prevention

Overselling is prevented using atomic conditional updates at the database layer.

Conceptually:

```sql
UPDATE SeatCategory
SET RemainingSeats = RemainingSeats - :quantity
WHERE Id = :categoryId
AND RemainingSeats >= :quantity;
```

### Concurrency Boundary

The `SeatCategory` row acts as a shared contention resource (hot row contention).

Concurrent reservation requests compete on this boundary during high traffic scenarios.

### Admission Control

The system implements layered admission control to reduce unnecessary database pressure:

```text
Client
  ↓
Edge Rate Limiter
  ↓
Probabilistic Admission Gate
  ↓
Database
```

This helps:

* Reduce sudden burst traffic
* Reduce wasteful DB attempts
* Preserve stable success rate near capacity

### Expiration Worker & Snapshot Worker

Reservations expire asynchronously using a background worker.
Expired reservations restore reserved seat back into the system.

Snapshot used to monitor how many attempts, DB attempts, cache reject, etc to monitor traffic.

## Architecture

This project follows Onion/Clean Architecture principles:

```text
API
  → HTTP endpoints

Application
  → Use cases and logic

Domain
  → Business rules and invariants

Infrastructure
  → Database and implementation details (interface's implementation)
```

## Tech Stack

* .NET
* ASP.NET Core
* EF Core
* PostgreSQL
* Docker (for Redis)

## Learning Goals

This project explores:

* Database concurrency control
* Transaction boundaries
* Hot-row contention
* Background worker patterns
* Admission control
* Rate limiting
* Probabilistic traffic shaping
* Operational observability under load

## Notes

This is a learning-focused project intended to explore real-world backend systems behavior under concurrency and pressure scenarios, expect some bugs and inconsistent updates.

Some parts of the system may still change as the project evolves.
