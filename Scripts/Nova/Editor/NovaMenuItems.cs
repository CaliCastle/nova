using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;

namespace Nova.Editor
{
    public sealed class NovaMenuItems
    {
        [MenuItem( "GameObject/Nova/UIWindow", false, 10 )]
        private static void CreateUIWindow( MenuCommand menuCommand )
        {
            GameObject window = CreateGameObject( menuCommand, "UIWindow", typeof( UIWindow ) );
            GameObject view = MakeUIView( menuCommand, "Main View" );
            view.transform.SetParent( window.transform );
            window.GetComponent<UIWindow>().Inject( view.GetComponent<UIView>() );
        }

        [MenuItem( "GameObject/Nova/UIView", false, 10 )]
        private static void CreateUIView( MenuCommand menuCommand )
        {
            MakeUIView( menuCommand );
        }

        private static GameObject MakeUIView( MenuCommand menuCommand, string name = "UIView" )
        {
            GameObject gameObject = CreateGameObject( menuCommand, name, typeof( Canvas ), typeof( CanvasScaler ),
                typeof( CanvasGroup ), typeof( UIView ) );

            Canvas canvas = gameObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            CanvasScaler scaler = gameObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;

            return gameObject;
        }

        private static GameObject CreateGameObject( MenuCommand menuCommand, string name, params Type[] types )
        {
            // create a custom view controller
            GameObject gameObject = new GameObject( name, types );
            // ensure it gets reparented if this was a context click (otherwise does nothing)
            GameObjectUtility.SetParentAndAlign( gameObject, menuCommand.context as GameObject );
            // register the creation in the undo system
            Undo.RegisterCreatedObjectUndo( gameObject, "Create " + gameObject.name );
            Selection.activeObject = gameObject;

            return gameObject;
        }

        private static void CreateScriptAsset( string templatePath, string destName )
        {
            typeof( ProjectWindowUtil )
                .GetMethod( "CreateScriptAsset",
                    System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic )
                ?.Invoke( null, new object[] {templatePath, destName} );
        }
    }
}
