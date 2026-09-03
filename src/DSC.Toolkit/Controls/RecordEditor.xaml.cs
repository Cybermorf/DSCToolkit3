using DSC.Toolkit.Core.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace DSC.Toolkit.Controls;

public sealed partial class RecordEditor : UserControl
{
    public static readonly DependencyProperty RecordProperty = DependencyProperty.Register(
        nameof(Record), typeof(ToolRecord), typeof(RecordEditor), new PropertyMetadata(null, OnRecordChanged));

    public ToolRecord? Record
    {
        get => (ToolRecord?)GetValue(RecordProperty);
        set => SetValue(RecordProperty, value);
    }

    public RecordEditor() => InitializeComponent();

    private static void OnRecordChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args) =>
        ((RecordEditor)dependencyObject).BuildFields();

    private void BuildFields()
    {
        FieldsPanel.Children.Clear();
        if (Record is null)
        {
            FieldsPanel.Children.Add(new TextBlock { Text = "Select or create a record to edit it." });
            return;
        }

        var nameBox = new TextBox { Header = "Record name", Text = Record.Name };
        nameBox.TextChanged += (_, _) => { Record.Name = nameBox.Text; Record.ModifiedUtc = DateTimeOffset.UtcNow; };
        FieldsPanel.Children.Add(nameBox);

        foreach (var field in Record.Fields.ToArray())
        {
            var fieldName = field.Key;
            var box = new TextBox { Header = fieldName, Text = field.Value, AcceptsReturn = true, TextWrapping = TextWrapping.Wrap, MinHeight = 52 };
            box.TextChanged += (_, _) => { Record.Fields[fieldName] = box.Text; Record.ModifiedUtc = DateTimeOffset.UtcNow; };
            FieldsPanel.Children.Add(box);
        }
    }
}
