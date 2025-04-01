using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Collections;
using System.Linq;
using System.Reflection;

using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;


/// <summary>
/// Scene splitter editor.
/// </summary>
public class SceneSplitterEditor : EditorWindow
{

	public int toolbarInt = 0;
	public string[] toolbarStrings = new string[] { "Scene Generation", "Build Settings" };

	/// <summary>
	/// The splits of tiles.
	/// </summary>
	List<Dictionary<string,GameObject>> splits = new List<Dictionary<string,GameObject>> ();

	/// <summary>
	/// The scene splitter settings.
	/// </summary>
	SceneSplitterSettings sceneSplitterSettings;

	/// <summary>
	/// Warning info
	/// </summary>
	string warning = "";

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
	/// The colliders collapsed.
	/// </summary>
	bool collidersCollapsed = true;
	/// <summary>
	/// The list size collections.
	/// </summary>
	int listSizeColliders = 0;
	/// <summary>
	/// The current collections.
	/// </summary>
	List<ColliderScene> currentColliders = new List<ColliderScene> ();


	string colliderScenesPath = "ColliderScenes";


	/// <summary>
	/// The layers collapsed.
	/// </summary>
	bool layersCollapsed = true;
	/// <summary>
	/// The scene layers.
	/// </summary>
	List<SceneCollection> sceneLayers = new List<SceneCollection> ();
	/// <summary>
	/// The layers to remove.
	/// </summary>
	List<SceneCollection> layersToRemove = new List<SceneCollection> ();

	/// <summary>
	/// The current scene.
	/// </summary>
	private static string currentScene;
	/// <summary>
	/// The generation cancel.
	/// </summary>
	private bool cancel = false;

	/// <summary>
	/// The scroll position.
	/// </summary>
	private Vector2 scrollPos;

	/// <summary>
	///  Add menu named "Scene splitter" to the Window menu
	/// </summary>
	[MenuItem ("World Streamer/Scene splitter")]
	static void Init ()
	{
		// Get existing open window or if none, make a new one:
		SceneSplitterEditor window = EditorWindow.GetWindow <SceneSplitterEditor> ("Scene Splitter");
		window.Show ();

		currentScene = EditorSceneManager.GetActiveScene ().path;
		window.SceneChanged ();
	}



	/// <summary>
	/// Raises the GUI event.
	/// </summary>
	void OnGUI ()
	{

		if (currentScene != EditorSceneManager.GetActiveScene ().path) {
			SceneChanged ();
		}

		scrollPos = EditorGUILayout.BeginScrollView (scrollPos);

	
		GUILayout.Space (10);

		toolbarInt = GUILayout.Toolbar (toolbarInt, toolbarStrings);

		switch (toolbarInt) {
		case 0:
			 
			//GUILayout.Label ("Base Settings - " + currentScene, EditorStyles.boldLabel);

			GUILayout.Space (10);
			sceneSplitterSettings.scenesPath = EditorGUILayout.TextField ("Scene create folder", sceneSplitterSettings.scenesPath);



			SceneSettings ();

			break;
		case 1:
			BuildSettings ();
			break;
		default:
			break;
		}

	

		//		if (GUILayout.Button ("Rob - Generate Random Scene")) {
//			foreach (var layer in sceneLayers) {
//				GenerateRandomSceneObjects (layer);
//			}
//		}

		
		EditorGUILayout.EndScrollView ();

	}

	#region UIMethods

