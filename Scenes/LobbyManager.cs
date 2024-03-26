using Godot;
using System;

public partial class LobbyManager : Node2D
{
	[Export]
	PackedScene MainGameScene;

	[Export]
	TextEdit NameTextbox;
	[Export]
	TextEdit IpAddressTextbox;
	[Export]
	TextEdit PortTextbox;
	[Export]
	Button HostButton;
	[Export]
	Button JoinButton;
	[Export]
	Button JoinFriendButton;
	[Export]
	Button InviteFriendButton;
	[Export]
	Button AcceptInviteButton;
	[Export]
	Button StartSteamServerButton;
	[Export]
	SteamFriendsList FriendsList;

	public override void _Ready()
	{
		HostButton.Pressed += this.HostButton_Pressed;
		JoinButton.Pressed += this.JoinButton_Pressed;
		JoinFriendButton.Pressed += this.JoinFriendButton_Pressed;
		InviteFriendButton.Pressed += this.InviteFriendButton_Pressed;
		AcceptInviteButton.Pressed += this.AcceptInviteButton_Pressed;
		StartSteamServerButton.Pressed += this.StartSteamServerButton_Pressed;
	}

	private void HostButton_Pressed()
	{
		GD.Print($"Clicked Host Port={PortTextbox.Text}");
		GameManager.CreateServer = true;
		GameManager.Server_Port = int.Parse(PortTextbox.Text);
		GameManager.Client_Name = NameTextbox.Text;
		GetTree().ChangeSceneToPacked(MainGameScene);
		
	}
	private void JoinButton_Pressed()
	{
		GD.Print($"Clicked Join Address={IpAddressTextbox.Text}:{PortTextbox.Text}");
		GameManager.CreateServer = false;
		GameManager.Client_Address = IpAddressTextbox.Text;
		GameManager.Client_Port = int.Parse(PortTextbox.Text);
		GameManager.Client_Name = NameTextbox.Text;
		GetTree().ChangeSceneToPacked(MainGameScene);
	}
	private void JoinFriendButton_Pressed()
	{
		GD.Print($"Clicked JoinFriend UserId={FriendsList.SelectedFriend.Id}:{PortTextbox.Text}");
		GameManager.CreateServer = false;
		GameManager.UseSteamSocket = true;
		GameManager.Client_SteamUserId = FriendsList.SelectedFriend.Id;
		GameManager.Client_Port = int.Parse(PortTextbox.Text);
		GetTree().ChangeSceneToPacked(MainGameScene);
	}
	private void InviteFriendButton_Pressed()
	{
		GD.Print("Clicked InviteFriend");
	}
	private void AcceptInviteButton_Pressed()
	{
		GD.Print("Clicked AcceptInvite");
	}
	private void StartSteamServerButton_Pressed()
	{
		GD.Print($"Clicked StartSteamServer Port={PortTextbox.Text}");
		GameManager.CreateServer = true;
		GameManager.UseSteamSocket = true;
		GameManager.Server_Port = int.Parse(PortTextbox.Text);
		GetTree().ChangeSceneToPacked(MainGameScene);
	}
}
