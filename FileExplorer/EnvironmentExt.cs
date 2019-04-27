using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace FileExplorer
{

    class EnvironmentExt
    {
        public static string Get(string name, bool ExpandVariables = true)
        {
            if (ExpandVariables)
            {
                return System.Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.User);
            }
            else
            {
                return (string)Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Session Manager\Environment\").GetValue(name, "", Microsoft.Win32.RegistryValueOptions.DoNotExpandEnvironmentNames);
            }
        }

        public static void Set(string name, string value)
        {
            Environment.SetEnvironmentVariable(name, value, EnvironmentVariableTarget.User);
        }

    }
}
