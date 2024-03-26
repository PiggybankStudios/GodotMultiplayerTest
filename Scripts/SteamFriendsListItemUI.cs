using Godot;
using System;

public partial class SteamFriendsListItemUI : HBoxContainer
{
	[Export]
	public Label NameLabel;
	[Export]
	public Label StatusLabel;
	[Export]
	public TextureRect ProfilePic;
	[Export]
	public Button Button;
	[Export]
	public ColorRect SelectedRect;
}
