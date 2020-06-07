﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Core2D;
using Core2D.Path;
using Core2D.Path.Segments;
using Core2D.Shapes;
using SkiaSharp;
using Spatial;
using Spatial.Arc;

namespace Core2D.Renderer.SkiaSharp
{
    public static class PathGeometryConverter
    {
        public static void CreateFigure(this IPathFigure pathFigure, Func<double, float> scale, SKPath path)
        {
            path.MoveTo(
                scale(pathFigure.StartPoint.X),
                scale(pathFigure.StartPoint.Y));

            foreach (var segment in pathFigure.Segments)
            {
                if (segment is ILineSegment lineSegment)
                {
                    path.LineTo(
                        scale(lineSegment.Point.X),
                        scale(lineSegment.Point.Y));
                }
                else if (segment is IQuadraticBezierSegment quadraticBezierSegment)
                {
                    path.QuadTo(
                        scale(quadraticBezierSegment.Point1.X),
                        scale(quadraticBezierSegment.Point1.Y),
                        scale(quadraticBezierSegment.Point2.X),
                        scale(quadraticBezierSegment.Point2.Y));
                }
                else if (segment is ICubicBezierSegment cubicBezierSegment)
                {
                    path.CubicTo(
                        scale(cubicBezierSegment.Point1.X),
                        scale(cubicBezierSegment.Point1.Y),
                        scale(cubicBezierSegment.Point2.X),
                        scale(cubicBezierSegment.Point2.Y),
                        scale(cubicBezierSegment.Point3.X),
                        scale(cubicBezierSegment.Point3.Y));
                }
                else if(segment is IArcSegment arcSegment)
                {
                    path.ArcTo(
                        scale(arcSegment.Size.Width),
                        scale(arcSegment.Size.Height),
                        (float)arcSegment.RotationAngle,
                        arcSegment.IsLargeArc ? SKPathArcSize.Large : SKPathArcSize.Small,
                        arcSegment.SweepDirection == SweepDirection.Clockwise ? SKPathDirection.Clockwise : SKPathDirection.CounterClockwise,
                        scale(arcSegment.Point.X),
                        scale(arcSegment.Point.Y));
                }
                else
                {
                    throw new NotSupportedException("Not supported segment type: " + segment.GetType());
                }
            }

            if (pathFigure.IsClosed)
            {
                path.Close();
            }
        }

        public static IPathGeometry ToPathGeometry(SKPath path, double dx, double dy, IFactory factory)
        {
            var geometry = factory.CreatePathGeometry(
                ImmutableArray.Create<IPathFigure>(),
                path.FillType == SKPathFillType.EvenOdd ? FillRule.EvenOdd : FillRule.Nonzero);

            var context = factory.CreateGeometryContext(geometry);

            using (var iterator = path.CreateRawIterator())
            {
                var points = new SKPoint[4];
                var pathVerb = SKPathVerb.Move;

                while ((pathVerb = iterator.Next(points)) != SKPathVerb.Done)
                {
                    switch (pathVerb)
                    {
                        case SKPathVerb.Move:
                            {
                                context.BeginFigure(
                                    factory.CreatePointShape(points[0].X, points[0].Y),
                                    false,
                                    false);
                            }
                            break;
                        case SKPathVerb.Line:
                            {
                                context.LineTo(
                                    factory.CreatePointShape(points[1].X, points[1].Y));
                            }
                            break;
                        case SKPathVerb.Cubic:
                            {
                                context.CubicBezierTo(
                                    factory.CreatePointShape(points[1].X, points[1].Y),
                                    factory.CreatePointShape(points[2].X, points[2].Y),
                                    factory.CreatePointShape(points[3].X, points[3].Y));
                            }
                            break;
                        case SKPathVerb.Quad:
                            {
                                context.QuadraticBezierTo(
                                    factory.CreatePointShape(points[1].X, points[1].Y),
                                    factory.CreatePointShape(points[2].X, points[2].Y));
                            }
                            break;
                        case SKPathVerb.Conic:
                            {
                                var quads = SKPath.ConvertConicToQuads(points[0], points[1], points[2], iterator.ConicWeight(), 1);
                                context.QuadraticBezierTo(
                                    factory.CreatePointShape(quads[1].X, quads[1].Y),
                                    factory.CreatePointShape(quads[2].X, quads[2].Y));
                                context.QuadraticBezierTo(
                                    factory.CreatePointShape(quads[3].X, quads[3].Y),
                                    factory.CreatePointShape(quads[4].X, quads[4].Y));
                            }
                            break;
                        case SKPathVerb.Close:
                            {
                                context.SetClosedState(true);
                            }
                            break;
                    }
                }
            }

            return geometry;
        }

