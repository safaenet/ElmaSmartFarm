﻿using System.Globalization;
using System.Windows.Media;
using System;

namespace ElmaSmartFarm.ClientWpf;
public static class Extensions
{
    public static string ToHex(this Color c) => $"#{c.A:X2}{c.R:X2}{c.G:X2}{c.B:X2}";
    public static Color ToColor(this string s)
    {
        if (long.TryParse(s.AsSpan(1), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out _))
            return (Color)ColorConverter.ConvertFromString(s);
        return Colors.Transparent;
    }
}