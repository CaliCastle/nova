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
        protected UIViewController m_initialViewController;

        [SerializeField]
        protected UINavigationBar m_navigationBar;

        protected bool m_isTransitioning;

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

            if ( m_navigationBar )
            {
                m_navigationBar.SetTitle( controller.Configuration.Title );
                m_navigationBar.Hidden = controller.Configuration.HideNavigationOnPush;
                m_navigationBar.BackButton.Hidden = ViewControllers.Count == 0;
            }

            ViewControllers.Push( controller );

            controller.Show( animates, onComplete );

            return controller;
        }

        public UIViewController Pop( bool animates = true, Action onComplete = null )
        {
            if ( ViewControllers.Count <= 1 )
            {
                return null;
            }

            UIViewController poppedViewController = ViewControllers.Pop();
            UIViewController prevViewController = ViewControllers.Peek();

            if ( m_navigationBar )
            {
                m_navigationBar.SetTitle( prevViewController.Configuration.Title );
                m_navigationBar.Hidden = prevViewController.Configuration.HideNavigationOnPush;
                m_navigationBar.BackButton.Hidden = ViewControllers.Count <= 1;
            }

            poppedViewController.ViewWillDisappear();
            poppedViewController.Hide( animates, () =>
            {
                onComplete?.Invoke();
                Destroy( poppedViewController.gameObject );
            } );

            return poppedViewController;
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

            if ( m_navigationBar == null )
            {
                Debug.LogError( $"<b>{GetType()}</b> doesn't have <i>`m_navigationBar`</i> field assigned." );
            }

            SetupNavigationBar();
        }

        protected override void ViewDidLoad()
        {
            base.ViewDidLoad();

            PresentInitialViewController();
        }

        #endregion UIViewController

        protected virtual void PresentInitialViewController()
        {
            if ( m_initialViewController == null )
            {
                return;
            }

            Push( m_initialViewController, false );
        }

        #region Private

        private void SetupNavigationBar()
        {
            if ( m_navigationBar != null )
            {
                m_navigationBar.BackDidTap = delegate { Pop(); };
            }
        }

        #endregion Private
    }
}