	/// <summary>
	/// UI for scene settings
	/// </summary>
	void SceneSettings ()
	{
		GUILayout.Space (20);

		
		GUILayout.Label ("Scene layers", EditorStyles.boldLabel);
		if (GUILayout.Button (new GUIContent ("Add layer", "Creates layer for objects group. Each layer generates separated virtual grid"))) {
			CreateLayer ();
		}
		layersCollapsed = EditorGUILayout.Foldout (layersCollapsed, "Layers: " + sceneLayers.Count);
		if (layersCollapsed) {
			
			EditorGUI.indentLevel++;
			SceneCollection layer = null;
			for (int layerNum = 0; layerNum < sceneLayers.Count; layerNum++) {
				layer = sceneLayers [layerNum];

				EditorGUILayout.BeginHorizontal ();
				layer.collapsed = EditorGUILayout.Foldout (layer.collapsed, "Layer: " + layer.prefixScene);
				if (GUILayout.Button (new GUIContent ("S", "Create virtual grid from this layer only. System will check objects prefix, pivot position and it will move objects into this layer and correct grid element. This will happen only if objects have prefix that match layer game object prefix."), GUILayout.Width (25))) {
					SplitScene (layer);
				}
				if (GUILayout.Button (new GUIContent ("G", "Ggenerate scenes from virtual grid for this layer only."), GUILayout.Width (25))) {

					EditorSceneManager.SaveScene (EditorSceneManager.GetActiveScene ());

					GenerateSceneCollidersForSplitScene ();
					GenerateScenesMulti (layer, 0, 1);

					listSizeCollections = currentCollections.Count;
					EditorUtility.ClearProgressBar ();
				}
				if (GUILayout.Button (new GUIContent ("C", "Remove virtual grid which is undo for \"S\" for this layer only."), GUILayout.Width (25))) {
					ClearSplitScene (layer);
				}
				if (GUILayout.Button (new GUIContent ("X", "Remove layer."), GUILayout.Width (25))) {
					DeleteLayer (layer);
					continue;
				}
				layer.color = EditorGUILayout.ColorField (new GUIContent ("", "Virtual grid gizmo color."), layer.color, GUILayout.Width (60));
				EditorGUILayout.EndHorizontal ();
				EditorGUI.indentLevel++;
				if (layer.collapsed) {
					layer.xSplitIs = EditorGUILayout.Toggle (new GUIContent ("Split x", "Use X axis in grid."), layer.xSplitIs);
					if (layer.xSplitIs) {
						layer.xSize = EditorGUILayout.IntField (new GUIContent ("Size X", "Grid element size in X axis."), layer.xSize);
					} else
						layer.xSize = 0;
					layer.ySplitIs = EditorGUILayout.Toggle (new GUIContent ("Split y", "Use Y axis in grid."), layer.ySplitIs);
					if (layer.ySplitIs) {
						layer.ySize = EditorGUILayout.IntField (new GUIContent ("Size Y", "Grid element size in Y axis."), layer.ySize);
					} else
						layer.ySize = 0;
					layer.zSplitIs = EditorGUILayout.Toggle (new GUIContent ("Split z", "Use Z axis in grid."), layer.zSplitIs);
					if (layer.zSplitIs) {
						layer.zSize = EditorGUILayout.IntField (new GUIContent ("Size Z", "Grid element size in Z axis."), layer.zSize);
					} else
						layer.zSize = 0;
					layer.prefixName = EditorGUILayout.TextField (new GUIContent ("GameObject Prefix", "First word in your object name, which means membership to this layer. Example:  StreamedT_Terrain where \"StreamedT \" is prefix, Terrain is object name. System will search for objects that have \"StreamedT\" at the beginning of the name. If you left this value as empty, system will  put all objects in the scene into this layer."), layer.prefixName);
					layer.prefixScene = EditorGUILayout.TextField (new GUIContent ("Scene Prefix", "Your future scenes and scene collection prefab names."), layer.prefixScene);
					layer.layerNumber = layerNum;
					GUILayout.Space (10);
				}
				EditorGUI.indentLevel--;
			}
			EditorGUI.indentLevel--;
		}
		foreach (var item in layersToRemove) {
			sceneLayers.Remove (item);
			DestroyImmediate (item.gameObject);
		}
		layersToRemove.Clear ();
		if (sceneSplitterSettings == null)
			CreateSettings ();
		
		GUILayout.Space (10);

		GUILayout.Label ("Splitting tiles", EditorStyles.boldLabel);

		if (GUILayout.Button (new GUIContent ("Split scene into virtual Grid", "Create virtual grids from all layers. System will check objects prefix, pivot position and it will move objects into correct layer and grid element."))) {
			foreach (var layer in sceneLayers) {
				SplitScene (layer);
			}
		}
		if (GUILayout.Button ("Clear Scene Split")) {
			foreach (var layer in sceneLayers) {
				ClearSplitScene (layer);
			}
		}

		GUILayout.Space (10);

		GUILayout.Label ("Scene Generation", EditorStyles.boldLabel);

		if (GUILayout.Button (new GUIContent ("Generate Scenes from virtual Grid", "System will generate scenes and \"Collider_Stream\" prefabs for all objects at your scene that contain \"ColliderScene\" script."))) {
			currentCollections.Clear ();
			EditorUtility.DisplayProgressBar ("Creating Scenes", "Preparing scene", 0);
			int currentLayerID = 0;
			EditorSceneManager.SaveScene (EditorSceneManager.GetActiveScene ());

			for (int i = 0; i < sceneLayers.Count; i++) {
				SceneCollection layer = sceneLayers [i];

				if (cancel)
					break;
				if (EditorUtility.DisplayCancelableProgressBar ("Preparing Scenes " + (currentLayerID + 1) + '/' + sceneLayers.Count, "Preparing scene " + layer.prefixScene, (currentLayerID / (float)sceneLayers.Count))) {
					cancel = true;
					break;
				}
				GenerateSceneCollidersForSplitScene ();
				GenerateScenesMulti (layer, currentLayerID, sceneLayers.Count);
			
				currentLayerID++;
			}
			if (cancel) {
				cancel = false;
			} else
				listSizeCollections = currentCollections.Count;
			EditorUtility.ClearProgressBar ();
		}

		GUILayout.Space (10);
		#if UNITY_5_3 || UNITY_5_3_OR_NEWER
		if (GUILayout.Button (new GUIContent ("Generates Scenes from colliders only", "System will generate scenes only from all objects that contains \"Collider_Stream\" script."))) {
			GenerateSceneColliders ();
		}
		#else
		if (GUILayout.Button (new GUIContent ("Generates Scenes from colliders only [UNITY 5.3+ NEEDED]", "UNITY 5.3+ NEEDED"))) {

		}
		#endif
		if (!string.IsNullOrEmpty (warning)) {

			GUILayout.Space (20);
			var TextStyle = new GUIStyle ();
			TextStyle.normal.textColor = Color.red;
			TextStyle.alignment = TextAnchor.MiddleCenter;
			TextStyle.fontStyle = FontStyle.Bold;
			GUILayout.Label (warning, TextStyle);

		}
	}

