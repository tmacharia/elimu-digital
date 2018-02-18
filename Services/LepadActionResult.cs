using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Services
{
    public class LepadActionResult : ActionResult
    {
        private HttpStatusCode _code;
        private object _obj;

        public LepadActionResult()
        {
            
        }
        public LepadActionResult(HttpStatusCode code)
        {
            _code = code;
        }

        public HttpStatusCode Code
        {
            get
            {
                return _code;
            }
            set
            {
                _code = value;
            }
        }
        public object Data
        {
            get
            {
                return _obj;
            }
            set
            {
                _obj = value;
            }
        }
        public byte[] Buffer
        {
            get
            {
                return Utils.ToByteArray(Data);
            }
        }

        public override void ExecuteResult(ActionContext context)
        {
            base.ExecuteResult(context);

            context.HttpContext.Response.StatusCode = (int)_code;
            context.HttpContext.Response.Body.Write(Buffer, 0, Buffer.Length);
        }
    }
}
