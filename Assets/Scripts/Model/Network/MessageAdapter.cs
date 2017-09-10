using System;

public class MessageAdapter : MessageListener {

	public virtual void onPlayerJoined (PlayerJoinedMessage message) {}

	public virtual void onPlayerLeft (PlayerLeftMessage message) {}

	public virtual void onSnapshot(SnapshotMessage message) {}

	public virtual void onJoin (JoinMessage message) {} 

	public virtual void onLeave (LeaveMessage message) {} 

	public virtual void onMovement (MovementMessage message) {}

}

