using UnityEngine;
using System.Collections;

/// <summary>
/// Collider scene.
/// </summary>
public class ColliderScene : MonoBehaviour
{
	#if UNITY_5_3 || UNITY_5_3_OR_NEWER

	/// <summary>
	/// The name of the scene.
	/// </summary>
	public string sceneName;

	/// <summary>
	/// Start this instance adds to world mover and searches for collider streamer prefab.
	/// </summary>
	void Start ()
	{
		GameObject.FindGameObjectWithTag (ColliderStreamerManager.COLLIDERSTREAMERMANAGERTAG).GetComponent<ColliderStreamerManager> ().AddColliderScene (this);

		GameObject mover = GameObject.FindGameObjectWithTag (WorldMover.WORLDMOVERTAG);
		if (mover)
			mover.GetComponent<WorldMover> ().AddObjectToMove (transform);

	}
	#endif
}
