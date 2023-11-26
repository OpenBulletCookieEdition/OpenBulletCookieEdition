using LiteDB;
using OpenBulletCE.Repositories;
using OpenBulletCE.Views.Main.Runner;
using RuriLib;
using RuriLib.Interfaces;
using RuriLib.Models;
using RuriLib.Runner;
using RuriLib.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace OpenBulletCE.ViewModels
{
    public class RunnerManagerViewModel : ViewModelBase, IRunnerManager
    {
        private LiteDBRepository<RunnerSessionData> _repo;

        public ObservableCollection<RunnerInstance> RunnersCollection { get; set; } = new ObservableCollection<RunnerInstance>();

        public IEnumerable<IRunner> Runners => RunnersCollection.Select(i => i.ViewModel);

        private Random rand = new Random();

        public RunnerManagerViewModel()
        {
            _repo = new LiteDBRepository<RunnerSessionData>(OB.dataBaseFile, "runners");
        }

        public RunnerInstance Get(int id)
        {
            return RunnersCollection.Where(r => r.Id == id).First();
        }

        public IRunner Create()
        {
            var instance = new RunnerInstance(rand.Next());
            instance.ViewModel.ConfigChanged += OnRunnerSessionChanged;
            instance.ViewModel.ListChanged += OnRunnerSessionChanged;
            RunnersCollection.Add(instance);
            return instance.ViewModel;
        }

        public void Remove(IRunner runner)
        {
            RunnersCollection.Remove(RunnersCollection.First(r => r.ViewModel == runner));
        }

        public void Remove(int id)
        {
            RunnersCollection.Remove(Get(id));
        }

        public void RemoveAll()
        {
            RunnersCollection.Clear();
        }

        public void OnRunnerSessionChanged(IRunnerMessaging obj)
        {
            SaveSession();
        }

        public void SaveSession()
        {
            _repo.RemoveAll();
            _repo.Add(RunnersCollection
                .Select(r => r.ViewModel)
                .Select(r => new RunnerSessionData()
            {
                Bots = r.BotsAmount,
                Config = r.Config != null ? r.ConfigName : "",
                Wordlist = r.Wordlist != null ? r.Wordlist.Path : "",
                Cookielist = r.Cookielist != null ? r.Cookielist.PathOwnerFolder : "",
                ProxyMode = r.ProxyMode
            }
            ));
        }

        // Returns true if a session was found
        public bool RestoreSession()
        {
            var runners = _repo.Get().ToArray();
            
            if (runners.Length == 0) return false;

            foreach (var r in runners)
            {
                try
                {
                    var instance = Create();
                    instance.BotsAmount = r.Bots;
                    instance.ProxyMode = r.ProxyMode;

                    var configVM = OB.ConfigManager.Configs.FirstOrDefault(c => c.Name == r.Config)
                        ?? throw new Exception($"The Config {r.Config} was not found in the ConfigManager");

                    instance.SetConfig(configVM.Config, false);

                    switch (configVM.Type)
                    {
                        case RuriLib.Enums.ConfigType.Default:
                            // Try to get the Wordlist from the Manager
                            var wordlist = OB.WordlistManager.Wordlists.FirstOrDefault(w => w.Path == r.Wordlist);

                            // If not found, try to get it from disk
                            if (wordlist == null)
                            {
                                if (!File.Exists(r.Wordlist))
                                    throw new Exception($"The Wordlist {r.Wordlist} was not found in the WordlistManager or on Disk");

                                wordlist = WordlistManagerViewModel.FileToWordlist(r.Wordlist);
                            }

                            // Set the Wordlist
                            instance.SetWordlist(wordlist);

                            // Retrieve the record from the DB
                            instance.StartingPoint = RetrieveRecord(configVM.Config, wordlist);
                            break;
                        case RuriLib.Enums.ConfigType.CookieEdition:

                            // Try to get the Cookie list from the Manager
                            var cookielist = OB.CookieManager.Cookies.FirstOrDefault(w => w.PathOwnerFolder == r.Cookielist);

                            // If not found, try to get it from disk
                            if (cookielist == null)
                            {
                                if (!File.Exists(r.Cookielist))
                                    throw new Exception($"The Cookie list {r.Cookielist} was not found in the WordlistManager or on Disk");

                                cookielist = CookieManagerViewModel.FileToCookielist(r.Cookielist);
                            }

                            // Set the Cookielist
                            instance.SetCookielist(cookielist);

                            // Retrieve the record from the DB
                            instance.StartingPoint = RetrieveRecord(configVM.Config, cookielist);
                            break;

                    }
                    
                }
                catch (Exception ex)
                {
                    OB.Logger.LogError(Components.RunnerManager, ex.Message);
                }
            }

            return true;
        }

        public int RetrieveRecord(Config config, Wordlist wordlist)
        {
            if (wordlist == null || config == null)
            {
                return 1;
            }

            using (var db = new LiteDatabase(OB.dataBaseFile))
            {
                var record = db.GetCollection<Record>("records").FindOne(r => r.ConfigName == config.Settings.Name && r.WordlistLocation == wordlist.Path);
                if (record != null)
                {
                    return record.Checkpoint;
                }
                else
                {
                    return 1;
                }
            }
        }

        public int RetrieveRecord(Config config, Cookie cookielist)
        {
            if (cookielist == null || config == null)
            {
                return 1;
            }

            using (var db = new LiteDatabase(OB.dataBaseFile))
            {
                var record = db.GetCollection<Record>("records").FindOne(r => r.ConfigName == config.Settings.Name && r.CookielistLocation == cookielist.PathOwnerFolder);
                if (record != null)
                {
                    return record.Checkpoint;
                }
                else
                {
                    return 1;
                }
            }
        }

        public void SaveRecord(Config config, Wordlist wordlist, int progress)
        {
            if (config == null || wordlist == null)
            {
                return;
            }

            using (var db = new LiteDatabase(OB.dataBaseFile))
            {
                var coll = db.GetCollection<Record>("records");
                var record = new Record(config.Settings.Name, wordlist.Path, "", progress);

                coll.Delete(r => r.ConfigName == config.Settings.Name && r.WordlistLocation == wordlist.Path);
                coll.Insert(record);
            }
        }

        public void SaveRecord(Config config, Cookie cookielist, int progress)
        {
            if (config == null || cookielist == null)
            {
                return;
            }

            using (var db = new LiteDatabase(OB.dataBaseFile))
            {
                var coll = db.GetCollection<Record>("records");
                var record = new Record(config.Settings.Name, "", cookielist.PathOwnerFolder, progress);

                coll.Delete(r => r.ConfigName == config.Settings.Name && r.CookielistLocation == cookielist.PathOwnerFolder);
                coll.Insert(record);
            }
        }
    }

    public class RunnerInstance
    {
        // TODO: Remove the View from here! It shouldn't be here and nothing should reference this View!
        public Runner View { get; private set; }
        public RunnerViewModel ViewModel { get; private set; }

        public int Id { get; set; }

        public RunnerInstance(int id)
        {
            Id = id;
            ViewModel = new RunnerViewModel(OB.Settings.Environment, OB.Settings.RLSettings);
            View = new Runner(ViewModel);
        }
    }
}
