using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DevSnip.Controllers
{
    /* Terminal - small amount of code but dam it took me a while to figure it out.
     * Only problem is when the terminal is promting the user for input we currently have no idea if thats happening.
     * Some ideas might be dispatch timer and checking thread states.
     * https://stackoverflow.com/questions/8978512/process-diagnostic-waiting-for-user-input
     * https://stackoverflow.com/questions/1704791/is-my-process-waiting-for-input
     */
    class Terminal
    {
        private Process process; //hold the terminal process
        Action<string,string> outPutHandler; //a delegate from the main window to do somehting with the terminals output
        public List<string> commandHistory;
        
        public Terminal(Action<string,string> outPutCallback)
        {
            commandHistory = new List<string> { "process started" };
            outPutHandler = outPutCallback;
        }

        public void StartProcess(string fileName="cmd.exe", string arguments="", bool hidden = true)
        {
            process = new Process();
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.CreateNoWindow = hidden;
            process.StartInfo.WorkingDirectory = @"C:\";
            process.StartInfo.FileName = fileName; //Path.Combine(Environment.SystemDirectory, "cmd.exe");

            // Redirects the standard input so that commands can be sent to the shell.
            process.StartInfo.RedirectStandardInput = true;

            //output event handlers
            process.OutputDataReceived += ProcessOutputDataHandler;
            process.ErrorDataReceived += ProcessErrorDataHandler;

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
        }

        public void sendCommand(string command)
        {
            process.StandardInput.WriteLine(command);
            commandHistory.Add(command);
        }

        private void ProcessErrorDataHandler(object sender, DataReceivedEventArgs e)
        {
            //the UI lives in another thread so we ahve to wrap the delegate in a dsipatcher
            Application.Current.Dispatcher.Invoke(() =>
            {
                outPutHandler(e.Data,"");
            });
        }

        private void ProcessOutputDataHandler(object sender, DataReceivedEventArgs e)
        {
            var str = "";// e.Data.ToString();
            var output = e.Data.ToString();
            foreach (ProcessThread thread in process.Threads)
            {
                if (//true
                    thread.ThreadState == System.Diagnostics.ThreadState.Wait
                    && thread.WaitReason == ThreadWaitReason.UserRequest
                    )
                {
                    str += Environment.NewLine + thread.ToString() + " - " + thread.WaitReason + Environment.NewLine;
                }
                //if () output += Environment.NewLine + "User was prompted for input such as password and I was unable to respond";
                //var last = commandHistory[commandHistory.Count() - 1];
                //if(str.Contains(last)) output += Environment.NewLine + "User was prompted for input such as password and I was unable to respond";
            }
            

            //the UI lives in another thread so we ahve to wrap the delegate in a dsipatcher
            Application.Current.Dispatcher.Invoke(() =>
            {
                outPutHandler(output, str);
            });
        }

    }
}
