using System;
using System.Collections.Generic;
using System.Diagnostics;
using Lib.Log;

namespace Lib.Base
{
    public static class Cmd
    {
        private static readonly LogWraper Log = new LogWraper("Cmd");

        public static void Regsvr32(string fullPath, bool silently = false)
        {
            //'/s' : Specifies regsvr32 to run silently and to not display any message boxes.
            //string arg_fileinfo = "/s \"" + fullPath + "\"";
            string arg_fileinfo = silently
                ? "/s \"" + fullPath + "\""
                : "\"" + fullPath + "\"";
            RunFile("regsvr32.exe", arg_fileinfo);
        }

        /// <summary>
        /// Run a CMD.
        /// </summary>
        /// <param name="cmd">cmd.</param>
        /// <param name="param">param.</param>
        /// <param name="silently">whether silently.</param>
        /// <param name="waitForExit">whether wait for exit.</param>
        /// <returns>Return the output string.</returns>
        public static string RunFile(string cmd, string param, bool silently = true, bool waitForExit = true)
        {
            try
            {
                using (Process process = new Process())
                {
                    process.StartInfo.FileName = cmd;
                    process.StartInfo.Arguments = param;
                    process.StartInfo.UseShellExecute = !silently;
                    process.StartInfo.CreateNoWindow = silently;
                    process.StartInfo.RedirectStandardOutput = silently;
                    process.Start();
                    if (waitForExit)
                    {
                        process.WaitForExit();
                    }
                    return process.StandardOutput.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                Log.Error("RunFile, failed.", ex);
                return "";
            }
        }

        public static void Run(string cmd, string param, bool silently = true, bool waitForExit = true)
        {
            try
            {
                using (Process process = new Process())
                {
                    process.StartInfo.FileName = cmd;
                    process.StartInfo.Arguments = param;
                    process.StartInfo.UseShellExecute = !silently;
                    process.StartInfo.CreateNoWindow = silently;
                    process.StartInfo.RedirectStandardOutput = silently;
                    process.Start();
                    if (waitForExit)
                    {
                        process.WaitForExit();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("RunFile, failed.", ex);
            }
        }

        /// <summary>
        /// Run a lot of DOS commands.
        /// </summary>
        /// <param name="cmds">Commands.</param>
        /// <returns>Return the output string.</returns>
        public static string RunCmds(List<string> cmds)
        {
            try
            {
                using (Process process = new Process())
                {
                    process.StartInfo.FileName = "cmd.exe";
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.RedirectStandardInput = true;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.RedirectStandardError = true;
                    process.StartInfo.CreateNoWindow = true;
                    process.Start();
                    process.StandardInput.AutoFlush = true;
                    for (int i = 0; i < cmds.Count; i++)
                        process.StandardInput.WriteLine(cmds[i]);
                    process.StandardInput.WriteLine("exit");
                    string retStr = process.StandardOutput.ReadToEnd();
                    process.WaitForExit();
                    return retStr;
                }
            }
            catch (Exception ex)
            {
                Log.Error("RunCmds, failed.", ex);
                return "";
            }
        }

        public static string RunCmd(string cmd)
        {
            try
            {
                var proc = new Process();
                proc.StartInfo.CreateNoWindow = true;
                proc.StartInfo.FileName = "cmd.exe";
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.RedirectStandardError = true;
                proc.StartInfo.RedirectStandardInput = true;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.Start();
                proc.StandardInput.WriteLine(cmd);
                proc.StandardInput.WriteLine("exit");
                proc.WaitForExit();
                string sOutput = proc.StandardOutput.ReadToEnd();
                return sOutput;
            }
            catch (Exception ex)
            {
                Log.Error("RunCmd, failed.", ex);
                return "";
            }
        }

        public static bool Ping(string targetIp, int size, bool dontFragment, out string inputStr, out string outputStr)
        {
            if (dontFragment)
                inputStr = string.Format("ping {0} -l {1} -f -n 1", targetIp, size);
            else
                inputStr = string.Format("ping {0} -l {1} -n 1", targetIp, size);

            outputStr = RunCmd(inputStr);
            if (outputStr != "" && outputStr.Contains("TTL="))
                return true;
            return false;

            /*
            - [Ok] --------------------------------------------------------------------
            Pinging 192.168.173.225 from 192.168.173.104 with 8000 bytes of data:
            Reply from 192.168.173.225: bytes=8000 time<1ms TTL=255

            Ping statistics for 192.168.173.225:
                Packets: Sent = 1, Received = 1, Lost = 0 (0% loss),
            Approximate round trip times in milli-seconds:
                Minimum = 0ms, Maximum = 0ms, Average = 0ms
            
            - [Ok] --------------------------------------------------------------------
            Pinging 192.168.173.225 from 192.168.173.104 with 32 bytes of data:
            Reply from 192.168.173.225: bytes=32 time<1ms TTL=255

            Ping statistics for 192.168.173.225:
                Packets: Sent = 1, Received = 1, Lost = 0 (0% loss),
            Approximate round trip times in milli-seconds:
                Minimum = 0ms, Maximum = 0ms, Average = 0ms
            
            - [Needs to be fragmented] --------------------------------------------------------------------
            Pinging 192.168.173.225 from 192.168.173.104 with 60000 bytes of data:
            Packet needs to be fragmented but DF set.

            Ping statistics for 192.168.173.225:
                Packets: Sent = 1, Received = 0, Lost = 1 (100% loss),
            
            - [Destination host unreachable] --------------------------------------------------------------------
            Pinging 192.168.173.229 from 192.168.173.104 with 60000 bytes of data:
            Reply from 192.168.173.104: Destination host unreachable.

            Ping statistics for 192.168.173.229:
                Packets: Sent = 1, Received = 1, Lost = 0 (0% loss),
            
            - [Request timed out] --------------------------------------------------------------------
            Pinging 12.168.173.225 from 192.168.173.104 with 32 bytes of data:
            Request timed out.

            Ping statistics for 12.168.173.225:
                Packets: Sent = 1, Received = 0, Lost = 1 (100% loss),
             */
        }
    }
}