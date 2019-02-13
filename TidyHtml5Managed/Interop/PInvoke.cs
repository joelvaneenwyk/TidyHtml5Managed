// Copyright (c) 2009 Mark Beaton
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Runtime.InteropServices;

namespace TidyManaged.Interop
{
    // http://api.html-tidy.org/tidy/tidylib_api_5.2.0/tidylib.html#return_code
    /// <summary>
    /// For all API funcs unless other stated; negative means severe error (usually equals to -errno)
    /// </summary>
    public enum ReturnCode
    {
        Good,
        Warnings,

        /// <summary>
        /// By default, Tidy will not produce output. You can force output with the TidyForceOutput option. As with warnings, check error buffer or track error messages for details.
        /// </summary>
        Errors
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal struct TidyBuffer
    {
        internal IntPtr allocator;  /**< Memory allocator */
        internal IntPtr bp;           /**< Pointer to bytes */
        internal uint size;         /**< # bytes currently in use */
        internal uint allocated;    /**< # bytes allocated */
        internal uint next;         /**< Offset of current input position */
    }

    internal class PInvoke
    {
        static PInvoke()
        {
            // Detect if the system is runnning at 32 or 64 bit and load the corresponding tidy.dll
            var is64 = IntPtr.Size == 8;
            var libraryPath = System.IO.Path.Combine(is64 ? "x64" : "x86", "tidy.dll");
            LoadLibrary(libraryPath);
        }

        [DllImport("kernel32.dll")]
        private static extern IntPtr LoadLibrary(string dllToLoad);

        [DllImport("tidy.dll")]
        internal static extern IntPtr tidyCreate();

        [DllImport("tidy.dll")]
        internal static extern void tidyRelease(IntPtr tdoc);

        [DllImport("tidy.dll")]
        internal static extern IntPtr tidyReleaseDate();

        [DllImport("tidy.dll")]
        internal static extern IntPtr tidyLibraryVersion();

        [DllImport("tidy.dll")]
        internal static extern IntPtr tidyOptGetValue(IntPtr tdoc, TidyOptionId optId);

        [DllImport("tidy.dll")]
        internal static extern bool tidyOptSetValue(IntPtr tdoc, TidyOptionId optId, string val);

        [DllImport("tidy.dll")]
        internal static extern uint tidyOptGetInt(IntPtr tdoc, TidyOptionId optId);

        [DllImport("tidy.dll")]
        internal static extern bool tidyOptSetInt(IntPtr tdoc, TidyOptionId optId, uint val);

        [DllImport("tidy.dll")]
        internal static extern bool tidyOptGetBool(IntPtr tdoc, TidyOptionId optId);

        [DllImport("tidy.dll")]
        internal static extern bool tidyOptSetBool(IntPtr tdoc, TidyOptionId optId, bool val);

        // TIDY_EXPORT void TIDY_CALL tidyBufFree(TidyBuffer* buf);
        /// <summary>
        /// Free current contents and zero out
        /// </summary>
        /// <param name="tdoc"></param>
        /// <param name="buffer"></param>
        /// <param name="buflen"></param>
        /// <returns></returns>
        [DllImport("tidy.dll")]
        internal static extern void tidyBufFree(ref TidyBuffer buf);

        [DllImport("tidy.dll")]
        internal static extern int tidyParseFile(IntPtr tdoc, string filename);

        [DllImport("tidy.dll")]
        internal static extern int tidyParseString(IntPtr tdoc, string content);

        [DllImport("tidy.dll")]
        internal static extern int tidyParseSource(IntPtr tdoc, ref TidyInputSource source);

        [DllImport("tidy.dll")]
        internal static extern int tidyCleanAndRepair(IntPtr tdoc);

        [DllImport("tidy.dll")]
        internal static extern int tidySaveFile(IntPtr tdoc, string filename);

        // // int TIDY_CALL        tidySaveString( TidyDoc tdoc, tmbstr buffer, uint* buflen )
        [DllImport("tidy.dll")]
        internal static extern int tidySaveString(IntPtr tdoc, IntPtr buffer, ref uint buflen);

        [DllImport("tidy.dll")]
        internal static extern int tidySaveSink(IntPtr tdoc, ref TidyOutputSink sink);

        // int TIDY_CALL    tidySetErrorBuffer(TidyDoc tdoc, TidyBuffer* errbuf)
        [DllImport("tidy.dll")]
        internal static extern int tidySetErrorBuffer(IntPtr tdoc, ref TidyBuffer buffer);

        // FILE* TIDY_CALL   tidySetErrorFile(TidyDoc tdoc, ctmbstr errfilnam)
        [DllImport("tidy.dll")]
        internal static extern IntPtr tidySetErrorFile(IntPtr tdoc, string filename);

        internal static string tidyOptGetValueString(IntPtr tdoc, TidyOptionId optId)
        {
            return Marshal.PtrToStringAnsi(tidyOptGetValue(tdoc, optId));
        }
    }
}
