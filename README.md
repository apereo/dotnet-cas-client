# .NET CAS Client

The Jasig .NET CAS Client provides CAS integration for applications that use
the .NET framework as well as applications or static content hosted in IIS
running in Integrated Mode.

## Building

The source is intended to be built with Visual Studio.  Solution files
for VS 2008 and VS 2010 are included with the project.  The project can
also be built on the command line using MSBuild, although that is advanced
usage and not documented.

## Running

The build produces a single managed assembly, DotNetClient.dll, that may
be included as a dependency of another project.  In addition to adding a
dependency, the CAS integration must be configured via the web.config file.
See the [.NET CAS Client documentation][docs] for detailed configuration
instructions.

[docs]: https://wiki.jasig.org/display/CASC/.Net+Cas+Client
