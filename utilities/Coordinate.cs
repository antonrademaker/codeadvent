namespace AoC.Utilities;
public readonly record struct Coordinate(int X, int Y)
{
    public static Coordinate OffsetLeft => new(-1, 0);
    public static Coordinate OffsetRight => new(1, 0);
    public static Coordinate OffsetUp => new(0, -1);
    public static Coordinate OffsetDown => new(0, 1);
    public static Coordinate OffsetUpLeft => new(-1, -1);
    public static Coordinate OffsetUpRight => new(1, -1);
    public static Coordinate OffsetDownLeft => new(-1, 1);
    public static Coordinate OffsetDownRight => new(1, 1);
    public static Coordinate operator +(Coordinate a, Coordinate b) => new(a.X + b.X, a.Y + b.Y);
    public static Coordinate operator +(Coordinate a, (int X, int Y) b) => new(a.X + b.X, a.Y + b.Y);
    public static Coordinate operator -(Coordinate a, Coordinate b) => new(a.X - b.X, a.Y - b.Y);
    public static Coordinate operator -(Coordinate a, (int X, int Y) b) => new(a.X - b.X, a.Y - b.Y);
    public static Coordinate operator *(Coordinate a, int multiplier) => new(a.X * multiplier, a.Y * multiplier);
    public Coordinate Left => new(X - 1, Y);
    public Coordinate Right => new(X + 1, Y);
    public Coordinate Up => new(X, Y - 1);
    public Coordinate Down => new(X, Y + 1);
    public Coordinate UpLeft => new(X - 1, Y - 1);
    public Coordinate UpRight => new(X + 1, Y - 1);
    public Coordinate DownLeft => new(X - 1, Y + 1);
    public Coordinate DownRight => new(X + 1, Y + 1);
    /// <summary>Horizontal & Vertical</summary>
    public Coordinate[] Neighbors => new[] { Left, Right, Up, Down };
    /// <summary>Horizontal, Vertical & Diagonal</summary>
    public Coordinate[] Adjacents => new[] { Left, Right, Up, Down, UpLeft, UpRight, DownLeft, DownRight };
    /// <summary>Adjacent or on top</summary>
    public bool IsAdjacent(Coordinate other) => Math.Abs(other.X - X) <= 1 && Math.Abs(other.Y - Y) <= 1;
    /// <summary>Calculate Manhattan Distance</summary>
    public int DistanceTo(Coordinate other) => Math.Abs(other.X - X) + Math.Abs(other.Y - Y);
    /// <summary>Calculate Offset/Direction to target</summary>
    public Coordinate OffsetTo(Coordinate other) => new((other.X - X == 0) ? 0 : (other.X - X < 0) ? -1 : 1, (other.Y - Y == 0) ? 0 : (other.Y - Y < 0) ? -1 : 1);
    public Coordinate RotateLeft => new(Y, -X);
    public Coordinate RotateRight => new(-Y, X);
    public override string ToString() => $"{X},{Y}";
}
