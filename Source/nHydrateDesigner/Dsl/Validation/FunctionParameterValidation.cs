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

namespace nHydrate2.Dsl
{
    [ValidationState(ValidationState.Enabled)]
    partial class FunctionParameter
    {
        #region Dirty
        [System.ComponentModel.Browsable(false)]
        internal bool IsDirty
        {
            get { return _isDirty; }
            set { _isDirty = value; }
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

                #region Check valid name
                if (!ValidationHelper.ValidDatabaseIdenitifer(this.DatabaseName))
                    context.LogError(string.Format(ValidationHelper.ErrorTextInvalidIdentifierFuncParam, this.Name, this.Function.Name), string.Empty, this);
                else if (!ValidationHelper.ValidCodeIdentifier(this.PascalName))
                    context.LogError(string.Format(ValidationHelper.ErrorTextInvalidIdentifierFuncParam, this.Name, this.Function.Name), string.Empty, this);
                #endregion

                #region Validate max lengths

                var validatedLength = this.DataType.ValidateDataTypeMax(this.Length);
                if (validatedLength != this.Length)
                {
                    context.LogError(string.Format(ValidationHelper.ErrorTextColumnMaxLengthViolation, this.Function.Name + "." + this.Name, validatedLength, this.DataType.ToString()), string.Empty, this);
                }

                #endregion

                #region MySQL
                if ((this.Function.nHydrateModel.SupportedPlatforms & DatabasePlatformConstants.MySQL) == DatabasePlatformConstants.MySQL)
                {
                    if (!ValidationHelper.MySQLSupportedDatatype(this.DataType))
                    {
                        context.LogError(string.Format(ValidationHelper.ErrorTextMySQLDatatypeParameter, this.DataType.ToString(), this.Function.Name + "." + this.Name), string.Empty, this);
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
                nHydrate2.Dsl.Custom.DebugHelper.StopTimer(timer, "Stored Procedure Parameter Validate - Main");
            }

        }
    }
}

