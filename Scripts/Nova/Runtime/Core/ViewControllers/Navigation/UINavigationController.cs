using System;
using UnityEngine;
using JetBrains.Annotations;
using System.Collections.Generic;

namespace Nova
{
    public abstract class UINavigationController : UIViewController
    {
        #region Properties

        public readonly Stack<UIViewController> ViewControllers = new Stack<UIViewController>();

        /// <summary>
        /// The initial view controller to be present on
        /// </summary>
        [SerializeField]
        private UIViewController m_initialViewController;

        private bool m_isTransitioning;

        #endregion Properties

        #region Public

        [CanBeNull]
        public T Push<T>( bool animates = true, Action<T> preparation = null, Action onComplete = null )
            where T : UIViewController
        {
            if ( m_isTransitioning )
            {
                return null;
            }

            T prefab = m_window.GetControllerPrefab<T>();
            if ( prefab == null )
            {
                return null;
            }

            return Push( prefab, animates, preparation, onComplete );
        }

        [CanBeNull]
        public T Push<T>( [NotNull] T viewController, bool animates = true, Action<T> preparation = null,
            Action onComplete = null ) where T : UIViewController
        {
            if ( m_isTransitioning )
            {
                return null;
            }

            T controller = Instantiate( viewController, m_view.Background );
            controller.Inject( m_window, this );
            controller.name = viewController.name;
            controller.transform.localScale = Vector3.one;

            preparation?.Invoke( controller );

            ViewControllers.Push( controller );

            controller.Show( animates, onComplete );

            return controller;
        }

        public UIViewController Pop( bool animates = true, Action onComplete = null )
        {
            UIViewController controller = ViewControllers.Pop();
            controller.ViewWillDisappear();
            controller.Hide( animates, () =>
            {
                onComplete?.Invoke();
                Destroy( controller.gameObject );
            } );

            return controller;
        }

        #endregion Public

        #region UIViewController

        protected override void ViewWillLoad()
        {
            base.ViewWillLoad();

            if ( m_initialViewController == null )
            {
                Debug.LogError( $"<b>{GetType()}</b> doesn't have <i>`m_initialViewController`</i> field assigned." );
            }

            PresentInitialViewController();
        }

        #endregion UIViewController

        #region Private

        private void PresentInitialViewController()
        {
            if ( m_initialViewController == null )
            {
                return;
            }

            Push( m_initialViewController, false );
        }

        #endregion Private
    }
}
