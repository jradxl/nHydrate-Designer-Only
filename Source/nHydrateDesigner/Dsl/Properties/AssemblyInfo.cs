#region Using directives

using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;

#endregion

//
// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
//
[assembly: AssemblyTitle(@"")]
[assembly: AssemblyDescription(@"")]
[assembly: AssemblyConfiguration("")]
//[assembly: AssemblyCompany(@"Company")]
[assembly: AssemblyProduct(@"nHydrateDesignerYY")]
//[assembly: AssemblyCopyright("")]
//[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
//[assembly: System.Resources.NeutralResourcesLanguage("en")]

//
// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Revision and Build Numbers 
// by using the '*' as shown below:

//[assembly: AssemblyVersion(@"1.0.0.0")]
[assembly: ComVisible(false)]
[assembly: CLSCompliant(true)]
[assembly: ReliabilityContract(Consistency.MayCorruptProcess, Cer.None)]

//
// Make the Dsl project internally visible to the DslPackage assembly
//
[assembly: InternalsVisibleTo(@"nHydrate2.DslPackage, PublicKey=0024000004800000940000000602000000240000525341310004000001000100578697E33453F13B77DE3E5C260972DAF16C53A0DBAC4C8B38D6F96EC8F9C44278D6B0CED0A2CE0B8981FB2C849F0CD5C4FC73B2DCAE44D7CBE4D50271B6ED759F09E4C5BB1864F24EDF7D4D93362517DC2C16DF3647F132C8B4DC085AAA9F3C196002E95811D81D13563B8AAAA31C025F24766522783CB4C428FE0F414D4CAC")]