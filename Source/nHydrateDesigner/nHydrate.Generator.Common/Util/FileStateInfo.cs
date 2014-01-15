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
namespace nHydrate.Generator.Common.Util
{
  public class FileStateInfo
  {
    #region Class Members

    private nHydrate.Generator.Common.Util.EnvDTEHelper.FileStateConstants _fileState = EnvDTEHelper.FileStateConstants.Success;
    private string _fileName = string.Empty;

    #endregion

    #region Constructor

    public FileStateInfo()
    {
    }

    public FileStateInfo(nHydrate.Generator.Common.Util.EnvDTEHelper.FileStateConstants fileState, string fileName)
    {
      _fileName = fileName;
      _fileState = fileState;
    }

    #endregion

    #region Property Impelementations

    public nHydrate.Generator.Common.Util.EnvDTEHelper.FileStateConstants FileState
    {
      get { return _fileState; }
      set { _fileState = value; }
    }

    public string FileName
    {
      get { return _fileName; }
      set { _fileName = value; }
    }

    #endregion

  }
}
