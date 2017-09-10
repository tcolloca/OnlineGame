using System;
using System.Net;

public class ParsedDatagram {

	public IPEndPoint endPoint { get; private set; }
	public Message message { get; private set; }

	public ParsedDatagram (IPEndPoint endPoint, Message message) {
		this.endPoint = endPoint;
		this.message = message;
	}
}

