﻿using System;
using System.Diagnostics;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using NLog;

namespace BitWhiskey
{
    static class Program
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {

            Application.ThreadException +=
            new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);

#if (!DEBUG)
      Application.SetUnhandledExceptionMode(UnhandledExceptionMode.ThrowException);
#endif

            // Add handler to handle the exception raised by additional threads
            AppDomain.CurrentDomain.UnhandledException +=
            new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            using (var mutex = new Mutex(false, "BitWhiskey"))
            {
                if (!mutex.WaitOne(TimeSpan.FromSeconds(1), false))
                {
                    Helper.Display("Copy of BitWhiskey is running! Exiting..");
                    return;
                }
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1());
            }

        }

        static void Application_ThreadException
               (object sender, System.Threading.ThreadExceptionEventArgs e)
        {// All exceptions thrown by the main thread are handled over this method

            logger.Error(e.Exception, "Thread Unhandled Exception!");
            //                MessageBox.Show(ex.ToString());
            Process proc = System.Diagnostics.Process.GetCurrentProcess();
            proc.Kill();

        }

        static void CurrentDomain_UnhandledException
            (object sender, UnhandledExceptionEventArgs e)
        {// All exceptions thrown by additional threads are handled in this method

            logger.Error(e.ExceptionObject);

            Process proc = System.Diagnostics.Process.GetCurrentProcess();
            proc.Kill();

        }

    }
}
