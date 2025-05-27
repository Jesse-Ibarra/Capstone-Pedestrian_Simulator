using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;


[CustomEditor (typeof(ColliderStreamerManager))]
class ColliderStreamerManagerEditor : Editor
{

	#if UNITY_5_3 || UNITY_5_3_OR_NEWER

	public override void OnInspectorGUI ()
	{

		DrawDefaultInspector ();



		ColliderStreamerManager myTarget = (ColliderStreamerManager)target;


		string newTag = EditorGUILayout.TagField (new GUIContent ("Player Tag", "Collider Streamer will search for player by tag."), myTarget.playerTag);

		if (myTarget.playerTag != newTag) {
			myTarget.playerTag = newTag;
			EditorUtility.SetDirty (myTarget);
		}

		//Check tag exist
		// Open tag manager
		SerializedObject tagManager = new SerializedObject (AssetDatabase.LoadAllAssetsAtPath ("ProjectSettings/TagManager.asset") [0]);
		SerializedProperty tagsProp = tagManager.FindProperty ("tags");

		// First check if it is not already present
		bool found = false;
		for (int i = 0; i < tagsProp.arraySize; i++) {
			SerializedProperty t = tagsProp.GetArrayElementAtIndex (i);
			if (t.stringValue.Equals (ColliderStreamerManager.COLLIDERSTREAMERMANAGERTAG)) {
				found = true;
				break;
			}
		}
		if (!found) {
			EditorGUILayout.HelpBox ("No tag " + ColliderStreamerManager.COLLIDERSTREAMERMANAGERTAG + " in unity.", MessageType.Error, true);
			if (GUILayout.Button ("Add tag to unity")) {
				found = false;
				for (int i = 0; i < tagsProp.arraySize; i++) {
					SerializedProperty t = tagsProp.GetArrayElementAtIndex (i);
					if (t.stringValue.Equals (ColliderStreamerManager.COLLIDERSTREAMERMANAGERTAG)) {
						found = true;
						break;
					}
				}
				if (!found) {
					tagsProp.InsertArrayElementAtIndex (0);
					SerializedProperty n = tagsProp.GetArrayElementAtIndex (0);
					n.stringValue = ColliderStreamerManager.COLLIDERSTREAMERMANAGERTAG;
					tagManager.ApplyModifiedProperties ();
				}

				myTarget.tag = Streamer.STREAMERTAG;
			}
		}

		if (myTarget.tag != ColliderStreamerManager.COLLIDERSTREAMERMANAGERTAG) {
			EditorGUILayout.HelpBox ("Streamer must have " + ColliderStreamerManager.COLLIDERSTREAMERMANAGERTAG + " Tag.", MessageType.Error, true);
			if (GUILayout.Button ("Change tag")) {
				myTarget.tag = ColliderStreamerManager.COLLIDERSTREAMERMANAGERTAG;
			}

		}



	}



	#endif
}