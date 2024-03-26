using Godot;
using GodotSteam;
using System;
using System.Collections.Generic;
using System.Security;
using System.Transactions;

//Reference: https://docs.godotengine.org/en/stable/tutorials/networking/high_level_multiplayer.html#example-lobby-implementation

public partial class Lobby : Node
{
	int numPlayers = 0;
	int numPlayersLoaded = 0;
	public class PlayerInfo
	{
		public long id;
		public string name;

		public PlayerInfo(long id, string name)
		{
			this.id = id;
			this.name = name;
		}
		public PlayerInfo(long id, Godot.Collections.Dictionary<string, string> dictionary)
		{
			this.id = id;
			this.name = dictionary["name"];
		}

		public Godot.Collections.Dictionary<string, string> ToDictionary()
		{
			return new Godot.Collections.Dictionary<string, string>()
			{
				{  "name", name }
			};
		}
	}
	Dictionary<long, PlayerInfo> players = new Dictionary<long, PlayerInfo>();

	PlayerInfo myInfo = new PlayerInfo(0, "Taylor");

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Multiplayer.PeerConnected += this.HandlePeerConnected;
		Multiplayer.PeerDisconnected += this.HandlePeerDisconnected;
		Multiplayer.ConnectedToServer += this.HandleConnectedToServer;
		Multiplayer.ConnectionFailed += this.HandleConnectionFailed;
		Multiplayer.ServerDisconnected += this.HandleServerDisconnected;
	}

	public bool JoinGame(string address = "", int port = 0)
	{
		if (address == "") { address = "127.0.0.1"; }
		if (port == 0) { port = 7000; }

		var peer = new ENetMultiplayerPeer();
		Error error = peer.CreateClient(address, port);
		if (error != Error.Ok) { GD.PrintErr(error); return false; }

		Multiplayer.MultiplayerPeer = peer;
		return true;
	}

	public bool CreateGame(int port)
	{
		var peer = new ENetMultiplayerPeer();
		Error error = peer.CreateServer(port);
		if (error != Error.Ok) { GD.PrintErr(error);  return false; }

		Multiplayer.MultiplayerPeer = peer;
		players[1] = myInfo;
		//TODO: Emit PlayerConnected signal?
		return true;
	}

	[Rpc(CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
	public void LoadGame(string scenePath)
	{
		GetTree().ChangeSceneToFile(scenePath);
	}
	
	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
	public void PlayerLoaded()
	{
		if (Multiplayer.IsServer())
		{
			numPlayersLoaded++;
			if (numPlayersLoaded >= players.Count)
			{
				//TODO: Call StartGame on the main Game script in the scene
				numPlayersLoaded = 0;
			}
		}
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
	public void RegisterPlayer(Godot.Collections.Dictionary<string, string> newPlayerInfo)
	{
		long senderId = Multiplayer.GetRemoteSenderId();
		players[senderId] = new PlayerInfo(senderId, newPlayerInfo);
		//TODO: Emit PlayerConnected signal?
	}

	private void HandleServerDisconnected()
	{
		Multiplayer.MultiplayerPeer = null;
		players.Clear();
		//TODO: Emit ServerDisconnected signal?
	}

	private void HandlePeerConnected(long id)
	{
		//Send our playerInfo to new peers that connect
		Error rpcError = RpcId(id, nameof(RegisterPlayer), myInfo.ToDictionary());
		//TODO: Handle the rpcError?
	}
	private void HandlePeerDisconnected(long id)
	{
		players.Remove(id);
		//TODO: Emit PlayerDisconnected signal?
	}

	private void HandleConnectedToServer()
	{
		var peerId = Multiplayer.GetUniqueId();
		myInfo.id = peerId;
		players[peerId] = myInfo;
		//TODO: Emit PlayerConnected signal?
	}
	private void HandleConnectionFailed()
	{
		Multiplayer.MultiplayerPeer = null;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
