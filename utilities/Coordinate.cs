using System.Globalization;
using System.Numerics;

namespace AoC.Utilities;

public readonly record struct Coordinate<T>(T X, T Y) where T: INumber<T> { 

    public static Coordinate<T> OffsetLeft => new(-T.One, T.Zero);
    public static Coordinate<T> OffsetRight => new(T.One, T.Zero);
    public static Coordinate<T> OffsetUp => new(T.Zero, -T.One);
    public static Coordinate<T> OffsetDown => new(T.Zero, T.One);
    public static Coordinate<T> OffsetUpLeft => new(-T.One, -T.One);
    public static Coordinate<T> OffsetUpRight => new(T.One, -T.One);
    public static Coordinate<T> OffsetDownLeft => new(-T.One, T.One);
    public static Coordinate<T> OffsetDownRight => new(T.One, T.One);

    public static Coordinate<T>[] Offsets => [OffsetLeft, OffsetRight, OffsetUp, OffsetDown];

    public static Coordinate<T> operator +(Coordinate<T> a, Coordinate<T> b) => new(a.X + b.X, a.Y + b.Y);
    public static Coordinate<T> operator +(Coordinate<T> a, (T X, T Y) b) => new(a.X + b.X, a.Y + b.Y);
    public static Coordinate<T> operator -(Coordinate<T> a, Coordinate<T> b) => new(a.X - b.X, a.Y - b.Y);
    public static Coordinate<T> operator -(Coordinate<T> a, (T X, T Y) b) => new(a.X - b.X, a.Y - b.Y);
    public static Coordinate<T> operator *(Coordinate<T> a, T multiplier) => new(a.X * multiplier, a.Y * multiplier);
    public Coordinate<T> Left => new(X - T.One, Y);
    public Coordinate<T> Right => new(X + T.One, Y);
    public Coordinate<T> Up => new(X, Y - T.One);
    public Coordinate<T> Down => new(X, Y + T.One);
    public Coordinate<T> UpLeft => new(X - T.One, Y - T.One);
    public Coordinate<T> UpRight => new(X + T.One, Y - T.One);
    public Coordinate<T> DownLeft => new(X - T.One, Y + T.One);
    public Coordinate<T> DownRight => new(X + T.One, Y + T.One);
    /// <summary>Horizontal & Vertical</summary>
    public Coordinate<T>[] Neighbors => [Left, Right, Up, Down];
    /// <summary>Horizontal, Vertical & Diagonal</summary>
    public Coordinate<T>[] Adjacents => [Left, Right, Up, Down, UpLeft, UpRight, DownLeft, DownRight];
    /// <summary>Adjacent or on top</summary>
    public bool IsAdjacent(Coordinate<T> other) => T.Abs(other.X - X) <= T.One && T.Abs(other.Y - Y) <= T.One;
    /// <summary>Calculate Manhattan Distance</summary>
    public T DistanceTo(Coordinate<T> other) => T.Abs(other.X - X) + T.Abs(other.Y - Y);
    /// <summary>Calculate Offset/Direction to target</summary>
    public Coordinate<T> OffsetTo(Coordinate<T> other) => new((other.X - X == T.Zero) ? T.Zero : (other.X - X < T.Zero) ? -T.One : T.One, (other.Y - Y == T.Zero) ? T.Zero : (other.Y - Y < T.Zero) ? -T.One : T.One);
    public Coordinate<T> RotateLeft => new(Y, -X);
    public Coordinate<T> RotateRight => new(-Y, X);
    public override string ToString() => $"{X},{Y}";
}

public readonly record struct Coordinate3D<T>(T X, T Y, T Z) where T : INumber<T>
{
    public override string ToString() => $"{X},{Y},{Z}";

    public static Coordinate3D<T> Parse(string input)
    {
        var values = input.Split(',').Select(v => T.Parse(v, CultureInfo.InvariantCulture)).ToArray();
        
        return new Coordinate3D<T>(values[0], values[1], values[2]);
    }

    public static Coordinate3D<T> operator +(Coordinate3D<T> a, Coordinate3D<T> b) => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);

    public static Coordinate3D<T> operator +(Coordinate3D<T> a, (T X, T Y, T Z) b) => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);

}