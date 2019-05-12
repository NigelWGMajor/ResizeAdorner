using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace ResizeAdorner
{
    public class ResizingAdorner : Adorner
    {
        private Thumb _topLeft, _topRight, _bottomRight, _bottomLeft, _left, _top, _right, _bottom;
        private readonly FrameworkElement _adornedElement;
        private readonly VisualCollection _visualChildren;
        private readonly Brush _brush = new SolidColorBrush(Colors.LightPink);
        private Thickness _thickness;
        private double _opacity = 0.3;
        private double _minWidth, _minHeight;

        /// <summary>
        /// Thickness defines width of drag zone for Left, Top, Right and Bottom respectively.
        /// Defaults to (0, 0, 6.0, 6.0)
        /// </summary>
        /// <param name="thickness"></param>

        public ResizingAdorner(UIElement adornedElement, Thickness thickness)
            : base(adornedElement)
        {

            _thickness = thickness;
            _visualChildren = new VisualCollection(this);
            _adornedElement = adornedElement as FrameworkElement;
            MakeThicknessThumbs(_adornedElement);
            var x = AdornerLayer.GetAdornerLayer(adornedElement);
            x.Add(this);
            EnsureValidSize(_adornedElement);
            _minWidth = _adornedElement.MinWidth.Equals(Double.NaN)
                ? 10 + thickness.Left + thickness.Right
                : _adornedElement.MinWidth;
            _minHeight = _adornedElement.MinHeight.Equals(Double.NaN)
               ? 10 + thickness.Top + thickness.Bottom
               : _adornedElement.MinHeight;
            _adornedElement.MinWidth = _minWidth;
            _adornedElement.MinHeight = _minHeight;
            MinHeight = _minHeight;
            MinWidth = _minWidth;
            if (!(_adornedElement.Parent is Canvas))
            {
                _thickness.Left = _thickness.Top = 0;
            }
        }
        private void MakeThicknessThumbs(FrameworkElement adornedElement)
        {
            // For each edge, make a thumb if the edge is not zero.
            if (_thickness.Left != 0.0)
            {
                _left = new Thumb
                {
                    Width = Math.Abs(_thickness.Left),
                    Height = Double.NaN,
                    Background = _brush,
                    Opacity = _opacity,
                    BorderBrush = new SolidColorBrush(Colors.Black),
                    BorderThickness = new Thickness(1)
                };
                _left.DragDelta += new DragDeltaEventHandler(DragLeft);
                _left.Cursor = Cursors.SizeWE;
            }
            if (_thickness.Top != 0.0)
            {
                _top = new Thumb
                {
                    Width = Double.NaN,
                    Height = Math.Abs(_thickness.Top),
                    Background = _brush,
                    Opacity = _opacity,
                    BorderBrush = new SolidColorBrush(Colors.Black),
                    BorderThickness = new Thickness(1)
                };
                _top.DragDelta += new DragDeltaEventHandler(DragTop);
                _top.Cursor = Cursors.SizeNS;
            }
            if (_thickness.Right != 0.0)
            {
                _right = new Thumb
                {
                    Width = Math.Abs(_thickness.Right),
                    Height = Double.NaN,
                    Background = _brush,
                    Opacity = _opacity,
                    BorderBrush = new SolidColorBrush(Colors.Black),
                    BorderThickness = new Thickness(1)
                };
                _right.DragDelta += new DragDeltaEventHandler(DragRight);
                _right.Cursor = Cursors.SizeWE;
            }
            if (_thickness.Bottom != 0.0)
            {
                _bottom = new Thumb
                {
                    Width = Double.NaN,
                    Height = Math.Abs(_thickness.Bottom),
                    Background = _brush,
                    Opacity = _opacity,
                    BorderBrush = new SolidColorBrush(Colors.Black),
                    BorderThickness = new Thickness(1)
                };
                _bottom.DragDelta += new DragDeltaEventHandler(DragBottom);
                _bottom.Cursor = Cursors.SizeNS;
            }
            // for each corner, make a corner thumb if both adjacent edges are non-zero.
            if (_thickness.Left * _thickness.Top != 0.0)
            {
                _topLeft = new Thumb
                {
                    Width = 2 * Math.Abs(_thickness.Left),
                    Height = 2 * Math.Abs(_thickness.Top),
                    Background = _brush,
                    Opacity = _opacity,
                    BorderBrush = new SolidColorBrush(Colors.Black),
                    BorderThickness = new Thickness(1)
                };
                _topLeft.DragDelta += new DragDeltaEventHandler(DragTopLeft);
                _topLeft.Cursor = Cursors.SizeNWSE;
            }
            if (_thickness.Top * _thickness.Right != 0.0)
            {
                _topRight = new Thumb
                {
                    Width = 2 * Math.Abs(_thickness.Right),
                    Height = 2 * Math.Abs(_thickness.Top),
                    Background = _brush,
                    Opacity = _opacity,
                    BorderBrush = new SolidColorBrush(Colors.Black),
                    BorderThickness = new Thickness(1)
                };
                _topRight.DragDelta += new DragDeltaEventHandler(DragTopRight);
                _topRight.Cursor = Cursors.SizeNESW;
            }
            if (_thickness.Bottom * _thickness.Left != 0)
            {
                _bottomLeft = new Thumb
                {
                    Width = 2 * Math.Abs(_thickness.Left),
                    Height = 2 * Math.Abs(_thickness.Bottom),
                    Background = _brush,
                    Opacity = _opacity,
                    BorderBrush = new SolidColorBrush(Colors.Black),
                    BorderThickness = new Thickness(1)
                };
                _bottomLeft.DragDelta += new DragDeltaEventHandler(DragBottomLeft);
                _bottomLeft.Cursor = Cursors.SizeNESW;
            }
            if (_thickness.Bottom * _thickness.Right != 0)
            {
                _bottomRight = new Thumb
                {
                    Width = 2 * Math.Abs(_thickness.Right),
                    Height = 2 * Math.Abs(_thickness.Bottom),
                    Background = _brush,
                    Opacity = _opacity,
                    BorderBrush = new SolidColorBrush(Colors.Black),
                    BorderThickness = new Thickness(1)
                };
                _bottomRight.DragDelta += new DragDeltaEventHandler(DragBottomRight);
                _bottomRight.Cursor = Cursors.SizeNWSE;
            }
            if (_left != null)
                _visualChildren.Add(_left);
            if (_top != null)
                _visualChildren.Add(_top);
            if (_right != null)
                _visualChildren.Add(_right);
            if (_bottom != null)
                _visualChildren.Add(_bottom);
            if (_topLeft != null)
                _visualChildren.Add(_topLeft);
            if (_topRight != null)
                _visualChildren.Add(_topRight);
            if (_bottomRight != null)
                _visualChildren.Add(_bottomRight);
            if (_bottomLeft != null)
                _visualChildren.Add(_bottomLeft);
        }

        private void DragLeft(object sender, DragDeltaEventArgs e)
        {
            Point p = GetLocation();
            p.X += e.HorizontalChange;
            SetLocation(p);
            if ((Keyboard.Modifiers & ModifierKeys.Shift) != ModifierKeys.Shift)
                _adornedElement.Width = Math.Max(_adornedElement.Width - e.HorizontalChange, _minWidth);

        }

        private void DragTop(object sender, DragDeltaEventArgs e)
        {
            Point p = GetLocation();
            p.Y += e.VerticalChange;
            SetLocation(p);
            if ((Keyboard.Modifiers & ModifierKeys.Shift) != ModifierKeys.Shift)
                _adornedElement.Height = Math.Max(_adornedElement.Height - e.VerticalChange, _minHeight);

        }

        private void DragRight(object sender, DragDeltaEventArgs e)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Shift) != ModifierKeys.Shift)
                _adornedElement.Width = Math.Max(_adornedElement.Width + e.HorizontalChange, _minWidth);
            else
            {
                DragLeft(sender, e);
            }
        }

        private void DragBottom(object sender, DragDeltaEventArgs e)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Shift) != ModifierKeys.Shift)
                _adornedElement.Height = Math.Max(_adornedElement.Height + e.VerticalChange, _minHeight);
            else
            {
                DragTop(sender, e);
            }
        }

        private void DragTopLeft(object sender, DragDeltaEventArgs e)
        {
            double dX = e.HorizontalChange;
            double dY = e.VerticalChange;
            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                double ratio = Width / Height;
                if (Math.Abs(e.HorizontalChange) > Math.Abs(e.VerticalChange))
                    dY = e.HorizontalChange / ratio;
                else
                    dX = e.VerticalChange * ratio;
                Debug.WriteLine($"({dX},{dY})");
            }
            Point p = GetLocation();
            p.X += dX;
            p.Y += dY;
            if ((Keyboard.Modifiers & ModifierKeys.Shift) != ModifierKeys.Shift)
            {
                _adornedElement.Width = Math.Max(_adornedElement.Width - dX, _minWidth);
                _adornedElement.Height = Math.Max(_adornedElement.Height - dY, _minHeight);
            }
            SetLocation(p);
        }

        private void DragTopRight(object sender, DragDeltaEventArgs e)
        {
            double dX = e.HorizontalChange;
            double dY = e.VerticalChange;
            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                double ratio = _adornedElement.Width / _adornedElement.Height;
                double x = Math.Min(e.HorizontalChange, e.VerticalChange);
                dY = (e.HorizontalChange - e.VerticalChange) / 2;
                dX = (e.VerticalChange + e.HorizontalChange) / 2 * ratio;
            }
            Point p = GetLocation();
           
            if ((Keyboard.Modifiers & ModifierKeys.Shift) != ModifierKeys.Shift)
            {
                if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                {
                    double ratio = _adornedElement.Width / _adornedElement.Height;
                    _adornedElement.Width = Math.Max(_adornedElement.Width + dX, _minWidth);
                    _adornedElement.Height = _adornedElement.Width / ratio;
                    p.Y -= dX / ratio;
                }
                else
                {
                    _adornedElement.Height = Math.Max(_adornedElement.Height - dY, _minHeight);
                    _adornedElement.Width = Math.Max(_adornedElement.Width + dX, _minWidth);
                    p.Y += dY;
                }
            }
            else
            {
                p.X += dX;
                p.Y += dY;
            }
            SetLocation(p);
        }

        private void DragBottomLeft(object sender, DragDeltaEventArgs e)
        {
            double dX = e.HorizontalChange;
            double dY = e.VerticalChange;
            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                double ratio = _adornedElement.Width / _adornedElement.Height;
                if (Math.Abs(e.HorizontalChange) > Math.Abs(e.VerticalChange))
                    dY = e.HorizontalChange / ratio;
                else
                    dX = e.VerticalChange * ratio;
            }
            Point p = GetLocation();
            p.X += dX;
            if ((Keyboard.Modifiers & ModifierKeys.Shift) != ModifierKeys.Shift)
            {
                _adornedElement.Width = Math.Max(_adornedElement.Width - dX, _minWidth);
                _adornedElement.Height = Math.Max(_adornedElement.Height + dY, _minHeight);
            }
            else
            {
                p.Y += dY;
            }
            SetLocation(p);
        }

        private void DragBottomRight(object sender, DragDeltaEventArgs e)
        {
            double dX = e.HorizontalChange;
            double dY = e.VerticalChange;
            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                double ratio = Width / Height;
                if (Math.Abs(e.HorizontalChange) > Math.Abs(e.VerticalChange))
                    dY = e.HorizontalChange / ratio;
                else
                    dX = e.VerticalChange * ratio;
            }
            Point p = GetLocation();
            p.X += dX;
            p.Y += dY;
            if ((Keyboard.Modifiers & ModifierKeys.Shift) != ModifierKeys.Shift)
            {
                _adornedElement.Width = Math.Max(_adornedElement.Width + dX, _minWidth);
                _adornedElement.Height = Math.Max(_adornedElement.Height + dY, _minHeight);
                this.ArrangeOverride(new Size(_adornedElement.Width, _adornedElement.Height));
            }
            else
            {
                SetLocation(p);
            }
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            double desiredWidth = Math.Max(finalSize.Width, _minWidth);
            double desiredHeight = Math.Max(finalSize.Height, _minHeight);
            _topLeft?.Arrange(SafeRect(0, 0, 2 * _thickness.Left, 2 * _thickness.Top));
            _top?.Arrange(SafeRect(2 * _thickness.Left, 0, desiredWidth - 2 * _thickness.Left - 2 * _thickness.Right, _thickness.Top));
            _topRight?.Arrange(SafeRect(desiredWidth - 2 * _thickness.Right, 0, 2 * _thickness.Right, 2 * _thickness.Top));
            _left?.Arrange(SafeRect(0, 2 * _thickness.Top, _thickness.Left, desiredHeight - 2 * _thickness.Top - 2 * _thickness.Bottom));
            _right?.Arrange(SafeRect(desiredWidth - _thickness.Right, 2 * _thickness.Top, _thickness.Right, desiredHeight - 2 * _thickness.Top - 2 * _thickness.Bottom));
            _bottomLeft?.Arrange(SafeRect(0, desiredHeight - 2 * _thickness.Bottom, 2 * _thickness.Left, 2 * _thickness.Bottom));
            _bottom?.Arrange(SafeRect(2 * _thickness.Left, desiredHeight - _thickness.Bottom, desiredWidth - 2 * _thickness.Left - 2 * _thickness.Right, _thickness.Bottom));
            _bottomRight?.Arrange(SafeRect(desiredWidth - 2.0 * _thickness.Right,
                desiredHeight - 2.0 * _thickness.Bottom, _thickness.Right * 2.0, _thickness.Bottom * 2.0));
            if (_adornedElement.Width < Width)
                _adornedElement.Width = Width;
            if (_adornedElement.Height < Height)
                _adornedElement.Height = Height;
            return finalSize;
        }

        private void EnsureValidSize(FrameworkElement adornedElement)
        {
            if (adornedElement.Width.Equals(Double.NaN))
                adornedElement.Width = Math.Max(adornedElement.DesiredSize.Width, _minWidth); // + _thickness.Left + _thickness.Right;
            if (adornedElement.Height.Equals(Double.NaN))
                adornedElement.Height = Math.Max(adornedElement.DesiredSize.Height, _minHeight); // + _thickness.Top + _thickness.Bottom;
            if (Width.Equals(Double.NaN))
                Width = adornedElement.Width;
            if (Height.Equals(Double.NaN))
                Height = adornedElement.Height;
        }

        protected override int VisualChildrenCount
        {
            get { return _visualChildren.Count; }
        }

        protected override Visual GetVisualChild(int index)
        {
            return _visualChildren[index];
        }
        private Point GetLocation()
        {
            Point p;

            p = new Point(
                Canvas.GetLeft(_adornedElement),
                Canvas.GetTop(_adornedElement));
            if (p.X.Equals(Double.NaN)) p.X = 11.11;
            if (p.Y.Equals(Double.NaN)) p.Y = 11.11;
            return p;
        }

        private void SetLocation(Point point)
        {
            if (!(_adornedElement.Parent is Canvas))
                return;
            EnsureValidSize(_adornedElement);
            Canvas.SetLeft(_adornedElement, point.X);
            Canvas.SetTop(_adornedElement, point.Y);
            this.ArrangeOverride(new Size(_adornedElement.Width, _adornedElement.Height));
        }

        private Rect SafeRect(double a, double b, double c, double d)
        {
            return new Rect(a, b, ZeroOr(c), ZeroOr(d));
        }
        private double ZeroOr(double value)
        {
            return value < 0 ? 0 : value;
        }
    }
}
