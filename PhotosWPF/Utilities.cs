using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PhotosWPF
{
    static class Utilities
    {
        private static TextBox _textBoxLog;

        public static void InitLogger(TextBox logbox)
        {
            _textBoxLog = logbox;
        }

        public static void Log(String message)
        {
            if (_textBoxLog.Text.Length > 1)
                _textBoxLog.Text += "\n" + message;
            else
                _textBoxLog.Text += message;
        }
    }
}
