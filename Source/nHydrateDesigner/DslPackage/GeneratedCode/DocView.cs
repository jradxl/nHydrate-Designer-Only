﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using DslModeling = global::Microsoft.VisualStudio.Modeling;
using DslDiagrams = global::Microsoft.VisualStudio.Modeling.Diagrams;
using DslShell = global::Microsoft.VisualStudio.Modeling.Shell;

namespace nHydrate2.DslPackage
{
	/// <summary>
	/// Double-derived class to allow easier code customization.
	/// </summary>
	internal partial class nHydrate2DocView : nHydrate2DocViewBase
	{
		/// <summary>
		/// Constructs a new nHydrate2DocView.
		/// </summary>
		public nHydrate2DocView(DslShell::ModelingDocData docData, global::System.IServiceProvider serviceProvider)
			: base(docData, serviceProvider)
		{
		}
	}

	/// <summary>
	/// Class that hosts the diagram surface in the Visual Studio document area.
	/// </summary>
	internal abstract class nHydrate2DocViewBase : DslShell::SingleDiagramDocView
	{
		/// <summary>
		/// Constructs a new nHydrate2DocView.
		/// </summary>
		protected nHydrate2DocViewBase(DslShell::ModelingDocData docData, global::System.IServiceProvider serviceProvider) : base(docData, serviceProvider)
		{
		}

		/// <summary>
		/// Called to initialize the view after the corresponding document has been loaded.
		/// </summary>
		protected override bool LoadView()
		{
			base.LoadView();

			global::System.Diagnostics.Debug.Assert(this.DocData.RootElement!=null);
			if (this.DocData.RootElement == null)
			{
				return false;
			}

			// The diagram should exist in the diagram partition by now, just need to find it and connect it to this view.
			nHydrate2DocDataBase docData = this.DocData as nHydrate2DocDataBase;
			global::System.Diagnostics.Debug.Assert(docData != null, "DocData for nHydrate2DocViewBase should be an nHydrate2DocDataBase!");
			DslModeling::Partition diagramPartition = docData.GetDiagramPartition();
			if (diagramPartition != null)
			{
				global::System.Collections.ObjectModel.ReadOnlyCollection<global::nHydrate2.Dsl.nHydrateDiagram> diagrams = docData.GetDiagramPartition().ElementDirectory.FindElements<global::nHydrate2.Dsl.nHydrateDiagram>();
				if (diagrams.Count > 0)
				{
					global::System.Diagnostics.Debug.Assert(diagrams.Count == 1, "Found more than one diagram, using the first one found.");
					this.Diagram = (DslDiagrams::Diagram)diagrams[0];
				}
				else
				{
					return false;
				}
			}
			else
			{
				return false;
			}

			return true;
		}

		/// <summary>
		/// Name of the toolbox tab that should be displayed when the diagram is opened.
		/// </summary>
		protected override string DefaultToolboxTabName
		{
			get
			{
				return global::nHydrate2.Dsl.nHydrate2ToolboxHelper.DefaultToolboxTabName;
			}
		}
		
		/// <summary>
		/// Returns the toolbox items count in the default tool box tab.
		/// </summary>
		protected override int DefaultToolboxTabToolboxItemsCount
		{
			get
			{
				return global::nHydrate2.Dsl.nHydrate2ToolboxHelper.DefaultToolboxTabToolboxItemsCount;			
			}
		}
		
		
		/// <summary>
		/// Context menu displayed when the user right-clicks on the design surface.
		/// </summary>
		protected override global::System.ComponentModel.Design.CommandID ContextMenuId
		{
			get
			{
				return Constants.nHydrate2DiagramMenu;
			}
		}
			
		/// <summary>
		/// Called when selection changes in this window.
		/// </summary>
		/// <remarks>
		/// Overriden to update the F1 help keyword for the selection.
		/// </remarks>
		/// <param name="e"></param>
		protected override void OnSelectionChanged(global::System.EventArgs e)
		{
			base.OnSelectionChanged(e);

			if(global::nHydrate2.Dsl.nHydrate2HelpKeywordHelper.Instance != null)
			{
				DslModeling::ModelElement selectedElement = this.PrimarySelection as DslModeling::ModelElement;
				if(selectedElement != null)
				{
					string f1Keyword = global::nHydrate2.Dsl.nHydrate2HelpKeywordHelper.Instance.GetHelpKeyword(selectedElement);

					// If this is a presentation element, check the underlying model element for a help keyword
					DslDiagrams::PresentationElement presentationElement = this.PrimarySelection as DslDiagrams::PresentationElement;
					if(presentationElement != null)
					{
						selectedElement = presentationElement.ModelElement;
						if(selectedElement != null)
						{
							string modelElementKeyword = global::nHydrate2.Dsl.nHydrate2HelpKeywordHelper.Instance.GetHelpKeyword(selectedElement);
							if(string.IsNullOrEmpty(f1Keyword))
							{
								// Presentation element does not have an F1 keyword, so push the keyword from the model element as an F1 keyword.
								f1Keyword = modelElementKeyword;
							}
							else if (!string.IsNullOrEmpty(modelElementKeyword) && this.SelectionHelpService != null)
							{
								// Presentation element has an F1 keyword, so push model element keyword as a general dynamic help keyword (non-F1).
								this.SelectionHelpService.AddContextAttribute(string.Empty, modelElementKeyword, global::System.ComponentModel.Design.HelpKeywordType.GeneralKeyword);
							}
						}
					}
					
					if(!string.IsNullOrEmpty(f1Keyword) && this.SelectionHelpService != null)
					{
						this.SelectionHelpService.AddContextAttribute(string.Empty, f1Keyword, global::System.ComponentModel.Design.HelpKeywordType.F1Keyword);
					}
				}
			}
		}
	}
}

