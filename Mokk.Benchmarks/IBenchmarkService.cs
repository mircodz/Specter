using System.Threading.Tasks;
using Mokk;
using Imposter.Abstractions;

// Mokk: generates MockBenchmarkService
[assembly: GenerateMock(typeof(Mokk.Benchmarks.IBenchmarkService))]
// Imposter: generates IBenchmarkServiceImposter
[assembly: GenerateImposter(typeof(Mokk.Benchmarks.IBenchmarkService))]

namespace Mokk.Benchmarks;

public interface IBenchmarkService
{
    bool Process(string input);
    string Transform(string input, int factor);
    Task<string> FetchAsync(int id);
    void Execute(int id);
}
