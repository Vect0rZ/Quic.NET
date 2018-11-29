<h1> <p align="center"> QuicNet </p> </h1>
<p align="center">
    <a href="https://travis-ci.org/Vect0rZ/QUICTransportNET"><img src="https://travis-ci.org/Vect0rZ/QUICTransportNET.svg?branch=master" alt="Build Status"></a>
</p>

# What is QUIC?

QUIC is an experimental transport layer protocol designed by Google, aiming to speed up the data transfer of connection-oriented web applications.
It is implemented in the application-spaces as opposed to residing in the operating system, 
The protocol aims to switch from TCP to UDP by using several techniques to resemble the TCP transfer while reducing the connection handshakes,
as well as to provide sensible multiplexing techniques in a way that different data entities can be interleaved during transfer.

# What is QuicNet?

QuicNet is a .NET implementation of the aforementioned QUIC protocol.
The implementation stays in line with the 16th version of the [quic-transport](https://datatracker.ietf.org/doc/draft-ietf-quic-transport/?include_text=1) draft,
and does NOT YET offer implementation of the following related drafts:

* [quic-tls](https://datatracker.ietf.org/doc/draft-ietf-quic-tls/?include_text=1)
* [quic-recovery](https://datatracker.ietf.org/doc/draft-ietf-quic-recovery/?include_text=1)

# Conrtibuting

Contributions are not yet open until the initial implementation of the quic-transport layer is finished at a working state.
Meanwhile feel free to fork, suggest, comment and overall be proactive.

# More

The quic-transport draft can be found, as previously mentioned at [quic-transport](https://datatracker.ietf.org/doc/draft-ietf-quic-transport/?include_text=1).

To test QUIC and find additional information, you can visit [Playing with QUIC](https://www.chromium.org/quic/playing-with-quic).

The official C++ source code can be found at [proto-quic](https://github.com/google/proto-quic).
