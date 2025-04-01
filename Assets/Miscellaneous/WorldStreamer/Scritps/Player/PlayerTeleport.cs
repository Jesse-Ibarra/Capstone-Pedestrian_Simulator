using UnityEngine;
using System.Collections;

/// <summary>
/// Player teleport which reloads tiles at teleport destination.
/// </summary>
public class PlayerTeleport : MonoBehaviour
{
	[Tooltip ("If teleport/respawn should initiate loading screen, drag and drop here your \"Loading Screen UI\" object  from your scene hierarchy or an object that contain \"UI Loading Streamer\" script.")]
	/// <summary>
	/// The loading screen UI.
	/// </summary>
	public UILoadingStreamer uiLoadingStreamer;


	[Tooltip ("If teleport/respawn should initiate player move to safe position.")]
	/// <summary>
	/// The loading screen UI.
	/// </summary>
	public PlayerMover playerMover;

	[Tooltip ("List of streamers. Drag and drop here all your streamer objects from scene hierarchy.")]
	/// <summary>
	/// The world streamer.
	/// </summary>
	public Streamer[] streamers;

	[Tooltip ("Object that should be moved during respawn/teleport process. It must be the same as object that streamer fallows during streaming process.")]
	/// <summary>
	/// The player transform.
	/// </summary>
	public Transform player;

	[Tooltip ("If you use Floating Point fix system drag and drop world mover prefab from your scene hierarchy.")]
	/// <summary>
	/// The world mover.
	/// </summary>
	public WorldMover worldMover;

	/// <summary>
	/// Teleport the player and shows loading screen.
	/// </summary>
	/// <param name="showLoadingScreen">If set to <c>true</c> shows loading screen after teleport.</param>
	public void Teleport (bool showLoadingScreen)
	{
		if (player != null) {
			
			player.position = transform.position + ((worldMover == null) ? Vector3.zero : worldMover.currentMove);
			player.rotation = transform.rotation;
			foreach (var streamer in streamers) {
				streamer.showLoadingScreen = showLoadingScreen;
				streamer.CheckPositionTiles ();

			}
			if (uiLoadingStreamer != null)
				uiLoadingStreamer.Show ();

			if (playerMover != null)
				playerMover.MovePlayer ();

		} else {
			if (streamers [0] != null && streamers [0].player != null) {
				player = streamers [0].player;
			}
		}
	}

	/// <summary>
	/// Raises the draw gizmos selected event.
	/// </summary>
	void OnDrawGizmosSelected ()
	{
		// Display the explosion radius when selected
		Gizmos.color = new Color (0.4f, 0.7f, 1, 0.5f);
		Gizmos.DrawSphere (transform.position, 1);
	}
}