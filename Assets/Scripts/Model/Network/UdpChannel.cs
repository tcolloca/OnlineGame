using UnityEngine;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System;
using System.Threading;
using System.Collections.Generic;

public class UdpChannel : IChannel<Datagram> {

	private UdpClient client;
	private Thread receiveThread;
	private bool keepAlive;

	private Queue<Datagram> datagramQueue = new Queue<Datagram> ();
	private System.Object datagramQueueLock = new System.Object ();

	public void Open (int port) {
		SetUpClient (port);
		SetUpReceiveThread ();
	}
		
	public void Close () {
		keepAlive = false;
		client.Close ();
	}

	public void Send (Datagram datagram) {
		Debug.Log ("Sending from: " + client.Client.LocalEndPoint.ToString ());
		client.Send (datagram.bytes, datagram.bytes.Length, datagram.endPoint);
	}

	public Datagram Receive () {
		if (datagramQueue.Count == 0) {
			return null;
		}
		lock (datagramQueueLock) {
			if (datagramQueue.Count == 0) {
				return null;
			}
			return datagramQueue.Dequeue ();
		}
	}

	private void BlockAndReceive () {
		while (keepAlive) {
			try {
				IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
				Debug.Log ("Blocking");
				byte[] data = client.Receive (ref remoteEndPoint);
				Debug.Log ("Unblocked!: " + remoteEndPoint.Port.ToString ());
				Datagram datagram = new Datagram (data, remoteEndPoint);
				lock (datagramQueueLock) {
					datagramQueue.Enqueue (datagram);
				}
			} catch (Exception err) {
				Debug.Log (err.ToString());
			}
		}
	}

	private void SetUpClient (int port) {
		client = new UdpClient ();

		uint IOC_IN = 0x80000000;
		uint IOC_VENDOR = 0x18000000;
		uint SIO_UDP_CONNRESET = IOC_IN | IOC_VENDOR | 12;
		client.Client.IOControl ((int)SIO_UDP_CONNRESET, new byte[] { Convert.ToByte (false) }, null);
		Debug.Log (port);
		IPEndPoint listenEndpoint = new IPEndPoint (IPAddress.Parse("192.168.0.14"), port);
		client.ExclusiveAddressUse = false;
		client.Client.SetSocketOption (SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
		client.Client.Bind (listenEndpoint);
	}

	private void SetUpReceiveThread() {
		keepAlive = true;
		receiveThread  = new Thread (BlockAndReceive);
		receiveThread.Start ();
	}
}
