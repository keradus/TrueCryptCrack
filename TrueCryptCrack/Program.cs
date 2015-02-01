using System;
using System.Collections.Generic;
using System.IO;

/// <summary>
/// Simple tool to recover lost password for TrueCrypt using dictionary.
/// It is not designed to be error proof, sorry.
/// You may get a false positive if Device you wan't to recover is already mounted.
/// </summary>
namespace TrueCryptCrack
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("No config file passed");
                return;
            }

            dynamic expandoConfig = JsonHelpers.DeserializeJson(File.ReadAllText(args[0]));
            Console.WriteLine("-= START =-");
            Console.WriteLine();

            System.DateTime timeStart = System.DateTime.Now;

            Cracker cracker = new Cracker();
            cracker.Exe = expandoConfig.exe;
            cracker.Debug = expandoConfig.debug;
            cracker.Device = expandoConfig.device;
            cracker.MountLetter = expandoConfig.mountLetter;
            cracker.DictionaryFile = expandoConfig.dictionaryFile;

            if (((IDictionary<String, object>)expandoConfig).ContainsKey("dictionaryMap"))
            {
                cracker.DictionaryMap = new Dictionary<char, string>();
                foreach (KeyValuePair<string, object> item in expandoConfig.dictionaryMap)
                {
                    cracker.DictionaryMap[item.Key[0]] = item.Value.ToString();
                }
            }

            if (((IDictionary<String, object>)expandoConfig).ContainsKey("toolType"))
            {
                try
                {
                    cracker.UsedToolType = (Cracker.ToolType)Enum.Parse(typeof(Cracker.ToolType), expandoConfig.toolType);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Invalid toolType");
                    return;
                }
            }

            string password = cracker.FindPassword();

            System.DateTime timeFinish = System.DateTime.Now;

            Console.WriteLine();

            if (null == password)
            {
                Console.WriteLine("No password works");
            }
            else
            {
                Console.WriteLine("Password found: " + password);
            }

            Console.WriteLine("Took time: " + timeFinish.Subtract(timeStart));
            Console.WriteLine("-= FINISH =-");
        }
    }
}
