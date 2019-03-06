using System;
using UnityEditor;
using UnityEngine;
using JetBrains.Annotations;

namespace Nova.Editor
{
    [CustomEditor( typeof( UIViewController ), true )]
    public sealed class UIViewControllerEditor : UnityEditor.Editor
    {
        #region Properties

        [NotNull]
        private UIViewController m_viewController
        {
            get { return ( UIViewController ) target; }
        }

        private const float c_buttonHeight = 22f;

        private readonly Color[] m_buttonPrimaryColors =
            { new Color( 0.85f, 0.92f, 0.73f ), new Color( 0.24f, 0.29f, 0.39f ) };

        private readonly Color[] m_buttonDangerColors =
            { new Color( 0.92f, 0.54f, 0.4f ), new Color( 0.57f, 0.29f, 0.16f ) };

        #endregion

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUILayout.BeginHorizontal();
            MakeIdentifierButton();
            MakeResetBoundsButton();
            GUILayout.EndHorizontal();

            MakeInitializeViewsButton();
        }

        private void MakeIdentifierButton()
        {
            if ( GUILayout.Button( "Generate ID", new GUIStyle( GUI.skin.button )
                { normal = { textColor = GetButtonTextColor( "primary" ) }, fixedHeight = c_buttonHeight } ) )
            {
                if ( string.IsNullOrEmpty( m_viewController.Configuration.Identifier ) )
                {
                    m_viewController.Configuration.Identifier =
                        m_viewController.GetType().Name.Replace( "ViewController", "" ).ToLower();
                }
            }
        }

        private void MakeResetBoundsButton()
        {
            if ( GUILayout.Button( "Reset Bounds",
                new GUIStyle( GUI.skin.button )
                    { normal = { textColor = GetButtonTextColor( "danger" ) }, fixedHeight = c_buttonHeight } ) )
            {
                m_viewController.ResetBounds();
            }
        }

        private void ResetRectTransform( RectTransform rectTransform )
        {
            if ( rectTransform == null )
            {
                return;
            }

            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
        }

        private void MakeInitializeViewsButton()
        {
            if ( GUILayout.Button( "Initialize Views",
                new GUIStyle( GUI.skin.button )
                    { normal = { textColor = GetButtonTextColor( "primary" ) }, fixedHeight = c_buttonHeight } ) )
            {
                if ( m_viewController.transform.childCount > 0 )
                {
                    Debug.Log( "Terminated view initialization on <b>" + m_viewController.GetType().Name +
                               "</b> because it has children game objects" );
                    return;
                }

                GameObject view = MakeGameObject( "Views", typeof( View ) );

                GameObject background = MakeGameObject( "Background" );
                GameObject safeArea = MakeGameObject( "Safe Area", typeof( SafeArea ) );
                GameObject foreground = MakeGameObject( "Foreground" );

                background.transform.SetParent( view.transform );
                safeArea.transform.SetParent( view.transform );
                foreground.transform.SetParent( view.transform );

                background.transform.SetAsFirstSibling();
                foreground.transform.SetAsLastSibling();

                view.GetComponent<View>().Inject( background.transform, safeArea.transform, foreground.transform );
            }
        }

        private GameObject MakeGameObject( string objectName, params Type[] components )
        {
            // Prepend RectTransform to components list
            Type[] newComponents = new Type[components.Length + 1];
            newComponents[0] = typeof( RectTransform );
            Array.Copy( components, 0, newComponents, 1, components.Length );

            GameObject originGameObject = new GameObject( objectName, newComponents );
            GameObject gameObject = Instantiate( originGameObject, m_viewController.transform );
            gameObject.name = objectName;
            gameObject.transform.localScale = Vector3.one;

            ResetRectTransform( ( RectTransform ) gameObject.transform );

            DestroyImmediate( originGameObject );

            return gameObject;
        }

        /// <summary>
        /// Get button colors based on editor skin.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private Color GetButtonTextColor( string type )
        {
            bool darkMode = EditorGUIUtility.isProSkin;

            switch ( type )
            {
                case "primary":
                    return darkMode ? m_buttonPrimaryColors[0] : m_buttonPrimaryColors[1];
                case "danger":
                    return darkMode ? m_buttonDangerColors[0] : m_buttonDangerColors[1];
                default:
                    return darkMode ? Color.white : Color.black;
            }
        }
    }
}
