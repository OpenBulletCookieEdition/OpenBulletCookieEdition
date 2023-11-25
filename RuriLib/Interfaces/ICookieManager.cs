using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RuriLib.Models;

namespace RuriLib.Interfaces
{
    /// <summary>
    /// Interface for a class that manages a collection of cookies.
    /// </summary>
    public interface ICookieManager
    {

        /// <summary>
        /// Adds a cookie to the collection.
        /// </summary>
        /// <param name="cookie">The cookie to add</param>
        void Add(Cookie cookie);

        /// <summary>
        /// The collection of available cookies.
        /// </summary>
        IEnumerable<Cookie> Cookies { get; }

        /// <summary>
        /// Updates a cookie.
        /// </summary>
        /// <param name="cookie">The updated cookie</param>
        void Update(Cookie cookie);

        /// <summary>
        /// Removes a given cookie from the collection.
        /// </summary>
        /// <param name="cookie">The wordlist to remove</param>
        void Remove(Cookie cookie);

        /// <summary>
        /// Deletes cookies that reference a missing file from the collection.
        /// </summary>
        void DeleteNotFound();

        /// <summary>
        /// Removes all cookies from the collection.
        /// </summary>
        void RemoveAll();

    }
}
