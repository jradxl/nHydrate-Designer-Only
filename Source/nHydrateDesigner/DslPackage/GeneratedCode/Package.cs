﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using VSShellInterop = global::Microsoft.VisualStudio.Shell.Interop;
using VSShell = global::Microsoft.VisualStudio.Shell;
using DslShell = global::Microsoft.VisualStudio.Modeling.Shell;
using DslDesign = global::Microsoft.VisualStudio.Modeling.Design;
using DslModeling = global::Microsoft.VisualStudio.Modeling;
using System;
using System.Diagnostics;
using System.Drawing.Design;
using System.Linq;
using System.Windows.Forms;
	
namespace nHydrate2.DslPackage
{
	/// <summary>
	/// This class implements the VS package that integrates this DSL into Visual Studio.
	/// </summary>
	[VSShell::DefaultRegistryRoot("Software\\Microsoft\\VisualStudio\\11.0")]
	[VSShell::PackageRegistration(RegisterUsing = VSShell::RegistrationMethod.Assembly, UseManagedResourcesOnly = true)]
	[VSShell::ProvideToolWindow(typeof(nHydrate2ExplorerToolWindow), MultiInstances = false, Style = VSShell::VsDockStyle.Tabbed, Orientation = VSShell::ToolWindowOrientation.Right, Window = "{3AE79031-E1BC-11D0-8F78-00A0C9110057}")]
	[VSShell::ProvideToolWindowVisibility(typeof(nHydrate2ExplorerToolWindow), Constants.nHydrate2EditorFactoryId)]
	[VSShell::ProvideStaticToolboxGroup("@nHydrate DesignerToolboxTab;nHydrate2.Dsl.dll", "nHydrate2.DslPackage.nHydrate DesignerToolboxTab")]
	[VSShell::ProvideStaticToolboxItem("nHydrate2.DslPackage.nHydrate DesignerToolboxTab",
					"@EntityToolboxItem;nHydrate2.Dsl.dll", 
					"nHydrate2.DslPackage.EntityToolboxItem", 
					"CF_TOOLBOXITEMCONTAINER,CF_TOOLBOXITEMCONTAINER_HASH,CF_TOOLBOXITEMCONTAINER_CONTENTS", 
					"Entity", 
					"@EntityToolboxBitmap;nHydrate2.Dsl.dll", 
					0xff00ff)]
	[VSShell::ProvideStaticToolboxItem("nHydrate2.DslPackage.nHydrate DesignerToolboxTab",
					"@AssociationToolboxItem;nHydrate2.Dsl.dll", 
					"nHydrate2.DslPackage.AssociationToolboxItem", 
					"CF_TOOLBOXITEMCONTAINER,CF_TOOLBOXITEMCONTAINER_HASH,CF_TOOLBOXITEMCONTAINER_CONTENTS", 
					"", 
					"@AssociationToolboxBitmap;nHydrate2.Dsl.dll", 
					0xff00ff)]
	[VSShell::ProvideStaticToolboxItem("nHydrate2.DslPackage.nHydrate DesignerToolboxTab",
					"@InheritanceToolboxItem;nHydrate2.Dsl.dll", 
					"nHydrate2.DslPackage.InheritanceToolboxItem", 
					"CF_TOOLBOXITEMCONTAINER,CF_TOOLBOXITEMCONTAINER_HASH,CF_TOOLBOXITEMCONTAINER_CONTENTS", 
					"", 
					"@InheritanceToolboxBitmap;nHydrate2.Dsl.dll", 
					0xff00ff)]
	[VSShell::ProvideStaticToolboxItem("nHydrate2.DslPackage.nHydrate DesignerToolboxTab",
					"@ViewToolboxItem;nHydrate2.Dsl.dll", 
					"nHydrate2.DslPackage.ViewToolboxItem", 
					"CF_TOOLBOXITEMCONTAINER,CF_TOOLBOXITEMCONTAINER_HASH,CF_TOOLBOXITEMCONTAINER_CONTENTS", 
					"View", 
					"@ViewToolboxBitmap;nHydrate2.Dsl.dll", 
					0xff00ff)]
	[VSShell::ProvideStaticToolboxItem("nHydrate2.DslPackage.nHydrate DesignerToolboxTab",
					"@FunctionToolboxItem;nHydrate2.Dsl.dll", 
					"nHydrate2.DslPackage.FunctionToolboxItem", 
					"CF_TOOLBOXITEMCONTAINER,CF_TOOLBOXITEMCONTAINER_HASH,CF_TOOLBOXITEMCONTAINER_CONTENTS", 
					"Function", 
					"@FunctionToolboxBitmap;nHydrate2.Dsl.dll", 
					0xff00ff)]
	[VSShell::ProvideStaticToolboxItem("nHydrate2.DslPackage.nHydrate DesignerToolboxTab",
					"@StoredProcedureToolboxItem;nHydrate2.Dsl.dll", 
					"nHydrate2.DslPackage.StoredProcedureToolboxItem", 
					"CF_TOOLBOXITEMCONTAINER,CF_TOOLBOXITEMCONTAINER_HASH,CF_TOOLBOXITEMCONTAINER_CONTENTS", 
					"StoredProcedure", 
					"@StoredProcedureToolboxBitmap;nHydrate2.Dsl.dll", 
					0xff00ff)]
	[VSShell::ProvideStaticToolboxItem("nHydrate2.DslPackage.nHydrate DesignerToolboxTab",
					"@ViewLinkToolboxItem;nHydrate2.Dsl.dll", 
					"nHydrate2.DslPackage.ViewLinkToolboxItem", 
					"CF_TOOLBOXITEMCONTAINER,CF_TOOLBOXITEMCONTAINER_HASH,CF_TOOLBOXITEMCONTAINER_CONTENTS", 
					"", 
					"@ViewLinkToolboxBitmap;nHydrate2.Dsl.dll", 
					0xff00ff)]
	[VSShell::ProvideEditorFactory(typeof(nHydrate2EditorFactory), 103, TrustLevel = VSShellInterop::__VSEDITORTRUSTLEVEL.ETL_AlwaysTrusted)]
	[VSShell::ProvideEditorExtension(typeof(nHydrate2EditorFactory), "." + Constants.DesignerFileExtension, 50)]
	[VSShell::ProvideEditorLogicalView(typeof(nHydrate2EditorFactory), "{7651A702-06E5-11D1-8EBD-00A0C90F26EA}")] // Designer logical view GUID i.e. VSConstants.LOGVIEWID_Designer
	[DslShell::ProvideRelatedFile("." + Constants.DesignerFileExtension, Constants.DefaultDiagramExtension,
		ProjectSystem = DslShell::ProvideRelatedFileAttribute.CSharpProjectGuid,
		FileOptions = DslShell::RelatedFileType.FileName)]
	[DslShell::ProvideRelatedFile("." + Constants.DesignerFileExtension, Constants.DefaultDiagramExtension,
		ProjectSystem = DslShell::ProvideRelatedFileAttribute.VisualBasicProjectGuid,
		FileOptions = DslShell::RelatedFileType.FileName)]
	[DslShell::RegisterAsDslToolsEditor]
	[global::System.Runtime.InteropServices.ComVisible(true)]
	[DslShell::ProvideBindingPath]
	[DslShell::ProvideXmlEditorChooserBlockSxSWithXmlEditor(@"nHydrate2", typeof(nHydrate2EditorFactory))]

