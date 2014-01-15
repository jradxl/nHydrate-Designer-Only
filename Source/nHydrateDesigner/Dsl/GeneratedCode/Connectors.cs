﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using DslModeling = global::Microsoft.VisualStudio.Modeling;
using DslDesign = global::Microsoft.VisualStudio.Modeling.Design;
using DslDiagrams = global::Microsoft.VisualStudio.Modeling.Diagrams;

namespace nHydrate2.Dsl
{
	/// <summary>
	/// Double-derived base class for DomainClass EntityAssociationConnector
	/// </summary>
	[DslDesign::DisplayNameResource("nHydrate2.Dsl.EntityAssociationConnector.DisplayName", typeof(global::nHydrate2.Dsl.nHydrate2DomainModel), "nHydrate2.Dsl.GeneratedCode.DomainModelResx")]
	[DslDesign::DescriptionResource("nHydrate2.Dsl.EntityAssociationConnector.Description", typeof(global::nHydrate2.Dsl.nHydrate2DomainModel), "nHydrate2.Dsl.GeneratedCode.DomainModelResx")]
	[DslModeling::DomainModelOwner(typeof(global::nHydrate2.Dsl.nHydrate2DomainModel))]
	[global::System.CLSCompliant(true)]
	[DslModeling::DomainObjectId("00d9c38d-d3b6-458e-b0c2-1b9603825d3d")]
	public abstract partial class EntityAssociationConnectorBase : DslDiagrams::BinaryLinkShape, System.ComponentModel.INotifyPropertyChanged
	{
		#region DiagramElement boilerplate
		private static DslDiagrams::StyleSet classStyleSet;
		private static global::System.Collections.Generic.IList<DslDiagrams::ShapeField> shapeFields;
		private static global::System.Collections.Generic.IList<DslDiagrams::Decorator> decorators;
		
		/// <summary>
		/// Per-class style set for this shape.
		/// </summary>
		protected override DslDiagrams::StyleSet ClassStyleSet
		{
			get
			{
				if (classStyleSet == null)
				{
					classStyleSet = CreateClassStyleSet();
				}
				return classStyleSet;
			}
		}
		
		/// <summary>
		/// Per-class ShapeFields for this shape.
		/// </summary>
		public override global::System.Collections.Generic.IList<DslDiagrams::ShapeField> ShapeFields
		{
			get
			{
				if (shapeFields == null)
				{
					shapeFields = CreateShapeFields();
				}
				return shapeFields;
			}
		}
		
		/// <summary>
		/// Event fired when decorator initialization is complete for this shape type.
		/// </summary>
		public static event global::System.EventHandler DecoratorsInitialized;
		
		/// <summary>
		/// List containing decorators used by this type.
		/// </summary>
		public override global::System.Collections.Generic.IList<DslDiagrams::Decorator> Decorators
		{
			get 
			{
				if(decorators == null)
				{
					decorators = CreateDecorators();
					
					// fire this event to allow the diagram to initialize decorator mappings for this shape type.
					if(DecoratorsInitialized != null)
					{
						DecoratorsInitialized(this, global::System.EventArgs.Empty);
					}
				}
				
				return decorators; 
			}
		}
		
		/// <summary>
		/// Finds a decorator associated with EntityAssociationConnector.
		/// </summary>
		public static DslDiagrams::Decorator FindEntityAssociationConnectorDecorator(string decoratorName)
		{	
			if(decorators == null) return null;
			return DslDiagrams::ShapeElement.FindDecorator(decorators, decoratorName);
		}
		
		
		/// <summary>
		/// Shape instance initialization.
		/// </summary>
		public override void OnInitialize()
		{
			base.OnInitialize();
			
			// Create host shapes for outer decorators.
			foreach(DslDiagrams::Decorator decorator in this.Decorators)
			{
				if(decorator.RequiresHost)
				{
					decorator.ConfigureHostShape(this);
				}
			}
			
		}
		#endregion
		
