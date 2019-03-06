using Nova;
using UnityEngine;

namespace Example
{
    public sealed class Example : MonoBehaviour, INovaLaunchable
    {
        public void LiftOff( UIWindow window )
        {
            window.Present<ExampleNavigationController>( false );
        }
    }
}
