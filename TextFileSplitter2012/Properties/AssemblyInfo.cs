using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
#if SQL2019
[assembly: AssemblyTitle("TextFileSplitter2019")]
[assembly: AssemblyProduct("TextFileSplitter2019")]
#endif
#if SQL2017
[assembly: AssemblyTitle("TextFileSplitter2017")]
[assembly: AssemblyProduct("TextFileSplitter2017")]
#endif
#if SQL2016
[assembly: AssemblyTitle("TextFileSplitter2016")]
[assembly: AssemblyProduct("TextFileSplitter2016")]
#endif
#if SQL2014
[assembly: AssemblyTitle("TextFileSplitter2014")]
[assembly: AssemblyProduct("TextFileSplitter2014")]
#endif
#if SQL2012
[assembly: AssemblyTitle("TextFileSplitter2012")]
[assembly: AssemblyProduct("TextFileSplitter2012")]
#endif
#if SQL2008
[assembly: AssemblyTitle("TextFileSplitter2008")]
[assembly: AssemblyProduct("TextFileSplitter2008")]
#endif

[assembly: AssemblyDescription("SSIS Component to split a text file into many outputs")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("None")]
[assembly: AssemblyCopyright("Copyright © Keith Martin 2021")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("12a41826-6155-4ec2-bbc2-16b5bd3747fb")]

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
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]
[assembly: InternalsVisibleTo("UnitTestTextFileSplitter, PublicKey=0024000004800000940000000602000000240000525341310004000001000100a75465ee722f3822e103de4cd89e88401e918e294653d5fd144ad421ddd9f07b8182056b27496d06110884a801c8c6148af44d1177f403885e861448d64c1402358e996399ba57be54cb4b045939199da1dba5c87391313377369830cc1ceab47b3dec3f45cf7ee93ff91e819d46c01c3ffab985505ac73a6e6abe0a611f94db")]