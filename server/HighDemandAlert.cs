using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace server
{
    // Alert representing high demand on the electricity grid
    // Concrete creator 
    public class HighDemandAlert : GridAlert
    {
        // Message displayed to clients when high demand is detected
        public override string GetMessage()
        {
            return "High electricity demand detected in your area.";
        }
    }
}