	/// <summary>
	/// UI for build settings
	/// </summary>
	void BuildSettings ()
	{
		GUILayout.Space (10);

		collectionsCollapsed = EditorGUILayout.Foldout (collectionsCollapsed, new GUIContent ("Scene collections: ", "Drag and drop here all scene collections which scenes you want to add/remove to/from build settings."));
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

		collidersCollapsed = EditorGUILayout.Foldout (collidersCollapsed, new GUIContent ("Collider Streamers: ", "Drag and drop here all collider streamer prefabs with scenes that you want to add/remove to/from build settings."));
		if (collidersCollapsed) {
			EditorGUI.indentLevel++;
			listSizeColliders = EditorGUILayout.IntField ("size", listSizeColliders);
			if (listSizeColliders != currentColliders.Count) {
				while (listSizeColliders > currentColliders.Count) {
					currentColliders.Add (null);
				}
				while (listSizeColliders < currentColliders.Count) {
					currentColliders.RemoveAt (currentColliders.Count - 1);
				}
			}
			for (int i = 0; i < currentColliders.Count; i++) {
				currentColliders [i] = (ColliderScene)EditorGUILayout.ObjectField (currentColliders [i], typeof(ColliderScene), true);
			}
			EditorGUI.indentLevel--;
		}
		GUILayout.Space (10);

		if (GUILayout.Button (new GUIContent ("Add scenes to build settings", "System will add all scenes from chosen scene collections in to build settings."))) {



			AddScenesToBuild ();


			string scenesPath = this.sceneSplitterSettings.scenesPath;

			if (!Directory.Exists (scenesPath + colliderScenesPath + "/")) {

				warning = "Scene colliders path doesn't exist - " + scenesPath + colliderScenesPath + "/";

				return;
			}



			foreach (var item in currentColliders) {
				AddScenesToBuildString (scenesPath + colliderScenesPath + "/" + item.name + ".unity");
			}
		}

		if (GUILayout.Button (new GUIContent ("Remove scenes from build settings", "System removes all scenes from build settings from chosen scene collections."))) {
			RemoveScenesFromBuild ();
			string scenesPath = this.sceneSplitterSettings.scenesPath;
			foreach (var item in currentColliders) {
				RemoveScenesString (scenesPath + colliderScenesPath + "/" + item.name + ".unity");
			}

		}
//		if (GUILayout.Button ("Find scene collections prefabs")) {
//			foreach (var layer in sceneLayers) {
//				FindCollection (layer);
//			}
//		}
	}

	#endregion

