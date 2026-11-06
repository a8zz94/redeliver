 # 1. Use RabbitMQ for durable delivery

Status: Accepted

Date: 2026-11-06

## Context

Events are currently held in an in-memory queue. A crash or restart loses them, even after the sender has received `202 Accepted`.

Once the service accepts an event, the event must survive a crash and still be delivered.

This project also exists to build hands-on experience with event-driven systems, so that is part of the decision.

## Decision

Use RabbitMQ to hold events until they have been delivered.

- Use durable queues and persistent messages.
- Return `202 Accepted` only after RabbitMQ confirms it has stored the event (publisher confirms).
- Acknowledge a message only after the receiver has responded successfully.

## Options considered

| Option | Why not chosen |
| --- | --- |
| Postgres table, polled | Simpler to run on one server and gives easy delivery history, but provides less experience with message brokers |
| Postgres with a wake-up signal | Same as above, with extra moving parts that are not needed yet |
| Kafka | Too heavy for one small server, and per-message retries are awkward |
| Redis Streams | Durability depends on persistence settings, which is a poor fit for not losing events |
| Hangfire or Wolverine | Would hide the delivery problems this project sets out to explore |

## Consequences

- Delivery is at-least-once. A crash after sending but before acknowledging causes a repeat, so receivers need to use the event ID to detect duplicates.
- There is one more system to run locally, in tests and in deployment.
- RabbitMQ has no simple built-in backoff, so delayed retries need extra work.
- A broker is a poor place to query delivery history, so a database will probably be needed later. Writing to both safely will need its own decision.
- Failure behaviour is not yet proven. It needs to be tested by breaking the system, including killing the service and the broker mid-delivery.
