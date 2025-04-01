using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LocalAreaSettings : ScriptableObject
{
	/// <summary>
	/// The collections collapsed.
	/// </summary>
	public bool collectionsCollapsed = true;

	/// <summary>
	/// The list size collections.
	/// </summary>
	public int listSizeCollections = 0;

	/// <summary>
	/// The current collections.
	/// </summary>
	public List<SceneCollection> currentCollections = new List<SceneCollection> ();

	/// <summary>
	/// The show loading point.
	/// </summary>
	public bool showLoadingPoint = true;

	/// <summary>
	/// The distance from center for scene loading.
	/// </summary>
	public int distanceFromCenter;

	/// <summary>
	/// Is distance meassured in tiles
	/// </summary>
	public bool tiles;

	/// <summary>
	/// The center point for scene loading.
	/// </summary>
	public Vector3 CenterPoint;

}
