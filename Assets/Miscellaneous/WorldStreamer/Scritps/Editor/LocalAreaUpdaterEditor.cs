using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;


/// <summary>
/// Multi scene helper for working with multi scene editor.
/// </summary>
public class LocalAreaUpdaterEditor : EditorWindow
{
	/// <summary>
	/// The collections collapsed.
	/// </summary>
	bool collectionsCollapsed = true;
	/// <summary>
	/// The list size collections.
	/// </summary>
	int listSizeCollections = 0;
	/// <summary>
	/// The current collections.
	/// </summary>
	List<SceneCollection> currentCollections = new List<SceneCollection> ();


	/// <summary>
	/// Warning info
	/// </summary>
	string warning = "";

	/// <summary>
	/// The loaded scenes.
	/// </summary>
	List<UnityEngine.SceneManagement.Scene> loadedScenes = new List<UnityEngine.SceneManagement.Scene> ();

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

	/// <summary>
	/// The old center point.
	/// </summary>
	private Vector3 oldCenterPoint;

	/// <summary>
	/// The splits of tiles.
	/// </summary>
	List<Dictionary<string,GameObject>> splits = new List<Dictionary<string,GameObject>> ();

	/// <summary>
	/// The scroll position.
	/// </summary>
	private Vector2 scrollPos;


	private bool closed = true;

	/// <summary>
	/// Init this instance.
	/// </summary>
	[MenuItem ("World Streamer/Local Area Updater")]
	static void Init ()
	{
		// Get existing open window or if none, make a new one:
		LocalAreaUpdaterEditor window = EditorWindow.GetWindow <LocalAreaUpdaterEditor> ("Local Area");
		window.Show ();
		SceneView.onSceneGUIDelegate += window.OnSceneGUI;

		window.loadedScenes.Clear ();
		for (int i = 0; i < EditorSceneManager.sceneCount; i++) {
			window.loadedScenes.Add (EditorSceneManager.GetSceneAt (i));
		}
		//window.loadedScenes.AddRange (EditorSceneManager.GetAllScenes ());

		//string[] guids = AssetDatabase.FindAssets ("localAreaSettingsAuto");

	}

	/// <summary>
	/// Raises the focus event.
	/// </summary>
	void OnFocus ()
	{
		SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
		SceneView.onSceneGUIDelegate += this.OnSceneGUI;
	}

	void OnEnable ()
	{
		string[] guids = AssetDatabase.FindAssets ("localAreaSettingsAuto");
		if (guids.Length == 1) {
			LoadSettings (AssetDatabase.GUIDToAssetPath (guids [0]).Replace ("Assets", ""));
		}
	}

	/// <summary>
	/// Raises the destroy event.
	/// </summary>
	void OnDestroy ()
	{
		string[] guids = AssetDatabase.FindAssets ("localAreaSettingsAuto");

		if (guids.Length == 1) {
			SaveSettings (AssetDatabase.GUIDToAssetPath (guids [0]));
		}

		SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
		closed = true;
		OnSceneGUI (null);
	}



