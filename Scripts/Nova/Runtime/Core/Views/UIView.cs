using UnityEngine;

namespace Nova
{
    [RequireComponent( typeof( CanvasGroup ) )]
    public class UIView : MonoBehaviour
    {
        public CanvasGroup CanvasGroup { get; private set; }

        public bool Hidden
        {
            get => CanvasGroup != null && Equals( CanvasGroup.alpha, 0f ) && !CanvasGroup.interactable;
            set
            {
                if ( CanvasGroup != null )
                {
                    CanvasGroup.alpha = value ? 0f : 1f;
                    CanvasGroup.interactable = !value;
                }
            }
        }

        protected virtual void Awake()
        {
            CanvasGroup = GetComponent<CanvasGroup>();

            if ( CanvasGroup == null )
            {
                CanvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
        }

        protected virtual void Start()
        {
        }
    }
}
