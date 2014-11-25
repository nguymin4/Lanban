using System;
using System.Web;
using System.Web.SessionState;

namespace Lanban
{
    /// Async handler for request from client
    public class FileHandler : IHttpAsyncHandler, IReadOnlySessionState
    {
        public bool IsReusable { get { return false; } }

        public IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback callback, Object state)
        {
            FileHandlerOperation operation = new FileHandlerOperation(callback, context, state);
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