	/// <summary>
	/// Raises the GUI event.
	/// </summary>
	void OnGUI ()
	{
		closed = false;
		if (!string.IsNullOrEmpty (warning)) {

			GUILayout.Space (20);
			var TextStyle = new GUIStyle ();
			TextStyle.normal.textColor = Color.red;
			TextStyle.alignment = TextAnchor.MiddleCenter;
			TextStyle.fontStyle = FontStyle.Bold;
			GUILayout.Label (warning, TextStyle);

		}

		GUILayout.Space (20);
		GUILayout.Label ("Settings", EditorStyles.boldLabel);
		GUILayout.Space (5);
		scrollPos = EditorGUILayout.BeginScrollView (scrollPos);

		EditorGUIUtility.wideMode = true;



		collectionsCollapsed = EditorGUILayout.Foldout (collectionsCollapsed, new GUIContent ("Scene collections: ", "List of scene collections from which you want to load content/objects. Remember about scene collection order. More info in manual."));
		if (collectionsCollapsed) {
			EditorGUI.indentLevel++;
			listSizeCollections = EditorGUILayout.IntField ("size", listSizeCollections);
			if (listSizeCollections != currentCollections.Count) {
				while (listSizeCollections > currentCollections.Count) {
					currentCollections.Add (null);
				}
				while (listSizeCollections < currentCollections.Count) {
					currentCollections.RemoveAt (currentCollections.Count - 1);
				}
			}

			for (int i = 0; i < currentCollections.Count; i++) {
				currentCollections [i] = (SceneCollection)EditorGUILayout.ObjectField (currentCollections [i], typeof(SceneCollection), true);
			}
			EditorGUI.indentLevel--;
		}
		GUILayout.Space (10);
		distanceFromCenter = EditorGUILayout.IntField (new GUIContent ("Loading distance", "Loaded world range in unity units or grid elements."), distanceFromCenter);
		if (distanceFromCenter < 0)
			distanceFromCenter = 0;
		
		EditorGUILayout.BeginHorizontal ();

		tiles = EditorGUILayout.Toggle (new GUIContent ("Tiles", "Set loaded world range to distance in grid elements."), tiles);
		tiles = !EditorGUILayout.Toggle (new GUIContent ("Units", "Set loaded world range to distance in unity units."), !tiles);

		EditorGUILayout.EndHorizontal ();

		GUILayout.Space (10);

		CenterPoint = EditorGUILayout.Vector3Field (new GUIContent ("Loading Center", "XYZ position in around which local area updater will load models/objects, in range that you specified above."), CenterPoint);

		EditorGUILayout.BeginHorizontal ();

		showLoadingPoint = EditorGUILayout.Toggle ("Show loading center", showLoadingPoint);

		if (GUILayout.Button ("Show center")) {

			if (SceneView.lastActiveSceneView != null) {
				SceneView.lastActiveSceneView.LookAt (CenterPoint);

	
			}

		}

		EditorGUILayout.EndHorizontal ();

		GUILayout.Space (10);
		GUILayout.Space (10);



		if (GUILayout.Button (new GUIContent ("Load Scenes from Collections around Center Point", "Will immediately load your scenes, from chosen collections around the point you chose and in range that you setup."))) {

			LoadScenesAroundCenterPoint ();

		}

		if (GUILayout.Button (new GUIContent ("Load All Scenes from Collections", "Will immediately load all  scenes  from  chosen collections."))) {

			LoadScenes ();

		}
		GUILayout.Space (20);
		GUILayout.Label ("Update", EditorStyles.boldLabel);
		GUILayout.Space (10);

		if (GUILayout.Button (new GUIContent ("Unsplit Scenes", "System will unpack your objects from virtual grid."))) {

			foreach (var sceneCollection in currentCollections) {
				splits.Add (new Dictionary<string, GameObject> ());
				UnSplitScene (sceneCollection);
			}

		}

		if (GUILayout.Button (new GUIContent ("Split Scenes", "Button will put new added or moved models to correct layers and grid elements."))) {

			UnityEngine.SceneManagement.Scene scene = EditorSceneManager.GetActiveScene ();

			if (!scene.isDirty || EditorUtility.DisplayDialog ("Save warning", "For spliting Your active scene must be saved!", "Save active scene now", "No") && EditorSceneManager.SaveScene (scene)) {
				
				splits = new List<Dictionary<string, GameObject>> ();

				foreach (var sceneCollection in currentCollections) {
					splits.Add (new Dictionary<string, GameObject> ());
					SplitScene (sceneCollection);
				}

				EditorSceneManager.MarkAllScenesDirty ();
				EditorSceneManager.SetActiveScene (scene);

			}
		}

		if (GUILayout.Button (new GUIContent ("Remove empty grid elements", "It removes empty virtual grid elements from scene collections."))) {

			RemoveEmptyScenes ();

		}

		GUILayout.Space (10);

		if (GUILayout.Button (new GUIContent ("Save loaded Scenes", "Button will immediately create new or actualize old scenes with changes that you made in grid elements. Before that you have to Click button \"Split scenes into virtual grid\". When you click save loaded scenes, unity will show you bugged unity window, simply click ok or enter. "))) {

			SaveScenes ();

		}

		if (GUILayout.Button (new GUIContent ("Unload loaded Scenes", "Will remove your loaded scenes and you will back you to clean empty scene."))) {

			UnloadScenes ();

		}

		GUILayout.Space (10);


		if (GUILayout.Button (new GUIContent ("Delete empty Scenes", "It removes scenes that not contains any grid element."))) {

			if (EditorUtility.DisplayDialog ("Delete warning", "Do You want do delete scene files from project folder. This operation can't be undone", "Yes, I want do delete scene files", "No"))
				RemoveEmptyScenes (true);

		}
		GUILayout.Space (10);
		GUILayout.Space (10);

		if (GUILayout.Button (new GUIContent ("Save settings", "Save Settings"))) {
			SaveSettings ();
		}
		if (GUILayout.Button (new GUIContent ("Load settings", "Loads Saved Settings"))) {
			LoadSettings ();
		}


		EditorGUILayout.EndScrollView ();
	}

