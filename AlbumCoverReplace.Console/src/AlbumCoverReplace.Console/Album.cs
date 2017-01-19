using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlbumCoverReplace
{
    public class Album
    {
        const string a = "Cover.jpg";
        const string b = "Folder.jpg";
        static int changeMode = 1;
        public static void Run()
        {
            string curt = AppContext.BaseDirectory;

            Console.WriteLine("Album Cover Replace 0.1");
            Console.WriteLine("本程序帮您处理目标文件夹音乐专辑图片替换问题，比如Cover.jpg和Album.jpg互换");
            Console.WriteLine("Current Folder is:" + curt + "Type 'n' to change another folder");

            if (Console.ReadLine().Trim().ToLower().Equals("n"))
            {
                while (true)
                {
                    Console.WriteLine("Input a Album Path,'exit' to exit");
                    string tmpPath = Console.ReadLine();
                    if (tmpPath.Trim().ToLower().Equals("exit"))
                    {
                        return;
                    }
                    else if (!Directory.Exists(tmpPath))
                    {
                        Console.WriteLine("Path is not exists!");
                        continue;
                    }
                    else
                    {
                        curt = tmpPath;
                        break;
                    }
                }
            }

            while (true)
            {
                Console.WriteLine("Input a replace Type:1. '{0}' -> '{1}' ,2. '{1}' -> '{0}' ,'exit' to exit", a, b);
                string tmpPath = Console.ReadLine();
                if (tmpPath.Trim().ToLower().Equals("exit"))
                {
                    return;
                }
                else if (tmpPath.Trim().Equals("1"))
                {
                    changeMode = 1;
                    Console.WriteLine("You Entered 1. '{0}' -> '{1}'", a, b);
                    break;
                }
                else if (tmpPath.Trim().Equals("2"))
                {
                    changeMode = 2;
                    Console.WriteLine("You Entered 2. '{1}' -> '{0}'", a, b);
                    break;
                }
                else
                {
                    continue;
                }
            }



            DirectoryInfo dir = new DirectoryInfo(curt);

            var SourceFiles = dir.GetFiles(changeMode == 1 ? a : b, SearchOption.AllDirectories);
            var DestFiles = dir.GetFiles(changeMode == 1 ? b : a, SearchOption.AllDirectories);

            List<DirectoryInfo> SourceDirs = new List<DirectoryInfo>();
            List<DirectoryInfo> DestDirs = new List<DirectoryInfo>();

            //初始化源文件和源文件目录
            foreach (var item in SourceFiles)
            {
                SourceDirs.Add(item.Directory);
            }
            //初始化已有要被覆盖的目录
            foreach (var item in DestFiles)
            {
                DestDirs.Add(item.Directory);
            }
            //无重复，可以copy 的目录
            var CopyDirs = SourceDirs.Where(c => !DestDirs.Exists(d => d.FullName == c.FullName));
            //var CopyDirs = SourceDirs.Except(DestDirs);
            //有重复，需要覆盖的目录
            var ReplaceDirs = SourceDirs.Where(c => DestDirs.Any(d => d.FullName == c.FullName));
            //var ReplaceDirs = SourceDirs.Intersect(DestDirs);

            List<FileInfo> ReplaceSourceFiles = new List<FileInfo>();
            List<FileInfo> ReplaceDestFiles = new List<FileInfo>();
            //输出需要替换的文件
            foreach (var item in ReplaceDirs)
            {
                ReplaceSourceFiles.AddRange(SourceFiles.Where(c => c.DirectoryName == item.FullName));
                ReplaceDestFiles.AddRange(DestFiles.Where(c => c.DirectoryName == item.FullName));
            }

            //列出替换的文件
            Console.WriteLine("Listing pictures can be changed:");
            int replaceCount = 0;
            foreach (var sourceFile in ReplaceSourceFiles)
            {
                replaceCount++;

                string pathname = sourceFile.DirectoryName.Replace(dir.FullName, "");
                string sourcefilename = sourceFile.FullName.Replace(dir.FullName, "");
                FileInfo destFile = ReplaceDestFiles.Where(c => c.DirectoryName == sourceFile.DirectoryName)
                    .First(c => c.Extension.ToLower().Equals(sourceFile.Extension.ToLower()));
                string destfilename = destFile.FullName.Replace(dir.FullName, "");


                Console.WriteLine("[{0:00}] [Source]{1}({2:-0.00} KB), [dest]{3}({4:-0.00} KB)",
                    replaceCount, sourcefilename, sourceFile.Length / 1024.0, destfilename, destFile.Length / 1024.0);
            }
            Console.WriteLine("Total {0} files will be replaced", replaceCount);
            Console.WriteLine("Total {0} files will be copyed", CopyDirs.Count());

            string tempPath;
            while (true)
            {
                Console.WriteLine("'all' to replace and copy all,'copy' to copy all,'exit' to exit");
                tempPath = Console.ReadLine();
                if (tempPath.Trim().ToLower().Equals("exit"))
                {
                    return;
                }
                else if (!tempPath.Trim().Equals("all") && !tempPath.Trim().Equals("copy"))
                {
                    continue;
                }
                else
                {
                    break;
                }
            }
            foreach (var dirs in CopyDirs)
            {
                var files = SourceFiles.Where(c => c.Directory.FullName == dirs.FullName);
                foreach (var file in files)
                {
                    var repName = (changeMode == 1 ? b : a).Substring(0, (changeMode == 1 ? b : a).LastIndexOf('.'));
                    file.CopyTo(dirs.FullName + @"\" + repName + file.Extension);
                }
            }
            if (tempPath.Trim().Equals("all"))
            {
                foreach (var file in ReplaceSourceFiles)
                {
                    var repName = (changeMode == 1 ? b : a).Substring(0, (changeMode == 1 ? b : a).LastIndexOf('.'));
                    file.CopyTo(file.DirectoryName + @"\" + repName + file.Extension, true);
                }
            }
            Console.WriteLine("Task finished");
            Console.Read();
        }
    }
}
