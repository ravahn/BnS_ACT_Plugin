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
    public class BNS_ACT_Plugin : UserControl, Advanced_Combat_Tracker.IActPluginV1
    {
        #region Designer Created Code (Avoid editing)
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            lstMessages = new System.Windows.Forms.ListBox();
            this.cmdClearMessages = new System.Windows.Forms.Button();
            this.cmdCopyProblematic = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(88, 13);
            this.label1.TabIndex = 82;
            this.label1.Text = "Parser Messages";
            // 
            // lstMessages
            // 
            lstMessages.FormattingEnabled = true;
            lstMessages.Location = new System.Drawing.Point(14, 41);
            lstMessages.Name = "lstMessages";
            lstMessages.ScrollAlwaysVisible = true;
            lstMessages.Size = new System.Drawing.Size(700, 264);
            lstMessages.TabIndex = 81;
            // 
            // cmdClearMessages
            // 
            this.cmdClearMessages.Location = new System.Drawing.Point(88, 311);
            this.cmdClearMessages.Name = "cmdClearMessages";
            this.cmdClearMessages.Size = new System.Drawing.Size(106, 26);
            this.cmdClearMessages.TabIndex = 84;
            this.cmdClearMessages.Text = "Clear";
            this.cmdClearMessages.UseVisualStyleBackColor = true;
            this.cmdClearMessages.Click += new System.EventHandler(this.cmdClearMessages_Click);
            // 
            // cmdCopyProblematic
            // 
            this.cmdCopyProblematic.Location = new System.Drawing.Point(478, 311);
            this.cmdCopyProblematic.Name = "cmdCopyProblematic";
            this.cmdCopyProblematic.Size = new System.Drawing.Size(118, 26);
            this.cmdCopyProblematic.TabIndex = 85;
            this.cmdCopyProblematic.Text = "Copy to Clipboard";
            this.cmdCopyProblematic.UseVisualStyleBackColor = true;
            this.cmdCopyProblematic.Click += new System.EventHandler(this.cmdCopyProblematic_Click);
            // 
            // UserControl1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cmdCopyProblematic);
            this.Controls.Add(this.cmdClearMessages);
            this.Controls.Add(this.label1);
            this.Controls.Add(lstMessages);
            this.Name = "UserControl1";
            this.Size = new System.Drawing.Size(728, 356);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.Label label1;
        private static System.Windows.Forms.ListBox lstMessages;
        private System.Windows.Forms.Button cmdClearMessages;
        private System.Windows.Forms.Button cmdCopyProblematic;

        #endregion

        public BNS_ACT_Plugin()
        {
            InitializeComponent();
        }
        // reference to the ACT plugin status label
        private Label lblStatus = null;

        public void InitPlugin(System.Windows.Forms.TabPage pluginScreenSpace, System.Windows.Forms.Label pluginStatusText)
        {
            // store a reference to plugin's status label
            lblStatus = pluginStatusText;

            try
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


                pluginScreenSpace.Controls.Add(this);   // Add this UserControl to the tab ACT provides
                this.Dock = DockStyle.Fill; // Expand the UserControl to fill the tab's client space

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
                LogWriter.Initialize();

                lblStatus.Text = "BnS Plugin Started.";
            }
            catch (Exception ex)
            {
                LogParserMessage("Exception during InitPlugin: " + ex.ToString().Replace(Environment.NewLine, " "));
                lblStatus.Text = "InitPlugin Error.";
            }
        }

        public void DeInitPlugin()
        {
            // remove event handler
            Advanced_Combat_Tracker.ActGlobals.oFormActMain.UpdateCheckClicked -= this.UpdateCheckClicked;
            Advanced_Combat_Tracker.ActGlobals.oFormActMain.BeforeLogLineRead -= LogParse.BeforeLogLineRead;

            LogWriter.Uninitialize();

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


        public static void LogParserMessage(string message)
        {
            lstMessages.Invoke(new Action(() => lstMessages.Items.Add(message)));
        }

        private void cmdClearMessages_Click(object sender, EventArgs e)
        {
            lstMessages.Items.Clear();
        }

        private void cmdCopyProblematic_Click(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            foreach (object itm in lstMessages.Items)
                sb.AppendLine((itm ?? "").ToString());

            if (sb.Length > 0)
                System.Windows.Forms.Clipboard.SetText(sb.ToString());
        }
    }
    #endregion

    #region Memory Scanning code
    public static class LogWriter
    {
        [DllImport("kernel32.dll")]
        internal static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, IntPtr nSize, ref IntPtr lpNumberOfBytesRead);

        private static Thread _thread = null;
        private static bool _stopThread = false;

        private static string _logFileName = "";

        public static void Initialize()
        {
            _stopThread = false;
            try
            {
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
            catch (Exception ex)
            {
                BNS_ACT_Plugin.LogParserMessage("Error [BNS_Log.Initialize] " + ex.ToString().Replace(Environment.NewLine, " "));
                _stopThread = true;
            }
        }

        public static void Uninitialize()
        {
            try
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
            catch (Exception ex)
            {
                BNS_ACT_Plugin.LogParserMessage("Error [BNS_Log.Uninitialize] " + ex.ToString().Replace(Environment.NewLine, " "));
            }
        }

        private const Int32 chatlogOffset = 0x00d6d8b0;

        private static void Scan()
        {
            Process process = null;
            IntPtr baseAddress = IntPtr.Zero;
            IntPtr chatlogPointer = IntPtr.Zero;

            DateTime lastPointerUpdate = DateTime.MinValue;

            int lastLine = -1;


            while (!_stopThread)
            {
                System.Threading.Thread.Sleep(10);
                try
                {
                    // update process and pointer every 10 seconds
                    if (process == null || DateTime.Now.Subtract(lastPointerUpdate).TotalSeconds > 10.0)
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

                        lastPointerUpdate = DateTime.Now;
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

                    for (int i = lastLine + 1; i <= lineCount; i++)
                    {
                        IntPtr lineHeaderPointer = IntPtr.Add(chatlogPointer, 4 + 0x80 * (i % 300));

                        byte[] header = new byte[0x80];

                        ReadBuffer(process.Handle, lineHeaderPointer, ref header, 0x80);

                        // first 4 bytes is a pointer
                        IntPtr linePointer = new IntPtr(BitConverter.ToUInt32(header, 0));

                        // # of bytes in the line is 4 bytes after that, int16
                        Int16 byteCount = header[0x10];

                        // Skip empty lines
                        if (byteCount == 0)
                            continue;

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
                catch (Exception ex)
                {
                    File.AppendAllText(_logFileName,
                        DateTime.Now.ToString("HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture) + 
                        " Error [BNS_Log.Scan] " + ex.ToString().Replace(Environment.NewLine, " "));

                    // do not exit scan thread, but pause so that the errors dont pile up.
                    System.Threading.Thread.Sleep(1000);
                }
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

    public static class LogParse
    {
        public static Regex regex_incomingdamage1 = new Regex(@"(?<target>.+?)?( received|Received) (?<damage>\d+(,\d+)*) ((?<critical>Critical) )?damage((,)?( and)? (?<HPDrain>\d+(,\d+)*) HP drain)?((,)?( and)? (?<FocusDrain>\d+) Focus drain)?((,)?( and)? (?<debuff>.+?))? from ((?<actor>.+?)&apos;s )?(?<skill>.+?)((,)?( but)? resisted (?<resistdebuff>.+?)( effect)?)?\.", RegexOptions.Compiled);
        public static Regex regex_incomingdamage2 = new Regex(@"((?<target>.+?) )?(Blocked|blocked|partially blocked|countered)( (?<actor>.+)&apos;s)? (?<skill>.+?) (but received|receiving)( (?<damage>\d+(,\d+)*) damage)?(( and)? (?<HPDrain>\d+(,\d+)*) HP drain)?( and?)?( (?<debuff>.+?))?\.", RegexOptions.Compiled);
        public static Regex regex_incomingdamage3 = new Regex(@"(?<actor>.+?)&apos;s (?<skill>.+?) inflicted( (?<damage>\d+(,\d+)*) damage)?( and)?( (?<debuff>.+?))*?( to (?<target>.+?))?\.", RegexOptions.Compiled);
        public static Regex regex_yourdamage = new Regex(@"(?<skill>.+?) (?<critical>(critically hit)|(hit)) (?<target>.+?) for (?<damage>\d+(,\d+)*) damage(((, draining| and drained) ((?<HPDrain>\d+(,\d+)*) HP)?( and )?((?<FocusDrain>\d+) Focus)?))?(, removing (?<skillremove>.+?))?\.", RegexOptions.Compiled);
        public static Regex regex_debuff2 = new Regex(@"((?<actor>.+?)&apos;s )?(?<skill>.+?)( (?<critical>(critically hit)|(hit)) (?<target>.+?))? ((and )?inflicted (?<debuff>.+?))?(but (?<debuff2>.+?) was resisted)?\.", RegexOptions.Compiled);
        public static Regex regex_evade = new Regex(@"(?<target>.+?) evaded (?<skill>.+?)\.", RegexOptions.Compiled);
        public static Regex regex_defeat = new Regex(@"(?<target>.+?) (was|were) (defeated|rendered near death|killed) by ((?<actor>.+?)&apos;s )?(?<skill>.+?)\.", RegexOptions.Compiled);
        public static Regex regex_debuff = new Regex(@"(?<target>.+?) (receives|resisted) (?<skill>.+?)\.", RegexOptions.Compiled);
        public static Regex regex_heal = new Regex(@"(?<target>.+?)?( recovered|Recovered) ((?<HPAmount>\d+(,\d+)*) HP)?((?<FocusAmount>\d+) Focus)? (with|from) (?<skill>.+?)\.");
        public static Regex regex_buff = new Regex(@"(?<skill>.+?) is now active\.", RegexOptions.Compiled);

        public static DateTime ParseLogDateTime(string message)
        {
            DateTime ret = DateTime.MinValue;

            try
            {
                if (message == null || message.IndexOf(' ') < 5)
                    return ret;

                if (!DateTime.TryParse(message.Substring(0, message.IndexOf(' ')), out ret))
                    return DateTime.MinValue;
            }
            catch (Exception ex)
            {
                BNS_ACT_Plugin.LogParserMessage("Error [ParseLogDateTime] " + ex.ToString().Replace(Environment.NewLine, " "));
            }
            return ret;
        }

        public static void BeforeLogLineRead(bool isImport, Advanced_Combat_Tracker.LogLineEventArgs logInfo)
        {
            string logLine = logInfo.logLine;

            try
            {
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
                    string actor = "You";
                    string target = m.Groups["target"].Success ? DecodeString(m.Groups["target"].Value) : "";

                    if (Advanced_Combat_Tracker.ActGlobals.oFormActMain.SetEncounter(timestamp, actor, target))
                    {
                        Advanced_Combat_Tracker.ActGlobals.oFormActMain.AddCombatAction(
                            (int)Advanced_Combat_Tracker.SwingTypeEnum.NonMelee,
                            m.Groups["critical"].Value == "critically hit",
                            "",
                            actor,
                            DecodeString(m.Groups["skill"].Value),
                            new Advanced_Combat_Tracker.Dnum(int.Parse(m.Groups["damage"].Value, System.Globalization.NumberStyles.AllowThousands)),
                            timestamp,
                            Advanced_Combat_Tracker.ActGlobals.oFormActMain.GlobalTimeSorter,
                            target,
                            "");

                        if (m.Groups["HPDrain"].Success)
                        {
                            Advanced_Combat_Tracker.ActGlobals.oFormActMain.AddCombatAction(
                                (int)Advanced_Combat_Tracker.SwingTypeEnum.Healing,
                                false,
                                "Drain",
                                actor,
                                DecodeString(m.Groups["skill"].Value),
                                new Advanced_Combat_Tracker.Dnum(int.Parse(m.Groups["HPDrain"].Value, System.Globalization.NumberStyles.AllowThousands)),
                                timestamp,
                                Advanced_Combat_Tracker.ActGlobals.oFormActMain.GlobalTimeSorter,
                                target,
                                "");
                        }

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
                    string target = m.Groups["target"].Success ? DecodeString(m.Groups["target"].Value) : "";
                    string actor = m.Groups["actor"].Success ? DecodeString(m.Groups["actor"].Value) : "";
                    string skill = m.Groups["skill"].Success ? DecodeString(m.Groups["skill"].Value) : "";

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

                    // todo: in the future, if damage is missing, still parse the buff portion
                    if (!m.Groups["damage"].Success)
                        return;
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

                        if (m.Groups["HPDrain"].Success)
                        {
                            Advanced_Combat_Tracker.ActGlobals.oFormActMain.AddCombatAction(
                                (int)Advanced_Combat_Tracker.SwingTypeEnum.Healing,
                                false,
                                "Drain",
                                actor,
                                skill,
                                new Advanced_Combat_Tracker.Dnum(int.Parse(m.Groups["HPDrain"].Value, System.Globalization.NumberStyles.AllowThousands)),
                                timestamp,
                                Advanced_Combat_Tracker.ActGlobals.oFormActMain.GlobalTimeSorter,
                                actor,
                                "");
                        }
                    }

                    return;
                }

                m = regex_heal.Match(logLine);
                if (m.Success)
                {
                    string target = m.Groups["target"].Success ? DecodeString(m.Groups["target"].Value) : "";
                    if (string.IsNullOrWhiteSpace(target))
                        target = "You";
                    string actor = "Unknown";

                    // do not process if there is no HP amount.
                    if (!m.Groups["HPAmount"].Success)
                        return;

                    if (Advanced_Combat_Tracker.ActGlobals.oFormActMain.SetEncounter(timestamp, actor, target))
                    {
                        Advanced_Combat_Tracker.ActGlobals.oFormActMain.AddCombatAction(
                            (int)Advanced_Combat_Tracker.SwingTypeEnum.Healing,
                            false,
                            "",
                            actor,
                            DecodeString(m.Groups["skill"].Value),
                            new Advanced_Combat_Tracker.Dnum(int.Parse(m.Groups["HPAmount"].Value, System.Globalization.NumberStyles.AllowThousands)),
                            timestamp,
                            Advanced_Combat_Tracker.ActGlobals.oFormActMain.GlobalTimeSorter,
                            target,
                            "");

                    }
                    return;
                }


                m = regex_debuff2.Match(logLine);
                if (m.Success)
                {
                    // todo: add debuff support
                    return;
                }

                m = regex_debuff.Match(logLine);
                if (m.Success)
                {
                    // todo: add debuff support
                    return;
                }

                m = regex_buff.Match(logLine);
                if (m.Success)
                {
                    // todo: add buff support
                    return;
                }

                m = regex_evade.Match(logLine);
                if (m.Success)
                {
                    // todo: add evade support
                    return;
                }

                m = regex_defeat.Match(logLine);
                if (m.Success)
                {
                    string target = m.Groups["target"].Success ? DecodeString(m.Groups["target"].Value) : "";
                    string actor = m.Groups["actor"].Success ? DecodeString(m.Groups["actor"].Value) : "";
                    if (string.IsNullOrWhiteSpace(actor))
                        actor = "Unknown";

                    if (Advanced_Combat_Tracker.ActGlobals.oFormActMain.SetEncounter(timestamp, actor, target))
                    {
                        Advanced_Combat_Tracker.ActGlobals.oFormActMain.AddCombatAction(
                            (int)Advanced_Combat_Tracker.SwingTypeEnum.NonMelee,
                            false,
                            "",
                            actor,
                            DecodeString(m.Groups["skill"].Value),
                            Advanced_Combat_Tracker.Dnum.Death,
                            timestamp,
                            Advanced_Combat_Tracker.ActGlobals.oFormActMain.GlobalTimeSorter,
                            target,
                            "");

                    }

                    return;
                }
            }
            catch (Exception ex)
            {
                BNS_ACT_Plugin.LogParserMessage("Error [LogParse.BeforeLogLineRead] " + ex.ToString().Replace(Environment.NewLine, " ") + " " + logInfo.logLine);
            }

            // For debugging
            if (!string.IsNullOrWhiteSpace(logLine))
                BNS_ACT_Plugin.LogParserMessage("Unhandled Line: " + logInfo.logLine);
        }

        private static string DecodeString(string data)
        {
            string ret = data.Replace("&apos;", "'")
                .Replace("&amp;", "&");

            return ret;
        }
    }

    #endregion


}