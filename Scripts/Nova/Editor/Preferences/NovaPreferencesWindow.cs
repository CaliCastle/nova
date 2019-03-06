using System;
using System.Linq;
using UnityEditor;
using System.Collections.Generic;

namespace Nova.Editor
{
    internal static class NovaPreferencesWindow
    {
        private static List<IPreferenceItem> s_preferences;

        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            s_preferences = new List<IPreferenceItem>();

            // add preference items
            Type interfaceType = typeof( IPreferenceItem );
            IEnumerable<Type> preferenceTypes = AppDomain.CurrentDomain.GetAssemblies()
                                                         .SelectMany( x => x.GetTypes() )
                                                         .Where( x => x.IsClass && interfaceType.IsAssignableFrom( x ) );

            foreach ( Type prefType in preferenceTypes )
            {
                if ( Activator.CreateInstance( prefType ) is IPreferenceItem preferenceItem )
                {
                    s_preferences.Add( preferenceItem ); 
                }
            }

            foreach ( var preference in s_preferences )
            {
                preference.OnInitialize();
            }
        }

        [PreferenceItem( "Nova" )]
        private static void PreferencesGUI()
        {
            EditorGUILayoutUtils.RichLabelField( "<b>Nova UI Framework</b> by Cali Castle" );

            EditorGUILayout.Separator();

            EditorGUILayoutUtils.HorizontalLine( 2f, 0.95f );

            for ( int i = 0; i < s_preferences.Count; i++ )
            {
                IPreferenceItem preference = s_preferences[i];

                EditorGUILayoutUtils.RichLabelField( $"<b>{preference.Name}</b>" );

                using ( new EditorGUI.IndentLevelScope() )
                {
                    preference.OnGUI();
                }

                if ( i < s_preferences.Count - 1 )
                {
                    EditorGUILayoutUtils.HorizontalLine( 1f, 0.85f );
                }
            }
        }
    }
}
