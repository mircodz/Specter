namespace Specter;

public class SetupBuilder<TReturn>(SetupEntry entry)
{
    public SetupBuilder<TReturn> Returns(TReturn value)
    {
        entry.ReturnFactory = _ => value;
        return this;
    }

    public SetupBuilder<TReturn> Returns(Func<TReturn> factory)
    {
        entry.ReturnFactory = _ => factory();
        return this;
    }

    public SetupBuilder<TReturn> Callback(Action callback)
    {
        entry.Callback = _ => callback();
        return this;
    }

    public SetupBuilder<TReturn> Callback(Action<object?[]> callback)
    {
        entry.Callback = callback;
        return this;
    }

    public void Throws<TException>() where TException : Exception, new()
        => entry.ThrowException = new TException();

    public void Throws(Exception ex)
        => entry.ThrowException = ex;
}

public class VoidSetupBuilder(SetupEntry entry)
{
    public VoidSetupBuilder Callback(Action callback)
    {
        entry.Callback = _ => callback();
        return this;
    }

    public VoidSetupBuilder Callback(Action<object?[]> callback)
    {
        entry.Callback = callback;
        return this;
    }

    public void Throws<TException>() where TException : Exception, new()
        => entry.ThrowException = new TException();

    public void Throws(Exception ex)
        => entry.ThrowException = ex;
}

public class SequenceSetupBuilder<TReturn>(SetupEntry entry)
{
    private readonly Queue<Func<object?>> _queue = entry.SequenceQueue = new();

    public SequenceSetupBuilder<TReturn> Returns(TReturn value)
    {
        _queue.Enqueue(() => value);
        return this;
    }

    public SequenceSetupBuilder<TReturn> Throws<TException>() where TException : Exception, new()
    {
        _queue.Enqueue(() => throw new TException());
        return this;
    }

    public SequenceSetupBuilder<TReturn> Throws(Exception ex)
    {
        _queue.Enqueue(() => throw ex);
        return this;
    }
}

public static class SetupBuilderExtensions
{
    public static SetupBuilder<Task<T>> ReturnsAsync<T>(this SetupBuilder<Task<T>> builder, T value)
        => builder.Returns(Task.FromResult(value));

    public static SetupBuilder<ValueTask<T>> ReturnsAsync<T>(this SetupBuilder<ValueTask<T>> builder, T value)
        => builder.Returns(new ValueTask<T>(value));
}