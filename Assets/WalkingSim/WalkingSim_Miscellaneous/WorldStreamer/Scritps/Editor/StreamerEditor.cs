using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;


[CustomEditor (typeof(Streamer))]
class StreamerEditor : Editor
{
	bool checkedScenes = false;
	bool scenesToAddCheck = false;

	public override void OnInspectorGUI ()
	{

		DrawDefaultInspector ();



		Streamer myTarget = (Streamer)target;

		if (myTarget.player != null && EditorUtility.IsPersistent (myTarget.player)) {
			myTarget.player = null;
		}

		if (myTarget.spawnedPlayer) {
			string newTag = EditorGUILayout.TagField (new GUIContent ("Player Tag", "Streamer will search for player by tag."), myTarget.playerTag);

			if (myTarget.playerTag != newTag) {
				myTarget.playerTag = newTag;
				EditorUtility.SetDirty (myTarget);
			}
		}

		if (myTarget.sceneCollection == null) {
			EditorGUILayout.HelpBox ("Add scene collection", MessageType.Error, true);
		} else if (myTarget.sceneCollection != null) {

			SceneCollection currentCollection = myTarget.sceneCollection;
		
			if (!Directory.Exists (currentCollection.path)) {

				EditorGUILayout.HelpBox ("Scene collection path doesn't exist.", MessageType.Error, true);
				return;

			}

			if (!checkedScenes) {
				checkedScenes = true;
				List<EditorBuildSettingsScene> scenesList = new List<EditorBuildSettingsScene> ();
				scenesList.AddRange (EditorBuildSettings.scenes);

				List<string> scenesToAdd = new List<string> ();
				scenesToAdd.AddRange (currentCollection.names);

				foreach (var item in scenesList) {
					if (scenesToAdd.Contains (item.path.Replace (currentCollection.path, "")) && item.enabled) {
						scenesToAdd.Remove (item.path.Replace (currentCollection.path, ""));
					}
				}

				if (scenesToAdd.Count > 0) {
					scenesToAddCheck = true;
				}
			}

			if (scenesToAddCheck)
				EditorGUILayout.HelpBox ("Add scenes from scene collection to build settings.", MessageType.Error, true);

			if (GUILayout.Button ("Add scenes to build settings")) {
				AddScenesToBuild (currentCollection);
				scenesToAddCheck = false;
			}


			//Check tag exist
			//Open tag manager
			SerializedObject tagManager = new SerializedObject (AssetDatabase.LoadAllAssetsAtPath ("ProjectSettings/TagManager.asset") [0]);
			SerializedProperty tagsProp = tagManager.FindProperty ("tags");

	

			// First check if it is not already present
			bool found = false;
			for (int i = 0; i < tagsProp.arraySize; i++) {
				SerializedProperty t = tagsProp.GetArrayElementAtIndex (i);
				if (t.stringValue.Equals (Streamer.STREAMERTAG)) {
					found = true;
					break;
				}
			}
			if (!found) {
				EditorGUILayout.HelpBox ("No tag " + Streamer.STREAMERTAG + " in unity.", MessageType.Error, true);
				if (GUILayout.Button ("Add tag to unity")) {
					found = false;
					for (int i = 0; i < tagsProp.arraySize; i++) {
						SerializedProperty t = tagsProp.GetArrayElementAtIndex (i);
						if (t.stringValue.Equals (Streamer.STREAMERTAG)) {
							found = true;
							break;
						}
					}
					if (!found) {
						tagsProp.InsertArrayElementAtIndex (0);
						SerializedProperty n = tagsProp.GetArrayElementAtIndex (0);
						n.stringValue = Streamer.STREAMERTAG;
						tagManager.ApplyModifiedProperties ();
					}

					myTarget.tag = Streamer.STREAMERTAG;
				}
			}

			if (myTarget.tag != Streamer.STREAMERTAG) {
				EditorGUILayout.HelpBox ("Streamer must have " + Streamer.STREAMERTAG + " Tag.", MessageType.Error, true);
				if (GUILayout.Button ("Change tag")) {
					myTarget.tag = Streamer.STREAMERTAG;
				}

			}

			if (myTarget.deloadingRange.x < myTarget.loadingRange.x || myTarget.deloadingRange.y < myTarget.loadingRange.y || myTarget.deloadingRange.z < myTarget.loadingRange.z) {
				EditorGUILayout.HelpBox ("Streamer deloading range must >= loading range", MessageType.Error, true);
			}
			if (myTarget.looping) {
				if (Mathf.Abs (myTarget.sceneCollection.xLimitsx - myTarget.sceneCollection.xLimitsy) < myTarget.deloadingRange.x * 2 ||
				    Mathf.Abs (myTarget.sceneCollection.yLimitsx - myTarget.sceneCollection.yLimitsy) < myTarget.deloadingRange.y * 2 ||
				    Mathf.Abs (myTarget.sceneCollection.zLimitsx - myTarget.sceneCollection.zLimitsy) < myTarget.deloadingRange.z * 2) {
					EditorGUILayout.HelpBox ("Streamer deloading range * 2 must > scene collection limits for looping to work correctly", MessageType.Warning, true);
				}
			}

		}


	}

	/// <summary>
	/// Adds the scenes to build.
	/// </summary>
	void AddScenesToBuild (SceneCollection sceneCollection)
	{
	
		List<EditorBuildSettingsScene> scenesList = new List<EditorBuildSettingsScene> ();
		scenesList.AddRange (EditorBuildSettings.scenes);
		
		List<string> scenesToAdd = new List<string> ();
		scenesToAdd.AddRange (sceneCollection.names);
		
		foreach (var item in scenesList) {
			if (scenesToAdd.Contains (item.path.Replace (sceneCollection.path, ""))) {
				scenesToAdd.Remove (item.path.Replace (sceneCollection.path, ""));
				item.enabled = true;
			}
		}
		
		foreach (var item in scenesToAdd) {
			scenesList.Add (new EditorBuildSettingsScene (sceneCollection.path + item, true));
		}
		
		EditorBuildSettings.scenes = scenesList.ToArray ();
	}


}