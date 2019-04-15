using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileExplorer;

using Outlook = Microsoft.Office.Interop.Outlook;
using Office = Microsoft.Office.Core;


namespace OutlookHandle
{
    class Extension
    {
        IEnumerable<Outlook.MailItem> GetSelectedEmails()
        {
            foreach (Outlook.MailItem email in new Microsoft.Office.Interop.Outlook.Application().ActiveExplorer().Selection)
            {
                yield return email;
            }
        }

        public List<string> GetEmailMarked()
        {
            string name, body, date, read;
            List<string> result = new List<string>();
            
            IEnumerable<Outlook.MailItem> list = GetSelectedEmails();


            foreach(Outlook.MailItem x in list)
            {
                name = x.SenderName;
                body = x.Body;
                date = string.Format(x.ReceivedTime.Year + "-" + x.ReceivedTime.Month + "-" + x.ReceivedTime.Day);
                read = date + "-" + name + "-" + body;
                result.Add(read);
            }

            return result;
        }

        public List<string> SaveEmailMarked(string path)
        {
            string name, body, date, read, save;
            List<string> result = new List<string>();

            IEnumerable<Outlook.MailItem> list = GetSelectedEmails();


            foreach (Outlook.MailItem x in list)
            {
                name = x.SenderName;
                body = x.Body;
                date = string.Format(x.ReceivedTime.Year + "-" + x.ReceivedTime.Month + "-" + x.ReceivedTime.Day);
                read = date + "-" + name;
                save = string.Format(path + "\\" + read);
                x.SaveAs(save);
                //result.Add(read);
            }

            FileExplorer.Form1.ActiveForm.Close();
            return result;
        }
    }
}
