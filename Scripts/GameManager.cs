using Godot;
using GodotSteam;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using GDictionary = Godot.Collections.Dictionary;
using GArray = Godot.Collections.Array;

public partial class GameManager : Node3D
{
	public static bool CreateServer = true;
	public static bool UseSteamSocket = false;
	public static int Server_Port = 7700;
	public static string Client_Address = "localhost";
	public static ulong Client_SteamUserId = 0;
	public static int Client_Port = 7700;
	public static string Client_Name = "Test";

	[Export]
	MultiplayerSpawner MpSpawner;
	[Export]
	Node SynchronizedNode;
	[Export]
	PackedScene PlayerPrefab;
	[Export]
	GDScript SteamMultiplayerPeerProviderScript;

	//Both sides
	Player self = null;
	Dictionary<long, long> playerIdMap = new Dictionary<long, long>();
	public Dictionary<long, long> PlayerIdMap => playerIdMap;
	public long SelfId => playerIdMap.GetValueOrDefault(Multiplayer.GetUniqueId(), 0);
	public Player Self => self;
	public string LogName => $"{(Multiplayer.IsServer() ? "Server" : "Client")}{(isConnected ? SelfId.ToString() : "?")}";
	GodotObject steamProvider = null;
	MultiplayerPeer steamMultiplayerPeer = null;

	//Server Side Only
	long nextPlayerId = 1;
	List<Player> players = new List<Player>();

	//Client Side Only
	long ourId = 0;
	bool isConnected = false;

	long ServerGetPlayerId(long uniqueId)
	{
		if (playerIdMap.TryGetValue(uniqueId, out long existingId)) { return existingId; }
		long result = nextPlayerId;
		playerIdMap[uniqueId] = nextPlayerId;
		nextPlayerId++;
		return result;
	}

	public override void _Ready()
	{
		//MpSpawner.SpawnPath = SynchronizedNode.GetPath();
		//MpSpawner.AddSpawnableScene(PlayerPrefab.ResourcePath);

		Multiplayer.ConnectedToServer += this.Multiplayer_ConnectedToServer;
		Multiplayer.ConnectionFailed += this.Multiplayer_ConnectionFailed;
		Multiplayer.PeerConnected += this.Multiplayer_PeerConnected;
		Multiplayer.PeerDisconnected += this.Multiplayer_PeerDisconnected;
		Multiplayer.ServerDisconnected += this.Multiplayer_ServerDisconnected;

		if (CreateServer)
		{
			GD.Print($"Opening {(UseSteamSocket ? "Steam " : "")}server on port {Server_Port}...");
			if (UseSteamSocket)
			{
				steamProvider = (GodotObject)SteamMultiplayerPeerProviderScript.New();
				MultiplayerPeer peer = steamProvider.Call("CreateHost", this, Server_Port).As<MultiplayerPeer>();
				GD.Print($"Peer => {peer}. Status: {peer.GetConnectionStatus()}");
				Multiplayer.MultiplayerPeer = peer;
			}
			else
			{
				var peer = new ENetMultiplayerPeer();
				uint steamSocket = Steam.CreateListenSocketP2P(7700, new GArray());
				Error createError = peer.CreateServer(Server_Port);
				if (createError != Error.Ok) { GD.PrintErr($"CreateServer failed: {createError}"); }
				Multiplayer.MultiplayerPeer = peer;
			}

			isConnected = true;
			ourId = ServerGetPlayerId(Multiplayer.GetUniqueId());
			self = AddPlayer(ourId, Client_Name);
			self.Initialize(this, SynchronizedNode);
		}
		else
		{
			GD.Print($"Connecting to {(UseSteamSocket ? "Steam " : "")}server at {(UseSteamSocket ? Client_SteamUserId : $"{Client_Address}")}:{Client_Port}...");
			if (UseSteamSocket)
			{
				steamProvider = (GodotObject)SteamMultiplayerPeerProviderScript.New();
				MultiplayerPeer peer = steamProvider.Call("CreateClient", this, Client_SteamUserId, Client_Port).As<MultiplayerPeer>();
				GD.Print($"Peer => {peer}. Status: {peer.GetConnectionStatus()}");
				Multiplayer.MultiplayerPeer = peer;
			}
			else
			{
				var peer = new ENetMultiplayerPeer();
				Error createError = peer.CreateClient(Client_Address, Client_Port);
				if (createError != Error.Ok) { GD.PrintErr($"CreateClient failed: {createError}"); }
				Multiplayer.MultiplayerPeer = peer;
			}
		}
	}

	public override void _Process(double delta)
	{
		if (Input.IsKeyPressed(Key.A))
		{
			GD.Print($"Peer => {Multiplayer.MultiplayerPeer}. Status: {Multiplayer.MultiplayerPeer.GetConnectionStatus()}");
		}
	}

	private void Peer_NetworkConnectionStatusChanged(int connectHandle, GDictionary connection, int oldState)
	{
		GD.Print($"NetworkConnectionStatusChanged({connectHandle}, {connection}, {oldState})");
	}

	Player AddPlayer(long id, string name)
	{
		Player newPlayer = (Player)PlayerPrefab.Instantiate();
		newPlayer.Name = $"Player{id}";
		newPlayer.DisplayName = name;
		newPlayer.Id = id;
		SynchronizedNode.AddChild(newPlayer);
		players.Add(newPlayer);
		return newPlayer;
	}
	
	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
	void AssignId(long id)
	{
		ourId = id;
		playerIdMap[1] = 1; //Server is always id 1
		playerIdMap[Multiplayer.GetUniqueId()] = id;
		isConnected = true;
		string expectedName = $"Player{id}";
		Node playerNode = SynchronizedNode.FindChild(expectedName, owned: false);
		GD.Print($"{LogName}: Looking for \"{expectedName}\" in [{SynchronizedNode.GetChildren().Select(node => node.Name.ToString()).StringJoin(", ")}] => {playerNode} <=");
		self = (Player)playerNode;
		self.Initialize(this, SynchronizedNode);
	}

	private void Multiplayer_PeerConnected(long id)
	{
		GD.Print($"{LogName}: PeerConnected({id})");
		if (Multiplayer.IsServer())
		{
			long newPlayerId = ServerGetPlayerId(id);
			Player newPlayer = AddPlayer(newPlayerId, "New Player");
			GD.Print($"{LogName}: Spawned \"{newPlayer.Name}\"");
			RpcId(id, nameof(AssignId), newPlayerId);
			newPlayer.Initialize(this, SynchronizedNode);
		}
	}
	private void Multiplayer_PeerDisconnected(long id)
	{
		GD.Print($"{LogName}: PeerDisconnected({id})");
	}

	private void Multiplayer_ConnectedToServer()
	{
		GD.Print($"{LogName}: ConnectedToServer()");
	}

	private void Multiplayer_ServerDisconnected()
	{
		GD.Print("ServerDisconnected()");
		isConnected = false;
	}
	private void Multiplayer_ConnectionFailed()
	{
		GD.Print("ConnectionFailed()");
		isConnected = false;
	}
}
