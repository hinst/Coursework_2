#define DEBUG_WRITE

using System;
using System.Threading;
using System.Globalization;

using NLog;

using MyCSharp;

namespace CentralProject
{

	class MainApplication
	{

		/// <summary>
		/// Main entry point
		/// </summary>
		/// <param name="args"></param>
		[STAThread]
		static void Main(string[] args)
		{
			var application = new MainApplication(args);
			application.Run();
		}

		protected static Logger log = LogManager.GetCurrentClassLogger();

		public MainApplication(string[] args)
		{
		}

		public void Run()
		{
			RunSafe();
		}

		private void RunSafe()
		{
			try
			{
				RunInternal();
			}
			catch (Exception exception)
			{
				log.Error(exception.GetExceptionDescriptionAsText());
				Assert.NotCritical(exception);
			}
		}

		private void RunInternal()
		{
			log.Info("[>>>] Starting application...");
			CurrentThreadCulture.SetEnglish();
			RunPresentation();
			log.Info("[XXX] Exiting application...");
		}

		/// <summary> Run Windows Presentation Foundation application </summary>
		public void RunPresentation()
		{
			var presentation = new PresentationApplication();
			presentation.Run();
		}

	}

}
