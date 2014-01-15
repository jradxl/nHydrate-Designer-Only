#region Copyright (c) 2006-2013 nHydrate.org, All Rights Reserved
// -------------------------------------------------------------------------- *
//                           NHYDRATE.ORG                                     *
//              Copyright (c) 2006-2013 All Rights reserved                   *
//                                                                            *
//                                                                            *
// Permission is hereby granted, free of charge, to any person obtaining a    *
// copy of this software and associated documentation files (the "Software"), *
// to deal in the Software without restriction, including without limitation  *
// the rights to use, copy, modify, merge, publish, distribute, sublicense,   *
// and/or sell copies of the Software, and to permit persons to whom the      *
// Software is furnished to do so, subject to the following conditions:       *
//                                                                            *
// The above copyright notice and this permission notice shall be included    *
// in all copies or substantial portions of the Software.                     *
//                                                                            *
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,            *
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES            *
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.  *
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY       *
// CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,       *
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE          *
// SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.                     *
// -------------------------------------------------------------------------- *
#endregion
using Microsoft.VisualStudio.Modeling.Validation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace nHydrate2.Dsl
{
    [ValidationState(ValidationState.Enabled)]
    partial class Entity
    {
        #region Dirty

        [System.ComponentModel.Browsable(false)]
        internal bool IsDirty
        {
            get { return _isDirty || this.Fields.IsDirty() || this.Indexes.IsDirty(); }
            set
            {
                _isDirty = value;
                if (!value)
                {
                    this.Fields.ForEach(x => x.IsDirty = false);
                    this.Indexes.ForEach(x => x.IsDirty = false);
                }
            }
        }

        private bool _isDirty = false;

        #endregion

        [ValidationMethod(ValidationCategories.Open | ValidationCategories.Save | ValidationCategories.Menu | ValidationCategories.Custom | ValidationCategories.Load)]
        public void Validate(ValidationContext context)
        {
            var timer = nHydrate2.Dsl.Custom.DebugHelper.StartTimer();
            try
            {
                if (!this.IsGenerated) return;
                //if (!this.IsDirty) return;
                System.Windows.Forms.Application.DoEvents();

                System.Diagnostics.Debug.WriteLine("Validating Entity: " + this.Name, "Generation");

                #region Cache this. It is expensive

                var heirList = this.GetTableHierarchy();
                var relationshipList = this.RelationshipList.ToList();
                var relationManyMany = relationshipList.Where(x => x.IsManyToMany).ToList();
                var relationFieldMap = new Dictionary<EntityHasEntities, IEnumerable<RelationField>>();

                //Cache the field map minus relations with missing fields (error condition)
                foreach (var relation in relationshipList)
                {
                    var mapList = relation.FieldMapList();
                    if (mapList.Count(x => x.GetSourceField(relation) == null || x.GetTargetField(relation) == null) == 0)
                        relationFieldMap.Add(relation, mapList);
                    else
                        relationFieldMap.Add(relation, new List<RelationField>());
                }

                #endregion

                var timer2 = nHydrate2.Dsl.Custom.DebugHelper.StartTimer();

                #region Check that non-key relationships have a unique index on fields

                foreach (var relation in relationshipList)
                {
                    if (!relation.IsPrimaryKeyRelation())
                    {
                        foreach (var columnRelationship in relationFieldMap[relation])
                        {
                            var parentColumn = columnRelationship.GetSourceField(relation);
                            if (!parentColumn.IsUnique)
                                context.LogError(string.Format(ValidationHelper.ErrorTextTableColumnNonPrimaryRelationNotUnique, parentColumn.DatabaseName, this.Name), string.Empty, this);
                            else
                                context.LogWarning(string.Format(ValidationHelper.WarningTextTableColumnNonPrimaryRelationNotUnique, parentColumn.DatabaseName, this.Name), string.Empty, this);
                        }
                    }
                }

                #endregion

                #region Check that object has at least one generated column

                if (!this.GeneratedColumns.Any())
                    context.LogError(ValidationHelper.ErrorTextColumnsRequired, string.Empty, this);

                #endregion

                #region Clean up bogus references (in case this happened)

                //Verify that no column has same name as container
                foreach (var field in this.Fields)
                {
                    if (string.Compare(field.PascalName, this.PascalName, true) == 0)
                    {
                        context.LogError(string.Format(ValidationHelper.ErrorTextTableColumnNameMatch, field.Name, this.Name), string.Empty, this);
                    }
                }

                //Verify relationships
                foreach (var relation in relationshipList)
                {
                    if (relation != null)
                    {
                        foreach (var relationField in relationFieldMap[relation])
                        {
                            var column1 = relationField.GetSourceField(relation);
                            var column2 = relationField.GetTargetField(relation);
                            if (column1 == null || column2 == null)
                            {
                                context.LogError(string.Format(ValidationHelper.ErrorTextRelationshipMustHaveFields, relation.ParentEntity.Name, relation.ChildEntity.Name), string.Empty, this);
                            }
                            else if (column1.DataType != column2.DataType)
                            {
                                context.LogError(ValidationHelper.ErrorTextRelationshipTypeMismatch + " (" + column1.ToString() + " -> " + column2.ToString() + ")", string.Empty, this);
                            }
                        }

                        if (!relationFieldMap[relation].Any())
                        {
                            context.LogError(string.Format(ValidationHelper.ErrorTextRelationshipMustHaveFields, relation.ParentEntity.Name, relation.ChildEntity.Name), string.Empty, this);
                        }
                        else if (this.nHydrateModel.UseModules)
                        {
                            if (this.nHydrateModel.RelationModules.Count(x => x.RelationID == relation.Id) == 0)
                                context.LogError(string.Format(ValidationHelper.ErrorTextRelationshipMustBeInModule, this.Name, relation.ChildEntity.Name), string.Empty, this);
                        }

                    }
                }

                //Verify that inheritance is setup correctly
                if (!this.IsValidInheritance)
                {
                    context.LogError(string.Format(ValidationHelper.ErrorTextInvalidInheritance, this.Name), string.Empty, this);
                }

                #endregion

                nHydrate2.Dsl.Custom.DebugHelper.StopTimer(timer2, "Entity Validate [" + this.Name + "] - A");
                var timer3 = nHydrate2.Dsl.Custom.DebugHelper.StartTimer();

                #region Check that table does not have same name as project

                if (this.PascalName == this.nHydrateModel.ProjectName)
                {
                    context.LogError(string.Format(ValidationHelper.ErrorTextTableProjectSameName, this.PascalName), string.Empty, this);
                }

                #endregion

                nHydrate2.Dsl.Custom.DebugHelper.StopTimer(timer3, "Entity Validate [" + this.Name + "] - B");
                var timer4 = nHydrate2.Dsl.Custom.DebugHelper.StartTimer();

                #region Check for classes that will confict with generated classes

                var classExtensions = new List<string>();
                classExtensions.Add("collection");
                classExtensions.Add("enumerator");
                classExtensions.Add("query");
                //classExtensions.Add("search");
                classExtensions.Add("pagingfielditem");
                classExtensions.Add("paging");
                classExtensions.Add("primarykey");
                classExtensions.Add("selectall");
                classExtensions.Add("pagedselect");
                classExtensions.Add("selectbypks");
                classExtensions.Add("selectbycreateddaterange");
                classExtensions.Add("selectbymodifieddaterange");
                classExtensions.Add("selectbysearch");
                classExtensions.Add("beforechangeeventargs");
                classExtensions.Add("afterchangeeventargs");

                foreach (var ending in classExtensions)
                {
                    if (this.PascalName.ToLower().EndsWith(ending))
                    {
                        context.LogError(string.Format(ValidationHelper.ErrorTextNameConfictsWithGeneratedCode, this.Name), string.Empty, this);
                    }
                }

                #endregion

                nHydrate2.Dsl.Custom.DebugHelper.StopTimer(timer4, "Entity Validate [" + this.Name + "] - C");
                var timer5 = nHydrate2.Dsl.Custom.DebugHelper.StartTimer();

                #region Verify that child tables have a relation to their parent table

                //if (this.ParentInheritedEntity != null)
                //{
                //  bool isValidRelation = true;
                //  var relations = this.ParentInheritedEntity.RelationshipList.FindByChildTable(this);
                //  foreach (var relation in relations)
                //  {
                //    if (relation.ColumnRelationships.Count == this.PrimaryKeyColumns.Count)
                //    {
                //      foreach (ColumnRelationship columnRelationship in relation.ColumnRelationships)
                //      {
                //        Column parentColumn = (Column)columnRelationship.ParentColumnRef.Object;
                //        Column childColumn = (Column)columnRelationship.ChildColumnRef.Object;
                //        isValidRelation |= ((this.parentTable.PrimaryKeyColumns.Contains(parentColumn)) && (!this.PrimaryKeyColumns.Contains(childColumn)));
                //      }
                //    }
                //    else
                //    {
                //      isValidRelation = false;
                //    }
                //  }
                //  if (!isValidRelation || relations.Count() == 0)
                //  {
                //    context.LogError(string.Format(ValidationHelper.ErrorTextParentTableNoRelation, this.ParentInheritedEntity.Name, this.Name), string.Empty, this);
                //  }
                //}

                #endregion

                #region Verify that parent/child relation have only one identity in hierarchy

                if (this.ParentInheritedEntity != null)
                {
                    foreach (var column in this.PrimaryKeyFields)
                    {
                        var list = this.GetBasePKColumnList(column);
                        if (list.Count(x => x.Identity == IdentityTypeConstants.Database) > 1)
                            context.LogError(string.Format(ValidationHelper.ErrorTextInheritFieldOnly1Identity, this.Name, column.Name), string.Empty, this);

                        var identityColumn = list.FirstOrDefault(x => x.Identity == IdentityTypeConstants.Database);
                        if (identityColumn != null && identityColumn.Entity.ParentInheritedEntity != null)
                        {
                            context.LogError(string.Format(ValidationHelper.ErrorTextInheritTopMostMustBeIndentity, this.Name, column.Name, list.Last().Entity.Name), string.Empty, this);
                        }
                    }
                }

                #endregion

                nHydrate2.Dsl.Custom.DebugHelper.StopTimer(timer5, "Entity Validate [" + this.Name + "] - D");
                var timer6 = nHydrate2.Dsl.Custom.DebugHelper.StartTimer();

                #region Check for inherit hierarchy that all tables are modifiable or not modifiable

                //If this table is Mutable then make sure it is NOT derived from an Immutable table
                if (!this.Immutable)
                {
                    var immutableCount = 0;
                    Entity immutableTable = null;
                    foreach (var h in heirList)
                    {
                        if (h.Immutable)
                        {
                            if (immutableTable == null) immutableTable = h;
                            immutableCount++;
                        }
                    }

                    //If the counts are different then show errors
                    if (immutableCount > 0)
                    {
                        context.LogError(string.Format(ValidationHelper.ErrorTextMutableInherit, this.Name, immutableTable.Name), string.Empty, this);
                    }
                }

                #endregion

                #region Check that all tables in inheritance chain are generated

                {
                    var nonGenCount = 0;
                    Entity nonGenTable = null;
                    foreach (var h in heirList)
                    {
                        if (!h.IsGenerated)
                        {
                            if (nonGenTable == null) nonGenTable = h;
                            nonGenCount++;
                        }
                    }

                    //If the counts are different then show errors
                    if (nonGenCount > 0)
                    {
                        context.LogError(string.Format(ValidationHelper.ErrorTextBaseTableNonGenerated, nonGenTable.Name, this.Name), string.Empty, this);
                    }
                }

                #endregion

                #region Type Tables must be immutable

                if (this.TypedEntity != TypedEntityConstants.None & !this.Immutable)
                    context.LogError(string.Format(ValidationHelper.ErrorTextTypeTableIsMutable, this.Name), string.Empty, this);

                #endregion

                #region Type Tables must have specific columns and data

                if (this.TypedEntity != TypedEntityConstants.None && (this.PrimaryKeyFields.Count > 0))
                {
                    //Must have one PK that is integer type
                    if ((this.PrimaryKeyFields.Count > 1) || !this.PrimaryKeyFields.First().IsIntegerType())
                    {
                        context.LogError(string.Format(ValidationHelper.ErrorTextTypeTablePrimaryKey, this.Name), string.Empty, this);
                    }

                    //Must have static data
                    if (this.StaticDatum.Count == 0)
                    {
                        context.LogError(string.Format(ValidationHelper.ErrorTextTypeTableNoData, this.CodeFacade), string.Empty, this);
                    }

                    //Must have a "Name" or "Description" field
                    var typeTableTextField = this.GeneratedColumns.FirstOrDefault(x => x.Name.ToLower() == "name");
                    if (typeTableTextField == null) typeTableTextField = this.GeneratedColumns.FirstOrDefault(x => x.Name.ToLower() == "description");
                    if (typeTableTextField == null)
                    {
                        context.LogError(string.Format(ValidationHelper.ErrorTextTypeTableTextField, this.Name), string.Empty, this);
                    }
                    else if (this.StaticDatum.Count > 0)
                    {
                        //Verify that type tables have data
                        foreach (var row in this.StaticDatum.ToRows())
                        {
                            //Primary key must be set
                            var cellValue = row[this.PrimaryKeyFields.First().Id];
                            if (cellValue == null)
                            {
                                context.LogError(string.Format(ValidationHelper.ErrorTextTypeTableStaticDataEmpty, this.Name), string.Empty, this);
                            }
                            else if (string.IsNullOrEmpty(cellValue))
                            {
                                context.LogError(string.Format(ValidationHelper.ErrorTextTypeTableStaticDataEmpty, this.Name), string.Empty, this);
                            }

                            //Enum name must be set
                            cellValue = row[typeTableTextField.Id];
                            if (cellValue == null)
                            {
                                context.LogError(string.Format(ValidationHelper.ErrorTextTypeTableStaticDataEmpty, this.Name), string.Empty, this);
                            }
                            else if (string.IsNullOrEmpty(cellValue))
                            {
                                context.LogError(string.Format(ValidationHelper.ErrorTextTypeTableStaticDataEmpty, this.Name), string.Empty, this);
                            }
                        }
                    }

                }

                //Verify that the static data is not duplicated
                if (this.StaticDatum.HasDuplicates(this))
                {
                    context.LogError(string.Format(ValidationHelper.ErrorTextDuplicateStaticData, this.Name), string.Empty, this);
                }

                #endregion

                #region Audit Fields must go down to base table

                //Ensure that audit fields are down in 1st base class
                foreach (var t in heirList)
                {
                    if (t.AllowCreateAudit != this.AllowCreateAudit) context.LogError(string.Format(ValidationHelper.ErrorTextAuditFieldMatchBase, this.Name), string.Empty, this);
                }
                foreach (var t in heirList)
                {
                    if (t.AllowModifyAudit != this.AllowModifyAudit) context.LogError(string.Format(ValidationHelper.ErrorTextAuditFieldMatchBase, this.Name), string.Empty, this);
                }
                foreach (var t in heirList)
                {
                    if (t.AllowTimestamp != this.AllowTimestamp) context.LogError(string.Format(ValidationHelper.ErrorTextAuditFieldMatchBase, this.Name), string.Empty, this);
                }

                #endregion

                #region Self-ref table MUST have role name

                #endregion

                nHydrate2.Dsl.Custom.DebugHelper.StopTimer(timer6, "Entity Validate [" + this.Name + "] - E");
                var timer7 = nHydrate2.Dsl.Custom.DebugHelper.StartTimer();

                #region Self-ref table cannot map child column to PK field

                foreach (var relation in relationshipList)
                {
                    var parentTable = relation.SourceEntity;
                    var childTable = relation.TargetEntity;
                    if (parentTable == childTable)
                    {
                        if (string.IsNullOrEmpty(relation.RoleName))
                        {
                            context.LogError(string.Format(ValidationHelper.ErrorTextSelfRefMustHaveRole, this.Name), string.Empty, this);
                        }
                        else
                        {
                            foreach (var columnRelationShip in relationFieldMap[relation])
                            {
                                if (this.PrimaryKeyFields.Contains(columnRelationShip.GetTargetField(relation)))
                                {
                                    context.LogError(string.Format(ValidationHelper.ErrorTextSelfRefChildColumnPK, this.Name), string.Empty, this);
                                }
                            }
                        }
                    }
                }

                #endregion

                nHydrate2.Dsl.Custom.DebugHelper.StopTimer(timer7, "Entity Validate [" + this.Name + "] - F");
                var timer8 = nHydrate2.Dsl.Custom.DebugHelper.StartTimer();

                #region There can be only 1 self reference per table

                if (this.AllRelationships.Count(x => x.TargetEntity == x.SourceEntity) > 1)
                {
                    context.LogError(string.Format(ValidationHelper.ErrorTextSelfRefOnlyOne, this.Name), string.Empty, this);
                }

                #endregion

                nHydrate2.Dsl.Custom.DebugHelper.StopTimer(timer8, "Entity Validate [" + this.Name + "] - G");
                var timer9 = nHydrate2.Dsl.Custom.DebugHelper.StartTimer();

                #region Verify Relations

                ////Relations must have fields
                //foreach (var relation in relationshipList)
                //{
                //  if (relationFieldMap[relation].Count == 0)
                //  {
                //    context.LogError(string.Format(ValidationHelper.ErrorTextRelationNoFields, relation.SourceEntity.Name + "" + relation.TargetEntity.Name), string.Empty, this);
                //  }
                //}

                var relationKeyList = new List<string>();
                foreach (var relation in relationshipList)
                {
                    var key = relation.RoleName;
                    var parentTable = relation.SourceEntity;
                    var childTable = relation.TargetEntity;
                    var foreignKeyMap = new List<string>();
                    var columnList = new List<string>();

                    var relationKeyMap = string.Empty;
                    foreach (var columnRelationship in relationFieldMap[relation].OrderBy(x => x.SourceFieldId).ThenBy(x => x.TargetFieldId))
                    {
                        var parentColumn = columnRelationship.GetSourceField(relation);
                        var childColumn = columnRelationship.GetTargetField(relation);
                        if (!string.IsNullOrEmpty(key)) key += ", ";
                        key += parentTable.Name + "." + childColumn.Name + " -> " + childTable.Name + "." + parentColumn.Name;
                        if ((parentColumn.Identity == IdentityTypeConstants.Database) &&
                            (childColumn.Identity == IdentityTypeConstants.Database))
                        {
                            context.LogError(string.Format(ValidationHelper.ErrorTextChildTableRelationIdentity, childTable.Name, parentTable.Name), string.Empty, this);
                        }

                        relationKeyMap += parentColumn.Name + "|" + childTable.Name + "|" + childColumn.Name;

                        //Verify that field name does not match foreign table name
                        if (childColumn.PascalName == parentTable.PascalName)
                        {
                            context.LogError(string.Format(ValidationHelper.ErrorTextRelationFieldNotMatchAssociatedTable, parentTable.Name, childTable.Name), string.Empty, this);
                        }

                        //Verify that a relation does not have duplicate column links
                        if (columnList.Contains(parentColumn.Name + "|" + childColumn.Name))
                        {
                            context.LogError(string.Format(ValidationHelper.ErrorTextRelationFieldDuplicated, parentTable.Name, childTable.Name), string.Empty, relation);
                        }
                        else
                            columnList.Add(parentColumn.Name + "|" + childColumn.Name);

                    }

                    if (foreignKeyMap.Contains(relationKeyMap))
                    {
                        context.LogWarning(string.Format(ValidationHelper.ErrorTextMultiFieldRelationsMapDifferentFields, parentTable.Name, childTable.Name), string.Empty, this);
                    }
                    foreignKeyMap.Add(relationKeyMap);

                    if (!relationFieldMap[relation].Any())
                    {
                        System.Diagnostics.Debug.Write(string.Empty);
                    }

                    //Role names cannot start with number
                    if (relation.PascalRoleName.Length > 0)
                    {
                        var roleFirstChar = relation.PascalRoleName.First();
                        if ("0123456789".Contains(roleFirstChar))
                        {
                            context.LogError(string.Format(ValidationHelper.ErrorTextRoleNoStartNumber, key), string.Empty, this);
                        }
                    }

                    //Verify that relations are not duplicated (T1.C1 -> T2.C2)
                    if (!relationKeyList.Contains(relation.PascalRoleName + "|" + key))
                    {
                        relationKeyList.Add(relation.PascalRoleName + "|" + key);
                    }
                    else
                    {
                        context.LogError(string.Format(ValidationHelper.ErrorTextDuplicateRelation, key, parentTable.Name, childTable.Name), string.Empty, this);
                    }

                }

                //Look for duplication relations based on child tables too
                //Check the full table hierarchy for relations
                if (this.ParentInheritedEntity != null)
                {
                    relationKeyList.Clear();
                    var anscestorRelationList = this.GetRelationsFullHierarchy().Where(x => (x.TargetEntity != this)).ToList();
                    foreach (var relation in anscestorRelationList)
                    {
                        var key = string.Empty;
                        var parentTable = relation.SourceEntity;
                        var childTable = relation.TargetEntity;

                        if (!this.IsInheritedFrom(childTable))
                        {
                            if (relation.IsOneToOne) key = relation.PascalRoleName + childTable.PascalName;
                            else key = relation.PascalRoleName + childTable.PascalName + "List";

                            //Verify that relations are not duplicated (T1.C1 -> T2.C2)
                            if (!relationKeyList.Contains(key))
                            {
                                relationKeyList.Add(key);
                            }
                            else
                            {
                                context.LogError(string.Format(ValidationHelper.ErrorTextDuplicateRelationFullHierarchy, this.Name, childTable.Name), string.Empty, this);
                            }
                        }

                    }
                }

                //Verify M:N relations have same role name on both sides
                foreach (var relation in relationManyMany)
                {
                    var relation2 = relation.GetAssociativeOtherRelation();
                    if (relation2 == null)
                    {
                        //TODO
                    }
                    else if (relation.RoleName != relation.GetAssociativeOtherRelation().RoleName)
                    {
                        context.LogError(string.Format(ValidationHelper.ErrorTextRelationM_NRoleMismatch, relation.TargetEntity.Name), string.Empty, this);
                    }
                }

                //Verify M:N relations do not map to same property names
                //This can happen if 2 M:N tables are defined between the same two tables...(why people do this I do not know)
                relationKeyList.Clear();
                foreach (var relation in relationManyMany)
                {
                    var relation2 = relation.GetAssociativeOtherRelation();
                    if (relation2 == null)
                    {
                        //TODO
                    }
                    else
                    {
                        var mappedName = relation.RoleName + "|" + relation.GetSecondaryAssociativeTable().Name;
                        if (relationKeyList.Contains(mappedName))
                        {
                            context.LogError(string.Format(ValidationHelper.ErrorTextRelationM_NNameDuplication, this.Name, relation.GetSecondaryAssociativeTable().Name), string.Empty, this);
                        }
                        else
                        {
                            relationKeyList.Add(mappedName);
                        }
                    }
                }

                {
                    //Verify that if related to an associative table I do not also have a direct link
                    var relatedTables = new List<string>();
                    foreach (var relation in relationManyMany)
                    {
                        relatedTables.Add(relation.GetSecondaryAssociativeTable().PascalName + relation.RoleName);
                    }

                    //Now verify that I have no relation to them
                    var invalid = false;
                    foreach (var relation in relationshipList)
                    {
                        if (!relation.IsManyToMany)
                        {
                            if (relatedTables.Contains(relation.TargetEntity.PascalName + relation.RoleName))
                            {
                                invalid = true;
                            }
                        }
                    }

                    if (invalid)
                        context.LogError(string.Format(ValidationHelper.ErrorTextRelationCausesNameConflict, this.Name, relatedTables.First()), string.Empty, this);

                }

                //Only 1 relation can exist from A->B on the same columns 
                {
                    var hashList = new List<string>();
                    var rList = relationshipList.Where(x => x.SourceEntity.IsGenerated && x.TargetEntity.IsGenerated).ToList();
                    foreach (var r in rList)
                    {
                        if (!hashList.Contains(r.LinkHash))
                            hashList.Add(r.LinkHash);
                    }
                    if (rList.Count != hashList.Count)
                        context.LogError(string.Format(ValidationHelper.ErrorTextRelationDuplicate, this.Name), string.Empty, this);
                }

                //Relation fields cannot be duolicated with a relation. All source fields must be unique and all target fields must be unique
                foreach (var relation in relationshipList.Where(x => x != null))
                {
                    var inError = false;
                    var colList1 = new List<Guid>();
                    var colList2 = new List<Guid>();
                    foreach (var columnRelationship in relationFieldMap[relation].OrderBy(x => x.SourceFieldId).ThenBy(x => x.TargetFieldId))
                    {
                        if (colList1.Contains(columnRelationship.SourceFieldId)) inError = true;
                        else colList1.Add(columnRelationship.SourceFieldId);

                        if (colList2.Contains(columnRelationship.TargetFieldId)) inError = true;
                        else colList2.Add(columnRelationship.TargetFieldId);
                    }

                    if (inError)
                    {
                        context.LogError(string.Format(ValidationHelper.ErrorTextRelationNeedUniqueFields, relation.ParentEntity.Name, relation.ChildEntity.Name), string.Empty, this);
                    }
                }

                #endregion

                nHydrate2.Dsl.Custom.DebugHelper.StopTimer(timer9, "Entity Validate [" + this.Name + "] - H");

                #region Associative Tables

                if (this.IsAssociative)
                {
                    var count = 0;
                    foreach (var relation in this.nHydrateModel.AllRelations)
                    {
                        if (relation.TargetEntity == this)
                        {
                            count++;
                        }
                    }
                    if (count != 2)
                    {
                        context.LogError(string.Format(ValidationHelper.ErrorTextAssociativeTableMustHave2Relations, this.Name, count), string.Empty, this);
                    }

                    //Associative tables cannot be inherited
                    if (this.ParentInheritedEntity != null)
                        context.LogError(string.Format(ValidationHelper.ErrorTextAssociativeTableNotInherited, this.Name), string.Empty, this);
                }

                #endregion

                #region There can be only 1 Identity per table

                var identityCount = this.GeneratedColumns.Count(x => x.Identity == IdentityTypeConstants.Database);
                if (identityCount > 1)
                {
                    //If there is an identity column, it can be the only PK
                    context.LogError(string.Format(ValidationHelper.ErrorTextIdentityOnlyOnePerTable, this.Name), string.Empty, this);
                }

                #endregion

                #region Identity PK can be only PK

                var pkIdentityCount = this.PrimaryKeyFields.Count(x => x.Identity != IdentityTypeConstants.None);
                if ((pkIdentityCount > 0) && (this.PrimaryKeyFields.Count != pkIdentityCount))
                {
                    //If there is an identity column, it can be the only PK
                    context.LogWarning(string.Format(ValidationHelper.ErrorTextIdentityPKNotOnlyKey, this.Name), string.Empty, this);
                }

                #endregion

                #region Associative table cannot be immutable

                if (this.IsAssociative & this.Immutable)
                {
                    context.LogError(ValidationHelper.ErrorTextAssociativeTableNotImmutable, string.Empty, this);
                }

                #endregion

                #region Tables must have a non-identity column

                if (!this.Immutable)
                {
                    if (this.GeneratedColumns.Count(x => x.Identity == IdentityTypeConstants.Database) == this.GeneratedColumns.Count(x => x.IsGenerated))
                    {
                        context.LogError(string.Format(ValidationHelper.ErrorTextTableNotHave1IdentityOnly, this.Name), string.Empty, this);
                    }
                }

                #endregion

                #region AllowAuditTracking

                //if (this.AllowAuditTracking && this.IsAssociative)
                //{
                //  context.LogError( string.Format(ValidationHelper.ErrorTextTableAssociativeNoAuditTracking, this.Name), string.Empty, this);
                //}

                #endregion

                #region Associative must have non-overlapping PK column

                if (this.IsAssociative)
                {
                    var rlist = this.GetRelationsWhereChild().ToList();
                    if (rlist.Count == 2)
                    {
                        var r1 = rlist.First();
                        var r2 = rlist.Last();
                        if (this.PrimaryKeyFields.Count != r1.GetSecondaryAssociativeTable().PrimaryKeyFields.Count + r2.GetSecondaryAssociativeTable().PrimaryKeyFields.Count)
                        {
                            context.LogError(string.Format(ValidationHelper.ErrorTextTableAssociativeNeedsNonOverlappingColumns, this.Name), string.Empty, this);
                        }
                    }

                    if (this.Fields.Count(x => x.IsGenerated && !x.IsPrimaryKey) > 0)
                    {
                        context.LogError(string.Format(ValidationHelper.ErrorTextTableAssociativeNeedsOnlyPK, this.Name), string.Empty, this);
                    }
                }

                #endregion

                #region Verify Static Data

                if (!this.StaticDatum.IsDataValid(this))
                    context.LogError(string.Format(ValidationHelper.ErrorTextTableBadStaticData, this.Name), string.Empty, this);

                #endregion

                #region Verify Metadata

                var metaKeyList = new List<string>();
                foreach (var item in this.EntityMetadata)
                {
                    if (string.IsNullOrEmpty(item.Key) || metaKeyList.Contains(item.Key.ToLower()))
                    {
                        context.LogError(ValidationHelper.ErrorTextMetadataInvalid, string.Empty, this);
                    }
                    else
                    {
                        metaKeyList.Add(item.Key.ToString());
                    }
                }

                #endregion

                #region Verify Indexes

                //Check for missing mapped index columns
                foreach (var index in this.Indexes)
                {
                    if (index.IndexColumns.Count == 0)
                    {
                        context.LogError(string.Format(ValidationHelper.ErrorTextEntityIndexInvalid, this.Name), string.Empty, this);
                    }
                    else
                    {
                        foreach (var ic in index.IndexColumns)
                        {
                            if (ic.GetField() == null)
                            {
                                context.LogError(string.Format(ValidationHelper.ErrorTextEntityIndexInvalid, this.Name), string.Empty, this);
                            }
                        }
                    }
                }

                //Check that there are no duplicate indexes
                var mappedOrderedIndexes = new List<string>();
                var mappedUnorderedIndexes = new List<string>();
                foreach (var index in this.Indexes)
                {
                    var indexKey = string.Empty;
                    var columns = index.IndexColumns.Where(x => x.GetField() != null);

                    //Ordered
                    foreach (var ic in columns.OrderBy(x => x.GetField().Name))
                    {
                        var field = ic.GetField();
                        indexKey += field.Id + ic.Ascending.ToString() + "|";
                    }
                    mappedOrderedIndexes.Add(indexKey);

                    //Unordered (by name..just in index order)
                    indexKey = string.Empty;
                    foreach (var ic in columns.OrderBy(x => x.SortOrder))
                    {
                        var field = ic.GetField();
                        indexKey += field.Id + ic.Ascending.ToString() + "|";
                    }
                    mappedUnorderedIndexes.Add(indexKey);
                }

                //Ordered duplicate is a warning
                if (mappedOrderedIndexes.Count != mappedOrderedIndexes.Distinct().Count())
                    context.LogWarning(string.Format(ValidationHelper.ErrorTextEntityIndexIsPossibleDuplicate, this.Name), string.Empty, this);

                //Unordered is an error since this is a REAL duplicate
                if (mappedUnorderedIndexes.Count != mappedUnorderedIndexes.Distinct().Count())
                    context.LogError(string.Format(ValidationHelper.ErrorTextEntityIndexIsDuplicate, this.Name), string.Empty, this);

                //Only one clustered allowed
                if (this.Indexes.Count(x => x.Clustered) > 1)
                {
                    context.LogError(string.Format(ValidationHelper.ErrorTextEntityIndexMultipleClustered, this.Name), string.Empty, this);
                }

                #endregion

                #region Associative Table

                //If no generated CRUD cannot have auditing on Associative
                if (this.IsAssociative && !this.nHydrateModel.UseGeneratedCRUD && (this.AllowCreateAudit || this.AllowModifyAudit || this.AllowTimestamp))
                {
                    context.LogError(string.Format(ValidationHelper.ErrorTextTableAssociativeNoCRUDAudit, this.Name), string.Empty, this);
                }

                #endregion

                #region Parent Table must be in Module

                if (this.nHydrateModel.UseModules)
                {
                    var myModules = this.Modules;
                    var alltables = this.GetTableHierarchy();
                    foreach (var t in alltables)
                    {
                        if (myModules.Intersect(t.Modules).Count() < myModules.Count)
                        {
                            context.LogError(string.Format(ValidationHelper.ErrorTextTableInheritMustContainParentInModule, this.Name, t.Name), string.Empty, this);
                        }
                    }
                }

                #endregion

                #region MySQL

                if (this.AllowAuditTracking && (this.nHydrateModel.SupportedPlatforms & DatabasePlatformConstants.MySQL) == DatabasePlatformConstants.MySQL)
                {
                    context.LogError(string.Format(ValidationHelper.ErrorTextMySQLAuditTables, this.Name), string.Empty, this);
                }

                #endregion

                #region Tenant

                if (this.IsTenant && (this.nHydrateModel.SupportedPlatforms & DatabasePlatformConstants.MySQL) == DatabasePlatformConstants.MySQL)
                {
                    context.LogError(string.Format(ValidationHelper.ErrorTextMySQLTenant, this.Name), string.Empty, this);
                }
                if (this.IsTenant && this.TypedEntity == TypedEntityConstants.DatabaseTable)
                {
                    context.LogError(string.Format(ValidationHelper.ErrorTextTenantTypeTable, this.Name), string.Empty, this);
                }
                if (this.IsTenant && this.Fields.Any(x => x.Name.ToLower() == this.nHydrateModel.TenantColumnName.ToLower()))
                {
                    context.LogError(string.Format(ValidationHelper.ErrorTextTenantTypeTableTenantColumnMatch, this.Name, this.nHydrateModel.TenantColumnName), string.Empty, this);
                }

                #endregion

                #region Warn if GUID is Clustered Key

                var fields = this.Indexes.Where(x => x.Clustered).SelectMany(x => x.FieldList).Where(x => x.DataType == DataTypeConstants.UniqueIdentifier).ToList();
                if (fields.Any())
                {
                    foreach (var f in fields)
                    {
                        context.LogWarning(string.Format(ValidationHelper.ErrorTextClusteredGuid, f.Name, this.Name), string.Empty, this);
                    }
                }

                #endregion

                #region Warn of defaults on nullable fields

                var nullableList = this.FieldList
                                       .ToList()
                                       .Cast<Field>()
                                       .Where<Field>(x => x.Nullable && !string.IsNullOrEmpty(x.Default))
                                       .ToList();

                foreach (var field in nullableList)
                {
                    context.LogWarning(string.Format(ValidationHelper.ErrorTextNullableFieldHasDefault, field.Name, this.Name), string.Empty, this);

                    //If default on nullbale that is FK then ERROR
                    foreach (var r in this.GetRelationsWhereChild())
                    {
                        foreach (var q in r.FieldMapList().Where(z => z.TargetFieldId == field.Id))
                        {
                            var f = this.FieldList.ToList().Cast<Field>().FirstOrDefault(x => x.Id == q.TargetFieldId);
                            if (f != null)
                                context.LogError(string.Format(ValidationHelper.ErrorTextNullableFieldHasDefaultWithRelation, f.Name, this.Name, r.SourceEntity.Name), string.Empty, this);
                        }
                    }
                }

                #endregion

                #region Verify Indexes in some module

                if (this.nHydrateModel.UseModules)
                {
                    foreach (var index in this.Indexes)
                    {
                        if (!this.nHydrateModel.IndexModules.Any(x => x.IndexID == index.Id))
                        {
                            context.LogError(string.Format(ValidationHelper.ErrorTextIndexNotInModule, this.Name, index.ToString()), string.Empty, this);
                        }
                    }
                }

                #endregion

                #region Ensure fields exist in a module if parent index does

                if (this.nHydrateModel.UseModules)
                {
                    foreach (var index in this.Indexes)
                    {
                        //Get all modules in which this index exists
                        var moduleList = this.nHydrateModel.IndexModules.Where(x => x.IndexID == index.Id).Select(x => x.ModuleId);
                        foreach (var moduleId in moduleList)
                        {
                            //get all fields that exists in a module
                            var allFieldList = this.nHydrateModel.Modules.Where(x => x.Id == moduleId).SelectMany(x => x.Fields).Select(x => x.Id).ToList();
                            //get all field IDs for current index
                            var indexFieldList = index.FieldList.Select(x => x.Id).ToList();
                            //ensure that all fields in current index exist in module
                            if (allFieldList.Count(x => indexFieldList.Contains(x)) != indexFieldList.Count)
                            {
                                context.LogError(string.Format(ValidationHelper.ErrorTextModuleContainIndexNotFields, this.Name, index.ToString(), this.nHydrateModel.Modules.First(x => x.Id == moduleId).Name), string.Empty, this);
                            }
                        }
                    }
                }

                #endregion

            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                nHydrate2.Dsl.Custom.DebugHelper.StopTimer(timer, "Entity Validate [" + this.Name + "] - Entities");
            }
        }

        [ValidationMethod(ValidationCategories.Open | ValidationCategories.Save | ValidationCategories.Menu | ValidationCategories.Custom | ValidationCategories.Load)]
        public void ValidateFields(ValidationContext context)
        {
            var timer = nHydrate2.Dsl.Custom.DebugHelper.StartTimer();

            try
            {
                if (!this.IsGenerated) return;
                var columnList = this.Fields.Where(x => x.IsGenerated).ToList();

                #region Check for duplicate names

                var nameList = new Hashtable();
                foreach (var column in columnList)
                {
                    var name = column.Name.ToLower();
                    if (nameList.ContainsKey(name))
                        context.LogError(string.Format(ValidationHelper.ErrorTextDuplicateName, column.Name), string.Empty, this);
                    else
                        nameList.Add(name, string.Empty);
                }

                #endregion

                #region Check for duplicate codefacades

                if (context.CurrentViolations.Count == 0)
                {
                    nameList = new Hashtable();
                    foreach (var column in columnList)
                    {
                        var name = column.PascalName.ToLower();
                        if (nameList.ContainsKey(name))
                            context.LogError(string.Format(ValidationHelper.ErrorTextDuplicateCodeFacade, column.Name), string.Empty, this);
                        else
                            nameList.Add(name, string.Empty);
                    }
                }

                #endregion

                #region Check for a primary key

                var isPrimaryNull = false;
                foreach (var column in columnList)
                {
                    if (column.IsPrimaryKey)
                    {
                        //hasPrimary = true;
                        isPrimaryNull |= column.Nullable;
                    }
                }

                #endregion

                #region Check for field named created,modfied,timestamp as these are taken

                foreach (var column in columnList)
                {
                    var name = column.Name.ToLower().Replace("_", string.Empty);
                    var t = this;
                    //if (t.AllowCreateAudit)
                    {
                        //If there is a CreateAudit then no fields can be named the predined values
                        if (string.Compare(name, this.nHydrateModel.CreatedByColumnName.Replace("_", string.Empty), true) == 0)
                            context.LogError(string.Format(ValidationHelper.ErrorTextPreDefinedNameField, column.Name), string.Empty, this);
                        else if (string.Compare(name, this.nHydrateModel.CreatedDateColumnName.Replace("_", string.Empty), true) == 0)
                            context.LogError(string.Format(ValidationHelper.ErrorTextPreDefinedNameField, column.Name), string.Empty, this);
                    }
                    //if (t.AllowModifyAudit)
                    {
                        if (string.Compare(name, this.nHydrateModel.ModifiedByColumnName.Replace("_", string.Empty), true) == 0)
                            context.LogError(string.Format(ValidationHelper.ErrorTextPreDefinedNameField, column.Name), string.Empty, this);
                        else if (string.Compare(name, this.nHydrateModel.ModifiedDateColumnName.Replace("_", string.Empty), true) == 0)
                            context.LogError(string.Format(ValidationHelper.ErrorTextPreDefinedNameField, column.Name), string.Empty, this);
                    }
                    //if (t.AllowTimestamp)
                    {
                        if (string.Compare(name, this.nHydrateModel.TimestampColumnName.Replace("_", string.Empty), true) == 0)
                            context.LogError(string.Format(ValidationHelper.ErrorTextPreDefinedNameField, column.Name), string.Empty, this);
                    }
                }

                #endregion

                #region Verify something is generated

                if (columnList.Count != 0)
                {
                    //Make sure all PK are generated
                    if (this.PrimaryKeyFields.Count != columnList.Count(x => x.IsGenerated && x.IsPrimaryKey == true))
                        context.LogError(string.Format(ValidationHelper.ErrorTextNoPrimaryKey, this.Name), string.Empty, this);
                    else if (this.PrimaryKeyFields.Count == 0)
                        context.LogError(string.Format(ValidationHelper.ErrorTextNoPrimaryKey, this.Name), string.Empty, this);
                    else if (isPrimaryNull)
                        context.LogError(ValidationHelper.ErrorTextPrimaryKeyNull, string.Empty, this);
                }

                #endregion

                #region Verify only one timestamp/table

                var timestampcount = this.AllowTimestamp ? 1 : 0;
                timestampcount += this.Fields.Count(x => x.IsGenerated && x.DataType == DataTypeConstants.Timestamp);
                if (timestampcount > 1)
                {
                    context.LogError(string.Format(ValidationHelper.ErrorTextOnlyOneTimestamp, this.Name), string.Empty, this);
                }

                #endregion

            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                nHydrate2.Dsl.Custom.DebugHelper.StopTimer(timer, "Entity Validate [" + this.Name + "] - Fields");
            }

        }

    }
}