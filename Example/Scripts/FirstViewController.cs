using Nova;
using System;
using System.Linq;

namespace Example
{
    internal sealed class FirstViewController : UIViewController
    {
        public void Push()
        {
            if ( NavigationController != null )
            {
                NavigationController.Push<SecondViewController>( preparation: controller =>
                {
                    controller.StuffToDisplay = RandomString( 10 );
                } );
            }
        }

        private static Random s_random = new Random();

        private static string RandomString( int length )
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string( Enumerable.Repeat( chars, length )
                .Select( s => s[s_random.Next( s.Length )] ).ToArray() );
        }
    }
}
