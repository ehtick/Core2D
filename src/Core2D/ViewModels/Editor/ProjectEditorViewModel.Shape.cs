﻿#nullable enable
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Core2D.Model;
using Core2D.Model.Editor;
using Core2D.Model.Renderer;
using Core2D.ViewModels.Containers;
using Core2D.ViewModels.Layout;
using Core2D.ViewModels.Shapes;

namespace Core2D.ViewModels.Editor
{
    public interface IShapeService
    {
        void OnDuplicateSelected();
        void OnGroupSelected();
        void OnUngroupSelected();
        void OnRotateSelected(string degrees);
        void OnFlipHorizontalSelected();
        void OnFlipVerticalSelected();
        void OnMoveUpSelected();
        void OnMoveDownSelected();
        void OnMoveLeftSelected();
        void OnMoveRightSelected();
        void OnStackHorizontallySelected();
        void OnStackVerticallySelected();
        void OnDistributeHorizontallySelected();
        void OnDistributeVerticallySelected();
        void OnAlignLeftSelected();
        void OnAlignCenteredSelected();
        void OnAlignRightSelected();
        void OnAlignTopSelected();
        void OnAlignCenterSelected();
        void OnAlignBottomSelected();
        void OnBringToFrontSelected();
        void OnBringForwardSelected();
        void OnSendBackwardSelected();
        void OnSendToBackSelected();
        void OnCreatePath();
        void OnCreateStrokePath();
        void OnCreateFillPath();
        void OnCreateWindingPath();
        void OnPathSimplify();
        void OnPathBreak();
        void OnPathOp(string op);
        GroupShapeViewModel? Group(ISet<BaseShapeViewModel>? shapes, string name);
        bool Ungroup(ISet<BaseShapeViewModel>? shapes);
        void BringToFront(BaseShapeViewModel source);
        void BringForward(BaseShapeViewModel source);
        void SendBackward(BaseShapeViewModel source);
        void SendToBack(BaseShapeViewModel source);
        void MoveShapesBy(IEnumerable<BaseShapeViewModel> shapes, decimal dx, decimal dy);
        void MoveBy(ISet<BaseShapeViewModel>? shapes, decimal dx, decimal dy);
        void MoveItem(LibraryViewModel libraryViewModel, int sourceIndex, int targetIndex);
        void SwapItem(LibraryViewModel libraryViewModel, int sourceIndex, int targetIndex);
        void InsertItem(LibraryViewModel libraryViewModel, ViewModelBase item, int index);
    }