	/// <summary>
	/// Generates the scene colliders.
	/// </summary>
	void GenerateSceneColliders ()
	{
		#if UNITY_5_3 || UNITY_5_3_OR_NEWER
		string scenesPath = this.sceneSplitterSettings.scenesPath;
		if (!Directory.Exists (scenesPath)) {
			warning = "Scene create folder doesn't exist.";
			return;
		}
		scenesPath += colliderScenesPath + "/";
		if (!Directory.Exists (scenesPath)) {
			Directory.CreateDirectory (scenesPath);
		}
		ColliderScene[] colliderScenes = GameObject.FindObjectsOfType<ColliderScene> ();
		foreach (var colliderScene in colliderScenes) {
			Collider collider = colliderScene.gameObject.GetComponent<Collider> ();
			GameObject go = new GameObject (colliderScene.name + "_stream");
			UnityEditorInternal.ComponentUtility.CopyComponent (collider);
			UnityEditorInternal.ComponentUtility.PasteComponentAsNew (go);
			UnityEditorInternal.ComponentUtility.CopyComponent (colliderScene.transform);
			UnityEditorInternal.ComponentUtility.PasteComponentValues (go.transform);
			string sceneName = scenesPath + colliderScene.name + ".unity";
			ColliderStreamer colliderStreamer = go.AddComponent<ColliderStreamer> ();
			colliderStreamer.sceneName = colliderScene.name;
			colliderStreamer.scenePath = sceneName;
			colliderScene.sceneName = colliderScene.name;
			Object prefab = PrefabUtility.CreateEmptyPrefab (scenesPath + "Prefabs/" + go.name + ".prefab");
			PrefabUtility.ReplacePrefab (go, prefab, ReplacePrefabOptions.Default);
			PrefabUtility.DisconnectPrefabInstance (go);
			GameObject.DestroyImmediate (go);
			Resources.UnloadAsset (prefab);
			Scene scene = EditorSceneManager.NewScene (NewSceneSetup.EmptyScene, NewSceneMode.Additive);
			DestroyImmediate (collider);
			SceneManager.MoveGameObjectToScene (colliderScene.gameObject, scene);
			EditorSceneManager.SaveScene (scene, sceneName);
		}
		AssetDatabase.SaveAssets ();
		AssetDatabase.Refresh ();
		Resources.UnloadUnusedAssets ();
		EditorSceneManager.LoadScene (currentScene);
		#endif
	}

	/// <summary>
	/// Generates the scene colliders for split scene.
	/// </summary>
	void GenerateSceneCollidersForSplitScene ()
	{
		#if UNITY_5_3 || UNITY_5_3_OR_NEWER
		string scenesPath = this.sceneSplitterSettings.scenesPath;
		if (!Directory.Exists (scenesPath)) {
			warning = "Scene create folder doesn't exist.";
			return;
		}
		scenesPath += colliderScenesPath + "/";
		if (!Directory.Exists (scenesPath)) {
			Directory.CreateDirectory (scenesPath);
		}
		ColliderScene[] colliderScenes = GameObject.FindObjectsOfType<ColliderScene> ();
		foreach (var colliderScene in colliderScenes) {
			Collider collider = colliderScene.gameObject.GetComponent<Collider> ();
			GameObject go = new GameObject (colliderScene.name + "_stream");
			UnityEditorInternal.ComponentUtility.CopyComponent (collider);
			UnityEditorInternal.ComponentUtility.PasteComponentAsNew (go);
			UnityEditorInternal.ComponentUtility.CopyComponent (colliderScene.transform);
			UnityEditorInternal.ComponentUtility.PasteComponentValues (go.transform);
			string sceneName = scenesPath + colliderScene.name + ".unity";
			ColliderStreamer colliderStreamer = go.AddComponent<ColliderStreamer> ();
			colliderStreamer.sceneName = colliderScene.name;
			colliderStreamer.scenePath = sceneName;
			colliderScene.sceneName = colliderScene.name;
			Object prefab = PrefabUtility.CreateEmptyPrefab (scenesPath + "Prefabs/" + go.name + ".prefab");
			PrefabUtility.ReplacePrefab (go, prefab, ReplacePrefabOptions.Default);

			//PrefabUtility.DisconnectPrefabInstance (go);
			//GameObject.DestroyImmediate (go);
			//Resources.UnloadAsset (prefab);

			go.transform.parent = colliderScene.transform.parent;
			go.transform.position = colliderScene.transform.position;

			Scene scene = EditorSceneManager.NewScene (NewSceneSetup.EmptyScene, NewSceneMode.Additive);
			DestroyImmediate (collider);

			colliderScene.gameObject.transform.parent = null;

			SceneManager.MoveGameObjectToScene (colliderScene.gameObject, scene);
			EditorSceneManager.SaveScene (scene, sceneName);
			EditorSceneManager.CloseScene (scene, true);
		}
		//AssetDatabase.SaveAssets ();
		//AssetDatabase.Refresh ();
		#endif
	}