	void SaveSettings (string pathToFile = "")
	{
		string path;
		if (pathToFile == "")
			path = EditorUtility.SaveFilePanelInProject ("Save settings", "localAreaSettings.asset", "asset",  
				"Please enter a file name to save the settings to");
		else
			path = pathToFile;
		
		if (path.Length != 0) {

			LocalAreaSettings asset = ScriptableObject.CreateInstance<LocalAreaSettings> ();
			asset.collectionsCollapsed = collectionsCollapsed;
			asset.listSizeCollections = listSizeCollections;
			asset.currentCollections = currentCollections;
			asset.showLoadingPoint = showLoadingPoint;
			asset.distanceFromCenter = distanceFromCenter;
			asset.tiles = tiles;
			asset.CenterPoint = CenterPoint;

			
			AssetDatabase.CreateAsset (asset, path);

			AssetDatabase.SaveAssets ();
			AssetDatabase.Refresh ();

		}


	}

	void LoadSettings (string pathToFile = "")
	{
		
		string path;
		if (pathToFile == "")
			path = EditorUtility.OpenFilePanel ("Open local area settings", "", "asset");
		else
			path = pathToFile;

		if (path.Length != 0) {
			path = "Assets" + path.Replace (Application.dataPath, "");

			LocalAreaSettings asset = AssetDatabase.LoadAssetAtPath<LocalAreaSettings> (path);
			if (asset != null) {
				collectionsCollapsed	= asset.collectionsCollapsed;
				listSizeCollections	= asset.listSizeCollections;
				currentCollections	= asset.currentCollections;
				showLoadingPoint = asset.showLoadingPoint;
				distanceFromCenter	= asset.distanceFromCenter;
				tiles	= asset.tiles;
				CenterPoint	= asset.CenterPoint;
			} else
				Debug.Log ("Wrong path: " + path);

		}
	}

	/// <summary>
	/// Raises the scene GUI event.
	/// </summary>
	/// <param name="sceneView">Scene view.</param>
	void OnSceneGUI (SceneView sceneView)
	{
		if (!closed) {
			Handles.color = new Color32 (147, 225, 58, 255);

			CenterPoint = Handles.PositionHandle (CenterPoint, Quaternion.identity);

			Handles.SphereHandleCap (0, CenterPoint, Quaternion.identity, 0.1f, EventType.Repaint);

			if (!tiles) {
				Handles.color = Color.white;
				Handles.SelectionFrame (0, CenterPoint - new Vector3 (0, 0, distanceFromCenter), Quaternion.Euler (0, 0, 0), distanceFromCenter);
				Handles.SelectionFrame (0, CenterPoint + new Vector3 (0, 0, distanceFromCenter), Quaternion.Euler (0, 0, 0), distanceFromCenter);
				Handles.SelectionFrame (0, CenterPoint - new Vector3 (distanceFromCenter, 0, 0), Quaternion.Euler (0, 90, 90), distanceFromCenter);
				Handles.SelectionFrame (0, CenterPoint + new Vector3 (distanceFromCenter, 0, 0), Quaternion.Euler (0, 90, 90), distanceFromCenter);
				Handles.SelectionFrame (0, CenterPoint, Quaternion.Euler (90, 0, 0), distanceFromCenter);
			}

		
			if (Vector3.Distance (CenterPoint, oldCenterPoint) > 0)
				Repaint ();
		
			oldCenterPoint = CenterPoint;
		
		} else
			Repaint ();
	}






