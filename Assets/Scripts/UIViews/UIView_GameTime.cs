using System;
using TMPro;
using UnityEngine;

public class UIView_GameTime : MonoBehaviour
{
    public TMP_Text Text;

    public void Init(int totalDays)
    {
        var months = (int)MathF.Ceiling(totalDays / 28f);
        var week = (int)MathF.Ceiling((totalDays / 7f) % 4);
        week = week != 0 ? week : 4;
        var day = totalDays % 7 != 0 ? totalDays % 7 : 7;
        Text.text = $"[Month: {months}, Week: {week}, Day: {day}";
    }
}
