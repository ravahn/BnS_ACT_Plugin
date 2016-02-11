#region License
// ========================================================================
// BnS_ACT_Plugin.cs
// Advanced Combat Tracker Plugin for Blade & Soul
// https://github.com/ravahn/BnS_ACT_Plugin
// 
// The MIT License(MIT)
//
// Copyright(c) 2016 Ravahn
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// ========================================================================
#endregion

using System;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.IO;

namespace BNS_ACT_Plugin
{
    #region ACT Plugin Code
    public class BNS_ACT_Plugin : Advanced_Combat_Tracker.IActPluginV1
    {
        // reference to the ACT plugin status label
        private Label lblStatus = null;

        public void InitPlugin(System.Windows.Forms.TabPage pluginScreenSpace, System.Windows.Forms.Label pluginStatusText)
        {
            // Configure ACT for updates, and check for update.
            Advanced_Combat_Tracker.ActGlobals.oFormActMain.UpdateCheckClicked += new Advanced_Combat_Tracker.FormActMain.NullDelegate(UpdateCheckClicked);
            if (Advanced_Combat_Tracker.ActGlobals.oFormActMain.GetAutomaticUpdatesAllowed())
            {
                Thread updateThread = new Thread(new ThreadStart(UpdateCheckClicked));
                updateThread.IsBackground = true;
                updateThread.Start();
            }

            // Update the listing of columns inside ACT.
            UpdateACTTables();

            // store a reference to plugin's status label
            lblStatus = pluginStatusText;

            // character name cannot be parsed from logfile name
            Advanced_Combat_Tracker.ActGlobals.oFormActMain.LogPathHasCharName = false;
            Advanced_Combat_Tracker.ActGlobals.oFormActMain.LogFileFilter = "*.log"; 

            // Default Timestamp length, but this can be overridden in parser code.
            Advanced_Combat_Tracker.ActGlobals.oFormActMain.TimeStampLen = DateTime.Now.ToString("HH:mm:ss.fff").Length + 1;

            // Set Date time format parsing. 
            Advanced_Combat_Tracker.ActGlobals.oFormActMain.GetDateTimeFromLog = new Advanced_Combat_Tracker.FormActMain.DateTimeLogParser(LogParse.ParseLogDateTime);

            // Set primary parser delegate for processing data
            Advanced_Combat_Tracker.ActGlobals.oFormActMain.BeforeLogLineRead += LogParse.BeforeLogLineRead;

            // Initialize logging thread
            BNS_Log.Initialize();

            lblStatus.Text = "BnS Plugin Started.";
        }

        public void DeInitPlugin()
        {
            // remove event handler
            Advanced_Combat_Tracker.ActGlobals.oFormActMain.UpdateCheckClicked -= this.UpdateCheckClicked;
            Advanced_Combat_Tracker.ActGlobals.oFormActMain.BeforeLogLineRead -= LogParse.BeforeLogLineRead;

            BNS_Log.Uninitialize();

            if (lblStatus != null)
            {
                lblStatus.Text = "BnS Plugin Unloaded.";
                lblStatus = null;
            }
        }


        public void UpdateCheckClicked()
        {
            /*
            try
            {
                DateTime localDate = Advanced_Combat_Tracker.ActGlobals.oFormActMain.PluginGetSelfDateUtc(this);
                DateTime remoteDate = Advanced_Combat_Tracker.ActGlobals.oFormActMain.PluginGetRemoteDateUtc(m_PluginId);
                if (localDate.AddHours(2) < remoteDate)
                {
                    DialogResult result = MessageBox.Show("There is an updated version of the BnS Parsing Plugin.  Update it now?\n\n(If there is an update to ACT, you should click No and update ACT first.)", "New Version", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        Advanced_Combat_Tracker.ActPluginData pluginData = Advanced_Combat_Tracker.ActGlobals.oFormActMain.PluginGetSelfData(this);
                        System.IO.FileInfo updatedFile = Advanced_Combat_Tracker.ActGlobals.oFormActMain.PluginDownload(m_PluginId);
                        pluginData.pluginFile.Delete();
                        updatedFile.MoveTo(pluginData.pluginFile.FullName);
                        Advanced_Combat_Tracker.ThreadInvokes.CheckboxSetChecked(Advanced_Combat_Tracker.ActGlobals.oFormActMain, pluginData.cbEnabled, false);
                        Application.DoEvents();
                        Advanced_Combat_Tracker.ThreadInvokes.CheckboxSetChecked(Advanced_Combat_Tracker.ActGlobals.oFormActMain, pluginData.cbEnabled, true);
                    }
                }
            }
            catch (Exception ex)
            {
                Advanced_Combat_Tracker.ActGlobals.oFormActMain.WriteExceptionLog(ex, "BnS Plugin Update Check.");
            }*/
        }