	/// <summary>
	/// Loads the scenes around center point within range.
	/// </summary>
	void LoadScenesAroundCenterPoint ()
	{
		foreach (var item in currentCollections) {
			foreach (var sceneName in item.names) {
				
				int posX = 0;
				int posY = 0;
				int posZ = 0;

				Vector3 pos = CenterPoint;


				int xPosCurrent = (item.xSize != 0) ? (int)(Mathf.FloorToInt (pos.x / item.xSize)) : 0;
				int yPosCurrent = (item.ySize != 0) ? (int)(Mathf.FloorToInt (pos.y / item.ySize)) : 0;
				int zPosCurrent = (item.zSize != 0) ? (int)(Mathf.FloorToInt (pos.z / item.zSize)) : 0;

				Streamer.SceneNameToPos (item, sceneName, out posX, out posY, out posZ);

				if (tiles) {

					if (Mathf.Abs (posX - xPosCurrent) < distanceFromCenter
					    && Mathf.Abs (posY - yPosCurrent) < distanceFromCenter
					    && Mathf.Abs (posZ - zPosCurrent) < distanceFromCenter)
						loadedScenes.Add (EditorSceneManager.OpenScene (item.path + sceneName, OpenSceneMode.Additive));
				} else {
					if (Mathf.Abs (posX * item.xSize - pos.x) < distanceFromCenter
					    && Mathf.Abs (posY * item.ySize - pos.y) < distanceFromCenter
					    && Mathf.Abs (posZ * item.zSize - pos.z) < distanceFromCenter)
						loadedScenes.Add (EditorSceneManager.OpenScene (item.path + sceneName, OpenSceneMode.Additive));
				}

			}
		}

	}

	/// <summary>
	/// Loads all the scenes from collections.
	/// </summary>
	void LoadScenes ()
	{

		foreach (var item in currentCollections) {
			foreach (var sceneName in item.names) {
				loadedScenes.Add (EditorSceneManager.OpenScene (item.path + sceneName, OpenSceneMode.Additive));
			}
		}
	}

	/// <summary>
	/// Saves the scenes.
	/// </summary>
	void SaveScenes ()
	{
		foreach (var layer in currentCollections) {
		
			if (layer != null) {
		
				foreach (var item in loadedScenes) {
//				Debug.Log (item + " " + layer);
//				Debug.Log (item.name + " " + layer.prefixScene);
//				Debug.Log (item.name == null);

					if (item.name != null && item.name.StartsWith (layer.prefixScene) && item.isLoaded) {
						if (GameObject.Find (item.name) == null) {

							GameObject split = new GameObject (item.name);
							SceneSplitManager sceneSplitManager = split.AddComponent<SceneSplitManager> ();
							sceneSplitManager.sceneName = split.name;

							sceneSplitManager.size = new Vector3 (layer.xSize != 0 ? layer.xSize : 100, layer.ySize != 0 ? layer.ySize : 100, layer.zSize != 0 ? layer.zSize : 100);
							int posx;
							int posy;
							int posz;
							Streamer.SceneNameToPos (layer, item.name, out posx, out posy, out posz);
							posx *= layer.xSize;
							posy *= layer.ySize;
							posz *= layer.zSize;
							sceneSplitManager.position = GetSplitPosition (new Vector3 (posx, posy, posz), layer);
							sceneSplitManager.color = layer.color;
							EditorSceneManager.MoveGameObjectToScene (split, item);

						}
					}
				}
				EditorUtility.SetDirty (layer);
			}
		}
		
		AssetDatabase.SaveAssets ();
		EditorSceneManager.SaveModifiedScenesIfUserWantsTo (loadedScenes.ToArray ());

	}

	/// <summary>
	/// Removes or deletes the empty scenes.
	/// </summary>
	/// <param name="delete">If set to <c>true</c> will delete scene files.</param>
	void RemoveEmptyScenes (bool delete = false)
	{

		Debug.Log ("RemoveEmptyScenes " + delete);
		List<UnityEngine.SceneManagement.Scene> scenesToDelete = new List<UnityEngine.SceneManagement.Scene> ();
		foreach (var scene in loadedScenes) {
			if (scene.rootCount == 0) {
				scenesToDelete.Add (scene);
			}
		}

		foreach (var sceneCollection in currentCollections) {
			List<string> sceneList = new List<string> ();
			sceneList.AddRange (sceneCollection.names);
			foreach (var scene in scenesToDelete) {
				loadedScenes.Remove (scene);
				sceneList.Remove (scene.name + ".unity");
			}
			sceneCollection.names = sceneList.ToArray ();
		}
		if (!delete) {
			foreach (var scene in scenesToDelete) {
				EditorSceneManager.CloseScene (scene, true);
			}
		} else {
			List<string> scenesToDeletePaths = new List<string> ();
			foreach (var scene in scenesToDelete) {
				scenesToDeletePaths.Add (scene.path);
				EditorSceneManager.CloseScene (scene, true);

			}


			foreach (var scenePath in scenesToDeletePaths) {
				if (scenePath != null && scenePath.Contains (".unity")) {
					File.Delete (scenePath);
				}

			}
			scenesToDelete.Clear ();
		}
	}

