using System.IO;
using UnityEditor;
using UnityEngine;
using System.Text.RegularExpressions;

namespace Nova.Editor
{
    internal sealed class ScriptingPreferences : IPreferenceItem
    {
        public static string NamespaceName => NovaPreferences.Get( c_namespacePrefKey, c_defaultNamespace );
        public static string NovaPath => NovaPreferences.Get( c_novaPathPrefKey, $"/Packages/Nova" );

        #region IPreferenceItem

        public string Name => "Scripting";

        public void OnInitialize()
        {
            m_namespaceName = NamespaceName;
            m_novaPath = NovaPath;
        }

        public void OnGUI()
        {
            // namespace
            EditorGUI.BeginChangeCheck();
            GUIContent namespaceContent = new GUIContent( "Namespace", "Namespace to use when generating code" );
            m_namespaceName = EditorGUILayout.TextField( namespaceContent, m_namespaceName );
            if ( EditorGUI.EndChangeCheck() )
            {
                m_namespaceName = ValidateCodeName( m_namespaceName );
                if ( string.IsNullOrEmpty( m_namespaceName ) )
                {
                    m_namespaceName = c_defaultNamespace;
                }

                NovaPreferences.Set( c_namespacePrefKey, m_namespaceName );
            }

            // nova path
            EditorGUI.BeginChangeCheck();
            GUIContent novaPathContent = new GUIContent( "Nova Package Path", "Path to the Nova package folder" );
            m_novaPath = EditorGUILayout.TextField( novaPathContent, m_novaPath );
            if ( EditorGUI.EndChangeCheck() )
            {
                m_novaPath = ValidatePath( m_novaPath );
                if ( m_novaPath.EndsWith( "/" ) == false )
                {
                    m_novaPath += "/";
                }

                NovaPreferences.Set( c_novaPathPrefKey, m_novaPath );
            }
        }

        #endregion

        private const string c_defaultNamespace = "Nova";
        private const string c_namespacePrefKey = "Scripting_Namespace";
        private const string c_novaPathPrefKey = "Scripting_NovaPath";

        private string m_namespaceName;
        private string m_novaPath;

        private static string ValidateCodeName( string str )
        {
            return Regex.Replace( str, @"^\s*[0-9]+|\s+|\W", string.Empty );
        }

        private string ValidatePath( string str )
        {
            string format =
                $"[{Regex.Escape( new string( Path.GetInvalidPathChars() ) )}]|[{Regex.Escape( ( ":*?|\"<>|" ) )}]";
            return Regex.Replace( str, format, string.Empty );
        }
    }
}
