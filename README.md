# Apereo .NET CAS Client #

[![Build status](https://ci.appveyor.com/api/projects/status/py9b6esq9smjr6u5/branch/master?svg=true)](https://ci.appveyor.com/project/mmoayyed/dotnet-cas-client/branch/master)
[![Stable nuget](https://img.shields.io/nuget/v/DotNetCasClient.svg?label=stable%20nuget)](https://www.nuget.org/packages/DotNetCasClient/)
[![Pre-release nuget](https://img.shields.io/myget/dotnetcasclient-prerelease/v/dotnetcasclient.svg?label=pre-release%20nuget)](https://www.myget.org/feed/dotnetcasclient-prerelease/package/nuget/DotNetCasClient)
[![Unstable nuget](https://img.shields.io/myget/dotnetcasclient-ci/v/dotnetcasclient.svg?label=unstable%20nuget)](https://www.myget.org/feed/dotnetcasclient-ci/package/nuget/DotNetCasClient)
[![License](https://img.shields.io/badge/License-Apache%202.0-blue.svg)](https://opensource.org/licenses/Apache-2.0)

[![Gitter](https://img.shields.io/gitter/room/apereo/cas.svg)](https://gitter.im/apereo/dotnet-cas-client)
[![Stack Overflow](https://img.shields.io/badge/stackoverflow-cas%20%2B%20.net-orange.svg)](https://stackoverflow.com/questions/tagged/cas%2b.net)

## Introduction ##

The Apereo .NET CAS client provides CAS integration for the Microsoft Windows platform via the .NET framework.

## Features ##

- Supports CAS Protocol 1.0 and 2.0 and SAML 1.1
- Supports CAS single sign-out
- Rich support for Microsoft ASP.NET platform integration through Forms Authentication framework

## Table of Contents ##

* [Downloads](#downloads)
* [Release Notes](#release-notes)
* [License](#license)
* [Documentation](#documentation)
    * [Building](#building)
    * [Running](#running)
    * [Integration Instructions](#integration-instructions)

## Downloads ##

* The latest (stable) release is [available on NuGet](https://www.nuget.org/packages/DotNetCasClient/) or can be [downloaded from GitHub](https://github.com/apereo/dotnet-cas-client/releases).
* Unstable pre-release versions are [available on MyGet](https://www.myget.org/feed/dotnetcasclient-prerelease/package/nuget/DotNetCasClient)

## Release Notes ##

See [ReleaseNotes.md](https://github.com/apereo/dotnet-cas-client/blob/master/ReleaseNotes.md) for details.

## License ##

The Apereo .NET CAS Client is open source software, licensed under the Apache License 2.0.

See [LICENSE.txt](https://github.com/apereo/dotnet-cas-client/blob/master/LICENSE.txt) for details.

## Documentation ##

- [Apereo .NET CAS Client](https://wiki.jasig.org/display/casc/.net+cas+client)
- [Apereo Central Authentication Service (CAS)](https://apereo.github.io/cas/)

### Building

The source is intended to be built with [Visual Studio 2017](https://www.visualstudio.com/downloads/) (any 2017 edition including the free Community edition will work.)  

The project can also be built via the command line using a [Cake](https://www.cakebuild.net/) build script.  If you are building from the command line, PowerShell or on a build server you must have the [Build Tools for Visual Studio 2017](https://www.visualstudio.com/downloads/) installed on the machine the build is occurring on.

To build the project from the command line, all you need to do is drop to PowerShell (or open a PowerShell window directly) and then run the **build.ps1** file to build the project and associated NuGet package.  The NuGet package will be copied to the **artifacts** folder in the root of the repo when the build process has completed.

### Running

The build (via Visual Studio 2017) produces a single managed assembly, DotNetCasClient.dll, that may be included as a dependency of another project.  However, it is recommended you install the NuGet package instead of referencing the DotNetCasClient.dll directly in a project because the NuGet package will handle adding/removing/updating other dependencies and references for you.

In addition to adding a dependency, the CAS integration must be configured via the web.config file. See the [Integration Instructions](#integration-instructions) below for details.

### Integration Instructions

The .NET CAS client integrates with ASP.NET applications by customizing the application web.config file. The client is implemented as an ASP.NET IHttpModule, CasAuthenticationModule, that provides hooks into the ASP.NET request/response pipeline through lifecycle events. This provides a familiar configuration path for client integration, including the following:

- Custom `casClientConfig` section containing CAS-specific configuration parameters that apply to `CasAuthenticationModule`
- ASP.NET forms authentication
- Registration of CasAuthenticationModule
- Authorization configuration
- Logging configuration (optional)

The `CasAuthenticationModule` must be made available to the application via the familiar process for .NET assemblies; either of the following is sufficient:

- Ensure it is deployed to the /Bin directory of the Web application
- Add it to the .NET Global Assembly Cache
- Configure `CasAuthenticationModule`

Define the casClientConfig configuration section:

- Register casClientConfig Section

```xml
<configSections>
  <section name="casClientConfig"
           type="DotNetCasClient.Configuration.CasClientConfiguration, DotNetCasClient"/>
  <!-- Other custom sections here -->
</configSections>
```

Place a `<casClientConfig>` configuration element directly under the root `<configuration>` element. The position of the `<casClientConfig>` element in the web.config file is unimportant.

```xml
<casClientConfig
  casServerLoginUrl="https://server.example.com/cas/login"
  casServerUrlPrefix="https://server.example.com/cas/"
  serverName="https://client.example.com:8443"
  notAuthorizedUrl="~/NotAuthorized.aspx"
  cookiesRequiredUrl="~/CookiesRequired.aspx"
  redirectAfterValidation="true"
  renew="false"
  singleSignOut="true"
  ticketValidatorName="Cas20"
  serviceTicketManager="CacheServiceTicketManager" />
```

The following attributes are supported in the casClientConfig configuration section.

| Attribute | Description | Required | 
| ----------| -----------| ----------|
| `casServerLoginUrl` | URL of CAS login form. | Yes
| `serverName ` | Host name of the server hosting this application. This is used to generate URLs that will be sent to the CAS server for redirection. The CAS server must be able to resolve this host name. If your web application is behind a load balancer, SSL offloader, or any other type of device that accepts incoming requests on behalf of the web application, you will generally need to supply the public facing host name unless your CAS server is in the same private network as the application server. The protocol prefix is optional (http:// or https://). If you are using a non-standard port number, be sure to include it (i.e., server.school.edu:8443 or https://server.school.edu:8443). Do not include the trailing backslash. | Yes
| `casServerUrlPrefix ` | URL to root of CAS server application. | Yes
| `ticketValidatorName ` | Name of ticket validator that validates CAS tickets using a particular protocol. Valid values are `Cas10, Cas20, and Saml11`. | Yes
| `gateway` | Enable CAS gateway feature, see http://www.jasig.org/cas/protocol section 2.1.1. Default is false. | No
| `renew` | Force user to reauthenticate to CAS before accessing this application. This provides additional security at the cost of usability since it effectively disables SSO for this application. Default is false. | No
| `singleSignOut ` | Enables this application to receive CAS single sign-out messages sent when the user's SSO session ends. This will cause the user's session in this application to be destroyed. Default is true. | No
| `ticketTimeTolerance` | Adds the given amount of tolerance in milliseconds to the client system time when evaluating the SAML assertion validity period. This effectively allows a given amount of system clock drift between the CAS client and server. Increasing this above the default value may have negative security consequences; we recommend fixing sources of clock drift rather than increasing this value. This configuration parameter is only meaningful in conjunction with `ticketValidatorName="Saml11"`. Default value is `30000`. | No
| `notAuthorizedUrl` | The URL to redirect to when the request has a valid CAS ticket but the user is not authorized to access the URL or resource. If this option is set, users will be redirected to this URL. If it is not set, the user will be redirected to the CAS login screen with a Renew option in the URL (to force for alternate credential collection). | No
| `serviceTicketManager` | The service ticket manager to use to store tickets returned by the CAS server for validation, revocation, and single sign out support. Without a ticket manager configured, these capabilities will be disabled. Valid value is `CacheTicketManager`. | No
| `proxyTicketManager ` | The proxy ticket manager to use to maintain state during proxy ticket requests. Without a proxy ticket manager configured your application will not be able to issue proxy tickets. | No
| `gatewayStatusCookieName ` | The name of the cookie used to store the Gateway status (NotAttempted, Success, Failed). This cookie is used to prevent the client from attempting to gateway authenticate every request. Default value is `cas_gateway_status`. | No
| `cookiesRequiredUrl ` | The URL to redirect to when the client is not accepting session cookies. This condition is detected only when gateway is enabled. This will lock the users onto a specific page. Otherwise, every request will cause a silent round-trip to the CAS server, adding a parameter to the URL. | No

#### Register CasAuthenticationModule

Register `CasAuthenticationModule` with the ASP.NET pipeline by adding it to the `<system.web><httpModules>` and `<system.webServer><modules>` sections as demonstrated in the following configuration blocks.

##### Register with httpModules Section

```xml
<system.web>
  <!-- Other system.web elements here -->
  <httpModules>
    <add name="DotNetCasClient"
         type="DotNetCasClient.CasAuthenticationModule,DotNetCasClient"/>
    <!-- Other modules here -->
  </httpModules>
</system.web>
```

##### Register with modules Section

```xml
<system.webServer>
  <!--
   Disabled Integrated Mode configuration validation.
   This will allow a single deployment to  run on IIS 5/6 and 7+
   without errors
  -->
  <validation validateIntegratedModeConfiguration="false"/>
  <modules>
  <!--
   Remove and Add the CasAuthenticationModule into the IIS7+
   Integrated Pipeline.  This has no effect on IIS5/6.
  -->
  <remove name="DotNetCasClient"/>
  <add name="DotNetCasClient"
       type="DotNetCasClient.CasAuthenticationModule,DotNetCasClient"/>
  <!-- Other modules here -->
  </modules>
</system.webServer>
```

#### Configure ASP.NET Forms Authentication
Configure the ASP.NET Forms authentication section, `<forms>`, so that it points to the login URL of the CAS server defined in the casServerLoginUrl attribute of the casClientConfig section. It is vitally important that the CAS login URL is the same in both locations.


```xml
<system.web>
  <authentication mode="Forms">
    <forms
      loginUrl="https://server.example.com/cas/login"
      timeout="30"
      defaultUrl="~/Default.aspx"
      cookieless="UseCookies"
      slidingExpiration="true"
      path="/ApplicationName/" />
  </authentication>
  <!-- Other system.web elements here -->
</system.web>
```

#### Configure Authorization

Configure authorization roles and resources using the familiar ASP.NET directives. We recommend the user of a role provider that queries a role store given the principal name returned from the CAS server. There is not support at present for extracting authorization data from the attributes released from CAS via the SAML protocol.

#### Configure Diagnostic Tracing (optional)

`CasAuthenticationModule` uses the .NET Framework `System.Diagnostics` tracing facility for internal logging. Enabling the internal trace switches should be the first step taken to troubleshoot integration problems.

`System.Diagnostics` tracing requires that the source be compiled with the /TRACE compiler option in order to produce output to trace listeners. The binary distributions of the .NET CAS Client provided here on GitHub and as NuGet packages are compiled in RELEASE mode with the /TRACE option enabled. (Note: binary distributions prior to version 1.1.0 are compiled in DEBUG mode with the /TRACE option enabled.)

The following web.config configuration section provides a sample trace configuration that should be used to troubleshoot integration problems.

```xml
<system.diagnostics>
  <trace autoflush="true" useGlobalLock="false" />
  <sharedListeners>
    <!--
      Writing trace output to a log file is recommended.
      IMPORTANT:
      The user account under which the containing application pool runs
      must have privileges to create and modify the trace log file.
    -->
    <add name="TraceFile"
         type="System.Diagnostics.TextWriterTraceListener"
         initializeData="C:\inetpub\logs\LogFiles\DotNetCasClient.Log"
         traceOutputOptions="DateTime" />
  </sharedListeners>
  <sources>
    <!-- Provides diagnostic information on module configuration parameters. -->
    <source name="DotNetCasClient.Config" switchName="Config" switchType="System.Diagnostics.SourceSwitch" >
      <listeners>
        <add name="TraceFile" />
      </listeners>
    </source>
    <!-- Traces IHttpModule lifecycle events and meaningful operations performed therein. -->
    <source name="DotNetCasClient.HttpModule" switchName="HttpModule" switchType="System.Diagnostics.SourceSwitch" >
      <listeners>
        <add name="TraceFile" />
      </listeners>
    </source>
    <!-- Provides protocol message and routing information. -->
    <source name="DotNetCasClient.Protocol" switchName="Protocol" switchType="System.Diagnostics.SourceSwitch" >
      <listeners>
        <add name="TraceFile" />
      </listeners>
    </source>
    <!-- Provides details on security operations and notable security conditions. -->
    <source name="DotNetCasClient.Security" switchName="Security" switchType="System.Diagnostics.SourceSwitch" >
      <listeners>
        <add name="TraceFile" />
      </listeners>
    </source>
  </sources>
  <switches>
    <!--
      Set trace switches to appropriate logging level.  Recommended values in order of increasing verbosity:
       - Off
       - Error
       - Warning
       - Information
       - Verbose
    -->
    <!--
      Config category displays detailed information about CasAuthenticationModule configuration.
      The output of this category is only displayed when the module is initialized, which happens
      for the first request following application/server startup.
    -->
    <add name="Config" value="Information"/>
    <!--
      Set this category to Verbose to trace HttpModule lifecycle events in CasAuthenticationModule.
      This category produces voluminous output in Verbose mode and should be avoided except for
      limited periods of time troubleshooting vexing integration problems.
    -->
    <add name="HttpModule" value="Information"/>
    <!--
      Set to Verbose to display protocol messages between the client and server.
      This category is very helpful for troubleshooting integration problems.
    -->
    <add name="Protocol" value="Verbose"/>
    <!--
      Displays important security-related information.
    -->
    <add name="Security" value="Information"/>
  </switches>
</system.diagnostics>
```

The configuration above will produce trace output to the file `C:\inetpub\logs\LogFiles\DotNetCasClient.Log`. This file path is only representative; a convenient and accessible path should be chosen based on deployer requirements.

**Note**: The application pool in which the CAS-enabled .NET application runs must execute under a user with permission to create and write the trace log file.
