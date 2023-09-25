using System.Collections;

// 带结果的Coroutine
public class YieldableTask<T> : IEnumerator
{
    private bool isDone;
    public T Result;
    bool IEnumerator.MoveNext() => !isDone;

    void IEnumerator.Reset() { }
    object IEnumerator.Current => null;
    public void Finish(T result = default)
    {
        isDone = true;
        this.Result = result;
    }
}

public class Test
{

    IEnumerator t()
    {
        YieldableTask<string> c = new YieldableTask<string>();
        return c;
    }
}
