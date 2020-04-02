using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.Grid.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using System.Windows.Media;

namespace SfDataGridDemo
{
    class SfDataGridBehavior :Behavior<SfDataGrid>
    {
        SfDataGrid datagrid = null;
        ToolTip toolTip = new ToolTip();

        protected override void OnAttached()
        {
            datagrid = this.AssociatedObject as SfDataGrid;
            datagrid.QueryRowHeight += datagrid_QueryRowHeight;
            datagrid.CurrentCellValueChanged += datagrid_CurrentCellValueChanged;
            datagrid.GridColumnSizer = new GridColumnAutoSizerExt(datagrid);
        }

        GridRowSizingOptions gridRowResizingOptions = new GridRowSizingOptions();

        //To get the calculated height from GetAutoRowHeight method.
        double Height;
        private void datagrid_QueryRowHeight(object sender, QueryRowHeightEventArgs e)
        {
            if (this.datagrid.GridColumnSizer.GetAutoRowHeight(e.RowIndex, gridRowResizingOptions, out Height))
            {
                // Reset the Height of the row to datagrid row height when the row height is less than the datagrid row height.

                if (Height < datagrid.RowHeight)
                {
                    Height = datagrid.RowHeight;
                }

                e.Height = Height;

                e.Handled = true;

            }
        }

        private void datagrid_CurrentCellValueChanged(object sender, CurrentCellValueChangedEventArgs args)
        {
            datagrid.InvalidateRowHeight(args.RowColumnIndex.RowIndex);

            datagrid.GetVisualContainer().InvalidateMeasureInfo();
        }

    }

    public class GridColumnAutoSizerExt : GridColumnSizer
    {
        public GridColumnAutoSizerExt(SfDataGrid grid) : base(grid)
        {

        }

        protected override Size MeasureTemplate(Size rect, object record, GridColumn column, GridQueryBounds bounds)
        {
            var data = record.GetType().GetProperty(column.MappingName).GetValue(record);
            var datatext = Convert.ToString(data);

            FormattedText formattedtext = GetFormattedText(column, record, datatext);
            formattedtext.Trimming = TextTrimming.None;

            formattedtext.MaxTextWidth = this.DataGrid.GetVisualContainer().ColumnWidths.DefaultLineSize;
            formattedtext.MaxTextHeight = double.MaxValue;

            if (formattedtext.MaxTextWidth > (Margin.Left + Margin.Right))
                formattedtext.MaxTextWidth -= (Margin.Left + Margin.Right);
            return new Size(formattedtext.Width, formattedtext.Height);
        }


    }
}
