# 4. Strategic DDD

This section documents the **Strategic Domain-Driven Design** decisions for the MyTrailer short-term rental solution. The focus is on identifying bounded contexts, defining their responsibilities, and describing how they collaborate through a context map and a service-oriented architecture.

The goal is to propose an architecture that is consistent with:
- the short-term rental scope (max 24 hours and midnight cut-off),
- the domain rules discovered in Event Storming,
- and the need to document message flows between services.

## 4.1 Domain overview and core domain

### Problem framing
MyTrailer enables customers to rent a trailer through a mobile app from partner locations. The short-term rental flow is time-constrained and must enforce:
- a maximum rental duration of 24 hours, and
- a strict midnight cut-off.

These rules shape both the domain model and the system boundaries.

### Core domain
The **core domain** is the short-term rental lifecycle itself (booking → start → return → close) including availability and the midnight rule. This is where correctness matters most, because it drives the customer experience and downstream billing/settlement.

## 4.2 Bounded contexts

The system is divided into bounded contexts to reduce coupling, separate responsibilities, and allow each domain area to evolve independently.

### 4.2.1 Rental Management (Core Domain)
**Purpose:** Owns the booking and rental lifecycle and enforces rental rules.

**Responsibilities**
- Trailer availability for booking (no overlap)
- Booking acceptance/rejection
- Rental start and return
- Calculation/enforcement of `AllowedEndTime` (24h rule + midnight cut-off)
- Publishing lifecycle events used by other contexts (e.g., `TrailerReturned`, `RentalClosed`)

**Key outputs**
- Domain events that drive billing and partner reporting

### 4.2.2 Billing & Payments (Supporting Domain)
**Purpose:** Owns pricing elements and payment integration.

**Responsibilities**
- Creating charges for rentals
- Adding line items:
  - Insurance fee (50 DKK when selected)
  - Late fee (excess rental fee when late return occurs)
- Capturing payment through an external payment provider
- Issuing receipts (conceptually)
- Publishing billing events (`PaymentCaptured`, etc.)

**Integration boundary**
- External payment provider is treated as a separate system

### 4.2.3 Partner Management (Supporting Domain)
**Purpose:** Owns partner/location representation and usage information needed for settlement.

**Responsibilities**
- Managing partner and location information (conceptually)
- Receiving usage/rental closure events and recording usage statistics per location
- Supporting later settlement/invoicing processes (kept high-level for this assignment)

### 4.2.4 Identity / Customer (Generic Domain)
**Purpose:** Authentication and customer identity.

**Responsibilities**
- Customer accounts and authentication
- Providing `customerId` to the rest of the system

This context is treated as generic because the case does not emphasize identity as a differentiator.

## 4.3 Context map (relationships between bounded contexts)

The context map describes how contexts collaborate and what they depend on.

### Relationship summary
- **Identity → Rental Management**: Upstream generic provider of customer identity (`customerId`)
- **Rental Management → Billing & Payments**: Rental is upstream for rental lifecycle events; Billing consumes them to bill insurance/late fees
- **Rental Management → Partner Management**: Rental publishes usage-relevant events; Partner Management consumes them for usage statistics and later settlement

A key choice is to communicate primarily through **domain events** (published language) to reduce coupling and clarify ownership.

### Context Map diagram

```mermaid
flowchart LR
  %% Bounded Contexts
  ID[Identity / Customer\n(Generic Domain)]:::generic
  RENTAL[Rental Management\n(Core Domain)]:::core
  BILL[Billing & Payments\n(Supporting Domain)]:::support
  PARTNER[Partner Management\n(Supporting Domain)]:::support

  %% Relationships (Context Map)
  ID -->|Upstream identity provider\n(customerId, auth)| RENTAL
  RENTAL -->|Published Language / Events\nBookingAccepted, RentalStarted,\nTrailerReturned, RentalClosed| BILL
  RENTAL -->|Published Language / Events\nRentalClosed, LocationUsageRecorded| PARTNER

  %% Styling
  classDef core fill:#e0f2fe,stroke:#0369a1,color:#0f172a;
  classDef support fill:#ecfccb,stroke:#4d7c0f,color:#0f172a;
  classDef generic fill:#f1f5f9,stroke:#475569,color:#0f172a;
```

## 4.4 Architectural style (high-level)

A suitable architecture for this problem is a **service-oriented, event-driven architecture** with clear service boundaries aligned to bounded contexts.

### Why event-driven communication fits this case
- Billing and partner usage are naturally triggered by what happens in the rental lifecycle (e.g., return and close).
- Events reduce direct synchronous coupling between services.
- It becomes explicit which context owns a decision:
  - Rental owns time rules and lifecycle transitions
  - Billing owns monetary calculations and charging logic
  - Partner owns usage aggregation/settlement logic

This does not remove synchronous APIs (the app still needs them), but it reduces tight coupling between internal services.

## 4.5 C4 diagrams (system context and container level)

C4 diagrams are used to communicate the system design clearly.

