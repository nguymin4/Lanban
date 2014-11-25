using System;
using System.Web;
using System.Web.SessionState;

namespace Lanban
{
    /// Async handler for request from client
    public class ItemHandler : IHttpAsyncHandler, IReadOnlySessionState
    {
        public bool IsReusable { get { return false; } }

        public IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback callback, Object state)
        {
            ItemHandlerOperation operation = new ItemHandlerOperation(callback, context, state);
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