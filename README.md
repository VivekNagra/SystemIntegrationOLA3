# System Integration OLA 3 – MyTrailer 

This repository contains the documentation and design artifacts for the **MyTrailer short-term trailer rental** case study.
The focus is on **requirements discovery (Event Storming)**, **Domain-Driven Design (strategic + tactical)**, and the **flow of messages between services** in a proposed architecture.

> Scope note: This work focuses on the **short-term rental via app** (max 24 hours, always ends by midnight). Long-term/overnight rental via the website is out of scope.

## How to read this repository

Start here (in order):

1. [Scope and Assumptions](docs/01-scope-and-assumptions.md)
2. [Ubiquitous Language](docs/02-ubiquitous-language.md)
3. [Event Storming](docs/03-event-storming.md)
4. [Strategic DDD](docs/04-strategic-ddd.md)
5. [Tactical DDD](docs/05-tactical-ddd.md)
6. [Message Flows](docs/06-message-flows.md)
7. [Definition of Done](docs/07-definition-of-done.md)

## Part 2 (implementation)

For the runnable services (rental + billing) and demo scripts, see `part2/`. The part2 README has run/test instructions and the automated demo script (`run-demo.ps1`).

## Diagrams

All diagrams are stored in `docs/diagrams/`.

Mermaid sources (for regeneration):
- `context-map.mmd` → `context-map.png`
- `c4-system-context.mmd` → `c4-system-context.png`
- `c4-container.mmd` → `c4-container.png`
- `message-flows.mmd` → `message-flows.png`
- `tactical-lifecycle.mmd` → `tactical-lifecycle.png`

Regenerate (requires Node/npm):
```bash
npx @mermaid-js/mermaid-cli -i docs/diagrams/context-map.mmd -o docs/diagrams/context-map.png
npx @mermaid-js/mermaid-cli -i docs/diagrams/c4-system-context.mmd -o docs/diagrams/c4-system-context.png
npx @mermaid-js/mermaid-cli -i docs/diagrams/c4-container.mmd -o docs/diagrams/c4-container.png
npx @mermaid-js/mermaid-cli -i docs/diagrams/message-flows.mmd -o docs/diagrams/message-flows.png
npx @mermaid-js/mermaid-cli -i docs/diagrams/tactical-lifecycle.mmd -o docs/diagrams/tactical-lifecycle.png
```