### 4.5.1 C4 System Context
Shows the MyTrailer system as a whole and its external actors and systems.

Add your generated diagram here:

```mermaid
flowchart LR
  %% People and Systems
  customer([Customer\nMobile App User]):::person
  partner([Partner Company\n(Locations)]):::external
  payment([Payment Provider\n(External System)]):::external
  system[[MyTrailer System]]

  %% Flows
  customer <--> system
  partner <--> system
  system <--> payment

  %% Labels
  customer ---|Booking, start, return,\nreceipts| system
  partner ---|Location data,\nusage reporting| system
  system ---|Capture payment| payment

  %% Styling
  classDef person fill:#fef9c3,stroke:#eab308,color:#0f172a;
  classDef external fill:#ede9fe,stroke:#7c3aed,color:#0f172a;
```

Minimum elements to include in the System Context diagram:
- Customer (mobile app user)
- Partner company (locations hosting trailers)
- Payment provider (external)
- MyTrailer System

### 4.5.2 C4 Container diagram
Shows the internal containers/services and their interactions.

Add your generated diagram here:

```mermaid
flowchart LR
  %% External
  payment[Payment Provider\n(External API)]:::external

  %% System Boundary
  subgraph MyTrailer System
    app[Mobile App / UI\n(Frontend)]:::container
    rental[Rental Management Service]:::core
    billing[Billing & Payments Service]:::container
    partner[Partner Management Service]:::container
    bus[(Event Bus / Message Broker)]:::infra
  end

  %% Flows
  app -->|HTTP/REST\nbooking/start/return| rental
  rental -->|Publish events| bus
  billing -->|Subscribe rental events\nBilling events| bus
  partner -->|Subscribe RentalClosed\nusage events| bus
  billing -->|Payment capture API| payment

  %% Styling
  classDef container fill:#e0f2fe,stroke:#0369a1,color:#0f172a;
  classDef core fill:#bae6fd,stroke:#0369a1,color:#0f172a;
  classDef infra fill:#f8fafc,stroke:#475569,color:#0f172a;
  classDef external fill:#ede9fe,stroke:#7c3aed,color:#0f172a;
```

Minimum containers to include:
- Mobile App / UI
- Rental Management Service
- Billing & Payments Service
- Partner Management Service
- Event Bus / Message Broker
- External Payment Provider

## 4.6 Event boundaries (published language)

To support event-driven collaboration, the system uses a small shared published language for cross-context events. Examples:
- From Rental Management:
  - `BookingAccepted`
  - `RentalStarted`
  - `TrailerReturned`
  - `RentalClosed`
- From Billing & Payments:
  - `LateFeeAdded`
  - `PaymentCaptured`
  - `ReceiptIssued`

The detailed event payloads are described later in Tactical DDD and Message Flows, but the strategic decision is that:
- Rental events represent domain facts (“what happened”)
- Billing events represent financial outcomes (“what was charged/paid”)

## 4.7 Diagram generation prompts (for AI tools)

Use the prompts below to generate the diagrams referenced in this section.

### Prompt A — Context Map (`context-map.png`)
Create a DDD context map for “MyTrailer short-term trailer rental”. Include bounded contexts as boxes:
1) Rental Management (Core Domain)
2) Billing & Payments (Supporting Domain)
3) Partner Management (Supporting Domain)
4) Identity/Customer (Generic Domain)
Draw relationships:
- Identity → Rental Management (upstream identity provider)
- Rental Management → Billing & Payments (events trigger billing)
- Rental Management → Partner Management (events trigger usage recording)
Label relationship style as “Published Language / Event-driven” for the two downstream consumers. Use a clean, minimal professional style with clear arrows and short captions.

### Prompt B — C4 System Context (`c4-system-context.png`)
Create a C4 System Context diagram for “MyTrailer short-term trailer rental”. Show:
- Person: Customer using the MyTrailer mobile app
- External system: Payment Provider
- External actor: Partner company (hosts trailer locations)
- System: MyTrailer System
Arrows:
- Customer ↔ MyTrailer System (booking, start rental, return, view receipts)
- MyTrailer System ↔ Payment Provider (capture payment)
- Partner company ↔ MyTrailer System (locations/trailer inventory info and usage reporting)
Clean, readable layout, minimal colors, export as PNG.

### Prompt C — C4 Container (`c4-container.png`)
Create a C4 Container diagram for “MyTrailer”. Inside the MyTrailer system boundary include:
- Mobile App / UI
- Rental Management Service
- Billing & Payments Service
- Partner Management Service
- Event Bus / Message Broker
Outside boundary include:
- Payment Provider (external)
Relationships:
- Mobile App calls Rental Management Service (HTTP/REST)
- Rental Management publishes events to Event Bus
- Billing & Payments subscribes to Rental events; publishes billing events
- Partner Management subscribes to RentalClosed (usage)
- Billing & Payments integrates with Payment Provider for payment capture
Label arrows with protocols (HTTP for app calls, events/pub-sub for internal, API for payment provider). Clean style, readable text, export PNG.
