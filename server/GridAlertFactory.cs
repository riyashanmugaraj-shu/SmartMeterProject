using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace server
{
    // Factory Pattern for creating alert objects at random times
    internal static class GridAlertFactory
    {
        private static readonly Random _random = new Random();

        public static GridAlert CreateAlert()
        {
            int alertType = _random.Next(1, 3);

            switch (alertType)
            {
                case 1:
                    return new HighDemandAlert();

                case 2:
                    return new MaintenanceAlert();

                default:
                    return new HighDemandAlert();
            }
        }
    }
}