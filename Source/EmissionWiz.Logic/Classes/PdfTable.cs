using System.Linq.Expressions;
using System.Reflection;
using EmissionWiz.Models.Exceptions;
using EmissionWiz.Models.Helpers;
using MigraDocCore.DocumentObjectModel;
using MigraDocCore.DocumentObjectModel.Tables;

namespace EmissionWiz.Logic.Classes;

internal static class CellExtensions
{
    public static void ApplyCellDefaultFormatting(this Cell cell)
    {
        cell.Format.SpaceAfter = 2;
        cell.Format.SpaceBefore = 2;
    }

    public static Paragraph AddParagraph(this Cell cell, string text, double? fontSize = null, ParagraphAlignment? alignment = null, bool? isBold = null)
    {
        var p = cell.AddParagraph(text);
        if (fontSize.HasValue)
        {
            p.Format.Font.Size = Unit.FromPoint(fontSize.Value);
        }

        if (alignment.HasValue)
        {
            p.Format.Alignment = alignment.Value;
        }

        if (isBold.HasValue)
        {
            p.Format.Font.Bold = isBold.Value;
        }

        return p;
    }

    public static Paragraph Format(this Paragraph paragraph, Action<ParagraphFormat> action)
    {
        action(paragraph.Format);
        return paragraph;
    }
}

public class PdfColumnDescription
{
    public string? ColumnName { get; set; }
    public PropertyInfo? PropertyInfo { get; set; }
    public string? Title { get; set; }
    public int ColumnRelativeWidth { get; set; }
    public ParagraphAlignment? Alignment { get; set; }

    public Action<Cell>? OnCellStyle { get; set; }

    public PdfColumnDescription Align(ParagraphAlignment alignment)
    {
        Alignment = alignment;
        return this;
    }

    public PdfColumnDescription AddStyle(Action<Cell> onCellStyle)
    {
        OnCellStyle = onCellStyle;
        return this;
    }
}

public class PdfTableOptions<T>
{
    public Action<Row, T?>? OnRenderRow { get; set; }
    public Action<Table>? OnDefineTableStyles { get; set; }
    public Action<Row>? OnDefineBandHeaderRowStyles { get; set; }
    public Action<Row>? OnDefineHeaderRowStyles { get; set; }
    public Action<Cell, PdfColumnDescription, T?, bool>? OnDefineCellStyles { get; set; }

    public bool HideHeader { get; set; }
    public bool CustomStyling { get; set; }
    public Unit? CustomWidth { get; set; }
    public int? MinRowsCount { get; set; }
}

public class PdfExtraRowDescription<T>
{
    private Func<T, bool> _condition = (_) => true;
    private Func<T, Paragraph>? _formatter;
    private ParagraphAlignment? _alignment;

    public PdfExtraRowDescription<T> When(Func<T, bool> condition)
    {
        if (condition == null) throw new ArgumentNullException(nameof(condition));

        _condition = condition;
        return this;
    }

    public PdfExtraRowDescription<T> AddRow(Func<T, Paragraph> formatter)
    {
        _formatter = formatter;
        return this;
    }

    public PdfExtraRowDescription<T> Align(ParagraphAlignment alignment)
    {
        _alignment = alignment;
        return this;
    }

    public void Render(Table table, T dataRow)
    {
        if (_formatter == null) return;
        if (!_condition(dataRow)) return;

        var row = table.AddRow();
        var cell = row.Cells[0];
        if (table.Columns.Count > 1)
        {
            cell.MergeRight = table.Columns.Count - 1;
        }
        if (_alignment != null)
        {
            cell.Format.Alignment = _alignment.Value;
        }
        cell.Add(_formatter(dataRow));
        cell.ApplyCellDefaultFormatting();
    }
}


public class PdfTable
{
    private readonly Type _dataType;
    private readonly List<PdfColumnDescription> _columns = new List<PdfColumnDescription>();
    private readonly List<Action<Row>> _rowActions = new List<Action<Row>>();

    public PdfTable(Type dataType)
    {
        _dataType = dataType;
    }

    public void AddColumn(Expression<Func<object?, object?>> columnSelector, string fieldName, int columnRelativeWidth, string? title = null)
    {
        var pi = _dataType.GetProperty(fieldName) ?? throw new Exception($"Failed to find property {fieldName} for {_dataType.FullName}");

        var column = new PdfColumnDescription
        {
            PropertyInfo = pi,
            ColumnName = fieldName,
            Title = title ?? ExpressionHelper.GetFieldName(columnSelector),
            ColumnRelativeWidth = columnRelativeWidth
        };
        _columns.Add(column);
    }

    public Table Render(Section? section, IEnumerable<object> data)
    {
        // Create the item table
        var table = section?.AddTable() ?? new Table();

        double availableWidth;
        UnitType unitType;
        if (section != null)
        {
            unitType = UnitType.Centimeter;
            var pageSetup = section.PageSetup;
            var widthCm = section.PageSetup.Orientation == Orientation.Landscape ? pageSetup.PageHeight.Centimeter : pageSetup.PageWidth.Centimeter;
            availableWidth = widthCm - pageSetup.LeftMargin.Centimeter - pageSetup.RightMargin.Centimeter;
        }
        else
        {
            throw new AppException($"Define {nameof(PdfTableOptions<object>.CustomWidth)} t specify table width");
        }

        var columnsColumnRelativeWidthsWidth = _columns.Sum(x => x.ColumnRelativeWidth);
        foreach (var column in _columns)
        {
            table.AddColumn(new Unit(availableWidth * column.ColumnRelativeWidth / columnsColumnRelativeWidthsWidth, unitType));
        }

        var headerRow = table.AddRow();


        var rowColumnNumber = 0;
        foreach (var column in _columns)
        {
            var cell = headerRow.Cells[rowColumnNumber];
            if (column.Alignment.HasValue)
            {
                cell.Format.Alignment = column.Alignment.Value;
            }

            headerRow.Cells[rowColumnNumber].AddParagraph(column.Title);
            rowColumnNumber++;
        }

        var dataRowsCount = 0;
        foreach (var dataRow in data)
        {
            dataRowsCount++;

            var row = table.AddRow();

            var columnNumber = 0;
            foreach (var column in _columns)
            {
                string? text = null;
                if (column.PropertyInfo != null)
                {
                    var fieldValue = column.PropertyInfo.GetValue(dataRow);
                    if (fieldValue != null)
                    {
                        text = fieldValue.ToString();
                    }
                }

                var cell = row.Cells[columnNumber];

                if (column.OnCellStyle != null)
                {
                    column.OnCellStyle(cell);
                }

                if (!string.IsNullOrEmpty(text))
                {
                    cell.AddParagraph(text);
                }

                if (column.Alignment.HasValue)
                {
                    cell.Format.Alignment = column.Alignment.Value;
                }

                columnNumber++;
            }
        }

        foreach (var ra in _rowActions)
        {
            var row = table.AddRow();
            ra(row);
        }

        return table;
    }

    public void AddRow(Action<Row> action)
    {
        _rowActions.Add(action);
    }

    public Cell GetCell(Row row, string columnName, Action<Cell>? cellAction = null)
    {
        var index = _columns.FindIndex(x => x.ColumnName == columnName);
        if (index == -1)
            throw new Exception($"Failed to find column with name {columnName}");

        var cell = row[index];
        cell.ApplyCellDefaultFormatting();

        cellAction?.Invoke(cell);
        return cell;
    }
}