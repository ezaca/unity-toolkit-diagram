using System;
using System.Linq;
using UnityEngine;

namespace EZaca.UIElements
{
    public static class UIUtility
    {
        public static Gradient Gradient(GradientMode mode, params (Color color, float t)[] keys)
        {
            return new Gradient()
            {
                mode = mode,
                colorKeys = Array.ConvertAll(keys, key => new GradientColorKey(key.color, key.t)),
                alphaKeys = Array.ConvertAll(keys, key => new GradientAlphaKey(key.color.a, key.t)),
            };
        }

        public static Gradient Gradient(GradientMode mode, params Color[] colors)
        {
            return new Gradient()
            {
                mode = mode,
                colorKeys = Enumerable.Range(0, colors.Length).Select(i => new GradientColorKey(colors[i], i / (colors.Length - 1))).ToArray(),
                alphaKeys = Enumerable.Range(0, colors.Length).Select(i => new GradientAlphaKey(colors[i].a, i / (colors.Length - 1))).ToArray(),
            };
        }

        public static Gradient Gradient(Color color)
        {
            return new Gradient()
            {
                mode = GradientMode.Fixed,
                colorKeys = new[] { new GradientColorKey(color, 0f), new GradientColorKey(color, 1f) },
                alphaKeys = new[] { new GradientAlphaKey(1f, 1f), new GradientAlphaKey(1f, 1f) },
            };
        }
    }
}
