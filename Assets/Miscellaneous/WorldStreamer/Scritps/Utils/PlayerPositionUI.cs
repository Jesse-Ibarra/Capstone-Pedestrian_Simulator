using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Player position U.
/// </summary>
public class PlayerPositionUI : MonoBehaviour
{
	[Tooltip ("Object that should be moved during respawn/teleport process. It must be the same as object that streamer fallows during streaming process.")]
	/// <summary>
	/// The player.
	/// </summary>
	public Transform player;

	[Tooltip ("If you use Floating Point fix system drag and drop world mover prefab from your scene hierarchy.")]
	/// <summary>
	/// The world mover.
	/// </summary>
	public WorldMover worldMover;

	/// <summary>
	/// The text.
	/// </summary>
	public Text text;

	public void Start ()
	{
		if (player == null)
			Debug.LogError ("Player is not connected to Position Gizmo");

		text.text = "Player position: Player is not connected to Position Gizmo";
	}

	/// <summary>
	/// Update this instance and shows player real position and player position after move.
	/// </summary>
	public void Update ()
	{
		if (player != null)
		if (worldMover != null)
			text.text = "Player position: " + player.transform.position + "\nPlayer real position: " + worldMover.playerPositionMovedLooped;
		else
			text.text = "Player position: " + player.transform.position + "\nPlayer real position: Not Connected to World Mover";
	}


}