        public static SKPath ToSKPath(this IEnumerable<IBaseShape> shapes, double dx, double dy, Func<double, float> scale)
        {
            var path = new SKPath
            {
                FillType = SKPathFillType.Winding
            };
            var previous = default(IPointShape);
            foreach (var shape in shapes)
            {
                switch (shape)
                {
                    case ILineShape lineShape:
                        {
                            if (previous == null || previous != lineShape.Start)
                            {
                                path.MoveTo(
                                    scale(lineShape.Start.X),
                                    scale(lineShape.Start.Y));
                            }
                            path.LineTo(
                                scale(lineShape.End.X),
                                scale(lineShape.End.Y));
                            previous = lineShape.End;
                        }
                        break;
                    case IRectangleShape rectangleShape:
                        {
                            path.AddRect(
                                SkiaSharpRenderer.CreateRect(rectangleShape.TopLeft, rectangleShape.BottomRight, scale),
                                SKPathDirection.Clockwise);
                        }
                        break;
                    case IEllipseShape ellipseShape:
                        {
                            path.AddOval(
                                SkiaSharpRenderer.CreateRect(ellipseShape.TopLeft, ellipseShape.BottomRight, scale),
                                SKPathDirection.Clockwise);
                        }
                        break;
                    case IArcShape arcShape:
                        {
                            var a = new GdiArc(
                                Point2.FromXY(arcShape.Point1.X, arcShape.Point1.Y),
                                Point2.FromXY(arcShape.Point2.X, arcShape.Point2.Y),
                                Point2.FromXY(arcShape.Point3.X, arcShape.Point3.Y),
                                Point2.FromXY(arcShape.Point4.X, arcShape.Point4.Y));
                            var rect = new SKRect(
                                scale(a.X),
                                scale(a.Y),
                                scale(a.X + a.Width),
                                scale(a.Y + a.Height));
                            path.AddArc(rect, (float)a.StartAngle, (float)a.SweepAngle);
                        }
                        break;
                    case ICubicBezierShape cubicBezierShape:
                        {
                            if (previous == null || previous != cubicBezierShape.Point1)
                            {
                                path.MoveTo(
                                    scale(cubicBezierShape.Point1.X),
                                    scale(cubicBezierShape.Point1.Y));
                            }
                            path.CubicTo(
                                scale(cubicBezierShape.Point2.X),
                                scale(cubicBezierShape.Point2.Y),
                                scale(cubicBezierShape.Point3.X),
                                scale(cubicBezierShape.Point3.Y),
                                scale(cubicBezierShape.Point4.X),
                                scale(cubicBezierShape.Point4.Y));
                            previous = cubicBezierShape.Point4;
                        }
                        break;
                    case IQuadraticBezierShape quadraticBezierShape:
                        {
                            if (previous == null || previous != quadraticBezierShape.Point1)
                            {
                                path.MoveTo(
                                    scale(quadraticBezierShape.Point1.X),
                                    scale(quadraticBezierShape.Point1.Y));
                            }
                            path.QuadTo(
                                scale(quadraticBezierShape.Point2.X),
                                scale(quadraticBezierShape.Point2.Y),
                                scale(quadraticBezierShape.Point3.X),
                                scale(quadraticBezierShape.Point3.Y));
                            previous = quadraticBezierShape.Point3;
                        }
                        break;
                    case ITextShape textShape:
                        {
                            var resultPath = ToSKPath(textShape, dx, dy, scale);
                            if (resultPath != null && !resultPath.IsEmpty)
                            {
                                path.AddPath(resultPath, SKPathAddMode.Append);
                            }
                        }
                        break;
                    case IPathShape pathShape:
                        {
                            var resultPath = ToSKPath(pathShape, dx, dy, scale);
                            if (resultPath != null && !resultPath.IsEmpty)
                            {
                                path.AddPath(resultPath, SKPathAddMode.Append);
                            }
                        }
                        break;
                    case IGroupShape groupShape:
                        {
                            var resultPath = ToSKPath(groupShape.Shapes, dx, dy, scale);
                            if (resultPath != null && !resultPath.IsEmpty)
                            {
                                path.AddPath(resultPath, SKPathAddMode.Append);
                            }
                        }
                        break;
                }
            }
            return path;
        }

