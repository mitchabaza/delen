using System;
using Delen.Agent.Communication;
using log4net;
using log4net.Core;

namespace Delen.Agent
{

    public class DefaultLogger : ILogger
    {
        private readonly ILog _inner;
        private readonly IServerChannel _server;

        public DefaultLogger(ILog inner, IServerChannel server)
        {
            _inner = inner;
            _server = server;
        }

        public void WriteToConsole(string output)
        {
            System.Console.WriteLine(output);
            _server.SendProgress(output);
        }

        public log4net.Core.ILogger Logger
        {
            get { return _inner.Logger; }
        }

        public void Debug(object message)
        {
            _inner.Debug(message);
        }

        public void Debug(object message, Exception exception)
        {
            _inner.Debug(message, exception);
        }

        public void DebugFormat(string format, params object[] args)
        {
            _inner.DebugFormat(format, args);
        }

        public void DebugFormat(string format, object arg0)
        {
            _inner.DebugFormat(format, arg0);
        }

        public void DebugFormat(string format, object arg0, object arg1)
        {
            _inner.DebugFormat(format, arg0, arg1);
        }

        public void DebugFormat(string format, object arg0, object arg1, object arg2)
        {
            _inner.DebugFormat(format, arg0, arg1, arg2);
        }

        public void DebugFormat(IFormatProvider provider, string format, params object[] args)
        {
            _inner.DebugFormat(provider, format, args);
        }

        public void Info(object message)
        {
            _inner.Info(message);
        }

        public void Info(object message, Exception exception)
        {
            _inner.Info(message, exception);
        }

        public void InfoFormat(string format, params object[] args)
        {
            _inner.InfoFormat(format, args);
        }

        public void InfoFormat(string format, object arg0)
        {
            _inner.InfoFormat(format, arg0);
        }

        public void InfoFormat(string format, object arg0, object arg1)
        {
            _inner.InfoFormat(format, arg0, arg1);
        }

        public void InfoFormat(string format, object arg0, object arg1, object arg2)
        {
            _inner.InfoFormat(format, arg0, arg1, arg2);
        }

        public void InfoFormat(IFormatProvider provider, string format, params object[] args)
        {
            _inner.InfoFormat(provider, format, args);
        }

        public void Warn(object message)
        {
            _inner.Warn(message);
        }

        public void Warn(object message, Exception exception)
        {
            _inner.Warn(message, exception);
        }

        public void WarnFormat(string format, params object[] args)
        {
            _inner.WarnFormat(format, args);
        }

        public void WarnFormat(string format, object arg0)
        {
            _inner.WarnFormat(format, arg0);
        }

        public void WarnFormat(string format, object arg0, object arg1)
        {
            _inner.WarnFormat(format, arg0, arg1);
        }

        public void WarnFormat(string format, object arg0, object arg1, object arg2)
        {
            _inner.WarnFormat(format, arg0, arg1, arg2);
        }

        public void WarnFormat(IFormatProvider provider, string format, params object[] args)
        {
            _inner.WarnFormat(provider, format, args);
        }

        public void Error(object message)
        {
            _inner.Error(message);
        }

        public void Error(object message, Exception exception)
        {
            _inner.Error(message, exception);
        }

        public void ErrorFormat(string format, params object[] args)
        {
            _inner.ErrorFormat(format, args);
        }

        public void ErrorFormat(string format, object arg0)
        {
            _inner.ErrorFormat(format, arg0);
        }

        public void ErrorFormat(string format, object arg0, object arg1)
        {
            _inner.ErrorFormat(format, arg0, arg1);
        }

        public void ErrorFormat(string format, object arg0, object arg1, object arg2)
        {
            _inner.ErrorFormat(format, arg0, arg1, arg2);
        }

        public void ErrorFormat(IFormatProvider provider, string format, params object[] args)
        {
            _inner.ErrorFormat(provider, format, args);
        }

        public void Fatal(object message)
        {
            _inner.Fatal(message);
        }

        public void Fatal(object message, Exception exception)
        {
            _inner.Fatal(message, exception);
        }

        public void FatalFormat(string format, params object[] args)
        {
            _inner.FatalFormat(format, args);
        }

        public void FatalFormat(string format, object arg0)
        {
            _inner.FatalFormat(format, arg0);
        }

        public void FatalFormat(string format, object arg0, object arg1)
        {
            _inner.FatalFormat(format, arg0, arg1);
        }

        public void FatalFormat(string format, object arg0, object arg1, object arg2)
        {
            _inner.FatalFormat(format, arg0, arg1, arg2);
        }

        public void FatalFormat(IFormatProvider provider, string format, params object[] args)
        {
            _inner.FatalFormat(provider, format, args);
        }

        public bool IsDebugEnabled
        {
            get { return _inner.IsDebugEnabled; }
        }

        public bool IsInfoEnabled
        {
            get { return _inner.IsInfoEnabled; }
        }

        public bool IsWarnEnabled
        {
            get { return _inner.IsWarnEnabled; }
        }

        public bool IsErrorEnabled
        {
            get { return _inner.IsErrorEnabled; }
        }

        public bool IsFatalEnabled
        {
            get { return _inner.IsFatalEnabled; }
        }
    }
}