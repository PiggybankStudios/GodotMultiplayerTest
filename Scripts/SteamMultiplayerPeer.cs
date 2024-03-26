using Godot;
using System;
using GArray = Godot.Collections.Array;
using GDictionary = Godot.Collections.Dictionary;

public partial class SteamMultiplayerPeer
{
	static string NATIVE_CLASS_NAME = "SteamMultiplayerPeer";

	public GodotObject nativeInstance;

	public SteamMultiplayerPeer()
	{
		if (!ClassDB.ClassExists(NATIVE_CLASS_NAME))
		{
			throw new Exception("SteamMultiplayerPeer is not installed.");
		}

		if (!ClassDB.CanInstantiate(NATIVE_CLASS_NAME))
		{
			throw new Exception("SteamMultiplayerPeer cannot be instantiated.");
		}

		nativeInstance = ClassDB.Instantiate(NATIVE_CLASS_NAME).AsGodotObject();

		GD.Print($"SignalList: {nativeInstance.GetSignalList()}");

		_avatarLoadedAction = (connectHandle, connection, oldState) =>
		{
			NetworkConnectionStatusChangedEvent?.Invoke(connectHandle, connection, oldState);
		};
	}

	public MultiplayerPeerExtension ToMultiplayerPeer()
	{
		return (nativeInstance as MultiplayerPeerExtension);
	}

	// == Methods ==

	public static class Methods
	{
		public static readonly StringName CreateHost = "create_host";
		public static readonly StringName CreateClient = "create_client";
		public static readonly StringName SetListenSocket = "set_listen_socket";
		public static readonly StringName GetListenSocket = "get_listen_socket";
		public static readonly StringName GetSteam64FromPeerId = "get_steam64_from_peer_id";
	}

	public Error CreateHost(int localVirtualPort, GArray options)
	{
		return nativeInstance.Call(Methods.CreateHost, localVirtualPort, options).As<Error>();
	}
	public Error CreateClient(ulong identityRemote, int remoteVirtualPort, GArray options)
	{
		return nativeInstance.Call(Methods.CreateHost, identityRemote, remoteVirtualPort, options).As<Error>();
	}

	public ulong GetSteam64FromPeerId(uint peerId)
	{
		return nativeInstance.Call(Methods.GetSteam64FromPeerId, peerId).AsUInt32();
	}

	//TODO: Add ListenSocket property

	// == Signals ==

	public static class Signals
	{
		public static StringName NetworkConnectionStatusChanged = "network_connection_status_changed";
	}

	public delegate void NetworkConnectionStatusChangedEventHandler(int connectHandle, GDictionary connection, int oldState);
	private event NetworkConnectionStatusChangedEventHandler NetworkConnectionStatusChangedEvent;
	Action<int, GDictionary, int> _avatarLoadedAction;
	public event NetworkConnectionStatusChangedEventHandler NetworkConnectionStatusChanged
	{
		add
		{
			if (NetworkConnectionStatusChangedEvent == null)
			{
				nativeInstance.Connect(Signals.NetworkConnectionStatusChanged, Callable.From(_avatarLoadedAction));
			}
			NetworkConnectionStatusChangedEvent += value;
		}
		remove
		{
			NetworkConnectionStatusChangedEvent -= value;
			if (NetworkConnectionStatusChangedEvent == null)
			{
				nativeInstance.Disconnect(Signals.NetworkConnectionStatusChanged, Callable.From(_avatarLoadedAction));
			}
		}
	}
}
