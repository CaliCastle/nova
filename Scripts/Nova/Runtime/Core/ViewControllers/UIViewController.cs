using System;
using UnityEngine;
using System.Collections;
using JetBrains.Annotations;

namespace Nova
{
    [Serializable]
    public sealed class UIViewControllerConfiguration
    {
        /// <summary>
        /// Unique ID
        /// </summary>
        public string Identifier = string.Empty;

        /// <summary>
        /// The view title for header to display
        /// </summary>
        public string Title = string.Empty;

        /// <summary>
        /// The transition type for presenting / dismissing
        /// </summary>
        public UIViewControllerTransitionType TransitionType = UIViewControllerTransitionType.Fade;

        /// <summary>
        /// Whether or not to hide the top and bottom gradient overlay when presenting
        /// </summary>
        public bool HideNavigationOverlays;

        /// <summary>
        /// Whether or not to hide the bottom close button when presenting
        /// </summary>
        public bool HideNavigationCloseButton;

        public bool HideNavigationTitle;
    }

    [RequireComponent( typeof( CanvasGroup ) )]
    public abstract class UIViewController : UIResponder
    {
        #region Properties

        [Header( "====== Nova View Controller ======" )]
        public UIViewControllerConfiguration Configuration;

        public View View => m_view;

        [CanBeNull]
        public UINavigationController NavigationController => m_navigationController;

        /// <summary>
        /// Root window
        /// </summary>
        protected UIWindow m_window { get; private set; }

        protected View m_view;

        [CanBeNull]
        private UINavigationController m_navigationController;

        [SerializeField, Range( 0.1f, 1f )]
        private float m_presentationDuration = 0.2f;

        /// <summary>
        /// Indicator for whether or not currently fading
        /// </summary>
        private bool m_isFading;

        private CanvasGroup m_canvasGroup;

        #endregion Properties

        #region Public

        public virtual void ViewWillDisappear()
        {
        }

        /// <summary>
        /// Present another view controller
        /// </summary>
        /// <param name="viewController"></param>
        /// <param name="duration"></param>
        /// <param name="onComplete"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual void Present( float? duration = null, Action onComplete = null )
        {
        }

        /// <summary>
        /// Dismiss self
        /// </summary>
        /// <param name="duration"></param>
        /// <param name="onComplete"></param>
        public virtual void Dismiss( float? duration = null, Action onComplete = null )
        {
            switch ( Configuration.TransitionType )
            {
                case UIViewControllerTransitionType.Fade:
                    FadeOut( duration, onComplete );
                    break;
                default:
                    return;
            }
        }

        public virtual void Show( bool animates = true, Action onComplete = null )
        {
            if ( !gameObject.activeInHierarchy )
            {
                gameObject.SetActive( true );
            }

            if ( !animates )
            {
                onComplete?.Invoke();
                return;
            }

            switch ( Configuration.TransitionType )
            {
                case UIViewControllerTransitionType.Fade:
                    FadeIn( onComplete: onComplete );
                    break;
                default:
                    return;
            }
        }

        public virtual void Hide( bool animates = true, Action onComplete = null )
        {
            if ( !animates )
            {
                onComplete?.Invoke();
                return;
            }

            switch ( Configuration.TransitionType )
            {
                case UIViewControllerTransitionType.Fade:
                    FadeOut( onComplete: onComplete );
                    break;
                default:
                    return;
            }
        }

        /// <summary>
        /// Fade the view in
        /// </summary>
        /// <param name="duration"></param>
        /// <param name="onComplete">callback handler</param>
        public void FadeIn( float? duration = null, Action onComplete = null )
        {
            if ( m_canvasGroup != null )
            {
                m_canvasGroup.alpha = 0f;
            }

            FadeTo( 1f, duration ?? m_presentationDuration, onComplete );
        }

