

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace PdfToImage
{
  public class PDFConvert
  {
    private static bool useSimpleAnsiConversion = true;
    private int _iFirstPageToConvert = -1;
    private int _iLastPageToConvert = -1;
    private int _iGraphicsAlphaBit = -1;
    private int _iTextAlphaBit = -1;
    private int _iRenderingThreads = -1;
    private bool _bThrowOnlyException = false;
    private bool _bRedirectIO = false;
    private bool _bForcePageSize = false;
    private bool _didOutputToMultipleFile = false;
    private List<string> _sFontPath = new List<string>();
    private bool _bDisablePlatformFonts = false;
    private bool _bDisableFontMap = false;
    private List<string> _sFontMap = new List<string>();
    private bool _bDisablePrecompiledFonts = false;
    public const string GhostScriptDLLName = "gsdll32.dll";
    private const string GS_OutputFileFormat = "-sOutputFile={0}";
    private const string GS_DeviceFormat = "-sDEVICE={0}";
    private const string GS_FirstParameter = "pdf2img";
    private const string GS_ResolutionXFormat = "-r{0}";
    private const string GS_ResolutionXYFormat = "-r{0}x{1}";
    private const string GS_GraphicsAlphaBits = "-dGraphicsAlphaBits={0}";
    private const string GS_TextAlphaBits = "-dTextAlphaBits={0}";
    private const string GS_FirstPageFormat = "-dFirstPage={0}";
    private const string GS_LastPageFormat = "-dLastPage={0}";
    private const string GS_FitPage = "-dPDFFitPage";
    private const string GS_PageSizeFormat = "-g{0}x{1}";
    private const string GS_DefaultPaperSize = "-sPAPERSIZE={0}";
    private const string GS_JpegQualityFormat = "-dJPEGQ={0}";
    private const string GS_RenderingThreads = "-dNumRenderingThreads={0}";
    private const string GS_Fixed1stParameter = "-dNOPAUSE";
    private const string GS_Fixed2ndParameter = "-dBATCH";
    private const string GS_Fixed3rdParameter = "-dSAFER";
    private const string GS_FixedMedia = "-dFIXEDMEDIA";
    private const string GS_QuiteOperation = "-q";
    private const string GS_StandardOutputDevice = "-";
    private const string GS_MultiplePageCharacter = "%";
    private const string GS_FontPath = "-sFONTPATH={0}";
    private const string GS_NoPlatformFonts = "-dNOPLATFONTS";
    private const string GS_NoFontMap = "-dNOFONTMAP";
    private const string GS_FontMap = "-sFONTMAP={0}";
    private const string GS_SubstitutionFont = "-sSUBSTFONT={0}";
    private const string GS_FCOFontFile = "-sFCOfontfile={0}";
    private const string GS_FAPIFontMap = "-sFAPIfontmap={0}";
    private const string GS_NoPrecompiledFonts = "-dNOCCFONTS";
    private const int e_Quit = -101;
    private const int e_NeedInput = -106;
    private static Mutex mutex;
    private string _sDeviceFormat;
    private string _sParametersUsed;
    private int _iWidth;
    private int _iHeight;
    private int _iResolutionX;
    private int _iResolutionY;
    private int _iJPEGQuality;
    private bool _bFitPage;
    private string _sDefaultPageSize;
    private IntPtr _objHandle;
    private Process myProcess;
    public StringBuilder output;
    private string _sSubstitutionFont;
    private string _sFCOFontFile;
    private string _sFAPIFontMap;

    public int RenderingThreads
    {
      get
      {
        return this._iRenderingThreads;
      }
      set
      {
        if (value == 0)
          this._iRenderingThreads = Environment.ProcessorCount;
        else
          this._iRenderingThreads = value;
      }
    }

    public string OutputFormat
    {
      get
      {
        return this._sDeviceFormat;
      }
      set
      {
        this._sDeviceFormat = value;
      }
    }

    public string DefaultPageSize
    {
      get
      {
        return this._sDefaultPageSize;
      }
      set
      {
        this._sDefaultPageSize = value;
      }
    }

    public bool ForcePageSize
    {
      get
      {
        return this._bForcePageSize;
      }
      set
      {
        this._bForcePageSize = value;
      }
    }

    public string ParametersUsed
    {
      get
      {
        return this._sParametersUsed;
      }
      set
      {
        this._sParametersUsed = value;
      }
    }

    public int Width
    {
      get
      {
        return this._iWidth;
      }
      set
      {
        this._iWidth = value;
      }
    }

    public int Height
    {
      get
      {
        return this._iHeight;
      }
      set
      {
        this._iHeight = value;
      }
    }

    public int ResolutionX
    {
      get
      {
        return this._iResolutionX;
      }
      set
      {
        this._iResolutionX = value;
      }
    }

    public int ResolutionY
    {
      get
      {
        return this._iResolutionY;
      }
      set
      {
        this._iResolutionY = value;
      }
    }

    public int GraphicsAlphaBit
    {
      get
      {
        return this._iGraphicsAlphaBit;
      }
      set
      {
        if (value > 4 | value == 3)
          throw new ArgumentOutOfRangeException("The Graphics Alpha Bit must have a value between 1 2 and 4, or <= 0 if not set");
        this._iGraphicsAlphaBit = value;
      }
    }

    public int TextAlphaBit
    {
      get
      {
        return this._iTextAlphaBit;
      }
      set
      {
        if (value > 4 | value == 3)
          throw new ArgumentOutOfRangeException("The Text Alpha Bit must have a value between 1 2 and 4, or <= 0 if not set");
        this._iTextAlphaBit = value;
      }
    }

    public bool FitPage
    {
      get
      {
        return this._bFitPage;
      }
      set
      {
        this._bFitPage = value;
      }
    }

    public int JPEGQuality
    {
      get
      {
        return this._iJPEGQuality;
      }
      set
      {
        this._iJPEGQuality = value;
      }
    }

    public int FirstPageToConvert
    {
      get
      {
        return this._iFirstPageToConvert;
      }
      set
      {
        this._iFirstPageToConvert = value;
      }
    }

    public int LastPageToConvert
    {
      get
      {
        return this._iLastPageToConvert;
      }
      set
      {
        this._iLastPageToConvert = value;
      }
    }

    public bool ThrowOnlyException
    {
      get
      {
        return this._bThrowOnlyException;
      }
      set
      {
        this._bThrowOnlyException = value;
      }
    }

    public bool RedirectIO
    {
      get
      {
        return this._bRedirectIO;
      }
      set
      {
        this._bRedirectIO = value;
      }
    }

    public bool OutputToMultipleFile
    {
      get
      {
        return this._didOutputToMultipleFile;
      }
      set
      {
        this._didOutputToMultipleFile = value;
      }
    }

    public bool UseMutex
    {
      get
      {
        return PDFConvert.mutex != null;
      }
      set
      {
        if (!value)
        {
          if (PDFConvert.mutex == null)
            return;
          PDFConvert.mutex.ReleaseMutex();
          PDFConvert.mutex.Close();
          PDFConvert.mutex = (Mutex) null;
        }
        else if (PDFConvert.mutex == null)
          PDFConvert.mutex = new Mutex(false, "MutexGhostscript");
      }
    }

    public List<string> FontPath
    {
      get
      {
        return this._sFontPath;
      }
      set
      {
        this._sFontPath = value;
      }
    }

    public bool DisablePlatformFonts
    {
      get
      {
        return this._bDisablePlatformFonts;
      }
      set
      {
        this._bDisablePlatformFonts = value;
      }
    }

    public bool DisableFontMap
    {
      get
      {
        return this._bDisableFontMap;
      }
      set
      {
        this._bDisableFontMap = value;
      }
    }

    public List<string> FontMap
    {
      get
      {
        return this._sFontMap;
      }
      set
      {
        this._sFontMap = value;
      }
    }

    public string SubstitutionFont
    {
      get
      {
        return this._sSubstitutionFont;
      }
      set
      {
        this._sSubstitutionFont = value;
      }
    }

    public string FCOFontFile
    {
      get
      {
        return this._sFCOFontFile;
      }
      set
      {
        this._sFCOFontFile = value;
      }
    }

    public string FAPIFontMap
    {
      get
      {
        return this._sFAPIFontMap;
      }
      set
      {
        this._sFAPIFontMap = value;
      }
    }

    public bool DisablePrecompiledFonts
    {
      get
      {
        return this._bDisablePrecompiledFonts;
      }
      set
      {
        this._bDisablePrecompiledFonts = value;
      }
    }

    public PDFConvert(IntPtr objHandle)
    {
      this._objHandle = objHandle;
    }

    public PDFConvert()
    {
      this._objHandle = IntPtr.Zero;
    }

    [DllImport("kernel32.dll", EntryPoint = "RtlMoveMemory")]
    private static extern void CopyMemory(IntPtr Destination, IntPtr Source, uint Length);

    [DllImport("gsdll32.dll")]
    private static extern int gsapi_new_instance(out IntPtr pinstance, IntPtr caller_handle);

    [DllImport("gsdll32.dll")]
    private static extern int gsapi_init_with_args(IntPtr instance, int argc, IntPtr argv);

    [DllImport("gsdll32.dll")]
    private static extern int gsapi_exit(IntPtr instance);

    [DllImport("gsdll32.dll")]
    private static extern void gsapi_delete_instance(IntPtr instance);

    [DllImport("gsdll32.dll")]
    private static extern int gsapi_revision(ref GS_Revision pGSRevisionInfo, int intLen);

    [DllImport("gsdll32.dll")]
    private static extern int gsapi_set_stdio(IntPtr lngGSInstance, StdioCallBack gsdll_stdin, StdioCallBack gsdll_stdout, StdioCallBack gsdll_stderr);

    public bool Convert(string inputFile, string outputFile)
    {
      return this.Convert(inputFile, outputFile, this._bThrowOnlyException, (string) null);
    }

    public bool Convert(string inputFile, string outputFile, string parameters)
    {
      return this.Convert(inputFile, outputFile, this._bThrowOnlyException, parameters);
    }

    private bool Convert(string inputFile, string outputFile, bool throwException, string options)
    {
      if (string.IsNullOrEmpty(inputFile))
        throw new ArgumentNullException("inputFile");
      if (!File.Exists(inputFile))
        throw new ArgumentException(string.Format("The file :'{0}' doesn't exist", (object) inputFile), "inputFile");
      if (string.IsNullOrEmpty(this._sDeviceFormat))
        throw new ArgumentNullException("Device");
      if (PDFConvert.mutex != null)
        PDFConvert.mutex.WaitOne();
      bool flag = false;
      try
      {
        flag = this.ExecuteGhostscriptCommand(this.GetGeneratedArgs(inputFile, outputFile, options));
      }
      finally
      {
        if (PDFConvert.mutex != null)
          PDFConvert.mutex.ReleaseMutex();
      }
      return flag;
    }

    public bool Print(string inputFile, string printParametersFile)
    {
      if (string.IsNullOrEmpty(inputFile))
        throw new ArgumentNullException("inputFile");
      if (!File.Exists(inputFile))
        throw new ArgumentException(string.Format("The file :'{0}' doesn't exist", (object) inputFile), "inputFile");
      if (string.IsNullOrEmpty(printParametersFile))
        throw new ArgumentNullException("printParametersFile");
      if (!File.Exists(printParametersFile))
        throw new ArgumentException(string.Format("The file :'{0}' doesn't exist", (object) printParametersFile), "printParametersFile");
      List<string> stringList = new List<string>(7);
      stringList.Add("printPdf");
      stringList.Add("-dNOPAUSE");
      stringList.Add("-dBATCH");
      if (this._iFirstPageToConvert > 0)
        stringList.Add(string.Format("-dFirstPage={0}", (object) this._iFirstPageToConvert));
      if (this._iLastPageToConvert > 0 && this._iLastPageToConvert >= this._iFirstPageToConvert)
        stringList.Add(string.Format("-dLastPage={0}", (object) this._iLastPageToConvert));
      stringList.Add(printParametersFile);
      stringList.Add(inputFile);
      bool flag = false;
      if (PDFConvert.mutex != null)
        PDFConvert.mutex.WaitOne();
      try
      {
        flag = this.ExecuteGhostscriptCommand(stringList.ToArray());
      }
      finally
      {
        if (PDFConvert.mutex != null)
          PDFConvert.mutex.ReleaseMutex();
      }
      return flag;
    }

    private bool ExecuteGhostscriptCommand(string[] sArgs)
    {
      IntPtr pinstance = IntPtr.Zero;
      int length = sArgs.Length;
      object[] objArray = new object[length];
      IntPtr[] numArray = new IntPtr[length];
      GCHandle[] aGCHandle = new GCHandle[length];
      for (int index = 0; index < length; ++index)
      {
        objArray[index] = (object) PDFConvert.StringToAnsiZ(sArgs[index]);
        aGCHandle[index] = GCHandle.Alloc(objArray[index], GCHandleType.Pinned);
        numArray[index] = aGCHandle[index].AddrOfPinnedObject();
      }
      GCHandle gchandleArgs = GCHandle.Alloc((object) numArray, GCHandleType.Pinned);
      IntPtr argv = gchandleArgs.AddrOfPinnedObject();
      int num1 = -1;
      try
      {
        if (PDFConvert.gsapi_new_instance(out pinstance, this._objHandle) < 0)
        {
          this.ClearParameters(ref aGCHandle, ref gchandleArgs);
          throw new ApplicationException("I can't create a new istance of Ghostscript please verify no other istance are running!");
        }
      }
      catch (BadImageFormatException ex)
      {
        this.ClearParameters(ref aGCHandle, ref gchandleArgs);
        if (IntPtr.Size == 8)
          throw new ApplicationException(string.Format("The gsdll32.dll you provide is not compatible with the current architecture that is 64bit,Please download any version above version 8.64 from the original website in the 64bit or x64 or AMD64 version!"));
        if (IntPtr.Size == 4)
          throw new ApplicationException(string.Format("The gsdll32.dll you provide is not compatible with the current architecture that is 32bit,Please download any version above version 8.64 from the original website in the 32bit or x86 or i386 version!"));
      }
      catch (DllNotFoundException ex)
      {
        this.ClearParameters(ref aGCHandle, ref gchandleArgs);
        throw new ApplicationException("The gsdll32.dll wasn't found in default dlls search pathor is not in correct version (doesn't expose the required methods). Please download at least the version 8.64 from the original website");
      }
      IntPtr num2 = IntPtr.Zero;
      if (this._bRedirectIO)
      {
        StdioCallBack gsdll_stdin = new StdioCallBack(this.gsdll_stdin);
        StdioCallBack gsdll_stdout = new StdioCallBack(this.gsdll_stdout);
        StdioCallBack gsdll_stderr = new StdioCallBack(this.gsdll_stderr);
        num1 = PDFConvert.gsapi_set_stdio(pinstance, gsdll_stdin, gsdll_stdout, gsdll_stderr);
        if (this.output == null)
          this.output = new StringBuilder();
        else
          this.output.Remove(0, this.output.Length);
        this.myProcess = Process.GetCurrentProcess();
        this.myProcess.OutputDataReceived += new DataReceivedEventHandler(this.SaveOutputToImage);
      }
      int num3 = -1;
      try
      {
        num3 = PDFConvert.gsapi_init_with_args(pinstance, length, argv);
      }
      catch (Exception ex)
      {
        throw new ApplicationException(ex.Message, ex);
      }
      finally
      {
        this.ClearParameters(ref aGCHandle, ref gchandleArgs);
        PDFConvert.gsapi_exit(pinstance);
        PDFConvert.gsapi_delete_instance(pinstance);
        if (this.myProcess != null && this._bRedirectIO)
          this.myProcess.OutputDataReceived -= new DataReceivedEventHandler(this.SaveOutputToImage);
      }
      return num3 == 0 | num3 == -101;
    }

    private void ClearParameters(ref GCHandle[] aGCHandle, ref GCHandle gchandleArgs)
    {
      for (int index = 0; index < aGCHandle.Length; ++index)
        aGCHandle[index].Free();
      gchandleArgs.Free();
    }

    private void SaveOutputToImage(object sender, DataReceivedEventArgs e)
    {
      this.output.Append(e.Data);
    }

    private string[] GetGeneratedArgs(string inputFile, string outputFile, string otherParameters)
    {
      if (string.IsNullOrEmpty(otherParameters))
        return this.GetGeneratedArgs(inputFile, outputFile, (string[]) null);
      return this.GetGeneratedArgs(inputFile, outputFile, otherParameters.Split(new string[1]{ " " }, StringSplitOptions.RemoveEmptyEntries));
    }

    private string[] GetGeneratedArgs(string inputFile, string outputFile, string[] presetParameters)
    {
      ArrayList arrayList = new ArrayList();
      string[] strArray;
      if (presetParameters == null || presetParameters.Length == 0)
      {
        if (this._sDeviceFormat == "jpeg" && this._iJPEGQuality > 0 && this._iJPEGQuality < 101)
          arrayList.Add((object) string.Format("-dJPEGQ={0}", (object) this._iJPEGQuality));
        if (this._iWidth > 0 && this._iHeight > 0)
          arrayList.Add((object) string.Format("-g{0}x{1}", (object) this._iWidth, (object) this._iHeight));
        else if (!string.IsNullOrEmpty(this._sDefaultPageSize))
        {
          arrayList.Add((object) string.Format("-sPAPERSIZE={0}", (object) this._sDefaultPageSize));
          if (this._bForcePageSize)
            arrayList.Add((object) "-dFIXEDMEDIA");
        }
        if (this._iGraphicsAlphaBit > 0)
          arrayList.Add((object) string.Format("-dGraphicsAlphaBits={0}", (object) this._iGraphicsAlphaBit));
        if (this._iTextAlphaBit > 0)
          arrayList.Add((object) string.Format("-dTextAlphaBits={0}", (object) this._iTextAlphaBit));
        if (this._bFitPage)
          arrayList.Add((object) "-dPDFFitPage");
        if (this._iResolutionX > 0)
        {
          if (this._iResolutionY > 0)
            arrayList.Add((object) string.Format("-r{0}x{1}", (object) this._iResolutionX, (object) this._iResolutionY));
          else
            arrayList.Add((object) string.Format("-r{0}", (object) this._iResolutionX));
        }
        if (this._iFirstPageToConvert > 0)
          arrayList.Add((object) string.Format("-dFirstPage={0}", (object) this._iFirstPageToConvert));
        if (this._iLastPageToConvert > 0)
        {
          if (this._iFirstPageToConvert > 0 && this._iFirstPageToConvert > this._iLastPageToConvert)
            throw new ArgumentOutOfRangeException(string.Format("The 1st page to convert ({0}) can't be after then the last one ({1})", (object) this._iFirstPageToConvert, (object) this._iLastPageToConvert));
          arrayList.Add((object) string.Format("-dLastPage={0}", (object) this._iLastPageToConvert));
        }
        if (this._iRenderingThreads > 0)
          arrayList.Add((object) string.Format("-dNumRenderingThreads={0}", (object) this._iRenderingThreads));
        if (!this._bRedirectIO)
          ;
        if (this._sFontPath != null && this._sFontPath.Count > 0)
          arrayList.Add((object) string.Format("-sFONTPATH={0}", (object) string.Join(";", this._sFontPath.ToArray())));
        if (this._bDisablePlatformFonts)
          arrayList.Add((object) "-dNOPLATFONTS");
        if (this._bDisableFontMap)
          arrayList.Add((object) "-dNOFONTMAP");
        if (this._sFontMap != null && this._sFontMap.Count > 0)
          arrayList.Add((object) string.Format("-sFONTMAP={0}", (object) string.Join(";", this._sFontMap.ToArray())));
        if (!string.IsNullOrEmpty(this._sSubstitutionFont))
          arrayList.Add((object) string.Format("-sSUBSTFONT={0}", (object) this._sSubstitutionFont));
        if (!string.IsNullOrEmpty(this._sFCOFontFile))
          arrayList.Add((object) string.Format("-sFCOfontfile={0}", (object) this._sFCOFontFile));
        if (!string.IsNullOrEmpty(this._sFAPIFontMap))
          arrayList.Add((object) string.Format("-sFAPIfontmap={0}", (object) this._sFAPIFontMap));
        if (this._bDisablePrecompiledFonts)
          arrayList.Add((object) "-dNOCCFONTS");
        int num = 7;
        int count = arrayList.Count;
        strArray = new string[num + arrayList.Count];
        strArray[1] = "-dNOPAUSE";
        strArray[2] = "-dBATCH";
        strArray[3] = "-dSAFER";
        strArray[4] = string.Format("-sDEVICE={0}", (object) this._sDeviceFormat);
        for (int index = 0; index < count; ++index)
          strArray[5 + index] = (string) arrayList[index];
      }
      else
      {
        strArray = new string[presetParameters.Length + 3];
        for (int index = 1; index <= presetParameters.Length; ++index)
          strArray[index] = presetParameters[index - 1];
      }
      strArray[0] = "pdf2img";
      if (this._didOutputToMultipleFile && !outputFile.Contains("%"))
      {
        int startIndex = outputFile.LastIndexOf('.');
        if (startIndex > 0)
          outputFile = outputFile.Insert(startIndex, "%d");
      }
      this._sParametersUsed = string.Empty;
      for (int index = 1; index < strArray.Length - 2; ++index)
      {
        PDFConvert pdfConvert = this;
        string str = pdfConvert._sParametersUsed + " " + strArray[index];
        pdfConvert._sParametersUsed = str;
      }
      strArray[strArray.Length - 2] = string.Format("-sOutputFile={0}", (object) outputFile);
      strArray[strArray.Length - 1] = string.Format("{0}", (object) inputFile);
      PDFConvert pdfConvert1 = this;
      string str1 = pdfConvert1._sParametersUsed + " " + string.Format("-sOutputFile={0}", (object) string.Format("\"{0}\"", (object) outputFile)) + " " + string.Format("\"{0}\"", (object) inputFile);
      pdfConvert1._sParametersUsed = str1;
      return strArray;
    }

    private static byte[] StringToAnsiZ(string str)
    {
      if (str == null)
        str = string.Empty;
      return Encoding.Default.GetBytes(str);
    }

    public static string AnsiZtoString(IntPtr strz)
    {
      if (strz != IntPtr.Zero)
        return Marshal.PtrToStringAnsi(strz);
      return string.Empty;
    }

    public static bool CheckDll()
    {
      return File.Exists("gsdll32.dll");
    }

    public int gsdll_stdin(IntPtr intGSInstanceHandle, IntPtr strz, int intBytes)
    {
      if (intBytes == 0)
        return 0;
      int num1 = Console.Read();
      if (num1 == -1)
        return 0;
      GCHandle gcHandle = GCHandle.Alloc((object) (byte) num1, GCHandleType.Pinned);
      IntPtr Source = gcHandle.AddrOfPinnedObject();
      PDFConvert.CopyMemory(strz, Source, 1U);
      IntPtr num2 = IntPtr.Zero;
      gcHandle.Free();
      return 1;
    }

    public int gsdll_stdout(IntPtr intGSInstanceHandle, IntPtr strz, int intBytes)
    {
      byte[] numArray = new byte[intBytes];
      GCHandle gcHandle = GCHandle.Alloc((object) numArray, GCHandleType.Pinned);
      PDFConvert.CopyMemory(gcHandle.AddrOfPinnedObject(), strz, (uint) intBytes);
      IntPtr num = IntPtr.Zero;
      gcHandle.Free();
      string str = "";
      for (int index = 0; index < intBytes; ++index)
        str += (string) (object) (char) numArray[index];
      this.output.Append(str);
      return intBytes;
    }

    public int gsdll_stderr(IntPtr intGSInstanceHandle, IntPtr strz, int intBytes)
    {
      return this.gsdll_stdout(intGSInstanceHandle, strz, intBytes);
    }

    public GhostScriptRevision GetRevision()
    {
      GS_Revision pGSRevisionInfo = new GS_Revision();
      GCHandle gcHandle = GCHandle.Alloc((object) pGSRevisionInfo, GCHandleType.Pinned);
      PDFConvert.gsapi_revision(ref pGSRevisionInfo, 16);
      GhostScriptRevision ghostScriptRevision;
      ghostScriptRevision.intRevision = pGSRevisionInfo.intRevision;
      ghostScriptRevision.intRevisionDate = pGSRevisionInfo.intRevisionDate;
      ghostScriptRevision.ProductInformation = PDFConvert.AnsiZtoString(pGSRevisionInfo.strProduct);
      ghostScriptRevision.CopyrightInformations = PDFConvert.AnsiZtoString(pGSRevisionInfo.strCopyright);
      gcHandle.Free();
      return ghostScriptRevision;
    }
  }
}
