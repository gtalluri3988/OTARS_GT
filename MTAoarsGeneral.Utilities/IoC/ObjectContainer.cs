using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity;
using AutoMapper;




namespace MTAoarsGeneral.Utilities.IoC {
    
    public class ObjectContainer {
        static ObjectContainer instance;
        IUnityContainer container = null;

        private ObjectContainer() {
            container = new UnityContainer();
        }

        public static IUnityContainer Container {
            get {
                if (instance == null) instance = new ObjectContainer();
                return instance.container;
            }
        }

       

       

        

    }// class

}// namespace
