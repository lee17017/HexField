using UnityEngine;

public static class HexMetrics {
    public const float outerRadius = 20.0f;// = side length = a
    public const float innerRadius = outerRadius * 0.866025404f; // = outRadius * sqrt(3)/2
    public const float solidFactor = 0.75f;
    public const float blendFactor = 1f - solidFactor;

    public const float elevationStep = outerRadius/2f;
    public const int terracesPerSlope = 2;
    public const int terraceSteps = terracesPerSlope * 2 + 1;
    public const float horizontalTerraceStepSize = 1f / terraceSteps;
    public const float verticalTerraceStepSize = 1f / (terracesPerSlope + 1f);

    private static Vector3[] corners = {
        new Vector3(0f, 0f, outerRadius),
        new Vector3(innerRadius, 0f, 0.5f * outerRadius),
		new Vector3(innerRadius, 0f, -0.5f * outerRadius),
		new Vector3(0f, 0f, -outerRadius),
		new Vector3(-innerRadius, 0f, -0.5f * outerRadius),
		new Vector3(-innerRadius, 0f, 0.5f * outerRadius),
        new Vector3(0f, 0f, outerRadius)
    };

    public static Vector3 TerraceLerp(Vector3 a, Vector3 b, int step)
    {
        
        Vector3 d = b - a;
        float hStep = step * HexMetrics.horizontalTerraceStepSize;
        a.x += d.x * hStep;
        a.z += d.z * hStep;

        float vStep = (step + 1) / 2 * HexMetrics.verticalTerraceStepSize;
        a.y += d.y * vStep;
        return a;   
    }

    public static Color TerraceLerp(Color a, Color b, int step)
    {
        float hStep = step * HexMetrics.horizontalTerraceStepSize;
        return Color.Lerp(a, b, hStep);
    }

    public static HexEdgeType GetEdgeType(int elevation1, int elevation2)
    {
        int diff = Mathf.Abs(elevation1 - elevation2);
        switch (diff)
        {
            case 0: return HexEdgeType.Flat;

            case 1: return HexEdgeType.Slope;

            default: return HexEdgeType.Cliff;
        }
    }

    public static Vector3 GetFirstCorner(HexDirection direction)
    {
        return corners[(int)direction];
    }

    public static Vector3 GetSecondCorner(HexDirection direction)
    {
        return corners[(int)direction + 1];
    }

    public static Vector3 GetFirstSolidCorner(HexDirection direction)
    {
        return corners[(int)direction] * solidFactor;
    }

    public static Vector3 GetSecondSolidCorner(HexDirection direction)
    {
        return corners[(int)direction + 1] * solidFactor;
    }

    public static Vector3 GetBridge(HexDirection direction)
    {
        return (corners[(int)direction] + corners[(int)direction + 1]) * blendFactor;
    }
}
