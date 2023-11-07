using CSharpMath;
using CSharpMath.Rendering.Text;
using CSharpMath.SkiaSharp;
using CSharpMath.Display;
using EmissionWiz.Common;
using EmissionWiz.Common.Helpers;
using EmissionWiz.Common.Templates;
using EmissionWiz.Models.Interfaces.Managers;
using Microsoft.Extensions.Logging;
using MigraDocCore.DocumentObjectModel;
using MigraDocCore.DocumentObjectModel.MigraDoc.DocumentObjectModel.Shapes;
using MigraDocCore.Rendering;
using PdfSharpCore.Utils;
using SixLabors.ImageSharp.PixelFormats;
using System.Dynamic;
using System.Xml.Linq;
using Typography.TextBreak;
using System.Xml.XPath;

namespace EmissionWiz.Logic.Managers;

public class ReportManager : IReportManager
{
    private readonly IPdfManager _pdfManager;
    private readonly ILogger<ReportManager> _logger;

    public ReportManager(
        IPdfManager pdfManager, 
        ILogger<ReportManager> logger)
    {
        _pdfManager = pdfManager;
        _logger = logger;
    }

    public async Task FromTemplate(Stream destination, string templatePath, object model)
    {
        var xmlDoc = await LoadEnrichedXml(templatePath, model);
        var pdfDoc = new Document();

        _pdfManager.DefinePdfStyles(pdfDoc);

        var section = pdfDoc.AddSection();
        section.PageSetup.PageFormat = PageFormat.A4;
        section.PageSetup.Orientation = Orientation.Portrait;

        section.PageSetup.PageWidth = Unit.FromCentimeter(21.0);
        section.PageSetup.PageHeight = Unit.FromCentimeter(29.7);

        section.PageSetup.BottomMargin = Unit.FromCentimeter(0);
        section.PageSetup.TopMargin = Unit.FromCentimeter(1);
        section.PageSetup.LeftMargin = Unit.FromCentimeter(1);
        section.PageSetup.RightMargin = Unit.FromCentimeter(0.2);

        section.PageSetup.FooterDistance = "0 cm";

        ParseXmlToPdf(xmlDoc, section);

        var renderer = new PdfDocumentRenderer(true)
        {
            Document = pdfDoc,
        };
        renderer.RenderDocument();

        renderer.PdfDocument.Save(destination);
    }

    private async Task<XDocument> LoadEnrichedXml(string path, object model)
    {
        var fullPath = Path.GetFullPath(path);
        var dirPath = Path.GetDirectoryName(fullPath);

        var flatModel = ExpandoObjectBuilder.FromObject(model, false);

        using var initialXmlFile = File.OpenRead(fullPath);
        var initialXml = await XDocument.LoadAsync(initialXmlFile, LoadOptions.PreserveWhitespace, CancellationToken.None);

        await LoadDocReferences(initialXml, dirPath ?? "");
        var xmlWithReferences = initialXml.ToString();
        var xmlWithValues = HbsTemplateManager.Format(xmlWithReferences, flatModel);

        var textReader = new StringReader(xmlWithValues);
        var xmlDoc = XDocument.Load(textReader);

        return xmlDoc;
    }

    private async Task LoadDocReferences(XDocument xmlDoc, string dirPath)
    {
        var blocks = xmlDoc.Elements("block");
        foreach (var block in blocks)
        {
            await LoadDocReferences(block, dirPath);
        }
    }

    private async Task<XElement> LoadDocReferences(XElement element, string dirPath)
    {
        var referenceAttribute = element.Attribute("ref");
        if (referenceAttribute != null)
        {
            var reference = referenceAttribute.Value;
            var referenceFullPath = dirPath + reference;
            var referenceDirPath = Path.GetDirectoryName(referenceFullPath);

            var xmlDoc = XDocument.Load(referenceFullPath);
            var referenceBlock = xmlDoc.Element("block");

            if (referenceBlock != null)
            {
                var enrichedBlock = await LoadDocReferences(referenceBlock, referenceDirPath);
                element.Elements().Remove();
                element.AddFirst(enrichedBlock);

                return element;
            }
            else
            {
                _logger.LogWarning("Xml reference was null: {0}", referenceFullPath);
            }
        }

        var subBlocks = element.Elements("block").ToList();
        foreach (var subBlock in subBlocks)
        {
            var enrichedBlock = await LoadDocReferences(subBlock, dirPath);
            subBlock.ReplaceWith(enrichedBlock);
        }

        return element;
    }

