using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.IO.Compression;
using System.Reflection;

namespace Launch_Manager
{
    public partial class Form1 : Form
    {
        // GLOBAL VARIABLES
        DateTime sessionStartTime;
        Process vmProcess;
        string bufferPassword = "";

        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vlc);
        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

     
        const int MY_HOTKEY_ID = 9000;
        // Using F10 only for maximum compatibility (No modifiers needed for testing)
     
        const int MOD_NONE = 0x0000;
        const int VK_F10 = 0x79;

        public Form1()
        {
            InitializeComponent();

            // Make it look like a CMD window
            this.BackColor = Color.Black;
            this.ForeColor = Color.White; // White text
            this.Font = new Font("Consolas", 10); // Terminal font
            this.Text = "C:\\Windows\\System32\\cmd.exe"; // Fake Title
            this.FormBorderStyle = FormBorderStyle.FixedToolWindow; // Simple border
            this.ShowInTaskbar = true; // Show initially so they see the "Error"
            this.Size = new Size(600, 300);

            // Enable Key Preview to catch the blind password
            this.KeyPreview = true;
            this.KeyPress += new KeyPressEventHandler(Form1_KeyPress);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // PRINT THE FAKE ERROR
            Label fakeError = new Label();
            fakeError.Text = "Microsoft Windows [Version 10.0.19045.4651]\n(c) Microsoft Corporation. All rights reserved.\n\nC:\\Users\\Admin> Photoshop.exe\n\n[ERROR] fatal error: module 'core_lib.dll' not found.\n[ERROR] application failed to initialize (0xC0000142).\n\nC:\\Users\\Admin>_";
            fakeError.AutoSize = true;
            fakeError.Location = new Point(10, 10);
            this.Controls.Add(fakeError);

            if (notifyIcon1.Icon == null) notifyIcon1.Icon = SystemIcons.Shield;
        }
        private async void StartSystem()
        {
            // 1. Hide the Console
            this.Hide();
            this.ShowInTaskbar = false;

            // 2. SHOW TRAY ICON IMMEDIATELY (So you know it's working)
            notifyIcon1.Visible = true;
            notifyIcon1.ShowBalloonTip(3000, "System Loading", "Decrypting engine... Please wait.", ToolTipIcon.Info);

            // 3. Register Hotkey
            RegisterHotKey(this.Handle, MY_HOTKEY_ID, MOD_NONE, VK_F10);

            // 4. EXTRACT ENGINE (Run in background task to prevent freezing)
            bool extractionSuccess = await Task.Run(() => ExtractEngine());

            if (extractionSuccess)
            {
                // 5. LAUNCH VM
                LaunchVM();
            }
        }

        private bool ExtractEngine()
        {
            try
            {
                string currentPath = AppDomain.CurrentDomain.BaseDirectory;
                string binPath = Path.Combine(currentPath, "bin");

                // If bin folder exists, verify contents or delete
                if (Directory.Exists(binPath))
                {
                    try { Directory.Delete(binPath, true); } catch { /* Ignore if locked */ }
                }

                Directory.CreateDirectory(binPath);

                // Extract Resource
                byte[] zipBytes = Properties.Resources.engine;

                string zipPath = Path.Combine(currentPath, "temp_engine.zip");
                File.WriteAllBytes(zipPath, zipBytes);

                ZipFile.ExtractToDirectory(zipPath, binPath);
                File.Delete(zipPath);

                return true; // Success
            }
            catch (Exception ex)
            {
                MessageBox.Show("Critical Error during Extraction:\n" + ex.Message);
                Application.Exit();
                return false;
            }
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            // If user presses ENTER, check password
            if (e.KeyChar == (char)13)
            {
                if (bufferPassword == "123")
                {
                    // SUCCESS: Start the sequence
                    sessionStartTime = DateTime.Now;
                    StartSystem();
                }
                else
                {
                    // WRONG PASSWORD: Just close like a broken app
                    Application.Exit();
                }
                bufferPassword = ""; // Reset
            }
            else
            {
                // Record the key
                bufferPassword += e.KeyChar;
            }
        }



        // --- HOTKEY LISTENER ---
        protected override void WndProc(ref Message m)
        {
            // Watch for the Hotkey Message (0x0312)
            if (m.Msg == 0x0312 && m.WParam.ToInt32() == MY_HOTKEY_ID)
            {
                // PANIC BUTTON PRESSED!
                PerformCleanAndExit();
            }
            base.WndProc(ref m);
        }


