namespace MediatorSample.Domain.Struct;

/// <summary>
/// A struct that can be used to represent a void return type in generic interfaces and methods.
/// </summary>
public readonly struct Unit
{
    public static readonly Unit Value = new();
}