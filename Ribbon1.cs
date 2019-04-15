using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Tools.Ribbon;

using Outlook = Microsoft.Office.Interop.Outlook;
using Office = Microsoft.Office.Core;


namespace OutlookSave
{
    public partial class Ribbon1
    {
        public FileExplorer.Form1 frm;

        private void Ribbon1_Load(object sender, RibbonUIEventArgs e)
        {

        }

        private void button1_Click(object sender, RibbonControlEventArgs e)
        {
            frm = new FileExplorer.Form1();
            frm.ShowDialog();
        }
    }
}
