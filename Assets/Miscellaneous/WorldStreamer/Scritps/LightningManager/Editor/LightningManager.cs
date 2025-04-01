using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;
using UnityEditor.SceneManagement;

/// <summary>
/// Lightning manager for scene collections.
/// </summary>
public class LightningManager : EditorWindow
{
#if UNITY_5_5_OR_NEWER
	//Enviroment
	Material skybox;
	Light sun;
	AmbientMode ambientMode;
	float ambientInstensity = 1;

	Color ambientSkyColor = new Color (0.212f, 0.227f, 0.259f);
	Color ambientEquatorColor = Color.grey;
	Color ambientGroundColor = Color.black;

	DefaultReflectionMode reflectionSource;
	int defaultReflectionResolution = 128;
	string[] resolutionValuesText = new string[] {
		"128", "256", "512", "1024"
	};
	int[] resolutionValues = new int[] {
		128, 256, 512, 1024
	};
	float reflectionInstensity = 1;
	int reflectionBounces;
	Cubemap reflectionCubemap;
	ReflectionCubemapCompression reflectionCubemapCompression = ReflectionCubemapCompression.Auto;

	//RealtimeGI
	bool realtimeGI = true;
	float resolutionRealtime = 2;

	//BakedGI
	bool bakedGI = true;
	int bakedResolution = 40;
	int bakedPadding = 2;
	bool bakedCompression = true;
	int atlasSize = 1024;
	string[] atlasSizeValuesText = new string[] {
		"32", "64", "128", "256", "512", "1024", "2048", "4096"
	};
	int[] atlasSizeValues = new int[] {
		32, 64, 128, 256, 512, 1024, 2048, 4096
	};

	//General
	float indirectIntensity = 1;
	float bounceBoost = 1;

	//Fog
	bool fog;
	Color fogColor = Color.gray;
	FogMode fogMode = FogMode.ExponentialSquared;
	float fogDensity = 0.01f;

	//Editor privates
	bool enviromentFoldout = true;
	bool realtimeFoldout = true;
	bool bakedFoldout = true;
	bool generalFoldout = true;
	bool fogFoldout = true;

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

	[MenuItem ("World Streamer/Lightning Manager")]
	static void Init ()
	{
		// Get existing open window or if none, make a new one:
		LightningManager window = EditorWindow.GetWindow <LightningManager> ("Lightning Manager");
		window.Show ();

	}