        private void UpdateACTTables()
        {

        }
    }
    #endregion

    #region Memory Scanning code
    public static class BNS_Log
    {
        [DllImport("kernel32.dll")]
        internal static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, IntPtr nSize, ref IntPtr lpNumberOfBytesRead);

        private static Thread _thread = null;
        private static bool _stopThread = false;

        private static string _logFileName = "";

        public static void Initialize()
        {
            _stopThread = false;
            _thread = new Thread(new ThreadStart(Scan));

            string folderName = Path.Combine(Advanced_Combat_Tracker.ActGlobals.oFormActMain.AppDataFolder.FullName, @"BNSLogs\");

            if (!Directory.Exists(folderName))
                Directory.CreateDirectory(folderName);

            _logFileName = Path.Combine(folderName, "combatlog_" + DateTime.Now.ToString("yyyy-MM-dd") + ".log");

            File.AppendAllText(_logFileName, null);

            // update filename in ACT
            Advanced_Combat_Tracker.ActGlobals.oFormActMain.LogFilePath = _logFileName;
            Advanced_Combat_Tracker.ActGlobals.oFormActMain.OpenLog(false, false); // GetCurrentZone flag means it will scan for the zone regex in the log file.

            _thread.Start();
        }

        public static void Uninitialize()
        {
            if (_thread != null)
            {
                _stopThread = true;

                for (int i = 0; i < 10; i++)
                {
                    if (_thread.ThreadState == System.Threading.ThreadState.Stopped)
                        break;
                    System.Threading.Thread.Sleep(50);
                    Application.DoEvents();
                }

                if (_thread.ThreadState != System.Threading.ThreadState.Stopped)
                    _thread.Abort();

                _thread = null;
            }
        }

        private const Int32 chatlogOffset = 0x00d6d8b0;

        private static void Scan()
        {
            Process process = null;
            IntPtr baseAddress = IntPtr.Zero;
            IntPtr chatlogPointer = IntPtr.Zero;

            int lastLine = -1;


            while (!_stopThread)
            {
                System.Threading.Thread.Sleep(10);

                if (process == null)
                {
                    Process[] processList = Process.GetProcessesByName("Client");
                    if (processList != null && processList.Length > 0)
                        process = processList[0];
                    else
                        continue;

                    // todo: validate process

                    // cache base address if it is missing
                    if (baseAddress == IntPtr.Zero)
                        baseAddress = process.MainModule.BaseAddress;

                    // cache chatlog pointer tree
                    chatlogPointer = ReadIntPtr(process.Handle, IntPtr.Add(baseAddress, chatlogOffset));
                    chatlogPointer = ReadIntPtr(process.Handle, IntPtr.Add(chatlogPointer, 0x34));
                    chatlogPointer = ReadIntPtr(process.Handle, IntPtr.Add(chatlogPointer, 0x514));
                    chatlogPointer = ReadIntPtr(process.Handle, IntPtr.Add(chatlogPointer, 0x4));
                }
                if (process == null || baseAddress == IntPtr.Zero || chatlogPointer == IntPtr.Zero)
                    continue;

                //read in the # of lines - offset 0x9600
                int lineCount = ReadInt32(process.Handle, IntPtr.Add(chatlogPointer, 0x9600));

                if (lineCount > 300)
                    throw new ApplicationException("line count too high: [" + lineCount.ToString() + "].");

                if (lineCount == lastLine)
                    continue;

                // first scan - do not parse past data since we do not have timestamps
                if (lastLine == -1)
                {
                    lastLine = lineCount;
                    continue;
                }

                // check for wrap-around
                if (lineCount < lastLine)
                    lineCount += lastLine;

                StringBuilder buffer = new StringBuilder(20 * (lineCount - lastLine));

                for (int i = lastLine+1; i <= lineCount; i++)
                {
                    IntPtr lineHeaderPointer = IntPtr.Add(chatlogPointer, 4 + 0x80 * (i % 300));

                    byte[] header = new byte[0x80];

                    ReadBuffer(process.Handle, lineHeaderPointer, ref header, 0x80);

                    // first 4 bytes is a pointer
                    IntPtr linePointer = new IntPtr(BitConverter.ToUInt32(header, 0));

                    // # of bytes in the line is 4 bytes after that, int16
                    Int16 byteCount = header[0x10];

                    // read bytes
                    byte[] text = new byte[byteCount * 2];

                    ReadBuffer(process.Handle, linePointer, ref text, (int)(byteCount * 2));

                    //for (int j = 0; j < 0x80; j+=4)
                        //buffer.Append(BitConverter.ToUInt32(header, j).ToString("X8") + "|");
                    buffer.Append(DateTime.Now.ToString("HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture) + " ");
                    buffer.AppendLine(System.Text.UnicodeEncoding.Unicode.GetString(text, 0, byteCount * 2));

                }

                File.AppendAllText(_logFileName, buffer.ToString());

                lastLine = lineCount % 300;
            }
        }

        private static void ReadBuffer(IntPtr ProcessHandle, IntPtr Offset, ref Byte[] buffer, int Length)
        {
            IntPtr bytesRead = IntPtr.Zero;

            if (!ReadProcessMemory(ProcessHandle, Offset, buffer, new IntPtr(Length), ref bytesRead))
                throw new ApplicationException("ReadProcessMemory returned false.  Offset: [" + Offset.ToString("X8") + "], Length: [" + Length.ToString() + "].");

            if (bytesRead != (IntPtr)Length)
                throw new ApplicationException("ReadProcessMemory returned incorrect byte count.  Expected: [" + Length.ToString() + "].  Actual: [" + bytesRead.ToString() + "].");
        }

        private static Int32 ReadInt32(IntPtr ProcessHandle, IntPtr Offset)
        {
            const int dataSize = sizeof(Int32);
            byte[] buffer = new byte[dataSize];
            IntPtr bytesRead = IntPtr.Zero;

            if (!ReadProcessMemory(ProcessHandle, Offset, buffer, new IntPtr(dataSize), ref bytesRead))
                return 0;

            return BitConverter.ToInt32(buffer, 0);
        }

        private static IntPtr ReadIntPtr(IntPtr ProcessHandle, IntPtr Offset)
        {
            return new IntPtr(ReadInt32(ProcessHandle, Offset));
        }

    }
    #endregion

    #region Parser Code

    static class LogParse
    {
        private static Regex regex_incomingdamage1 = new Regex(@"(?<target>.*?)?( received|Received) (?<damage>\d+(,\d+)*) (?<critical>(Critical) )?damage (and (?<HPDrain>\d+(,\d+)*) HP drain )?(and (?<FocusDrain>\d+) Focus drain )?(and Knockback )?(and Daze )?from ((?<actor>.*?)&apos;s )?(?<skill>.*?)\.", RegexOptions.Compiled);
        private static Regex regex_incomingdamage2 = new Regex(@"(?<target>.*?)?(Blocked|blocked) (?<actor>.*?)&apos;s (?<skill>.*?) but received ((?<damage>\d+(,\d+)*) damage)?(?<debuff>.*?)?\.", RegexOptions.Compiled);
        private static Regex regex_incomingdamage3 = new Regex(@"(?<actor>.*?)&apos;s (?<skill>.*?) inflicted (?<damage>\d+(,\d+)*) damage( and Knockdown)?( and Daze)?( and Airborne)?( to (?<target>.*?))?\.", RegexOptions.Compiled);
        private static Regex regex_yourdamage = new Regex(@"(?<skill>.*?) (?<critical>(critically hit)|(hit)) (?<target>.*?) for (?<damage>\d+(,\d+)*) damage(((, draining| and drained) ((?<HPDrain>\d+(,\d+)*) HP)?( and )?((?<FocusDrain>\d+) Focus)?))?(, removing (?<skillremove>.*?))?\.", RegexOptions.Compiled);
        private static Regex regex_yourdebuff = new Regex(@"(?<skill>.*?) (?<critical>(critically hit)|(hit)) (?<target>.*?) (and inflicted (?<debuff>.*?))?(but (?<debuff2>.*?) was resisted)?\.", RegexOptions.Compiled);
        private static Regex regex_evade = new Regex(@"(?<target>.*?) evaded (?<skill>.*?)\.", RegexOptions.Compiled);
        private static Regex regex_yourdefeat = new Regex(@"(?<target>.*?) was defeated by (?<skill>.*?)\.", RegexOptions.Compiled);

        public static DateTime ParseLogDateTime(string message)
        {
            DateTime ret = DateTime.MinValue;

            if (message == null || message.IndexOf(' ') < 5)
                return ret;

            if (!DateTime.TryParse(message.Substring(0, message.IndexOf(' ')), out ret))
                return DateTime.MinValue;
            return ret;
        }

        public static void BeforeLogLineRead(bool isImport, Advanced_Combat_Tracker.LogLineEventArgs logInfo)
        {
            string logLine = logInfo.logLine;


            // parse datetime
            DateTime timestamp = ParseLogDateTime(logLine);
            if (logLine.IndexOf(' ') >= 5)
                logLine = logLine.Substring(logLine.IndexOf(' '));

            // reformat logline
            logInfo.logLine = "[" + timestamp.ToString("HH:mm:ss.fff") + "] " + logLine;
            // timestamp = DateTime.ParseExact(logLine.Substring(1, logLine.IndexOf(']') - 1), "HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture);

            Match m;

            m = regex_yourdamage.Match(logLine);
            if (m.Success)
            {
                if (Advanced_Combat_Tracker.ActGlobals.oFormActMain.SetEncounter(timestamp, "You", m.Groups["target"].Value))
                {
                    Advanced_Combat_Tracker.ActGlobals.oFormActMain.AddCombatAction(
                        (int)Advanced_Combat_Tracker.SwingTypeEnum.NonMelee,
                        m.Groups["critical"].Value == "critically hit",
                        "",
                        "You",
                        m.Groups["skill"].Value,
                        new Advanced_Combat_Tracker.Dnum(int.Parse(m.Groups["damage"].Value, System.Globalization.NumberStyles.AllowThousands)),
                        timestamp,
                        Advanced_Combat_Tracker.ActGlobals.oFormActMain.GlobalTimeSorter,
                        m.Groups["target"].Value,
                        "");

                }

                return;
            }

            m = regex_yourdebuff.Match(logLine);
            if (m.Success)
            {
                // todo: add debuff support
                return;
            }

            m = regex_evade.Match(logLine);
            if (m.Success)
            {
                // todo: add evade support
                return;
            }

            m = regex_yourdefeat.Match(logLine);
            if (m.Success)
            {
                if (Advanced_Combat_Tracker.ActGlobals.oFormActMain.SetEncounter(timestamp, "You", m.Groups["target"].Value))
                {
                    Advanced_Combat_Tracker.ActGlobals.oFormActMain.AddCombatAction(
                        (int)Advanced_Combat_Tracker.SwingTypeEnum.NonMelee,
                        false,
                        "",
                        "You",
                        m.Groups["skill"].Value,
                        Advanced_Combat_Tracker.Dnum.Death,
                        timestamp,
                        Advanced_Combat_Tracker.ActGlobals.oFormActMain.GlobalTimeSorter,
                        m.Groups["target"].Value,
                        "");

                }

                return;
            }

            m = regex_incomingdamage1.Match(logLine);
            if (!m.Success)
                m = regex_incomingdamage2.Match(logLine);
            if (!m.Success)
                m = regex_incomingdamage3.Match(logLine);
            if (m.Success)
            {
                string target = m.Groups["target"].Success ? m.Groups["target"].Value : "";
                string actor = m.Groups["actor"].Success ? m.Groups["actor"].Value : "";
                string skill = m.Groups["skill"].Success ? m.Groups["skill"].Value : "";

                // if skillname is blank, the skillname and actor may be transposed
                if (string.IsNullOrWhiteSpace(skill))
                {
                    if (!string.IsNullOrWhiteSpace(actor))
                    {
                        // "Received 1373 damage from Rising Blaze&apos;s ."
                        skill = actor;
                    }
                }

                if (string.IsNullOrWhiteSpace(target))
                    target = "You";

                if (string.IsNullOrWhiteSpace(actor))
                    actor = "Unknown";

                if (Advanced_Combat_Tracker.ActGlobals.oFormActMain.SetEncounter(timestamp, actor, target))
                {
                    Advanced_Combat_Tracker.ActGlobals.oFormActMain.AddCombatAction(
                        (int)Advanced_Combat_Tracker.SwingTypeEnum.NonMelee,
                        false,
                        "",
                        actor,
                        skill,
                        new Advanced_Combat_Tracker.Dnum(int.Parse(m.Groups["damage"].Value, System.Globalization.NumberStyles.AllowThousands)),
                        timestamp,
                        Advanced_Combat_Tracker.ActGlobals.oFormActMain.GlobalTimeSorter,
                        target,
                        "");

                }

                return;
            }
            //
            // for debugging!
            logInfo.logLine = "xxx " + logInfo.logLine;
        }
    }

    #endregion
}