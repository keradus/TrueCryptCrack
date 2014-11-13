using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace TrueCryptCrack
{
    class Cracker
    {
        public bool Debug = false;
        public string Device = null;
        public string DictionaryFile = null;
        public Dictionary<char, string> DictionaryMap = null;
        public string Exe = null;
        public string MountLetter = null;

        public bool CheckPass(string password)
        {
            Process tc = new Process();

            tc.StartInfo.FileName = this.Exe;
            tc.StartInfo.Arguments = string.Format("/v \"{0}\" /l \"{1}\" /p \"{2}\" /q /s", this.Device, this.MountLetter, password);

            tc.Start();
            tc.WaitForExit();

            return 0 == tc.ExitCode;
        }

        public string ResolvePasswordUsingMap(string password)
        {
            if (null == this.DictionaryMap)
            {
                return password;
            }

            string pass = "";

            foreach (char passwordChar in password)
            {
                pass += (this.DictionaryMap.ContainsKey(passwordChar) ? this.DictionaryMap[passwordChar] : passwordChar.ToString());
            }

            return pass;
        }

        public string FindPassword()
        {
            int i = 0;
            string resolvedPassword;

            foreach (string password in this.GetNextPassword())
            {
                resolvedPassword = this.ResolvePasswordUsingMap(password);

                if (this.Debug)
                {
                    Console.WriteLine(++i + ": -= " + resolvedPassword + " =-");
                }

                if (this.CheckPass(resolvedPassword))
                {
                    return resolvedPassword;
                }
            }

            return null;
        }

        public IEnumerable<string> GetNextPassword()
        {
            string line;
            StreamReader file = new StreamReader(this.DictionaryFile);

            while ((line = file.ReadLine()) != null)
            {
                yield return line;
            }

            file.Close();
        }
    }
}
