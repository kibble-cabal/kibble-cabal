using System;
using System.Collections.Generic;
using System.Linq;

public readonly struct Result<T, E>
{
    public class ExpectedSuccessException : System.Exception { }
    public class ExpectedErrorException : System.Exception { }

    private readonly bool _ok;
    public readonly T Value;
    public readonly E Error;

    private Result(T v, E e, bool success)
    {
        Value = v;
        Error = e;
        _ok = success;
    }

    public bool IsOk => _ok;

    public static Result<T, E> Ok(T v) => new(v, default(E), true);
    public static Result<T, E> Err(E e) => new(default(T), e, false);

    public static implicit operator Result<T, E>(T v) => new(v, default(E), true);
    public static implicit operator Result<T, E>(E e) => new(default(T), e, false);

    public R Match<R>(Func<T, R> ok, Func<E, R> error) => _ok ? ok(Value) : error(Error);

    public void Match(System.Action<T> ok, System.Action<E> error)
    {
        if (_ok) ok(Value);
        else error(Error);
    }

    public void MatchOK(System.Action<T> ok) => Match(ok, _ => { });
    public void MatchError(System.Action<E> error) => Match(_ => { }, error);

    public T AsSuccess()
    {
        if (!IsOk) throw new ExpectedSuccessException();
        return Value;
    }

    public E AsError()
    {
        if (IsOk) throw new ExpectedErrorException();
        return Error;
    }
}

public static class ResultExtensions
{
    public static IEnumerable<T> WhereOK<T, E>(this IEnumerable<Result<T, E>> results) => results.Where(result => result.IsOk).Select(result => result.AsSuccess());
    public static IEnumerable<E> WhereError<T, E>(this IEnumerable<Result<T, E>> results) => results.Where(result => !result.IsOk).Select(result => result.AsError());
}

public static class Result
{
    public static Result<T, E> FromException<T, E>(System.Func<T> onSuccess, System.Func<Exception, E> onError)
    {
        try { return onSuccess(); }
        catch (Exception exception)
        { return onError(exception); }
    }
}