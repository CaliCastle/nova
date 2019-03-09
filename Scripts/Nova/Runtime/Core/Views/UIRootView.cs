using UnityEngine;

namespace Nova
{
    [RequireComponent( typeof( Canvas ) )]
    public class UIRootView : UIView
    {
        public Canvas Canvas { get; private set; }

        protected override void Awake()
        {
            base.Awake();

            Canvas = GetComponent<Canvas>();
            if ( Canvas == null )
            {
                Canvas = gameObject.AddComponent<Canvas>();
            }
        }
    }
}