	/// <summary>
	/// Unloads all scenes.
	/// </summary>
	void UnloadScenes ()
	{
		UnityEngine.SceneManagement.Scene scene = EditorSceneManager.GetActiveScene ();
		for (int i = 0; i < EditorSceneManager.sceneCount; i++) {
			
			Scene item = EditorSceneManager.GetSceneAt (i);
		
			if (item != scene)
				EditorSceneManager.CloseScene (item, true);
		}

	}

	/// <summary>
	/// Unsplits the scene from tiles.
	/// </summary>	
	void UnSplitScene (SceneCollection layer)
	{

		warning = "";
		splits [layer.layerNumber] = new Dictionary<string, GameObject> ();




		GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject> ();

		FindSceneGO (layer.prefixScene, allObjects, splits [layer.layerNumber]);

		ClearSceneGO (layer);

	}

	/// <summary>
	/// Splits the scene into tiles.
	/// </summary>	
	void SplitScene (SceneCollection layer)
	{
		warning = "";
		splits [layer.layerNumber] = new Dictionary<string, GameObject> ();


	


		GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject> ();

		FindSceneGO (layer.prefixScene, allObjects, splits [layer.layerNumber]);

		ClearSceneGO (layer);


		foreach (var item in allObjects) {
			if (item == null || item.transform.parent != null || !item.name.StartsWith (layer.prefixName)
			    || item.GetComponent<SceneSplitManager> () != null || item.GetComponent<SceneCollection> () != null || item.GetComponent<SceneSplitterSettings> () != null)
				continue;
			
			string splitName = layer.prefixScene + GetID (item.transform.position, layer);
		
			UnityEngine.SceneManagement.Scene scene = EditorSceneManager.GetSceneByName (splitName);//EditorSceneManager.NewScene (NewSceneSetup.EmptyScene, NewSceneMode.Additive);



			if (scene.name == null) {

				string splitSceneName = splitName + ".unity";
				string sceneName = layer.path + splitSceneName;

				bool contains = false;

				foreach (var nameSplit in layer.names) {
					if (nameSplit == splitSceneName) {
						contains = true;
						break;
					}
				}

				if (!contains) {
					List<string> names = new List<string> ();
					names.AddRange (layer.names);
					names.Add (splitSceneName);
					layer.names = names.ToArray ();

					scene = EditorSceneManager.NewScene (NewSceneSetup.EmptyScene, NewSceneMode.Additive);

					EditorSceneManager.SaveScene (scene, sceneName);
					loadedScenes.Add (scene);

				} else {
					scene = EditorSceneManager.OpenScene (sceneName, OpenSceneMode.Additive);
					loadedScenes.Add (scene);
				}



			}
		}

		allObjects = UnityEngine.Object.FindObjectsOfType<GameObject> ();

		FindSceneGO (layer.prefixScene, allObjects, splits [layer.layerNumber]);

		ClearSceneGO (layer);

		foreach (var item in allObjects) {
			if (item == null || item.transform.parent != null || !item.name.StartsWith (layer.prefixName)
			    || item.GetComponent<SceneSplitManager> () != null || item.GetComponent<SceneCollection> () != null || item.GetComponent<SceneSplitterSettings> () != null)
				continue;
			string itemId = GetID (item.transform.position, layer);

			GameObject split = null;
			if (!splits [layer.layerNumber].TryGetValue (itemId, out split)) {
				split = new GameObject (layer.prefixScene + itemId);
				SceneSplitManager sceneSplitManager = split.AddComponent<SceneSplitManager> ();
				sceneSplitManager.sceneName = split.name;

				sceneSplitManager.size = new Vector3 (layer.xSize != 0 ? layer.xSize : 100, layer.ySize != 0 ? layer.ySize : 100, layer.zSize != 0 ? layer.zSize : 100);
				sceneSplitManager.position = GetSplitPosition (item.transform.position, layer);
				sceneSplitManager.color = layer.color;
				
				splits [layer.layerNumber].Add (itemId, split);

				Vector3 splitPosId = GetSplitPositionID (item.transform.position, layer);

				if (layer.xSplitIs) {
					if (splitPosId.x < layer.xLimitsx) {
						layer.xLimitsx = (int)splitPosId.x;
					}
					if (splitPosId.x > layer.xLimitsy) {
						layer.xLimitsy = (int)splitPosId.x;
					}
				} else {
					layer.xLimitsx = 0;
					layer.xLimitsy = 0;
				}

				if (layer.ySplitIs) {
					if (splitPosId.y < layer.yLimitsx) {
						layer.yLimitsx = (int)splitPosId.y;
					}
					if (splitPosId.y > layer.yLimitsy) {
						layer.yLimitsy = (int)splitPosId.y;
					}
				} else {
					layer.yLimitsx = 0;
					layer.yLimitsy = 0;
				}

				if (layer.zSplitIs) {
					if (splitPosId.z < layer.zLimitsx) {
						layer.zLimitsx = (int)splitPosId.z;
					}
					if (splitPosId.z > layer.zLimitsy) {
						layer.zLimitsy = (int)splitPosId.z;
					}
				} else {
					layer.zLimitsx = 0;
					layer.zLimitsy = 0;
				}

				UnityEngine.SceneManagement.Scene scene = EditorSceneManager.GetSceneByName (sceneSplitManager.sceneName);


			

				EditorSceneManager.MoveGameObjectToScene (split, scene);

			}



			item.transform.SetParent (split.transform);
		}

		if (splits.Count == 0) {
			warning = "No objects to split. Check GameObject or Scene Prefix.";
		}

		foreach (var item in loadedScenes) {
			if (!string.IsNullOrEmpty (item.name) && GameObject.Find (item.name) == null) {

				GameObject split = new GameObject (item.name);
				SceneSplitManager sceneSplitManager = split.AddComponent<SceneSplitManager> ();
				sceneSplitManager.sceneName = split.name;

				sceneSplitManager.size = new Vector3 (layer.xSize != 0 ? layer.xSize : 100, layer.ySize != 0 ? layer.ySize : 100, layer.zSize != 0 ? layer.zSize : 100);
				int posx;
				int posy;
				int posz;
                
				Streamer.SceneNameToPos (layer, item.name, out posx, out posy, out posz);
                
				posx *= layer.xSize;
				posy *= layer.ySize;
				posz *= layer.zSize;
				sceneSplitManager.position = GetSplitPosition (new Vector3 (posx, posy, posz), layer);
				sceneSplitManager.color = layer.color;

				splits [layer.layerNumber].Add (split.name.Replace (layer.prefixScene, ""), split);
				EditorSceneManager.MoveGameObjectToScene (split, item);

			}
		}


	}

