#region Copyright (c) 2006-2013 nHydrate.org, All Rights Reserved
// -------------------------------------------------------------------------- *
//                           NHYDRATE.ORG                                     *
//              Copyright (c) 2006-2013 All Rights reserved                   *
//                                                                            *
//                                                                            *
// Permission is hereby granted, free of charge, to any person obtaining a    *
// copy of this software and associated documentation files (the "Software"), *
// to deal in the Software without restriction, including without limitation  *
// the rights to use, copy, modify, merge, publish, distribute, sublicense,   *
// and/or sell copies of the Software, and to permit persons to whom the      *
// Software is furnished to do so, subject to the following conditions:       *
//                                                                            *
// The above copyright notice and this permission notice shall be included    *
// in all copies or substantial portions of the Software.                     *
//                                                                            *
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,            *
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES            *
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.  *
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY       *
// CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,       *
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE          *
// SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.                     *
// -------------------------------------------------------------------------- *
#endregion
using System;
using System.Threading;
using System.Windows.Forms;
using nHydrate.Generator.Common.Logging;

namespace nHydrate.Generator.Common.Exceptions
{
	public class ThreadExceptionHandler
	{

		public void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
		{
			nHydrateLog.LogVerbose("Call: Application_ThreadException(object sender, ThreadExceptionEventArgs e)");
			try
			{
				ShowThreadExceptionDialog(e.Exception);
			}
			catch
			{
				try
				{
					MessageBox.Show("Fatal Error", 
						"Fatal Error",
						MessageBoxButtons.OK, 
						MessageBoxIcon.Stop);
				}
				finally
				{
					Application.Exit();
				}
			}
		}

		public void Application_ThreadException(object sender, UnhandledExceptionEventArgs e)
		{
			nHydrateLog.LogVerbose("Call: Application_ThreadException(object sender, UnhandledExceptionEventArgs e)");
			try
			{
				if (e.IsTerminating)
				{
					nHydrateLog.LogError(e.ExceptionObject.ToString());
				}
				else
				{
					ShowThreadExceptionDialog(e.ExceptionObject);
				}
			}
			catch
			{
				try
				{
					MessageBox.Show("Fatal Error",
						"Fatal Error",
						MessageBoxButtons.OK,
						MessageBoxIcon.Stop);
				}
				finally
				{
					Application.Exit();
				}
			}
		}


		private void ShowThreadExceptionDialog(object ex)
		{
			var newLine = Environment.NewLine;
			var errorMessage = String.Empty;
			if (ex.GetType().IsAssignableFrom(typeof(System.Exception))) 
			{
				var systemException = (Exception)ex;
				errorMessage = "Unhandled Exception: " + systemException.Message + newLine +
				"Exception Type: " + systemException.GetType() + newLine +
				"Stack Trace:" + newLine +
				systemException.StackTrace;
			}
			else
			{
				errorMessage = ex.ToString();
			}

			var exceptionForm = new ThreadExceptionHandlerForm(errorMessage);
			if (exceptionForm.ShowDialog() == DialogResult.Abort)
			{
				Application.Exit();
			}
		}
	} 
}

