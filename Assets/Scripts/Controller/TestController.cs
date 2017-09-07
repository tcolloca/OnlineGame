using UnityEngine;
using System.Collections;
using System.Text;
using System.Net;

public class TestController : MonoBehaviour {

	private IChannel<Datagram> channel;

	// Use this for initialization
	void Start () {
		channel = new UdpChannel ();
		channel.Open (9090);
	}
	
	// Update is called once per frame
	void Update () {
		//channel.Send (new Datagram (Encoding.ASCII.GetBytes ("hola"), new IPEndPoint (IPAddress.Parse("127.0.0.1"), 9091)));
//		Datagram dg;
//		int i = 0;
//		do {
//			dg = channel.Receive ();
//			if (dg != null) {
//				Debug.Log (Encoding.ASCII.GetString (dg.bytes));
//			}
//			i++;
//		} while (dg != null);
//		Debug.Log ("Finished update :) " + i);
	}

	public void OnApplicationQuit () {      
		Debug.Log ("QUIT!");
		channel.Close ();
	}
}
