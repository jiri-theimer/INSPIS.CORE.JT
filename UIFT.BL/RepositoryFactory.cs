using System;

namespace UIFT.Repository
{
    public class RepositoryFactory
    {
        private readonly BL.Factory Factory;
        private readonly AppConfiguration Configuration;
        private Repository _repository;
        private int _a11id = 0;

        public RepositoryFactory(BL.Factory factory, AppConfiguration configuration)
        {
            this.Configuration = configuration;
            this.Factory = factory;
        }

        public string GetGlobalParams(string key)
        {
            return this.Factory.GlobalParams.LoadParam(key);
        }

        public Repository Get(int? a11id = null)
        {
            if ((_a11id != a11id && a11id.HasValue) || _repository == null)
            {
                _repository = new Repository(Factory, Configuration, a11id.GetValueOrDefault());
            }
            return _repository;
        }
    }
}