using System;

public interface MessageListener {

	/* SERVER MESSAGES */

	void onPlayerJoined(PlayerJoinedMessage message);

	void onPlayerLeft(PlayerLeftMessage message);

	void onSnapshot(SnapshotMessage message);

	/* CLIENT MESSAGES */

	void onJoin (JoinMessage message);

	void onLeave (LeaveMessage message);

	void onMovement (MovementMessage message);
}

