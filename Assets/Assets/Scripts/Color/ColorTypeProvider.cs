using UnityEngine;

public class ColorTypeProvider : MonoBehaviour
{
    private static Color[] colorMap = new Color[]
    {
        new Color(0.0f, 0.5f, 1.0f),    // Blue
        new Color(1.0f, 0.0f, 0.0f),    // Red
        new Color(0.0f, 1.0f, 0.0f),    // Green
        new Color(1.0f, 1.0f, 0.0f),    // Yellow
        new Color(0.5f, 0.0f, 1.0f),    // Purple
        new Color(1.0f, 0.5f, 0.0f)     // Orange
    };

    public static Color GetColor(ColorType colorType)
    {
        int index = (int)colorType;
        
        if (index >= 0 && index < colorMap.Length)
        {
            return colorMap[index];
        }
        
        Debug.LogWarning($"ColorType {colorType} index {index} is out of range. Returning white.");
        return Color.white;
    }
}