		#region Connector styles
		/// <summary>
		/// Initializes style set resources for this shape type
		/// </summary>
		/// <param name="classStyleSet">The style set for this shape class</param>
		protected override void InitializeResources(DslDiagrams::StyleSet classStyleSet)
		{
			base.InitializeResources(classStyleSet);
			
			// Line pen settings for this connector.
			DslDiagrams::PenSettings linePen = new DslDiagrams::PenSettings();
			linePen.Color = global::System.Drawing.Color.FromKnownColor(global::System.Drawing.KnownColor.DimGray);
			classStyleSet.OverridePen(DslDiagrams::DiagramPens.ConnectionLineDecorator, linePen);
			linePen.Width = 0.01f;
			classStyleSet.OverridePen(DslDiagrams::DiagramPens.ConnectionLine, linePen);
			DslDiagrams::BrushSettings lineBrush = new DslDiagrams::BrushSettings();
			lineBrush.Color = global::System.Drawing.Color.FromKnownColor(global::System.Drawing.KnownColor.DimGray);
			classStyleSet.OverrideBrush(DslDiagrams::DiagramBrushes.ConnectionLineDecorator, lineBrush);
			
			// Custom font styles
			DslDiagrams::FontSettings fontSettings;
			fontSettings = new DslDiagrams::FontSettings();
			fontSettings.Style =  global::System.Drawing.FontStyle.Italic ;
			fontSettings.Size = 8/72.0F;
			classStyleSet.AddFont(new DslDiagrams::StyleSetResourceId(string.Empty, "ShapeTextItalic8"), DslDiagrams::DiagramFonts.ShapeText, fontSettings);
		}
		
		/// <summary>
		/// Initializes resources associated with this connector instance.
		/// </summary>
		protected override void InitializeInstanceResources()
		{
			base.InitializeInstanceResources();
			this.SetDecorators(DslDiagrams::LinkDecorator.DecoratorEmptyDiamond, new DslDiagrams::SizeD(0.1,0.1), DslDiagrams::LinkDecorator.DecoratorFilledArrow, new DslDiagrams::SizeD(0.1,0.1), false);
		}
		
		#endregion
		
		#region Decorators
		/// <summary>
		/// Initialize the collection of shape fields associated with this shape type.
		/// </summary>
		protected override void InitializeShapeFields(global::System.Collections.Generic.IList<DslDiagrams::ShapeField> shapeFields)
		{
			base.InitializeShapeFields(shapeFields);
		}
		
		/// <summary>
		/// Initialize the collection of decorators associated with this shape type.  This method also
		/// creates shape fields for outer decorators, because these are not part of the shape fields collection
		/// associated with the shape, so they must be created here rather than in InitializeShapeFields.
		/// </summary>
		protected override void InitializeDecorators(global::System.Collections.Generic.IList<DslDiagrams::ShapeField> shapeFields, global::System.Collections.Generic.IList<DslDiagrams::Decorator> decorators)
		{
			base.InitializeDecorators(shapeFields, decorators);
			
			DslDiagrams::TextField field1 = new DslDiagrams::TextField("SourceEntityRelationTextDecorator");
			field1.DefaultText = string.Empty;
			field1.DefaultFocusable = true;
			field1.DefaultAutoSize = true;
			field1.AnchoringBehavior.MinimumHeightInLines = 1;
			field1.AnchoringBehavior.MinimumWidthInCharacters = 1;
			field1.DefaultAccessibleState = global::System.Windows.Forms.AccessibleStates.Invisible;
			field1.DefaultFontId = new DslDiagrams::StyleSetResourceId(string.Empty, "ShapeTextItalic8");			
			DslDiagrams::Decorator decorator1 = new DslDiagrams::ConnectorDecorator(field1, DslDiagrams::ConnectorDecoratorPosition.SourceTop, DslDiagrams::PointD.Empty, true);
			decorators.Add(decorator1);
				
			DslDiagrams::TextField field2 = new DslDiagrams::TextField("DestEntityRelationTextDecorator");
			field2.DefaultText = string.Empty;
			field2.DefaultFocusable = true;
			field2.DefaultAutoSize = true;
			field2.AnchoringBehavior.MinimumHeightInLines = 1;
			field2.AnchoringBehavior.MinimumWidthInCharacters = 1;
			field2.DefaultAccessibleState = global::System.Windows.Forms.AccessibleStates.Invisible;
			field2.DefaultFontId = new DslDiagrams::StyleSetResourceId(string.Empty, "ShapeTextItalic8");			
			DslDiagrams::Decorator decorator2 = new DslDiagrams::ConnectorDecorator(field2, DslDiagrams::ConnectorDecoratorPosition.TargetTop, DslDiagrams::PointD.Empty, true);
			decorators.Add(decorator2);
				
		}
		
		#endregion
		
		#region INotifyPropertyChanged
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
		protected virtual void OnPropertyChanged(System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (this.PropertyChanged != null)
				this.PropertyChanged(this, e);
		}
		#endregion
	
		#region Constructors, domain class Id
	
