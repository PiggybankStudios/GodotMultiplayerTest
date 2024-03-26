using Godot;
using System;
using System.Linq;

public partial class TestSignalReceiver : Node
{
	[Export]
	SteamFriendsList friendsList;

	public void UlongSignalReceived(ulong id)
	{
		GD.Print($"UlongSignalReceived({id})");
		if (friendsList != null)
		{
			SteamFriendsList.Friend selectedFriend = friendsList.Friends.FirstOrDefault(f => f.Id == id);
			if (selectedFriend != null)
			{
				GD.Print($"Friend: {selectedFriend.Name}");
			}
			else
			{
				GD.Print("Friend: Unknown");
			}
		}
	}
}