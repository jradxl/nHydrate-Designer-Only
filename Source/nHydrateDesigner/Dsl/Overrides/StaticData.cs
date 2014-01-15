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
using System;
using System.Collections.Generic;
using System.Linq;

namespace nHydrate2.Dsl
{
    partial class StaticData
    {
    }

    static partial class Extensions
    {
        /// <summary>
        /// Converts a static data list to a list of rows each containing a Dictionary of Column/Value pairs
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static List<Dictionary<Guid, string>> ToRows(this LinkedElementCollection<StaticData> list)
        {
            var retval = new List<Dictionary<Guid, string>>();

            var last = -1;
            Dictionary<Guid, string> row = null;
            foreach (var item in list.OrderBy(x => x.OrderKey))
            {
                if (item.OrderKey != last)
                {
                    last = item.OrderKey;
                    row = new Dictionary<Guid, string>();
                    retval.Add(row);
                }
                row.Add(item.ColumnKey, item.Value);
            }
            return retval;
        }

        public static bool HasDuplicates(this LinkedElementCollection<StaticData> dataList, Entity entity)
        {
            try
            {
                //var id = string.Empty;
                //var name = string.Empty;
                //var description = string.Empty;
                var pk = entity.PrimaryKeyFields.FirstOrDefault();
                if (pk == null) return false;
                var processed = new List<string>();
                foreach (var cellEntry in dataList.Where(x => x.ColumnKey == pk.Id))
                {
                    if (processed.Contains(cellEntry.Value)) return true;
                    processed.Add(cellEntry.Value);
                }
                return false;
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public static bool IsDataValid(this LinkedElementCollection<StaticData> dataList, Entity entity)
        {
            //Some of these are not fool-proof but they are close enough!!
            var retval = true;
            foreach (var data in dataList)
            {
                //var column = entity.Store.ElementDirectory.AllElements.FirstOrDefault(x => x.Id == data.Id) as Field;
                var column = entity.Fields.FirstOrDefault(x => x.Id == data.ColumnKey);
                if (column == null) return true; //No Verification

                long vlong;
                int vint;
                bool vbool;
                decimal vdecimal;
                DateTime vdate;
                short vshort;
                byte vbyte;

                if (column.Nullable && data.Value.ToLower() == "(null)")
                {
                    //Do nothing. This is a nullable field so set to NULL
                }
                else
                {
                    switch (column.DataType)
                    {
                        case DataTypeConstants.BigInt:
                            retval &= long.TryParse(data.Value, out vlong);
                            break;
                        case DataTypeConstants.Binary:
                            retval &= true; //no validation
                            break;
                        case DataTypeConstants.Bit:
                            var v2 = data.Value + string.Empty;
                            if (v2 == "0") v2 = "false";
                            if (v2 == "1") v2 = "true";
                            retval &= bool.TryParse(v2.ToLower(), out vbool);
                            break;
                        case DataTypeConstants.Char:
                            retval &= true;
                            break;
                        case DataTypeConstants.Date:
                            retval &= DateTime.TryParse(data.Value, out vdate);
                            break;
                        case DataTypeConstants.DateTime:
                            retval &= DateTime.TryParse(data.Value, out vdate);
                            break;
                        case DataTypeConstants.DateTime2:
                            retval &= DateTime.TryParse(data.Value, out vdate);
                            break;
                        case DataTypeConstants.DateTimeOffset:
                            retval &= true; //no validation
                            break;
                        case DataTypeConstants.Decimal:
                            retval &= decimal.TryParse(data.Value, out vdecimal);
                            break;
                        case DataTypeConstants.Float:
                            retval &= decimal.TryParse(data.Value, out vdecimal);
                            break;
                        case DataTypeConstants.Image:
                            retval &= true;
                            break;
                        case DataTypeConstants.Int:
                            retval &= int.TryParse(data.Value, out vint);
                            break;
                        case DataTypeConstants.Money:
                            retval &= decimal.TryParse(data.Value, out vdecimal);
                            break;
                        case DataTypeConstants.NChar:
                            retval &= true;
                            break;
                        case DataTypeConstants.NText:
                            retval &= true;
                            break;
                        case DataTypeConstants.NVarChar:
                            retval &= true;
                            break;
                        case DataTypeConstants.Real:
                            retval &= decimal.TryParse(data.Value, out vdecimal);
                            break;
                        case DataTypeConstants.SmallDateTime:
                            retval &= DateTime.TryParse(data.Value, out vdate);
                            break;
                        case DataTypeConstants.SmallInt:
                            retval &= short.TryParse(data.Value, out vshort);
                            break;
                        case DataTypeConstants.SmallMoney:
                            retval &= decimal.TryParse(data.Value, out vdecimal);
                            break;
                        case DataTypeConstants.Structured:
                            retval &= true;
                            break;
                        case DataTypeConstants.Text:
                            retval &= true;
                            break;
                        case DataTypeConstants.Time:
                            retval &= DateTime.TryParse(data.Value, out vdate);
                            break;
                        case DataTypeConstants.Timestamp:
                            retval &= true; //no validation
                            break;
                        case DataTypeConstants.TinyInt:
                            retval &= byte.TryParse(data.Value, out vbyte);
                            break;
                        case DataTypeConstants.Udt:
                            retval &= true; //no validation
                            break;
                        case DataTypeConstants.UniqueIdentifier:
                            try { var g = new Guid(data.Value); retval &= true; }
                            catch { retval &= false; }
                            break;
                        case DataTypeConstants.VarBinary:
                            retval &= true; //no validation
                            break;
                        case DataTypeConstants.VarChar:
                            retval &= true;
                            break;
                        case DataTypeConstants.Variant:
                            retval &= true; //no validation
                            break;
                        case DataTypeConstants.Xml:
                            retval &= true;
                            break;
                        default:
                            retval &= true;
                            break;
                    }
                }
            }
            return retval;
        }

    }

}
