using EmissionWiz.Models.Helpers;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System.Linq.Expressions;

namespace EDW.AtoSales.Logic.Classes;

public enum NpoiReportColumnType
{
    String,
    Date,
    Percent
}

public class NpoiReport<T>
    where T : class
{
    class ColumnDescription
    {
        public string? FieldName { get; set; }
        public string? DisplayName { get; set; }
        public Func<T, object?>? GetData { get; set; }
        public Func<T, object?>? Formatter { get; set; }
        public int? Width { get; set; }
        public NpoiReportColumnType Type { get; set; }
    }

    private readonly IWorkbook _wb;
    private readonly string _name;
    private readonly List<ColumnDescription> _columns = new();
    private readonly string? _title;

    public NpoiReport(IWorkbook wb, string name, string? title = null)
    {
        _wb = wb;
        _name = name;
        _title = title;
    }

    public void AddColumn(Expression<Func<T, object?>>? fieldSelector, string displayName, int? width = null, NpoiReportColumnType? type = null, Func<T, object?>? formatter = null)
    {
        var fieldName = fieldSelector == null ? null : ExpressionHelper.GetFieldName(fieldSelector);
        _columns.Add(new ColumnDescription
        {
            DisplayName = displayName,
            FieldName = fieldName,
            GetData = fieldSelector?.Compile(),
            Formatter = formatter,
            Width = width,
            Type = type ?? NpoiReportColumnType.String
        });
    }

    private int GetNextTableId()
    {
        var result = 0;
        var numberOfSheets = _wb.NumberOfSheets;
        for (int i = 0; i < numberOfSheets; i++)
        {
            var sheet = (XSSFSheet)_wb.GetSheetAt(i);
            var tables = sheet.GetTables();
            result += tables.Count;
        }

        return result + 1;
    }

    public void Generate(IList<T> data)
    {
        var ws = (XSSFSheet)_wb.CreateSheet(_name);
        var hasTitle = !string.IsNullOrEmpty(_title)
            ? 1
            : 0;

        var upperRows = hasTitle;
        ws.CreateFreezePane(1, 1 + hasTitle);

        IRow? titleRow = null;

        var headerRow = ws.CreateRow(upperRows++);
        headerRow.Height = 600;

        var headerFont = (XSSFFont)_wb.CreateFont();
        headerFont.FontName = "Segoe UI";
        headerFont.FontHeight = 200;
        headerFont.IsBold = true;

        var headerCellStyle = (XSSFCellStyle)_wb.CreateCellStyle();
        headerCellStyle.WrapText = true;

        var headerColor = new XSSFColor(new byte[] { 240, 240, 240 });
        headerCellStyle.SetFillForegroundColor(headerColor);
        headerCellStyle.FillPattern = FillPattern.SolidForeground;
        headerCellStyle.SetFont(headerFont);
        headerCellStyle.VerticalAlignment = VerticalAlignment.Top;
        headerCellStyle.Alignment = HorizontalAlignment.Center;

        var cellNumber = 0;
        foreach (var column in _columns)
        {
            var headerCell = headerRow.CreateCell(cellNumber);
            headerCell.CellStyle = headerCellStyle;
            headerCell.SetCellValue(column.DisplayName);

            if (column.Width.HasValue)
            {
                ws.SetColumnWidth(cellNumber, column.Width.Value);
            }

            cellNumber++;
        }

        if (!data.Any()) return;

        var regularFont = (XSSFFont)_wb.CreateFont();
        regularFont.FontName = "Segoe UI";
        regularFont.FontHeight = 200;

        var dataCellStyle = (XSSFCellStyle)_wb.CreateCellStyle();
        dataCellStyle.VerticalAlignment = VerticalAlignment.Top;
        dataCellStyle.Alignment = HorizontalAlignment.Left;
        dataCellStyle.SetFont(regularFont);

        var percentStyle = (XSSFCellStyle)_wb.CreateCellStyle();
        percentStyle.SetDataFormat(BuiltinFormats.GetBuiltinFormat("0.00%"));
        percentStyle.SetFont(regularFont);

        IDataFormat dataFormatCustom = _wb.CreateDataFormat();
        var dateCellStyle = (XSSFCellStyle)_wb.CreateCellStyle();
        dateCellStyle.VerticalAlignment = VerticalAlignment.Top;
        dateCellStyle.Alignment = HorizontalAlignment.Left;
        dateCellStyle.SetFont(regularFont);
        dateCellStyle.DataFormat = dataFormatCustom.GetFormat("yyyy-MM-dd");

        var rowNumber = upperRows;
        foreach (var item in data)
        {
            var row = ws.CreateRow(rowNumber);

            cellNumber = 0;
            foreach (var column in _columns)
            {
                var cell = row.CreateCell(cellNumber);
                cell.CellStyle = dataCellStyle;

                object? value = null;
                if (column.Formatter != null)
                {
                    value = column.Formatter(item);
                }
                else if (column.GetData != null)
                {
                    value = column.GetData(item);
                }

                if (value is string)
                {
                    cell.SetCellValue((string)value);
                }

                if (value is DateTime)
                {
                    var date = (DateTime)value;
                    cell.SetCellValue(date);
                    cell.CellStyle = dateCellStyle;
                }

                if (value is int)
                {
                    cell.SetCellValue((int)value);
                }

                if (value is decimal)
                {
                    cell.SetCellValue((double)(decimal)value);

                    if (column.Type == NpoiReportColumnType.Percent)
                    {
                        cell.CellStyle = percentStyle;
                    }
                }

                if (value != null && value.GetType().IsEnum)
                {
                    cell.SetCellValue(value.ToString());
                }

                cellNumber++;
            }

            rowNumber++;
        }

        cellNumber = 0;
        foreach (var column in _columns)
        {
            if (!column.Width.HasValue)
            {
                ws.AutoSizeColumn(cellNumber);
            }

            cellNumber++;
        }

        if (!string.IsNullOrEmpty(_title))
        {
            titleRow = ws.CreateRow(0);
            titleRow.Height = 600;
            var titleCell = titleRow.CreateCell(0);
            titleCell.CellStyle = headerCellStyle;
            titleCell.SetCellValue(_title);
            ws.AddMergedRegion(new CellRangeAddress(0, 0, 0, cellNumber - 1));
        }
    }
}