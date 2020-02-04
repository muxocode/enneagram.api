using System;
using System.Threading.Tasks;

namespace model.services
{
    internal class ErrorHandler : mxcd.core.services.IErrorHandler
    {
        public Task Trace(string message, Exception exception = null)
        {
            return Task.Run(() => { });
        }

        public Task Trace(Exception exception)
        {
            return Task.Run(() => { });
        }
    }
}
