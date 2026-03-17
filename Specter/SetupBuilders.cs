using System;
using System.Collections.Generic;

namespace Specter;

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
