using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace server
{
    // Alert is raised when planned maintenance is scheduled
    // Concrete creator
    public class MaintenanceAlert : GridAlert
    {
        public override string GetMessage()
        {
            return "Electricity grid maintenance is scheduled.";
        }
    }
}