using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LMT_Auto_Messenger_.Net_Core
{
    class Vietnamese
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll")]
        static extern bool ReadConsoleW(IntPtr hConsoleInput, [Out] byte[]
                lpBuffer, uint nNumberOfCharsToRead, out uint lpNumberOfCharsRead,
            IntPtr lpReserved);

        public IntPtr GetWin32InputHandle()
        {
            const int STD_INPUT_HANDLE = -10;
            IntPtr inHandle = GetStdHandle(STD_INPUT_HANDLE);
            return inHandle;
        }

        /// <summary>
        /// Nhận tiếng việt
        /// </summary>
        /// <returns></returns>
        public string ReadLine()
        {
            const int bufferSize = 1024;
            var buffer = new byte[bufferSize];

            uint charsRead = 0;

            ReadConsoleW(GetWin32InputHandle(), buffer, bufferSize, out charsRead, (IntPtr)0);
            // -2 to remove ending \n\r
            int nc = ((int)charsRead - 2) * 2;
            var b = new byte[nc];
            for (var i = 0; i < nc; i++)
                b[i] = buffer[i];

            var utf8enc = Encoding.UTF8;
            var unicodeenc = Encoding.Unicode;
            return utf8enc.GetString(Encoding.Convert(unicodeenc, utf8enc, b));
        }
    }
}
