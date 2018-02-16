using System;
using System.Collections.Generic;
using System.Text;

namespace CSFWebSocket.Util {
    internal static class Logger {

        private static LogLevel LastLogLevel = LogLevel.None;
        private static string Prefix;
        private static bool VerboseLogging = true;

        private static void Echo(LogLevel level, string msg) {
            if (LastLogLevel != level) {
                LastLogLevel = level;

                Console.BackgroundColor = ConsoleColor.Black;

                switch (level) {
                    case LogLevel.Debug:
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Prefix = "DEBUG";
                        break;
                    case LogLevel.Info:
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Prefix = "INFO";
                        break;
                    case LogLevel.Warning:
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Prefix = "WARNING";
                        break;
                    case LogLevel.Error:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Prefix = "ERROR";
                        break;
                    case LogLevel.Important:
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        Prefix = "IMPORTANT";
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Prefix = "???";
                        break;
                }
            }

            Console.WriteLine($"[{DateTime.Now.ToString("HH:mm:ss.ffffff")} {Prefix}] {msg}");
        }

        public static void Debug(string msg) {
            if (VerboseLogging) {
                Echo(LogLevel.Debug, msg);
            }
        }

        public static void Info(string msg) {
            Echo(LogLevel.Info, msg);
        }

        public static void Warning(string msg) {
            Echo(LogLevel.Warning, msg);
        }

        public static void Error(string msg) {
            Echo(LogLevel.Error, msg);
        }

        public static void Important(string msg) {
            Echo(LogLevel.Important, msg);
        }

        private enum LogLevel {
            None, Debug, Info, Warning, Error, Important
        }

    }
}
