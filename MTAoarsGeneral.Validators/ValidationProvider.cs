using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Microsoft.Practices.Unity;
using MTAoarsGeneral.Utilities.IoC;

namespace MTAoarsGeneral.Validators {

    public class ValidationProvider : IValidationProvider {

        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        IDictionary<Type, IList<dynamic>> validators;

        public ValidationProvider() {
            validators = new Dictionary<Type, IList<dynamic>>();
            Populate();
        }

        public IEnumerable<IValidator<T>> GetValidators<T>(T item) {
            var result = new List<IValidator<T>>();
            var type = typeof(T);
            while (type != null && type.Name != "Object") {
                result.AddRange(SelectValidators<T>(type));
                foreach (var intr in type.GetInterfaces()) {
                    result.AddRange(SelectValidators<T>(intr));
                }
                type = type.BaseType;
            }
            logger.Info("[RegistrationValidation] Selected {0} validator(s) for model type {1}.", result.Count, typeof(T).FullName);
            //return result;
            foreach (var validator in result) {
                yield return ObjectContainer.Container.Resolve(validator.GetType()) as IValidator<T>;
            }
        }

        IEnumerable<IValidator<T>> SelectValidators<T>(Type type) {
            IList<dynamic> list;
            if (validators.TryGetValue(type, out list)) {
               
                return list.Cast<IValidator<T>>().AsEnumerable();
            }
            return new List<IValidator<T>>();
        }

        void Populate() {
            // GetCallingAssembly is affected by runtime inlining and can return a
            // caller such as the Services assembly under IIS. Always scan the
            // assembly that contains ValidationProvider and all custom validators.
            var validatorAssembly = typeof(ValidationProvider).Assembly;
            logger.Info("[RegistrationValidation] Discovering validators from {0}; Location={1}",
                validatorAssembly.FullName, validatorAssembly.Location);
            var list = validatorAssembly.GetTypes()
                 .Where(
                         type => type.GetInterfaces().Any(
                                     iType => iType.IsGenericType && iType.GetGenericTypeDefinition() == typeof(IValidator<>)
                                     )
                     ).ToList();
            logger.Info("[RegistrationValidation] Discovered {0} validator(s) in {1}.", list.Count, validatorAssembly.FullName);
            foreach (Type type in list) {
                Add(type);
            }
        }

        void Add(Type type) {
            var interfaceType = type.GetInterfaces().Single(
                  iType => iType.IsGenericType
                      && iType.GetGenericTypeDefinition() == typeof(IValidator<>)
                      );
            Type genericType = interfaceType.GetGenericArguments().Single();
            IList<dynamic> list = null;
            if (!validators.TryGetValue(genericType, out list)) {
                list = new List<dynamic>();
                validators.Add(genericType, list);
            }
            (list as List<dynamic>).Add(ObjectContainer.Container.Resolve(type));
        }

    }// class

}// namespace