        /// <summary>
        /// Fade the view out
        /// </summary>
        /// <param name="duration"></param>
        /// <param name="onComplete">callback handler</param>
        public void FadeOut( float? duration = null, Action onComplete = null )
        {
            if ( m_canvasGroup != null )
            {
                m_canvasGroup.alpha = 1f;
            }

            FadeTo( 0f, duration ?? m_presentationDuration, onComplete );
        }

        /// <summary>
        /// Fade alpha to a certain value
        /// </summary>
        /// <param name="opacity"></param>
        /// <param name="duration"></param>
        /// <param name="onComplete"></param>
        public void FadeTo( float opacity, float duration, Action onComplete = null )
        {
            if ( m_isFading || !gameObject.activeInHierarchy ) return;

            StartCoroutine( FadeOpacityTo( opacity, duration, onComplete ) );
        }

        /// <summary>
        /// Reset transform bounds to pin to full screen edges
        /// </summary>
        public void ResetBounds()
        {
            RectTransform rectTransform = transform as RectTransform;
            if ( rectTransform == null )
            {
                return;
            }

            rectTransform.localRotation = Quaternion.identity;
            rectTransform.localScale = Vector3.one;
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
        }

        /// <summary>
        /// Inject the desired dependencies
        /// </summary>
        /// <param name="window"></param>
        /// <param name="navigationController"></param>
        public void Inject( [NotNull] UIWindow window, UINavigationController navigationController = null )
        {
            m_window = window;
            m_navigationController = navigationController;
        }

        #endregion Public

        #region MonoBehaviour

        protected void Awake()
        {
            GetRootWindow();
            GetView();
            GetCanvasGroup();

            ViewWillLoad();
        }

        protected void Start()
        {
            ViewDidLoad();
        }

        protected void OnEnable()
        {
            ViewIsEnabled();
        }

        protected void OnDisable()
        {
            ViewIsDisabled();
        }

        protected void OnDestroy()
        {
            ViewWillUnload();
        }

        #endregion MonoBehaviour

        #region Protected

        /// <summary>
        /// Will be called on Awake
        /// </summary>
        protected virtual void ViewWillLoad()
        {
        }

        protected virtual void ViewDidLoad()
        {
        }

        /// <summary>
        /// Will be called on OnEnable
        /// </summary>
        protected virtual void ViewIsEnabled()
        {
        }

        /// <summary>
        /// Will be called on OnDisable
        /// </summary>
        protected virtual void ViewIsDisabled()
        {
        }

        /// <summary>
        /// Will be called on OnDestroy
        /// </summary>
        protected virtual void ViewWillUnload()
        {
        }

        #endregion Protected

        #region Private

        private void GetRootWindow()
        {
            m_window = GetComponentInParent<UIWindow>();
        }

        private void GetView()
        {
            Transform child = transform.GetChild( 0 );
            if ( child )
            {
                m_view = child.GetComponent<View>();
            }
        }

        /// <summary>
        /// Create a canvas group if there is none
        /// </summary>
        private void GetCanvasGroup()
        {
            m_canvasGroup = GetComponent<CanvasGroup>();

            if ( m_canvasGroup == null )
            {
                m_canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
        }

        /// <summary>
        /// Coroutine: Fade alpha to a certain value
        /// </summary>
        /// <param name="value"></param>
        /// <param name="duration"></param>
        /// <param name="onComplete"></param>
        /// <returns></returns>
        private IEnumerator FadeOpacityTo( float value, float duration, [CanBeNull] Action onComplete )
        {
            m_isFading = true;

            var t = 0f;
            var alpha = m_canvasGroup.alpha;
            var startAlpha = alpha;
            var deltaAlpha = value - alpha;

            while ( t <= 1 )
            {
                t += Time.deltaTime / duration;
                m_canvasGroup.alpha = startAlpha + t * deltaAlpha;

                yield return null;
            }

            m_canvasGroup.alpha = value;

            m_isFading = false;

            onComplete?.Invoke();
        }

        #endregion Private
    }
}
