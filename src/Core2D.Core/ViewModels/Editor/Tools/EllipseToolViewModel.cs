﻿#nullable enable
using System;
using System.Collections.Generic;
using Core2D.Model;
using Core2D.Model.Editor;
using Core2D.Model.Input;
using Core2D.ViewModels.Editor.Tools.Selection;
using Core2D.ViewModels.Shapes;
using Core2D.ViewModels.Style;
using static System.Math;

namespace Core2D.ViewModels.Editor.Tools;

public partial class EllipseToolViewModel : ViewModelBase, IEditorTool
{
    public enum State { TopLeft, BottomRight }
    public enum Mode { Rectangle, Circle }
    private State _currentState = State.TopLeft;
    private Mode _currentMode = Mode.Rectangle;
    private EllipseShapeViewModel? _ellipse;
    private EllipseSelection? _selection;
    private double _centerX;
    private double _centerY;

    public string Title => "Ellipse";

    public EllipseToolViewModel(IServiceProvider? serviceProvider) : base(serviceProvider)
    {
    }

    public override object Copy(IDictionary<object, object>? shared)
    {
        throw new NotImplementedException();
    }

    private static void CircleConstrain(PointShapeViewModel tl, PointShapeViewModel br, double cx, double cy, double px, double py)
    {
        double r = Max(Abs(cx - px), Abs(cy - py));
        tl.X = (double)(cx - r);
        tl.Y = (double)(cy - r);
        br.X = (double)(cx + r);
        br.Y = (double)(cy + r);
    }

    public void BeginDown(InputArgs args)
    {
        var factory = ServiceProvider.GetService<IViewModelFactory>();
        var editor = ServiceProvider.GetService<ProjectEditorViewModel>();
        var selection = ServiceProvider.GetService<ISelectionService>();
        var viewModelFactory = ServiceProvider.GetService<IViewModelFactory>();

        if (factory is null || editor?.Project?.Options is null || selection is null || viewModelFactory is null)
        {
            return;
        }

        var (sx, sy) = selection.TryToSnap(args);
        switch (_currentState)
        {
            case State.TopLeft:
            {
                editor.IsToolIdle = false;
                if (_currentMode == Mode.Circle)
                {
                    _centerX = sx;
                    _centerY = sy;
                }

                var style = editor.Project.CurrentStyleLibrary?.Selected is { } ?
                    editor.Project.CurrentStyleLibrary.Selected :
                    viewModelFactory.CreateShapeStyle(ProjectEditorConfiguration.DefaultStyleName);
                _ellipse = factory.CreateEllipseShape(
                    (double)sx, (double)sy,
                    (ShapeStyleViewModel)style.Copy(null),
                    editor.Project.Options.DefaultIsStroked,
                    editor.Project.Options.DefaultIsFilled);

                editor.SetShapeName(_ellipse);

                var result = selection.TryToGetConnectionPoint((double)sx, (double)sy);
                if (result is { })
                {
                    _ellipse.TopLeft = result;
                }

                editor.Project.CurrentContainer.WorkingLayer.Shapes = editor.Project.CurrentContainer.WorkingLayer.Shapes.Add(_ellipse);
                editor.Project.CurrentContainer.WorkingLayer.RaiseInvalidateLayer();
                ToStateBottomRight();
                Move(_ellipse);
                _currentState = State.BottomRight;
            }
                break;
            case State.BottomRight:
            {
                if (_ellipse is { })
                {
                    if (_currentMode == Mode.Circle)
                    {
                        CircleConstrain(_ellipse.TopLeft, _ellipse.BottomRight, _centerX, _centerY, sx, sy);
                    }
                    else
                    {
                        _ellipse.BottomRight.X = (double)sx;
                        _ellipse.BottomRight.Y = (double)sy;
                    }

                    var result = selection.TryToGetConnectionPoint((double)sx, (double)sy);
                    if (result is { })
                    {
                        _ellipse.BottomRight = result;
                    }

                    editor.Project.CurrentContainer.WorkingLayer.Shapes = editor.Project.CurrentContainer.WorkingLayer.Shapes.Remove(_ellipse);
                    Finalize(_ellipse);
                    editor.Project.AddShape(editor.Project.CurrentContainer.CurrentLayer, _ellipse);

                    Reset();
                }
            }
                break;
        }
    }

    public void BeginUp(InputArgs args)
    {
    }

    public void EndDown(InputArgs args)
    {
        switch (_currentState)
        {
            case State.TopLeft:
                break;
            case State.BottomRight:
                Reset();
                break;
        }
    }

    public void EndUp(InputArgs args)
    {
    }

    public void Move(InputArgs args)
    {
        var editor = ServiceProvider.GetService<ProjectEditorViewModel>();
        var selection = ServiceProvider.GetService<ISelectionService>();
        var (sx, sy) = selection.TryToSnap(args);
        switch (_currentState)
        {
            case State.TopLeft:
            {
                if (editor.Project.Options.TryToConnect)
                {
                    selection.TryToHoverShape((double)sx, (double)sy);
                }
            }
                break;
            case State.BottomRight:
            {
                if (_ellipse is { })
                {
                    if (editor.Project.Options.TryToConnect)
                    {
                        selection.TryToHoverShape((double)sx, (double)sy);
                    }

                    if (_currentMode == Mode.Circle)
                    {
                        CircleConstrain(_ellipse.TopLeft, _ellipse.BottomRight, _centerX, _centerY, sx, sy);
                    }
                    else
                    {
                        _ellipse.BottomRight.X = (double)sx;
                        _ellipse.BottomRight.Y = (double)sy;
                    }
                    editor.Project.CurrentContainer.WorkingLayer.RaiseInvalidateLayer();
                    Move(_ellipse);
                }
            }
                break;
        }
    }

    public void ToStateBottomRight()
    {
        var editor = ServiceProvider.GetService<ProjectEditorViewModel>();
        if (editor is null)
        {
            return;
        }

        _selection = new EllipseSelection(
            ServiceProvider,
            editor.Project.CurrentContainer.HelperLayer,
            _ellipse,
            editor.PageState.HelperStyle);

        _selection?.ToStateBottomRight();
    }

    public void Move(BaseShapeViewModel shape)
    {
        _selection?.Move();
    }

    public void Finalize(BaseShapeViewModel shape)
    {
    }

    public void Reset()
    {
        var editor = ServiceProvider.GetService<ProjectEditorViewModel>();
            
        if (editor is null)
        {
            return;
        }
            
        switch (_currentState)
        {
            case State.TopLeft:
                break;
            case State.BottomRight:
            {
                editor.Project.CurrentContainer.WorkingLayer.Shapes = editor.Project.CurrentContainer.WorkingLayer.Shapes.Remove(_ellipse);
                editor.Project.CurrentContainer.WorkingLayer.RaiseInvalidateLayer();
            }
                break;
        }

        _currentState = State.TopLeft;

        if (_selection is { })
        {
            _selection.Reset();
            _selection = null;
        }

        editor.IsToolIdle = true;
    }
}
