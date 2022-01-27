﻿#nullable enable
using System;
using System.Collections.Generic;
using Core2D.Model;
using Core2D.Model.Editor;
using Core2D.Model.Input;
using Core2D.ViewModels.Editor.Tools.Selection;
using Core2D.ViewModels.Shapes;
using Core2D.ViewModels.Style;

namespace Core2D.ViewModels.Editor.Tools;

public partial class LineToolViewModel : ViewModelBase, IEditorTool
{
    public enum State { Start, End }
    private State _currentState = State.Start;
    private LineShapeViewModel? _line;
    private LineSelection? _selection;

    public string Title => "Line";

    public LineToolViewModel(IServiceProvider? serviceProvider) : base(serviceProvider)
    {
    }

    public override object Copy(IDictionary<object, object>? shared)
    {
        throw new NotImplementedException();
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

        (double x, double y) = args;
        var (sx, sy) = selection.TryToSnap(args);
        switch (_currentState)
        {
            case State.Start:
            {
                editor.IsToolIdle = false;
                var style = editor.Project.CurrentStyleLibrary?.Selected is { } ?
                    editor.Project.CurrentStyleLibrary.Selected :
                    viewModelFactory.CreateShapeStyle(ProjectEditorConfiguration.DefaultStyleName);
                _line = factory.CreateLineShape(
                    (double)sx, (double)sy,
                    (ShapeStyleViewModel)style.Copy(null),
                    editor.Project.Options.DefaultIsStroked);

                editor.SetShapeName(_line);

                if (editor.Project.Options.TryToConnect)
                {
                    var result = selection.TryToGetConnectionPoint((double)sx, (double)sy);
                    if (result is { })
                    {
                        _line.Start = result;
                    }
                    else
                    {
                        selection.TryToSplitLine(x, y, _line.Start);
                    }
                }
                editor.Project.CurrentContainer.WorkingLayer.Shapes = editor.Project.CurrentContainer.WorkingLayer.Shapes.Add(_line);
                editor.Project.CurrentContainer.WorkingLayer.RaiseInvalidateLayer();
                ToStateEnd();
                Move(_line);
                _currentState = State.End;
            }
                break;
            case State.End:
            {
                if (_line is { })
                {
                    _line.End.X = (double)sx;
                    _line.End.Y = (double)sy;

                    if (editor.Project.Options.TryToConnect)
                    {
                        var result = selection.TryToGetConnectionPoint((double)sx, (double)sy);
                        if (result is { })
                        {
                            _line.End = result;
                        }
                        else
                        {
                            selection.TryToSplitLine(x, y, _line.End);
                        }
                    }

                    editor.Project.CurrentContainer.WorkingLayer.Shapes = editor.Project.CurrentContainer.WorkingLayer.Shapes.Remove(_line);
                    Finalize(_line);
                    editor.Project.AddShape(editor.Project.CurrentContainer.CurrentLayer, _line);

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
            case State.Start:
                break;
            case State.End:
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
            case State.Start:
            {
                if (editor.Project.Options.TryToConnect)
                {
                    selection.TryToHoverShape((double)sx, (double)sy);
                }
            }
                break;
            case State.End:
            {
                if (_line is { })
                {
                    if (editor.Project.Options.TryToConnect)
                    {
                        selection.TryToHoverShape((double)sx, (double)sy);
                    }
                    _line.End.X = (double)sx;
                    _line.End.Y = (double)sy;
                    editor.Project.CurrentContainer.WorkingLayer.RaiseInvalidateLayer();
                    Move(_line);
                }
            }
                break;
        }
    }

    public void ToStateEnd()
    {
        var editor = ServiceProvider.GetService<ProjectEditorViewModel>();
        _selection = new LineSelection(
            ServiceProvider,
            editor.Project.CurrentContainer.HelperLayer,
            _line,
            editor.PageState.HelperStyle);

        _selection.ToStateEnd();
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
            case State.Start:
                break;
            case State.End:
            {
                editor.Project.CurrentContainer.WorkingLayer.Shapes = editor.Project.CurrentContainer.WorkingLayer.Shapes.Remove(_line);
                editor.Project.CurrentContainer.WorkingLayer.RaiseInvalidateLayer();
            }
                break;
        }

        _currentState = State.Start;

        if (_selection is { })
        {
            _selection.Reset();
            _selection = null;
        }

        editor.IsToolIdle = true;
    }
}