		/// <summary>
		/// EntityAssociationConnector domain class Id.
		/// </summary>
		public static readonly new global::System.Guid DomainClassId = new global::System.Guid(0x00d9c38d, 0xd3b6, 0x458e, 0xb0, 0xc2, 0x1b, 0x96, 0x03, 0x82, 0x5d, 0x3d);
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="partition">Partition where new element is to be created.</param>
		/// <param name="propertyAssignments">List of domain property id/value pairs to set once the element is created.</param>
		protected EntityAssociationConnectorBase(DslModeling::Partition partition, DslModeling::PropertyAssignment[] propertyAssignments)
			: base(partition, propertyAssignments)
		{
		}
		#endregion
	}
	/// <summary>
	/// DomainClass EntityAssociationConnector
	/// Connect two entities
	/// </summary>
	[global::System.CLSCompliant(true)]
			
	public partial class EntityAssociationConnector : EntityAssociationConnectorBase
	{
		#region Constructors
		// Constructors were not generated for this class because it had HasCustomConstructor
		// set to true. Please provide the constructors below in a partial class.
		///// <summary>
		///// Constructor
		///// </summary>
		///// <param name="store">Store where new element is to be created.</param>
		///// <param name="propertyAssignments">List of domain property id/value pairs to set once the element is created.</param>
		//public EntityAssociationConnector(DslModeling::Store store, params DslModeling::PropertyAssignment[] propertyAssignments)
		//	: this(store != null ? store.DefaultPartitionForClass(DomainClassId) : null, propertyAssignments)
		//{
		//}
		//
		///// <summary>
		///// Constructor
		///// </summary>
		///// <param name="partition">Partition where new element is to be created.</param>
		///// <param name="propertyAssignments">List of domain property id/value pairs to set once the element is created.</param>
		//public EntityAssociationConnector(DslModeling::Partition partition, params DslModeling::PropertyAssignment[] propertyAssignments)
		//	: base(partition, propertyAssignments)
		//{
		//}
		#endregion
	}
}
namespace nHydrate2.Dsl
{
	/// <summary>
	/// Double-derived base class for DomainClass EntityInheritanceConnector
	/// </summary>
	[DslDesign::DisplayNameResource("nHydrate2.Dsl.EntityInheritanceConnector.DisplayName", typeof(global::nHydrate2.Dsl.nHydrate2DomainModel), "nHydrate2.Dsl.GeneratedCode.DomainModelResx")]
	[DslDesign::DescriptionResource("nHydrate2.Dsl.EntityInheritanceConnector.Description", typeof(global::nHydrate2.Dsl.nHydrate2DomainModel), "nHydrate2.Dsl.GeneratedCode.DomainModelResx")]
	[DslModeling::DomainModelOwner(typeof(global::nHydrate2.Dsl.nHydrate2DomainModel))]
	[global::System.CLSCompliant(true)]
	[DslModeling::DomainObjectId("1d2aa755-243b-474b-8208-5b11989ecba1")]
	public abstract partial class EntityInheritanceConnectorBase : DslDiagrams::BinaryLinkShape, System.ComponentModel.INotifyPropertyChanged
	{
		#region DiagramElement boilerplate
		private static DslDiagrams::StyleSet classStyleSet;
		private static global::System.Collections.Generic.IList<DslDiagrams::ShapeField> shapeFields;
		private static global::System.Collections.Generic.IList<DslDiagrams::Decorator> decorators;
		
		/// <summary>
		/// Per-class style set for this shape.
		/// </summary>
		protected override DslDiagrams::StyleSet ClassStyleSet
		{
			get
			{
				if (classStyleSet == null)
				{
					classStyleSet = CreateClassStyleSet();
				}
				return classStyleSet;
			}
		}
		
		/// <summary>
		/// Per-class ShapeFields for this shape.
		/// </summary>
		public override global::System.Collections.Generic.IList<DslDiagrams::ShapeField> ShapeFields
		{
			get
			{
				if (shapeFields == null)
				{
					shapeFields = CreateShapeFields();
				}
				return shapeFields;
			}
		}
		
		/// <summary>
		/// Event fired when decorator initialization is complete for this shape type.
		/// </summary>
		public static event global::System.EventHandler DecoratorsInitialized;
		
		/// <summary>
		/// List containing decorators used by this type.
		/// </summary>
		public override global::System.Collections.Generic.IList<DslDiagrams::Decorator> Decorators
		{
			get 
			{
				if(decorators == null)
				{
					decorators = CreateDecorators();
					
					// fire this event to allow the diagram to initialize decorator mappings for this shape type.
					if(DecoratorsInitialized != null)
					{
						DecoratorsInitialized(this, global::System.EventArgs.Empty);
					}
				}
				
				return decorators; 
			}
		}
		
		/// <summary>
		/// Finds a decorator associated with EntityInheritanceConnector.
		/// </summary>
		public static DslDiagrams::Decorator FindEntityInheritanceConnectorDecorator(string decoratorName)
		{	
			if(decorators == null) return null;
			return DslDiagrams::ShapeElement.FindDecorator(decorators, decoratorName);
		}
		
