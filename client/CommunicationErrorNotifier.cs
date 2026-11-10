using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace client
{
    public class CommunicationErrorNotifier
    {
        private readonly List<ICommunicationErrorObserver> _observers = new();

        public void Attach(ICommunicationErrorObserver observer)
        {
            if (!_observers.Contains(observer))
            {
                _observers.Add(observer);
            }
        }

        public void Detach(ICommunicationErrorObserver observer)
        {
            _observers.Remove(observer);
        }

        public void Notify(string message)
        {
            foreach (var observer in _observers)
            {
                observer.OnCommunicationError(message);
            }
        }
    }
}