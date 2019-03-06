using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Nova.Editor
{
    internal class CreateViewControllerEditorWindow : EditorWindow
    {
        private string m_viewControllerInput = "ViewController";

        [MenuItem( "Assets/Create/Nova/ViewController", false, 11 )]
        private static void CreateUIViewController( MenuCommand menuCommand )
        {
            CreateViewControllerEditorWindow window = CreateInstance<CreateViewControllerEditorWindow>();
            window.position = new Rect( Screen.width / 2f, Screen.height / 2f, 300f, 100f );
            window.titleContent = new GUIContent( "Create a ViewController" );
            window.ShowUtility();
        }

        private void OnGUI()
        {
            m_viewControllerInput = EditorGUILayout.TextField( "Class Name: ", m_viewControllerInput );

            GUILayout.Space( 15f );
            GUILayout.BeginHorizontal();

            if ( GUILayout.Button( "Create" ) || Event.current.keyCode == KeyCode.Return )
            {
                MakeViewController( m_viewControllerInput );
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

            stringBuilder.Append( indent.ApplyIndent( $"public sealed class {className} : UIViewController\n" ) );
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
