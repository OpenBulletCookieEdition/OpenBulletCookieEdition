using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RuriLib.Utils
{
    /// <summary>
    /// Utils
    /// </summary>
    public static class ParseCookieFiles
    {
        /// <summary>
        /// Parse all cookies files in the root path.
        /// </summary>
        /// <param name="path">Root path</param>
        /// <returns></returns>
        public static List<string> Parse(string path)
        {
            var cookieFiles = new List<string>();

            string[] directories = null;
            string[] directories2 = null;

            var t1 = new Thread(delegate ()
            {
                Thread.CurrentThread.IsBackground = true;
                directories = Alphaleonis.Win32.Filesystem.Directory.GetDirectories(path, "*Cookies*", SearchOption.AllDirectories);
                if (directories != null)
                {
                    foreach (string directory in directories)
                    {
                        if (Alphaleonis.Win32.Filesystem.Directory.Exists(directory))
                        {
                            string[] files = Alphaleonis.Win32.Filesystem.Directory.GetFiles(directory, "*.txt*", SearchOption.AllDirectories);
                            foreach (string item in files)
                            {
                                cookieFiles.Add(item);
                            }
                        }
                    }
                }
            });
            var t2 = new Thread(delegate ()
            {
                Thread.CurrentThread.IsBackground = true;
                directories2 = Alphaleonis.Win32.Filesystem.Directory.GetDirectories(path, "*Browsers*", SearchOption.AllDirectories);
                if (directories2 != null)
                {
                    foreach (string directory in directories2)
                    {
                        if (Alphaleonis.Win32.Filesystem.Directory.Exists(directory))
                        {
                            string[] files = Alphaleonis.Win32.Filesystem.Directory.GetFiles(directory, "*.txt*", SearchOption.AllDirectories);
                            foreach (string item in files)
                            {
                                cookieFiles.Add(item);
                            }
                        }
                    }
                }
            });
            t1.Start();
            t2.Start();
            t1.Join();
            t2.Join();

            return cookieFiles;
        }
    }
}
