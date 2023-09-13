using ColossalFramework.UI;
using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace GlobalVariables
{
    public class CodeLogger




    {

        private static readonly string LogFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Colossal Order\Cities_Skylines\Addons\Mods\AirplaneLineRenewal\code_log.txt");
        private static StringBuilder logBuilder = new StringBuilder();





        public static void Log(string message)
        {
            logBuilder.AppendLine(message);
            

        }

        public static void ExportLogToFile()
        {
            File.WriteAllText(LogFilePath, logBuilder.ToString());
            ExceptionPanel panel = UIView.library.ShowModal<ExceptionPanel>("ExceptionPanel");
            panel.SetMessage("exported", "Logs exported", false);
            Console.WriteLine("Code log exported to: " + LogFilePath);
        }
    }


}
