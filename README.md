# How to resize the row based on the Height of multi line textbox entry in WPF DataGrid (SfDataGrid)?

## About the sample

This example illustrates how to resize the row based on the Height of multi line textbox entry in [WPF DataGrid](https://www.syncfusion.com/wpf-ui-controls/datagrid) (SfDataGrid)?	

You can automatically increase the height of a row when typing multiline text in the text box of [GridTemplateColumn](https://help.syncfusion.com/cr/wpf/Syncfusion.UI.Xaml.Grid.GridTemplateColumn.html) using the [QueryRowHeight](https://help.syncfusion.com/cr/wpf/Syncfusion.UI.Xaml.Grid.SfDataGrid.html#Syncfusion_UI_Xaml_Grid_SfDataGrid_QueryRowHeight) event in [WPF DataGrid](https://www.syncfusion.com/wpf-ui-controls/datagrid) (SfDataGrid).


In the sample, the height of a row is increased after editing the text box of GridTemplateColumn.

```XML
<syncfusion:SfDataGrid x:Name="grid"
                        RowHeight="24"
                        ShowRowHeader="True"
                        AllowEditing="True"  
                        AllowGrouping="True"
                        ShowGroupDropArea="True"
                        ItemsSource="{Binding Emp}">
    <interactivity:Interaction.Behaviors>
        <local:SfDataGridBehavior/>
    </interactivity:Interaction.Behaviors>
    <syncfusion:SfDataGrid.Resources>
        <ResourceDictionary>
            <Style TargetType="TextBox">
                <Setter Property="AcceptsReturn"
                        Value="True" />
            </Style>
        </ResourceDictionary>
    </syncfusion:SfDataGrid.Resources>
  
    <syncfusion:SfDataGrid.Columns>
        <syncfusion:GridTemplateColumn.CellTemplate>
            <DataTemplate>
                 <TextBlock Text="{Binding EmployeeInfo}" TextWrapping="Wrap" />
            </DataTemplate>
        </syncfusion:GridTemplateColumn.CellTemplate>
        <syncfusion:GridTemplateColumn.EditTemplate>
            <DataTemplate>
                 <TextBox AcceptsReturn="True" ScrollViewer.VerticalScrollBarVisibility="Visible" TextWrapping="Wrap" Text="{Binding EmployeeInfo,Mode=TwoWay}"  />  
            </DataTemplate>
        </syncfusion:GridTemplateColumn.EditTemplate>
    </syncfusion:SfDataGrid.Columns>
</syncfusion:SfDataGrid>
```

```C#
datagrid.QueryRowHeight += datagrid_QueryRowHeight;
  
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
```

The CurrentCellValueChanged event will be triggered when editing a particular cell. So, you can change the row height by calling the SfDataGrid.InvalidateRowHeight and VisualContainer.InvalidateMeasureInfo methods.

```C#
datagrid.CurrentCellValueChanged += datagrid_CurrentCellValueChanged;
  
private void datagrid_CurrentCellValueChanged(object sender, CurrentCellValueChangedEventArgs args)
{
    datagrid.InvalidateRowHeight(args.RowColumnIndex.RowIndex);
  
    datagrid.GetVisualContainer().InvalidateMeasureInfo();
}
```

You can measure the text height when editing the text box of GridTemplateColumn using the GridColumnSizer.MeasureTemplate method.

```C#
public class GridColumnAutoSizerExt : GridColumnSizer
{
    public GridColumnAutoSizerExt(SfDataGrid grid): base(grid)
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
```

In WPF, you can get the formatted text height by using the following code snippet.

```C#
private FormattedText GetFormattedText(GridColumn column, object record, string datatext)
{
        FormattedText formattedtext;
        formattedtext = new FormattedText(datatext, System.Globalization.CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface(DataGrid.FontFamily, new FontStyle(), FontWeights.Normal, DataGrid.FontStretch), DataGrid.FontSize, Brushes.Black);
        return formattedtext;  
}
```

In UWP, you can get the formatted text height by using the following code snippet.

```C#
protected override Size MeasureTemplate(Size rect, object record, GridColumn column, GridQueryBounds bounds)
{
    var data = record.GetType().GetProperty(column.MappingName).GetValue(record);
    var datatext = Convert.ToString(data);
    return GetTextHeight(datatext);
}
  
private Size GetTextHeight(string dataText)
{
    TextBlock textBlock = new TextBlock { Text = dataText, FontSize = DataGrid.FontSize, FontFamily = DataGrid.FontFamily, FontStretch = DataGrid.FontStretch, FontStyle = new FontStyle(), FontWeight = FontWeights.Normal, FlowDirection = FlowDirection.LeftToRight };
    textBlock.Measure(new Size(Double.PositiveInfinity, double.PositiveInfinity));
    Size defaultFormattedText = textBlock.DesiredSize;
    return defaultFormattedText;
}
```

## Requirements to run the demo
Visual Studio 2015 and above versions