		#endregion
		
		#region Connector styles
		/// <summary>
		/// Initializes style set resources for this shape type
		/// </summary>
		/// <param name="classStyleSet">The style set for this shape class</param>
		protected override void InitializeResources(DslDiagrams::StyleSet classStyleSet)
		{
			base.InitializeResources(classStyleSet);
			
			// Line pen settings for this connector.
			DslDiagrams::PenSettings linePen = new DslDiagrams::PenSettings();
			linePen.Color = global::System.Drawing.Color.FromKnownColor(global::System.Drawing.KnownColor.DimGray);
			classStyleSet.OverridePen(DslDiagrams::DiagramPens.ConnectionLineDecorator, linePen);
			linePen.Width = 0.01f;
			classStyleSet.OverridePen(DslDiagrams::DiagramPens.ConnectionLine, linePen);
			DslDiagrams::BrushSettings lineBrush = new DslDiagrams::BrushSettings();
			lineBrush.Color = global::System.Drawing.Color.FromKnownColor(global::System.Drawing.KnownColor.DimGray);
			classStyleSet.OverrideBrush(DslDiagrams::DiagramBrushes.ConnectionLineDecorator, lineBrush);
			
		}
		
		/// <summary>
		/// Initializes resources associated with this connector instance.
		/// </summary>
		protected override void InitializeInstanceResources()
		{
			base.InitializeInstanceResources();
			this.SetDecorators(null, new DslDiagrams::SizeD(0.1,0.1), DslDiagrams::LinkDecorator.DecoratorHollowArrow, new DslDiagrams::SizeD(0.1,0.1), false);
		}
		
		#endregion
		
		#region INotifyPropertyChanged
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
		protected virtual void OnPropertyChanged(System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (this.PropertyChanged != null)
				this.PropertyChanged(this, e);
		}
		#endregion
	
		#region Constructors, domain class Id
	
		/// <summary>
		/// EntityInheritanceConnector domain class Id.
		/// </summary>
		public static readonly new global::System.Guid DomainClassId = new global::System.Guid(0x1d2aa755, 0x243b, 0x474b, 0x82, 0x08, 0x5b, 0x11, 0x98, 0x9e, 0xcb, 0xa1);
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="partition">Partition where new element is to be created.</param>
		/// <param name="propertyAssignments">List of domain property id/value pairs to set once the element is created.</param>
		protected EntityInheritanceConnectorBase(DslModeling::Partition partition, DslModeling::PropertyAssignment[] propertyAssignments)
			: base(partition, propertyAssignments)
		{
		}
		#endregion
		#region BaseType domain property code
		
		/// <summary>
		/// BaseType domain property Id.
		/// </summary>
		public static readonly global::System.Guid BaseTypeDomainPropertyId = new global::System.Guid(0x3837e412, 0x7144, 0x401b, 0x85, 0x72, 0x41, 0xad, 0xdd, 0xba, 0x83, 0x9f);
		
		/// <summary>
		/// Gets or sets the value of BaseType domain property.
		/// </summary>
		[DslDesign::DisplayNameResource("nHydrate2.Dsl.EntityInheritanceConnector/BaseType.DisplayName", typeof(global::nHydrate2.Dsl.nHydrate2DomainModel), "nHydrate2.Dsl.GeneratedCode.DomainModelResx")]
		[DslDesign::DescriptionResource("nHydrate2.Dsl.EntityInheritanceConnector/BaseType.Description", typeof(global::nHydrate2.Dsl.nHydrate2DomainModel), "nHydrate2.Dsl.GeneratedCode.DomainModelResx")]
		[global::System.ComponentModel.ReadOnly(true)]
		[DslModeling::DomainProperty(Kind = DslModeling::DomainPropertyKind.Calculated)]
		[DslModeling::DomainObjectId("3837e412-7144-401b-8572-41adddba839f")]
		public virtual global::System.String BaseType
		{
			[global::System.Diagnostics.DebuggerStepThrough]
			get
			{
				return BaseTypePropertyHandler.Instance.GetValue(this);
			}
		}
		/// <summary>
		/// Value handler for the EntityInheritanceConnector.BaseType domain property.
		/// </summary>
		internal sealed partial class BaseTypePropertyHandler : DslModeling::CalculatedPropertyValueHandler<EntityInheritanceConnectorBase, global::System.String>
		{
			private BaseTypePropertyHandler() { }
		
			/// <summary>
			/// Gets the singleton instance of the EntityInheritanceConnector.BaseType domain property value handler.
			/// </summary>
			public static readonly BaseTypePropertyHandler Instance = new BaseTypePropertyHandler();
		
