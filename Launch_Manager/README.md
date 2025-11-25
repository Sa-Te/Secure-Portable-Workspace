# Secure Portable Workspace Manager

A C# Windows Forms application that acts as a secure wrapper/launcher for QEMU. It provides portability, stealth features, and forensic cleanup capabilities for running a virtual machine from a USB drive.

## Features

* **Zero-Installation Portability:** Automatically detects the current drive letter and patches QEMU configuration paths on the fly.
* **Stealth Interface:** Application launches as a decoy Console Window. Access requires a "blind" password input.
* **Forensic Cleanup:**
    * **Registry Scrub:** Removes software usage traces from `HKCU`.
    * **USB Sanitation:** Automates `USBDeview` to wipe history of specific USB dongles (WiFi/4G) used during the session.
* **Panic Mode:** Global Hotkey (`Ctrl + Alt + F10`) instantly terminates the VM and triggers the cleanup sequence.
* **Self-Destruct:** Option to permanently delete the Virtual Disk and configuration data.

## Architecture

The application expects the following directory structure to function:

```text
Root/
├── Launcher.exe
├── bin/                <-- Place QEMU binaries, Qtemu.exe, and USBDeview.exe here
└── data/               <-- Place vm_disk.qcow2 and qtemu.conf here
```

## Requirements

.NET Framework 4.7.2 or higher.

QEMU for Windows (x64).

USBDeview (NirSoft) - Required for USB history cleaning.

Administrator Privileges - Required for accessing the Registry and USB logs.

## Usage

 Launch: Run the executable as Administrator.

Authenticate: Type the access password blindly into the console window and press Enter.

Panic/Exit: Press the Global Hotkey to terminate and clean the session.

## Disclaimer

This tool is intended for privacy protection and forensic security testing. The author is not responsible for misuse.

