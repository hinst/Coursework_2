#define DEBUG_WRITE

using System;
using System.Threading;
using System.Globalization;

namespace CentralProject
{

	class MainApplication
	{

		/// <summary>
		/// Main entry point
		/// </summary>
		/// <param name="args"></param>
		static void Main(string[] args)
		{
		}

		public MainApplication(string[] args)
		{
			Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-us");
			Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en-us");
		}

	}

}
