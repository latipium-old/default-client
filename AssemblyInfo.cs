// AssemblyInfo.cs
//
// Copyright (c) 2016 Zach Deibert.
// All Rights Reserved.
using System;
using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: AssemblyTitle("Com.Latipium.Defaults.Client")]
[assembly: AssemblyDescription("The default client implementation for Latipium")]
#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif
[assembly: AssemblyCompany("Latipium")]
[assembly: AssemblyProduct("Com.Latipium.Client")]
[assembly: AssemblyCopyright("Zach Deibert")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

[assembly: AssemblyVersion("1.0.*")]

[assembly: CLSCompliant(true)]
