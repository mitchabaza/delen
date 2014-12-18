using System;
using Raven.Abstractions.Data;

namespace Delen.Agent.Tests
{
    public class DocumentObserver : IObserver<DocumentChangeNotification>
    {
        private readonly Action<DocumentChangeNotification> _action;

        public DocumentObserver(Action<DocumentChangeNotification> action)
        {
            _action = action;
        }


        public void OnNext(DocumentChangeNotification value)
        {
            _action(value);
        }

        public void OnError(Exception error)
        {
        }

        public void OnCompleted()
        {
        }
    }
}