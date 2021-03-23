using Magician.Common.Util;
using System;
using System.Collections.Generic;

namespace Magician.Common.Emit
{
    public static class PropertyQuickerFactory
    {
        private static readonly ReflectPropertyQuicker ReflectPropertyQuicker = new ReflectPropertyQuicker();
        private static readonly PropertyQuickerEmitter PropertyQuickerEmitter = new PropertyQuickerEmitter(false);
        private static readonly Dictionary<Type, IPropertyQuicker> PropertyQuickerDic = new Dictionary<Type, IPropertyQuicker>();

        public static IPropertyQuicker<TEntity> CreatePropertyQuicker<TEntity>()
        {
            return (IPropertyQuicker<TEntity>)CreatePropertyQuicker(typeof(TEntity));
        }

        public static IPropertyQuicker CreatePropertyQuicker(Type entityType)
        {
            if (entityType.IsGenericType)
            {
                return ReflectPropertyQuicker;
            }

            lock (PropertyQuickerEmitter)
            {
                if (!PropertyQuickerDic.ContainsKey(entityType))
                {
                    Type propertyQuickerType = PropertyQuickerEmitter.CreatePropertyQuickerType(entityType);
                    //PropertyQuickerEmitter.Save();
                    IPropertyQuicker quicker = (IPropertyQuicker)Activator.CreateInstance(propertyQuickerType);
                    PropertyQuickerDic.Add(entityType, quicker);
                }

                return PropertyQuickerDic[entityType];
            }
        }
    }

    internal class ReflectPropertyQuicker : IPropertyQuicker
    {
        public object GetValue(object entity, string propertyName)
        {
            return ReflectionUtil.GetProperty(entity, propertyName);
        }

        public void SetValue(object entity, string propertyName, object propertyValue)
        {
            ReflectionUtil.SetProperty(entity, propertyName, propertyValue);
        }
    }

}