using System;
using UnityEngine;

public class Serializer {

	public byte[] Serialize (ISerializable obj) {
		BitBuffer bitBuffer = new BitBuffer ();
		if (ConfigProperties.Instance.isServer) {
			bitBuffer.EnqueueEnum (MessageType.SERVER, MessageType.TOTAL);
			if (obj is PlayerJoinedMessage) {
				bitBuffer.EnqueueEnum (ServerMessageType.PLAYER_JOINED, MessageType.SERVER);
			}
			if (obj is PlayerLeftMessage) {
				bitBuffer.EnqueueEnum (ServerMessageType.PLAYER_LEFT, MessageType.SERVER);
			}
			if (obj is SnapshotMessage) {
				bitBuffer.EnqueueEnum (ServerMessageType.SNAPSHOT, MessageType.SERVER);
			}
		} else {
			bitBuffer.EnqueueEnum (MessageType.CLIENT, MessageType.TOTAL);
			if (obj is GameEvent) {
				bitBuffer.EnqueueEnum (ClientMessageType.GAME, ClientMessageType.TOTAL);
				if (obj is MovementMessage) {
					bitBuffer.EnqueueEnum (GameMessageType.MOVEMENT, GameMessageType.TOTAL);
				}
			}
		}
		bitBuffer.EnqueueBytes (obj.Serialize ());
//		bitBuffer.Print ();
//		Debug.Log ("----");
		return bitBuffer.Bytes;
	}

	public 	Message Deserialize (byte[] bytes) {
		BitBuffer bitBuffer = new BitBuffer (bytes);
//		Debug.Log ("****");
//		bitBuffer.Print ();
		switch ((MessageType) bitBuffer.DequeueEnum (MessageType.TOTAL)) {
		case MessageType.CLIENT:
			switch ((ClientMessageType)bitBuffer.DequeueEnum (ClientMessageType.TOTAL)) {
			case ClientMessageType.JOIN:
				return new JoinMessage (bitBuffer);
			case ClientMessageType.LEAVE:
				return new LeaveMessage (bitBuffer);
			case ClientMessageType.GAME:
				switch ((GameMessageType)bitBuffer.DequeueEnum (GameMessageType.TOTAL)) {
				case GameMessageType.MOVEMENT:
					return new MovementMessage (bitBuffer);
				}
				break;
			}
			break;
		case MessageType.SERVER:
			switch ((ServerMessageType)bitBuffer.DequeueEnum (ServerMessageType.TOTAL)) {
			case ServerMessageType.PLAYER_JOINED:
				return new PlayerJoinedMessage (bitBuffer);
			case ServerMessageType.PLAYER_LEFT:
				return new PlayerLeftMessage (bitBuffer);
			case ServerMessageType.SNAPSHOT:
				return new SnapshotMessage (bitBuffer);
			}
			break;
		}
		throw new Exception ("Unknown object when deserializing!");
	}
}

