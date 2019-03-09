using UnityEngine;
using UnityEngine.UI;

namespace Nova
{
    [RequireComponent( typeof( Button ) )]
    public class UIButton : UIView
    {
        public Button Button { get; private set; }

        protected override void Awake()
        {
            base.Awake();

            Button = GetComponent<Button>();
        }
    }
}
