using System;

namespace Specter;

public abstract class Mock<T>(bool strict = false, T? wrapping = null, Action<string>? onUnusedSetup = null) where T : class
{
    public T Instance => (T)(object)this;
    public MockInterceptor Interceptor { get; } = new(strict, wrapping, typeof(T), onUnusedSetup);

    public void Reset()
    {
        Interceptor.Reset();
        OnReset();
    }

    protected virtual void OnReset() { }

    public void CheckUnusedSetups() => Interceptor.CheckUnusedSetups();
}
