using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Globalization;

namespace FileManagerConsole
{
    class Program
    {
        static public IEnumerable<DirectoryInfo> listOfDirectories;
        static public IEnumerable<FileInfo> listOfFiles;

        static void Main(string[] args)
        {
            string path = "";
            string newPath = "";
            int position = 0;

            if (args != null && args.Length > 0)
                path = args[0];

            bool showDir = true;


            while (true)
            {
                if (showDir)
                    path = ListDirectoryContents(path);

                var pressedKey = Console.ReadKey();
                if (pressedKey.Key == ConsoleKey.Tab)
                {
                    TabPressed(path, position);
                    showDir = false;
                    if (position < listOfDirectories.Count() + listOfFiles.Count() - 1)
                        position++;
                    else
                        position = 0;
                }
                else
                {
                    Console.WriteLine("Please put new path or type 'Exit'  ");
                    newPath = Console.ReadLine().ToLower();

                    if (newPath == "exit")
                        return;
                    else
                    {
                        if (newPath.StartsWith("read "))
                        {
                            PrintFile(path, newPath);
                            showDir = false;
                        }
                        else
                        {
                            string potentialNewPath = Path.Combine(path, newPath);

                            if (Directory.Exists(potentialNewPath))
                                path = potentialNewPath;
                            else
                                path = newPath;
                            showDir = true;
                        }
                    }
                }
            } 
        }

        private static void TabPressed(string path, int position)
        {
            var dirInfo = new DirectoryInfo(path);
            string putString = "";

            if (position < listOfDirectories.Count())
                putString = listOfDirectories.ElementAt(position).Name;
            else if (position < listOfDirectories.Count() + listOfFiles.Count())
                putString = listOfFiles.ElementAt(position - listOfDirectories.Count()).Name;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write("                                                              ");
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(putString);
        }

        private static void PrintFile(string path, string newPath)
        {
            if (!CheckFilePath(ref path, ref newPath))
                return;

            IEnumerable<string> file = File.ReadLines(path);
            int countOfReadedLines = 0;

            while (file.Count() > countOfReadedLines)
            {
                string printString = String.Join("\n", file.Skip(countOfReadedLines).Take(10));

                Console.WriteLine(printString);
                Console.Write("Would you like to read next 10 lines? (Y=yes)  ");

                string readNext = Console.ReadLine().ToLower();

                if (readNext != "y")
                    break;
                else countOfReadedLines += 10;
            }
        }

        private static bool CheckFilePath(ref string path, ref string newPath)
        {
            //  "read read me.txt"
            newPath = newPath.Substring(5);
            string potentialNewPath = Path.Combine(path, newPath);

            if (File.Exists(potentialNewPath))
                path = potentialNewPath;
            else if (File.Exists(newPath))
                path = newPath;
            else
            {
                Console.WriteLine("File {0} doesn't exist", newPath);
                return false;
            }
            return true;
        }

        private static string ListDirectoryContents(string args)
        {
            string path = "..\\..";

            if (!String.IsNullOrEmpty(args)) 
            {
                if (!Directory.Exists(args))
                {
                    Console.WriteLine("Path {0} doesn't exist", args);
                    return path;
                }
                else
                    path = args;
            }

            var dirInfo = new DirectoryInfo(path);
            int dirCount = 0;
            int fileCount = 0;
            long fileSize = 0;

            Console.WriteLine("Folder contents:{0} ", dirInfo.FullName);

            ListFoldersInDirectory(dirInfo, ref dirCount);
            ListFilesInDiretory(dirInfo, ref fileCount, ref fileSize);

            Console.WriteLine("\t\t{0} directories", dirCount);
            Console.WriteLine("\t\t{0} files\t{1:n0} bytes", fileCount, fileSize);

            Console.WriteLine("Please put new path or type 'Exit'  ");
            return path;
        }

        private static void ListFilesInDiretory(DirectoryInfo dirInfo, ref int fileCount, ref long fileSize)
        {
            listOfFiles = dirInfo.EnumerateFiles();
            foreach (var file in listOfFiles)
            {
                Console.WriteLine("{2}\t{0:n0}\t\t{1}", file.Length, file, file.LastWriteTime.ToString("dd.MM.yyyy HH:mm"));
                fileCount++;
                fileSize += file.Length;
            }
        }

        public static void ListFoldersInDirectory(DirectoryInfo dirInfo, ref int dirCount)
        {
            listOfDirectories = dirInfo.EnumerateDirectories();
            foreach (var dir in listOfDirectories)
            {
                Console.WriteLine("{0}\t<DIR>\t\t{1}", dir.LastWriteTime.ToString("dd.MM.yyyy HH:mm"), dir);
                dirCount++;
            }
        }
    }
}