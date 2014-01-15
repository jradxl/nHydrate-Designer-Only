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
using System.Collections.Generic;

namespace nHydrate2.Dsl
{
    [ValidationState(ValidationState.Enabled)]
    partial class Field
    {
        #region Dirty
        [System.ComponentModel.Browsable(false)]
        internal bool IsDirty
        {
            get { return _isDirty; }
            set { _isDirty = value; }
        }
        private bool _isDirty = false;

        protected override void OnPropertyChanged(System.ComponentModel.PropertyChangedEventArgs e)
        {
            this.IsDirty = true;
            base.OnPropertyChanged(e);
        }
        #endregion

        [ValidationMethod(ValidationCategories.Open | ValidationCategories.Save | ValidationCategories.Menu | ValidationCategories.Custom | ValidationCategories.Load)]
        public void Validate(ValidationContext context)
        {
            var timer = nHydrate2.Dsl.Custom.DebugHelper.StartTimer();
            try
            {
                if (!this.IsGenerated) return;
                //if (!this.IsDirty) return;

                #region Check valid name
                if (!ValidationHelper.ValidDatabaseIdenitifer(this.DatabaseName))
                    context.LogError(string.Format(ValidationHelper.ErrorTextInvalidIdentifier, this.Entity.Name + "." + this.Name), string.Empty, this);
                else if (!ValidationHelper.ValidCodeIdentifier(this.PascalName))
                    context.LogError(string.Format(ValidationHelper.ErrorTextInvalidIdentifier, this.Entity.Name + "." + this.Name), string.Empty, this);
                else if (!ValidationHelper.ValidFieldIdentifier(this.PascalName))
                    context.LogError(string.Format(ValidationHelper.ErrorTextInvalidIdentifier, this.Entity.Name + "." + this.Name), string.Empty, this);

                #endregion

                #region Check valid name based on codefacade
                if ((!string.IsNullOrEmpty(this.CodeFacade)) && !ValidationHelper.ValidDatabaseIdenitifer(this.CodeFacade))
                    context.LogError(ValidationHelper.ErrorTextInvalidCodeFacade, string.Empty, this);

                if (this.IsNumericType())
                {
                    if (!double.IsNaN(this.Min) && (!double.IsNaN(this.Max)))
                    {
                        if (this.Min > this.Max)
                            context.LogError(ValidationHelper.ErrorTextMinMaxValueMismatch, string.Empty, this);
                    }
                }
                else //Non-numeric
                {
                    //Neither should be set
                    if (!double.IsNaN(this.Min) || (!double.IsNaN(this.Max)))
                    {
                        context.LogError(ValidationHelper.ErrorTextMinMaxValueInvalidType, string.Empty, this);
                    }
                }
                #endregion

                #region Validate identity field
                if (this.Identity != IdentityTypeConstants.None && !this.DataType.SupportsIdentity())
                {
                    context.LogError(string.Format(ValidationHelper.ErrorTextInvalidIdentityColumn, this.Name), string.Empty, this);
                }
                #endregion

                #region Varchar Max only supported in SQL 2008

                //if (((ModelRoot)this.Root).SQLServerType == SQLServerTypeConstants.SQL2005)
                //{
                //  if (ModelHelper.SupportsMax(this.DataType) && this.Length == 0)
                //  {
                //    context.LogError(string.Format(ValidationHelper.ErrorTextColumnMaxNotSupported, this.Name), string.Empty, this);
                //  }
                //}

                #endregion

                #region Columns cannot be 0 length

                if (!this.DataType.SupportsMax() && this.Length == 0)
                {
                    context.LogError(string.Format(ValidationHelper.ErrorTextColumnLengthNotZero, this.Name), string.Empty, this);
                }

                #endregion

                #region Validate Decimals

                if (this.DataType == DataTypeConstants.Decimal)
                {
                    if (this.Length < 1 || this.Length > 38)
                        context.LogError(string.Format(ValidationHelper.ErrorTextColumnDecimalPrecision, this.Name), string.Empty, this);
                    if (this.Scale < 0 || this.Scale > this.Length)
                        context.LogError(string.Format(ValidationHelper.ErrorTextColumnDecimalScale, this.Name), string.Empty, this);
                }

                #endregion

                #region Validate max lengths

                var validatedLength = this.DataType.ValidateDataTypeMax(this.Length);
                if (validatedLength != this.Length)
                {
                    context.LogError(string.Format(ValidationHelper.ErrorTextColumnMaxLengthViolation, this.Entity.Name + "." + this.Name, validatedLength, this.DataType.ToString()), string.Empty, this);
                }

                #endregion

                #region Verify Datatypes for SQL 2005/2008

                if ((this.Entity.nHydrateModel.SupportedPlatforms & DatabasePlatformConstants.SQLServer) == DatabasePlatformConstants.SQLServer)
                {
                    if (!this.DataType.IsSupportedType(this.Entity.nHydrateModel.SQLServerType))
                    {
                        context.LogError(string.Format(ValidationHelper.ErrorTextDataTypeNotSupported, this.Name), string.Empty, this);
                    }
                }

                #endregion

                #region Computed Column

                if (this.IsCalculated)
                {
                    if (this.Formula.Trim() == "")
                    {
                        context.LogError(string.Format(ValidationHelper.ErrorTextComputeColumnNoFormula, this.Name), string.Empty, this);
                    }

                    if (this.IsPrimaryKey)
                    {
                        context.LogError(string.Format(ValidationHelper.ErrorTextComputeColumnNoPK, this.Name), string.Empty, this);
                    }

                }

                if (!this.IsCalculated && !string.IsNullOrEmpty(this.Formula))
                {
                    context.LogError(string.Format(ValidationHelper.ErrorTextComputeNonColumnHaveFormula, this.Name), string.Empty, this);
                }

                #endregion

                #region Validate Defaults

                if (!string.IsNullOrEmpty(this.Default))
                {
                    if ((this.Entity.nHydrateModel.SupportedPlatforms | DatabasePlatformConstants.SQLServer) == DatabasePlatformConstants.SQLServer
                        && !this.CanHaveDefault())
                    {
                        context.LogError(string.Format(ValidationHelper.ErrorTextColumnCannotHaveDefault, this.Name), string.Empty, this);
                    }
                    else if (!this.IsValidDefault())
                    {
                        context.LogWarning(string.Format(ValidationHelper.ErrorTextColumnInvalidDefault, this.Name), string.Empty, this);
                    }
                }

                #endregion

                #region Check Decimals for common error

                if (this.DataType == DataTypeConstants.Decimal)
                {
                    if (this.Length == 1)
                        context.LogError(string.Format(ValidationHelper.ErrorTextDecimalColumnTooSmall, this.Name, this.Length.ToString()), string.Empty, this);
                }

                #endregion

                #region Verify Metadata

                var metaKeyList = new List<string>();
                foreach (var item in this.FieldMetadata)
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

                #region Identity Columns cannot have defaults

                if (this.Identity != IdentityTypeConstants.None && !string.IsNullOrEmpty(this.Default))
                {
                    context.LogError(string.Format(ValidationHelper.ErrorTextColumnIdentityHasDefault, this.Entity.Name + "." + this.Name), string.Empty, this);
                }

                #endregion

                #region Non-nullable, ReadOnly propeties must have a default (except identities)
                if (this.IsReadOnly && !this.Nullable && (this.Identity != IdentityTypeConstants.Database) && string.IsNullOrEmpty(this.Default))
                {
                    context.LogError(string.Format(ValidationHelper.ErrorTextColumnReadonlyNeedsDefault, this.Entity.Name + "." + this.Name), string.Empty, this);
                }
                #endregion

                #region Verify Entity is in some module
                if (this.Entity.nHydrateModel.UseModules && (this.Modules.Count == 0) && this.IsGenerated)
                {
                    context.LogError(string.Format(ValidationHelper.ErrorTextModuleItemNotInModule, this.Entity.Name + "." + this.Name), string.Empty, this);
                }
                #endregion

                #region MySQL
                if ((this.Entity.nHydrateModel.SupportedPlatforms & DatabasePlatformConstants.MySQL) == DatabasePlatformConstants.MySQL)
                {
                    if (!ValidationHelper.MySQLSupportedDatatype(this.DataType))
                    {
                        context.LogError(string.Format(ValidationHelper.ErrorTextMySQLDatatypeField, this.DataType.ToString(), this.Entity.Name + "." + this.Name), string.Empty, this);
                    }

                    if (this.IsCalculated)
                    {
                        context.LogError(string.Format(ValidationHelper.ErrorTextMySQLCalculated, this.Entity.Name + "." + this.Name), string.Empty, this);
                    }

                    //There is no varchar max
                    if (this.DataType.SupportsMax() && this.Length == 0)
                    {
                        context.LogError(string.Format(ValidationHelper.ErrorTextMySQLNoMaxLength, this.Entity.Name + "." + this.Name), string.Empty, this);
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
                nHydrate2.Dsl.Custom.DebugHelper.StopTimer(timer, "Field Validate - Fields");
            }
        }

    }
}

