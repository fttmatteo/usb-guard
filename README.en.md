# 🛡️ USB Guard

*[Leer en Español](README.md)*

A powerful Zero-Trust security system for Windows environments that transforms any standard USB drive into a Cryptographic Master Key to lock and unlock your computer.

<p align="center">
  <img src="https://img.shields.io/badge/.NET-10.0-512BD4?style=for-the-badge&logo=dotnet" alt=".NET 10" />
  <img src="https://img.shields.io/badge/WPF-UI-0078D4?style=for-the-badge&logo=windows" alt="WPF UI" />
  <img src="https://img.shields.io/badge/License-MIT-green?style=for-the-badge" alt="MIT License" />
</p>

## Key Features

* **Zero-Trust Security:** Cloning files is not enough. The system reads the hardware serial number embedded in your USB drive to validate ownership.
* **High-Fidelity Glassmorphism Interface:** An immersive lock screen that natively blends with your desktop wallpaper (via direct Win32 calls) achieving aesthetics comparable to macOS and Windows 11.
* **Cryptographic Brute-Force Protection:** Rigorous implementation of exponential backoff (Rate-Limiting). Three failed attempts freeze the interface using progressive calculations that prevent automated attacks (*Rubber Ducky*).
* **Local Kernel Integration:** Built-in functions to mute the machine while locked, arm silently on startup via the Windows Run Registry, and persist in system memory.

## Quick Start & Installation

1. **Build:** Open the `USBGuard.sln` solution in Visual Studio. Ensure you have the **.NET 10.0 for Windows (WPF)** SDK installed.
2. **Release:** Change the build profile from "Debug" to "Release" to maximize obfuscation and performance, and press F5.
3. **Your First Key:**
   * Insert a standard USB Flash Drive into your computer.
   * Go to the Device manager inside USB Guard.
   * Enter a strong recovery password and press "Create Key".
4. **Arming:** Click the giant shield button on the Main Panel to arm the system. Once you pull out the USB drive, the computer will instantly lock!

## Why is USB Guard different?

Most commercial "locker" software only checks if a text file exists inside the USB. **USB Guard** takes that concept and fortifies it:
* It encrypts the internal key and validates the matrix hardware via WMI.
* It cannot be bypassed by simulated aggressive disconnections; it waits for 1.5 asynchronous seconds to allow Windows to assimilate the USB hardware IRQ interrupts.
* It features an Emergency Disarm; if you accidentally delete all your key databases, it will abort the trigger to prevent perpetual lockouts (Self-hijacking).

## Contributing

This ecosystem has been packaged for the open-source world. Anyone is welcome to contribute C++ Hooks, credential improvements, or optimizations! Please read `CONTRIBUTING.en.md` before making a *Pull Request*.

## Credits and License

Created and maintained by **[Mateo Valencia Ardila](https://github.com/fttmatteo)**.

This software operates under the **MIT License**. You are free to distribute, edit, or use this architectural base even for commercial development under the mandatory condition of explicitly preserving the copyright and attribution paragraphs to Mateo Valencia as the main creator. (Read the `LICENSE` document for more legal instructions).
