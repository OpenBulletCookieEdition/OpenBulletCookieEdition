using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Alphaleonis.Win32.Filesystem;

namespace RuriLib.Models
{
    /// <summary>
    /// Represents a file as a source of input data that needs to be tested against a Config by the Runner.
    /// </summary>
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
        /// Creates an instance of a Cookie list.
        /// </summary>
        /// <param name="name">The name of the Root logs path</param>
        /// <param name="pathownerfolder">The path to the folder on disk</param>
        /// <param name="cookiefiles">The paths to the cookie files on disk</param>
        public Cookie(string name, string pathownerfolder, List<string> cookiefiles)
        {
            Name = name;
            PathOwnerFolder = pathownerfolder;
            PathAllCookiesFiles = cookiefiles;
            TotalCookiesFiles = PathAllCookiesFiles.Count;
        }
    }
}
