﻿using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Test.KSoft.BCL")]
[assembly: AssemblyDescription("")]

[assembly: AssemblyProduct("Test.KSoft.BCL")]

// Setting ComVisible to false makes the types in this assembly not visible
// to COM components.  If you need to access a type in this assembly from
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("2e3f7664-9f4a-413b-9bc1-39401af0c665")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("0.0.*")]

[assembly: SuppressMessage("Microsoft.Design",
	"CA1303:DoNotPassLiteralsAsLocalizedParameters",
	Justification = "I'm not localizing this stuff, go away")]
[assembly: SuppressMessage("Microsoft.Design",
	"CA1707:IdentifiersShouldNotContainUnderscores",
	Justification="Because I do this all over the place in unit tests")]
