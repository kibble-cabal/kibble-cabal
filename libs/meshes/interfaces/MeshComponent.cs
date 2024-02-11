
using System.Linq;

public interface IMeshComponent
{
    bool Invert { get; set; }
    int Surface { get; set; }

    Triangle[] GetTriangles();
}