	/// <summary>
	/// Raises the GUI event.
	/// </summary>
	void OnGUI ()
	{

		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		GUILayout.Box ("", GUILayout.ExpandWidth (true), GUILayout.Height (1));

		//Enviroment parameters
		EditorGUILayout.BeginHorizontal ();
		enviromentFoldout = EditorGUILayout.Foldout (enviromentFoldout, "");
		EditorGUILayout.LabelField ("Enviroment lightning", EditorStyles.boldLabel);
		EditorGUILayout.EndHorizontal ();

		if (enviromentFoldout) {
			EditorGUI.indentLevel++;
			skybox = (Material)EditorGUILayout.ObjectField ("Skybox", skybox, typeof(Material), false);
			sun = (Light)EditorGUILayout.ObjectField ("Sun", sun, typeof(Light), true);


			EditorGUILayout.Space ();
			ambientMode = (AmbientMode)EditorGUILayout.EnumPopup ("Ambient Source", ambientMode);

			if (skybox != null && ambientMode == AmbientMode.Skybox) {
				EditorGUI.indentLevel++;
				ambientInstensity = EditorGUILayout.Slider ("Ambient Intensity", ambientInstensity, 0, 8);
				EditorGUI.indentLevel--;
			}
			if (ambientMode == AmbientMode.Flat) {
				EditorGUI.indentLevel++;
				ambientEquatorColor = EditorGUILayout.ColorField ("Ambient Color", ambientEquatorColor);
				EditorGUI.indentLevel--;

			}
			if (ambientMode == AmbientMode.Trilight) {
				EditorGUI.indentLevel++;
				ambientSkyColor = EditorGUILayout.ColorField ("Sky Color", ambientSkyColor);
				ambientEquatorColor = EditorGUILayout.ColorField ("Equator Color", ambientEquatorColor);
				ambientGroundColor = EditorGUILayout.ColorField ("Ground Color", ambientGroundColor);
				EditorGUI.indentLevel--;

			}

			EditorGUILayout.Space ();
		
			reflectionSource = (DefaultReflectionMode)EditorGUILayout.EnumPopup ("Reflection Source", reflectionSource);
			EditorGUI.indentLevel++;
			if (reflectionSource == DefaultReflectionMode.Skybox)
				defaultReflectionResolution = EditorGUILayout.IntPopup ("Resolution", defaultReflectionResolution, resolutionValuesText, resolutionValues);
			else {
				reflectionCubemap = (Cubemap)EditorGUILayout.ObjectField ("Cubemap", reflectionCubemap, typeof(Cubemap), true);
			}
			reflectionCubemapCompression = (ReflectionCubemapCompression)EditorGUILayout.EnumPopup ("Compression", reflectionCubemapCompression);
			EditorGUI.indentLevel--;
			reflectionInstensity = EditorGUILayout.Slider ("Reflection Instensity", reflectionInstensity, 0, 1);
			reflectionBounces = EditorGUILayout.IntSlider ("Reflection Bounces", reflectionBounces, 1, 5);
		}

		//Realtime parameters		
		GUILayout.Box ("", GUILayout.ExpandWidth (true), GUILayout.Height (1));
		EditorGUILayout.BeginHorizontal ();
		realtimeFoldout = EditorGUILayout.Foldout (realtimeFoldout, "");
		realtimeGI = EditorGUILayout.ToggleLeft ("Precomputed Realtime GI", realtimeGI, EditorStyles.boldLabel);
		EditorGUILayout.EndHorizontal ();


		if (realtimeFoldout) {
			EditorGUI.BeginDisabledGroup (!realtimeGI);
			EditorGUI.indentLevel++;
			resolutionRealtime = EditorGUILayout.FloatField ("Realtime Resolution", resolutionRealtime);
			EditorGUI.indentLevel--;
			EditorGUI.EndDisabledGroup ();
		}

		//Baked parameters
		GUILayout.Box ("", GUILayout.ExpandWidth (true), GUILayout.Height (1));
		EditorGUILayout.BeginHorizontal ();
		bakedFoldout = EditorGUILayout.Foldout (bakedFoldout, "");
		bakedGI = EditorGUILayout.ToggleLeft ("Baked GI", bakedGI, EditorStyles.boldLabel);
		 
		EditorGUILayout.EndHorizontal ();


		if (bakedFoldout) {
			EditorGUI.BeginDisabledGroup (!bakedGI);
			EditorGUI.indentLevel++;
			bakedResolution = EditorGUILayout.IntField ("Baked Resolution", bakedResolution);
			bakedPadding = EditorGUILayout.IntField ("Baked Padding", bakedPadding);
			bakedCompression = EditorGUILayout.Toggle ("Compressed", bakedCompression);
			atlasSize = EditorGUILayout.IntPopup ("Atlas Size", atlasSize, atlasSizeValuesText, atlasSizeValues);

			if (!realtimeGI) {

				EditorGUILayout.Space ();
				resolutionRealtime = EditorGUILayout.FloatField ("Indirect Resolution", resolutionRealtime);
				EditorGUILayout.Space ();
			}

			EditorGUI.indentLevel--;

			EditorGUI.EndDisabledGroup ();
		}

		//GeneralGI parameters
		GUILayout.Box ("", GUILayout.ExpandWidth (true), GUILayout.Height (1));
		EditorGUILayout.BeginHorizontal ();
		generalFoldout = EditorGUILayout.Foldout (generalFoldout, "");
		EditorGUILayout.LabelField ("General GI", EditorStyles.boldLabel);
		EditorGUILayout.EndHorizontal ();

		if (generalFoldout) {
			EditorGUI.indentLevel++;
			indirectIntensity = EditorGUILayout.Slider ("Indirect Instensity", indirectIntensity, 0, 5);
			bounceBoost = EditorGUILayout.Slider ("Bounce Boost", bounceBoost, 1, 10);

			EditorGUI.indentLevel--;
		}

		//Fog parameters
		GUILayout.Box ("", GUILayout.ExpandWidth (true), GUILayout.Height (1));
		EditorGUILayout.BeginHorizontal ();
		fogFoldout = EditorGUILayout.Foldout (fogFoldout, "");
		fog = EditorGUILayout.ToggleLeft ("Fog", fog, EditorStyles.boldLabel);
		EditorGUILayout.EndHorizontal ();


		if (fogFoldout) {
			EditorGUI.indentLevel++;
			EditorGUI.BeginDisabledGroup (!fog);
			fogColor = EditorGUILayout.ColorField ("Fog color", fogColor);
			fogMode = (FogMode)EditorGUILayout.EnumPopup ("Fog Mode", fogMode);
			EditorGUI.indentLevel++;
			fogDensity = EditorGUILayout.FloatField ("Density", fogDensity);
			EditorGUI.indentLevel--;
			EditorGUI.EndDisabledGroup ();
			EditorGUI.indentLevel--;
		}


		//Setup
		GUILayout.Box ("", GUILayout.ExpandWidth (true), GUILayout.Height (1));

		EditorGUILayout.Space ();
		if (GUILayout.Button ("Setup light settings in current scene")) {
			SetupLight ();
		}

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



		if (GUILayout.Button ("Setup light settings in scene collections")) {
			int sceneId = 1;
			int sceneCount = 0;
			foreach (var sceneCollection in currentCollections) {
				sceneCount += sceneCollection.names.Length;
			}

			foreach (var sceneCollection in currentCollections) {

				foreach (var item in sceneCollection.names) {
					EditorUtility.DisplayProgressBar ("Setting Lightning Settings", "Scene " + sceneId + "/" + sceneCount, sceneId / (float)sceneCount);
					EditorSceneManager.OpenScene (sceneCollection.path + item);
					SetupLight ();
					EditorSceneManager.SaveOpenScenes ();
					sceneId++;

				}
			}
			EditorUtility.ClearProgressBar ();
		}
	}