			/// <summary>
			/// Gets the Id of the EntityInheritanceConnector.BaseType domain property.
			/// </summary>
			public sealed override global::System.Guid DomainPropertyId
			{
				[global::System.Diagnostics.DebuggerStepThrough]
				get
				{
					return BaseTypeDomainPropertyId;
				}
			}
			
			/// <summary>
			/// Gets a strongly-typed value of the property on specified element.
			/// </summary>
			/// <param name="element">Element which owns the property.</param>
			/// <returns>Property value.</returns>
			public override sealed global::System.String GetValue(EntityInheritanceConnectorBase element)
			{
				if (element == null) throw new global::System.ArgumentNullException("element");
				// There is no storage for BaseType because its Kind is
				// set to Calculated. Please provide the GetBaseTypeValue()
				// method on the domain class.
				return element.GetBaseTypeValue();
			}
		
		}
		
		#endregion
		#region DerivedType domain property code
		
		/// <summary>
		/// DerivedType domain property Id.
		/// </summary>
		public static readonly global::System.Guid DerivedTypeDomainPropertyId = new global::System.Guid(0xf3452069, 0xaecf, 0x4f34, 0x96, 0x60, 0x84, 0x94, 0x18, 0x60, 0xf4, 0x76);
		
		/// <summary>
		/// Gets or sets the value of DerivedType domain property.
		/// </summary>
		[DslDesign::DisplayNameResource("nHydrate2.Dsl.EntityInheritanceConnector/DerivedType.DisplayName", typeof(global::nHydrate2.Dsl.nHydrate2DomainModel), "nHydrate2.Dsl.GeneratedCode.DomainModelResx")]
		[DslDesign::DescriptionResource("nHydrate2.Dsl.EntityInheritanceConnector/DerivedType.Description", typeof(global::nHydrate2.Dsl.nHydrate2DomainModel), "nHydrate2.Dsl.GeneratedCode.DomainModelResx")]
		[global::System.ComponentModel.ReadOnly(true)]
		[DslModeling::DomainProperty(Kind = DslModeling::DomainPropertyKind.Calculated)]
		[DslModeling::DomainObjectId("f3452069-aecf-4f34-9660-84941860f476")]
		public virtual global::System.String DerivedType
		{
			[global::System.Diagnostics.DebuggerStepThrough]
			get
			{
				return DerivedTypePropertyHandler.Instance.GetValue(this);
			}
		}
		/// <summary>
		/// Value handler for the EntityInheritanceConnector.DerivedType domain property.
		/// </summary>
		internal sealed partial class DerivedTypePropertyHandler : DslModeling::CalculatedPropertyValueHandler<EntityInheritanceConnectorBase, global::System.String>
		{
			private DerivedTypePropertyHandler() { }
		
			/// <summary>
			/// Gets the singleton instance of the EntityInheritanceConnector.DerivedType domain property value handler.
			/// </summary>
			public static readonly DerivedTypePropertyHandler Instance = new DerivedTypePropertyHandler();
		
			/// <summary>
			/// Gets the Id of the EntityInheritanceConnector.DerivedType domain property.
			/// </summary>
			public sealed override global::System.Guid DomainPropertyId
			{
				[global::System.Diagnostics.DebuggerStepThrough]
				get
				{
					return DerivedTypeDomainPropertyId;
				}
			}
			
			/// <summary>
			/// Gets a strongly-typed value of the property on specified element.
			/// </summary>
			/// <param name="element">Element which owns the property.</param>
			/// <returns>Property value.</returns>
			public override sealed global::System.String GetValue(EntityInheritanceConnectorBase element)
			{
				if (element == null) throw new global::System.ArgumentNullException("element");
				// There is no storage for DerivedType because its Kind is
				// set to Calculated. Please provide the GetDerivedTypeValue()
				// method on the domain class.
				return element.GetDerivedTypeValue();
			}
		
		}
		
		#endregion
	}
	/// <summary>
	/// DomainClass EntityInheritanceConnector
	/// Creates an inheritance relationship between two entities
	/// </summary>
	[global::System.CLSCompliant(true)]
			
