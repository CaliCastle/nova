using System.Collections;
using Nova;
using UnityEngine;
using UnityEngine.UI;

namespace Example
{
    internal sealed class SecondViewController : UIViewController
    {
        [HideInInspector]
        public string StuffToDisplay;

        [SerializeField]
        private Text m_text;

        protected override void ViewDidLoad()
        {
            base.ViewDidLoad();

            m_text.text = StuffToDisplay;

            StartCoroutine( Hide() );
        }

        private IEnumerator Hide()
        {
            yield return new WaitForSeconds( 2f );

            NavigationController?.Pop();
        }
    }
}
