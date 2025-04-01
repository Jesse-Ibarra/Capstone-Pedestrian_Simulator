using UnityEngine;
using System.Collections;

/// <summary>
/// Object to move by world mover.
/// </summary>
public class ObjectToMove : MonoBehaviour
{
	/// <summary>
	/// The world mover.
	/// </summary>
	//	public WorldMover worldMover;

	void Start ()
	{

		GameObject.FindGameObjectWithTag (WorldMover.WORLDMOVERTAG).GetComponent<WorldMover> ().AddObjectToMove (transform);

	}
}
