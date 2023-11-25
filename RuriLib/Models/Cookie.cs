using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RuriLib.Models
{
    public class Cookie : Persistable<Guid>
    {
        /// <summary>The name of the Cookie.</summary>
        public string Name { get; set; }

        /// <summary>The path where the file is stored on disk.</summary>
        public string PathOwnerFolder { get; set; }

        public List<string> PathAllCookiesFolders { get; set; }

        /// <summary>The total number of data lines of the file.</summary>
        public int TotalCookiesFolders { get; set; }

        /// <summary>Needed for NoSQL deserialization.</summary>
        public Cookie() { }

        /// <summary>
        /// Creates an instance of a Wordlist.
        /// </summary>
        /// <param name="name">The name of the Cookies</param>
        /// <param name="path">The path to the file on disk</param>
        public Cookie(string name, string pathownerfolder)
        {
            Name = name;
            PathOwnerFolder = pathownerfolder;
            PathAllCookiesFolders = new List<string>();
            var folders = new List<string>();
            folders.AddRange(Directory.GetDirectories(pathownerfolder, "*Cookies*", SearchOption.AllDirectories));
            folders.AddRange(Directory.GetDirectories(pathownerfolder, "*Browsers*", SearchOption.AllDirectories));
            foreach (var folder in folders)
            {
                string[] files = Directory.GetFiles(folder, "*.txt*", SearchOption.AllDirectories);
                foreach (string item in files.Distinct<string>())
                {
                    PathAllCookiesFolders.Add(item);
                }
            }
            TotalCookiesFolders = PathAllCookiesFolders.Count;
        }
    }
}
