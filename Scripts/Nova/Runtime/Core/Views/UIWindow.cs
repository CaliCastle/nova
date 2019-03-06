using System;
using UnityEngine;
using JetBrains.Annotations;
using System.Collections.Generic;

namespace Nova
{
    public interface INovaLaunchable
    {
        /// <summary>
        /// Entry point for when the UI is configured and prepared.
        /// </summary>
        /// <param name="window">The key window</param>
        void LiftOff( UIWindow window );
    }

    public class UIWindow : UIResponder
    {
        #region Properties

        public UIView View => m_view;

        [SerializeField]
        private UIView m_view;

        [SerializeField]
        private bool m_shouldResetView = true;

        [SerializeField]
        private List<UIViewController> m_viewControllerPrefabPool;

        /// <summary>
        /// View controller references for resetting only.
        /// </summary>
        private readonly List<UIViewController> m_viewControllers = new List<UIViewController>();

        #endregion Properties

        #region Public

        /// <summary>
        /// 
        /// </summary>
        /// <param name="animates"></param>
        /// <param name="preparation"></param>
        /// <param name="onComplete"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        [CanBeNull]
        public T Present<T>( bool animates = true, Action<T> preparation = null, Action onComplete = null ) where T : UIViewController
        {
            T prefab = GetControllerPrefab<T>();

            if ( prefab == null )
            {
                return null;
            }

            T controller = Instantiate( prefab, m_view.transform );
            controller.gameObject.name = typeof( T ).Name;
            controller.ResetBounds();
            controller.transform.SetAsLastSibling();
            controller.Inject( this );

            preparation?.Invoke( controller );

            controller.Show( animates, onComplete );

            return controller;
        }

        [CanBeNull]
        public T GetControllerPrefab<T>() where T : UIViewController
        {
            T prefab = m_viewControllerPrefabPool.Find( vc => vc is T ) as T;
            if ( prefab == null )
            {
                Debug.LogError( $"<b>{typeof( T )}</b> couldn't be found in the controller pool from {gameObject.name}." );
            }

            return prefab;
        }

        public void Inject( [NotNull] UIView view )
        {
            m_view = view;
        }

        #endregion Public

        #region MonoBehaviour

        private void Awake()
        {
            if ( m_view == null )
            {
                Debug.LogError( $"<b>{gameObject.name}</b> doesn't have <i>`m_view`</i> field assigned." );
                return;
            }

            ResetView();
            Launch();
        }

        #endregion MonoBehaviour

        #region Private

        /// <summary>
        /// Destroy any
        /// </summary>
        private void ResetView()
        {
            if ( !m_shouldResetView )
            {
                return;
            }

            m_view.GetComponentsInChildren( true, m_viewControllers );
            foreach ( UIViewController controller in m_viewControllers )
            {
                if ( controller )
                {
                    Destroy( controller.gameObject );
                }
            }
        }

        /// <summary>
        /// Once everything is ready, launch the NovaLaunchable implemented script.
        /// </summary>
        private void Launch()
        {
            INovaLaunchable nova = gameObject.GetComponent<INovaLaunchable>();
            nova?.LiftOff( this );
        }

        #endregion Private
    }
}
