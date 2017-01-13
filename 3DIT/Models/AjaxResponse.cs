using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _3DIT.Models
{
	public class AjaxResponse
	{
		public bool Status { get; set; }

		public string Message { get; set; }

		public List<object> Objects { get; set; }

		public AjaxResponse()
		{
			Objects = new List<object>();
		}

		public AjaxResponse(bool status)
		{
			Objects = new List<object>();
			Status = status;
		}

		public AjaxResponse(bool status, string message)
		{
			Objects = new List<object>();
			Status = status;
			Message = message;
		}
	}
}