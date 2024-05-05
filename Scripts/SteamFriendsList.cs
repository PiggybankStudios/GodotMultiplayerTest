using Godot;
using GodotSteam;
using System;
using System.Linq;
using System.Collections.Generic;
using GDictionary = Godot.Collections.Dictionary;
using System.Runtime.Serialization;
using static SteamFriendsList;

public partial class SteamFriendsList : Node
{
	[Export]
	public PackedScene ItemPrefab;
	[Export]
	public Node List;
	[Export]
	TextEdit SearchBox;
	[Export]
	Button SearchClearButton;
	[Export]
	double RefreshPeriodSecs = 0;
	[Signal]
	public delegate void SelectionChangedEventHandler(ulong selectedFriendId);

	public class Friend
	{
		public ulong Id;
		public string Name;
		public Steam.PersonaState State;
		public uint CurrentGameId;
		public string CurrentGameName;
		public Texture2D ProfileTexture;
		public SteamFriendsListItemUI UI = null;
		public bool IsSelected = false;

		public string DisplayStatus => (CurrentGameId != 0 && !CurrentGameName.IsNullOrEmpty()) ? $"Playing {CurrentGameName}" : State.ToString();

		public Friend(ulong id, string name, Texture2D profileTexture = null, Steam.PersonaState state = Steam.PersonaState.Offline, uint currentGameId = 0)
		{
			this.Id = id;
			this.Name = name;
			this.ProfileTexture = profileTexture;
			this.State = state;
			this.CurrentGameId = currentGameId;
			//TODO: For some reason Steam.GetAppName does not work properly. It returns a garbage string. For now we will just say "Another Game"
			this.CurrentGameName = (CurrentGameId != 0) ? (CurrentGameId == SteamManager.AppId ? SteamManager.AppName : "Another Game") : "";
		}

		public int Sort(Friend other)
		{
			//TODO: Add sorting of close friends to the top!
			//Prioritize players playing the same game first
			if (CurrentGameId == SteamManager.AppId && other.CurrentGameId != SteamManager.AppId) { return -1; }
			if (CurrentGameId != SteamManager.AppId && other.CurrentGameId == SteamManager.AppId) { return 1; }
			//Then players that are playing any game
			if (CurrentGameId != 0 && other.CurrentGameId == 0) { return -1; }
			if (CurrentGameId == 0 && other.CurrentGameId != 0) { return 1; }
			//Then sort based on regular PersonaState (Online, Away, etc.)
			int myStateSort = SteamManager.GetSortingOfPersonaState(State);
			int otherStateSort = SteamManager.GetSortingOfPersonaState(other.State);
			if (myStateSort > otherStateSort) { return 1; }
			else if (myStateSort < otherStateSort) { return -1; }
			else
			{
				//Finally sort on name to keep the order of the list relatively deterministic
				return Name.CompareTo(other.Name);
			}
		}
	}

	List<Friend> _friends = new List<Friend>();
	Friend _selectedFriend = null;
	public Friend SelectedFriend => _selectedFriend;
	public IEnumerable<Friend> Friends => _friends;
	Timer _refreshTimer = null;

	public override void _Ready()
	{
		if (SearchBox != null)
		{
			SearchBox.TextChanged += this.SearchBox_TextChanged;
		}
		if (SearchClearButton != null)
		{
			SearchClearButton.Pressed += this.SearchClearButton_Pressed;
		}

		Populate();

		if (SteamManager.Initialized && RefreshPeriodSecs > 0)
		{
			_refreshTimer = new Timer();
			_refreshTimer.WaitTime = RefreshPeriodSecs;
			_refreshTimer.Connect(Timer.SignalName.Timeout, Callable.From(() =>
			{
				GD.Print("Refreshing friends list");
				Populate();
			}));
			this.AddChild(_refreshTimer);
			_refreshTimer.Start();
		}
	}

