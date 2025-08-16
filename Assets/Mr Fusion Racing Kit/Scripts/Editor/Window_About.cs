using UnityEngine;
using UnityEditor;
using System.IO;

public class Window_About : EditorWindow
{
    private static readonly Vector2 minWindowSize = new Vector2(480f, 600f);
    private string[] tabs = { "About", "Changelog" };

    private GUIStyle mButtonStyle;
    private GUIStyle centerLabelStyle;
    private GUIStyle rightLabelStyle;
    private GUIStyle mNotesStyle;

    private Texture2D Logo { get { return (Texture2D)Resources.Load("Editor/RGSK/Logo"); } }
    private Vector2 changeLogScrollPos = Vector2.zero;
    private string changeLog;
    private int tabIndex;

    private void OnEnable()
    {
        TextAsset file = (TextAsset)Resources.Load("Editor/RGSK/Changelog");
        if (file != null)
        {
            StringReader reader = new StringReader(file.text);
            changeLog = reader.ReadToEnd();
            reader.Close();
            return;
        }

        changeLog = "Not Found!";
    }

    private void OnGUI()
    {
        if (EditorApplication.isCompiling)
        {
            Close();
        }

        if (mButtonStyle == null)
        {
            mButtonStyle = new GUIStyle(GUI.skin.button);
            mButtonStyle.richText = true;
        }

        if (centerLabelStyle == null)
        {
            centerLabelStyle = new GUIStyle(EditorStyles.label);
            centerLabelStyle.wordWrap = true;
            centerLabelStyle.richText = true;
            centerLabelStyle.alignment = TextAnchor.MiddleCenter;
        }

        if (rightLabelStyle == null)
        {
            rightLabelStyle = new GUIStyle(EditorStyles.label);
            rightLabelStyle.wordWrap = true;
            rightLabelStyle.richText = true;
            rightLabelStyle.alignment = TextAnchor.MiddleRight;
        }

        if (mNotesStyle == null)
        {
            mNotesStyle = new GUIStyle(EditorStyles.textArea);
            mNotesStyle.richText = true;
            mNotesStyle.wordWrap = true;
        }

        if (Logo != null)
        {
            GUILayout.Label(Logo, centerLabelStyle);
        }

        tabIndex = GUILayout.SelectionGrid(tabIndex, tabs, 2);
        GUILayout.Space(20);

        switch (tabIndex)
        {
            case 0:
                EditorGUILayout.LabelField("<size=15><i>A powerful tool designed for creating racing games.</i></size>", centerLabelStyle);

                GUILayout.Space(10);

                GUILayout.BeginVertical("Box");
                GUILayout.Label("<b>Forums</b>", centerLabelStyle);
                EditorGUILayout.HelpBox("The forum is a great place to discuss and see what others are creating!", MessageType.Info);
                if (GUILayout.Button("<size=12>Forum</size>", mButtonStyle))
                {
                    Application.OpenURL("https://goo.gl/GRBxIg");
                }
                GUILayout.EndVertical();

                GUILayout.Space(10);
                GUILayout.BeginVertical("Box");
                GUILayout.Label("<b>Documentation</b>", centerLabelStyle);
                EditorGUILayout.HelpBox("A written manual on how to use the Racing Game Starter Kit!", MessageType.Info);
                if (GUILayout.Button("<size=12>Documentation</size>", mButtonStyle))
                {
                    Application.OpenURL("file://" + Application.dataPath + "/Racing Game Starter Kit/Documentation/Documentation.pdf");
                }
                GUILayout.EndVertical();

                GUILayout.Space(10);
                GUILayout.BeginVertical("Box");
                GUILayout.Label("<b>Video Tutorials</b>", centerLabelStyle);
                EditorGUILayout.HelpBox("A video tutorial series on how to use the Racing Game Starter Kit!", MessageType.Info);
                if (GUILayout.Button("<size=12>Video Tutorials</size>", mButtonStyle))
                {
                    Application.OpenURL("https://www.youtube.com/playlist?list=PLdNzy1P_hi4SQ9qg9Lv1CNC9wa6zs3Va3");
                }
                GUILayout.EndVertical();

                GUILayout.Space(10);
                GUILayout.BeginVertical("Box");
                GUILayout.Label("<b>Rate It</b>", centerLabelStyle);
                EditorGUILayout.HelpBox("Rate the Racing Game Starter Kit on the Unity Asset Store!", MessageType.Info);
                if (GUILayout.Button("<size=12>Unity Asset Store</size>", mButtonStyle))
                {
                    Application.OpenURL("https://www.assetstore.unity3d.com/en/#!/content/22615");
                }
                GUILayout.EndVertical();

                //GUILayout.Space(10);
                //EditorGUILayout.LabelField("<size=12>For support, contact <i>RGSKSupport@gmail.com</i></size>", centerLabelStyle);
                break;

            case 1:
                using (var scrollScope = new EditorGUILayout.ScrollViewScope(changeLogScrollPos, GUI.skin.box))
                {
                    changeLogScrollPos = scrollScope.scrollPosition;
                    EditorGUILayout.LabelField(changeLog, mNotesStyle);
                }
                break;
        }
    }

    [MenuItem("Window/Racing Game Starter Kit/About")]
    public static void OpenAboutWindow()
    {
        Window_About window = EditorWindow.GetWindow<Window_About>();

        window.titleContent = new GUIContent("About");
        window.minSize = minWindowSize;
        window.Show(true);
    }
}