	/// <summary>
	/// Clears the scene Game objects.
	/// </summary>
	void ClearSceneGO (SceneCollection layer)
	{
		Scene scene =	SceneManager.GetActiveScene ();

	

		List<string> toRemove = new List<string> ();

		foreach (var item in splits [layer.layerNumber]) {
			if (item.Value.GetComponent<SceneSplitManager> ()) {
				Transform splitTrans = item.Value.transform;
				foreach (Transform splitChild in splitTrans) {
					splitChild.parent = null;
					SceneManager.MoveGameObjectToScene (splitChild.gameObject, scene);
				}

				while (splitTrans.childCount > 0) {

					foreach (Transform splitChild in splitTrans) {
						splitChild.parent = null;
						SceneManager.MoveGameObjectToScene (splitChild.gameObject, scene);
					}

				}
				GameObject.DestroyImmediate (splitTrans.gameObject);
				toRemove.Add (item.Key);
			}
		}
		foreach (var item in toRemove) {
			splits [layer.layerNumber].Remove (item);
		}
	}

	/// <summary>
	/// Finds the scene stream splits.
	/// </summary>
	/// <param name="allObjects">All objects in scene.</param>
	/// <param name="destroyOther">If set to <c>true</c> destroy other objects in scene.</param>
	void FindSceneGO (string prefixScene, GameObject[] allObjects, Dictionary<string, GameObject> splits, bool destroyOther = false)
	{
		foreach (var item in allObjects) {
			if (item == null)
				continue;

			if (item != null && (item.transform.parent != null || !item.name.StartsWith (prefixScene))) {
				if (destroyOther && item.transform.parent == null) {
					DestroyImmediate (item);
				}
				continue;
			}

			GameObject go;
			string sceneID = "";

			sceneID = item.name.Replace (prefixScene, "");
			if (!splits.TryGetValue (sceneID, out go))
				splits.Add (sceneID, item);
		}
	}