	public void Populate()
	{
		ulong oldSelectedId = _selectedFriend?.Id ?? 0;
		DeselectItem();
		foreach (Friend friend in _friends)
		{
			if (friend.UI != null) { List.RemoveChild(friend.UI); }
		}
		bool isRefreshing = (_friends.Count > 0);
		_friends = new List<Friend>();

		if (SteamManager.Initialized)
		{
			{
				ulong localUserId = Steam.GetSteamID();
				string localUserName = Steam.GetFriendPersonaName(localUserId);
				uint localUserGameId = (uint)SteamManager.AppId;
				Texture2D localProfileTexture = SteamManager.GetProfilePictureForUser(localUserId);
				Friend localUser = new Friend(localUserId, localUserName, localProfileTexture, Steam.PersonaState.Online, localUserGameId);
				_friends.Add(localUser);
			}

			int numFriends = Steam.GetFriendCount(Steam.FriendFlags.Immediate);
			if (!isRefreshing) { GD.Print($"Found {numFriends} Steam Friends"); }
			for (int fIndex = 0; fIndex < numFriends; fIndex++)
			{
				//TODO: I bet some of these functions can fail. We should handle the failures gracefully
				ulong friendId = Steam.GetFriendByIndex(fIndex, Steam.FriendFlags.Immediate);
				string friendName = Steam.GetFriendPersonaName(friendId);
				Steam.PersonaState friendState = Steam.GetFriendPersonaState(friendId);
				var getGamePlayerResult = Steam.GetFriendGamePlayed(friendId);
				uint friendGameId = getGamePlayerResult.ContainsKey("id") ? getGamePlayerResult["id"].AsUInt32() : 0;
				Texture2D profileTexture = SteamManager.GetProfilePictureForUser(friendId);
				Friend newFriend = new Friend(friendId, friendName, profileTexture, friendState, friendGameId);
				_friends.Add(newFriend);
			}

			if (_friends.Count == 0)
			{
				_friends.Add(new Friend(0, "No Friends Found..."));
			}
		}
		else
		{
			GD.PrintErr("Steam not initialized! Friends list cannot populate!");
			_friends.Add(new Friend(0, "Steam not Available..."));
		}

		_friends.Sort((friend1, friend2) => friend1.Sort(friend2));

		if (List != null && ItemPrefab != null)
		{
			foreach (Friend item in _friends)
			{
				SteamFriendsListItemUI newUI = (SteamFriendsListItemUI)ItemPrefab.Instantiate();
				newUI.NameLabel.Text = item.Name;
				newUI.NameLabel.Modulate = SteamManager.GetColorForPersonaState(item.State, item.CurrentGameId != 0);
				newUI.StatusLabel.Text = item.DisplayStatus;
				newUI.StatusLabel.Modulate = SteamManager.GetColorForPersonaState(item.State, item.CurrentGameId != 0);
				if (item.ProfileTexture != null)
				{
					newUI.ProfilePic.Texture = item.ProfileTexture;
					newUI.ProfilePic.SetAlpha((item.State == Steam.PersonaState.Offline) ? 0.5f : 1.0f);
				}
				newUI.Button.Disabled = (item.Id == 0);
				newUI.Button.Connect(BaseButton.SignalName.Pressed, Callable.From(() => { FriendSelected(item); }));
				List.AddChild(newUI);
				item.UI = newUI;
			}
		}

		UpdateFiltering();

		if (oldSelectedId != 0)
		{
			Friend newSelectedFriend = _friends.FirstOrDefault(f => f.Id == oldSelectedId);
			if (newSelectedFriend != null && newSelectedFriend.UI.Visible)
			{
				SelectItem(newSelectedFriend);
			}
		}
	}

	private void UpdateFiltering()
	{
		if (!SearchBox.Text.IsNullOrEmpty())
		{
			foreach (Friend item in _friends)
			{
				string matchString = $"{item.Name} {item.DisplayStatus}";
				if (matchString.MatchesSearch(SearchBox.Text))
				{
					item.UI.Visible = true;
				}
				else
				{
					item.UI.Visible = false;
					if (item.IsSelected)
					{
						DeselectItem();
					}
				}
			}
		}
		else
		{
			foreach (Friend item in _friends)
			{
				item.UI.Visible = true;
			}
		}
	}

	private void SearchClearButton_Pressed()
	{
		SearchBox.Text = "";
		UpdateFiltering();
	}

	private void SearchBox_TextChanged()
	{
		UpdateFiltering();
	}

	private void DeselectItem()
	{
		if (_selectedFriend != null)
		{
			_selectedFriend.IsSelected = false;
			_selectedFriend.UI.SelectedRect.Modulate = Colors.Transparent;
			_selectedFriend = null;
		}
	}
	private void SelectItem(Friend item)
	{
		DeselectItem();
		item.IsSelected = true;
		item.UI.SelectedRect.Modulate = new Color(Monokai.White, 0.25f);
		_selectedFriend = item;
	}

	private void FriendSelected(Friend item)
	{
		SelectItem(item);
		EmitSignal(SignalName.SelectionChanged, item.Id);
	}
}
