using UnityEditor;
using UnityEngine;

namespace SMG.EzRenamer
{
    public class EzRR_Style : Editor
    {
	    public static GUIStyle customStyle = new GUIStyle();
	    public static readonly Color32 uiLineColor = new Color32(89, 89, 89, 255);
	    
	    public delegate void ButtonAction();

        public static void DrawHeader(string title)
        {
            // Configurate the style to mach the foldout one
            customStyle = new GUIStyle(EditorStyles.label);
            customStyle.fontStyle = FontStyle.Bold;
            // Draw the header
            EditorGUILayout.LabelField(title, customStyle);
            // Revert the style
            customStyle = EditorStyles.label;
        }

        public static bool DrawFoldoutHeader(string title, bool show)
        {
            // Configurate the style to mach the foldout one
            customStyle = new GUIStyle(EditorStyles.foldout);
	        customStyle.fontStyle = FontStyle.Bold;
            // Draw the header
	        show = EditorGUILayout.Foldout(show, title, customStyle);
            // Revert the style
            customStyle = EditorStyles.foldout;
            return show;
        }

        public static void DrawUILine(Color color, int thickness = 1, int padding = 10)
        {
            Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
            r.height = thickness;
            r.y += padding / 2;
            r.x -= 2;
            r.width += 6;
            EditorGUI.DrawRect(r, color);
        }
        
	    public static void DrawHelpButton(string url)
	    {
	    	// Creates the custom style to remove the button layout and leave just the texture
	    	customStyle = new GUIStyle();
	    	RectOffset _rectOffset = customStyle.border;
	    	_rectOffset.left = 0;
	    	_rectOffset.right = 0;
	    	_rectOffset.top = 0;
	    	_rectOffset.bottom = 0;
	    	// Creates the button using the _Help texture from Unity
		    if(GUILayout.Button(EditorGUIUtility.FindTexture("_Help"), customStyle, GUILayout.Width(20), GUILayout.Height(20)))
		    {
			    Application.OpenURL(url);
		    }
	    }
        
	    public static bool ToggleLeftBold(string title, bool value)
	    {
	    	customStyle = new GUIStyle(EditorStyles.label);
	    	customStyle.fontStyle = FontStyle.Bold;
	    	value = EditorGUILayout.ToggleLeft(title, value, customStyle);
	    	customStyle = EditorStyles.foldout;
	    	return value;
	    }
	    
	    public static void DrawButton(string title, ButtonAction action, float height = 20, float width = 0)
	    {
	    	EditorGUILayout.BeginHorizontal();
		    GUILayout.Space(20);
		    if(width > 0)
		    {
		    	if (GUILayout.Button(title, GUILayout.Width(width), GUILayout.Height(height)))
			    	action();
		    }
		    else
		    {
		    	if (GUILayout.Button(title, GUILayout.Height(height)))
			    	action();
		    }
		    GUILayout.Space(20);
		    EditorGUILayout.EndHorizontal();
	    }
	    
	    public static void DrawFooter()
	    {
		    EditorGUILayout.BeginHorizontal();
		    GUILayout.Space(20);
		    if (GUILayout.Button("More Assets", GUILayout.Height(20)))
			    Application.OpenURL("https://www.assetstore.unity3d.com/en/#!/search/page=1/sortby=popularity/query=publisher:11524");
		    if (GUILayout.Button("Website", GUILayout.Height(20)))
			    Application.OpenURL("https://solomidgames.com");
		    GUILayout.Space(20);
		    EditorGUILayout.EndHorizontal();

		    EditorGUILayout.BeginHorizontal();
		    GUILayout.Space(20);
		    if (GUILayout.Button("Help", GUILayout.Height(20)))
			    Application.OpenURL("mailto:help@solomidgames.com");
		    GUILayout.Space(20);
		    EditorGUILayout.EndHorizontal();    
                
		    EditorGUILayout.BeginHorizontal();
		    GUILayout.Space(20);
		    if (GUILayout.Button("Follow us on Twitter", GUILayout.Height(20)))
			    Application.OpenURL("https://twitter.com/solomidgames");
		    GUILayout.Space(20);
		    EditorGUILayout.EndHorizontal();  
	    }
    }
}