using System.Numerics;

namespace Birch.Extensions;

public static class Vector2Extensions
{
    public static float Lerp(float av, float bv, float blend) => blend * (bv - av) + av;
    public static Vector2 Lerp(this Vector2 p, Vector2 v, float blend) => new(Lerp(p.X, v.X, blend), Lerp(p.Y, v.Y, blend));
    public static float Distance(this Vector2 a, Vector2 b) => Vector2.Distance(a, b);
    public static Vector2 Normalize(this Vector2 v) => Vector2.Normalize(v);

    public static Vector2[] RotatePoints(this Vector2[] pointsToRotate, Vector2 centerPoint, float angle)
    {
        return pointsToRotate.Select(point => RotatePoint(point, centerPoint, angle)).ToArray();
    }

    public static Vector2 RotatePoint(this Vector2 pointToRotate, Vector2 centerPoint, float angle)
    {
        var cosTheta = MathF.Cos(angle);
        var sinTheta = MathF.Sin(angle);

        return new Vector2
        {
            X =
                cosTheta * (pointToRotate.X - centerPoint.X) -
                sinTheta * (pointToRotate.Y - centerPoint.Y) + centerPoint.X,
            Y = sinTheta * (pointToRotate.X - centerPoint.X) +
                cosTheta * (pointToRotate.Y - centerPoint.Y) + centerPoint.Y
        };
    }

    public static bool IsPointOnLine(this Vector2 pt, Vector2 pt1, Vector2 pt2, double epsilon = 0.001)
    {
        if (pt.X - MathF.Max(pt1.X, pt2.X) > epsilon ||
            MathF.Min(pt1.X, pt2.X) - pt.X > epsilon ||
            pt.Y - MathF.Max(pt1.Y, pt2.Y) > epsilon ||
            MathF.Min(pt1.Y, pt2.Y) - pt.Y > epsilon)
            return false;

        if (MathF.Abs(pt2.X - pt1.X) < epsilon)
            return MathF.Abs(pt1.X - pt.X) < epsilon || MathF.Abs(pt2.X - pt.X) < epsilon;
        if (MathF.Abs(pt2.Y - pt1.Y) < epsilon)
            return MathF.Abs(pt1.Y - pt.Y) < epsilon || MathF.Abs(pt2.Y - pt.Y) < epsilon;

        var x = pt1.X + (pt.Y - pt1.Y) * (pt2.X - pt1.X) / (pt2.Y - pt1.Y);
        var y = pt1.Y + (pt.X - pt1.X) * (pt2.Y - pt1.Y) / (pt2.X - pt1.X);

        return MathF.Abs(pt.X - x) < epsilon || MathF.Abs(pt.Y - y) < epsilon;
    }

    public static bool ContainsInPolygon(this Vector2[] poly, Vector2 point) =>
        ContainsInPolygon(poly, point.X, point.Y);

    public static Vector2 IntersectionTwoLines(this Vector2[] lines)
    {
        var A = lines[0];
        var B = lines[1];
        var C = lines[2];
        var D = lines[3];

        float xo = A.X, yo = A.Y;
        float p = B.X - A.X, q = B.Y - A.Y;

        float x1 = C.X, y1 = C.Y;
        float p1 = D.X - C.X, q1 = D.Y - C.Y;

        float x = (xo * q * p1 - x1 * q1 * p - yo * p * p1 + y1 * p * p1) /
                  (q * p1 - q1 * p);
        float y = (yo * p * q1 - y1 * p1 * q - xo * q * q1 + x1 * q * q1) /
                  (p * q1 - p1 * q);

        return new Vector2(x, y);
    }

    public static bool IntersectWithPolygon(this Vector2[] polygonA, Vector2[] polygonB)
    {
        foreach (var pointA in polygonA)
        {
            if (polygonB.ContainsInPolygon(pointA))
            {
                return true;
            }
        }

        foreach (var pointB in polygonB)
        {
            if (polygonA.ContainsInPolygon(pointB))
            {
                return true;
            }
        }

        return false;
    }

    public static bool ContainsInPolygon(this Vector2[] poly, float x, float y)
    {
        var p = new Vector2(x, y);
        Vector2 p1, p2;
        bool inside = false;

        if (poly.Length < 3)
        {
            return inside;
        }

        var oldPoint = new Vector2(poly[poly.Length - 1].X, poly[poly.Length - 1].Y);

        for (int i = 0; i < poly.Length; i++)
        {
            var newPoint = new Vector2(poly[i].X, poly[i].Y);

            if (newPoint.X > oldPoint.X)
            {
                p1 = oldPoint;
                p2 = newPoint;
            }
            else
            {
                p1 = newPoint;
                p2 = oldPoint;
            }

            if ((newPoint.X < p.X) == (p.X <= oldPoint.X)
                && (p.Y - (long) p1.Y)*(p2.X - p1.X)
                < (p2.Y - (long) p1.Y)*(p.X - p1.X))
            {
                inside = !inside;
            }

            oldPoint = newPoint;
        }

        return inside;
    }
}
