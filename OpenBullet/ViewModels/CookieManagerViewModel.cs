using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using OpenBulletCE.Repositories;
using RuriLib.Interfaces;
using RuriLib.Models;
using RuriLib.ViewModels;

namespace OpenBulletCE.ViewModels
{
    public class CookieManagerViewModel : ViewModelBase, ICookieManager
    {
        private LiteDBRepository<Cookie> _repo;

        private ObservableCollection<Cookie> cookiesCollection;
        public ObservableCollection<Cookie> CookiesCollection
        {
            get => cookiesCollection;
            private set
            {
                cookiesCollection = value;
                OnPropertyChanged();
            }
        }

        public int Total => CookiesCollection.Count;

        public IEnumerable<Cookie> Cookies => CookiesCollection;

        public CookieManagerViewModel()
        {
            _repo = new LiteDBRepository<Cookie>(OB.dataBaseFile, "cookies");
            CookiesCollection = new ObservableCollection<Cookie>();
            RefreshList();
        }

        #region Filters
        private string searchString = "";
        public string SearchString
        {
            get => searchString;
            set
            {
                searchString = value;
                OnPropertyChanged();
                CollectionViewSource.GetDefaultView(CookiesCollection).Refresh();
                OnPropertyChanged(nameof(Total));
            }
        }

        public void HookFilters()
        {
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(CookiesCollection);
            view.Filter = CookiesFilter;
        }

        private bool CookiesFilter(object item)
        {
            return (item as Cookie).Name.ToLower().Contains(searchString.ToLower());
        }
        #endregion
        public Cookie GetCookieByName(string name)
        {
            return CookiesCollection.Where(x => x.Name == name).First();
        }
        public static Cookie FileToCookielist(string path)
        {
            // Build the wordlist object
            var cookie = new Cookie(Path.GetFileNameWithoutExtension(path), path);

            return cookie;
        }
        #region CRUD Operations
        // Create
        public void Add(Cookie cookie)
        {
            if (CookiesCollection.Any(w => w.PathOwnerFolder == cookie.PathOwnerFolder))
            {
                throw new Exception($"Cookie already present: {cookie.PathOwnerFolder}");
            }

            CookiesCollection.Add(cookie);
            _repo.Add(cookie);
        }

        // Read
        public void RefreshList()
        {
            CookiesCollection = new ObservableCollection<Cookie>(_repo.Get());
            HookFilters();
        }

        // Update
        public void Update(Cookie cookie)
        {
            _repo.Update(cookie);
        }

        // Delete
        public void Remove(Cookie cookie)
        {
            CookiesCollection.Remove(cookie);
            _repo.Remove(cookie);
        }

        public void RemoveAll()
        {
            CookiesCollection.Clear();
            _repo.RemoveAll();
        }
        #endregion

        #region Delete methods
        public void DeleteNotFound()
        {
            foreach (var cookie in CookiesCollection.ToList())
            {
                if (!Directory.Exists(cookie.PathOwnerFolder))
                {
                    Remove(cookie);

                }
            }
        }
        #endregion
    }
}
