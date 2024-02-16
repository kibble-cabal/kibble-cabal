using System;

public interface IIdentifiable<I> where I : notnull, IEquatable<I>
{
    I ID { get; set; }
}