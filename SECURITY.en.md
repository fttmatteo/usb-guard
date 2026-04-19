# Security Policy

*[Leer en Español](SECURITY.md)*

## Supported Versions

Below are the official open-source project branches that are currently receiving biological patches and network security architectural fixes.

| Version | Actively Supported? |
| ------- | ------------------ |
| v1.1.x | :white_check_mark: |
| Older    | :x:                |

## Current Scope and Limitations

**USB Guard** is a consumer/pro-sumer grade software. It currently successfully intercepts windows in the User Layer through asynchronous hooks and *TopMost* / *Taskbar Hiding* properties.

Please note that *USB Guard is not an Antivirus, nor does it encrypt the physical hardware stream of the C port*. Furthermore, it is recommended to sheath the program with global locking policies from the Group Policy Object Administrator (GPO), as theoretically, advanced users with `NT Administrator` privileges could forcibly terminate the native process via the *Task Manager*, interrupting the C++ P/Invoke. For this reason, we suggest choosing the "Native Windows Lock" if your office environment is inherently hostile.

## Reporting a Vulnerability

The **Brute Force** authentication structure prevents automated injections (Rate Limits). 
Even so, if you believe you have found the mathematical way to break the local `System.Security.Cryptography` algorithm, please **DO NOT CREATE A PUBLIC ISSUE** that compromises current functional installations. 

1. Send a private email or direct message to the project packager (**Mateo Valencia Ardila**) with the subject `USB GUARD VULNERABILITY`.
2. Provide a PoC (Proof of Concept) or a reproducible injection code.
3. We will try to mitigate the breach by publishing a security "Hotfix" within the weekly cycle following the formal reading of the report. We immensely thank anyone who detects lethal errors with a desire to help.

## Disclosure

We follow responsible disclosure practices. We will not disclose the vulnerability until a patch is available and users have had time to update, unless it's in the best interest of the community.



