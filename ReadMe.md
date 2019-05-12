# NixNotes
This adorner adds resizing cursors to any edges and corners of a UIElement.
If the UIElement's direct parent is a Canvas, Top and Left will work, otherwise not.

* The corner thumb is twice the thickness of the edge zones.
* If the element is not on a canvas the top and left edges will be zeroed.

## Usage
The adorner is simple to use.  Just use a one-line constructor that takes the control to be resizable.  A thickness is given for the edge resizers, 5 is a middle range.  Corner draggers of twice that size will be placed at corners where both sides have thickness.

Holding shift while dragging will move instead.

Control is meant to preserve aspect ratio but is not very well behaved.

The top-right corner version currently has the most promise.


```cs
private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
{
    new ResizingAdorner(MyBox, new Thickness(6));       // Resizers all edges and corners
    new ResizingAdorner(MyBox, new Thickness(0,0,5,5)); // Resizers Bottom, Right and 
                                                        // bottom-right corner)
}
```




