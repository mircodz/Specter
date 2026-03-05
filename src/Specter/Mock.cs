namespace Specter;

public abstract class Mock<T> where T : class
{
    public T Instance => (T)(object)this;
    public MockInterceptor Interceptor { get; } = new();
}