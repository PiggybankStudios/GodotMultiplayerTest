using Godot;
using System;
using System.Reflection.Metadata.Ecma335;
using GArray = Godot.Collections.Array;
using GDictionary = Godot.Collections.Dictionary;

#if false
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
	}

	public MultiplayerPeerExtension ToMultiplayerPeer()
	{
		return (nativeInstance as MultiplayerPeerExtension)!;
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
		return nativeInstance.Call(Methods.CreateClient, identityRemote, remoteVirtualPort, options).As<Error>();
	}

	public ulong GetSteam64FromPeerId(uint peerId)
	{
		return nativeInstance.Call(Methods.GetSteam64FromPeerId, peerId).AsUInt32();
	}

	public int ListenSocket
	{
		get => nativeInstance.Call(Methods.GetListenSocket).As<int>();
		set => nativeInstance.Call(Methods.SetListenSocket, value).As<int>();
	}

	//TODO: Add ListenSocket property

	// == Signals ==

	public static class Signals
	{
		public static StringName NetworkConnectionStatusChanged = "network_connection_status_changed";
	}

#if false
	public delegate void NetworkConnectionStatusChangedEventHandler(int connectHandle, GDictionary connection, int oldState);
	private static event NetworkConnectionStatusChangedEventHandler NetworkConnectionStatusChangedEvent;
	public static Action<int, GDictionary, int> _networkConnectionStatusChangedAction = () =>
	{
		InputActionEventDelegate?.Invoke();
	};
	public event NetworkConnectionStatusChangedEventHandler NetworkConnectionStatusChanged
	{
		add
		{
			if (NetworkConnectionStatusChangedEvent == null)
			{
				nativeInstance.Connect(Signals.NetworkConnectionStatusChanged, Callable.From(_networkConnectionStatusChangedAction));
			}
			NetworkConnectionStatusChangedEvent += value;
		}
		remove
		{
			NetworkConnectionStatusChangedEvent -= value;
			if (NetworkConnectionStatusChangedEvent == null)
			{
				nativeInstance.Disconnect(Signals.NetworkConnectionStatusChanged, Callable.From(_networkConnectionStatusChangedAction));
			}
		}
	}
#endif

	public delegate void NetworkConnectionStatusChangedSignalEventHandler(int connectHandle, GDictionary connection, int oldState);
	private static event NetworkConnectionStatusChangedSignalEventHandler NetworkConnectionStatusChangedSignalEvent;
	static Action<int, GDictionary, int> _networkConnectionStatusChangedSignalAction = (connectHandle, connection, oldState) =>
	{
		NetworkConnectionStatusChangedSignalEvent?.Invoke(connectHandle, connection, oldState);
	};
	public event NetworkConnectionStatusChangedSignalEventHandler NetworkConnectionStatusChanged
	{
		add
		{
			if (NetworkConnectionStatusChangedSignalEvent == null)
			{
				nativeInstance.Connect(Signals.NetworkConnectionStatusChanged, Callable.From(_networkConnectionStatusChangedSignalAction));
			}

			NetworkConnectionStatusChangedSignalEvent += value;
		}
		remove
		{
			NetworkConnectionStatusChangedSignalEvent -= value;

			if (NetworkConnectionStatusChangedSignalEvent == null)
			{
				nativeInstance.Disconnect(Signals.NetworkConnectionStatusChanged, Callable.From(_networkConnectionStatusChangedSignalAction));
			}
		}
	}
}
#endif
