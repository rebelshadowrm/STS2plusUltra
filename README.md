# STS2Plus

This repo is the source project for the `STS2Plus` mod.

Repo scope:
- source code under `STS2Plus*`, `GodotPlugins.Game`, `Properties`
- project files such as `STS2Plus.csproj`, `build.ps1`, `mod_manifest.json`

Not part of the repo:
- build output in `bin/`, `obj/`, `dist/`
- local editor/tool state in `.claude/`, `.vs/`
- outer workspace reverse-engineering scratch files

Build:

```powershell
dotnet build -c Release
```

If your Slay the Spire 2 install is not in the default path, pass:

```powershell
dotnet build -c Release -p:STS2GameDir="C:\Path\To\Slay the Spire 2"
```
