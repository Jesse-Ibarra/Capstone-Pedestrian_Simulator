using UnityEngine;
using System.Collections;

#if UNITY_5_3 || UNITY_5_3_OR_NEWER
using UnityEngine.SceneManagement;
#endif

/// <summary>
/// Collider streamer.
/// </summary>
public class ColliderStreamer : MonoBehaviour
{
	#if UNITY_5_3 || UNITY_5_3_OR_NEWER

	[Tooltip ("Scene name that belongs to this collider.")]
	/// <summary>
	/// The name of the scene.
	/// </summary>
	public string sceneName;


	[Tooltip ("Path where collider streamer should find scene which has to loaded after collider hit.")]
	/// <summary>
	/// The scene path.
	/// </summary>
	public string scenePath;

	[HideInInspector]
	/// <summary>
	/// The scene game object.
	/// </summary>
	public GameObject sceneGameObject;

	[HideInInspector]
	/// <summary>
	/// The collider streamer manager.
	/// </summary>
	public ColliderStreamerManager colliderStreamerManager;


	[Tooltip ("If it's checkboxed only player could activate collider to start loading, otherwise every physical hit could activate it.")]
	/// <summary>
	/// The player only activate.
	/// </summary>
	public bool playerOnlyActivate = true;


	[Tooltip ("Time in seconds after which scene will be unloaded when \"Player\" or object that activate loading will left collider area.")]
	/// <summary>
	/// The unload timer.
	/// </summary>
	public float unloadTimer = 0;

	private bool loaded = false;

	/// <summary>
	/// Start this instance adds to world mover and searches for collider streamer prefab.
	/// </summary>
	void Start ()
	{
		colliderStreamerManager = GameObject.FindGameObjectWithTag (ColliderStreamerManager.COLLIDERSTREAMERMANAGERTAG).GetComponent<ColliderStreamerManager> ();

		colliderStreamerManager.AddColliderStreamer (this);


		GameObject mover = GameObject.FindGameObjectWithTag (WorldMover.WORLDMOVERTAG);
		if (mover)
			mover.GetComponent<WorldMover> ().AddObjectToMove (transform);

	}

	/// <summary>
	/// Sets the scene game object and moves it to collider streamer position
	/// </summary>
	/// <param name="sceneGameObject">Scene game object.</param>
	public void SetSceneGameObject (GameObject sceneGameObject)
	{
		this.sceneGameObject = sceneGameObject;
		this.sceneGameObject.transform.position = transform.position;

	}


	/// <summary>
	/// Raises the trigger enter event and loads collider scene
	/// </summary>
	/// <param name="other">Other.</param>
	void OnTriggerEnter (Collider other)
	{
		//Debug.Log ("ontrigg " + other.transform.name);

		if (!playerOnlyActivate || other.transform == colliderStreamerManager.player) {
			//Debug.Log ("loadscene async");
			if (!loaded) {
				loaded = true;
				SceneManager.LoadSceneAsync (sceneName, LoadSceneMode.Additive);
			}
		}
	
	}

	/// <summary>
	/// Raises the trigger exit event, and destroys scene game object
	/// </summary>
	/// <param name="other">Other.</param>
	void OnTriggerExit (Collider other)
	{
		
		if ((!playerOnlyActivate || other.transform == colliderStreamerManager.player) && sceneGameObject) {
			loaded = false;
			Invoke ("UnloadScene", unloadTimer);
		}
	}

	/// <summary>
	/// Unloads the scene.
	/// </summary>
	void UnloadScene ()
	{
		
		Destroy (sceneGameObject);
	}

	#endif
}
