using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using MvcSPAWin8Client.Model;

namespace MvcSPAWin8Client
{
    public class MyGridView : GridView
    {
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            var obj = item as TodoList;
            var gi = element as GridViewItem;
            if (obj.Todos != null && obj.Todos.Count() > 4)
            {
                //int height = obj.Todos.Count() * 150 + 200;
                int height = 500;
                gi.SetValue(VariableSizedWrapGrid.HeightProperty, height);
                gi.SetValue(VariableSizedWrapGrid.MinHeightProperty, height);
                gi.SetValue(VariableSizedWrapGrid.MaxHeightProperty, height);
            }
            else
            {
                gi.SetValue(VariableSizedWrapGrid.HeightProperty, 500);
                gi.SetValue(VariableSizedWrapGrid.MinHeightProperty, 500);
                gi.SetValue(VariableSizedWrapGrid.MaxHeightProperty, 500);
            }
            gi.VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Top;
            gi.VerticalContentAlignment = Windows.UI.Xaml.VerticalAlignment.Top;

            base.PrepareContainerForItemOverride(gi, item);
        }
    }
}
