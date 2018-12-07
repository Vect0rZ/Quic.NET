<h1 align="center"> QuicNet</h1>
<p align="center">
    <a href="https://travis-ci.org/Vect0rZ/Quic.NET">
        <img src="https://travis-ci.org/Vect0rZ/Quic.NET.svg?branch=master" alt="Build Status">
    </a>
    <a href="https://semver.org/">
        <img src="https://img.shields.io/badge/semver-2.0.0-blue.svg">
    </a>
    <img src="https://img.shields.io/badge/version-0.0.0-red.svg">
</p>

# What is QuicNet?

QuicNet is a .NET implementation of the QUIC protocol mentioned below.
The implementation stays in line with the 16th version of the [quic-transport](https://datatracker.ietf.org/doc/draft-ietf-quic-transport/?include_text=1) draft,
and does NOT YET offer implementation of the following related drafts:

* [quic-tls](https://datatracker.ietf.org/doc/draft-ietf-quic-tls/?include_text=1)
* [quic-recovery](https://datatracker.ietf.org/doc/draft-ietf-quic-recovery/?include_text=1)

# What is QUIC?

QUIC is an experimental transport layer protocol designed by Google, aiming to speed up the data transfer of connection-oriented web applications.
This application-level protocol aims to switch from TCP to UDP by using several techniques to resemble the TCP transfer while reducing the connection handshakes,
as well as to provide sensible multiplexing techniques in a way that different data entities can be interleaved during transfer.

## Connections
Connections are the first tier logical channels representing a communication between two endpoints. When a connection is established, a ConnectionId is negotiated between the two endpoints. The ConnectionId is used for identifying connection even if changes on the lower protocol layers, such as a Phone changing Wi-Fi or switching from Wi-Fi to Mobile data. This mechanism prevents restarting the negotiation flow and resending data.

## Streams
Streams are second tier logical channels representing streams of data. A single connection can have a negotiated number of streams (8 maximum for example) which serve as multiplexing entities. Every stream has it's own, generated StreamId, used for identifiying the different data objects being transferred. Streams are closed when all of the data is read, or the negotiated maximum data transfer is reached.

## Packet
Packets are the data transfer units. The packet header contains information about the connection that this packet is being sent to, and cryptographic information. After stipping off the additional transfer information, what is left are the Frames of data (A packet can have multiple frames).

## Frame
Frames are the smallest unit that contain the data that needs to be trasferred to the Endpoint.

# Contributing

Contributions are not yet open until the initial implementation of the quic-transport layer is finished at a working state.
Meanwhile feel free to fork, suggest, comment and overall be proactive.

# More

The quic-transport draft can be found, as previously mentioned at [quic-transport](https://datatracker.ietf.org/doc/draft-ietf-quic-transport/?include_text=1).

To test QUIC and find additional information, you can visit [Playing with QUIC](https://www.chromium.org/quic/playing-with-quic).

The official C++ source code can be found at [proto-quic](https://github.com/google/proto-quic).
