using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager {

	private IChannel<Datagram> channel;

	public NetworkManager (int port) {
		channel = new UdpChannel ();
		channel.Open (9090);
	}


}
