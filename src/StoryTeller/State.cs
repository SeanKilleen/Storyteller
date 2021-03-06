using System;
using FubuCore.Util;

namespace StoryTeller
{
    // SAMPLE: IState
    public interface IState
    {
        void Store<T>(T value);
        void Store<T>(string key, T value);
        T Retrieve<T>();
        T Retrieve<T>(string key);
    }
    // ENDSAMPLE

    public class State : IDisposable, IState
    {
        // TODO -- replace w/ the lightweight cache from StructureMap
        private readonly Cache<Type, object> _byType = new Cache<Type, object>();
        private readonly Cache<Type, Cache<string, object>> _byName = new Cache<Type, Cache<string, object>>(t => new Cache<string, object>()); 

        public void Store<T>(T value)
        {
            _byType[typeof (T)] = value;
        }

        public void Store<T>(string key, T value)
        {
            _byName[typeof (T)][key] = value;
        }

        public T Retrieve<T>()
        {
            return (T) _byType[typeof (T)];
        }

        public T Retrieve<T>(string key)
        {
            return (T) _byName[typeof (T)][key];
        }

        public object CurrentObject;

        void IDisposable.Dispose()
        {
            _byName.ClearAll();
            _byType.ClearAll();
        }
    }
}