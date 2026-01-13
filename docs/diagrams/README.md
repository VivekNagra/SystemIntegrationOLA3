# Diagrams

This folder contains images referenced by the documentation.

Current diagrams (PNG, with Mermaid sources where applicable):
- `context-map.png` ← `context-map.mmd`
- `c4-system-context.png` ← `c4-system-context.mmd`
- `c4-container.png` ← `c4-container.mmd`
- `message-flows.png` ← `message-flows.mmd`
- `tactical-lifecycle.png` ← `tactical-lifecycle.mmd`
- `event-storming-cleaned.png` (static)
- `late-return-focus.png` (static)

Regenerate Mermaid-based PNGs with:
- `npx @mermaid-js/mermaid-cli -i context-map.mmd -o context-map.png`
- `npx @mermaid-js/mermaid-cli -i c4-system-context.mmd -o c4-system-context.png`
- `npx @mermaid-js/mermaid-cli -i c4-container.mmd -o c4-container.png`
- `npx @mermaid-js/mermaid-cli -i message-flows.mmd -o message-flows.png`
- `npx @mermaid-js/mermaid-cli -i tactical-lifecycle.mmd -o tactical-lifecycle.png`