	public partial class EntityInheritanceConnector : EntityInheritanceConnectorBase
	{
		#region Constructors
		// Constructors were not generated for this class because it had HasCustomConstructor
		// set to true. Please provide the constructors below in a partial class.
		///// <summary>
		///// Constructor
		///// </summary>
		///// <param name="store">Store where new element is to be created.</param>
		///// <param name="propertyAssignments">List of domain property id/value pairs to set once the element is created.</param>
		//public EntityInheritanceConnector(DslModeling::Store store, params DslModeling::PropertyAssignment[] propertyAssignments)
		//	: this(store != null ? store.DefaultPartitionForClass(DomainClassId) : null, propertyAssignments)
		//{
		//}
		//
		///// <summary>
		///// Constructor
		///// </summary>
		///// <param name="partition">Partition where new element is to be created.</param>
		///// <param name="propertyAssignments">List of domain property id/value pairs to set once the element is created.</param>
		//public EntityInheritanceConnector(DslModeling::Partition partition, params DslModeling::PropertyAssignment[] propertyAssignments)
		//	: base(partition, propertyAssignments)
		//{
		//}
		#endregion
	}
}
namespace nHydrate2.Dsl
{
	/// <summary>
	/// Double-derived base class for DomainClass EntityCompositeConnector
	/// </summary>
	[DslDesign::DisplayNameResource("nHydrate2.Dsl.EntityCompositeConnector.DisplayName", typeof(global::nHydrate2.Dsl.nHydrate2DomainModel), "nHydrate2.Dsl.GeneratedCode.DomainModelResx")]
	[DslDesign::DescriptionResource("nHydrate2.Dsl.EntityCompositeConnector.Description", typeof(global::nHydrate2.Dsl.nHydrate2DomainModel), "nHydrate2.Dsl.GeneratedCode.DomainModelResx")]
	[DslModeling::DomainModelOwner(typeof(global::nHydrate2.Dsl.nHydrate2DomainModel))]
	[global::System.CLSCompliant(true)]
	[DslModeling::DomainObjectId("62c983a8-9996-45b6-8c2a-56a8f9b40e04")]
	public abstract partial class EntityCompositeConnectorBase : DslDiagrams::BinaryLinkShape, System.ComponentModel.INotifyPropertyChanged
	{
		#region DiagramElement boilerplate
		private static DslDiagrams::StyleSet classStyleSet;
		private static global::System.Collections.Generic.IList<DslDiagrams::ShapeField> shapeFields;
		private static global::System.Collections.Generic.IList<DslDiagrams::Decorator> decorators;
		
		/// <summary>
		/// Per-class style set for this shape.
		/// </summary>
		protected override DslDiagrams::StyleSet ClassStyleSet
		{
			get
			{
				if (classStyleSet == null)
				{
					classStyleSet = CreateClassStyleSet();
				}
				return classStyleSet;
			}
		}
		
		/// <summary>
		/// Per-class ShapeFields for this shape.
		/// </summary>
		public override global::System.Collections.Generic.IList<DslDiagrams::ShapeField> ShapeFields
		{
			get
			{
				if (shapeFields == null)
				{
					shapeFields = CreateShapeFields();
				}
				return shapeFields;
			}
		}
		
		/// <summary>
		/// Event fired when decorator initialization is complete for this shape type.
		/// </summary>
		public static event global::System.EventHandler DecoratorsInitialized;
		
		/// <summary>
		/// List containing decorators used by this type.
		/// </summary>
		public override global::System.Collections.Generic.IList<DslDiagrams::Decorator> Decorators
		{
			get 
			{
				if(decorators == null)
				{
					decorators = CreateDecorators();
					
					// fire this event to allow the diagram to initialize decorator mappings for this shape type.
					if(DecoratorsInitialized != null)
					{
						DecoratorsInitialized(this, global::System.EventArgs.Empty);
					}
				}
				
				return decorators; 
			}
		}
		
		/// <summary>
		/// Finds a decorator associated with EntityCompositeConnector.
		/// </summary>
		public static DslDiagrams::Decorator FindEntityCompositeConnectorDecorator(string decoratorName)
		{	
			if(decorators == null) return null;
			return DslDiagrams::ShapeElement.FindDecorator(decorators, decoratorName);
		}
		
		#endregion
		
		#region Connector styles
		/// <summary>
		/// Initializes style set resources for this shape type
		/// </summary>
		/// <param name="classStyleSet">The style set for this shape class</param>
		protected override void InitializeResources(DslDiagrams::StyleSet classStyleSet)
		{
			base.InitializeResources(classStyleSet);
			
			// Line pen settings for this connector.
			DslDiagrams::PenSettings linePen = new DslDiagrams::PenSettings();
			linePen.Color = global::System.Drawing.Color.FromKnownColor(global::System.Drawing.KnownColor.DimGray);
			classStyleSet.OverridePen(DslDiagrams::DiagramPens.ConnectionLineDecorator, linePen);
			linePen.Width = 0.01f;
			linePen.DashStyle = global::System.Drawing.Drawing2D.DashStyle.Dash;
			classStyleSet.OverridePen(DslDiagrams::DiagramPens.ConnectionLine, linePen);
			DslDiagrams::BrushSettings lineBrush = new DslDiagrams::BrushSettings();
			lineBrush.Color = global::System.Drawing.Color.FromKnownColor(global::System.Drawing.KnownColor.DimGray);
			classStyleSet.OverrideBrush(DslDiagrams::DiagramBrushes.ConnectionLineDecorator, lineBrush);
			
		}
		