        public static SKPath ToSKPath(this IBaseShape shape, double dx, double dy, Func<double, float> scale)
        {
            return shape switch
            {
                ILineShape lineShape => ToSKPath(lineShape, dx, dy, scale),
                IRectangleShape rectangleShape => ToSKPath(rectangleShape, dx, dy, scale),
                IEllipseShape ellipseShape => ToSKPath(ellipseShape, dx, dy, scale),
                IImageShape imageShape => ToSKPath(imageShape, dx, dy, scale),
                IArcShape arcShape => ToSKPath(arcShape, dx, dy, scale),
                ICubicBezierShape cubicBezierShape => ToSKPath(cubicBezierShape, dx, dy, scale),
                IQuadraticBezierShape quadraticBezierShape => ToSKPath(quadraticBezierShape, dx, dy, scale),
                ITextShape textShape => ToSKPath(textShape, dx, dy, scale),
                IPathShape pathShape => ToSKPath(pathShape, dx, dy, scale),
                IGroupShape groupShape => ToSKPath(groupShape.Shapes, dx, dy, scale),
                _ => null,
            };
        }

        public static SKPath ToSKPath(this ILineShape line, double dx, double dy, Func<double, float> scale)
        {
            var path = new SKPath
            {
                FillType = SKPathFillType.Winding
            };
            path.MoveTo(
                scale(line.Start.X),
                scale(line.Start.Y));
            path.LineTo(
                scale(line.End.X),
                scale(line.End.Y));
            return path;
        }

        public static SKPath ToSKPath(this IRectangleShape rectangle, double dx, double dy, Func<double, float> scale)
        {
            var path = new SKPath
            {
                FillType = SKPathFillType.Winding
            };
            path.AddRect(
                SkiaSharpRenderer.CreateRect(rectangle.TopLeft, rectangle.BottomRight, scale),
                SKPathDirection.Clockwise);
            if (rectangle is ITextShape text)
            {
                var textPath = ToSKPath(text, dx, dy, scale);
                if (textPath != null && !textPath.IsEmpty)
                {
                    path.AddPath(textPath, SKPathAddMode.Append);
                }
            }
            return path;
        }

        public static SKPath ToSKPath(this IEllipseShape ellipse, double dx, double dy, Func<double, float> scale)
        {
            var path = new SKPath
            {
                FillType = SKPathFillType.Winding
            };
            path.AddOval(
                SkiaSharpRenderer.CreateRect(ellipse.TopLeft, ellipse.BottomRight, scale),
                SKPathDirection.Clockwise);
            if (ellipse is ITextShape text)
            {
                var textPath = ToSKPath(text, dx, dy, scale);
                if (textPath != null && !textPath.IsEmpty)
                {
                    path.AddPath(textPath, SKPathAddMode.Append);
                }
            }
            return path;
        }

        public static SKPath ToSKPath(this IImageShape image, double dx, double dy, Func<double, float> scale)
        {
            var path = new SKPath
            {
                FillType = SKPathFillType.Winding
            };
            path.AddRect(
                SkiaSharpRenderer.CreateRect(image.TopLeft, image.BottomRight, scale),
                SKPathDirection.Clockwise);
            if (image is ITextShape text)
            {
                var textPath = ToSKPath(text, dx, dy, scale);
                if (textPath != null && !textPath.IsEmpty)
                {
                    path.AddPath(textPath, SKPathAddMode.Append);
                }
            }
            return path;
        }

        public static SKPath ToSKPath(this IArcShape arc, double dx, double dy, Func<double, float> scale)
        {
            var path = new SKPath
            {
                FillType = SKPathFillType.Winding
            };
            var a = new GdiArc(
                Point2.FromXY(arc.Point1.X, arc.Point1.Y),
                Point2.FromXY(arc.Point2.X, arc.Point2.Y),
                Point2.FromXY(arc.Point3.X, arc.Point3.Y),
                Point2.FromXY(arc.Point4.X, arc.Point4.Y));
            var rect = new SKRect(
                scale(a.X),
                scale(a.Y),
                scale(a.X + a.Width),
                scale(a.Y + a.Height));
            path.AddArc(rect, (float)a.StartAngle, (float)a.SweepAngle);
            return path;
        }

