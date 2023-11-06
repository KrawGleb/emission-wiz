using CSharpMath.SkiaSharp;
using EmissionWiz.Common;
using EmissionWiz.Common.Templates;
using EmissionWiz.Models.Interfaces.Managers;
using EmissionWiz.Models.Reports.Areas;
using EmissionWiz.Models.Reports.Blocks;
using MigraDocCore.DocumentObjectModel;
using MigraDocCore.DocumentObjectModel.MigraDoc.DocumentObjectModel.Shapes;
using MigraDocCore.Rendering;
using PdfSharpCore.Utils;
using SixLabors.ImageSharp.PixelFormats;

namespace EmissionWiz.Logic.Managers;

public class CalculationReportManager : ICalculationReportManager
{
    private readonly IPdfManager _pdfManager;
    private List<BaseBlock> _blocks = new();
    private string? _title;

    public CalculationReportManager(IPdfManager pdfManager)
    {
        _pdfManager = pdfManager;
    }

    public void Reset()
    {
        _blocks = new();
        _title = null;
    }

    public ICalculationReportManager SetTitle(string title)
    {
        _title = title;
        return this;
    }

    public void Generate(Stream destination)
    {
        var document = new Document();

        _pdfManager.DefinePdfStyles(document);

        var section = document.AddSection();
        section.PageSetup.PageFormat = PageFormat.A4;
        section.PageSetup.Orientation = Orientation.Portrait;

        section.PageSetup.PageWidth = Unit.FromCentimeter(21.0);
        section.PageSetup.PageHeight = Unit.FromCentimeter(29.7);

        section.PageSetup.BottomMargin = Unit.FromCentimeter(0);
        section.PageSetup.TopMargin = Unit.FromCentimeter(1);
        section.PageSetup.LeftMargin = Unit.FromCentimeter(1);
        section.PageSetup.RightMargin = Unit.FromCentimeter(0.2);

        section.PageSetup.FooterDistance = "0 cm";

        if (!string.IsNullOrWhiteSpace(_title))
        {
            var title = section.AddParagraph(_title, Constants.Pdf.CustomStyleNames.Title);
            title.Format.Alignment = ParagraphAlignment.Center;
            title.Format.SpaceAfter = Unit.FromCentimeter(0.5);
        }

        foreach (var block in _blocks)
        {
            if (block is FormulaBlock)
            {
                RenderFormulaBlock(section, (FormulaBlock)block);
            }
        }

        var renderer = new PdfDocumentRenderer(true)
        {
            Document = document,
        };
        renderer.RenderDocument();

        renderer.PdfDocument.Save(destination);
    }

    public void AddBlock(BaseBlock block)
    {
        var order = !_blocks.Any()
            ? 1
            : _blocks.Max(x => x.Order) + 1;

        block.Order = order;
        _blocks.Add(block);
    }

    private void RenderFormulaBlock(Section section, FormulaBlock block)
    {
        var blockComment = HbsTemplateManager.Format(block.Comment, Constants.MathCharsObj);
        if (!string.IsNullOrWhiteSpace(blockComment))
        {
            var blockCommentParagraph = section.AddParagraph(blockComment);
            blockCommentParagraph.Format.SpaceAfter = Unit.FromCentimeter(0.35);
        }

        foreach (var nestedBlock in block.NestedBlocks)
        {
            RenderFormulaBlock(section, nestedBlock);
        }

        foreach (var (formula, model) in block.Formulas)
        {
            var latex = formula.Format(model);
            var comment = HbsTemplateManager.Format(formula.Comment, Constants.MathCharsObj); 
            var nearbyComment = HbsTemplateManager.Format(formula.NearbyComment, Constants.MathCharsObj);
            
            if (!string.IsNullOrWhiteSpace(comment))
            {
                var commentParagraph = section.AddParagraph(comment);
                commentParagraph.Format.SpaceAfter = Unit.FromCentimeter(0.35);
            }
            
            var painter = new MathPainter()
            {
                LaTeX = latex,
                FontSize = 13,
                
            };

            

            if (ImageSource.ImageSourceImpl == null)
                ImageSource.ImageSourceImpl = new ImageSharpImageSource<Rgba32>();

            if (string.IsNullOrEmpty(nearbyComment))
            {
                var image = painter.DrawAsStream(format: SkiaSharp.SKEncodedImageFormat.Png)
                    ?? throw new InvalidOperationException("Failed to draw formula");
                var sectionImage = section.AddImage(ImageSource.FromStream(Guid.NewGuid().ToString(), () => image, 100));
            }
            else
            {
                var image = painter.DrawAsStream(format: SkiaSharp.SKEncodedImageFormat.Png, textPainterCanvasWidth: 1000)
                    ?? throw new InvalidOperationException("Failed to draw formula");
                
                var layoutTable = section.AddTable();
                layoutTable.Borders.Visible = false;

                var leftColumn = layoutTable.AddColumn();
                leftColumn.Format.Alignment = ParagraphAlignment.Center;
                leftColumn.Width = Unit.FromCentimeter(section.PageSetup.PageWidth.Centimeter / 2d);

                var rightColumn = layoutTable.AddColumn();
                rightColumn.Format.Alignment = ParagraphAlignment.Left;
                rightColumn.Width = Unit.FromCentimeter(section.PageSetup.PageWidth.Centimeter / 2d);

                var row = layoutTable.AddRow();
                var imageCell = row.Cells[0];

                imageCell.AddImage(ImageSource.FromStream(Guid.NewGuid().ToString(), () => image, 100));

                var commentCell = row.Cells[1];
                commentCell.AddParagraph(nearbyComment);
            }

            section.AddParagraph();
        }
    }
}
