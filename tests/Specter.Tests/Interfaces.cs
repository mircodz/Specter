using Specter;

[assembly: GenerateMock(typeof(Specter.Tests.IEmailService))]
[assembly: GenerateMock(typeof(Specter.Tests.IUserRepository))]
[assembly: GenerateMock(typeof(Specter.Tests.IBaseService))]
[assembly: GenerateMock(typeof(Specter.Tests.IExtendedService))]

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
