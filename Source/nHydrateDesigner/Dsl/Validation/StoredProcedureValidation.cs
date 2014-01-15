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
using System.Linq;

namespace nHydrate2.Dsl
{
    [ValidationState(ValidationState.Enabled)]
    partial class StoredProcedure
    {
        #region Dirty
        [System.ComponentModel.Browsable(false)]
        internal bool IsDirty
        {
            get { return _isDirty || this.Fields.IsDirty() || this.Parameters.IsDirty(); }
            set
            {
                _isDirty = value;
                if (!value)
                {
                    this.Fields.ForEach(x => x.IsDirty = false);
                    this.Parameters.ForEach(x => x.IsDirty = false);
                }
            }
        }
        private bool _isDirty = false;
        #endregion

        [ValidationMethod(ValidationCategories.Open | ValidationCategories.Save | ValidationCategories.Menu | ValidationCategories.Custom | ValidationCategories.Load)]
        public void Validate(ValidationContext context)
        {
            if (!this.IsGenerated) return;

            System.Windows.Forms.Application.DoEvents();
            var timer = nHydrate2.Dsl.Custom.DebugHelper.StartTimer();
            try
            {
                //if (!this.IsDirty) return;

                #region Check valid name

                ////Check valid name
                //if (!ValidationHelper.ValidDatabaseIdenitifer(this.DatabaseName))
                //  context.LogError(string.Format(ValidationHelper.ErrorTextInvalidIdentifier, this.Name), string.Empty, this);
                //else if (!ValidationHelper.ValidCodeIdentifier(this.PascalName))
                //  context.LogError(string.Format(ValidationHelper.ErrorTextInvalidIdentifier, this.Name), string.Empty, this);

                #endregion

                #region Check StoredProcedure SQL

                if (!this.IsExisting && string.IsNullOrEmpty(this.SQL))
                {
                    context.LogError(string.Format(ValidationHelper.ErrorTextSQLRequiredStoredProcedure, this.Name), string.Empty, this);
                }

                #endregion

                #region Check existing database name

                if (this.IsExisting && string.IsNullOrEmpty(this.DatabaseObjectName))
                {
                    context.LogError(string.Format(ValidationHelper.ErrorTextExistingSPNeedsDBName, this.Name), string.Empty, this);
                }

                #endregion

                #region Verify that no column has same name as container

                foreach (var field in this.Fields)
                {
                    if (string.Compare(field.PascalName, this.PascalName, true) == 0)
                    {
                        context.LogError(string.Format(ValidationHelper.ErrorTextTableColumnNameMatch, field.Name, this.Name), string.Empty, this);
                    }
                }

                #endregion

                #region Verify there are columns (fix for EF 4.1 bug)

                if (!this.Fields.Any(x => x.IsGenerated))
                {
                    context.LogError(string.Format(ValidationHelper.ErrorTextStoredProcNoColumns, this.Name), string.Empty, this);
                }

                #endregion

                #region Check Parameters Duplicates

                var paramList = this.Parameters.Select(x => x.Name.ToLower());
                if (paramList.Count() != paramList.Distinct().Count())
                {
                    context.LogError(string.Format(ValidationHelper.ErrorTextDuplicateParameters, this.Name), string.Empty, this);
                }

                #endregion

            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                nHydrate2.Dsl.Custom.DebugHelper.StopTimer(timer, "Stored Procedure Validate - Main");
            }

        }
    }
}