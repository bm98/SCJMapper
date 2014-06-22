//--------------------------------------------------------------------------------------
// File: AssemblyInfo.cs
//
// Assembly information for all managed samples
//
// Copyright (c) Microsoft Corporation. All rights reserved.
//--------------------------------------------------------------------------------------

using System;
using System.Security;
using System.Security.Permissions;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

// Set assembly information
[assembly: AssemblyTitle( "SC Joystick Mapper" )]
[assembly: AssemblyDescription( "SC Joystick mapping tool" )]
[assembly: AssemblyCompany( "Cassini (SC handle)" )]
[assembly: AssemblyProduct( "SCJMapper" )]
[assembly: AssemblyCopyright( "Copyright (c) 2014 M.Burri" )]
// Update version
[assembly: AssemblyVersion( "1.3.00.7" )]

// We will use UInt which isn't CLS compliant, possible unsafe code as well
[assembly: CLSCompliant(false)]
[assembly: ComVisible(false)]

// Security information
[assembly: SecurityPermissionAttribute(SecurityAction.RequestMinimum, UnmanagedCode=true)]
[assembly: SecurityPermissionAttribute(SecurityAction.RequestMinimum, Execution=true)]

// We want to be able to read the registry key
[assembly: RegistryPermissionAttribute(SecurityAction.RequestMinimum,Read="HKEY_LOCAL_MACHINE\\Software\\Microsoft\\DirectX SDK")]
[assembly: AssemblyFileVersionAttribute( "1.3.00.7" )]
[assembly: GuidAttribute( "D287A6AA-1492-450F-8BF2-5E3523FE9C9B" )]
