using UnityEngine;

namespace Nova
{
    public sealed class SafeAreaSimulation : MonoBehaviour
    {
        public SafeArea.SimDevice Device;

        private void Awake()
        {
            if ( !Application.isEditor )
            {
                Destroy( gameObject );
            }
        }

        private void Update()
        {
            if ( !Equals( Device, SafeArea.Sim ) )
            {
                SafeArea.Sim = Device;
            }
        }
    }
}