	/// <summary>
	/// Scenes the changed.
	/// </summary>
	void SceneChanged ()
	{
		currentScene = EditorSceneManager.GetActiveScene ().path;

		sceneSplitterSettings = FindObjectOfType (typeof(SceneSplitterSettings)) as SceneSplitterSettings;

		if (sceneSplitterSettings == null)
			CreateSettings ();


		SceneCollection[] sceneCollections = FindObjectsOfType (typeof(SceneCollection)) as SceneCollection[];
		currentCollections.Clear ();
		listSizeCollections = 0;
		sceneLayers.Clear ();
		if (sceneCollections.Length > 0) {
			if (sceneLayers == null)
				sceneLayers = new List<SceneCollection> ();
			sceneLayers.AddRange (sceneCollections);



			splits = new List<Dictionary<string,GameObject>> ();
			foreach (var item in sceneLayers) {
			
				if (item.transform.parent != sceneSplitterSettings.transform)
					item.transform.parent = sceneSplitterSettings.transform;
				splits.Add (new Dictionary<string, GameObject> ());
			}
		}
		foreach (var layer in sceneLayers) {
			FindCollection (layer);
		}

		currentColliders.Clear ();
		currentColliders.AddRange (FindObjectsOfType (typeof(ColliderScene)) as ColliderScene[]);
		listSizeColliders = currentColliders.Count;

	}

	/// <summary>
	/// Creates the settings gameObject.
	/// </summary>
	void CreateSettings ()
	{
		
		sceneSplitterSettings = FindObjectOfType (typeof(SceneSplitterSettings)) as SceneSplitterSettings;
		
		if (sceneSplitterSettings == null) {

			GameObject gameObject = new GameObject ("_SceneSplitterSettings");
			sceneSplitterSettings = gameObject.AddComponent<SceneSplitterSettings> (); 

		}
	}

	/// <summary>
	/// Deletes the layer.
	/// </summary>
	/// <param name="layer">Layer.</param>
	void DeleteLayer (SceneCollection layer)
	{
		layersToRemove.Add (layer);
		splits.RemoveAt (0);
	}

	/// <summary>
	/// Creates the layer.
	/// </summary>
	void CreateLayer ()
	{
		GameObject sceneCollectionGO = new GameObject ("SC_" + sceneLayers.Count);
		
		sceneCollectionGO.transform.parent = sceneSplitterSettings.transform;

		SceneCollection newSceneCollection = sceneCollectionGO.AddComponent<SceneCollection> ();
		newSceneCollection.color = new Color (Random.value, Random.value, Random.value, 255);
		sceneLayers.Add (newSceneCollection);
		splits.Add (new Dictionary<string, GameObject> ());
	}

	/// <summary>
	/// Adds the scenes to build.
	/// </summary>
	void AddScenesToBuild ()
	{
		warning = "";
		List<EditorBuildSettingsScene> scenesList = new List<EditorBuildSettingsScene> ();
		scenesList.AddRange (EditorBuildSettings.scenes);
		foreach (var currentCollection in currentCollections) {

			if (!Directory.Exists (currentCollection.path)) {

				warning = "Scene collection path doesn't exist - " + currentCollection.name;

				return;
			}
			
		
                            
			List<string> scenesToAdd = new List<string> ();
			scenesToAdd.AddRange (currentCollection.names);

			foreach (var item in scenesList) {
				if (scenesToAdd.Contains (item.path.Replace (currentCollection.path, ""))) {
					scenesToAdd.Remove (item.path.Replace (currentCollection.path, ""));
				}
			}

			foreach (var item in scenesToAdd) {
				scenesList.Add (new EditorBuildSettingsScene (currentCollection.path + item, true));
			}
		}
		EditorBuildSettings.scenes = scenesList.ToArray ();



	}

	/// <summary>
	/// Adds the scenes to build.
	/// </summary>
	/// <param name="scenePath">Scene path.</param>
	void AddScenesToBuildString (string scenePath)
	{

		List<EditorBuildSettingsScene> scenesList = new List<EditorBuildSettingsScene> ();
		scenesList.AddRange (EditorBuildSettings.scenes);

		List<string> scenesToAdd = new List<string> ();
		scenesToAdd.Add (scenePath);

		foreach (var item in scenesList) {
			if (scenesToAdd.Contains (item.path)) {
				scenesToAdd.Remove (item.path);
			}
		}

		foreach (var item in scenesToAdd) {
			scenesList.Add (new EditorBuildSettingsScene (item, true));
		}

		EditorBuildSettings.scenes = scenesList.ToArray ();
	}

