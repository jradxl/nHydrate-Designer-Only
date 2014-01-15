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
using Microsoft.VisualStudio.Modeling;
using nHydrate.Generator.Common.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;

namespace nHydrate2.Dsl.Custom
{
    public static class SQLFileManagement
    {
        private const string FOLDER_ET = "_Entities";
        private const string FOLDER_SP = "_StoredProcedures";
        private const string FOLDER_VW = "_Views";
        private const string FOLDER_FC = "_Functions";

        public static string GetModelFolder(string rootFolder, string modelName)
        {
            return Path.Combine(rootFolder, "_" + modelName);
        }

        public static void SaveToDisk(nHydrateModel modelRoot, string rootFolder, string modelName, nHydrateDiagram diagram)
        {
            modelRoot.IsSaving = true;
            try
            {
                var modelFolder = GetModelFolder(rootFolder, modelName);
                var generatedFileList = new List<string>();
                nHydrate2.Dsl.Custom.SQLFileManagement.SaveToDisk(modelRoot, modelRoot.ModelMetadata, modelFolder, diagram, generatedFileList);
                nHydrate2.Dsl.Custom.SQLFileManagement.SaveToDisk(modelRoot, modelRoot.Modules, modelFolder, diagram, generatedFileList);
                nHydrate2.Dsl.Custom.SQLFileManagement.SaveToDisk(modelRoot, modelRoot.Views, modelFolder, diagram, generatedFileList); //must come before entities (view relations)
                nHydrate2.Dsl.Custom.SQLFileManagement.SaveToDisk(modelRoot, modelRoot.Entities, modelFolder, diagram, generatedFileList);
                nHydrate2.Dsl.Custom.SQLFileManagement.SaveToDisk(modelRoot, modelRoot.StoredProcedures, modelFolder, diagram, generatedFileList);
                nHydrate2.Dsl.Custom.SQLFileManagement.SaveToDisk(modelRoot, modelRoot.Functions, modelFolder, diagram, generatedFileList);
                nHydrate2.Dsl.Custom.SQLFileManagement.SaveDiagramFiles(modelFolder, diagram, generatedFileList);
                RemoveOrphans(modelFolder, generatedFileList);
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                modelRoot.IsSaving = false;
            }
        }

