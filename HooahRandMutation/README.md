# Template README

## How to use

* using `dotnet new`
* using `Rider`
  * You can't set template specific variable with rider's GUI tool. I highly recommend to use cli tool to bootstrap your .
* using `Visual Studio`

## Useful Tutorials

* My Modding Documentation: https://hooh-hooah.github.io

## Releasing the binary

Please do not upload the binary in some random file-hosting service. Upload with the source or use Version Control Services such as GitHub, GitLab and GitBucket.

You can automate the releasing process with Azure Pipeline. 

Please use `**/azurebuild/*.dll` to search up all final output binaries.

# HooahRandMutation

## Summary

HooahRandMutation is a plugin for AI/HS2 Game

## Requirement

An IDE that capable of developing a Unity Application and Library Binary

* Visual Studio 
* Visual Studio Code
* Rider

An Operation System that can build C# Application and Library Binary

* An OS that can run `.NET Framework` application
* An OS that can run `git`

## Building the project

**Cloning the repository**

```bash
git clone [INSERT YOUR GIT REPOSITORY HERE]
```

**Building the project**

* Using Visual Studio
* Using Rider
* Using MSBuild
* Using Azure

**Exporting the Output to the game**

* Manually Copying the binary to BepinEx Plugin Folder
* Setting up MSBuild Build Property
  * AIPath
  * HS2Path