        private void LaunchVM()
        {
            try
            {
                string currentPath = AppDomain.CurrentDomain.BaseDirectory;
                string binPath = Path.Combine(currentPath, "bin");
                string diskPath = Path.Combine(currentPath, "data", "resource.dat");

                // FIX: Search for the exe in subfolders in case zip structure was weird
                string qemuExe = "";
                string[] files = Directory.GetFiles(binPath, "qemu-system-x86_64.exe", SearchOption.AllDirectories);

                if (files.Length > 0)
                {
                    qemuExe = files[0];
                }
                else
                {
                    MessageBox.Show("Critical Error: QEMU Engine not found in extracted files.");
                    PerformCleanAndExit();
                    return;
                }

                if (!File.Exists(diskPath))
                {
                    MessageBox.Show("Critical Error: Hard Drive not found at:\n" + diskPath);
                    Application.Exit();
                    return;
                }


                string args = $"-L . -m 2048 -hda \"{diskPath}\" -boot c -vga std -accel tcg";

                // 4. LAUNCH DIRECTLY
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = qemuExe;
                startInfo.Arguments = args;

                // CRITICAL: Run inside the bin folder so it finds .dlls
                startInfo.WorkingDirectory = Path.GetDirectoryName(qemuExe);

                vmProcess = Process.Start(startInfo);

                // 5. STEALTH MODE
                notifyIcon1.Visible = true;
                notifyIcon1.ShowBalloonTip(3000, "System Active", "Secure VM is running.", ToolTipIcon.Info);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Launch Error: " + ex.Message);
                Application.Exit();
            }
        }

        private async void PerformCleanAndExit()
        {
            // 1. Show the "Console" again for psychological trust
            this.Controls.Clear(); // Clear the old error text
            this.Show();
            this.WindowState = FormWindowState.Normal;
            this.TopMost = true; // Force to top
            this.BringToFront();

            Label logLabel = new Label();
            logLabel.ForeColor = Color.LimeGreen; // Hacker Green
            logLabel.Font = new Font("Consolas", 10);
            logLabel.AutoSize = true;
            logLabel.Location = new Point(10, 10);
            this.Controls.Add(logLabel);

            // 2. Kill VM
            Log(logLabel, "Terminating Virtual Machine...");
            await Task.Delay(500);
            try
            {
                if (vmProcess != null && !vmProcess.HasExited)
                {
                    vmProcess.Kill();
                    vmProcess.WaitForExit();
                }
            }
            catch { }
            Log(logLabel, "[OK] Process Terminated.");

            // 3. Registry Clean
            await Task.Delay(300);
            Log(logLabel, "Scanning Registry for QEMU traces...");
            CleanRegistry();
            await Task.Delay(400);
            Log(logLabel, "[OK] Registry Keys Wiped.");

            Log(logLabel, "Wiping temporary engine files...");
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            string binPath = Path.Combine(currentPath, "bin");

            // We do NOT delete 'data' because that holds his OS (resource.dat)
            // We ONLY delete 'bin' (the QEMU engine we extracted)

            try
            {
                if (Directory.Exists(binPath))
                {
                    Directory.Delete(binPath, true);
                    Log(logLabel, "[OK] Engine Removed. Traces gone.");
                }
            }
            catch { Log(logLabel, "[FAIL] Could not delete bin folder."); }

            // 4. USB Clean
            Log(logLabel, "Scanning USB Hub History...");
            await Task.Delay(500); // Fake delay for drama
            CleanUSBTraces(logLabel); // Pass label to update status
            Log(logLabel, "[OK] USB History Sanitized.");

            // 5. Finish
            Log(logLabel, "--------------------------------");
            Log(logLabel, "CLEANUP COMPLETE. SYSTEM SAFE.");
            Log(logLabel, "Closing in 3 seconds...");

            notifyIcon1.Visible = false;
            UnregisterHotKey(this.Handle, MY_HOTKEY_ID); // Clean up hotkey

            await Task.Delay(3000);
            Application.Exit();
        }

        private void Log(Label lbl, string text)
        {
            lbl.Text += text + "\n";
            this.Refresh(); // Force UI update
        }


