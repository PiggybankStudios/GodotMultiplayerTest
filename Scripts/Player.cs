using Godot;
using System;

public partial class Player : MeshInstance3D
{
	[Export]
	float CameraDistance = 4;
	[Export]
	float CameraRotationSpeed = 0.05f;
	[Export]
	float WalkSpeed = 5f;
	[Export]
	float RunSpeed = 15f;
	[Export]
	public Label3D NameLabel;
	[Export]
	public string DisplayName = "";
	[Export]
	public MultiplayerSynchronizer Synchronizer;
	[Export]
	public PackedScene BulletPrefab;
	[Export]
	public long Id = 0;

	bool _initialized = false;
	GameManager GameManager;
	Node SynchronizedNode;

	float cameraAngleHori = 0.0f;
	float cameraAngleVert = 0.0f;

	public bool IsLocal => (GameManager != null && Id == GameManager.SelfId);
	public string NetworkName => $"{(Multiplayer.IsServer() ? "Server" : "Client")}{Multiplayer.GetUniqueId()}";
	public Camera3D Camera = null;

	public override void _EnterTree()
	{
		Synchronizer.Synchronized += this._Synchronized;
		GD.Print($"{NetworkName}: \"{Name}\" ready to initialize...");
	}

	public void Initialize(GameManager GameManager, Node SynchronizedNode)
	{
		this.GameManager = GameManager;
		this.SynchronizedNode = SynchronizedNode;
		if (!_initialized)
		{
			if (IsLocal)
			{
				Camera = new Camera3D();
				Camera.Position = new Vector3(10, 0, 10);
				AddChild(Camera);
				cameraAngleHori = Mathf.Atan2(Camera.Position.Z - Position.Z, Camera.Position.X - Position.X);
				cameraAngleVert = Mathf.Atan2(Camera.Position.Y - Position.Y, (new Vector2(Camera.Position.X, Camera.Position.Z) - new Vector2(Position.X, Position.Z)).Length());
				if (cameraAngleHori < 0) { cameraAngleHori += Mathf.Pi * 2; }
				cameraAngleVert = Math.Clamp(cameraAngleVert, -Mathf.Pi / 2 + 0.01f, Mathf.Pi / 2 - 0.01f);
			}
			GD.Print($"{NetworkName}: Initialized Player{Id} \"{DisplayName}\" as {(IsLocal ? "local" : "remote")}");
			UpdateNameLabel();
		}
	}

	private void UpdateNameLabel()
	{
		if (NameLabel != null)
		{
			NameLabel.Text = $"{DisplayName}{(IsLocal ? " (You)" : "")}";
		}
	}

	private void _Synchronized()
	{
		UpdateNameLabel();
	}

	static int nextBulletId = 1;
	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal=true)]
	void SpawnBullet()
	{
		Node3D newBullet = (Node3D)BulletPrefab.Instantiate();
		newBullet.Position = Position;
		newBullet.Name = $"Bullet{nextBulletId++}";
		SynchronizedNode.AddChild(newBullet);
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal=true)]
	void MovePlayer(Vector3 newPosition)
	{
		Position = newPosition;
	}

	public override void _Process(double delta)
	{
		//GD.Print($"Player[{Id}]: \"{DisplayName}\"{(IsLocal ? " (Local)" : "")}");

		if (IsLocal && Camera != null)
		{
			if (Input.IsKeyPressed(Key.Left))
			{
				cameraAngleHori += CameraRotationSpeed;
				if (cameraAngleHori >= Mathf.Pi * 2) { cameraAngleHori -= Mathf.Pi * 2; }
			}
			if (Input.IsKeyPressed(Key.Right))
			{
				cameraAngleHori -= CameraRotationSpeed;
				if (cameraAngleHori < 0) { cameraAngleHori += Mathf.Pi * 2; }
			}
			if (Input.IsKeyPressed(Key.Up))
			{
				cameraAngleVert += CameraRotationSpeed;
				cameraAngleVert = Math.Clamp(cameraAngleVert, -Mathf.Pi / 2 + 0.01f, Mathf.Pi / 2 - 0.01f);
			}
			if (Input.IsKeyPressed(Key.Down))
			{
				cameraAngleVert -= CameraRotationSpeed;
				cameraAngleVert = Math.Clamp(cameraAngleVert, -Mathf.Pi / 2 + 0.01f, Mathf.Pi / 2 - 0.01f);
			}

			float moveSpeed = (Input.IsKeyPressed(Key.Shift) ? RunSpeed : WalkSpeed) * (float)delta;
			Vector3 horiForward = new Vector3(-Mathf.Cos(cameraAngleHori), 0, -Mathf.Sin(cameraAngleHori));
			Vector3 horiRight = new Vector3(Mathf.Sin(cameraAngleHori), 0, -Mathf.Cos(cameraAngleHori));

			float cameraHoriDist = Mathf.Cos(cameraAngleVert) * CameraDistance;
			Camera.Position = new Vector3(
				Mathf.Cos(cameraAngleHori) * cameraHoriDist,
				Mathf.Sin(cameraAngleVert) * CameraDistance,
				Mathf.Sin(cameraAngleHori) * cameraHoriDist
			);
			Camera.LookAt(Position);

			if (Input.IsKeyPressed(Key.W))
			{
				Position += horiForward * moveSpeed;
			}
			if (Input.IsKeyPressed(Key.A))
			{
				Position -= horiRight * moveSpeed;
			}
			if (Input.IsKeyPressed(Key.S))
			{
				Position -= horiForward * moveSpeed;
			}
			if (Input.IsKeyPressed(Key.D))
			{
				Position += horiRight * moveSpeed;
			}

			RpcId(1, nameof(MovePlayer), Position);

			if (Input.IsKeyPressed(Key.Space) && BulletPrefab != null)
			{
				RpcId(1, nameof(SpawnBullet));
			}
		}
	}
}
