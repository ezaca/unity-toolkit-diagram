using EZaca.UIElements;
using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace EZaca.Diagrams
{
    public class BasicConnectionPaint : IConnectionPainter
    {
        public Gradient color;
        public float tickness;
        public ConnectionStyle style;
        public Direction direction;
        public float offset;

        public BasicConnectionPaint()
        {
            color = UIUtility.Gradient(GradientMode.Blend, (Color.gray2, 0f), (Color.gray2, 0.1f), (Color.white, 1f));
            tickness = 2f;
            offset = 36f;
        }

        public void PaintConnection(Painter2D painter, PortElement from, PortElement to)
        {
            PaintConnection(painter, from.center, to.center);
        }

        public void PaintConnection(Painter2D painter, Vector2 from, Vector2 to)
        {
            painter.lineCap = LineCap.Round;
            painter.lineJoin = LineJoin.Round;
            painter.lineWidth = tickness;
            painter.strokeGradient = color;

            Vector2 offset = CalculateOffsetVector();
            painter.BeginPath();
            painter.MoveTo(from);
            PaintConnection(painter, from, to, offset);
            painter.Stroke();
        }

        protected virtual void PaintConnection(Painter2D painter, Vector2 from, Vector2 to, Vector2 offset)
        {
            switch (style)
            {
                case ConnectionStyle.Line:
                    painter.LineTo(from + offset);
                    painter.LineTo(to - offset);
                    painter.LineTo(to);
                    break;
                case ConnectionStyle.Bezier:
                    painter.BezierCurveTo(from + offset, to - offset, to);
                    break;
                default:
                    throw new NotImplementedException(style.ToString());
            }
        }

        protected virtual Vector2 CalculateOffsetVector()
        {
            return direction switch
            {
                Direction.LeftToRight => new Vector2(offset, 0f),
                Direction.RightToLeft => new Vector2(-offset, 0f),
                Direction.TopDown => new Vector2(0f, offset),
                Direction.BottomUp => new Vector2(0f, -offset),
                _ => throw new NotImplementedException(direction.ToString()),
            };
        }

        public enum ConnectionStyle
        {
            Bezier,
            Line,
        }

        public enum Direction
        {
            LeftToRight,
            RightToLeft,
            TopDown,
            BottomUp,
        }
    }
}