        public static SKPath ToSKPath(this ICubicBezierShape cubicBezier, double dx, double dy, Func<double, float> scale)
        {
            var path = new SKPath
            {
                FillType = SKPathFillType.Winding
            };
            path.MoveTo(
                scale(cubicBezier.Point1.X),
                scale(cubicBezier.Point1.Y));
            path.CubicTo(
                scale(cubicBezier.Point2.X),
                scale(cubicBezier.Point2.Y),
                scale(cubicBezier.Point3.X),
                scale(cubicBezier.Point3.Y),
                scale(cubicBezier.Point4.X),
                scale(cubicBezier.Point4.Y));
            return path;
        }

        public static SKPath ToSKPath(this IQuadraticBezierShape quadraticBezier, double dx, double dy, Func<double, float> scale)
        {
            var path = new SKPath
            {
                FillType = SKPathFillType.Winding
            };
            path.MoveTo(
                scale(quadraticBezier.Point1.X),
                scale(quadraticBezier.Point1.Y));
            path.QuadTo(
                scale(quadraticBezier.Point2.X),
                scale(quadraticBezier.Point2.Y),
                scale(quadraticBezier.Point3.X),
                scale(quadraticBezier.Point3.Y));
            return path;
        }

        public static SKPath ToSKPath(this ITextShape text, double dx, double dy, Func<double, float> scale)
        {
            var path = new SKPath
            {
                FillType = SKPathFillType.Winding
            };

            if (!(text.GetProperty(nameof(ITextShape.Text)) is string tbind))
            {
                tbind = text.Text;
            }

            if (tbind == null)
            {
                return path;
            }

            using var pen = new SKPaint();
            SkiaSharpRenderer.GetSKPaint(tbind, text.Style, text.TopLeft, text.BottomRight, scale, 96.0, 96.0, true, pen, out var origin);

            using var outlinePath = pen.GetTextPath(tbind, origin.X, origin.Y);
            using var fillPath = pen.GetFillPath(outlinePath);

            path.AddPath(fillPath, SKPathAddMode.Append);

            return path;
        }

        public static SKPath ToSKPath(this IPathGeometry pathGeometry, Func<double, float> scale)
        {
            var fillType = pathGeometry.FillRule == FillRule.EvenOdd ? SKPathFillType.EvenOdd : SKPathFillType.Winding;
            var path = new SKPath
            {
                FillType = fillType
            };

            foreach (var pathFigure in pathGeometry.Figures)
            {
                CreateFigure(pathFigure, scale, path);
            }

            return path;
        }

        public static SKPath ToSKPath(this IPathShape path, double dx, double dy, Func<double, float> scale)
        {
            return ToSKPath(path.Geometry, scale);
        }

        public static SKPathOp ToSKPathOp(PathOp op)
        {
            return op switch
            {
                PathOp.Intersect => SKPathOp.Intersect,
                PathOp.Union => SKPathOp.Union,
                PathOp.Xor => SKPathOp.Xor,
                PathOp.ReverseDifference => SKPathOp.ReverseDifference,
                _ => SKPathOp.Difference,
            };
        }

        public static void Op(SKPath first, SKPath second, SKPathOp op, out SKPath result, out bool haveResult)
        {
            haveResult = false;
            result = new SKPath(first) { FillType = first.FillType };

            var next = result.Op(second, op);
            if (next != null)
            {
                result.Dispose();
                result = next;
                haveResult = true;
            }
        }

        public static void Op(IList<SKPath> paths, IList<SKPathOp> ops, out SKPath result, out bool haveResult)
        {
            using var builder = new SKPath.OpBuilder();

            for (int i = 0; i < paths.Count; i++)
            {
                builder.Add(paths[i], ops[i]);
            }

            result = new SKPath(paths[0]) { FillType = paths[0].FillType };
            haveResult = builder.Resolve(result);
        }

        public static void Op(IList<SKPath> paths, SKPathOp op, out SKPath result, out bool haveResult)
        {
            haveResult = false;
            result = new SKPath(paths[0]) { FillType = paths[0].FillType };

            if (paths.Count == 1)
            {
                using var empty = new SKPath() { FillType = paths[0].FillType };
                result = empty.Op(paths[0], op);
                haveResult = true;
            }
            else
            {
                for (int i = 1; i < paths.Count; i++)
                {
                    var next = result.Op(paths[i], op);
                    if (next != null)
                    {
                        result.Dispose();
                        result = next;
                        haveResult = true;
                    }
                }
            }
        }
    }
}