		/// <summary>
		/// Initializes resources associated with this connector instance.
		/// </summary>
		protected override void InitializeInstanceResources()
		{
			base.InitializeInstanceResources();
			this.SetDecorators(DslDiagrams::LinkDecorator.DecoratorFilledDiamond, new DslDiagrams::SizeD(0.1,0.1), null, new DslDiagrams::SizeD(0.1,0.1), false);
		}
		
		#endregion
		
		#region INotifyPropertyChanged
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
		protected virtual void OnPropertyChanged(System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (this.PropertyChanged != null)
				this.PropertyChanged(this, e);
		}
		#endregion
	
		#region Constructors, domain class Id
	
		/// <summary>
		/// EntityCompositeConnector domain class Id.
		/// </summary>
		public static readonly new global::System.Guid DomainClassId = new global::System.Guid(0x62c983a8, 0x9996, 0x45b6, 0x8c, 0x2a, 0x56, 0xa8, 0xf9, 0xb4, 0x0e, 0x04);
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="partition">Partition where new element is to be created.</param>
		/// <param name="propertyAssignments">List of domain property id/value pairs to set once the element is created.</param>
		protected EntityCompositeConnectorBase(DslModeling::Partition partition, DslModeling::PropertyAssignment[] propertyAssignments)
			: base(partition, propertyAssignments)
		{
		}
		#endregion
	}
	/// <summary>
	/// DomainClass EntityCompositeConnector
	/// </summary>
	[global::System.CLSCompliant(true)]
			
	public partial class EntityCompositeConnector : EntityCompositeConnectorBase
	{
		#region Constructors
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="store">Store where new element is to be created.</param>
		/// <param name="propertyAssignments">List of domain property id/value pairs to set once the element is created.</param>
		public EntityCompositeConnector(DslModeling::Store store, params DslModeling::PropertyAssignment[] propertyAssignments)
			: this(store != null ? store.DefaultPartitionForClass(DomainClassId) : null, propertyAssignments)
		{
		}
		
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="partition">Partition where new element is to be created.</param>
		/// <param name="propertyAssignments">List of domain property id/value pairs to set once the element is created.</param>
		public EntityCompositeConnector(DslModeling::Partition partition, params DslModeling::PropertyAssignment[] propertyAssignments)
			: base(partition, propertyAssignments)
		{
		}
		#endregion
	}
}
namespace nHydrate2.Dsl
{
	/// <summary>
	/// DomainClass EntityViewAssociationConnector
	/// Connect an entity and a view
	/// </summary>
	[DslDesign::DisplayNameResource("nHydrate2.Dsl.EntityViewAssociationConnector.DisplayName", typeof(global::nHydrate2.Dsl.nHydrate2DomainModel), "nHydrate2.Dsl.GeneratedCode.DomainModelResx")]
	[DslDesign::DescriptionResource("nHydrate2.Dsl.EntityViewAssociationConnector.Description", typeof(global::nHydrate2.Dsl.nHydrate2DomainModel), "nHydrate2.Dsl.GeneratedCode.DomainModelResx")]
	[DslModeling::DomainModelOwner(typeof(global::nHydrate2.Dsl.nHydrate2DomainModel))]
	[global::System.CLSCompliant(true)]
	[DslModeling::DomainObjectId("1715d1db-8274-453b-8248-a36e795c89b8")]
	public partial class EntityViewAssociationConnector : DslDiagrams::BinaryLinkShape, System.ComponentModel.INotifyPropertyChanged
	{
		#region DiagramElement boilerplate
		private static DslDiagrams::StyleSet classStyleSet;
		private static global::System.Collections.Generic.IList<DslDiagrams::ShapeField> shapeFields;
		private static global::System.Collections.Generic.IList<DslDiagrams::Decorator> decorators;
		
		/// <summary>
		/// Per-class style set for this shape.
		/// </summary>
		protected override DslDiagrams::StyleSet ClassStyleSet
		{
			get
			{
				if (classStyleSet == null)
				{
					classStyleSet = CreateClassStyleSet();
				}
				return classStyleSet;
			}
		}
		