	void RemoveScenesString (string scenePath)
	{

		warning = "";
		List<EditorBuildSettingsScene> scenesList = new List<EditorBuildSettingsScene> ();
		scenesList.AddRange (EditorBuildSettings.scenes);




		List<string> scenesToAdd = new List<string> ();
		scenesToAdd.Add (scenePath);

		List<EditorBuildSettingsScene> newScenesList = new List<EditorBuildSettingsScene> ();
		foreach (var item in scenesList) {
			if (scenesToAdd.Contains (item.path)) {
				newScenesList.Add (item);
			}
		}
		foreach (var removeScene in newScenesList) {
			scenesList.Remove (removeScene);
		}



		EditorBuildSettings.scenes = scenesList.ToArray ();
	}

	/// <summary>
	/// Removes the scenes from build.
	/// </summary>
	void RemoveScenesFromBuild ()
	{
		warning = "";
		List<EditorBuildSettingsScene> scenesList = new List<EditorBuildSettingsScene> ();
		scenesList.AddRange (EditorBuildSettings.scenes);



		foreach (var currentCollection in currentCollections) {

			List<string> scenesToAdd = new List<string> ();
			scenesToAdd.AddRange (currentCollection.names);

			List<EditorBuildSettingsScene> newScenesList = new List<EditorBuildSettingsScene> ();
			foreach (var item in scenesList) {
				if (scenesToAdd.Contains (item.path.Replace (currentCollection.path, ""))) {
					newScenesList.Add (item);
				}
			}
			foreach (var removeScene in newScenesList) {
				scenesList.Remove (removeScene);
			}

		}
        
		EditorBuildSettings.scenes = scenesList.ToArray ();
	}

