# Quic.NET Contributing Guide

Any contribution from a small typo, issue submission, bug fix to a feature is welcome! Read the short contributing guide below to get started.

# Requirements

- Visual Studio 2017
- Minimum .NET 4.5.2

# Setup

There are no additional dependencies, and opening the solution and rebuilding should be good to go.

# Solution structure

   - [QuicNet](#quicnet)
   - [Infrastructure](#quicnet-infrastructure)
   - [Utilities](#quicnet-utilities)
   - [Unit Tests](#quicnet-tests-unit)
   - [More](#more)
   
## QuicNet
This project contains the main API like QuicListener and QuicClient. It contains the base classes for Connections and Streams in which
resides the protocol logic.

## QuicNet Infrastructure
The infrastructure contains all the packet and frame definitions, error codes, stream and connection states.

## QuicNet Utilities
This project contains mostly helper methods and couple of protocol related transformations such as VariableInteger and StreamId.

## QuicNet Tests Unit
The unit test project.

## More
The other two projects named ServerClient and ConsoleClient and console projects containing some examples for working with the protocol.
