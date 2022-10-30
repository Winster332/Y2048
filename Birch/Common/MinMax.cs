namespace Birch.Common;

public class MinMax<T>
{
    public T? Min { get; private set; }
    public T? Max { get; private set; }

    public MinMax()
    {
        Min = default;
        Max = default;
    }

    public MinMax(T min, T max)
    {
        Min = min;
        Max = max;
    }

    public void SetMin(T? min) => Min = min;
    public void SetMax(T? max) => Max = max;

    public void Set(T? min, T? max)
    {
        SetMin(min);
        SetMax(max);
    }
}