using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.Services.Shared;
using MTAoarsGeneral.Builders.Interfaces;

namespace MTAoarsGeneral.Builders.Shared {
    public class BaseBuilder : IBaseBuilder
    {
        protected static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        BuilderContext context;
        public BaseBuilder()
        {
            context = new BuilderContext();
        }

        public BuilderContext CurrentContext
        {
            get { return context; }
            set { context = value; }
        }

    }// class
}// namespace
