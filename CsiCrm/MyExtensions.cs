using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CsiCrm
{
    static class MyExtensions
    {
        public static string Args(this string text, params object[] args)
        {
            return string.Format(text, args);
        }

        public static bool HasInfo(this TextBox tb)
        {
            return tb.Text.Length > 0;
        }

        public static void UpdateInfo(this ComboBox cbx)
        {
            ComboBox infoholder = new ComboBox();
            infoholder.DataSource = cbx.DataSource;
            cbx.DataSource = null;
            cbx.DataSource = infoholder.DataSource;
        }

    }
}
