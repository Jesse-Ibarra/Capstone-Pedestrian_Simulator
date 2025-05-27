using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;


[CustomEditor (typeof(ColliderStreamer))]
class ColliderStreamerEditor : Editor
{

	#if UNITY_5_3 || UNITY_5_3_OR_NEWER
		
	public override void OnInspectorGUI ()
	{

		DrawDefaultInspector ();

		ColliderStreamer myTarget = (ColliderStreamer)target;

		if (string.IsNullOrEmpty (myTarget.sceneName)) {
			EditorGUILayout.HelpBox ("Set scene name", MessageType.Error, true);
		} else {

		
		
			List<EditorBuildSettingsScene> scenesList = new List<EditorBuildSettingsScene> ();
			scenesList.AddRange (EditorBuildSettings.scenes);

			
			if (!string.IsNullOrEmpty (myTarget.scenePath) && !File.Exists (myTarget.scenePath)) {

				EditorGUILayout.HelpBox ("Scene collider doesn't exist.", MessageType.Error, true);


				return;
			}

//			myTarget.sceneName
			bool addScene = true;
			foreach (var item in scenesList) {
				if (myTarget.scenePath == item.path && item.enabled) {
					addScene = false;
					break;
				}
			}

			if (addScene) {
				EditorGUILayout.HelpBox ("Add scenes from scene collection to build settings.", MessageType.Error, true);
				if (GUILayout.Button ("Add scenes to build settings")) {
					AddScenesToBuild (myTarget.scenePath);
				}
			}


		}


	}

	/// <summary>
	/// Adds the scenes to build.
	/// </summary>
	void AddScenesToBuild (string scenePath)
	{
	
		List<EditorBuildSettingsScene> scenesList = new List<EditorBuildSettingsScene> ();
		scenesList.AddRange (EditorBuildSettings.scenes);
		
		List<string> scenesToAdd = new List<string> ();
		scenesToAdd.Add (scenePath);
		
		foreach (var item in scenesList) {
			if (scenesToAdd.Contains (item.path)) {
				scenesToAdd.Remove (item.path);
				item.enabled = true;
			}
		}
		
		foreach (var item in scenesToAdd) {
			scenesList.Add (new EditorBuildSettingsScene (item, true));
		}
		
		EditorBuildSettings.scenes = scenesList.ToArray ();
	}


	#endif
}