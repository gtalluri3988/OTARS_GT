using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentValidation;
using Microsoft.Practices.Unity;

namespace MTAoarsGeneral.Utilities.Mvc {

    public class UnityValidatorFactory : ValidatorFactoryBase {
        private readonly IUnityContainer _container;

        public UnityValidatorFactory(IUnityContainer container) {
            _container = container;
        }

        public override IValidator CreateInstance(Type validatorType) {
            if (_container.IsRegistered(validatorType)) {
                return _container.Resolve(validatorType) as IValidator;
            }
            return null;
        }
    }// class
    
}// namespace
