using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_5_3 || UNITY_5_3_OR_NEWER
using UnityEngine.SceneManagement;
#endif

public class ColliderStreamerManager : MonoBehaviour
{
	#if UNITY_5_3 || UNITY_5_3_OR_NEWER
	
	[Tooltip ("Object that will start loading process after it hits the collider.")]
	/// <summary>
	/// The player transform.
	/// </summary>
	public Transform player;

	[Tooltip ("Collider Streamer Manager will wait for player spawn and fill it automatically")]
	/// <summary>
	/// Collider Streamer Manager will wait for player spawn and fill it automatically
	/// </summary>
	public bool spawnedPlayer;

	[HideInInspector]
	public string playerTag = "Player";
	
	/// <summary>
	/// The tag of collider streamer manager.
	/// </summary>
	public static string COLLIDERSTREAMERMANAGERTAG = "ColliderStreamerManager";

	[HideInInspector]
	/// <summary>
	/// The collider streamers.
	/// </summary>
	public List<ColliderStreamer> colliderStreamers;


	
	/// <summary>
	/// Adds the collider streamer.
	/// </summary>
	/// <param name="colliderStreamer">Collider streamer.</param>
	public void AddColliderStreamer (ColliderStreamer colliderStreamer)
	{
		colliderStreamers.Add (colliderStreamer);
	}

	/// <summary>
	/// Adds the collider scene.
	/// </summary>
	/// <param name="colliderScene">Collider scene.</param>
	public void AddColliderScene (ColliderScene colliderScene)
	{
		foreach (var item in colliderStreamers) {
			if (item != null && item.sceneName == colliderScene.sceneName) {
				item.SetSceneGameObject (colliderScene.gameObject);
				break;
			}
		}
	}

	public void Update ()
	{
		if (spawnedPlayer && player == null && !string.IsNullOrEmpty (playerTag)) {
			GameObject playerGO = GameObject.FindGameObjectWithTag (playerTag);
			if (playerGO != null)
				player = playerGO.transform;
		}
	}

	#endif
}
