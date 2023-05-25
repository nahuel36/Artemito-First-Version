using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextTimeCalculator : ITextTimeCalculator
{
    public TextTimeCalculator()
    {
    }

    public float CalculateTime(string text)
    {
        return 1 + text.Length * 0.075f;
    }
}