	/// <summary>
	/// Setups the light parameters.
	/// </summary>
	void SetupLight ()
	{		
		
		RenderSettings.skybox = skybox;
		RenderSettings.sun = sun;
		RenderSettings.ambientMode = ambientMode;
		RenderSettings.ambientIntensity = ambientInstensity;
		RenderSettings.ambientSkyColor = ambientSkyColor;
		RenderSettings.ambientEquatorColor = ambientEquatorColor;
		RenderSettings.ambientGroundColor = ambientGroundColor;

		RenderSettings.defaultReflectionMode = reflectionSource;
		RenderSettings.defaultReflectionResolution = defaultReflectionResolution;
		RenderSettings.reflectionIntensity = reflectionInstensity;
		RenderSettings.reflectionBounces = reflectionBounces;
		RenderSettings.customReflection = reflectionCubemap;
		LightmapEditorSettings.reflectionCubemapCompression = reflectionCubemapCompression;
		
		//Realtime GI
		Lightmapping.realtimeGI = realtimeGI;
		LightmapEditorSettings.realtimeResolution = resolutionRealtime;
		LightmapEditorSettings.maxAtlasHeight = LightmapEditorSettings.maxAtlasSize = atlasSize;

		//BakedGI
		Lightmapping.bakedGI = bakedGI;
		LightmapEditorSettings.bakeResolution = bakedResolution;
		LightmapEditorSettings.padding = bakedPadding;
		LightmapEditorSettings.textureCompression = bakedCompression;

		Lightmapping.indirectOutputScale = indirectIntensity;
		Lightmapping.bounceBoost = bounceBoost;

		//Fog
		RenderSettings.fog = fog;
		RenderSettings.fogColor = fogColor;
		RenderSettings.fogMode = fogMode;
		RenderSettings.fogDensity = fogDensity;
		EditorSceneManager.MarkAllScenesDirty ();
	}

#else

    [MenuItem ("World Streamer/Lightning Manager (Reguires Unity 5.5 or newer)")]
	static void Init ()
	{
	// Get existing open window or if none, make a new one:
	//LightningManager window = EditorWindow.GetWindow <LightningManager> ("Lightning Manager");
	//window.Show ();

	}

	#endif

}
