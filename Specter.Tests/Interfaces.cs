using System.Threading.Tasks;
using Specter;

[assembly: GenerateMock(typeof(Specter.Tests.IEmailService))]
[assembly: GenerateMock(typeof(Specter.Tests.IUserRepository))]
[assembly: GenerateMock(typeof(Specter.Tests.IBaseService))]
[assembly: GenerateMock(typeof(Specter.Tests.IExtendedService))]
[assembly: GenerateMock(typeof(Specter.Tests.ITemplatedService))]
[assembly: GenerateMock(typeof(Specter.Tests.AbstractNotificationService))]

namespace Specter.Tests;

public interface IEmailService
{
    bool Send(string to, string subject);
    string GetTemplate(string name, int version);
}

public interface IUserRepository
{
    string Name { get; set; }
    int Age { get; }
    Task<string> GetUserAsync(int id);
    ValueTask<int> CountAsync();
    void Delete(int id);
}

public interface IBaseService
{
    string GetName();
}

public interface IExtendedService : IBaseService
{
    int GetCount();
}

public interface ITemplatedService
{
    T DoSomething<T>(T value);
}

// Real implementation used by wrapping tests
public class RealEmailService : IEmailService
{
    public bool Send(string to, string subject) => true;
    public string GetTemplate(string name, int version) => $"real:{name}-v{version}";
}

// Abstract class used to test abstract class mock generation
public abstract class AbstractNotificationService
{
    public abstract bool Notify(string recipient, string message);
    public abstract string GetStatus(int id);
    public virtual string ServiceName => "base";
    protected abstract void Log(string entry);
}