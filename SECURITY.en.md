# Security Policy

*[Leer en Español](SECURITY.md)*

## Supported Versions

Below are the official open-source project branches that are currently receiving biological patches and network security architectural fixes.

| Version | Actively Supported? |
| ------- | ------------------ |
| v1.1.x   | :white_check_mark: |
| Older    | :x:                |

## Current Scope and Limitations (v1.0.x)

**USB Guard** is a consumer/pro-sumer grade software. It currently successfully intercepts windows in the User Layer through asynchronous hooks and *TopMost* / *Taskbar Hiding* properties.

Please note that *USB Guard is not an Antivirus, nor does it encrypt the physical hardware stream of the C port*. Furthermore, it is recommended to sheath the program with global locking policies from the Group Policy Object Administrator (GPO), as theoretically, advanced users with `NT Administrator` privileges could forcibly terminate the native process via the *Task Manager*, interrupting the C++ P/Invoke. For this reason, we suggest choosing the "Native Windows Lock" if your office environment is inherently hostile.

## Reporting a Vulnerability

The **Brute Force** authentication structure prevents automated injections (Rate Limits). 
Even so, if you believe you have found the mathematical way to break the local `System.Security.Cryptography` algorithm, please **DO NOT CREATE A PUBLIC ISSUE** that compromises current functional installations. 

1. Send a private email or direct message to the project packager (**Mateo Valencia Ardila**) with the subject `USB GUARD VULNERABILITY`.
2. Provide a PoC (Proof of Concept) or a reproducible injection code.
3. We will try to mitigate the breach by publishing a security "Hotfix" within the weekly cycle following the formal reading of the report. We immensely thank anyone who detects lethal errors with a desire to help.

## Code Signing Policy

Official USB Guard binaries are digitally signed with a certificate provided by the [SignPath Foundation](https://signpath.org) under their open-source program.

### Chain of Trust

* **Certificate issued by:** SignPath Foundation — a recognized certificate authority for OSS projects.
* **Verifiable origin:** Every signed artifact is built directly from the source code in the official GitHub repository through an automated CI pipeline. Manually compiled binaries are never signed.
* **Build integrity:** The signing process is linked to the continuous integration system (GitHub Actions), ensuring that the distributed `.exe` corresponds exactly to the public source code.

### Signature Verification

Users can verify the authenticity of any official USB Guard release:

1. Right-click on the `.exe` file → **Properties** → **Digital Signatures** tab.
2. Verify that the signer is **"SignPath Foundation"** and the status shows **"This digital signature is OK"**.

### Commitment

* Only builds generated automatically from the `main` branch of the official repository are signed.
* No contributor has direct access to the certificate's private key.
* Any release without a valid digital signature should not be considered official.
