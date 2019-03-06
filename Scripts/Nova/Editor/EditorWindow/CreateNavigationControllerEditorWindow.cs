using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Nova.Editor
{
    internal class CreateNavigationControllerEditorWindow : EditorWindow
    {
        private string m_navigationControllerInput = "NavigationController";

        [MenuItem( "Assets/Create/Nova/NavigationController", false, 10 )]
        private static void CreateUINavigationController( MenuCommand menuCommand )
        {
            CreateNavigationControllerEditorWindow window = CreateInstance<CreateNavigationControllerEditorWindow>();
            window.position = new Rect( Screen.width / 2f, Screen.height / 2f, 300f, 100f );
            window.titleContent = new GUIContent( "Create a NavigationController" );
            window.ShowUtility();
        }

        private void OnGUI()
        {
            m_navigationControllerInput = EditorGUILayout.TextField( "Class Name: ", m_navigationControllerInput );

            GUILayout.Space( 15f );
            GUILayout.BeginHorizontal();

            if ( GUILayout.Button( "Create" ) || Event.current.keyCode == KeyCode.Return )
            {
                MakeViewController( m_navigationControllerInput );
                Close();
            }

            if ( GUILayout.Button( "Cancel" ) )
            {
                Close();
            }

            GUILayout.EndHorizontal();
            Repaint();
        }

        private void MakeViewController( string className )
        {
            string selectedPath = AssetDatabase.GUIDToAssetPath( Selection.assetGUIDs[0] );

            // generate code text
            StringBuilder stringBuilder = new StringBuilder();
            TextIndentHelper indent = TextIndentHelper.StandardSpacesHelper;

            stringBuilder.Append( "using Nova;\n" );
            stringBuilder.Append( "\n" );
            stringBuilder.Append( $"namespace {ScriptingPreferences.NamespaceName}\n" );
            stringBuilder.Append( "{\n" );
            indent.IndentLevel++;

            stringBuilder.Append( indent.ApplyIndent( $"public sealed class {className} : UINavigationController\n" ) );
            stringBuilder.Append( indent.ApplyIndent( "{\n" ) );
            indent.IndentLevel++;

            stringBuilder.Append( indent.ApplyIndent( "\n" ) );
            indent.IndentLevel--;

            stringBuilder.Append( indent.ApplyIndent( "}\n" ) );

            indent.IndentLevel--;
            stringBuilder.Append( indent.ApplyIndent( "}\n" ) );

            string fullPath = $"{selectedPath}/{className}.cs";

            // save to file
            using ( StreamWriter writer = new StreamWriter( fullPath, false ) )
            {
                writer.Write( stringBuilder.ToString() );
            }

            AssetDatabase.ImportAsset( fullPath, ImportAssetOptions.ForceUpdate );
        }
    }
}
