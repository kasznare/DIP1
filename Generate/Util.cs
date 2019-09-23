using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WindowsFormsApp1 {
    class Util {

        #region LogVariables
        public static string LocalLogPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\DIP1\";
        public static string LogName = "Diploma-" + DateTime.Today.ToString("yyyy.MM.dd") + ".log";
        private static StreamWriter file;
        private static Thread FileMonitorThread = null;     //thread that starts when we wrote to file, and releases the file after 1 second
        static DateTime LastWriteTime = DateTime.Now;       //to have data when we wrote the last line
        static DateTime LastUploadTime = DateTime.MinValue; //to have data when we wrote the last line
        private static string LastLine = "";
        private static int LineCounter = 0;
        private static int MaxLineSequence = 100;            //to stop log spamming
        private static float LogHoldTime = 1;               //holding the log open for the given seconds
        private static float UploadHoldTime = 300;          //holding the log open for the given seconds
        private static int warnLevel = 2;                   //2 all, 1 debug, 0 exceptions
        private static bool running = false;

        #endregion
        public static void WriteLog<T>(List<T> IncomingMessage) {
            StringBuilder builder = new StringBuilder();

            foreach (var item in IncomingMessage) {
                {
                    builder.Append(item.ToString());
                }

                builder.Append("; ");
            }
            WriteLog(builder);
        }
        /// <summary>
        /// Write any object to the log, string is the recommended default though
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="IncomingMessage"></param>
        /// <param name="MessageLocation"></param>
        public static void WriteLog<T>(T IncomingMessage) {
            string Message = IncomingMessage.ToString();
            LastWriteTime = DateTime.Now;

            //Same message is now only written to the log 'MaxLineSequence' times in sequence
            if (LastLine.Equals(Message)) {
                LineCounter++;
            }
            else {
                LineCounter = 0;
            }

            if (FileMonitorThread == null || !FileMonitorThread.IsAlive) {
                ThreadStart threadDelegate = new ThreadStart(Util.CloseLogAndThread);
                FileMonitorThread = new Thread(threadDelegate);
                FileMonitorThread.Start();
            }

            if (LineCounter < MaxLineSequence) {
                LastLine = Message;
                try {
                    if (!Directory.Exists(LocalLogPath)) {
                        Directory.CreateDirectory(LocalLogPath);
                    }

                    if (file == null) {
                        try {
                            file = File.AppendText(LocalLogPath + LogName);
                        }
                        catch (Exception) {
                            file = File.AppendText(LocalLogPath + "_" + LogName);
                        }
                    }

                    file.WriteLineAsync($"{DateTime.Now.ToString(@"yyyy.MM.dd. HH:mm:ss")} : {Message}");

                    if (Message.ToLower().Contains("error") || Message.ToLower().Contains("warning")) {
                        //UploadLogs();
                    }
                }
                catch (Exception) {

                }
            }
            else if (LineCounter == MaxLineSequence) {
                if (file == null) {
                    file = File.AppendText(LocalLogPath + LogName);
                }

                file.WriteLineAsync($"{DateTime.Now.ToString(@"yyyy.MM.dd. HH:mm:ss")} : ...");
            }
        }
        private static void CloseLogAndThread() {
            try {
                while (true) {
                    Thread.Sleep(400);
                    if (DateTime.Now.Subtract(LastWriteTime).TotalSeconds > LogHoldTime) {
                        file.Close();
                        file.Dispose();
                        file = null;
                        FileMonitorThread.Abort();
                    }
                }
            }
            catch (Exception) { }
        }


    }
}
