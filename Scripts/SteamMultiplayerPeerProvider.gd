extends Node

func CreateHost(nodeReference, port):
#
	var peer = SteamMultiplayerPeer.new();
	var createResult = peer.create_host(port, []);
	# nodeReference.multiplayer.set_multiplayer_peer(peer);
	print("Created Host ", type_string(typeof(peer)), " ", peer, " -> ", createResult, "!");
	return peer;
#

func CreateClient(nodeReference, friendId, port):
#
	var peer = SteamMultiplayerPeer.new();
	var createResult = peer.create_client(friendId, port, []);
	# nodeReference.multiplayer.set_multiplayer_peer(peer);
	print("Created Client ", type_string(typeof(peer)), " ", peer, " -> ", createResult, "!");
	return peer;
#
