using System;
using System.Collections.Generic;
using System.Linq;

public readonly struct Result<T, E>
{
    public class ExpectedSuccessException : Exception { }
    public class ExpectedErrorException : Exception { }

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

    public static Result<T, E> Ok(T v) => new(v, default!, true);
    public static Result<T, E> Err(E e) => new(default!, e, false);

    public static implicit operator Result<T, E>(T v) => new(v, default!, true);
    public static implicit operator Result<T, E>(E e) => new(default!, e, false);

    public R Match<R>(Func<T, R> ok, Func<E, R> error) => _ok ? ok(Value) : error(Error);

    public void Match(Action<T> ok, Action<E> error)
    {
        if (_ok) ok(Value);
        else error(Error);
    }

    public void MatchOK(Action<T> ok) => Match(ok, _ => { });
    public void MatchError(Action<E> error) => Match(_ => { }, error);

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
    public static Result<T, E> FromException<T, E>(Func<T> onSuccess, Func<Exception, E> onError)
    {
        try { return onSuccess(); }
        catch (Exception exception)
        { return onError(exception); }
    }
    public static Result<bool, E> FromException<E>(Action onSuccess, Func<Exception, E> onError) => FromException<bool, E>(() => { onSuccess(); return true; }, onError);
}