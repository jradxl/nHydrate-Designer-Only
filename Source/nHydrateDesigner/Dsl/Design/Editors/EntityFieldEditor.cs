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
using System;
using System.ComponentModel;
using System.Linq;

namespace nHydrate2.Dsl.Design.Editors
{
    internal class EntityFieldEditor : System.Drawing.Design.UITypeEditor
    {
        public override System.Drawing.Design.UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return System.Drawing.Design.UITypeEditorEditStyle.DropDown;
        }

        private System.Windows.Forms.Design.IWindowsFormsEditorService edSvc = null;
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            var retval = Guid.Empty;
            try
            {
                edSvc = (System.Windows.Forms.Design.IWindowsFormsEditorService)provider.GetService(typeof(System.Windows.Forms.Design.IWindowsFormsEditorService));

                var indexColumn = context.Instance as IndexColumn;
                var field = indexColumn.Index.Entity.Fields.FirstOrDefault(x => x.Id == indexColumn.FieldID);
                var fieldList = indexColumn.Index.Entity.Fields.OrderBy(x => x.Name).ToList();

                //Create the list box
                var newBox = new System.Windows.Forms.ListBox();
                newBox.Click += new EventHandler(newBox_Click);
                newBox.IntegralHeight = false;

                newBox.Items.AddRange(fieldList.Select(x => x.Name).ToArray());
                if (field != null)
                    newBox.SelectedIndex = fieldList.IndexOf(field);

                edSvc.DropDownControl(newBox);
                if ((indexColumn != null) && (newBox.SelectedIndex != -1))
                    retval = fieldList[newBox.SelectedIndex].Id;

            }
            catch (Exception ex) { }
            return retval;
        }

        private void newBox_Click(object sender, System.EventArgs e)
        {
            if (edSvc != null)
                edSvc.CloseDropDown();
        }

    }
}

