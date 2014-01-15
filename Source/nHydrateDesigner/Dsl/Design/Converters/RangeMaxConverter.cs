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

namespace nHydrate2.Dsl.Design.Converters
{
    internal class RangeMaxConverter : TypeConverter
    {
        public RangeMaxConverter()
        {
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            var column = context.Instance as nHydrate2.Dsl.Field;
            if (destinationType == typeof(string)) return true;
            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            var column = context.Instance as nHydrate2.Dsl.Field;
            try
            {
                if (destinationType == typeof(string))
                {
                    var retval = string.Empty;
                    if (column != null && column.IsNumericType())
                    {
                        retval = column.Max.ToString();
                        if (double.IsNaN(column.Max))
                            retval = string.Empty;
                        else if (column.IsCalculated)
                            retval = "undefined";
                    }
                    return retval;
                }
            }
            catch (Exception ex) { }
            return null;
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            var column = context.Instance as nHydrate2.Dsl.Field;
            if (sourceType == typeof(string))
                return true;
            else if (sourceType == typeof(double))
                return true;
            else
                return false;
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            var column = context.Instance as nHydrate2.Dsl.Field;
            if (value is string)
            {
                if (((string)value).Trim() == string.Empty)
                {
                    return double.NaN;
                }
                else
                {
                    double d;
                    if (double.TryParse((string)value, out d))
                        return d;
                    //else
                    //  throw new Exception("Cannot convert to double!");
                }
                return column.Max;
            }
            return base.ConvertFrom(context, culture, value);
        }

    }
}

