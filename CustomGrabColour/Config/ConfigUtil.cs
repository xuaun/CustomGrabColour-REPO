using UnityEngine;

namespace CustomGrabColour.Config;

public static class ConfigUtil
{
    public static string ColorToString(Color color)
    {
        return color.r + ", " + color.g + ", " + color.b + ", " + color.a;
    }

    public static Color StringToColor(string color, Color defaultColour)
    {
        string[] splitString = color.Split(',');
        switch (splitString.Length) {
            case 3:
            {
                float[] elements = new float[3];
                for (int i = 0; i < splitString.Length; i++)
                {
                    try
                    {
                        elements.SetValue(float.Parse(splitString[i].Trim()), i);
                    } catch
                    {
                        return defaultColour;
                    }
                }
                return new Color(elements[0], elements[1], elements[2], CustomGrabColourConfig.DefaultOpacity);
            }
            case 4:
            {
                float[] elements = new float[4];
                for (int i = 0; i < splitString.Length; i++)
                {
                    try
                    {
                        elements.SetValue(float.Parse(splitString[i].Trim()), i);
                    }
                    catch
                    {
                        return defaultColour;
                    }
                }
                return new Color(elements[0], elements[1], elements[2], elements[3]);
            }
            default:
                return defaultColour;
        }
    }
}