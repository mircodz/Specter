using System.Threading.Tasks;
using Specter;
using Imposter.Abstractions;

// Specter: generates MockBenchmarkService
[assembly: GenerateMock(typeof(Specter.Benchmarks.IBenchmarkService))]
// Imposter: generates IBenchmarkServiceImposter
[assembly: GenerateImposter(typeof(Specter.Benchmarks.IBenchmarkService))]

namespace Specter.Benchmarks;

public interface IBenchmarkService
{
    bool Process(string input);
    string Transform(string input, int factor);
    Task<string> FetchAsync(int id);
    void Execute(int id);
}