		/// <summary>
		/// Per-class ShapeFields for this shape.
		/// </summary>
		public override global::System.Collections.Generic.IList<DslDiagrams::ShapeField> ShapeFields
		{
			get
			{
				if (shapeFields == null)
				{
					shapeFields = CreateShapeFields();
				}
				return shapeFields;
			}
		}
		
		/// <summary>
		/// Event fired when decorator initialization is complete for this shape type.
		/// </summary>
		public static event global::System.EventHandler DecoratorsInitialized;
		
		/// <summary>
		/// List containing decorators used by this type.
		/// </summary>
		public override global::System.Collections.Generic.IList<DslDiagrams::Decorator> Decorators
		{
			get 
			{
				if(decorators == null)
				{
					decorators = CreateDecorators();
					
					// fire this event to allow the diagram to initialize decorator mappings for this shape type.
					if(DecoratorsInitialized != null)
					{
						DecoratorsInitialized(this, global::System.EventArgs.Empty);
					}
				}
				
				return decorators; 
			}
		}
		
		/// <summary>
		/// Finds a decorator associated with EntityViewAssociationConnector.
		/// </summary>
		public static DslDiagrams::Decorator FindEntityViewAssociationConnectorDecorator(string decoratorName)
		{	
			if(decorators == null) return null;
			return DslDiagrams::ShapeElement.FindDecorator(decorators, decoratorName);
		}
		
		#endregion
		
		#region Connector styles
		/// <summary>
		/// Initializes style set resources for this shape type
		/// </summary>
		/// <param name="classStyleSet">The style set for this shape class</param>
		protected override void InitializeResources(DslDiagrams::StyleSet classStyleSet)
		{
			base.InitializeResources(classStyleSet);
			
			// Line pen settings for this connector.
			DslDiagrams::PenSettings linePen = new DslDiagrams::PenSettings();
			linePen.Color = global::System.Drawing.Color.FromKnownColor(global::System.Drawing.KnownColor.DimGray);
			classStyleSet.OverridePen(DslDiagrams::DiagramPens.ConnectionLineDecorator, linePen);
			linePen.Width = 0.01f;
			classStyleSet.OverridePen(DslDiagrams::DiagramPens.ConnectionLine, linePen);
			DslDiagrams::BrushSettings lineBrush = new DslDiagrams::BrushSettings();
			lineBrush.Color = global::System.Drawing.Color.FromKnownColor(global::System.Drawing.KnownColor.DimGray);
			classStyleSet.OverrideBrush(DslDiagrams::DiagramBrushes.ConnectionLineDecorator, lineBrush);
			
			DslDiagrams::BrushSettings textBrush = new DslDiagrams::BrushSettings();
			textBrush.Color = global::System.Drawing.Color.FromKnownColor(global::System.Drawing.KnownColor.DimGray);
			classStyleSet.OverrideBrush(DslDiagrams::DiagramBrushes.ShapeText, textBrush);
		
		}
		
		/// <summary>
		/// Initializes resources associated with this connector instance.
		/// </summary>
		protected override void InitializeInstanceResources()
		{
			base.InitializeInstanceResources();
			this.SetDecorators(DslDiagrams::LinkDecorator.DecoratorEmptyDiamond, new DslDiagrams::SizeD(0.1,0.1), DslDiagrams::LinkDecorator.DecoratorEmptyArrow, new DslDiagrams::SizeD(0.1,0.1), false);
		}
		
		#endregion
		
		#region INotifyPropertyChanged
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
		protected virtual void OnPropertyChanged(System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (this.PropertyChanged != null)
				this.PropertyChanged(this, e);
		}
		#endregion
	
		#region Constructors, domain class Id
	
		/// <summary>
		/// EntityViewAssociationConnector domain class Id.
		/// </summary>
		public static readonly new global::System.Guid DomainClassId = new global::System.Guid(0x1715d1db, 0x8274, 0x453b, 0x82, 0x48, 0xa3, 0x6e, 0x79, 0x5c, 0x89, 0xb8);
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="store">Store where new element is to be created.</param>
		/// <param name="propertyAssignments">List of domain property id/value pairs to set once the element is created.</param>
		public EntityViewAssociationConnector(DslModeling::Store store, params DslModeling::PropertyAssignment[] propertyAssignments)
			: this(store != null ? store.DefaultPartitionForClass(DomainClassId) : null, propertyAssignments)
		{
		}
		
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="partition">Partition where new element is to be created.</param>
		/// <param name="propertyAssignments">List of domain property id/value pairs to set once the element is created.</param>
		public EntityViewAssociationConnector(DslModeling::Partition partition, params DslModeling::PropertyAssignment[] propertyAssignments)
			: base(partition, propertyAssignments)
		{
		}
		#endregion
	}
}

