using UnityEngine;
using System.Collections;
using UnityEditor;


[CustomEditor (typeof(TerrainCullingSystem))]
[CanEditMultipleObjects]
public class TerrainCullingSystemEditor : Editor
{
	
	//TerrainCullingSystem _target;

	//	void OnEnable ()
	//	{
	//		_target = (TerrainCullingSystem)target;
	//
	//	}

	public override void OnInspectorGUI ()
	{
		TerrainCullingSystem culling = (TerrainCullingSystem)target;

	

		DrawDefaultInspector ();

		EditorGUILayout.HelpBox ("This option could generate crash on some unity versions - engine bug, it increase performance radically", MessageType.Warning);


		if (culling.disableTrees && GUILayout.Button ("Foliage culling On")) {
			foreach (Object obj in targets) { 
				((TerrainCullingSystem)obj).disableTrees = false;
			}
		}
		if (!culling.disableTrees && GUILayout.Button ("Foliage culling Off")) {
			foreach (Object obj in targets) { 
				((TerrainCullingSystem)obj).disableTrees = true;
			}
		}

	}
}
