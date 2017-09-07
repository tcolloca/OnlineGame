using UnityEngine;
using System.Collections;
using System.Net;

public class Datagram {

	public byte[] bytes { get; private set; }
	public IPEndPoint endPoint { get; private set; }

	public Datagram (byte[] bytes, IPEndPoint endPoint) {
		this.endPoint = endPoint;
		this.bytes = bytes;
	}
}