	internal abstract partial class nHydrate2PackageBase : DslShell::ModelingPackage
	{
		protected global::nHydrate2.Dsl.nHydrate2ToolboxHelper toolboxHelper;	
		
		/// <summary>
		/// Initialization method called by the package base class when this package is loaded.
		/// </summary>
		protected override void Initialize()
		{
			base.Initialize();

			// Register the editor factory used to create the DSL editor.
			this.RegisterEditorFactory(new nHydrate2EditorFactory(this));
			
			// Initialize the toolbox helper
			toolboxHelper = new global::nHydrate2.Dsl.nHydrate2ToolboxHelper(this);

			// Create the command set that handles menu commands provided by this package.
			nHydrate2CommandSet commandSet = new nHydrate2CommandSet(this);
			commandSet.Initialize();
			
			// Create the command set that handles cut/copy/paste commands provided by this package.
			nHydrate2ClipboardCommandSet clipboardCommandSet = new nHydrate2ClipboardCommandSet(this);
			clipboardCommandSet.Initialize();
			
			// Register the model explorer tool window for this DSL.
			this.AddToolWindow(typeof(nHydrate2ExplorerToolWindow));

			// Initialize Extension Registars
			// this is a partial method call
			this.InitializeExtensions();

			// Add dynamic toolbox items
			this.SetupDynamicToolbox();
		}

		/// <summary>
		/// Partial method to initialize ExtensionRegistrars (if any) in the DslPackage
		/// </summary>
		partial void InitializeExtensions();
		
		/// <summary>
		/// Returns any dynamic tool items for the designer
		/// </summary>
		/// <remarks>The default implementation is to return the list of items from the generated toolbox helper.</remarks>
		protected override global::System.Collections.Generic.IList<DslDesign::ModelingToolboxItem> CreateToolboxItems()
		{
			try
			{
				Debug.Assert(toolboxHelper != null, "Toolbox helper is not initialized");
				return toolboxHelper.CreateToolboxItems();
			}
			catch(global::System.Exception e)
			{
				global::System.Diagnostics.Debug.Fail("Exception thrown during toolbox item creation.  This may result in Package Load Failure:\r\n\r\n" + e);
				throw;
			}
		}
		
		
		/// <summary>
		/// Given a toolbox item "unique ID" and a data format identifier, returns the content of
		/// the data format. 
		/// </summary>
		/// <param name="itemId">The unique ToolboxItem to retrieve data for</param>
		/// <param name="format">The desired format of the resulting data</param>
		protected override object GetToolboxItemData(string itemId, DataFormats.Format format)
		{
			Debug.Assert(toolboxHelper != null, "Toolbox helper is not initialized");
		
			// Retrieve the specified ToolboxItem from the DSL
			return toolboxHelper.GetToolboxItemData(itemId, format);
		}
	}

}

//
// Package attributes which may need to change are placed on the partial class below, rather than in the main include file.
//
namespace nHydrate2.DslPackage
{
    /// <summary>
    /// Double-derived class to allow easier code customization.
    /// </summary>
    [VSShell::ProvideMenuResource("1000.ctmenu", 2)]
    [VSShell::ProvideToolboxItems(1)]
    [global::Microsoft.VisualStudio.TextTemplating.VSHost.ProvideDirectiveProcessor(typeof(global::nHydrate2.Dsl.nHydrate2DirectiveProcessor), global::nHydrate2.Dsl.nHydrate2DirectiveProcessor.nHydrate2DirectiveProcessorName, "A directive processor that provides access to nHydrate2 files")]
    [global::System.Runtime.InteropServices.Guid(Constants.nHydrate2PackageId)]
    internal sealed partial class nHydrate2Package : nHydrate2PackageBase
    {
    }
}