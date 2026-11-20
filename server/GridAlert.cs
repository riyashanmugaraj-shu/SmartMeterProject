using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace server
{
    // Base class for grid alerts (potential to add more)
    // Creator for factory pattern
    public abstract class GridAlert
    {
        // Returns the alert message that will be pushed to clients
        public abstract string GetMessage();
    }
}