    private void ParseXmlToPdf(XDocument xml, Section pdf)
    {
        foreach (var element in xml.Elements())
        {
            RenderElement(element, pdf);
        }
    }
   
    private void RenderElement(XElement element, Section pdf)
    {
        var elementName = element.Name.LocalName;
        switch (elementName)
        {
            case "block":
                RenderBlock(element, pdf);
                break;
            case "title":
                RenderTitle(element, pdf);
                break;
            case "text":
                RenderText(element, pdf);
                break;
            case "formula":
                RenderFormula(element, pdf);
                break;
            default:
                _logger.LogInformation("Unknown element name: {0}", elementName);
                break;
        }
    }

    private void RenderBlock(XElement xml, Section pdf)
    {
        foreach (var element in xml.Elements())
            RenderElement(element, pdf);
    }

    private void RenderTitle(XElement element, Section pdf)
    {
        var title = pdf.AddParagraph(element.Value.Trim(), Constants.Pdf.CustomStyleNames.Title);
        title.Format.Alignment = ParagraphAlignment.Center;
        title.Format.SpaceAfter = Unit.FromCentimeter(0.5);
    }

    private void RenderText(XElement element, Section pdf)
    {
        var paragraph = pdf.AddParagraph();

        foreach (var node in element.Nodes())
        {
            var nodeAsDoc = XDocument.Parse($"<root>{node}</root>");

            var small = nodeAsDoc.Root?.Element("small");
            if (small != null)
            {
                paragraph.AddFormattedText(small.Value.Trim() + " ", Constants.Pdf.CustomStyleNames.SmallText);
            }
            else
            {
                var fixMathChars = node.ToString()
                    .Replace("&lt;", "<")
                    .Replace("&gt;", ">")
                    .Trim();

                paragraph.AddText(fixMathChars);
            }
        }

        paragraph.Format.SpaceAfter = Unit.FromCentimeter(0.35);
    }

    private void RenderFormula(XElement element, Section pdf)
    {
        if (ImageSource.ImageSourceImpl == null)
            ImageSource.ImageSourceImpl = new ImageSharpImageSource<Rgba32>();

        var latex = element.Value.Trim();
        var commentValue = element.Attribute("comment")?.Value;
        var comment = commentValue?.ToString()
                    .Replace("&lt;", "<")
                    .Replace("&gt;", ">");

        var painter = new MathPainter()
        {
            LaTeX = latex,
            FontSize = 13,
        };

        TextLaTeXParser.AdditionalBreakingEngines.Add(new EngBreakingEngine());
        var image = painter.DrawAsStream(format: SkiaSharp.SKEncodedImageFormat.Png)
                   ?? throw new InvalidOperationException("Failed to draw formula");

        if (string.IsNullOrEmpty(comment))
        {
            pdf.AddImage(ImageSource.FromStream(Guid.NewGuid().ToString(), () => image, 100));
        }
        else
        {
            var layoutTable = pdf.AddTable();
            layoutTable.Borders.Visible = false;

            var leftColumn = layoutTable.AddColumn();
            leftColumn.Format.Alignment = ParagraphAlignment.Center;
            leftColumn.Width = Unit.FromCentimeter(pdf.PageSetup.PageWidth.Centimeter / 2d);

            var rightColumn = layoutTable.AddColumn();
            rightColumn.Format.Alignment = ParagraphAlignment.Left;
            rightColumn.Width = Unit.FromCentimeter(pdf.PageSetup.PageWidth.Centimeter / 2d);

            var row = layoutTable.AddRow();
            var imageCell = row.Cells[0];

            imageCell.AddImage(ImageSource.FromStream(Guid.NewGuid().ToString(), () => image, 100));

            var commentCell = row.Cells[1];
            commentCell.AddParagraph(comment);
        }

        pdf.AddParagraph();
    }
}