	/// <summary>
	/// Gets the ID of tile by position.
	/// </summary>
	/// <returns>The ID of tile.</returns>
	/// <param name="position">Position of tile.</param>
	/// <param name="position">Position of tile.</param>
	/// <param name="layer">Layer.</param>
	string GetID (Vector3 position, SceneCollection layer)
	{
		int xId = (int)(Mathf.FloorToInt (position.x / layer.xSize));

		if (Mathf.Abs ((position.x / layer.xSize) - Mathf.RoundToInt (position.x / layer.xSize)) < 0.001f) {
			xId = (int)Mathf.RoundToInt (position.x / layer.xSize);
		}


		int yId = (int)(Mathf.FloorToInt (position.y / layer.ySize));

		if (Mathf.Abs ((position.y / layer.ySize) - Mathf.RoundToInt (position.y / layer.ySize)) < 0.001f) {
			yId = (int)Mathf.RoundToInt (position.y / layer.ySize);
		}

		int zId = (int)(Mathf.FloorToInt (position.z / layer.zSize));

		if (Mathf.Abs ((position.z / layer.zSize) - Mathf.RoundToInt (position.z / layer.zSize)) < 0.001f) {
			zId = (int)Mathf.RoundToInt (position.z / layer.zSize);
		}


		return (layer.xSplitIs ? "_x" + xId : "") +
		(layer.ySplitIs ? "_y" + yId : "")
		+ (layer.zSplitIs ? "_z" + zId : "");
	}

	/// <summary>
	/// Gets the split position.
	/// </summary>
	/// <returns>The split position.</returns>
	/// <param name="position">Position of tile.</param>
	/// <param name="layer">Layer.</param>
	Vector3 GetSplitPosition (Vector3 position, SceneCollection layer)
	{
		int x = (int)(Mathf.FloorToInt (position.x / layer.xSize));

		if (Mathf.Abs ((position.x / layer.xSize) - Mathf.RoundToInt (position.x / layer.xSize)) < 0.001f) {
			x = (int)Mathf.RoundToInt (position.x / layer.xSize);
		}


		int y = (int)(Mathf.FloorToInt (position.y / layer.ySize));

		if (Mathf.Abs ((position.y / layer.ySize) - Mathf.RoundToInt (position.y / layer.ySize)) < 0.001f) {
			y = (int)Mathf.RoundToInt (position.y / layer.ySize);
		}

		int z = (int)(Mathf.FloorToInt (position.z / layer.zSize));

		if (Mathf.Abs ((position.z / layer.zSize) - Mathf.RoundToInt (position.z / layer.zSize)) < 0.001f) {
			z = (int)Mathf.RoundToInt (position.z / layer.zSize);
		}


		return new Vector3 (x * layer.xSize, y * layer.ySize, z * layer.zSize);
	}

	/// <summary>
	/// Gets the split position divided by size.
	/// </summary>
	/// <returns>The split position I.</returns>
	/// <param name="position">Position.</param>
	/// <param name="layer">Layer.</param>
	Vector3 GetSplitPositionID (Vector3 position, SceneCollection layer)
	{
		int x = (int)(Mathf.FloorToInt (position.x / layer.xSize));

		if (Mathf.Abs ((position.x / layer.xSize) - Mathf.RoundToInt (position.x / layer.xSize)) < 0.001f) {
			x = (int)Mathf.RoundToInt (position.x / layer.xSize);
		}


		int y = (int)(Mathf.FloorToInt (position.y / layer.ySize));

		if (Mathf.Abs ((position.y / layer.ySize) - Mathf.RoundToInt (position.y / layer.ySize)) < 0.001f) {
			y = (int)Mathf.RoundToInt (position.y / layer.ySize);
		}

		int z = (int)(Mathf.FloorToInt (position.z / layer.zSize));

		if (Mathf.Abs ((position.z / layer.zSize) - Mathf.RoundToInt (position.z / layer.zSize)) < 0.001f) {
			z = (int)Mathf.RoundToInt (position.z / layer.zSize);
		}


		return new Vector3 (x, y, z);
	}
	


}
