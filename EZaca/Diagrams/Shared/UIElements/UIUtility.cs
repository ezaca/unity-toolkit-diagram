using System;
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
    }
}
