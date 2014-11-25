using System;
using System.Web;
using System.Web.SessionState;

namespace Lanban
{
    /// Async handler for request from client
    public class ChartHandler : IHttpAsyncHandler, IReadOnlySessionState
    {
        public bool IsReusable { get { return false; } }

        public IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback callback, Object state)
        {
            ChartHandlerOperation operation = new ChartHandlerOperation(callback, context, state);
            operation.QueueWork();
            return operation;
        }

        public void EndProcessRequest(IAsyncResult result)
        {

        }

        public void ProcessRequest(HttpContext context)
        {
            throw new InvalidOperationException();
        }
    }
}