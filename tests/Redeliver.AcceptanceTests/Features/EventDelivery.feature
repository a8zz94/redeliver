Feature: Event delivery
  So that a customer's systems can react to what happens in my application
  As an application sending events
  I want each accepted event delivered to the customer's receiver

  Scenario: An accepted event is delivered to its receiver
    Given a receiver is configured
    When an "invoice.paid" event is submitted
    Then the event is accepted
    And the receiver eventually gets the event
