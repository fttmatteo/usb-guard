# Contributing Guide for USB Guard

*[Leer en Español](CONTRIBUTING.md)*

Thank you for your interest in developing and improving **USB Guard**! As an open-source project, we rely on brilliant and analytical minds to strengthen the architecture that protects us all.

## Development Environment

To start contributing to the codebase, make sure you meet the following requirements:
1. Microsoft Visual Studio 2022.
2. **.NET 10.0** SDK (Desktop Development Workload / WPF).
3. Standard intermediate knowledge of the **MVVM** (Model-View-ViewModel) architecture and `CommunityToolkit.Mvvm` libraries.

## Critical Paths (Recommended Intervention Endpoints)

If you wish to work on vulnerabilities or broaden the scope, we recommend investigating these core branches of the project for *Pull Requests*:

* **Vulnerabilities and Global Hooks:** `USBGuard.Core\Interop`. Currently, the program overrides standard inputs to the lock screen (F4, Esc, AltTab). We are looking for secure C++ Unmanaged P/Invoke `SetWindowsHookEx` implementations to completely freeze the keyboard preventing deep physical injections.
* **GINA or Credential Provider Integration:** Exploratory module. Scaling USBGuard beyond running in the user layer by injecting it as a Native Credential Provider beneath the Windows OS LockScreen, which would prevent the process from being terminated via Task Manager.

## Basic Ground Rules

1. **Maintain Zero-Trust:** Any proposal that involves OS access or arming must be programmed assuming that *the final user will forget the steps or make a mistake*. Provide asynchronous paths to avoid "Auto-Lockouts".
2. **Visual Conventions:** Any redesign must flow through the `WPF-UI` styles. Avoid overloading solid colors and prioritize components with opacities to visually blend your component with native desktop wallpapers.
3. Do not submit PRs breaking the licensing structure (Required Attribution) of Mateo Valencia Ardila.

---
To get started, branch from main (`git checkout -b feature/MyAmazingIdea`), inject your code and push your commits (`git commit -m 'Added amazing feature'`). We are looking forward to it!
