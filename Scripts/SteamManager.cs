using Godot;
using GodotSteam;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using GDictionary = Godot.Collections.Dictionary;

public partial class SteamManager : HBoxContainer
{
	static int ANIMATE_IN_SPEED = 400; //ms
	static int ANIMATE_OUT_SPEED = 600; //ms

	[Export(hintString:"Whether we should try and initialize Steam when this script is added to the scene")]
	public bool InitAtStartup = true;
	[Export(hintString:"If true, the game will exit if Steam fails to initialize (or it will try to launch the game through Steam if the player owns the game and they just launched the exe directly)")]
	public bool SteamEnforced = false;
	[Export(hintString:"Used for starting the game from steam, if the user tries to start the installed exe directly (ony if SteamEnforced)")]
	public long RestartAppId = 0;
	[Export(hintString:"(Optional) This label will display \"Steam Enabled!\" if Steam initializes successfully")]
	public Label SteamStatusLabel = null;
	[Export(hintString: "(Optional) A reference to an container that holds the label and icon and should be animated off screen after a period of time")]
	public Container AnimatedContainer = null;
	[Export(hintString: "(Optional) If AnimatedContainer is set and this option is non-zero, the animated container will fly off screen after this many milliseconds of displaying a success status")]
	public float SuccessStatusDisappearTimeMs = 0;
	[Export(hintString: "(Optional) If AnimatedContainer is set and this option is non-zero, the animated container will fly off screen after this many milliseconds of displaying a failure status")]
	public float FailureStatusDisappearTimeMs = 0;

	Vector2 _containerInitialPos = Vector2.Zero;

	public override void _EnterTree()
	{
		_containerInitialPos = AnimatedContainer?.Position ?? Vector2.Zero;

		if (InitAtStartup && !_triedToInitialize)
		{
			string initErrorString = Initialize(SteamEnforced, RestartAppId, GetTree());
			if (SteamStatusLabel != null)
			{
				SteamStatusLabel.Text = Initialized ? "Steam Enabled!" : $"Steam Init Failed: {initErrorString}";
				SteamStatusLabel.SetModulateNonAlpha(Initialized ? Monokai.White : Monokai.Red);
			}
			else { GD.Print("WARNING: SteamStatusLabel is not set on SteamManager script"); }
		}
	}

	public override void _Process(double delta)
	{
		base._Process(delta);

		Steam.RunCallbacks();

		if (AnimatedContainer != null)
		{
			if (_triedToInitialize)
			{
				if (TimeSinceSteamInit < (float)ANIMATE_IN_SPEED)
				{
					// Animate the message in quickly right after initialization
					float animTime = 1.0f - Math.Clamp((float)TimeSinceSteamInit / (float)ANIMATE_IN_SPEED, 0.0f, 1.0f);
					float animDist = _containerInitialPos.X + AnimatedContainer.Size.X;
					AnimatedContainer.SetX(_containerInitialPos.X - (Easing.QuadraticIn(animTime) * animDist));
				}
				else if (Initialized && SuccessStatusDisappearTimeMs > 0 && (float)TimeSinceSteamInit >= SuccessStatusDisappearTimeMs)
				{
					// Animate out the success message
					float animTime = Math.Clamp(((float)TimeSinceSteamInit - SuccessStatusDisappearTimeMs) / (float)ANIMATE_OUT_SPEED, 0.0f, 1.0f);
					float animDist = _containerInitialPos.X + AnimatedContainer.Size.X;
					AnimatedContainer.SetX(_containerInitialPos.X - (Easing.QuadraticOut(animTime) * animDist));
				}
				else if (!Initialized && FailureStatusDisappearTimeMs > 0 && (float)TimeSinceSteamInit >= FailureStatusDisappearTimeMs)
				{
					// Animate out the failure message
					float animTime = Math.Clamp(((float)TimeSinceSteamInit - FailureStatusDisappearTimeMs) / (float)ANIMATE_OUT_SPEED, 0.0f, 1.0f);
					float animDist = _containerInitialPos.X + AnimatedContainer.Size.X;
					AnimatedContainer.SetX(_containerInitialPos.X - (Easing.QuadraticOut(animTime) * animDist));
				}
				else
				{
					AnimatedContainer.Position = _containerInitialPos;
				}
			}
			else
			{
				//Place it off screen if we haven't tried to initialize
				AnimatedContainer.SetX(-AnimatedContainer.Size.X);
			}
		}
	}

