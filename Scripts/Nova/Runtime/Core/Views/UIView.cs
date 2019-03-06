using UnityEngine;

namespace Nova
{
    [RequireComponent( typeof( Canvas ), typeof( CanvasGroup ) )]
    public class UIView : MonoBehaviour
    {
        public Canvas Canvas { get; private set; }

        public CanvasGroup CanvasGroup { get; private set; }

        private void Awake()
        {
            Canvas = GetComponent<Canvas>();
            CanvasGroup = GetComponent<CanvasGroup>();
        }
    }
}
