using CSharpMath.SkiaSharp;
using EmissionWiz.Models.Interfaces.Managers;
using EmissionWiz.Models.Reports.Areas;
using EmissionWiz.Models.Reports.Blocks;
using MigraDocCore.DocumentObjectModel;
using MigraDocCore.DocumentObjectModel.MigraDoc.DocumentObjectModel.Shapes;
using MigraDocCore.Rendering;
using PdfSharpCore.Utils;
using SixLabors.ImageSharp.PixelFormats;
using System.Runtime.InteropServices;

namespace EmissionWiz.Logic.Managers;

public class CalculationReportManager : ICalculationReportManager
{
    private readonly List<BaseBlock> _blocks = new();
    private readonly IPdfManager _pdfManager;

    public CalculationReportManager(IPdfManager pdfManager)
    {
        _pdfManager = pdfManager;
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
        var blockComment = block.Comment;
        if (!string.IsNullOrWhiteSpace(blockComment))
        {
            var blockCommentParagraph = section.AddParagraph(blockComment);
            blockCommentParagraph.Format.SpaceAfter = Unit.FromCentimeter(0.35);
        }

        foreach (var (formula, model) in block.Formulas)
        {
            var latex = formula.Format(model);
            var comment = formula.Comment;
            
            if (!string.IsNullOrWhiteSpace(comment))
            {
                var commentParagraph = section.AddParagraph(comment);
                commentParagraph.Format.SpaceAfter = Unit.FromCentimeter(0.35);
            }
            
            var painter = new MathPainter()
            {
                LaTeX = latex,
                FontSize = 12
            };

            var image = painter.DrawAsStream(format: SkiaSharp.SKEncodedImageFormat.Png)
                ?? throw new InvalidOperationException("Failed to draw formula");

            if (ImageSource.ImageSourceImpl == null)
                ImageSource.ImageSourceImpl = new ImageSharpImageSource<Rgba32>();

            var sectionImage = section.AddImage(ImageSource.FromStream(Guid.NewGuid().ToString(), () => image, 100));
            section.AddParagraph();
        }
    }
}