        /// <summary>
        /// Saves Stored Procedures to disk
        /// </summary>
        private static void SaveToDisk(nHydrateModel modelRoot, IEnumerable<Entity> list, string rootFolder, nHydrateDiagram diagram, List<string> generatedFileList)
        {
            var folder = Path.Combine(rootFolder, FOLDER_ET);
            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

            #region Save other parameter/field information
            foreach (var item in list)
            {
                var document = new XmlDocument();
                document.LoadXml(@"<configuration type=""entity"" name=""" + item.Name + @"""></configuration>");

                XmlHelper.AddLineBreak(document.DocumentElement);
                XmlHelper.AddCData(document.DocumentElement, "summary", item.Summary);
                XmlHelper.AddLineBreak(document.DocumentElement);

                XmlHelper.AddAttribute(document.DocumentElement, "id", item.Id);
                XmlHelper.AddAttribute(document.DocumentElement, "allowaudittracking", item.AllowAuditTracking);
                XmlHelper.AddAttribute(document.DocumentElement, "allowcreateaudit", item.AllowCreateAudit);
                XmlHelper.AddAttribute(document.DocumentElement, "allowmodifyaudit", item.AllowModifyAudit);
                XmlHelper.AddAttribute(document.DocumentElement, "allowtimestamp", item.AllowTimestamp);
                XmlHelper.AddAttribute(document.DocumentElement, "codefacade", item.CodeFacade);
                XmlHelper.AddAttribute(document.DocumentElement, "immutable", item.Immutable);
                XmlHelper.AddAttribute(document.DocumentElement, "enforceprimarykey", item.EnforcePrimaryKey);
                XmlHelper.AddAttribute(document.DocumentElement, "isassociative", item.IsAssociative);
                XmlHelper.AddAttribute(document.DocumentElement, "typedentity", item.TypedEntity.ToString());
                XmlHelper.AddAttribute(document.DocumentElement, "schema", item.Schema);
                XmlHelper.AddAttribute(document.DocumentElement, "generatesdoublederived", item.GeneratesDoubleDerived);
                XmlHelper.AddAttribute(document.DocumentElement, "isgenerated", item.IsGenerated);
                XmlHelper.AddAttribute(document.DocumentElement, "isTenant", item.IsTenant);

                #region Fields
                {
                    var fieldsNodes = XmlHelper.AddElement(document.DocumentElement, "fieldset") as XmlElement;
                    XmlHelper.AddLineBreak((XmlElement)fieldsNodes);
                    foreach (var field in item.Fields.OrderBy(x => x.Name))
                    {
                        var fieldNode = XmlHelper.AddElement(fieldsNodes, "field");

                        XmlHelper.AddLineBreak((XmlElement)fieldNode);
                        XmlHelper.AddCData((XmlElement)fieldNode, "summary", field.Summary);
                        XmlHelper.AddLineBreak((XmlElement)fieldNode);

                        XmlHelper.AddAttribute(fieldNode, "id", field.Id);
                        XmlHelper.AddAttribute(fieldNode, "name", field.Name);
                        XmlHelper.AddAttribute(fieldNode, "nullable", field.Nullable);
                        XmlHelper.AddAttribute(fieldNode, "datatype", field.DataType.ToString());
                        XmlHelper.AddAttribute(fieldNode, "identity", field.Identity.ToString());
                        XmlHelper.AddAttribute(fieldNode, "codefacade", field.CodeFacade);
                        XmlHelper.AddAttribute(fieldNode, "dataformatstring", field.DataFormatString);
                        XmlHelper.AddAttribute(fieldNode, "default", field.Default);
                        XmlHelper.AddAttribute(fieldNode, "defaultisfunc", field.DefaultIsFunc);
                        XmlHelper.AddAttribute(fieldNode, "formula", field.Formula);
                        XmlHelper.AddAttribute(fieldNode, "isgenerated", field.IsGenerated);
                        XmlHelper.AddAttribute(fieldNode, "isindexed", field.IsIndexed);
                        XmlHelper.AddAttribute(fieldNode, "isprimarykey", field.IsPrimaryKey);
                        XmlHelper.AddAttribute(fieldNode, "Iscalculated", field.IsCalculated);
                        XmlHelper.AddAttribute(fieldNode, "isunique", field.IsUnique);
                        XmlHelper.AddAttribute(fieldNode, "length", field.Length);
                        XmlHelper.AddAttribute(fieldNode, "scale", field.Scale);
                        XmlHelper.AddAttribute(fieldNode, "sortorder", field.SortOrder);
                        XmlHelper.AddAttribute(fieldNode, "isreadonly", field.IsReadOnly);
                        XmlHelper.AddAttribute(fieldNode, "category", field.Category);
                        XmlHelper.AddAttribute(fieldNode, "collate", field.Collate);
                        XmlHelper.AddAttribute(fieldNode, "friendlyname", field.FriendlyName);
                        XmlHelper.AddAttribute(fieldNode, "isbrowsable", field.IsBrowsable);
                        XmlHelper.AddAttribute(fieldNode, "max", field.Max);
                        XmlHelper.AddAttribute(fieldNode, "min", field.Min);
                        XmlHelper.AddAttribute(fieldNode, "validationexpression", field.ValidationExpression);

                        XmlHelper.AddLineBreak((XmlElement)fieldsNodes);
                    }
                    XmlHelper.AddLineBreak((XmlElement)document.DocumentElement);
                }
                #endregion

                XmlHelper.AddLineBreak(document.DocumentElement);
                var f = Path.Combine(folder, item.Name + ".configuration.xml");
                WriteFileIfNeedBe(f, document.ToIndentedString(), generatedFileList);

                //Save other files
                SaveEntityIndexes(folder, item, generatedFileList);
                SaveModules(folder, item, generatedFileList);
                SaveRelations(folder, item, generatedFileList);
                SaveEntityStaticData(folder, item, generatedFileList);
                SaveEntityMetaData(folder, item, generatedFileList);
                SaveEntityComposites(folder, item, generatedFileList);

            }
            #endregion

            WriteReadMeFile(folder, generatedFileList);
        }

        private static void SaveEntityIndexes(string folder, Entity item, List<string> generatedFileList)
        {
            var document = new XmlDocument();
            document.LoadXml(@"<configuration type=""entity.indexes"" id=""" + item.Id + @"""></configuration>");

            XmlHelper.AddLineBreak((XmlElement)document.DocumentElement);
            foreach (var index in item.Indexes)
            {
                var indexNode = XmlHelper.AddElement(document.DocumentElement, "index");

                XmlHelper.AddLineBreak((XmlElement)indexNode);
                XmlHelper.AddCData((XmlElement)indexNode, "summary", index.Summary);
                XmlHelper.AddLineBreak((XmlElement)indexNode);

                XmlHelper.AddAttribute(indexNode, "id", index.Id);
                XmlHelper.AddAttribute(indexNode, "clustered", index.Clustered);
                XmlHelper.AddAttribute(indexNode, "importedname", index.ImportedName);
                XmlHelper.AddAttribute(indexNode, "indextype", index.IndexType.ToString("d"));
                XmlHelper.AddAttribute(indexNode, "isunique", index.IsUnique);

                XmlHelper.AddLineBreak((XmlElement)document.DocumentElement);

                //Process the columns
                var indexColumnsNodes = XmlHelper.AddElement((XmlElement)indexNode, "indexcolumnset") as XmlElement;
                XmlHelper.AddLineBreak((XmlElement)indexColumnsNodes);
                foreach (var indexColumn in index.IndexColumns)
                {
                    var indexColumnNode = XmlHelper.AddElement(indexColumnsNodes, "column");
                    XmlHelper.AddAttribute(indexColumnNode, "id", indexColumn.Id);
                    XmlHelper.AddAttribute(indexColumnNode, "ascending", indexColumn.Ascending);
                    XmlHelper.AddAttribute(indexColumnNode, "fieldid", indexColumn.FieldID);
                    XmlHelper.AddAttribute(indexColumnNode, "sortorder", indexColumn.SortOrder);

                    XmlHelper.AddLineBreak((XmlElement)indexColumnsNodes);
                }
            }

            var f = Path.Combine(folder, item.Name + ".indexes.xml");
            WriteFileIfNeedBe(f, document.ToIndentedString(), generatedFileList);

        }

        private static void SaveEntityComposites(string folder, Entity item, List<string> generatedFileList)
        {
            var document = new XmlDocument();
            document.LoadXml(@"<configuration type=""composite"" id=""" + item.Id + @"""></configuration>");

            if (item.Composites.Count == 0)
                return;

            XmlHelper.AddLineBreak((XmlElement)document.DocumentElement);
            foreach (var composite in item.Composites)
            {
                var indexNode = XmlHelper.AddElement(document.DocumentElement, "composite");

                XmlHelper.AddLineBreak((XmlElement)indexNode);
                XmlHelper.AddCData((XmlElement)indexNode, "summary", composite.Summary);
                XmlHelper.AddLineBreak((XmlElement)indexNode);

                XmlHelper.AddAttribute(indexNode, "codefacade", composite.CodeFacade);
                XmlHelper.AddAttribute(indexNode, "name", composite.Name);
                XmlHelper.AddAttribute(indexNode, "isgenerated", composite.IsGenerated);

                XmlHelper.AddLineBreak((XmlElement)document.DocumentElement);

                //Process the columns
                var columnsNodes = XmlHelper.AddElement((XmlElement)indexNode, "columnset") as XmlElement;
                XmlHelper.AddLineBreak((XmlElement)columnsNodes);
                foreach (var column in composite.Fields)
                {
                    var indexColumnNode = XmlHelper.AddElement(columnsNodes, "column");
                    XmlHelper.AddAttribute(indexColumnNode, "fieldid", column.FieldId);

                    XmlHelper.AddLineBreak((XmlElement)columnsNodes);
                }
            }

            var f = Path.Combine(folder, item.Name + ".composites.xml");
            WriteFileIfNeedBe(f, document.ToIndentedString(), generatedFileList);

        }

        private static void SaveModules(string folder, Entity item, List<string> generatedFileList)
        {
            var document = new XmlDocument();
            document.LoadXml(@"<configuration type=""entity.modules"" id=""" + item.Id + @"""></configuration>");

            XmlHelper.AddLineBreak((XmlElement)document.DocumentElement);

            //Fields
            foreach (var field in item.Fields)
            {
                foreach (var module in field.Modules)
                {
                    var newNode = XmlHelper.AddElement(document.DocumentElement, "map");
                    XmlHelper.AddAttribute(newNode, "source", field.Id);
                    XmlHelper.AddAttribute(newNode, "module", module.Id);
                    XmlHelper.AddAttribute(newNode, "type", "field");
                    XmlHelper.AddLineBreak((XmlElement)document.DocumentElement);
                }
            }

            //Relations
            foreach (var relation in item.RelationshipList)
            {
                foreach (var rm in item.nHydrateModel.RelationModules.Where(x => x.RelationID == relation.Id))
                {
                    var newNode = XmlHelper.AddElement(document.DocumentElement, "map");
                    XmlHelper.AddAttribute(newNode, "source", relation.InternalId);
                    XmlHelper.AddAttribute(newNode, "module", rm.ModuleId);
                    XmlHelper.AddAttribute(newNode, "isenforced", rm.IsEnforced);
                    XmlHelper.AddAttribute(newNode, "type", "relation");
                    XmlHelper.AddLineBreak((XmlElement)document.DocumentElement);
                }
            }

            //Indexes
            foreach (var index in item.IndexList)
            {
                foreach (var rm in item.nHydrateModel.IndexModules.Where(x => x.IndexID == index.Id))
                {
                    var newNode = XmlHelper.AddElement(document.DocumentElement, "map");
                    XmlHelper.AddAttribute(newNode, "source", index.Id);
                    XmlHelper.AddAttribute(newNode, "module", rm.ModuleId);
                    XmlHelper.AddAttribute(newNode, "type", "index");
                    XmlHelper.AddLineBreak((XmlElement)document.DocumentElement);
                }
            }

            var f = Path.Combine(folder, item.Name + ".modules.xml");
            WriteFileIfNeedBe(f, document.ToIndentedString(), generatedFileList);

        }

        private static void SaveModules(string folder, View item, List<string> generatedFileList)
        {
            var document = new XmlDocument();
            document.LoadXml(@"<configuration type=""view.modules"" id=""" + item.Id + @"""></configuration>");

            XmlHelper.AddLineBreak((XmlElement)document.DocumentElement);
            foreach (var module in item.Modules)
            {
                var newNode = XmlHelper.AddElement(document.DocumentElement, "map");
                XmlHelper.AddAttribute(newNode, "module", module.Id);
                XmlHelper.AddLineBreak((XmlElement)document.DocumentElement);
            }

            var f = Path.Combine(folder, item.Name + ".modules.xml");
            WriteFileIfNeedBe(f, document.ToIndentedString(), generatedFileList);
        }

        private static void SaveModules(string folder, StoredProcedure item, List<string> generatedFileList)
        {
            var document = new XmlDocument();
            document.LoadXml(@"<configuration type=""storedprocedure.modules"" id=""" + item.Id + @"""></configuration>");

            XmlHelper.AddLineBreak((XmlElement)document.DocumentElement);
            foreach (var module in item.Modules)
            {
                var newNode = XmlHelper.AddElement(document.DocumentElement, "map");
                XmlHelper.AddAttribute(newNode, "module", module.Id);
                XmlHelper.AddLineBreak((XmlElement)document.DocumentElement);
            }

            var f = Path.Combine(folder, item.Name + ".modules.xml");
            WriteFileIfNeedBe(f, document.ToIndentedString(), generatedFileList);
        }

        private static void SaveModules(string folder, Function item, List<string> generatedFileList)
        {
            var document = new XmlDocument();
            document.LoadXml(@"<configuration type=""function.modules"" id=""" + item.Id + @"""></configuration>");

            XmlHelper.AddLineBreak((XmlElement)document.DocumentElement);
            foreach (var module in item.Modules)
            {
                var newNode = XmlHelper.AddElement(document.DocumentElement, "map");
                XmlHelper.AddAttribute(newNode, "module", module.Id);
                XmlHelper.AddLineBreak((XmlElement)document.DocumentElement);
            }

            var f = Path.Combine(folder, item.Name + ".modules.xml");
            WriteFileIfNeedBe(f, document.ToIndentedString(), generatedFileList);
        }

        private static void SaveToDisk(nHydrateModel modelRoot, IEnumerable<ModelMetadata> list, string rootFolder, nHydrateDiagram diagram, List<string> generatedFileList)
        {
            if (!list.Any())
                return;

            var document = new XmlDocument();
            document.LoadXml(@"<configuration type=""model.metadata""></configuration>");

            XmlHelper.AddLineBreak((XmlElement)document.DocumentElement);
            foreach (var metadata in list)
            {
                var metaDataNode = XmlHelper.AddElement(document.DocumentElement, "metadata");
                XmlHelper.AddAttribute(metaDataNode, "key", metadata.Key);
                XmlHelper.AddAttribute(metaDataNode, "value", metadata.Value);
                XmlHelper.AddLineBreak((XmlElement)document.DocumentElement);
            }

            var f = Path.Combine(rootFolder, "metadata.configuration.xml");
            WriteFileIfNeedBe(f, document.ToIndentedString(), generatedFileList);
        }

        private static void SaveEntityMetaData(string folder, Entity item, List<string> generatedFileList)
        {
            var f = Path.Combine(folder, item.Name + ".metadata.xml");
            if (item.StaticDatum.Count == 0)
                return;

            var fieldMetaData = item.Fields.SelectMany(x => x.FieldMetadata).ToList();
            if (item.EntityMetadata.Count == 0 && fieldMetaData.Count == 0)
                return;

            var document = new XmlDocument();
            document.LoadXml(@"<configuration type=""entity.metadata"" id=""" + item.Id + @"""></configuration>");

            XmlHelper.AddLineBreak((XmlElement)document.DocumentElement);
            foreach (var metadata in item.EntityMetadata)
            {
                var metaDataNode = XmlHelper.AddElement(document.DocumentElement, "metadata");
                XmlHelper.AddAttribute(metaDataNode, "key", metadata.Key);
                XmlHelper.AddAttribute(metaDataNode, "value", metadata.Value);
                XmlHelper.AddLineBreak((XmlElement)document.DocumentElement);

                //Metadata for fields
                var metadataColumnsNodes = XmlHelper.AddElement((XmlElement)document.DocumentElement, "fieldset") as XmlElement;
                XmlHelper.AddLineBreak((XmlElement)metadataColumnsNodes);
                foreach (var fmd in fieldMetaData)
                {
                    var fmdNode = XmlHelper.AddElement(metadataColumnsNodes, "metadata");
                    XmlHelper.AddAttribute(fmdNode, "columnid", fmd.Field.Id);
                    XmlHelper.AddAttribute(fmdNode, "key", fmd.Key);
                    XmlHelper.AddAttribute(fmdNode, "value", fmd.Value);

                    XmlHelper.AddLineBreak((XmlElement)metadataColumnsNodes);
                }

            }

            WriteFileIfNeedBe(f, document.ToIndentedString(), generatedFileList);
        }

        private static void SaveEntityStaticData(string folder, Entity item, List<string> generatedFileList)
        {
            var f = Path.Combine(folder, item.Name + ".staticdata.xml");
            if (item.StaticDatum.Count == 0)
                return;

            var document = new XmlDocument();
            document.LoadXml(@"<configuration type=""entity.staticdata"" id=""" + item.Id + @"""></configuration>");

            XmlHelper.AddLineBreak((XmlElement)document.DocumentElement);
            foreach (var data in item.StaticDatum)
            {
                var dataNode = XmlHelper.AddElement(document.DocumentElement, "data");

                XmlHelper.AddAttribute(dataNode, "columnkey", data.ColumnKey);
                XmlHelper.AddAttribute(dataNode, "value", data.Value);
                XmlHelper.AddAttribute(dataNode, "orderkey", data.OrderKey);
                XmlHelper.AddLineBreak((XmlElement)document.DocumentElement);
            }

            WriteFileIfNeedBe(f, document.ToIndentedString(), generatedFileList);
        }

        private static void SaveRelations(string folder, Entity item, List<string> generatedFileList)
        {
            var document = new XmlDocument();
            document.LoadXml(@"<configuration type=""entity.relations"" id=""" + item.Id + @"""></configuration>");

            XmlHelper.AddLineBreak((XmlElement)document.DocumentElement);
            foreach (var relation in item.RelationshipList)
            {
                var relationNode = XmlHelper.AddElement(document.DocumentElement, "relation");

                XmlHelper.AddLineBreak((XmlElement)relationNode);
                XmlHelper.AddCData((XmlElement)relationNode, "summary", relation.Summary);
                XmlHelper.AddLineBreak((XmlElement)relationNode);

                XmlHelper.AddAttribute(relationNode, "id", relation.InternalId);
                XmlHelper.AddAttribute(relationNode, "childid", relation.ChildEntity.Id);
                XmlHelper.AddAttribute(relationNode, "isenforced", relation.IsEnforced);
                XmlHelper.AddAttribute(relationNode, "isinherited", false);
                XmlHelper.AddAttribute(relationNode, "rolename", relation.RoleName);

                XmlHelper.AddLineBreak((XmlElement)document.DocumentElement);

                //Process the columns
                var relationColumnsNodes = XmlHelper.AddElement((XmlElement)relationNode, "relationfieldset") as XmlElement;
                XmlHelper.AddLineBreak((XmlElement)relationColumnsNodes);
                foreach (var relationField in relation.FieldMapList())
                {
                    var realtionFieldNode = XmlHelper.AddElement(relationColumnsNodes, "field");
                    XmlHelper.AddAttribute(realtionFieldNode, "id", relationField.Id);
                    XmlHelper.AddAttribute(realtionFieldNode, "sourcefieldid", relationField.SourceFieldId);
                    XmlHelper.AddAttribute(realtionFieldNode, "targetfieldid", relationField.TargetFieldId);

                    XmlHelper.AddLineBreak((XmlElement)relationColumnsNodes);
                }
            }

            foreach (var relation in item.ParentRelationshipList)
            {
                var relationNode = XmlHelper.AddElement(document.DocumentElement, "relation");

                XmlHelper.AddLineBreak((XmlElement)relationNode);
                XmlHelper.AddCData((XmlElement)relationNode, "summary", relation.Summary);
                XmlHelper.AddLineBreak((XmlElement)relationNode);

                XmlHelper.AddAttribute(relationNode, "id", relation.InternalId);
                XmlHelper.AddAttribute(relationNode, "parentid", relation.ParentInheritedEntity.Id);
                XmlHelper.AddAttribute(relationNode, "isenforced", relation.IsEnforced);
                XmlHelper.AddAttribute(relationNode, "isinherited", true);
                XmlHelper.AddAttribute(relationNode, "rolename", relation.RoleName);
                XmlHelper.AddLineBreak((XmlElement)document.DocumentElement);
            }

            foreach (var relation in item.RelationshipViewList)
            {
                var relationNode = XmlHelper.AddElement(document.DocumentElement, "relation");
                XmlHelper.AddAttribute(relationNode, "isviewrelation", "true");

                XmlHelper.AddLineBreak((XmlElement)relationNode);
                XmlHelper.AddCData((XmlElement)relationNode, "summary", relation.Summary);
                XmlHelper.AddLineBreak((XmlElement)relationNode);

                XmlHelper.AddAttribute(relationNode, "id", relation.InternalId);
                XmlHelper.AddAttribute(relationNode, "childid", relation.ChildView.Id);
                XmlHelper.AddAttribute(relationNode, "rolename", relation.RoleName);

                XmlHelper.AddLineBreak((XmlElement)document.DocumentElement);

                //Process the columns
                var relationColumnsNodes = XmlHelper.AddElement((XmlElement)relationNode, "relationfieldset") as XmlElement;
                XmlHelper.AddLineBreak((XmlElement)relationColumnsNodes);
                foreach (var relationField in relation.FieldMapList())
                {
                    var realtionFieldNode = XmlHelper.AddElement(relationColumnsNodes, "field");
                    XmlHelper.AddAttribute(realtionFieldNode, "id", relationField.Id);
                    XmlHelper.AddAttribute(realtionFieldNode, "sourcefieldid", relationField.SourceFieldId);
                    XmlHelper.AddAttribute(realtionFieldNode, "targetfieldid", relationField.TargetFieldId);

                    XmlHelper.AddLineBreak((XmlElement)relationColumnsNodes);
                }
            }

            var f = Path.Combine(folder, item.Name + ".relations.xml");
            WriteFileIfNeedBe(f, document.ToIndentedString(), generatedFileList);

        }

        private static void SaveDiagramFiles(string rootFolder, nHydrateDiagram diagram, List<string> generatedFileList)
        {
            var fileName = Path.Combine(rootFolder, "diagram.xml");
            var document = new XmlDocument();
            document.LoadXml(@"<configuration type=""diagram""></configuration>");
            foreach (var shape in diagram.NestedChildShapes)
            {
                if (shape is EntityShape)
                {
                    var item = ((shape as EntityShape).ModelElement as Entity);
                    var node = XmlHelper.AddElement(document.DocumentElement, "element");
                    XmlHelper.AddAttribute(node, "id", shape.ModelElement.Id);
                    XmlHelper.AddAttribute(node, "bounds", shape.AbsoluteBoundingBox.ToXmlValue());
                }
                else if (shape is ViewShape)
                {
                    var item = ((shape as ViewShape).ModelElement as View);
                    var node = XmlHelper.AddElement(document.DocumentElement, "element");
                    XmlHelper.AddAttribute(node, "id", shape.ModelElement.Id);
                    XmlHelper.AddAttribute(node, "bounds", shape.AbsoluteBoundingBox.ToXmlValue());
                }
                else if (shape is StoredProcedureShape)
                {
                    var item = ((shape as StoredProcedureShape).ModelElement as StoredProcedure);
                    var node = XmlHelper.AddElement(document.DocumentElement, "element");
                    XmlHelper.AddAttribute(node, "id", shape.ModelElement.Id);
                    XmlHelper.AddAttribute(node, "bounds", shape.AbsoluteBoundingBox.ToXmlValue());
                }
                else if (shape is FunctionShape)
                {
                    var item = ((shape as FunctionShape).ModelElement as Function);
                    var node = XmlHelper.AddElement(document.DocumentElement, "element");
                    XmlHelper.AddAttribute(node, "id", shape.ModelElement.Id);
                    XmlHelper.AddAttribute(node, "bounds", shape.AbsoluteBoundingBox.ToXmlValue());
                }
            }
            WriteFileIfNeedBe(fileName, document.ToIndentedString(), generatedFileList);
        }

        /// <summary>
        /// Saves Stored Procedures to disk
        /// </summary>
        private static void SaveToDisk(nHydrateModel modelRoot, IEnumerable<StoredProcedure> list, string rootFolder, nHydrateDiagram diagram, List<string> generatedFileList)
        {
            var folder = Path.Combine(rootFolder, FOLDER_SP);
            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
            foreach (var item in list)
            {
                var f = Path.Combine(folder, item.Name + ".sql");
                WriteFileIfNeedBe(f, item.SQL, generatedFileList);
            }

            #region Save other parameter/field information
            foreach (var item in list)
            {
                var document = new XmlDocument();
                document.LoadXml(@"<configuration type=""storedprocedure"" name=""" + item.Name + @"""></configuration>");

                XmlHelper.AddLineBreak(document.DocumentElement);
                XmlHelper.AddCData(document.DocumentElement, "summary", item.Summary);
                XmlHelper.AddLineBreak(document.DocumentElement);

                XmlHelper.AddAttribute(document.DocumentElement, "id", item.Id);
                XmlHelper.AddAttribute(document.DocumentElement, "precedenceorder", item.PrecedenceOrder);
                XmlHelper.AddAttribute(document.DocumentElement, "codefacade", item.CodeFacade);
                XmlHelper.AddAttribute(document.DocumentElement, "databaseobjectname", item.DatabaseObjectName);
                XmlHelper.AddAttribute(document.DocumentElement, "isexisting", item.IsExisting);
                XmlHelper.AddAttribute(document.DocumentElement, "isgenerated", item.IsGenerated);
                XmlHelper.AddAttribute(document.DocumentElement, "schema", item.Schema);
                XmlHelper.AddAttribute(document.DocumentElement, "generatesdoublederived", item.GeneratesDoubleDerived);

                var fieldsNodes = XmlHelper.AddElement(document.DocumentElement, "fieldset") as XmlElement;
                XmlHelper.AddLineBreak((XmlElement)fieldsNodes);

                foreach (var field in item.Fields.OrderBy(x => x.Name))
                {
                    var fieldNode = XmlHelper.AddElement(fieldsNodes, "field");

                    XmlHelper.AddLineBreak((XmlElement)fieldNode);
                    XmlHelper.AddCData((XmlElement)fieldNode, "summary", field.Summary);
                    XmlHelper.AddLineBreak((XmlElement)fieldNode);

                    XmlHelper.AddAttribute(fieldNode, "id", field.Id);
                    XmlHelper.AddAttribute(fieldNode, "name", field.Name);
                    XmlHelper.AddAttribute(fieldNode, "nullable", field.Nullable);
                    XmlHelper.AddAttribute(fieldNode, "datatype", field.DataType.ToString());
                    XmlHelper.AddAttribute(fieldNode, "codefacade", field.CodeFacade);
                    XmlHelper.AddAttribute(fieldNode, "default", field.Default);
                    XmlHelper.AddAttribute(fieldNode, "isgenerated", field.IsGenerated);
                    XmlHelper.AddAttribute(fieldNode, "length", field.Length);
                    XmlHelper.AddAttribute(fieldNode, "scale", field.Scale);

                    XmlHelper.AddLineBreak((XmlElement)fieldsNodes);
                }

                var parametersNodes = XmlHelper.AddElement(document.DocumentElement, "parameterset") as XmlElement;
                XmlHelper.AddLineBreak((XmlElement)parametersNodes);

                foreach (var parameter in item.Parameters.OrderBy(x => x.Name))
                {
                    var parameterNode = XmlHelper.AddElement(parametersNodes, "parameter");

                    XmlHelper.AddLineBreak((XmlElement)parameterNode);
                    XmlHelper.AddCData((XmlElement)parameterNode, "summary", parameter.Summary);
                    XmlHelper.AddLineBreak((XmlElement)parameterNode);

                    XmlHelper.AddAttribute(parameterNode, "id", parameter.Id);
                    XmlHelper.AddAttribute(parameterNode, "name", parameter.Name);
                    XmlHelper.AddAttribute(parameterNode, "nullable", parameter.Nullable);
                    XmlHelper.AddAttribute(parameterNode, "datatype", parameter.DataType.ToString());
                    XmlHelper.AddAttribute(parameterNode, "codefacade", parameter.CodeFacade);
                    XmlHelper.AddAttribute(parameterNode, "default", parameter.Default);
                    XmlHelper.AddAttribute(parameterNode, "isgenerated", parameter.IsGenerated);
                    XmlHelper.AddAttribute(parameterNode, "isoutput", parameter.IsOutputParameter);
                    XmlHelper.AddAttribute(parameterNode, "length", parameter.Length);
                    XmlHelper.AddAttribute(parameterNode, "scale", parameter.Scale);

                    XmlHelper.AddLineBreak((XmlElement)parametersNodes);
                }

                XmlHelper.AddLineBreak(document.DocumentElement);
                var f = Path.Combine(folder, item.Name + ".configuration.xml");
                WriteFileIfNeedBe(f, document.ToIndentedString(), generatedFileList);

                //Save other files
                SaveModules(folder, item, generatedFileList);

            }
            #endregion

            WriteReadMeFile(folder, generatedFileList);
        }

        /// <summary>
        /// Saves Views to disk
        /// </summary>
        private static void SaveToDisk(nHydrateModel modelRoot, IEnumerable<View> list, string rootFolder, nHydrateDiagram diagram, List<string> generatedFileList)
        {
            var folder = Path.Combine(rootFolder, FOLDER_VW);
            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
            foreach (var item in list)
            {
                var f = Path.Combine(folder, item.Name + ".sql");
                WriteFileIfNeedBe(f, item.SQL, generatedFileList);
            }

            #region Save other parameter/field information
            foreach (var item in list)
            {
                var document = new XmlDocument();
                document.LoadXml(@"<configuration type=""view"" name=""" + item.Name + @"""></configuration>");

                XmlHelper.AddLineBreak(document.DocumentElement);
                XmlHelper.AddCData(document.DocumentElement, "summary", item.Summary);
                XmlHelper.AddLineBreak(document.DocumentElement);

                XmlHelper.AddAttribute(document.DocumentElement, "id", item.Id);
                XmlHelper.AddAttribute(document.DocumentElement, "precedenceorder", item.PrecedenceOrder);
                XmlHelper.AddAttribute(document.DocumentElement, "codefacade", item.CodeFacade);
                XmlHelper.AddAttribute(document.DocumentElement, "isgenerated", item.IsGenerated);
                XmlHelper.AddAttribute(document.DocumentElement, "schema", item.Schema);
                XmlHelper.AddAttribute(document.DocumentElement, "generatesdoublederived", item.GeneratesDoubleDerived);

                var fieldsNodes = XmlHelper.AddElement(document.DocumentElement, "fieldset") as XmlElement;
                XmlHelper.AddLineBreak((XmlElement)fieldsNodes);

                foreach (var field in item.Fields.OrderBy(x => x.Name))
                {
                    var fieldNode = XmlHelper.AddElement(fieldsNodes, "field");

                    XmlHelper.AddLineBreak((XmlElement)fieldNode);
                    XmlHelper.AddCData((XmlElement)fieldNode, "summary", field.Summary);
                    XmlHelper.AddLineBreak((XmlElement)fieldNode);

                    XmlHelper.AddAttribute(fieldNode, "id", field.Id);
                    XmlHelper.AddAttribute(fieldNode, "name", field.Name);
                    XmlHelper.AddAttribute(fieldNode, "nullable", field.Nullable);
                    XmlHelper.AddAttribute(fieldNode, "datatype", field.DataType.ToString());
                    XmlHelper.AddAttribute(fieldNode, "codefacade", field.CodeFacade);
                    XmlHelper.AddAttribute(fieldNode, "default", field.Default);
                    XmlHelper.AddAttribute(fieldNode, "isgenerated", field.IsGenerated);
                    XmlHelper.AddAttribute(fieldNode, "length", field.Length);
                    XmlHelper.AddAttribute(fieldNode, "scale", field.Scale);

                    XmlHelper.AddLineBreak((XmlElement)fieldsNodes);
                }

                XmlHelper.AddLineBreak(document.DocumentElement);
                var f = Path.Combine(folder, item.Name + ".configuration.xml");
                WriteFileIfNeedBe(f, document.ToIndentedString(), generatedFileList);

                //Save other files
                SaveModules(folder, item, generatedFileList);

            }
            #endregion

            WriteReadMeFile(folder, generatedFileList);
        }

        /// <summary>
        /// Saves Functions to disk
        /// </summary>
        private static void SaveToDisk(nHydrateModel modelRoot, IEnumerable<Function> list, string rootFolder, nHydrateDiagram diagram, List<string> generatedFileList)
        {
            var folder = Path.Combine(rootFolder, FOLDER_FC);
            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
            foreach (var item in list)
            {
                var f = Path.Combine(folder, item.Name + ".sql");
                WriteFileIfNeedBe(f, item.SQL, generatedFileList);
            }

            #region Save other parameter/field information
            foreach (var item in list)
            {
                var document = new XmlDocument();
                document.LoadXml(@"<configuration type=""function"" name=""" + item.Name + @"""></configuration>");

                XmlHelper.AddLineBreak(document.DocumentElement);
                XmlHelper.AddCData(document.DocumentElement, "summary", item.Summary);
                XmlHelper.AddLineBreak(document.DocumentElement);

                XmlHelper.AddAttribute(document.DocumentElement, "id", item.Id);
                XmlHelper.AddAttribute(document.DocumentElement, "precedenceorder", item.PrecedenceOrder);
                XmlHelper.AddAttribute(document.DocumentElement, "codefacade", item.CodeFacade);
                XmlHelper.AddAttribute(document.DocumentElement, "isgenerated", item.IsGenerated);
                XmlHelper.AddAttribute(document.DocumentElement, "schema", item.Schema);
                XmlHelper.AddAttribute(document.DocumentElement, "istable", item.IsTable);
                XmlHelper.AddAttribute(document.DocumentElement, "returnvariable", item.ReturnVariable);

                var fieldsNodes = XmlHelper.AddElement(document.DocumentElement, "fieldset") as XmlElement;
                XmlHelper.AddLineBreak((XmlElement)fieldsNodes);

                foreach (var field in item.Fields.OrderBy(x => x.Name))
                {
                    var fieldNode = XmlHelper.AddElement(fieldsNodes, "field");

                    XmlHelper.AddLineBreak((XmlElement)fieldNode);
                    XmlHelper.AddCData((XmlElement)fieldNode, "summary", field.Summary);
                    XmlHelper.AddLineBreak((XmlElement)fieldNode);

                    XmlHelper.AddAttribute(fieldNode, "id", field.Id);
                    XmlHelper.AddAttribute(fieldNode, "name", field.Name);
                    XmlHelper.AddAttribute(fieldNode, "nullable", field.Nullable);
                    XmlHelper.AddAttribute(fieldNode, "datatype", field.DataType.ToString());
                    XmlHelper.AddAttribute(fieldNode, "codefacade", field.CodeFacade);
                    XmlHelper.AddAttribute(fieldNode, "default", field.Default);
                    XmlHelper.AddAttribute(fieldNode, "isgenerated", field.IsGenerated);
                    XmlHelper.AddAttribute(fieldNode, "length", field.Length);
                    XmlHelper.AddAttribute(fieldNode, "scale", field.Scale);

                    XmlHelper.AddLineBreak((XmlElement)fieldsNodes);
                }

                var parametersNodes = XmlHelper.AddElement(document.DocumentElement, "parameterset") as XmlElement;
                XmlHelper.AddLineBreak((XmlElement)parametersNodes);

                foreach (var parameter in item.Parameters.OrderBy(x => x.Name))
                {
                    var parameterNode = XmlHelper.AddElement(parametersNodes, "parameter");

                    XmlHelper.AddLineBreak((XmlElement)parameterNode);
                    XmlHelper.AddCData((XmlElement)parameterNode, "summary", parameter.Summary);
                    XmlHelper.AddLineBreak((XmlElement)parameterNode);

                    XmlHelper.AddAttribute(parameterNode, "id", parameter.Id);
                    XmlHelper.AddAttribute(parameterNode, "name", parameter.Name);
                    XmlHelper.AddAttribute(parameterNode, "nullable", parameter.Nullable);
                    XmlHelper.AddAttribute(parameterNode, "datatype", parameter.DataType.ToString());
                    XmlHelper.AddAttribute(parameterNode, "codefacade", parameter.CodeFacade);
                    XmlHelper.AddAttribute(parameterNode, "default", parameter.Default);
                    XmlHelper.AddAttribute(parameterNode, "isgenerated", parameter.IsGenerated);
                    XmlHelper.AddAttribute(parameterNode, "length", parameter.Length);
                    XmlHelper.AddAttribute(parameterNode, "scale", parameter.Scale);

                    XmlHelper.AddLineBreak((XmlElement)parametersNodes);
                }

                XmlHelper.AddLineBreak(document.DocumentElement);
                var f = Path.Combine(folder, item.Name + ".configuration.xml");
                WriteFileIfNeedBe(f, document.ToIndentedString(), generatedFileList);

                //Save other files
                SaveModules(folder, item, generatedFileList);

            }
            #endregion

            WriteReadMeFile(folder, generatedFileList);
        }

        /// <summary>
        /// Saves Modules to disk
        /// </summary>
        private static void SaveToDisk(nHydrateModel modelRoot, IEnumerable<Module> list, string rootFolder, nHydrateDiagram diagram, List<string> generatedFileList)
        {
            var folder = rootFolder;
            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

            var document = new XmlDocument();
            document.LoadXml(@"<configuration type=""module""></configuration>");

            foreach (var item in list)
            {
                var moduleNode = XmlHelper.AddElement(document.DocumentElement, "module") as XmlElement;
                XmlHelper.AddLineBreak(moduleNode);
                XmlHelper.AddCData(moduleNode, "summary", item.Summary);
                XmlHelper.AddAttribute(moduleNode, "id", item.Id);
                XmlHelper.AddAttribute(moduleNode, "name", item.Name);
                XmlHelper.AddLineBreak(moduleNode);

                var rulesNodes = XmlHelper.AddElement(moduleNode, "ruleset") as XmlElement;
                XmlHelper.AddLineBreak((XmlElement)rulesNodes);

                foreach (var rule in item.ModuleRules.OrderBy(x => x.Name))
                {
                    var fieldNode = XmlHelper.AddElement(rulesNodes, "rule");

                    XmlHelper.AddLineBreak((XmlElement)fieldNode);
                    XmlHelper.AddCData((XmlElement)fieldNode, "summary", rule.Summary);
                    XmlHelper.AddLineBreak((XmlElement)fieldNode);

                    XmlHelper.AddAttribute(fieldNode, "status", rule.Status.ToString("d"));
                    XmlHelper.AddAttribute(fieldNode, "dependentmodule", rule.DependentModule);
                    XmlHelper.AddAttribute(fieldNode, "name", rule.Name);
                    XmlHelper.AddAttribute(fieldNode, "inclusion", rule.Inclusion);
                    XmlHelper.AddAttribute(fieldNode, "enforced", rule.Enforced);

                    XmlHelper.AddLineBreak((XmlElement)rulesNodes);
                }

            }

            var f = Path.Combine(folder, "modules.configuration.xml");
            WriteFileIfNeedBe(f, document.ToIndentedString(), generatedFileList);
            WriteReadMeFile(folder, generatedFileList);
        }

        public static void LoadFromDisk(nHydrateModel model, string rootFolder, Microsoft.VisualStudio.Modeling.Store store, string modelName)
        {
            model.IsSaving = true;
            try
            {
                var modelFolder = GetModelFolder(rootFolder, modelName);
                nHydrate2.Dsl.Custom.SQLFileManagement.LoadFromDisk(model.ModelMetadata, model, modelFolder, store);
                nHydrate2.Dsl.Custom.SQLFileManagement.LoadFromDisk(model.Modules, model, modelFolder, store);
                nHydrate2.Dsl.Custom.SQLFileManagement.LoadFromDisk(model.Views, model, modelFolder, store); //must coem before entities (view relations)
                nHydrate2.Dsl.Custom.SQLFileManagement.LoadFromDisk(model.Entities, model, modelFolder, store);
                nHydrate2.Dsl.Custom.SQLFileManagement.LoadFromDisk(model.StoredProcedures, model, modelFolder, store);
                nHydrate2.Dsl.Custom.SQLFileManagement.LoadFromDisk(model.Functions, model, modelFolder, store);
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                model.IsSaving = false;
            }
        }

        private static void LoadFromDisk(IEnumerable<Entity> list, nHydrateModel model, string rootFolder, Microsoft.VisualStudio.Modeling.Store store)
        {
            var folder = Path.Combine(rootFolder, FOLDER_ET);
            if (!Directory.Exists(folder)) return;

            #region Load other parameter/field information
            var fList = Directory.GetFiles(folder, "*.configuration.xml");
            foreach (var f in fList)
            {
                var document = new XmlDocument();
                try
                {
                    document.Load(f);
                }
                catch (Exception ex)
                {
                    //Do Nothing
                    MessageBox.Show("The file '" + f + "' is not valid and could not be loaded!", "Load Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var fi = new FileInfo(f);
                var name = fi.Name.Substring(0, fi.Name.Length - ".configuration.xml".Length).ToLower();
                var itemID = XmlHelper.GetAttributeValue(document.DocumentElement, "id", Guid.Empty);
                var item = list.FirstOrDefault(x => x.Id == itemID);
                if (item == null)
                {
                    item = new Entity(model.Partition, new PropertyAssignment[] { new PropertyAssignment(ElementFactory.IdPropertyAssignment, XmlHelper.GetAttributeValue(document.DocumentElement, "id", Guid.NewGuid())) });
                    model.Entities.Add(item);
                }

                System.Windows.Forms.Application.DoEvents();

                #region Properties

                item.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(FieldParameter_PropertyChanged);
                item.Name = XmlHelper.GetAttributeValue(document.DocumentElement, "name", item.Name);
                item.AllowAuditTracking = XmlHelper.GetAttributeValue(document.DocumentElement, "allowaudittracking", item.AllowAuditTracking);
                item.AllowCreateAudit = XmlHelper.GetAttributeValue(document.DocumentElement, "allowcreateaudit", item.AllowCreateAudit);
                item.AllowModifyAudit = XmlHelper.GetAttributeValue(document.DocumentElement, "allowmodifyaudit", item.AllowModifyAudit);
                item.AllowTimestamp = XmlHelper.GetAttributeValue(document.DocumentElement, "allowtimestamp", item.AllowTimestamp);
                item.CodeFacade = XmlHelper.GetAttributeValue(document.DocumentElement, "codefacade", item.CodeFacade);
                item.Immutable = XmlHelper.GetAttributeValue(document.DocumentElement, "immutable", item.Immutable);
                item.EnforcePrimaryKey = XmlHelper.GetAttributeValue(document.DocumentElement, "enforceprimarykey", item.EnforcePrimaryKey);
                item.IsAssociative = XmlHelper.GetAttributeValue(document.DocumentElement, "isassociative", item.IsAssociative);
                item.GeneratesDoubleDerived = XmlHelper.GetAttributeValue(document.DocumentElement, "generatesdoublederived", item.GeneratesDoubleDerived);
                item.IsGenerated = XmlHelper.GetAttributeValue(document.DocumentElement, "isgenerated", item.IsGenerated);
                item.IsTenant = XmlHelper.GetAttributeValue(document.DocumentElement, "isTenant", item.IsTenant);

                var tev = XmlHelper.GetAttributeValue(document.DocumentElement, "typedentity", item.TypedEntity.ToString());
                TypedEntityConstants te;
                if (Enum.TryParse<TypedEntityConstants>(tev, true, out te))
                    item.TypedEntity = te;

                item.Schema = XmlHelper.GetAttributeValue(document.DocumentElement, "schema", item.Schema);
                item.Summary = XmlHelper.GetNodeValue(document.DocumentElement, "summary", item.Summary);
                item.PropertyChanged -= new System.ComponentModel.PropertyChangedEventHandler(FieldParameter_PropertyChanged);

                #endregion

                #region Fields

                var fieldsNodes = document.DocumentElement.SelectSingleNode("//fieldset");
                if (fieldsNodes != null)
                {
                    var nameList = new List<string>();
                    foreach (XmlNode n in fieldsNodes.ChildNodes)
                    {
                        var subItemID = XmlHelper.GetAttributeValue(n, "id", Guid.Empty);
                        var field = item.Fields.FirstOrDefault(x => x.Id == subItemID);
                        if (field == null)
                        {
                            field = new Field(item.Partition, new PropertyAssignment[] { new PropertyAssignment(ElementFactory.IdPropertyAssignment, XmlHelper.GetAttributeValue(n, "id", Guid.NewGuid())) });
                            item.Fields.Add(field);
                        }
                        field.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(FieldParameter_PropertyChanged);
                        field.Name = XmlHelper.GetAttributeValue(n, "name", string.Empty);
                        field.CodeFacade = XmlHelper.GetAttributeValue(n, "codefacade", field.CodeFacade);
                        nameList.Add(field.Name.ToLower());
                        field.Nullable = XmlHelper.GetAttributeValue(n, "nullable", field.Nullable);

                        var dtv = XmlHelper.GetAttributeValue(n, "datatype", field.DataType.ToString());
                        DataTypeConstants dt;
                        if (Enum.TryParse<DataTypeConstants>(dtv, true, out dt))
                            field.DataType = dt;

                        var itv = XmlHelper.GetAttributeValue(n, "identity", field.Identity.ToString());
                        IdentityTypeConstants it;
                        if (Enum.TryParse<IdentityTypeConstants>(itv, true, out it))
                            field.Identity = it;

                        field.DataFormatString = XmlHelper.GetNodeValue(n, "dataformatstring", field.DataFormatString);
                        field.Default = XmlHelper.GetAttributeValue(n, "default", field.Default);
                        field.DefaultIsFunc = XmlHelper.GetAttributeValue(n, "defaultisfunc", field.DefaultIsFunc);
                        field.Formula = XmlHelper.GetAttributeValue(n, "formula", field.Formula);
                        field.IsGenerated = XmlHelper.GetAttributeValue(n, "isgenerated", field.IsGenerated);
                        field.IsIndexed = XmlHelper.GetAttributeValue(n, "isindexed", field.IsIndexed);
                        field.IsPrimaryKey = XmlHelper.GetAttributeValue(n, "isprimarykey", field.IsPrimaryKey);
                        field.IsCalculated = XmlHelper.GetAttributeValue(n, "Iscalculated", field.IsCalculated);
                        field.IsUnique = XmlHelper.GetAttributeValue(n, "isunique", field.IsUnique);
                        field.Length = XmlHelper.GetAttributeValue(n, "length", field.Length);
                        field.Scale = XmlHelper.GetAttributeValue(n, "scale", field.Scale);
                        field.SortOrder = XmlHelper.GetAttributeValue(n, "sortorder", field.SortOrder);
                        field.IsReadOnly = XmlHelper.GetAttributeValue(n, "isreadonly", field.IsReadOnly);
                        field.Category = XmlHelper.GetAttributeValue(n, "category", field.Category);
                        field.Collate = XmlHelper.GetAttributeValue(n, "collate", field.Collate);
                        field.FriendlyName = XmlHelper.GetAttributeValue(n, "friendlyname", field.FriendlyName);
                        field.IsBrowsable = XmlHelper.GetAttributeValue(n, "isbrowsable", field.IsBrowsable);
                        field.Max = XmlHelper.GetAttributeValue(n, "max", field.Max);
                        field.Min = XmlHelper.GetAttributeValue(n, "min", field.Min);
                        field.ValidationExpression = XmlHelper.GetAttributeValue(n, "validationexpression", field.ValidationExpression);
                        field.Summary = XmlHelper.GetNodeValue(n, "summary", field.Summary);
                        field.PropertyChanged -= new System.ComponentModel.PropertyChangedEventHandler(FieldParameter_PropertyChanged);
                    }
                    if (item.Fields.Remove(x => !nameList.Contains(x.Name.ToLower())) > 0)
                        item.nHydrateModel.IsDirty = true;
                }

                #endregion

                LoadEntityIndexes(folder, item);

                //Order fields (skip for model that did not have sort order when saved)
                var fc = new FieldOrderComparer();
                if (item.Fields.Count(x => x.SortOrder > 0) > 0)
                    item.Fields.Sort(fc.Compare);
            }

            //Must load relations AFTER ALL entities are loaded
            foreach (var item in model.Entities)
            {
                LoadEntityRelations(folder, item);
                LoadModules(folder, item);
                LoadEntityStaticData(folder, item);
                LoadEntityMetaData(folder, item);
                LoadEntityComposites(folder, item);
            }

            #endregion

        }

        private static void LoadEntityIndexes(string folder, Entity entity)
        {
            XmlDocument document = null;
            var fileName = Path.Combine(folder, entity.Name + ".indexes.xml");
            if (!File.Exists(fileName)) return;
            try
            {
                document = new XmlDocument();
                document.Load(fileName);
            }
            catch (Exception ex)
            {
                //Do Nothing
                MessageBox.Show("The file '" + fileName + "' is not valid and could not be loaded!", "Load Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            foreach (XmlNode n in document.DocumentElement)
            {
                var id = XmlHelper.GetAttributeValue(n, "id", Guid.NewGuid());
                var newIndex = entity.Indexes.FirstOrDefault(x => x.Id == id);
                if (newIndex == null)
                {
                    newIndex = new Index(entity.Partition, new PropertyAssignment[] { new PropertyAssignment(ElementFactory.IdPropertyAssignment, id) });
                    entity.Indexes.Add(newIndex);
                }
                newIndex.Clustered = XmlHelper.GetAttributeValue(n, "clustered", newIndex.Clustered);
                newIndex.ImportedName = XmlHelper.GetAttributeValue(n, "importedname", newIndex.ImportedName);
                newIndex.IndexType = (IndexTypeConstants)XmlHelper.GetAttributeValue(n, "indextype", int.Parse(newIndex.IndexType.ToString("d")));
                newIndex.IsUnique = XmlHelper.GetAttributeValue(n, "isunique", newIndex.IsUnique);

                var indexColumnsNode = n.SelectSingleNode("indexcolumnset");
                if (indexColumnsNode != null)
                {
                    foreach (XmlNode m in indexColumnsNode.ChildNodes)
                    {
                        id = XmlHelper.GetAttributeValue(m, "id", Guid.NewGuid());
                        var newIndexColumn = newIndex.IndexColumns.FirstOrDefault(x => x.Id == id);
                        if (newIndexColumn == null)
                        {
                            newIndexColumn = new IndexColumn(entity.Partition, new PropertyAssignment[] { new PropertyAssignment(ElementFactory.IdPropertyAssignment, id) });
                            newIndex.IndexColumns.Add(newIndexColumn);
                        }
                        newIndexColumn.Ascending = XmlHelper.GetAttributeValue(m, "ascending", newIndexColumn.Ascending);
                        newIndexColumn.FieldID = XmlHelper.GetAttributeValue(m, "fieldid", newIndexColumn.FieldID);
                        newIndexColumn.SortOrder = XmlHelper.GetAttributeValue(m, "sortorder", newIndexColumn.SortOrder);
                        newIndexColumn.IsInternal = true;
                    }
                }
            }

        }

        private static void LoadEntityMetaData(string folder, Entity entity)
        {
            XmlDocument document = null;
            var fileName = Path.Combine(folder, entity.Name + ".metadata.xml");
            if (!File.Exists(fileName)) return;
            try
            {
                document = new XmlDocument();
                document.Load(fileName);
            }
            catch (Exception ex)
            {
                //Do Nothing
                MessageBox.Show("The file '" + fileName + "' is not valid and could not be loaded!", "Load Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            foreach (XmlNode n in document.DocumentElement)
            {
                if (n.Name == "fieldset")
                {
                    foreach (XmlElement n2 in n.ChildNodes)
                    {
                        var columnid = XmlHelper.GetAttributeValue(n2, "columnid", Guid.Empty);
                        var field = entity.Fields.FirstOrDefault(x => x.Id == columnid);
                        if (field != null)
                        {
                            var fieldMD = new FieldMetadata(entity.Partition);
                            fieldMD.Key = XmlHelper.GetAttributeValue(n2, "key", fieldMD.Key);
                            fieldMD.Value = XmlHelper.GetAttributeValue(n2, "value", fieldMD.Value);
                            field.FieldMetadata.Add(fieldMD);
                        }
                    }
                }
                else
                {
                    var newMD = new EntityMetadata(entity.Partition);
                    newMD.Key = XmlHelper.GetAttributeValue(n, "key", newMD.Key);
                    newMD.Value = XmlHelper.GetAttributeValue(n, "value", newMD.Value);
                    entity.EntityMetadata.Add(newMD);
                }
            }
        }

        private static void LoadEntityRelations(string folder, Entity entity)
        {
            XmlDocument document = null;
            var fileName = Path.Combine(folder, entity.Name + ".relations.xml");
            if (!File.Exists(fileName)) return;
            try
            {
                document = new XmlDocument();
                document.Load(fileName);
            }
            catch (Exception ex)
            {
                //Do Nothing
                MessageBox.Show("The file '" + fileName + "' is not valid and could not be loaded!", "Load Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            foreach (XmlNode n in document.DocumentElement)
            {
                var isViewRelation = XmlHelper.GetAttributeValue(n, "isviewrelation", false);
                var isInherited = XmlHelper.GetAttributeValue(n, "isinherited", false);
                if (isViewRelation) //View Relation
                {
                    var childid = XmlHelper.GetAttributeValue(n, "childid", Guid.Empty);
                    var child = entity.nHydrateModel.Views.FirstOrDefault(x => x.Id == childid);
                    if (child != null)
                    {
                        entity.ChildViews.Add(child);
                        var connection = entity.Store.CurrentContext.Partitions.First().Value.ElementDirectory.AllElements.Last() as EntityHasViews;
                        connection.InternalId = XmlHelper.GetAttributeValue(n, "id", Guid.Empty);
                        connection.RoleName = XmlHelper.GetAttributeValue(n, "rolename", connection.RoleName);

                        var relationColumnsNode = n.SelectSingleNode("relationfieldset");
                        if (relationColumnsNode != null)
                        {
                            foreach (XmlNode m in relationColumnsNode.ChildNodes)
                            {
                                var sourceFieldID = XmlHelper.GetAttributeValue(m, "sourcefieldid", Guid.Empty);
                                var targetFieldID = XmlHelper.GetAttributeValue(m, "targetfieldid", Guid.Empty);
                                var sourceField = entity.Fields.FirstOrDefault(x => x.Id == sourceFieldID);
                                var targetField = entity.nHydrateModel.Views.SelectMany(x => x.Fields).FirstOrDefault(x => x.Id == targetFieldID);
                                if ((sourceField != null) && (targetField != null))
                                {
                                    var id = XmlHelper.GetAttributeValue(m, "id", Guid.NewGuid());
                                    var newRelationField = new RelationField(entity.Partition, new PropertyAssignment[] { new PropertyAssignment(ElementFactory.IdPropertyAssignment, id) });
                                    newRelationField.SourceFieldId = sourceFieldID;
                                    newRelationField.TargetFieldId = targetFieldID;
                                    newRelationField.RelationID = connection.Id;
                                    entity.nHydrateModel.RelationFields.Add(newRelationField);
                                }
                            }
                        }
                    }

                }
                else if (isInherited) //Inheritence Relation
                {
                    var parentid = XmlHelper.GetAttributeValue(n, "parentid", Guid.Empty);
                    var parent = entity.nHydrateModel.Entities.FirstOrDefault(x => x.Id == parentid);
                    if (parent != null)
                    {
                        entity.ChildDerivedEntities.Add(parent);
                        var connection = entity.Store.CurrentContext.Partitions.First().Value.ElementDirectory.AllElements.Last() as EntityInheritsEntity;
                        connection.InternalId = XmlHelper.GetAttributeValue(n, "id", Guid.Empty);
                        connection.IsEnforced = XmlHelper.GetAttributeValue(n, "isenforced", connection.IsEnforced);
                        connection.RoleName = XmlHelper.GetAttributeValue(n, "rolename", connection.RoleName);
                    }

                }
                else //Normal Relation
                {
                    var childid = XmlHelper.GetAttributeValue(n, "childid", Guid.Empty);
                    var child = entity.nHydrateModel.Entities.FirstOrDefault(x => x.Id == childid);
                    if (child != null)
                    {
                        entity.ChildEntities.Add(child);
                        var connection = entity.Store.CurrentContext.Partitions.First().Value.ElementDirectory.AllElements.Last() as EntityHasEntities;
                        connection.InternalId = XmlHelper.GetAttributeValue(n, "id", Guid.Empty);
                        connection.IsEnforced = XmlHelper.GetAttributeValue(n, "isenforced", connection.IsEnforced);
                        connection.RoleName = XmlHelper.GetAttributeValue(n, "rolename", connection.RoleName);

                        var relationColumnsNode = n.SelectSingleNode("relationfieldset");
                        if (relationColumnsNode != null)
                        {
                            foreach (XmlNode m in relationColumnsNode.ChildNodes)
                            {
                                var sourceFieldID = XmlHelper.GetAttributeValue(m, "sourcefieldid", Guid.Empty);
                                var targetFieldID = XmlHelper.GetAttributeValue(m, "targetfieldid", Guid.Empty);
                                var sourceField = entity.Fields.FirstOrDefault(x => x.Id == sourceFieldID);
                                var targetField = entity.nHydrateModel.Entities.SelectMany(x => x.Fields).FirstOrDefault(x => x.Id == targetFieldID);
                                if ((sourceField != null) && (targetField != null))
                                {
                                    var id = XmlHelper.GetAttributeValue(m, "id", Guid.NewGuid());
                                    var newRelationField = new RelationField(entity.Partition, new PropertyAssignment[] { new PropertyAssignment(ElementFactory.IdPropertyAssignment, id) });
                                    newRelationField.SourceFieldId = sourceFieldID;
                                    newRelationField.TargetFieldId = targetFieldID;
                                    newRelationField.RelationID = connection.Id;
                                    entity.nHydrateModel.RelationFields.Add(newRelationField);
                                }
                            }
                        }
                    }

                }

            }

        }

        private static void LoadEntityStaticData(string folder, Entity entity)
        {
            XmlDocument document = null;
            var fileName = Path.Combine(folder, entity.Name + ".staticdata.xml");
            if (!File.Exists(fileName)) return;
            try
            {
                document = new XmlDocument();
                document.Load(fileName);
            }
            catch (Exception ex)
            {
                //Do Nothing
                MessageBox.Show("The file '" + fileName + "' is not valid and could not be loaded!", "Load Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            foreach (XmlNode n in document.DocumentElement)
            {
                var newData = new StaticData(entity.Partition);
                entity.StaticDatum.Add(newData);
                newData.OrderKey = XmlHelper.GetAttributeValue(n, "orderkey", newData.OrderKey);
                newData.Value = XmlHelper.GetAttributeValue(n, "value", newData.Value);
                newData.ColumnKey = XmlHelper.GetAttributeValue(n, "columnkey", newData.ColumnKey);
            }

        }

        private static void LoadEntityComposites(string folder, Entity entity)
        {
            XmlDocument document = null;
            var fileName = Path.Combine(folder, entity.Name + ".composites.xml");
            if (!File.Exists(fileName)) return;
            try
            {
                document = new XmlDocument();
                document.Load(fileName);
            }
            catch (Exception ex)
            {
                //Do Nothing
                MessageBox.Show("The file '" + fileName + "' is not valid and could not be loaded!", "Load Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            foreach (XmlNode n in document.DocumentElement)
            {
                var newComposite = new Composite(entity.Partition);
                entity.Composites.Add(newComposite);

                newComposite.Summary = XmlHelper.GetNodeValue(document.DocumentElement, "summary", newComposite.Summary);
                newComposite.CodeFacade = XmlHelper.GetAttributeValue(n, "codefacade", newComposite.CodeFacade);
                newComposite.IsGenerated = XmlHelper.GetAttributeValue(n, "isgenerated", newComposite.IsGenerated);
                newComposite.Name = XmlHelper.GetAttributeValue(n, "name", newComposite.Name);

                var columnsNode = n.SelectSingleNode("columnset");
                if (columnsNode != null)
                {
                    foreach (XmlNode m in columnsNode.ChildNodes)
                    {
                        var newField = new CompositeField(entity.Partition);
                        newComposite.Fields.Add(newField);
                        newField.FieldId = XmlHelper.GetAttributeValue(m, "fieldid", newField.FieldId);
                    }
                }

            }

        }

        private static void LoadModules(string folder, Entity item)
        {
            XmlDocument document = null;
            var fileName = Path.Combine(folder, item.Name + ".modules.xml");
            if (!File.Exists(fileName)) return;
            try
            {
                document = new XmlDocument();
                document.Load(fileName);
            }
            catch (Exception ex)
            {
                //Do Nothing
                MessageBox.Show("The file '" + fileName + "' is not valid and could not be loaded!", "Load Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                foreach (XmlNode n in document.DocumentElement)
                {
                    var sourceId = XmlHelper.GetAttributeValue(n, "source", Guid.Empty);
                    var moduleId = XmlHelper.GetAttributeValue(n, "module", Guid.Empty);

                    var field = item.Fields.FirstOrDefault(x => x.Id == sourceId);
                    var relation = item.RelationshipList.FirstOrDefault(x => x.InternalId == sourceId);
                    var module = item.nHydrateModel.Modules.FirstOrDefault(x => x.Id == moduleId);
                    var index = item.IndexList.FirstOrDefault(x => x.Id == sourceId);

                    //If field...
                    if (module != null && field != null)
                    {
                        if (!item.Modules.Contains(module))
                            item.Modules.Add(module);
                        if (!field.Modules.Contains(module))
                            field.Modules.Add(module);
                    }

                    //If relation...
                    if (module != null && relation != null)
                    {
                        if (item.nHydrateModel.RelationModules.Count(x => x.RelationID == relation.Id && x.ModuleId == module.Id) == 0)
                        {
                            var rm = new RelationModule(item.Partition) { RelationID = relation.Id, ModuleId = module.Id };
                            item.nHydrateModel.RelationModules.Add(rm);
                            rm.IsEnforced = XmlHelper.GetAttributeValue(n, "isenforced", relation.IsEnforced);
                        }
                    }

                    //If index...
                    if (module != null && index != null)
                    {
                        if (item.nHydrateModel.IndexModules.Count(x => x.IndexID == index.Id && x.ModuleId == module.Id) == 0)
                        {
                            var rm = new IndexModule(item.Partition) { IndexID = index.Id, ModuleId = module.Id };
                            item.nHydrateModel.IndexModules.Add(rm);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private static void LoadModules(string folder, View item)
        {
            XmlDocument document = null;
            var fileName = Path.Combine(folder, item.Name + ".modules.xml");
            if (!File.Exists(fileName)) return;
            try
            {
                document = new XmlDocument();
                document.Load(fileName);
            }
            catch (Exception ex)
            {
                //Do Nothing
                MessageBox.Show("The file '" + fileName + "' is not valid and could not be loaded!", "Load Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            foreach (XmlNode n in document.DocumentElement)
            {
                var moduleId = XmlHelper.GetAttributeValue(n, "module", Guid.Empty);
                var module = item.nHydrateModel.Modules.FirstOrDefault(x => x.Id == moduleId);
                if (module != null)
                    item.Modules.Add(module);
            }

        }

        private static void LoadModules(string folder, StoredProcedure item)
        {
            XmlDocument document = null;
            var fileName = Path.Combine(folder, item.Name + ".modules.xml");
            if (!File.Exists(fileName)) return;
            try
            {
                document = new XmlDocument();
                document.Load(fileName);
            }
            catch (Exception ex)
            {
                //Do Nothing
                MessageBox.Show("The file '" + fileName + "' is not valid and could not be loaded!", "Load Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            foreach (XmlNode n in document.DocumentElement)
            {
                var moduleId = XmlHelper.GetAttributeValue(n, "module", Guid.Empty);
                var module = item.nHydrateModel.Modules.FirstOrDefault(x => x.Id == moduleId);
                if (module != null)
                    item.Modules.Add(module);
            }

        }

        private static void LoadModules(string folder, Function item)
        {
            XmlDocument document = null;
            var fileName = Path.Combine(folder, item.Name + ".modules.xml");
            if (!File.Exists(fileName)) return;
            try
            {
                document = new XmlDocument();
                document.Load(fileName);
            }
            catch (Exception ex)
            {
                //Do Nothing
                MessageBox.Show("The file '" + fileName + "' is not valid and could not be loaded!", "Load Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            foreach (XmlNode n in document.DocumentElement)
            {
                var moduleId = XmlHelper.GetAttributeValue(n, "module", Guid.Empty);
                var module = item.nHydrateModel.Modules.FirstOrDefault(x => x.Id == moduleId);
                if (module != null)
                    item.Modules.Add(module);
            }

        }

        private static void LoadFromDisk(IEnumerable<StoredProcedure> list, nHydrateModel model, string rootFolder, Microsoft.VisualStudio.Modeling.Store store)
        {
            var folder = Path.Combine(rootFolder, FOLDER_SP);
            if (!Directory.Exists(folder)) return;

            #region Load other parameter/field information
            var fList = Directory.GetFiles(folder, "*.configuration.xml");
            foreach (var f in fList)
            {
                var document = new XmlDocument();
                try
                {
                    document.Load(f);
                }
                catch (Exception ex)
                {
                    //Do Nothing
                    MessageBox.Show("The file '" + f + "' is not valid and could not be loaded!", "Load Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var fi = new FileInfo(f);
                var name = fi.Name.Substring(0, fi.Name.Length - ".configuration.xml".Length).ToLower();
                var itemID = XmlHelper.GetAttributeValue(document.DocumentElement, "id", Guid.Empty);
                var item = list.FirstOrDefault(x => x.Id == itemID);
                if (item == null)
                {
                    item = new StoredProcedure(model.Partition, new PropertyAssignment[] { new PropertyAssignment(ElementFactory.IdPropertyAssignment, XmlHelper.GetAttributeValue(document.DocumentElement, "id", Guid.NewGuid())) });
                    model.StoredProcedures.Add(item);
                }

                System.Windows.Forms.Application.DoEvents();

                item.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(FieldParameter_PropertyChanged);
                item.Name = XmlHelper.GetAttributeValue(document.DocumentElement, "name", item.Name);
                item.PrecedenceOrder = XmlHelper.GetAttributeValue(document.DocumentElement, "precedenceorder", item.PrecedenceOrder);
                item.IsGenerated = XmlHelper.GetAttributeValue(document.DocumentElement, "isgenerated", item.IsGenerated);
                item.CodeFacade = XmlHelper.GetAttributeValue(document.DocumentElement, "codefacade", item.CodeFacade);
                item.Schema = XmlHelper.GetAttributeValue(document.DocumentElement, "schema", item.Schema);
                item.DatabaseObjectName = XmlHelper.GetAttributeValue(document.DocumentElement, "databaseobjectname", item.DatabaseObjectName);
                item.IsExisting = XmlHelper.GetAttributeValue(document.DocumentElement, "isexisting", item.IsExisting);
                item.GeneratesDoubleDerived = XmlHelper.GetAttributeValue(document.DocumentElement, "generatesdoublederived", item.GeneratesDoubleDerived);
                item.Summary = XmlHelper.GetNodeValue(document.DocumentElement, "summary", item.Summary);
                item.PropertyChanged -= new System.ComponentModel.PropertyChangedEventHandler(FieldParameter_PropertyChanged);

                //Fields
                var fieldsNodes = document.DocumentElement.SelectSingleNode("//fieldset");
                if (fieldsNodes != null)
                {
                    var nameList = new List<string>();
                    foreach (XmlNode n in fieldsNodes.ChildNodes)
                    {
                        var subItemID = XmlHelper.GetAttributeValue(n, "id", Guid.Empty);
                        var field = item.Fields.FirstOrDefault(x => x.Id == subItemID);
                        if (field == null)
                        {
                            field = new StoredProcedureField(item.Partition, new PropertyAssignment[] { new PropertyAssignment(ElementFactory.IdPropertyAssignment, XmlHelper.GetAttributeValue(n, "id", Guid.NewGuid())) });
                            item.Fields.Add(field);
                        }

                        field.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(FieldParameter_PropertyChanged);
                        field.Name = XmlHelper.GetAttributeValue(n, "name", field.Name);
                        field.CodeFacade = XmlHelper.GetAttributeValue(n, "codefacade", field.CodeFacade);
                        nameList.Add(field.Name.ToLower());
                        field.Nullable = XmlHelper.GetAttributeValue(n, "nullable", field.Nullable);
                        var dtv = XmlHelper.GetAttributeValue(n, "datatype", field.DataType.ToString());
                        DataTypeConstants dt;
                        if (Enum.TryParse<DataTypeConstants>(dtv, true, out dt))
                            field.DataType = dt;
                        field.Default = XmlHelper.GetAttributeValue(n, "default", field.Default);
                        field.IsGenerated = XmlHelper.GetAttributeValue(n, "isgenerated", field.IsGenerated);
                        field.Length = XmlHelper.GetAttributeValue(n, "length", field.Length);
                        field.Scale = XmlHelper.GetAttributeValue(n, "scale", field.Scale);
                        field.Summary = XmlHelper.GetNodeValue(n, "summary", field.Summary);
                        field.PropertyChanged -= new System.ComponentModel.PropertyChangedEventHandler(FieldParameter_PropertyChanged);
                    }
                    if (item.Fields.Remove(x => !nameList.Contains(x.Name.ToLower())) > 0)
                        item.nHydrateModel.IsDirty = true;
                }

                //Parameters
                var parametersNodes = document.DocumentElement.SelectSingleNode("//parameterset");
                if (parametersNodes != null)
                {
                    var nameList = new List<string>();
                    foreach (XmlNode n in parametersNodes.ChildNodes)
                    {
                        var subItemID = XmlHelper.GetAttributeValue(n, "id", Guid.Empty);
                        var parameter = item.Parameters.FirstOrDefault(x => x.Id == subItemID);
                        if (parameter == null)
                        {
                            parameter = new StoredProcedureParameter(item.Partition, new PropertyAssignment[] { new PropertyAssignment(ElementFactory.IdPropertyAssignment, XmlHelper.GetAttributeValue(n, "id", Guid.NewGuid())) });
                            item.Parameters.Add(parameter);
                        }

                        parameter.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(FieldParameter_PropertyChanged);
                        parameter.Name = XmlHelper.GetAttributeValue(n, "name", parameter.Name);
                        parameter.CodeFacade = XmlHelper.GetAttributeValue(n, "codefacade", parameter.CodeFacade);
                        parameter.IsOutputParameter = XmlHelper.GetAttributeValue(n, "isoutput", parameter.IsOutputParameter);
                        nameList.Add(parameter.Name.ToLower());
                        parameter.Nullable = XmlHelper.GetAttributeValue(n, "nullable", parameter.Nullable);
                        var dtv = XmlHelper.GetAttributeValue(n, "datatype", parameter.DataType.ToString());
                        DataTypeConstants dt;
                        if (Enum.TryParse<DataTypeConstants>(dtv, true, out dt))
                            parameter.DataType = dt;
                        parameter.Default = XmlHelper.GetAttributeValue(n, "default", parameter.Default);
                        parameter.IsGenerated = XmlHelper.GetAttributeValue(n, "isgenerated", parameter.IsGenerated);
                        parameter.Length = XmlHelper.GetAttributeValue(n, "length", parameter.Length);
                        parameter.Scale = XmlHelper.GetAttributeValue(n, "scale", parameter.Scale);
                        parameter.Summary = XmlHelper.GetNodeValue(n, "summary", parameter.Summary);
                        parameter.PropertyChanged -= new System.ComponentModel.PropertyChangedEventHandler(FieldParameter_PropertyChanged);
                    }
                    if (item.Parameters.Remove(x => !nameList.Contains(x.Name.ToLower())) > 0)
                        item.nHydrateModel.IsDirty = true;
                }

                LoadModules(folder, item);

            }
            #endregion

            #region Load SQL
            fList = Directory.GetFiles(folder, "*.sql");
            foreach (var f in fList)
            {
                var fi = new FileInfo(f);
                if (fi.Name.ToLower().EndsWith(".sql"))
                {
                    var name = fi.Name.Substring(0, fi.Name.Length - ".sql".Length).ToLower();
                    var item = list.FirstOrDefault(x => x.Name.ToLower() == name);
                    if (item != null)
                    {
                        item.SQL = File.ReadAllText(f);
                        System.Windows.Forms.Application.DoEvents();
                    }
                }
            }
            #endregion

        }

        private static void LoadFromDisk(IEnumerable<View> list, nHydrateModel model, string rootFolder, Microsoft.VisualStudio.Modeling.Store store)
        {
            var folder = Path.Combine(rootFolder, FOLDER_VW);
            if (!Directory.Exists(folder)) return;

            #region Load other parameter/field information
            var fList = Directory.GetFiles(folder, "*.configuration.xml");
            foreach (var f in fList)
            {
                var document = new XmlDocument();
                try
                {
                    document.Load(f);
                }
                catch (Exception ex)
                {
                    //Do Nothing
                    MessageBox.Show("The file '" + f + "' is not valid and could not be loaded!", "Load Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var fi = new FileInfo(f);
                var name = fi.Name.Substring(0, fi.Name.Length - ".configuration.xml".Length).ToLower();
                var itemID = XmlHelper.GetAttributeValue(document.DocumentElement, "id", Guid.Empty);
                var item = list.FirstOrDefault(x => x.Id == itemID);
                if (item == null)
                {
                    item = new View(model.Partition, new PropertyAssignment[] { new PropertyAssignment(ElementFactory.IdPropertyAssignment, XmlHelper.GetAttributeValue(document.DocumentElement, "id", Guid.NewGuid())) });
                    model.Views.Add(item);
                }

                System.Windows.Forms.Application.DoEvents();

                item.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(FieldParameter_PropertyChanged);
                item.Name = XmlHelper.GetAttributeValue(document.DocumentElement, "name", item.Name);
                item.PrecedenceOrder = XmlHelper.GetAttributeValue(document.DocumentElement, "precedenceorder", item.PrecedenceOrder);
                item.IsGenerated = XmlHelper.GetAttributeValue(document.DocumentElement, "isgenerated", item.IsGenerated);
                item.CodeFacade = XmlHelper.GetAttributeValue(document.DocumentElement, "codefacade", item.CodeFacade);
                item.Schema = XmlHelper.GetAttributeValue(document.DocumentElement, "schema", item.Schema);
                item.GeneratesDoubleDerived = XmlHelper.GetAttributeValue(document.DocumentElement, "generatesdoublederived", item.GeneratesDoubleDerived);
                item.Summary = XmlHelper.GetNodeValue(document.DocumentElement, "summary", item.Summary);
                item.PropertyChanged -= new System.ComponentModel.PropertyChangedEventHandler(FieldParameter_PropertyChanged);

                //Fields
                var fieldsNodes = document.DocumentElement.SelectSingleNode("//fieldset");
                if (fieldsNodes != null)
                {
                    var nameList = new List<string>();
                    foreach (XmlNode n in fieldsNodes.ChildNodes)
                    {
                        var subItemID = XmlHelper.GetAttributeValue(n, "id", Guid.Empty);
                        var field = item.Fields.FirstOrDefault(x => x.Id == subItemID);
                        if (field == null)
                        {
                            field = new ViewField(item.Partition, new PropertyAssignment[] { new PropertyAssignment(ElementFactory.IdPropertyAssignment, XmlHelper.GetAttributeValue(n, "id", Guid.NewGuid())) });
                            item.Fields.Add(field);
                        }

                        field.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(FieldParameter_PropertyChanged);
                        field.Name = XmlHelper.GetAttributeValue(n, "name", field.Name);
                        field.CodeFacade = XmlHelper.GetAttributeValue(n, "codefacade", field.CodeFacade);
                        nameList.Add(field.Name.ToLower());
                        field.Nullable = XmlHelper.GetAttributeValue(n, "nullable", field.Nullable);
                        var dtv = XmlHelper.GetAttributeValue(n, "datatype", field.DataType.ToString());
                        DataTypeConstants dt;
                        if (Enum.TryParse<DataTypeConstants>(dtv, true, out dt))
                            field.DataType = dt;
                        field.Default = XmlHelper.GetAttributeValue(n, "default", field.Default);
                        field.IsGenerated = XmlHelper.GetAttributeValue(n, "isgenerated", field.IsGenerated);
                        field.Length = XmlHelper.GetAttributeValue(n, "length", field.Length);
                        field.Scale = XmlHelper.GetAttributeValue(n, "scale", field.Scale);
                        field.Summary = XmlHelper.GetNodeValue(n, "summary", field.Summary);
                        field.PropertyChanged -= new System.ComponentModel.PropertyChangedEventHandler(FieldParameter_PropertyChanged);
                    }
                    if (item.Fields.Remove(x => !nameList.Contains(x.Name.ToLower())) > 0)
                        item.nHydrateModel.IsDirty = true;
                }

                LoadModules(folder, item);
            }
            #endregion

            #region Load SQL
            fList = Directory.GetFiles(folder, "*.sql");
            foreach (var f in fList)
            {
                var fi = new FileInfo(f);
                if (fi.Name.ToLower().EndsWith(".sql"))
                {
                    var name = fi.Name.Substring(0, fi.Name.Length - 4).ToLower();
                    var item = list.FirstOrDefault(x => x.Name.ToLower() == name);
                    if (item != null)
                    {
                        item.SQL = File.ReadAllText(f);
                        System.Windows.Forms.Application.DoEvents();
                    }
                }
            }
            #endregion

        }

        private static void LoadFromDisk(IEnumerable<Function> list, nHydrateModel model, string rootFolder, Microsoft.VisualStudio.Modeling.Store store)
        {
            var folder = Path.Combine(rootFolder, FOLDER_FC);
            if (!Directory.Exists(folder)) return;

            #region Load other parameter/field information
            var fList = Directory.GetFiles(folder, "*.configuration.xml");
            foreach (var f in fList)
            {
                var document = new XmlDocument();
                try
                {
                    document.Load(f);
                }
                catch (Exception ex)
                {
                    //Do Nothing
                    MessageBox.Show("The file '" + f + "' is not valid and could not be loaded!", "Load Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var fi = new FileInfo(f);
                var name = fi.Name.Substring(0, fi.Name.Length - ".configuration.xml".Length).ToLower();
                var itemID = XmlHelper.GetAttributeValue(document.DocumentElement, "id", Guid.Empty);
                var item = list.FirstOrDefault(x => x.Id == itemID);
                if (item == null)
                {
                    item = new Function(model.Partition, new PropertyAssignment[] { new PropertyAssignment(ElementFactory.IdPropertyAssignment, XmlHelper.GetAttributeValue(document.DocumentElement, "id", Guid.NewGuid())) });
                    model.Functions.Add(item);
                }

                System.Windows.Forms.Application.DoEvents();

                item.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(FieldParameter_PropertyChanged);
                item.Name = XmlHelper.GetAttributeValue(document.DocumentElement, "name", item.Name);
                item.PrecedenceOrder = XmlHelper.GetAttributeValue(document.DocumentElement, "precedenceorder", item.PrecedenceOrder);
                item.IsGenerated = XmlHelper.GetAttributeValue(document.DocumentElement, "isgenerated", item.IsGenerated);
                item.CodeFacade = XmlHelper.GetAttributeValue(document.DocumentElement, "codefacade", item.CodeFacade);
                item.Schema = XmlHelper.GetAttributeValue(document.DocumentElement, "schema", item.Schema);
                item.IsTable = XmlHelper.GetAttributeValue(document.DocumentElement, "istable", item.IsTable);
                item.ReturnVariable = XmlHelper.GetAttributeValue(document.DocumentElement, "returnvariable", item.ReturnVariable);
                item.Summary = XmlHelper.GetNodeValue(document.DocumentElement, "summary", item.Summary);
                item.PropertyChanged -= new System.ComponentModel.PropertyChangedEventHandler(FieldParameter_PropertyChanged);

                //Fields
                var fieldsNodes = document.DocumentElement.SelectSingleNode("//fieldset");
                if (fieldsNodes != null)
                {
                    var nameList = new List<string>();
                    foreach (XmlNode n in fieldsNodes.ChildNodes)
                    {
                        var subItemID = XmlHelper.GetAttributeValue(n, "id", Guid.Empty);
                        var field = item.Fields.FirstOrDefault(x => x.Id == subItemID);
                        if (field == null)
                        {
                            field = new FunctionField(item.Partition, new PropertyAssignment[] { new PropertyAssignment(ElementFactory.IdPropertyAssignment, XmlHelper.GetAttributeValue(n, "id", Guid.NewGuid())) });
                            item.Fields.Add(field);
                        }

                        field.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(FieldParameter_PropertyChanged);
                        field.Name = XmlHelper.GetAttributeValue(n, "name", field.Name);
                        field.CodeFacade = XmlHelper.GetAttributeValue(n, "codefacade", field.CodeFacade);
                        nameList.Add(field.Name.ToLower());
                        field.Nullable = XmlHelper.GetAttributeValue(n, "nullable", field.Nullable);
                        var dtv = XmlHelper.GetAttributeValue(n, "datatype", field.DataType.ToString());
                        DataTypeConstants dt;
                        if (Enum.TryParse<DataTypeConstants>(dtv, true, out dt))
                            field.DataType = dt;
                        field.Default = XmlHelper.GetAttributeValue(n, "default", field.Default);
                        field.IsGenerated = XmlHelper.GetAttributeValue(n, "isgenerated", field.IsGenerated);
                        field.Length = XmlHelper.GetAttributeValue(n, "length", field.Length);
                        field.Scale = XmlHelper.GetAttributeValue(n, "scale", field.Scale);
                        field.Summary = XmlHelper.GetNodeValue(n, "summary", field.Summary);
                        field.PropertyChanged -= new System.ComponentModel.PropertyChangedEventHandler(FieldParameter_PropertyChanged);
                    }
                    if (item.Fields.Remove(x => !nameList.Contains(x.Name.ToLower())) > 0)
                        item.nHydrateModel.IsDirty = true;
                }

                //Parameters
                var parametersNodes = document.DocumentElement.SelectSingleNode("//parameterset");
                if (parametersNodes != null)
                {
                    var nameList = new List<string>();
                    foreach (XmlNode n in parametersNodes.ChildNodes)
                    {
                        var subItemID = XmlHelper.GetAttributeValue(n, "id", Guid.Empty);
                        var parameter = item.Parameters.FirstOrDefault(x => x.Id == subItemID);
                        if (parameter == null)
                        {
                            parameter = new FunctionParameter(item.Partition, new PropertyAssignment[] { new PropertyAssignment(ElementFactory.IdPropertyAssignment, XmlHelper.GetAttributeValue(n, "id", Guid.NewGuid())) });
                            item.Parameters.Add(parameter);
                        }

                        parameter.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(FieldParameter_PropertyChanged);
                        parameter.CodeFacade = XmlHelper.GetAttributeValue(n, "codefacade", parameter.CodeFacade);
                        parameter.Name = XmlHelper.GetAttributeValue(n, "name", parameter.Name);
                        nameList.Add(parameter.Name.ToLower());
                        parameter.Nullable = XmlHelper.GetAttributeValue(n, "nullable", parameter.Nullable);
                        var dtv = XmlHelper.GetAttributeValue(n, "datatype", parameter.DataType.ToString());
                        DataTypeConstants dt;
                        if (Enum.TryParse<DataTypeConstants>(dtv, true, out dt))
                            parameter.DataType = dt;
                        parameter.Default = XmlHelper.GetAttributeValue(n, "default", parameter.Default);
                        parameter.IsGenerated = XmlHelper.GetAttributeValue(n, "isgenerated", parameter.IsGenerated);
                        parameter.Length = XmlHelper.GetAttributeValue(n, "length", parameter.Length);
                        parameter.Scale = XmlHelper.GetAttributeValue(n, "scale", parameter.Scale);
                        parameter.Summary = XmlHelper.GetNodeValue(n, "summary", parameter.Summary);
                        parameter.PropertyChanged -= new System.ComponentModel.PropertyChangedEventHandler(FieldParameter_PropertyChanged);
                    }
                    if (item.Parameters.Remove(x => !nameList.Contains(x.Name.ToLower())) > 0)
                        item.nHydrateModel.IsDirty = true;
                }

                LoadModules(folder, item);
            }
            #endregion

            #region Load SQL
            fList = Directory.GetFiles(folder, "*.sql");
            foreach (var f in fList)
            {
                var fi = new FileInfo(f);
                if (fi.Name.ToLower().EndsWith(".sql"))
                {
                    var name = fi.Name.Substring(0, fi.Name.Length - 4).ToLower();
                    var item = list.FirstOrDefault(x => x.Name.ToLower() == name);
                    if (item != null)
                    {
                        item.SQL = File.ReadAllText(f);
                        System.Windows.Forms.Application.DoEvents();
                    }
                }
            }
            #endregion

        }

        private static void LoadFromDisk(IEnumerable<ModelMetadata> list, nHydrateModel model, string rootFolder, Microsoft.VisualStudio.Modeling.Store store)
        {
            var folder = rootFolder;
            if (!Directory.Exists(folder)) return;

            #region Load other parameter/field information
            var fileName = Path.Combine(folder, "metadata.configuration.xml");
            if (!File.Exists(fileName)) return;
            XmlDocument document = null;
            try
            {
                document = new XmlDocument();
                document.Load(fileName);
            }
            catch (Exception ex)
            {
                //Do Nothing
                MessageBox.Show("The file '" + fileName + "' is not valid and could not be loaded!", "Load Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            foreach (XmlNode n in document.DocumentElement.ChildNodes)
            {
                var item = new ModelMetadata(model.Partition);
                model.ModelMetadata.Add(item);
                item.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(FieldParameter_PropertyChanged);
                item.Key = XmlHelper.GetAttributeValue(n, "key", item.Key);
                item.Value = XmlHelper.GetAttributeValue(n, "value", item.Value);
                item.PropertyChanged -= new System.ComponentModel.PropertyChangedEventHandler(FieldParameter_PropertyChanged);
            }
            #endregion

        }

        private static void LoadFromDisk(IEnumerable<Module> list, nHydrateModel model, string rootFolder, Microsoft.VisualStudio.Modeling.Store store)
        {
            var folder = rootFolder;
            if (!Directory.Exists(folder)) return;

            #region Load other parameter/field information
            var fileName = Path.Combine(folder, "modules.configuration.xml");
            if (!File.Exists(fileName)) return;
            XmlDocument document = null;
            try
            {
                document = new XmlDocument();
                document.Load(fileName);
            }
            catch (Exception ex)
            {
                //Do Nothing
                MessageBox.Show("The file '" + fileName + "' is not valid and could not be loaded!", "Load Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            foreach (XmlNode n in document.DocumentElement.ChildNodes)
            {
                var id = XmlHelper.GetAttributeValue(n, "id", Guid.NewGuid());
                var item = model.Modules.FirstOrDefault(x => x.Id == id);
                if (item == null)
                {
                    item = new Module(model.Partition, new PropertyAssignment[] { new PropertyAssignment(ElementFactory.IdPropertyAssignment, id) });
                    model.Modules.Add(item);
                }

                item.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(FieldParameter_PropertyChanged);
                item.Name = XmlHelper.GetAttributeValue(n, "name", item.Name);
                item.Summary = XmlHelper.GetNodeValue(n, "summary", item.Summary);
                item.PropertyChanged -= new System.ComponentModel.PropertyChangedEventHandler(FieldParameter_PropertyChanged);

                //Rules
                var rulesNodes = n.SelectSingleNode("ruleset");
                if (rulesNodes != null)
                {
                    var nameList = new List<string>();
                    foreach (XmlNode m in rulesNodes.ChildNodes)
                    {
                        var rule = new ModuleRule(item.Partition);
                        item.ModuleRules.Add(rule);
                        rule.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(FieldParameter_PropertyChanged);
                        rule.Name = XmlHelper.GetAttributeValue(m, "name", rule.Name);
                        rule.Status = (ModuleRuleStatusTypeConstants)int.Parse(XmlHelper.GetAttributeValue(m, "status", rule.Status.ToString("d")));
                        rule.DependentModule = XmlHelper.GetAttributeValue(m, "dependentmodule", rule.DependentModule);
                        rule.Inclusion = XmlHelper.GetAttributeValue(m, "inclusion", rule.Inclusion);
                        rule.Enforced = XmlHelper.GetAttributeValue(m, "enforced", rule.Enforced);
                        rule.Summary = XmlHelper.GetNodeValue(m, "summary", rule.Summary);
                        rule.PropertyChanged -= new System.ComponentModel.PropertyChangedEventHandler(FieldParameter_PropertyChanged);
                    }
                }

            }
            #endregion

        }

        public static void LoadDiagramFiles(nHydrateModel model, string rootFolder, string modelName, nHydrateDiagram diagram)
        {
            if (!model.ModelToDisk)
                return;

            var fileName = Path.Combine(GetModelFolder(rootFolder, modelName), "diagram.xml");
            if (!File.Exists(fileName)) return;
            using (var transaction = model.Store.TransactionManager.BeginTransaction(Guid.NewGuid().ToString()))
            {
                var document = new XmlDocument();
                var id = Guid.Empty;
                try
                {
                    document.Load(fileName);
                    if (document.DocumentElement == null) throw new Exception("No Root"); //this is thrown away

                    foreach (XmlNode node in document.DocumentElement.ChildNodes)
                    {
                        id = XmlHelper.GetAttributeValue(node, "id", Guid.Empty);
                        var shape = diagram.NestedChildShapes.FirstOrDefault(x => x.ModelElement.Id == id) as Microsoft.VisualStudio.Modeling.Diagrams.NodeShape;
                        if (shape != null)
                            shape.Bounds = Extensions.ConvertRectangleDFromXmlValue(XmlHelper.GetAttributeValue(node, "bounds", string.Empty));
                    }
                }
                catch (Exception ex) { return; }
                transaction.Commit();
            }

        }

        #region Private Helpers

        private static void FieldParameter_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            //if (sender is StoredProcedureField)
            //  (sender as StoredProcedureField).StoredProcedure.nHydrateModel.IsDirty = true;
            //else if (sender is StoredProcedureParameter)
            //  (sender as StoredProcedureParameter).StoredProcedure.nHydrateModel.IsDirty = true;
            //else if (sender is ViewField)
            //  (sender as ViewField).View.nHydrateModel.IsDirty = true;
            //else if (sender is FunctionField)
            //  (sender as FunctionField).Function.nHydrateModel.IsDirty = true;
            //else if (sender is FunctionParameter)
            //  (sender as FunctionParameter).Function.nHydrateModel.IsDirty = true;

            //else if (sender is Entity)
            //  (sender as Entity).nHydrateModel.IsDirty = true;
            //else if (sender is View)
            //  (sender as View).nHydrateModel.IsDirty = true;
            //else if (sender is StoredProcedure)
            //  (sender as StoredProcedure).nHydrateModel.IsDirty = true;
            //else if (sender is Function)
            //  (sender as Function).nHydrateModel.IsDirty = true;
        }

        private static void WriteFileIfNeedBe(string fileName, string contents, List<string> generatedFileList)
        {
            if (fileName.ToLower().EndsWith(".xml"))
            {
                generatedFileList.Add(fileName);
                try
                {
                    //Load formatted original XML
                    var origXML = string.Empty;
                    if (File.Exists(fileName))
                    {
                        var xmlText = File.ReadAllText(fileName);
                        if (!string.IsNullOrEmpty(xmlText))
                        {
                            var documentCheck = new XmlDocument();
                            documentCheck.LoadXml(xmlText);
                            origXML = documentCheck.ToIndentedString();
                        }
                    }

                    //Load formatted new XML
                    var newXML = string.Empty;
                    {
                        var documentCheck = new XmlDocument();
                        documentCheck.LoadXml(contents);
                        newXML = documentCheck.ToIndentedString();
                    }

                    if (origXML == newXML)
                        return;
                    else
                        contents = newXML;
                }
                catch (Exception ex)
                {
                    //If there is an error then process like a non-XML file
                    //Do Nothing
                }
            }
            else
            {
                //Check if this is the same content and if so do nothing
                generatedFileList.Add(fileName);
                if (File.Exists(fileName))
                {
                    var t = File.ReadAllText(fileName);
                    if (t == contents)
                        return;
                }
            }

            File.WriteAllText(fileName, contents);
            System.Windows.Forms.Application.DoEvents();
        }

        private static void WriteReadMeFile(string folder, List<string> generatedFileList)
        {
            var f = Path.Combine(folder, "ReadMe.nHydrate.txt");
            WriteFileIfNeedBe(f, "This is a managed folder of a nHydrate model. You may change '*.configuration.xml' and '*.sql' files in any text editor if desired but do not add or remove files from this folder. This is a distributed model and making changes can break the model load.", generatedFileList);
        }

        private static void RemoveOrphans(string rootFolder, List<string> generatedFiles)
        {
            //Only get these specific folder in case there is version control or some other third-party application running
            //Only touch the files we know about
            var files = new List<string>();
            files.AddRange(Directory.GetFiles(rootFolder, "*.*", SearchOption.TopDirectoryOnly));
            files.AddRange(Directory.GetFiles(Path.Combine(rootFolder, "_Entities"), "*.*", SearchOption.TopDirectoryOnly));
            files.AddRange(Directory.GetFiles(Path.Combine(rootFolder, "_Functions"), "*.*", SearchOption.TopDirectoryOnly));
            files.AddRange(Directory.GetFiles(Path.Combine(rootFolder, "_StoredProcedures"), "*.*", SearchOption.TopDirectoryOnly));
            files.AddRange(Directory.GetFiles(Path.Combine(rootFolder, "_Views"), "*.*", SearchOption.TopDirectoryOnly));
            files.ToList().ForEach(x => x = x.ToLower());
            generatedFiles.ToList().ForEach(x => x = x.ToLower());

            foreach (var f in files)
            {
                var fi = new FileInfo(f);
                if (fi.Name.ToLower().Contains("readme.nhydrate.txt"))
                {
                    //Skip
                }
                else if (generatedFiles.Contains(f))
                {
                    //Skip
                }
                else
                {
                    File.Delete(f);
                }

            }
        }

        #endregion

    }
}
