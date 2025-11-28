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
Build Instructions (How to Re-Link the Engine)

Since the virtualization engine is too large for GitHub, it is excluded from the repo. Follow these steps to build the project after cloning:

## Prepare the Engine Files:

Download QEMU for Windows (x64) and USBDeview.

Place all QEMU files (qemu-system-x86_64.exe, DLLs, etc.) and USBDeview.exe into a single folder.

Select ALL files in that folder -> Right Click -> Send to -> Compressed (zipped) folder.

Name the file: engine.zip. (Ensure the zip is "flat" - it should not contain a folder inside, just the files).

## Add to Resources:

Open the solution in Visual Studio.

Go to Project Properties -> Resources.

Click the arrow next to "Add Resource" -> Add Existing File.

Select your engine.zip.

Important: Rename the resource entry to engine (if it isn't already).

## Compile:

Switch build configuration to Release.

Build Solution.

## Deployment & Usage

## Setup:

Copy the compiled .exe to your USB drive. Rename it (e.g., Photoshop.exe) for camouflage.

Create a folder named data next to it.

Place your QEMU Virtual Disk (.qcow2) inside data and rename it to resource.dat.

(Optional) Set the data folder to Hidden.

## Launch:

Run the executable as Administrator (Required for cleanup permissions).

A black "Error" window will appear.

## Authenticate:

Type the password blindly (Default: 123) and press Enter.

The app will extract the engine and launch the VM.

## Panic/Exit:

Press F10 at any time.

The VM will terminate, traces will be scrubbed, and the temporary engine files will be deleted.

## Disclaimer

This tool is intended for privacy protection and forensic security testing. The author is not responsible for misuse.