	/// <summary>
	/// Generates the random scene objects.
	/// </summary>
	void GenerateRandomSceneObjects (SceneCollection layer)
	{
		warning = "";
		for (int i = 0; i < 100; i++) {
			GameObject cube = GameObject.CreatePrimitive (PrimitiveType.Cube);
			cube.transform.position = Random.insideUnitSphere * 100;
			cube.name = layer.prefixName + "_" + i;
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
	/// Splits the scene into tiles.
	/// </summary>	
	void SplitScene (SceneCollection layer)
	{
		warning = "";
		splits [layer.layerNumber] = new Dictionary<string, GameObject> ();


		layer.xLimitsx = int.MaxValue;
		layer.xLimitsy = int.MinValue;
		layer.yLimitsx = int.MaxValue;
		layer.yLimitsy = int.MinValue;
		layer.zLimitsx = int.MaxValue;
		layer.zLimitsy = int.MinValue;


		GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject> ();

		FindSceneGO (layer.prefixScene, allObjects, splits [layer.layerNumber]);
		
		ClearSceneGO (layer);

		foreach (var item in allObjects) {
			if (item == null)
				continue;
			
			if (item.GetComponent<SceneSplitManager> () != null || item.GetComponent<SceneCollection> () != null || item.GetComponent<SceneSplitterSettings> () != null
			    || item.GetComponent<ObjectsParent> () != null)
				continue;

			if (!item.name.StartsWith (layer.prefixName))
				continue;
						
			if (item.transform.parent != null && item.transform.parent.GetComponent<ObjectsParent> () == null)
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
			}

						
			item.transform.SetParent (split.transform);
		}

		if (splits.Count == 0) {
			warning = "No objects to split. Check GameObject or Scene Prefix.";
		}

	}

	/// <summary>
	/// Finds the scene stream splits.
	/// </summary>
	/// <param name="allObjects">All objects in scene.</param>
	void FindSceneGO (string prefixScene, GameObject[] allObjects, Dictionary<string, GameObject> splits)
	{
		foreach (var item in allObjects) {
			
			if (item == null)
				continue;

			if (item.GetComponent<SceneSplitManager> () == null) {
				continue;
			}

			if (item.transform.parent != null || !item.name.StartsWith (prefixScene)) {
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
	/// Clears the split scene.
	/// </summary>
	void ClearSplitScene (SceneCollection layer)
	{
		warning = "";
		splits [layer.layerNumber] = new Dictionary<string, GameObject> ();
		
		GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject> ();
		
		FindSceneGO (layer.prefixScene, allObjects, splits [layer.layerNumber]);
		ClearSceneGO (layer);
	}

	/// <summary>
	/// Clears the scene Game objects.
	/// </summary>
	void ClearSceneGO (SceneCollection layer)
	{
		ObjectsParent[] objectsParents = FindObjectsOfType<ObjectsParent> ();
		ObjectsParent objectsParent = null;

		foreach (var parent in objectsParents) {
			if (parent.gameObjectPrefix == layer.prefixName) {
				objectsParent = parent;
				break;
			}
		}

		List<string> toRemove = new List<string> ();

		foreach (var item in splits [layer.layerNumber]) {
			if (item.Value.GetComponent<SceneSplitManager> ()) {
				
				
				Transform splitTrans = item.Value.transform;
				foreach (Transform splitChild in splitTrans) {
					
					if (objectsParent != null && (splitChild.name.StartsWith (objectsParent.gameObjectPrefix) || string.IsNullOrEmpty (objectsParent.gameObjectPrefix)))
						splitChild.SetParent (objectsParent.transform, true);
					else
						splitChild.parent = null;
				}

				while (splitTrans.childCount > 0) {

					foreach (Transform splitChild in splitTrans) {
						
						if (objectsParent != null && (splitChild.name.StartsWith (objectsParent.gameObjectPrefix) || string.IsNullOrEmpty (objectsParent.gameObjectPrefix)))
							splitChild.SetParent (objectsParent.transform, true);
						else
							splitChild.parent = null;
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
	/// Generates scenes from splits with multi scene.
	/// </summary>
	void GenerateScenesMulti (SceneCollection layer, int currentLayerID, int layersCount)
	{
		if (cancel)
			return;
		warning = "";

	

		string scenesPath = this.sceneSplitterSettings.scenesPath;
		if (!Directory.Exists (scenesPath)) {

			warning = "Scene create folder doesn't exist.";

			return;
		}

		scenesPath += layer.prefixScene + "/";
		if (!Directory.Exists (scenesPath)) {
			Directory.CreateDirectory (scenesPath);
		}

		List<string> sceneNames = new List<string> ();

		//EditorApplication.SaveScene ();

		Dictionary<string, GameObject> mainSplits = new Dictionary<string, GameObject> ();

		GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject> ();

		FindSceneGO (layer.prefixScene, allObjects, mainSplits);

		string currentScene = EditorSceneManager.GetActiveScene ().path;
		//Debug.Log (currentScene);
		Dictionary<string,string> scenes = new Dictionary<string,string> ();


		List<string> splitsNames = new List<string> ();
		foreach (var split in mainSplits) {
			splitsNames.Add (split.Value.name);

		}

		if (splits.Count == 0) {

			warning = "No objects to build scenes.";
			return;
		}

		int i = 0;
		foreach (var split in splitsNames) {
			if (cancel)
				return;
			sceneNames.Add (split + ".unity");
			string sceneName = scenesPath + split + ".unity";

			Scene scene = EditorSceneManager.NewScene (NewSceneSetup.EmptyScene, NewSceneMode.Additive);

			SceneManager.MoveGameObjectToScene (GameObject.Find (split), scene);

			EditorSceneManager.SaveScene (scene, sceneName);
				
			scenes.Add (split, sceneName);

			if (EditorUtility.DisplayCancelableProgressBar ("Creating Scenes " + (currentLayerID + 1) + '/' + layersCount + " (" + layer.prefixScene + ")", "Creating scene " + Path.GetFileNameWithoutExtension (EditorSceneManager.GetActiveScene ().name) + " " + i + " from " + splitsNames.Count, (currentLayerID + (i / (float)splitsNames.Count)) / (float)layersCount)) {
				cancel = true;
				EditorUtility.ClearProgressBar ();
				return;
			}
			i++;
		}
		

		EditorSceneManager.OpenScene (currentScene, OpenSceneMode.Single);

		SceneCollection sceneCollection;
		GameObject createdCollectionGO;
		if (AssetDatabase.LoadAssetAtPath (scenesPath + "SC_" + layer.prefixScene + ".prefab", typeof(GameObject))) {

			GameObject prefab = (GameObject)AssetDatabase.LoadAssetAtPath (scenesPath + "SC_" + layer.prefixScene + ".prefab", typeof(GameObject));

			sceneCollection = prefab.GetComponent<SceneCollection> ();
			sceneCollection.path = scenesPath;
			sceneCollection.names = sceneNames.ToArray ();
			sceneCollection.prefixName = layer.prefixName;
			sceneCollection.prefixScene = layer.prefixScene;
			sceneCollection.xSplitIs = layer.xSplitIs;
			sceneCollection.ySplitIs = layer.ySplitIs;
			sceneCollection.zSplitIs = layer.zSplitIs;
			sceneCollection.xSize = layer.xSize;
			sceneCollection.ySize = layer.ySize;
			sceneCollection.zSize = layer.zSize;
			sceneCollection.xLimitsx = layer.xLimitsx;
			sceneCollection.xLimitsy = layer.xLimitsy;
			sceneCollection.yLimitsx = layer.yLimitsx;
			sceneCollection.yLimitsy = layer.yLimitsy;
			sceneCollection.zLimitsx = layer.zLimitsx;
			sceneCollection.zLimitsy = layer.zLimitsy;
			sceneCollection.color = layer.color;

			createdCollectionGO = prefab;
			if (!currentCollections.Contains (sceneCollection)) {
				currentCollections.Add (createdCollectionGO.GetComponent<SceneCollection> ());


			}
			EditorUtility.SetDirty (prefab);
			AssetDatabase.SaveAssets ();

		} else {

			GameObject sceneCollectionGO = new GameObject ("SC_" + layer.prefixScene);

			sceneCollection = sceneCollectionGO.AddComponent<SceneCollection> ();
			sceneCollection.path = scenesPath;
			sceneNames.Sort ();
			sceneCollection.names = sceneNames.ToArray ();
			sceneCollection.prefixName = layer.prefixName;
			sceneCollection.prefixScene = layer.prefixScene;
			sceneCollection.xSplitIs = layer.xSplitIs;
			sceneCollection.ySplitIs = layer.ySplitIs;
			sceneCollection.zSplitIs = layer.zSplitIs;
			sceneCollection.xSize = layer.xSize;
			sceneCollection.ySize = layer.ySize;
			sceneCollection.zSize = layer.zSize;
			sceneCollection.xLimitsx = layer.xLimitsx;
			sceneCollection.xLimitsy = layer.xLimitsy;
			sceneCollection.yLimitsx = layer.yLimitsx;
			sceneCollection.yLimitsy = layer.yLimitsy;
			sceneCollection.zLimitsx = layer.zLimitsx;
			sceneCollection.zLimitsy = layer.zLimitsy;
			sceneCollection.color = layer.color;

			createdCollectionGO = PrefabUtility.CreatePrefab (scenesPath + sceneCollectionGO.name + ".prefab", sceneCollectionGO);
			currentCollections.Add (createdCollectionGO.GetComponent<SceneCollection> ());
			GameObject.DestroyImmediate (sceneCollection.gameObject);
		}

		AssetDatabase.SaveAssets ();
		Resources.UnloadUnusedAssets ();
	}


	/// <summary>
	/// Finds the collections.
	/// </summary>
	public void FindCollection (SceneCollection layer)
	{
		string scenesPath = this.sceneSplitterSettings.scenesPath + layer.prefixScene + "/";
		GameObject prefab = (GameObject)AssetDatabase.LoadAssetAtPath (scenesPath + "SC_" + layer.prefixScene + ".prefab", typeof(GameObject));

		if (prefab != null) {
			SceneCollection sceneCollection = prefab.GetComponent<SceneCollection> ();
			if (!currentCollections.Contains (sceneCollection))
				currentCollections.Add (sceneCollection);
			listSizeCollections = currentCollections.Count;
		}
		
	}

	//	/// <summary>
	//	/// Gets all disabled scene objects.
	//	/// </summary>
	//	/// <returns>The all disabled scene objects.</returns>
	//	public static Transform[] GetAllDisabledSceneObjects ()
	//	{
	//		var allTransforms = GameObject.FindObjectsOfTypeAll (typeof(Transform));
	//		Debug.Log (allTransforms.Length);
	//		Debug.Log (Resources.FindObjectsOfTypeAll (typeof(Transform)).Length);
	//
	//		var previousSelection = Selection.objects;
	//
	//		Selection.objects = allTransforms.Cast<Transform> ()
	//			.Where (x => x != null && x.parent == null)
	//				.Select (x => x.gameObject)
	//				.Where (x => x != null && !x.activeSelf)
	//				.Cast<UnityEngine.Object> ().ToArray ();
	//
	//
	//		var selectedTransforms = Selection.GetTransforms (SelectionMode.TopLevel | SelectionMode.Editable | SelectionMode.ExcludePrefab);
	//
	//		Selection.objects = previousSelection;
	//
	//		return selectedTransforms;
	//	}
}