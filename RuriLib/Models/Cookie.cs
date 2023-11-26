using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Alphaleonis.Win32.Filesystem;

namespace RuriLib.Models
{
    public class Cookie : Persistable<Guid>
    {
        /// <summary>The name of the Cookie.</summary>
        public string Name { get; set; }

        /// <summary>The path where the file is stored on disk.</summary>
        public string PathOwnerFolder { get; set; }

        /// <summary>
        /// All cookies files found int the folder PathOwnerFolder
        /// </summary>
        public List<string> PathAllCookiesFiles { get; set; }

        /// <summary>The total number of data lines of the file.</summary>
        public int TotalCookiesFiles { get; set; }

        /// <summary>Needed for NoSQL deserialization.</summary>
        public Cookie() { }

        /// <summary>
        /// Creates an instance of a Wordlist.
        /// </summary>
        /// <param name="name">The name of the Cookies</param>
        /// <param name="pathownerfolder">The path to the folder on disk</param>
        public Cookie(string name, string pathownerfolder)
        {
            Name = name;
            PathOwnerFolder = pathownerfolder;
            PathAllCookiesFiles = new List<string>();

            IEnumerable<string> directories = Alphaleonis.Win32.Filesystem.Directory.EnumerateDirectories(pathownerfolder, "*Cookies*", SearchOption.AllDirectories);
            directories.Concat(Alphaleonis.Win32.Filesystem.Directory.EnumerateDirectories(pathownerfolder, "*Browsers*", SearchOption.AllDirectories));
            directories.Distinct();

            #region Testing
            //(from directory in directories.AsParallel()
            //where Alphaleonis.Win32.Filesystem.Directory.Exists(directory)
            //select directory).ForAll(dir => {

            //    string[] files = Alphaleonis.Win32.Filesystem.Directory.GetFiles(dir, "*.txt*", SearchOption.AllDirectories);
            //    foreach (string item in files.Distinct<string>())
            //    {
            //        PathAllCookiesFiles.Add(item);
            //    }

            //});

            //Parallel.ForEach(directories, path =>
            //{
            //    if (Alphaleonis.Win32.Filesystem.Directory.Exists(path))
            //    {
            //        string[] files = Alphaleonis.Win32.Filesystem.Directory.GetFiles(path, "*.txt*", SearchOption.AllDirectories);
            //        foreach (string item in files.Distinct<string>())
            //        {
            //            PathAllCookiesFiles.Add(item);
            //        }
            //    }
            //});
            #endregion

            foreach (string path in directories)
            {
                if (Alphaleonis.Win32.Filesystem.Directory.Exists(path))
                {
                    string[] files = Alphaleonis.Win32.Filesystem.Directory.GetFiles(path, "*.txt*", SearchOption.AllDirectories);
                    foreach (string item in files.Distinct<string>())
                    {
                        PathAllCookiesFiles.Add(item);
                    }
                }
            }
            TotalCookiesFiles = PathAllCookiesFiles.Count;
        }
    }
}
