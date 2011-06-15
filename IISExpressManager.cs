using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnvDTE80;
using System.IO;
using System.Diagnostics;

namespace IISExpress
{
    /// <summary>
    /// Management class for IIS Express
    /// </summary>
    public class IISExpressManager
    {
        private const string CustomOutputWindowPaneName = "IIS Express Manager";
        private DTE2 _dte;

        /// <summary>
        /// Initializes a new instance of the <see cref="IISExpressManager"/> class.
        /// </summary>
        /// <param name="dte">The DTE.</param>
        public IISExpressManager(DTE2 dte)
        {
            this._dte = dte;
        }

        /// <summary>
        /// Attaches all processes.
        /// </summary>
        public void AttachAllProcesses()
        {
            foreach (EnvDTE.Process process in this._dte.Debugger.LocalProcesses)
            {
                if (process.Name.Contains("iisexpress.exe")) // || process.Name.Contains("w3wp.exe"))
                {
                    process.Attach();
                }
            }
        }

        /// <summary>
        /// Starts the IIS express.
        /// </summary>
        public void StartIISExpress()
        {
            EnvDTE.OutputWindow outputWindow = _dte.Windows.Item(EnvDTE.Constants.vsWindowKindOutput).Object as EnvDTE.OutputWindow;
            EnvDTE.OutputWindowPane outputPane = null;

            foreach (EnvDTE.OutputWindowPane pane in outputWindow.OutputWindowPanes)
            {
                if (pane.Name == CustomOutputWindowPaneName)
                {
                    outputPane = pane;
                    break;
                }
            }

            if (outputPane == null)
            {
                outputPane = outputWindow.OutputWindowPanes.Add(CustomOutputWindowPaneName);
            }

            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                System.Diagnostics.Process iisxProcess = new System.Diagnostics.Process();
                iisxProcess.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                iisxProcess.StartInfo.CreateNoWindow = true;
                iisxProcess.StartInfo.UseShellExecute = false;
                iisxProcess.StartInfo.RedirectStandardOutput = true;
                iisxProcess.StartInfo.RedirectStandardError = true;
                iisxProcess.StartInfo.FileName = @"C:\Program Files (x86)\IIS Express\iisexpress.exe";
                iisxProcess.StartInfo.Arguments = @"/site:Mmm.Informatics.UI";

                iisxProcess.Start();

                while (!iisxProcess.HasExited)
                {
                    outputPane.OutputString(iisxProcess.StandardOutput.ReadLine() + Environment.NewLine);
                }
            });
        }

        /// <summary>
        /// Stops the IIS express.
        /// </summary>
        public void StopIISExpress()
        {
            var processNames = new[] { "iisexpress" };

            foreach (var runningProcess in processNames.Select(Process.GetProcessesByName).SelectMany(p => p))
            {
                runningProcess.Kill();
            }
        }
    }
}
