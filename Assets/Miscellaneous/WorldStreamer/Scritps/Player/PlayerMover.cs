using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerMover : MonoBehaviour
{

	[Tooltip ("List of streamers objects that should affect loading screen. Drag and drop here all your streamer objects from scene hierarchy which should be used in loading screen.")]
	/// <summary>
	/// The streamers.
	/// </summary>
	public Streamer[] streamers;

	[Space (10)]
	[Tooltip ("Drag and drop here, an object that system have to follow during streaming process.")]
	/// <summary>
	/// The player transform.
	/// </summary>
	public Transform player;


	[Tooltip ("The player safe position during loading.")]
	/// <summary>
	/// The player safe position during loading.
	/// </summary>
	public Transform safePosition;

	[Space (10)]
	public UnityEvent onDone;


	private GameObject temporaryObject;

	private float progress = 0;

	private bool waitForPlayer = false;
	private bool playerMoved = false;

	/// <summary>
	/// Awake this instance.
	/// </summary>
	void Awake ()
	{
		if (streamers.Length > 0) {
			if (streamers [0].spawnedPlayer == false) {
				MovePlayer ();

			} else {
				waitForPlayer = true;
			}
		}


	}




	/// <summary>
	/// Update this instance, and sets current progress of streaming.
	/// </summary>
	void Update ()
	{
		if (waitForPlayer) {
			if (player == null && !string.IsNullOrEmpty (streamers [0].playerTag)) {
				GameObject playerGO = GameObject.FindGameObjectWithTag (streamers [0].playerTag);
				if (playerGO != null) {
					player = playerGO.transform;
					MovePlayer ();
					waitForPlayer = false;
				}
			}
		} else {
			if (!playerMoved) {
				if (streamers.Length > 0) {

					bool initialized = true;

					progress = 0;
					foreach (var item in streamers) {
						progress += item.LoadingProgress / (float)streamers.Length;
						initialized = initialized && item.initialized;
					}
					if (initialized) {
						if (progress >= 1) {
							if (onDone != null)
								onDone.Invoke ();
							Done ();
						}
					}

				} else
					Debug.Log ("No streamer Attached");
			}
		}
	}

	public void Done ()
	{
		player.position = temporaryObject.transform.position;
		player.rotation = temporaryObject.transform.rotation;

		foreach (var item in streamers) {
			item.player = player;
		}
		Destroy (temporaryObject);
		playerMoved = true;
		gameObject.SetActive (false);
	}

	/// <summary>
	/// Moves player for loading;
	/// </summary>
	public void MovePlayer ()
	{
		temporaryObject = new GameObject ("Temporary");
		temporaryObject.transform.position = player.position;
		temporaryObject.transform.rotation = player.rotation;
		foreach (var item in streamers) {
			item.player = temporaryObject.transform;
		}
		player.position = safePosition.position;
		player.rotation = safePosition.rotation;

		gameObject.SetActive (true);
		playerMoved = false;
	}

}
