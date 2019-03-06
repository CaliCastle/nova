using UnityEngine;

namespace Nova
{
    public class View : MonoBehaviour
    {
        public Transform Background => m_background;
        public Transform SafeArea => m_safeArea;
        public Transform Foreground => m_foreground;

        [SerializeField]
        private Transform m_background;

        [SerializeField]
        private Transform m_safeArea;

        [SerializeField]
        private Transform m_foreground;

        /// <summary>
        /// Inject dependencies
        /// </summary>
        /// <param name="background"></param>
        /// <param name="safeArea"></param>
        /// <param name="foreground"></param>
        public void Inject( Transform background, Transform safeArea, Transform foreground )
        {
            m_background = background;
            m_safeArea = safeArea;
            m_foreground = foreground;
        }

        private void Awake()
        {
            if ( !SanityCheck() )
            {
                Debug.LogError( $"<b>{gameObject.name}</b> have null fields!" );
            }
        }

        private void Reset()
        {
            Inject( transform.GetChild( 0 ), transform.GetChild( 1 ), transform.GetChild( 2 ) );
        }

        private bool SanityCheck() => m_background != null && m_safeArea != null && m_foreground != null;
    }
}
