using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class BitBuffer {

	private int start;
	private int size;
	private readonly LinkedList<byte> bytes;
	public byte[] Bytes { get { return bytes.ToArray (); } }

	public BitBuffer () {
		start = 0;
		size = 0;
		bytes = new LinkedList<byte> ();
	}

	public BitBuffer (byte[] bytes) {
		start = 0;
		size = bytes.Count ();
		this.bytes = new LinkedList<byte> (bytes);
	}

	public void EnqueueBool (bool value) {
		EnqueueBit ((byte) (value ? 1 : 0));
	}

	public void EnqueueInt (int value) {
		EnqueueBytes (BitConverter.GetBytes (value));
	}

	public void EnqueueBytes (byte[] values) {
		foreach (byte value in values) {
			EnqueueByte (value);
		}
	}
		
	public void EnqueueBit (byte value) {
		if (value != 0 && value != 1) {
			throw new ArgumentOutOfRangeException ("Value should be 0 or 1.");
		}
		if (size % 8 == 0) {
			bytes.AddFirst (0);
		}
		EnqueueValue (value, 1);
	}

	public void EnqueueByte (byte value) {
		if (size % 8 == 0) {
			bytes.AddFirst (value);
			size += 8;
		} else {
			int firstN = 8 - (size % 8);
			int lastN = size % 8; 
			EnqueueValue ((byte) value, firstN);
			bytes.AddFirst (0);
			EnqueueValue ((byte) (value >> firstN), lastN);
		}
	}
		
	private void EnqueueValue (byte value, int n) {
		byte b = bytes.First.Value;
		bytes.RemoveFirst ();
		b = (byte) (b | value << (size % 8));
		size += n;
		bytes.AddFirst (b);
	}

	public bool DequeueBool () {
		return DequeueBit () == 1;
	}

	public int DequeueInt () {
		return BitConverter.ToInt32 (DequeueBytes (4), 0);
	}

	public byte[] DequeueBytes (int n) {
		byte[] bytes = new byte[n];
		for (int i = 0; i < n; i++) {
			bytes [i] = DequeueByte ();
		}
		return bytes;
	}

	public byte DequeueBit () {
		return DequeueValue (1);
	}

	public byte DequeueByte () {
		if (start % 8 == 0) {
			byte b = bytes.Last.Value;
			bytes.RemoveLast ();
			return b; 
		} 
		int firstN = 8 - start;
		int lastN = start;
		byte firstB = DequeueValue (firstN);
		byte lastB = DequeueValue (lastN);
		return (byte) (firstB | (lastB << lastN));
	}

	private byte DequeueValue (int n) {
		byte b = bytes.Last.Value;
		byte mask = (byte) ((1 << n) - 1 << start);
		byte value = (byte) ((b & mask) >> start);
		start += n;
		size -= n;
		if (start % 8 == 0 || size == 0) {
			start = 0;
			bytes.RemoveLast ();
		}
		return value;
	}

	public void Print () {
		foreach (byte b in bytes) {
			Debug.Log(Convert.ToString(b, 2).PadLeft(8, '0'));
		}
	}
}
