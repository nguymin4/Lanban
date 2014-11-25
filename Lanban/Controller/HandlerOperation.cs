using System;
using System.Threading;
using System.Web;
using System.Web.SessionState;

namespace Lanban
{
    public class HandlerOperation : IAsyncResult, IReadOnlySessionState
    {
        protected bool _completed;
        protected Object _state;
        protected AsyncCallback _callback;
        protected HttpContext _context;

        bool IAsyncResult.IsCompleted { get { return _completed; } }
        WaitHandle IAsyncResult.AsyncWaitHandle { get { return null; } }
        Object IAsyncResult.AsyncState { get { return _state; } }
        bool IAsyncResult.CompletedSynchronously { get { return false; } }

        protected string action;
        protected string errorPage = "/404/404.html";
        protected string result = "";

        public HandlerOperation(AsyncCallback callback, HttpContext context, Object state)
        {
            _callback = callback;
            _context = context;
            _state = state;
            _completed = false;
            
            action = _context.Request.Params["action"];
            _context.Response.ContentType = "text/plain";
        }

        protected void FinishWork()
        {
            _context.Response.Write(result);
            _completed = true;
            _callback(this);
        }

        protected void RedirectPage(string url)
        {
            _context.Response.Redirect(url);
        }
    }
}