        // --- HELPER: KILLS THE VM PROCESS ---
        private void KillVM()
        {
            try
            {
                if (vmProcess != null && !vmProcess.HasExited)
                {
                    vmProcess.Kill(); // Force close QEMU
                    vmProcess.WaitForExit(); // Wait until it's actually gone
                }
            }
            catch
            {
                // Ignore errors if it's already closed
            }
        }

        private void CleanRegistry()
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true))
                {
                    if (key != null && key.OpenSubKey("Qtemu") != null)
                    {
                        key.DeleteSubKeyTree("Qtemu");
                    }
                }
            }
            catch { }
        }

        private void CleanUSBTraces(Label logLabel) // Updated to accept Label for logs
        {
            try
            {
                string currentPath = AppDomain.CurrentDomain.BaseDirectory;
                string binPath = Path.Combine(currentPath, "bin");

                string usbExe = "";
                string[] files = Directory.GetFiles(binPath, "USBDeview.exe", SearchOption.AllDirectories);
                if (files.Length > 0) usbExe = files[0];
                else return;

                string reportPath = Path.Combine(Path.GetDirectoryName(usbExe), "report.txt");

                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = usbExe;
                psi.Arguments = $"/stab \"{reportPath}\" /DisplayDisconnected 1";
                psi.WindowStyle = ProcessWindowStyle.Hidden;
                psi.Verb = "runas";
                Process.Start(psi).WaitForExit(5000);

                if (File.Exists(reportPath))
                {
                    string[] lines = File.ReadAllLines(reportPath);
                    foreach (string line in lines)
                    {
                        if (string.IsNullOrWhiteSpace(line)) continue;
                        string fullLineData = line.ToLower();

                        bool isTarget = false;
                        if (fullLineData.Contains("virtualbox") || fullLineData.Contains("tablet")) isTarget = true;
                        if (fullLineData.Contains("huawei") || fullLineData.Contains("rndis")) isTarget = true;

                        if (isTarget)
                        {
                            Match match = Regex.Match(line, @"[a-zA-Z0-9]+\\VID_[A-Za-z0-9&_\\.-]+", RegexOptions.IgnoreCase);
                            if (!match.Success) match = Regex.Match(line, @"(USB|HID|PCI|SCSI)\\[A-Za-z0-9&_\\.-]+", RegexOptions.IgnoreCase);

                            if (match.Success)
                            {

                                if (logLabel != null)
                                {
                                    Log(logLabel, "  > Removing artifact: " + match.Value);
                                }

                                Log(logLabel, "  > Removing artifact: " + match.Value); // SHOW THE USER
                                ProcessStartInfo killPsi = new ProcessStartInfo();
                                killPsi.FileName = usbExe;
                                killPsi.Arguments = $"/remove_device_by_instance_id \"{match.Value}\"";
                                killPsi.WindowStyle = ProcessWindowStyle.Hidden;
                                killPsi.Verb = "runas";
                                Process.Start(killPsi).WaitForExit(1000);
                            }
                        }
                    }
                    File.Delete(reportPath);
                }
            }
            catch { }
        }

        // --- MODE A: SAVE & CLEAN ---
        private void cleanAndSaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PerformCleanAndExit();
        }

        // --- MODE B: DESTROY EVERYTHING ---
        private void dESTROYEVERYTHINGToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "WARNING: DESTROY MODE\n\nThis will DELETE your Data folder and the Virtual Machine disk.\nThis cannot be undone.\n\nAre you sure?",
                "CONFIRM DESTRUCTION",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                try
                {
                    // 1. Kill VM immediately
                    KillVM();

                    // 2. Clean Traces
                    CleanRegistry();
                    CleanUSBTraces(null);

                    // 3. DELETE THE DATA FOLDER
                    string currentPath = AppDomain.CurrentDomain.BaseDirectory;
                    string dataPath = Path.Combine(currentPath, "data");

                    if (Directory.Exists(dataPath))
                    {
                        Directory.Delete(dataPath, true); // 'true' means recursive delete (all files inside)
                    }

                    // 4. (Optional) Delete Bin folder too?
                    // string binPath = Path.Combine(currentPath, "bin");
                    // if (Directory.Exists(binPath)) Directory.Delete(binPath, true);

                    MessageBox.Show("Destruction Complete.\nData folder has been erased.", "System Destroyed");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error destroying files: " + ex.Message);
                }
                finally
                {
                    notifyIcon1.Visible = false;
                    Application.Exit();
                }
            }
        }
    }
}