    public class ShapeServiceViewModel : ViewModelBase, IShapeService
    {
        public ShapeServiceViewModel(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override object Copy(IDictionary<object, object>? shared)
        {
            throw new NotImplementedException();
        }

        public void OnDuplicateSelected()
        {
            var Project = ServiceProvider.GetService<ProjectEditorViewModel>()?.Project;
            if (Project is null)
            {
                return;
            }

            if (Project.SelectedShapes is null)
            {
                return;
            }

            try
            {
                var copy = ServiceProvider.GetService<IClipboardService>()?.Copy(Project.SelectedShapes.ToList());
                if (copy is { })
                {
                    ServiceProvider.GetService<IClipboardService>()?.OnPasteShapes(copy);
                }
            }
            catch (Exception ex)
            {
                ServiceProvider.GetService<ILog>()?.LogException(ex);
            }
        }

        public void OnGroupSelected()
        {
            var Project = ServiceProvider.GetService<ProjectEditorViewModel>()?.Project;
            if (Project is null)
            {
                return;
            }

            var group = Group(Project?.SelectedShapes, ProjectEditorConfiguration.DefaulGroupName);
            if (group is { })
            {
                ServiceProvider.GetService<ISelectionService>()?.Select(Project?.CurrentContainer?.CurrentLayer, group);
            }
        }

        public void OnUngroupSelected()
        {
            var Project = ServiceProvider.GetService<ProjectEditorViewModel>()?.Project;
            if (Project is null)
            {
                return;
            }

            var result = Ungroup(Project?.SelectedShapes);
            if (result)
            {
                if (Project is { })
                {
                    Project.SelectedShapes = null;
                }
                ServiceProvider.GetService<ISelectionService>()?.OnHideDecorator();
            }
        }

        public void OnRotateSelected(string degrees)
        {
            var Project = ServiceProvider.GetService<ProjectEditorViewModel>()?.Project;
            if (Project is null)
            {
                return;
            }

            if (!double.TryParse(degrees, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var value))
            {
                return;
            }

            var sources = Project?.SelectedShapes;
            if (sources is { })
            {
                BoxLayout.Rotate(sources, (decimal)value, Project?.History);
                ServiceProvider.GetService<ISelectionService>()?.OnUpdateDecorator();
            }
        }

        public void OnFlipHorizontalSelected()
        {
            var Project = ServiceProvider.GetService<ProjectEditorViewModel>()?.Project;
            if (Project is null)
            {
                return;
            }

            var sources = Project?.SelectedShapes;
            if (sources is { })
            {
                BoxLayout.Flip(sources, FlipMode.Horizontal, Project?.History);
                ServiceProvider.GetService<ISelectionService>()?.OnUpdateDecorator();
            }
        }

        public void OnFlipVerticalSelected()
        {
            var Project = ServiceProvider.GetService<ProjectEditorViewModel>()?.Project;
            if (Project is null)
            {
                return;
            }

            var sources = Project?.SelectedShapes;
            if (sources is { })
            {
                BoxLayout.Flip(sources, FlipMode.Vertical, Project?.History);
                ServiceProvider.GetService<ISelectionService>()?.OnUpdateDecorator();
            }
        }

        public void OnMoveUpSelected()
        {
            var Project = ServiceProvider.GetService<ProjectEditorViewModel>()?.Project;
            if (Project is null)
            {
                return;
            }

            if (Project?.Options is null)
            {
                return;
            }
            MoveBy(
                Project.SelectedShapes,
                0m,
                Project.Options.SnapToGrid ? (decimal)-Project.Options.SnapY : -1m);
        }

        public void OnMoveDownSelected()
        {
            var Project = ServiceProvider.GetService<ProjectEditorViewModel>()?.Project;
            if (Project is null)
            {
                return;
            }

            if (Project?.Options is null)
            {
                return;
            }
            MoveBy(
                Project.SelectedShapes,
                0m,
                Project.Options.SnapToGrid ? (decimal)Project.Options.SnapY : 1m);
        }

        public void OnMoveLeftSelected()
        {
            var Project = ServiceProvider.GetService<ProjectEditorViewModel>()?.Project;
            if (Project is null)
            {
                return;
            }

            if (Project?.Options is null)
            {
                return;
            }
            MoveBy(
                Project.SelectedShapes,
                Project.Options.SnapToGrid ? (decimal)-Project.Options.SnapX : -1m,
                0m);
        }

        public void OnMoveRightSelected()
        {
            var Project = ServiceProvider.GetService<ProjectEditorViewModel>()?.Project;
            if (Project is null)
            {
                return;
            }

            if (Project?.Options is null)
            {
                return;
            }
            MoveBy(
                Project.SelectedShapes,
                Project.Options.SnapToGrid ? (decimal)Project.Options.SnapX : 1m,
                0m);
        }

        public void OnStackHorizontallySelected()
        {
            var Project = ServiceProvider.GetService<ProjectEditorViewModel>()?.Project;
            if (Project is null)
            {
                return;
            }

            var shapes = Project?.SelectedShapes;
            if (shapes is { })
            {
                var items = shapes.Where(s => !s.State.HasFlag(ShapeStateFlags.Locked));
                BoxLayout.Stack(items, StackMode.Horizontal, Project?.History);
                ServiceProvider.GetService<ISelectionService>()?.OnUpdateDecorator();
            }
        }

        public void OnStackVerticallySelected()
        {
            var Project = ServiceProvider.GetService<ProjectEditorViewModel>()?.Project;
            if (Project is null)
            {
                return;
            }

            var shapes = Project?.SelectedShapes;
            if (shapes is { })
            {
                var items = shapes.Where(s => !s.State.HasFlag(ShapeStateFlags.Locked));
                BoxLayout.Stack(items, StackMode.Vertical, Project?.History);
                ServiceProvider.GetService<ISelectionService>()?.OnUpdateDecorator();
            }
        }

        public void OnDistributeHorizontallySelected()
        {
            var Project = ServiceProvider.GetService<ProjectEditorViewModel>()?.Project;
            if (Project is null)
            {
                return;
            }

            var shapes = Project?.SelectedShapes;
            if (shapes is { })
            {
                var items = shapes.Where(s => !s.State.HasFlag(ShapeStateFlags.Locked));
                BoxLayout.Distribute(items, DistributeMode.Horizontal, Project?.History);
                ServiceProvider.GetService<ISelectionService>()?.OnUpdateDecorator();
            }
        }

        public void OnDistributeVerticallySelected()
        {
            var Project = ServiceProvider.GetService<ProjectEditorViewModel>()?.Project;
            if (Project is null)
            {
                return;
            }

            var shapes = Project?.SelectedShapes;
            if (shapes is { })
            {
                var items = shapes.Where(s => !s.State.HasFlag(ShapeStateFlags.Locked));
                BoxLayout.Distribute(items, DistributeMode.Vertical, Project?.History);
                ServiceProvider.GetService<ISelectionService>()?.OnUpdateDecorator();
            }
        }

        public void OnAlignLeftSelected()
        {
            var Project = ServiceProvider.GetService<ProjectEditorViewModel>()?.Project;
            if (Project is null)
            {
                return;
            }

            var shapes = Project?.SelectedShapes;
            if (shapes is { })
            {
                var items = shapes.Where(s => !s.State.HasFlag(ShapeStateFlags.Locked));
                BoxLayout.Align(items, AlignMode.Left, Project?.History);
                ServiceProvider.GetService<ISelectionService>()?.OnUpdateDecorator();
            }
        }

        public void OnAlignCenteredSelected()
        {
            var Project = ServiceProvider.GetService<ProjectEditorViewModel>()?.Project;
            if (Project is null)
            {
                return;
            }

            var shapes = Project?.SelectedShapes;
            if (shapes is { })
            {
                var items = shapes.Where(s => !s.State.HasFlag(ShapeStateFlags.Locked));
                BoxLayout.Align(items, AlignMode.Centered, Project?.History);
                ServiceProvider.GetService<ISelectionService>()?.OnUpdateDecorator();
            }
        }

        public void OnAlignRightSelected()
        {
            var Project = ServiceProvider.GetService<ProjectEditorViewModel>()?.Project;
            if (Project is null)
            {
                return;
            }

            var shapes = Project?.SelectedShapes;
            if (shapes is { })
            {
                var items = shapes.Where(s => !s.State.HasFlag(ShapeStateFlags.Locked));
                BoxLayout.Align(items, AlignMode.Right, Project?.History);
                ServiceProvider.GetService<ISelectionService>()?.OnUpdateDecorator();
            }
        }

        public void OnAlignTopSelected()
        {
            var Project = ServiceProvider.GetService<ProjectEditorViewModel>()?.Project;
            if (Project is null)
            {
                return;
            }

            var shapes = Project?.SelectedShapes;
            if (shapes is { })
            {
                var items = shapes.Where(s => !s.State.HasFlag(ShapeStateFlags.Locked));
                BoxLayout.Align(items, AlignMode.Top, Project?.History);
                ServiceProvider.GetService<ISelectionService>()?.OnUpdateDecorator();
            }
        }

        public void OnAlignCenterSelected()
        {
            var Project = ServiceProvider.GetService<ProjectEditorViewModel>()?.Project;
            if (Project is null)
            {
                return;
            }

            var shapes = Project?.SelectedShapes;
            if (shapes is { })
            {
                var items = shapes.Where(s => !s.State.HasFlag(ShapeStateFlags.Locked));
                BoxLayout.Align(items, AlignMode.Center, Project?.History);
                ServiceProvider.GetService<ISelectionService>()?.OnUpdateDecorator();
            }
        }

        public void OnAlignBottomSelected()
        {
            var Project = ServiceProvider.GetService<ProjectEditorViewModel>()?.Project;
            if (Project is null)
            {
                return;
            }

            var shapes = Project?.SelectedShapes;
            if (shapes is { })
            {
                var items = shapes.Where(s => !s.State.HasFlag(ShapeStateFlags.Locked));
                BoxLayout.Align(items, AlignMode.Bottom, Project?.History);
                ServiceProvider.GetService<ISelectionService>()?.OnUpdateDecorator();
            }
        }

        public void OnBringToFrontSelected()
        {
            var Project = ServiceProvider.GetService<ProjectEditorViewModel>()?.Project;
            if (Project is null)
            {
                return;
            }

            var sources = Project?.SelectedShapes;
            if (sources is { })
            {
                foreach (var s in sources)
                {
                    BringToFront(s);
                }
            }
        }

        public void OnBringForwardSelected()
        {
            var Project = ServiceProvider.GetService<ProjectEditorViewModel>()?.Project;
            if (Project is null)
            {
                return;
            }

            var sources = Project?.SelectedShapes;
            if (sources is { })
            {
                foreach (var s in sources)
                {
                    BringForward(s);
                }
            }
        }

        public void OnSendBackwardSelected()
        {
            var Project = ServiceProvider.GetService<ProjectEditorViewModel>()?.Project;
            if (Project is null)
            {
                return;
            }

            var sources = Project?.SelectedShapes;
            if (sources is { })
            {
                foreach (var s in sources.Reverse())
                {
                    SendBackward(s);
                }
            }
        }

        public void OnSendToBackSelected()
        {
            var Project = ServiceProvider.GetService<ProjectEditorViewModel>()?.Project;
            if (Project is null)
            {
                return;
            }

            var sources = Project?.SelectedShapes;
            if (sources is { })
            {
                foreach (var s in sources.Reverse())
                {
                    SendToBack(s);
                }
            }
        }

        public void OnCreatePath()
        {
            var Project = ServiceProvider.GetService<ProjectEditorViewModel>()?.Project;
            if (Project is null)
            {
                return;
            }

            var PathConverter = ServiceProvider.GetService<IPathConverter>();
            if (PathConverter is null)
            {
                return;
            }

            var layer = Project?.CurrentContainer?.CurrentLayer;
            if (layer is null)
            {
                return;
            }

            var sources = Project?.SelectedShapes;
            var source = Project?.SelectedShapes?.FirstOrDefault();

            if (sources is { Count: 1 } && source is not null)
            {
                CreatePath(source, layer);
            }

            if (sources is { Count: > 1 })
            {
                CreatePath(sources, layer);
            }
        }

        private void CreatePath(BaseShapeViewModel source, LayerContainerViewModel layer)
        {
            var Project = ServiceProvider.GetService<ProjectEditorViewModel>()?.Project;
            if (Project is null)
            {
                return;
            }

            var path = ServiceProvider.GetService<IPathConverter>()?.ToPathShape(source);
            if (path == null)
            {
                return;
            }
            
            var shapesBuilder = layer.Shapes.ToBuilder();

            var index = shapesBuilder.IndexOf(source);
            shapesBuilder[index] = path;

            var previous = layer.Shapes;
            var next = shapesBuilder.ToImmutable();
            Project?.History?.Snapshot(previous, next, (p) => layer.Shapes = p);
            layer.Shapes = next;

            ServiceProvider.GetService<ISelectionService>()?.Select(layer, path);
        }

        private void CreatePath(ISet<BaseShapeViewModel> sources, LayerContainerViewModel layer)
        {
            var Project = ServiceProvider.GetService<ProjectEditorViewModel>()?.Project;
            if (Project is null)
            {
                return;
            }

            var path = ServiceProvider.GetService<IPathConverter>()?.ToPathShape(sources);
            if (path is null)
            {
                return;
            }

            var shapesBuilder = layer.Shapes.ToBuilder();

            foreach (var shape in sources)
            {
                shapesBuilder.Remove(shape);
            }

            shapesBuilder.Add(path);

            var previous = layer.Shapes;
            var next = shapesBuilder.ToImmutable();
            Project?.History?.Snapshot(previous, next, (p) => layer.Shapes = p);
            layer.Shapes = next;

            ServiceProvider.GetService<ISelectionService>()?.Select(layer, path);
        }

        public void OnCreateStrokePath()
        {
            var Project = ServiceProvider.GetService<ProjectEditorViewModel>()?.Project;
            if (Project is null)
            {
                return;
            }

            var PathConverter = ServiceProvider.GetService<IPathConverter>();
            if (PathConverter is null)
            {
                return;
            }

            var layer = Project?.CurrentContainer?.CurrentLayer;
            if (layer is null)
            {
                return;
            }

            var sources = Project?.SelectedShapes;
            var source = Project?.SelectedShapes?.FirstOrDefault();

            if (sources is { Count: 1 } && source is not null)
            {
                CreateStrokePath(source, layer);
            }

            if (sources is { Count: > 1 })
            {
                CreateStrokePath(sources, layer);
            }
        }

        private void CreateStrokePath(BaseShapeViewModel source, LayerContainerViewModel layer)
        {
            var Project = ServiceProvider.GetService<ProjectEditorViewModel>()?.Project;
            if (Project is null)
            {
                return;
            }

            var path = ServiceProvider.GetService<IPathConverter>()?.ToStrokePathShape(source);
            if (path is null)
            {
                return;
            }
            
            path.IsStroked = false;
            path.IsFilled = true;

            var shapesBuilder = layer.Shapes.ToBuilder();

            var index = shapesBuilder.IndexOf(source);
            shapesBuilder[index] = path;

            var previous = layer.Shapes;
            var next = shapesBuilder.ToImmutable();
            Project?.History?.Snapshot(previous, next, (p) => layer.Shapes = p);
            layer.Shapes = next;

            ServiceProvider.GetService<ISelectionService>()?.Select(layer, path);
        }

        private void CreateStrokePath(ISet<BaseShapeViewModel> sources, LayerContainerViewModel layer)
        {
            var Project = ServiceProvider.GetService<ProjectEditorViewModel>()?.Project;
            if (Project is null)
            {
                return;
            }

            var paths = new List<PathShapeViewModel>();
            var shapes = new List<BaseShapeViewModel>();

            foreach (var s in sources)
            {
                var path = ServiceProvider.GetService<IPathConverter>()?.ToStrokePathShape(s);
                if (path is null)
                {
                    continue;
                }
                
                path.IsStroked = false;
                path.IsFilled = true;

                paths.Add(path);
                shapes.Add(s);
            }

            if (paths.Count <= 0)
            {
                return;
            }
            
            var shapesBuilder = layer.Shapes.ToBuilder();

            for (int i = 0; i < paths.Count; i++)
            {
                var index = shapesBuilder.IndexOf(shapes[i]);
                shapesBuilder[index] = paths[i];
            }

            var previous = layer.Shapes;
            var next = shapesBuilder.ToImmutable();
            Project?.History?.Snapshot(previous, next, (p) => layer.Shapes = p);
            layer.Shapes = next;

            ServiceProvider.GetService<ISelectionService>()?.Select(layer, new HashSet<BaseShapeViewModel>(paths));
        }

        public void OnCreateFillPath()
        {
            var Project = ServiceProvider.GetService<ProjectEditorViewModel>()?.Project;
            if (Project is null)
            {
                return;
            }

            var PathConverter = ServiceProvider.GetService<IPathConverter>();
            if (PathConverter is null)
            {
                return;
            }

            var layer = Project?.CurrentContainer?.CurrentLayer;
            if (layer is null)
            {
                return;
            }

            var sources = Project?.SelectedShapes;
            var source = Project?.SelectedShapes?.FirstOrDefault();

            if (sources is { Count: 1 } && source is not null)
            {
                CreateFillPath(source, layer);
            }

            if (sources is { Count: > 1 })
            {
                CreateFillPath(sources, layer);
            }
        }

        private void CreateFillPath(BaseShapeViewModel source, LayerContainerViewModel layer)
        {
            var Project = ServiceProvider.GetService<ProjectEditorViewModel>()?.Project;
            if (Project is null)
            {
                return;
            }

            var path = ServiceProvider.GetService<IPathConverter>()?.ToFillPathShape(source);
            if (path is null)
            {
                return;
            }
            
            path.IsStroked = false;
            path.IsFilled = true;

            var shapesBuilder = layer.Shapes.ToBuilder();

            var index = shapesBuilder.IndexOf(source);
            shapesBuilder[index] = path;

            var previous = layer.Shapes;
            var next = shapesBuilder.ToImmutable();
            Project?.History?.Snapshot(previous, next, (p) => layer.Shapes = p);
            layer.Shapes = next;

            ServiceProvider.GetService<ISelectionService>()?.Select(layer, path);
        }

        private void CreateFillPath(ISet<BaseShapeViewModel> sources, LayerContainerViewModel layer)
        {
            var Project = ServiceProvider.GetService<ProjectEditorViewModel>()?.Project;
            if (Project is null)
            {
                return;
            }

            var paths = new List<PathShapeViewModel>();
            var shapes = new List<BaseShapeViewModel>();

            foreach (var s in sources)
            {
                var path = ServiceProvider.GetService<IPathConverter>()?.ToFillPathShape(s);
                if (path is null)
                {
                    continue;
                }
                
                path.IsStroked = false;
                path.IsFilled = true;

                paths.Add(path);
                shapes.Add(s);
            }

            if (paths.Count <= 0)
            {
                return;
            }
            
            var shapesBuilder = layer.Shapes.ToBuilder();

            for (int i = 0; i < paths.Count; i++)
            {
                var index = shapesBuilder.IndexOf(shapes[i]);
                shapesBuilder[index] = paths[i];
            }

            var previous = layer.Shapes;
            var next = shapesBuilder.ToImmutable();
            Project?.History?.Snapshot(previous, next, (p) => layer.Shapes = p);
            layer.Shapes = next;

            ServiceProvider.GetService<ISelectionService>()?.Select(layer, new HashSet<BaseShapeViewModel>(paths));
        }

        public void OnCreateWindingPath()
        {
            var Project = ServiceProvider.GetService<ProjectEditorViewModel>()?.Project;
            if (Project is null)
            {
                return;
            }

            var PathConverter = ServiceProvider.GetService<IPathConverter>();
            if (PathConverter is null)
            {
                return;
            }

            var layer = Project?.CurrentContainer?.CurrentLayer;
            if (layer is null)
            {
                return;
            }

            var sources = Project?.SelectedShapes;
            var source = Project?.SelectedShapes?.FirstOrDefault();

            if (sources is { Count: 1 } && source is not null)
            {
                CreateWindingPath(source, layer);
            }

            if (sources is { Count: > 1 })
            {
                CreateWindingPath(sources, layer);
            }
        }

        private void CreateWindingPath(BaseShapeViewModel source, LayerContainerViewModel layer)
        {
            var Project = ServiceProvider.GetService<ProjectEditorViewModel>()?.Project;
            if (Project is null)
            {
                return;
            }

            var path = ServiceProvider.GetService<IPathConverter>()?.ToWindingPathShape(source);
            if (path is null)
            {
                return;
            }
            
            path.IsStroked = false;
            path.IsFilled = true;

            var shapesBuilder = layer.Shapes.ToBuilder();

            var index = shapesBuilder.IndexOf(source);
            shapesBuilder[index] = path;

            var previous = layer.Shapes;
            var next = shapesBuilder.ToImmutable();
            Project?.History?.Snapshot(previous, next, (p) => layer.Shapes = p);
            layer.Shapes = next;

            ServiceProvider.GetService<ISelectionService>()?.Select(layer, path);
        }

        private void CreateWindingPath(ISet<BaseShapeViewModel> sources, LayerContainerViewModel layer)
        {
            var Project = ServiceProvider.GetService<ProjectEditorViewModel>()?.Project;
            if (Project is null)
            {
                return;
            }

            var paths = new List<PathShapeViewModel>();
            var shapes = new List<BaseShapeViewModel>();

            foreach (var s in sources)
            {
                var path = ServiceProvider.GetService<IPathConverter>()?.ToWindingPathShape(s);
                if (path is null)
                {
                    continue;
                }
                
                path.IsStroked = false;
                path.IsFilled = true;

                paths.Add(path);
                shapes.Add(s);
            }

            if (paths.Count <= 0)
            {
                return;
            }
            
            var shapesBuilder = layer.Shapes.ToBuilder();

            for (int i = 0; i < paths.Count; i++)
            {
                var index = shapesBuilder.IndexOf(shapes[i]);
                shapesBuilder[index] = paths[i];
            }

            var previous = layer.Shapes;
            var next = shapesBuilder.ToImmutable();
            Project?.History?.Snapshot(previous, next, (p) => layer.Shapes = p);
            layer.Shapes = next;

            ServiceProvider.GetService<ISelectionService>()?.Select(layer, new HashSet<BaseShapeViewModel>(paths));
        }

        public void OnPathSimplify()
        {
            var Project = ServiceProvider.GetService<ProjectEditorViewModel>()?.Project;
            if (Project is null)
            {
                return;
            }

            var PathConverter = ServiceProvider.GetService<IPathConverter>();
            if (PathConverter is null)
            {
                return;
            }

            var layer = Project?.CurrentContainer?.CurrentLayer;
            if (layer is null)
            {
                return;
            }

            var sources = Project?.SelectedShapes;
            var source = Project?.SelectedShapes?.FirstOrDefault();

            if (sources is { Count: 1 } && source is not null)
            {
                PathSimplify(source, layer);
            }

            if (sources is { Count: > 1 })
            {
                PathSimplify(sources, layer);
            }
        }

        private void PathSimplify(BaseShapeViewModel source, LayerContainerViewModel layer)
        {
            var Project = ServiceProvider.GetService<ProjectEditorViewModel>()?.Project;
            if (Project is null)
            {
                return;
            }

            var path = ServiceProvider.GetService<IPathConverter>()?.Simplify(source);
            if (path is null)
            {
                return;
            }
            
            var shapesBuilder = layer.Shapes.ToBuilder();

            var index = shapesBuilder.IndexOf(source);
            shapesBuilder[index] = path;

            var previous = layer.Shapes;
            var next = shapesBuilder.ToImmutable();
            Project?.History?.Snapshot(previous, next, (p) => layer.Shapes = p);
            layer.Shapes = next;

            ServiceProvider.GetService<ISelectionService>()?.Select(layer, path);
        }

        private void PathSimplify(ISet<BaseShapeViewModel> sources, LayerContainerViewModel layer)
        {
            var Project = ServiceProvider.GetService<ProjectEditorViewModel>()?.Project;
            if (Project is null)
            {
                return;
            }

            var paths = new List<PathShapeViewModel>();
            var shapes = new List<BaseShapeViewModel>();

            foreach (var s in sources)
            {
                var path = ServiceProvider.GetService<IPathConverter>()?.Simplify(s);
                if (path is null)
                {
                    continue;
                }
                
                paths.Add(path);
                shapes.Add(s);
            }

            if (paths.Count <= 0)
            {
                return;
            }
            
            var shapesBuilder = layer.Shapes.ToBuilder();

            for (int i = 0; i < paths.Count; i++)
            {
                var index = shapesBuilder.IndexOf(shapes[i]);
                shapesBuilder[index] = paths[i];
            }

            var previous = layer.Shapes;
            var next = shapesBuilder.ToImmutable();
            Project?.History?.Snapshot(previous, next, (p) => layer.Shapes = p);
            layer.Shapes = next;

            ServiceProvider.GetService<ISelectionService>()?.Select(layer, new HashSet<BaseShapeViewModel>(paths));
        }

        public void OnPathBreak()
        {
            var Project = ServiceProvider.GetService<ProjectEditorViewModel>()?.Project;
            if (Project is null)
            {
                return;
            }

            var PathConverter = ServiceProvider.GetService<IPathConverter>();
            if (PathConverter is null)
            {
                return;
            }

            var layer = Project?.CurrentContainer?.CurrentLayer;
            if (layer is null)
            {
                return;
            }

            var sources = Project?.SelectedShapes;

            if (sources is not { Count: >= 1 })
            {
                return;
            }
            
            var result = new List<BaseShapeViewModel>();
            var remove = new List<BaseShapeViewModel>();

            foreach (var s in sources)
            {
                ServiceProvider.GetService<ShapeEditor>()?.BreakShape(s, result, remove);
            }

            if (result.Count <= 0)
            {
                return;
            }
            
            var shapesBuilder = layer.Shapes.ToBuilder();

            foreach (var t in remove)
            {
                shapesBuilder.Remove(t);
            }

            foreach (var t in result)
            {
                shapesBuilder.Add(t);
            }

            var previous = layer.Shapes;
            var next = shapesBuilder.ToImmutable();
            Project?.History?.Snapshot(previous, next, (p) => layer.Shapes = p);
            layer.Shapes = next;

            ServiceProvider.GetService<ISelectionService>()?.Select(layer, new HashSet<BaseShapeViewModel>(result));
        }

        public void OnPathOp(string op)
        {
            var Project = ServiceProvider.GetService<ProjectEditorViewModel>()?.Project;
            if (Project is null)
            {
                return;
            }

            if (!Enum.TryParse<PathOp>(op, true, out var pathOp))
            {
                return;
            }

            var PathConverter = ServiceProvider.GetService<IPathConverter>();
            if (PathConverter is null)
            {
                return;
            }

            var sources = Project?.SelectedShapes;
            if (sources is null)
            {
                return;
            }

            var layer = Project?.CurrentContainer?.CurrentLayer;
            if (layer is null)
            {
                return;
            }

            var path = PathConverter.Op(sources, pathOp);
            if (path is null)
            {
                return;
            }

            var shapesBuilder = layer.Shapes.ToBuilder();
            foreach (var shape in sources)
            {
                shapesBuilder.Remove(shape);
            }
            shapesBuilder.Add(path);

            var previous = layer.Shapes;
            var next = shapesBuilder.ToImmutable();
            Project?.History?.Snapshot(previous, next, (p) => layer.Shapes = p);
            layer.Shapes = next;

            ServiceProvider.GetService<ISelectionService>()?.Select(layer, path);
        }

        private GroupShapeViewModel? Group(LayerContainerViewModel? layer, ISet<BaseShapeViewModel>? shapes, string name)
        {
            var Project = ServiceProvider.GetService<ProjectEditorViewModel>()?.Project;
            if (Project is null)
            {
                return null;
            }

            if (layer is null || shapes is null)
            {
                return null;
            }
            var source = layer.Shapes.ToBuilder();
            var group = ServiceProvider.GetService<IViewModelFactory>()?.CreateGroupShape(name);
            group?.Group(shapes, source);

            var previous = layer.Shapes;
            var next = source.ToImmutable();
            Project?.History?.Snapshot(previous, next, (p) => layer.Shapes = p);
            layer.Shapes = next;

            return group;

        }

        private void Ungroup(LayerContainerViewModel? layer, ISet<BaseShapeViewModel>? shapes)
        {
            var Project = ServiceProvider.GetService<ProjectEditorViewModel>()?.Project;
            if (Project is null)
            {
                return;
            }

            if (layer is null || shapes is null)
            {
                return;
            }
            var source = layer.Shapes.ToBuilder();

            foreach (var shape in shapes)
            {
                if (shape is GroupShapeViewModel group)
                {
                    group.Ungroup(source);
                }
            }

            var previous = layer.Shapes;
            var next = source.ToImmutable();
            Project?.History?.Snapshot(previous, next, (p) => layer.Shapes = p);
            layer.Shapes = next;
        }

        public GroupShapeViewModel? Group(ISet<BaseShapeViewModel>? shapes, string name)
        {
            var Project = ServiceProvider.GetService<ProjectEditorViewModel>()?.Project;
            if (Project is null)
            {
                return null;
            }

            var layer = Project?.CurrentContainer?.CurrentLayer;
            return layer is { } ? Group(layer, shapes, name) : null;
        }

        public bool Ungroup(ISet<BaseShapeViewModel>? shapes)
        {
            var Project = ServiceProvider.GetService<ProjectEditorViewModel>()?.Project;
            if (Project is null)
            {
                return false;
            }

            var layer = Project?.CurrentContainer?.CurrentLayer;
            if (layer is null || shapes is null)
            {
                return false;
            }
            Ungroup(layer, shapes);
            return true;

        }

        private void Swap(BaseShapeViewModel shape, int sourceIndex, int targetIndex)
        {
            var Project = ServiceProvider.GetService<ProjectEditorViewModel>()?.Project;
            if (Project is null)
            {
                return;
            }

            var layer = Project?.CurrentContainer?.CurrentLayer;
            if (layer?.Shapes is null)
            {
                return;
            }
            if (sourceIndex < targetIndex)
            {
                Project.SwapShape(layer, shape, targetIndex + 1, sourceIndex);
            }
            else
            {
                if (layer.Shapes.Length + 1 > sourceIndex + 1)
                {
                    Project.SwapShape(layer, shape, targetIndex, sourceIndex + 1);
                }
            }
        }

        public void BringToFront(BaseShapeViewModel source)
        {
            var Project = ServiceProvider.GetService<ProjectEditorViewModel>()?.Project;
            if (Project is null)
            {
                return;
            }

            var layer = Project?.CurrentContainer?.CurrentLayer;
            if (layer is null)
            {
                return;
            }
            var items = layer.Shapes;
            var sourceIndex = items.IndexOf(source);
            var targetIndex = items.Length - 1;
            if (targetIndex >= 0 && sourceIndex != targetIndex)
            {
                Swap(source, sourceIndex, targetIndex);
            }
        }

        public void BringForward(BaseShapeViewModel source)
        {
            var Project = ServiceProvider.GetService<ProjectEditorViewModel>()?.Project;
            if (Project is null)
            {
                return;
            }

            var layer = Project?.CurrentContainer?.CurrentLayer;
            if (layer is null)
            {
                return;
            }
            var items = layer.Shapes;
            var sourceIndex = items.IndexOf(source);
            var targetIndex = sourceIndex + 1;
            if (targetIndex < items.Length)
            {
                Swap(source, sourceIndex, targetIndex);
            }
        }

        public void SendBackward(BaseShapeViewModel source)
        {
            var Project = ServiceProvider.GetService<ProjectEditorViewModel>()?.Project;
            if (Project is null)
            {
                return;
            }

            var layer = Project?.CurrentContainer?.CurrentLayer;
            if (layer is null)
            {
                return;
            }
            var items = layer.Shapes;
            var sourceIndex = items.IndexOf(source);
            var targetIndex = sourceIndex - 1;
            if (targetIndex >= 0)
            {
                Swap(source, sourceIndex, targetIndex);
            }
        }

        public void SendToBack(BaseShapeViewModel source)
        {
            var Project = ServiceProvider.GetService<ProjectEditorViewModel>()?.Project;
            if (Project is null)
            {
                return;
            }

            var layer = Project?.CurrentContainer?.CurrentLayer;
            if (layer is null)
            {
                return;
            }
            var items = layer.Shapes;
            var sourceIndex = items.IndexOf(source);
            var targetIndex = 0;
            if (sourceIndex != targetIndex)
            {
                Swap(source, sourceIndex, targetIndex);
            }
        }

        public void MoveShapesBy(IEnumerable<BaseShapeViewModel> shapes, decimal dx, decimal dy)
        {
            foreach (var shape in shapes)
            {
                if (!shape.State.HasFlag(ShapeStateFlags.Locked))
                {
                    shape.Move(null, dx, dy);
                }
            }
            ServiceProvider.GetService<ISelectionService>()?.OnUpdateDecorator();
        }

        private void MoveShapesByWithHistory(List<BaseShapeViewModel>? shapes, decimal dx, decimal dy)
        {
            var Project = ServiceProvider.GetService<ProjectEditorViewModel>()?.Project;
            if (Project is null)
            {
                return;
            }

            if (shapes is null)
            {
                return;
            }
            MoveShapesBy(shapes, dx, dy);
            ServiceProvider.GetService<ISelectionService>()?.OnUpdateDecorator();

            var previous = new { DeltaX = -dx, DeltaY = -dy, Shapes = shapes };
            var next = new { DeltaX = dx, DeltaY = dy, Shapes = shapes };
            Project?.History?.Snapshot(previous, next, (s) =>
            {
                if (s is null)
                {
                    return;
                }
                MoveShapesBy(s.Shapes, s.DeltaX, s.DeltaY);
            });
        }

        public void MoveBy(ISet<BaseShapeViewModel>? shapes, decimal dx, decimal dy)
        {
            var Project = ServiceProvider.GetService<ProjectEditorViewModel>()?.Project;
            if (Project is null)
            {
                return;
            }

            if (shapes is null)
            {
                return;
            }
            
            switch (Project?.Options?.MoveMode)
            {
                case MoveMode.Point:
                {
                    var points = new List<PointShapeViewModel>();

                    foreach (var shape in shapes)
                    {
                        if (!shape.State.HasFlag(ShapeStateFlags.Locked))
                        {
                            shape.GetPoints(points);
                        }
                    }

                    var distinct = points.Distinct().Cast<BaseShapeViewModel>().ToList();
                    MoveShapesByWithHistory(distinct, dx, dy);
                    break;
                }
                case MoveMode.Shape:
                {
                    var items = shapes.Where(s => !s.State.HasFlag(ShapeStateFlags.Locked)).ToList();
                    MoveShapesByWithHistory(items, dx, dy);
                    break;
                }
            }
        }

        public void MoveItem(LibraryViewModel libraryViewModel, int sourceIndex, int targetIndex)
        {
            var Project = ServiceProvider.GetService<ProjectEditorViewModel>()?.Project;
            if (Project is null)
            {
                return;
            }

            if (sourceIndex < targetIndex)
            {
                var item = libraryViewModel.Items[sourceIndex];
                var builder = libraryViewModel.Items.ToBuilder();
                builder.Insert(targetIndex + 1, item);
                builder.RemoveAt(sourceIndex);

                var previous = libraryViewModel.Items;
                var next = builder.ToImmutable();
                Project?.History?.Snapshot(previous, next, (p) => libraryViewModel.Items = p);
                libraryViewModel.Items = next;
            }
            else
            {
                var removeIndex = sourceIndex + 1;
                if (libraryViewModel.Items.Length + 1 > removeIndex)
                {
                    var item = libraryViewModel.Items[sourceIndex];
                    var builder = libraryViewModel.Items.ToBuilder();
                    builder.Insert(targetIndex, item);
                    builder.RemoveAt(removeIndex);

                    var previous = libraryViewModel.Items;
                    var next = builder.ToImmutable();
                    Project?.History?.Snapshot(previous, next, (p) => libraryViewModel.Items = p);
                    libraryViewModel.Items = next;
                }
            }
        }

        public void SwapItem(LibraryViewModel libraryViewModel, int sourceIndex, int targetIndex)
        {
            var Project = ServiceProvider.GetService<ProjectEditorViewModel>()?.Project;
            if (Project is null)
            {
                return;
            }

            var item1 = libraryViewModel.Items[sourceIndex];
            var item2 = libraryViewModel.Items[targetIndex];
            var builder = libraryViewModel.Items.ToBuilder();
            builder[targetIndex] = item1;
            builder[sourceIndex] = item2;

            var previous = libraryViewModel.Items;
            var next = builder.ToImmutable();
            Project?.History?.Snapshot(previous, next, (p) => libraryViewModel.Items = p);
            libraryViewModel.Items = next;
        }

        public void InsertItem(LibraryViewModel libraryViewModel, ViewModelBase item, int index)
        {
            var Project = ServiceProvider.GetService<ProjectEditorViewModel>()?.Project;
            if (Project is null)
            {
                return;
            }

            var builder = libraryViewModel.Items.ToBuilder();
            builder.Insert(index, item);

            var previous = libraryViewModel.Items;
            var next = builder.ToImmutable();
            Project?.History?.Snapshot(previous, next, (p) => libraryViewModel.Items = p);
            libraryViewModel.Items = next;
        }
    }
}