	// +--------------------------------------------------------------+
	// |                        Static Members                        |
	// +--------------------------------------------------------------+
	static ulong _steamInitTimeMs = 0;
	public static ulong SteamInitTimeMs => _steamInitTimeMs;
	public static ulong TimeSinceSteamInit => (Time.GetTicksMsec() - _steamInitTimeMs);

	static bool _triedToInitialize = false;
	static bool _initializedSuccessfully = false;
	public static bool Initialized => _initializedSuccessfully;

	static long _steamAppId = 0;
	public static long AppId => _steamAppId;
	public static string AppIdString => _steamAppId.ToString();
	static ulong _steamUserId = 0;
	public static ulong UserId => _steamUserId;
	public static string UserIdString => _steamUserId.ToString();
	static string _steamAppName = "SoC1"; //TODO: Can we somehow ask Steam for this name?
	public static string AppName => _steamAppName;

	public static Dictionary<ulong, ImageTexture> UserProfilePictures = new Dictionary<ulong, ImageTexture>();

	/// <summary>Called by script automatically if InitAtStartup is true</summary>
	/// <remarks>
	/// If Steam is failing to initialize, make sure you have a valid steam_appid.txt in the project folder and Steam is open+logged in with an account that owns the proper game
	/// </remarks>
	/// <returns>Returns an error string on failure. Returns null on success</returns>
	public static string Initialize(bool enforce, long restartAppId = 0, SceneTree sceneTree = null)
	{
		string result = null;


		if (!_triedToInitialize)
		{
			GDictionary initResult = Steam.SteamInit();

			if (initResult["status"].AsInt32() == 1)
			{
				GD.Print($"Steam initialized successfully! \"{initResult["verbal"]}\"");
				_initializedSuccessfully = true;
				_steamAppId = Steam.GetAppID();
				_steamUserId = Steam.GetSteamID();
			}
			else
			{
				result = $"Steam failed to initialize: Status={initResult["status"]} Message: \"{initResult["verbal"]}\"";
				GD.PrintErr(result);
				if (enforce)
				{
					if (restartAppId != 0) { Steam.RestartAppIfNecessary(restartAppId); }
					sceneTree.Quit();
				}
			}

			_steamInitTimeMs = Time.GetTicksMsec();
			_triedToInitialize = true;
		}
		else if (!_initializedSuccessfully)
		{
			result = "Previous initialization failed";
		}

		return result;
	}

	// +--------------------------------------------------------------+
	// |                General Information Functions                 |
	// +--------------------------------------------------------------+
	public static int GetSortingOfPersonaState(Steam.PersonaState state)
	{
		if (state == Steam.PersonaState.Online || state == Steam.PersonaState.LookingToPlay || state == Steam.PersonaState.LookingToTrade)
		{
			return 0;
		}
		else if (state == Steam.PersonaState.Snooze)
		{
			return 1;
		}
		else if (state == Steam.PersonaState.Away)
		{
			return 2;
		}
		return 10;
	}
	public static Color GetColorForPersonaState(Steam.PersonaState state, bool isInGame = false)
	{
		if (isInGame) { return Monokai.Green; }
		switch (state)
		{
			case Steam.PersonaState.Online: return Monokai.Blue;
			case Steam.PersonaState.Away: return new Color(Monokai.Blue, 0.75f);
			case Steam.PersonaState.Snooze: return Monokai.Orange;
			case Steam.PersonaState.Offline: return Monokai.LightGray;
			default: return Monokai.White;
		}
	}

	public static ImageTexture GetProfilePictureForUser(ulong userId)
	{
		if (UserProfilePictures.TryGetValue(userId, out ImageTexture existingTexture)) { return existingTexture; }

		//TODO: I bet some of these functions can fail. We should handle the failures gracefully
		int profilePicHandle = Steam.GetMediumFriendAvatar(userId);
		var getImageSizeResult = Steam.GetImageSize(profilePicHandle);
		int imageWidth = getImageSizeResult["width"].AsInt32();
		int imageHeight = getImageSizeResult["height"].AsInt32();
		GDictionary getImageResult = Steam.GetImageRGBA(profilePicHandle);
		byte[] pixelBuffer = getImageResult["buffer"].AsByteArray();
		Image profileImage = Image.CreateFromData(imageWidth, imageHeight, false, Image.Format.Rgba8, pixelBuffer);
		ImageTexture profileTexture = ImageTexture.CreateFromImage(profileImage);

		UserProfilePictures[userId] = profileTexture;
		return profileTexture;
	}
}
