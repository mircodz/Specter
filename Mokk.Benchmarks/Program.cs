using BenchmarkDotNet.Running;

// Run all benchmark classes
BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).RunAll();
