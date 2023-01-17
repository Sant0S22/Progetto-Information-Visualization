Contact: stephen@turnthegameon.com

This package uses the new C# Job System and Burst Compiler. You must make the following project configurations to enable these Unity features.

Open the Player Settings (Edit > Project Settings > Player).
Player Settings:
-Scripting Runtime Version .Net 4.x Equivalent 
-API Compatibility Level .Net 4.x

Open the Package Manager (Window > Package Manager), enable Show Preview Packages.
Package Manager Dependencies:
-Burst
-Collections
-Jobs
-Mathematics

Enable "Jobs > Burst > Enable Compilation" from the Unity menu bar.

To use the burst compiler in standalone builds, you need to install the Windows SDK and VC++ toolkit from the Visual Studio Installer.