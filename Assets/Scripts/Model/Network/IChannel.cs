using UnityEngine;
using System.Collections;

public interface IChannel<T> {

	void Open (int port);

	void Close ();

	void Send (T content);

	T Receive ();
}
