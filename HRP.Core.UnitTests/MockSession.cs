namespace HRP.Core.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;

    public class MockSession : ISession
    {
        private readonly Dictionary<string, dynamic> sessionFactory = new Dictionary<string, dynamic>();

        public bool IsAvailable => throw new NotImplementedException();
        public string Id => throw new NotImplementedException();
        public IEnumerable<string> Keys => throw new NotImplementedException();

        public void Clear()
        {
            sessionFactory.Clear();
        }

        public Task CommitAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task LoadAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public void Remove(string key)
        {
            sessionFactory.Remove(key);
        }

        public void Set(string key, byte[] value)
        {
            if (!sessionFactory.ContainsKey(key))
            {
                sessionFactory.Add(key, value);
            }
            else
            {
                sessionFactory[key] = value;
            }
        }

        public bool TryGetValue(string key, out byte[] value)
        {
            if (sessionFactory.ContainsKey(key) && sessionFactory[key] != null)
            {
                value = sessionFactory[key];
                return true;
            }
            else
            {
                value = null;
                return false;
            }
        }
    }
}