using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class BitBuffer {

	private int start;
	private int size;
	private readonly LinkedList<byte> bytes;

	public byte[] Bytes { 
		get { 
			if (start == 0) {
				return bytes.ToArray (); 
			} else {
				return DequeueBytes ((int) Math.Ceiling (size / 8.0));
			}
		} 
	}

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

	public void EnqueueSerializable (IBitBufferSerializable serializable) {
		serializable.Serialize (this);
	}

	public void EnqueueBool (bool value) {
		EnqueueBit ((byte) (value ? 1 : 0));
	}

	public void EnqueueInt (int value) {
		EnqueueBytes (BitConverter.GetBytes (value));
	}

	public void EnqueueEnum (Enum value, Enum total) {
		EnqueueBits ((byte) Convert.ToByte(value), requiredBits (total));
	}

	public void EnqueueBytes (byte[] values) {
		foreach (byte value in values) {
			EnqueueByte (value);
		}
	}
		
	public void EnqueueByte (byte value) {
		EnqueueBits (value, 8);
	}
		
	public void EnqueueBit (byte value) {
		if (value != 0 && value != 1) {
			throw new ArgumentOutOfRangeException ("Value should be 0 or 1.");
		}
		EnqueueValue ((byte) value, 1);
	}

	public void EnqueueBits (byte value, int n) {
		if (n == 0) {
			Debug.LogWarning ("Enqueuing 0 bits.");
			return;
		}
		int firstN = Math.Min (8 - (size % 8), n);
		int lastN = n - firstN;
		EnqueueValue ((byte) value, firstN);
		if (lastN > 0) {
			EnqueueValue ((byte) (value >> firstN), lastN);
		}
	}
		
	private void EnqueueValue (byte value, int n) {
		if (size % 8 == 0) {
			bytes.AddFirst (0);
		}
		int mask = (1 << n) - 1;
		byte b = bytes.First.Value;
		bytes.RemoveFirst ();
		b = (byte) (b | ((mask & value) << (size % 8)));
		size += n;
		bytes.AddFirst (b);
	}

	public bool DequeueBool () {
		return DequeueBit () == 1;
	}

	public int DequeueInt () {
		return BitConverter.ToInt32 (DequeueBytes (4), 0);
	}

	public int DequeueEnum (Enum total) {
		return DequeueBits (requiredBits (total));
	}

	public byte[] DequeueBytes (int n) {
		byte[] bytes = new byte[n];
		for (int i = 0; i < n; i++) {
			bytes [i] = DequeueByte ();
		}
		return bytes;
	}

	public byte DequeueByte () {
		return DequeueBits (8);
	}

	public byte DequeueBit () {
		return DequeueValue (1);
	}

	public byte DequeueBits (int n) {
		if (n == 0) {
			Debug.LogWarning ("Dequeueing 0 bits");
			return 0;
		}
		int firstN = n - start;
		int lastN = n - firstN;
		byte firstB = DequeueValue (firstN);
		if (lastN > 0) {
			byte lastB = DequeueValue (lastN);
			return (byte) (firstB | (lastB << lastN));
		}
		return firstB;
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

	private int requiredBits (Enum totalEnum) {
		int n = 0;
		int total = Convert.ToInt32 (totalEnum) - 1;
		while (total != 0) {
			total = total >> 1;
			n++;
		}
		return n;
	}

	public void Print () {
		foreach (byte b in bytes) {
			Debug.Log(Convert.ToString(b, 2).PadLeft(8, '0'));
		}
	}
}
