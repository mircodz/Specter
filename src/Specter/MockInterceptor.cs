namespace Specter;

public class SetupEntry(string methodName, IMatcher[] matchers)
{
    public string MethodName { get; } = methodName;
    public IMatcher[] Matchers { get; } = matchers;
    public Func<object?[], object?>? ReturnFactory { get; set; }
    public Exception? ThrowException { get; set; }
    public Action<object?[]>? Callback { get; set; }
    public Queue<Func<object?>>? SequenceQueue { get; set; }

    public bool IsMatch(string method, object?[] args)
        => method == MethodName
        && Matchers.Length == args.Length
        && Matchers.Zip(args).All(p => p.First.Matches(p.Second));
}

public class MockInterceptor
{
    private readonly List<SetupEntry> _setups = [];
    private readonly List<(string Method, object?[] Args)> _calls = [];

    public SetupEntry AddSetup(string methodName, IMatcher[] matchers)
    {
        var entry = new SetupEntry(methodName, matchers);
        _setups.Add(entry);
        return entry;
    }

    public TReturn Intercept<TReturn>(string methodName, object?[] args)
    {
        _calls.Add((methodName, args));

        var setup = _setups.LastOrDefault(s => s.IsMatch(methodName, args));
        if (setup is null) return default!;

        setup.Callback?.Invoke(args);

        if (setup.ThrowException is not null)
            throw setup.ThrowException;

        if (setup.SequenceQueue is not null && setup.SequenceQueue.Count > 0)
        {
            var factory = setup.SequenceQueue.Dequeue();
            return (TReturn)factory()!;
        }

        return setup.ReturnFactory is not null
            ? (TReturn)setup.ReturnFactory(args)!
            : default!;
    }

    public void InterceptVoid(string methodName, object?[] args)
    {
        _calls.Add((methodName, args));

        var setup = _setups.LastOrDefault(s => s.IsMatch(methodName, args));
        if (setup is null) return;

        setup.Callback?.Invoke(args);

        if (setup.ThrowException is not null)
            throw setup.ThrowException;
    }

    public void Verify(string methodName, IMatcher[] matchers, int? exactly = null, int? atLeast = null)
    {
        int count = _calls.Count(c =>
            c.Method == methodName
            && matchers.Length == c.Args.Length
            && matchers.Zip(c.Args).All(p => p.First.Matches(p.Second)));

        string sig = $"{methodName}({string.Join(", ", matchers.Select(m => m.Describe()))})";

        if (exactly.HasValue && count != exactly.Value)
            throw new VerificationException(
                $"Verify failed: {sig} — expected exactly {exactly.Value} call(s), got {count}.");

        if (atLeast.HasValue && count < atLeast.Value)
            throw new VerificationException(
                $"Verify failed: {sig} — expected at least {atLeast.Value} call(s), got {count}.");
    }
}

public class VerificationException : Exception
{
    public VerificationException(string message) : base(message) { }
}