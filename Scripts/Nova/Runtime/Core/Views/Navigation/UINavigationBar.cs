using TMPro;
using UnityEngine;

namespace Nova
{
    public delegate void NavigationBackButtonDidTap();

    public class UINavigationBar : UIView
    {
        public UIButton BackButton => m_backButton;

        public TMP_Text TitleText => m_titleText;

        public NavigationBackButtonDidTap BackDidTap;

        [SerializeField]
        private UIButton m_backButton;

        [SerializeField]
        private TMP_Text m_titleText;

        public void SetTitle( string title )
        {
            if ( m_titleText )
            {
                m_titleText.text = title;
            }
        }

        protected override void Start()
        {
            base.Start();

            m_backButton.Button.onClick.AddListener( BackButtonDidTap );
        }

        private void BackButtonDidTap()
        {
            BackDidTap?.Invoke();
        }
    }
}
