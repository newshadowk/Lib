// ReSharper disable CSharpWarnings::CS1591
using System;
using System.Collections;
using System.Collections.Generic;

namespace Lib.Base
{
    public sealed class WeakList<T> : IEnumerator<T>, IEnumerable<T>
    {
        #region Fields

        private readonly List<WeakReference> _list = new List<WeakReference>();
        private List<WeakReference>.Enumerator _innerEnumerator;

        #endregion Fields

        #region Properties Private

        object IEnumerator.Current
        {
            get { return ((IEnumerator<T>) this).Current; }
        }

        T IEnumerator<T>.Current
        {
            get
            {
                WeakReference weakReference = _innerEnumerator.Current;
                if (weakReference != null)
                    return (T) weakReference.Target;
                return default(T);
            }
        }

        #endregion Properties Private

        #region Methods Public

        public void Add(T item)
        {
            _list.Add(new WeakReference(item));
        }

        public void ClearDead()
        {
            List<WeakReference> deads = new List<WeakReference>();

            foreach (var wr in _list)
            {
                object target = wr.Target;
                if (target == null)
                    deads.Add(wr);
            }

            foreach (var dead in deads)
                _list.Remove(dead);
        }

        public T Find(Predicate<T> match)
        {
            foreach (var wr in _list)
            {
                object target = wr.Target;

                if (target == null)
                    continue;

                if (match((T) target))
                    return (T) target;
            }

            return default(T);
        }

        public List<T> FindAll(Predicate<T> match)
        {
            List<T> ret = new List<T>();
            foreach (var wr in _list)
            {
                object target = wr.Target;
                if (target == null)
                    continue;

                if (match((T) target))
                    ret.Add((T) target);
            }

            return ret;
        }

        public IEnumerator<T> GetEnumerator()
        {
            _innerEnumerator = _list.GetEnumerator();
            return this;
        }

        public void Dispose()
        {
            _innerEnumerator.Dispose();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        bool IEnumerator.MoveNext()
        {
            return _innerEnumerator.MoveNext();
        }

        void IEnumerator.Reset()
        {
            ((IEnumerator)_innerEnumerator).Reset();
        }

        #endregion Methods Public
    }
}