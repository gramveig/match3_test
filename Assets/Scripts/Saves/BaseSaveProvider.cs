using System;
using Newtonsoft.Json;
using UnityEngine;
//must be installed first via Package Manager Window > Add Package from GIT URL > com.unity.nuget.newtonsoft-json

namespace Match3Test.Saves
{
    public abstract class BaseSaveProvider<TSave> where TSave : class, new()
    {
        private TSave _save;
        private readonly JsonSerializerSettings _serializerSettings;

        protected abstract string Key { get; }

        protected BaseSaveProvider()
        {
            _serializerSettings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Auto
            };
        }

        public TSave Read()
        {
            if (_save == null)
            {
                var json = PlayerPrefs.GetString(Key);

                if (string.IsNullOrEmpty(json))
                {
                    _save = DefaultSave();
                    Save(_save);
                }
                else
                    _save = JsonConvert.DeserializeObject<TSave>(json, _serializerSettings);
            }

            return _save;
        }

        public TSave ModifyAndSave(Action<TSave> handler)
        {
            var model = Read();
            handler?.Invoke(model);
            Save(model);

            return model;
        }

        protected abstract TSave DefaultSave();

        public void Save(TSave saveModel, bool flushChanges = true)
        {
            _save = saveModel;

            if (flushChanges)
            {
                FlushChanges();
            }
        }

        public void Save()
        {
            _save = Read();
            FlushChanges();
        }

        public void Reset()
        {
            Save(DefaultSave());
        }

        public string GetModelAsJsonString(TSave saveModel)
        {
            var json = JsonConvert.SerializeObject(saveModel, _serializerSettings);

            return json;
        }

        public TSave GetModelFromJsonString(string jsonString)
        {
            TSave model;
            try
            {
                model = JsonConvert.DeserializeObject<TSave>(jsonString, _serializerSettings);
            }
            catch
            {
                Debug.LogError("Unable to convert json string to valid model");
                model = DefaultSave();
            }

            return model;
        }

        private void FlushChanges()
        {
            var json = JsonConvert.SerializeObject(_save, _serializerSettings);
            PlayerPrefs.SetString(Key, json);
        }
    }
}