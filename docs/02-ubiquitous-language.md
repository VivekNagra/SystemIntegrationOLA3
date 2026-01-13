# 2. Ubiquitous Language

This document defines the **Ubiquitous Language** for the MyTrailer short-term rental solution. The goal is to ensure that the same terms and meanings are used consistently across Event Storming, strategic DDD, tactical DDD, and the architecture/message flow documentation.

The definitions below are intentionally written in business language first, with light technical clarifications where needed.

## 2.1 Core Concepts

### MyTrailer
The product/service that enables customers to book and rent trailers from partner locations using a mobile application.

### Customer
A person using the MyTrailer mobile app to book and rent trailers. In the model, a customer is represented by a `customerId`.

### Partner
A company that collaborates with MyTrailer by hosting trailers at one or more locations (e.g., stores or parking areas). Partners pay MyTrailer for providing the service and enabling rentals.

### Location
A physical partner site where trailers are available. Each location is identified by a unique `locationId`.

### Trailer
A physical trailer placed at a partner location and made available through the MyTrailer app.

### TrailerId
A value object that uniquely identifies a trailer as:

- `TrailerId = (locationId, trailerNumber)`

This reflects the case description that trailers are referenced by both the partner location and the trailer number.

### Booking
A reservation made by a customer for a specific trailer and an intended rental period. A booking can be accepted or rejected based on business rules (e.g., availability, time constraints).

### Rental
The active period where the customer is in possession of the trailer. A rental is the lifecycle that begins when the trailer is picked up/started and ends when the trailer is returned and the rental is closed.

## 2.2 Time and Pricing Terms

### Short-term rental
A rental performed via the mobile app with these constraints:
- Maximum duration: **24 hours**
- Must end by **midnight at the latest**

This is a core business rule and shapes the domain model and message flows.

### StartTime
The time the rental starts (or is considered started). In the model, the start time is used to compute the allowed end time.

### AllowedEndTime
The latest time the trailer may be returned without being considered late. This time is calculated using the rule:

`AllowedEndTime = min(StartTime + 24 hours, midnight cut-off)`

The “midnight cut-off” is justified directly from the case text stating that the rental must end by midnight at the latest.

### ReturnTime
The time the trailer is actually returned.

### Late return
A return where `ReturnTime > AllowedEndTime`.

### Excess rental fee (Late fee)
A fee applied when a trailer is returned late. The case describes that late return results in an excess rental fee; the exact calculation method is treated as a design decision (documented later).

### Insurance
An optional add-on purchased by the customer during the rental flow. In the case, the insurance is described as commonly purchased and has a fixed price.

### Insurance fee
A fixed price of **50 DKK** when insurance is selected.

### Base rental price
The trailer rental price is **0 DKK**. The business earns revenue through partner collaboration rather than direct rental cost.

## 2.3 Billing and Payment Terms

### Charge
A billable financial representation for a rental. A charge can contain line items (e.g., insurance fee, late fee) and is used to initiate payment processing.

### Line item
A priced component of a charge, such as:
- Insurance fee (50 DKK)
- Late fee (excess rental fee)

### Payment authorization
An optional step where a payment method is validated or reserved. The model may include authorization depending on the chosen architecture.

### Payment capture
The step where money is actually collected from the customer (e.g., to pay insurance or late fees).

### Receipt
A confirmation produced after payment capture.

## 2.4 Operational Terms

### Availability
A trailer is available if it is not already booked/rented for an overlapping period. The system enforces availability when accepting bookings.

### Booking conflict
A situation where a booking request cannot be accepted because the trailer is not available for the requested period.

### Rental closure
The final state transition where the rental is considered complete. Typically requires that:
- the trailer is returned, and
- any required fees are added, and
- payment has been captured (if there is a non-zero amount)

## 2.5 External Systems

### Payment provider
An external system used for payment authorization and capture. The design models this as an integration boundary rather than implementing a specific provider.

### Notification service (optional)
An external service used for sending confirmations, receipts, or reminders. Included as an optional integration point if needed for message flow clarity.

## 2.6 Events and Commands (Naming Convention)

To keep documentation consistent:

- **Commands** are written as imperative verbs (e.g., `RequestBooking`, `StartRental`, `ReturnTrailer`).
- **Domain Events** are written in past tense (e.g., `BookingAccepted`, `RentalStarted`, `TrailerReturned`, `PaymentCaptured`).

This convention is used throughout the Event Storming and later tactical DDD sections.
