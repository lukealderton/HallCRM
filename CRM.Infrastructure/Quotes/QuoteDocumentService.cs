using CRM.Core.Quotes;

using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace CRM.Infrastructure.Quotes
{
    public sealed class QuoteDocumentService
        : IQuoteDocumentService
    {
        private readonly IQuoteService _quoteService;

        public QuoteDocumentService(
            IQuoteService objQuoteService)
        {
            _quoteService =
                objQuoteService;
        }

        ///<inheritdoc/>
        public async Task<Byte[]> GenerateQuoteAsync(
            Guid objQuoteId,
            CancellationToken objToken = default)
        {
            if (objQuoteId == Guid.Empty)
            {
                throw new ArgumentException(
                    "Quote id is required.",
                    nameof(objQuoteId));
            }

            Quote? objQuote =
                await _quoteService.GetByIdAsync(
                    objQuoteId,
                    objToken);

            if (objQuote == null)
                throw new InvalidOperationException("The selected quote could not be found.");
            IDocument objDocument =
                Document.Create(
                    objContainer =>
                    {
                        objContainer.Page(
                            objPage =>
                            {
                                objPage.Size(
                                    PageSizes.A4);

                                objPage.Margin(
                                    18,
                                    Unit.Millimetre);

                                objPage.PageColor(
                                    Colors.White);

                                objPage.DefaultTextStyle(
                                    objStyle =>
                                        objStyle
                                            .FontSize(9)
                                            .FontColor(
                                                Colors.Grey.Darken3));

                                objPage.Header()
                                    .Element(
                                        objHeader =>
                                            ComposeHeader(
                                                objHeader,
                                                objQuote));

                                objPage.Content()
                                    .PaddingTop(14)
                                    .Element(
                                        objContent =>
                                            ComposeContent(
                                                objContent,
                                                objQuote));

                                objPage.Footer()
                                    .Element(
                                        objFooter =>
                                            ComposeFooter(
                                                objFooter,
                                                objQuote));
                            });
                    });

            return objDocument.GeneratePdf();
        }

        private static void ComposeHeader(
            IContainer objContainer,
            Quote objQuote)
        {
            objContainer
                .Row(
                    objRow =>
                    {
                        objRow.RelativeItem()
                            .Column(
                                objColumn =>
                                {
                                    objColumn.Item()
                                        .PaddingBottom(8)
                                        .Text("Hall Home Maintenance")
                                        .FontSize(16)
                                        .SemiBold()
                                        .FontColor(
                                            Colors.Grey.Darken3);

                                    objColumn.Item()
                                        .Text("QUOTE")
                                        .FontSize(24)
                                        .Bold()
                                        .FontColor(
                                            Colors.Blue.Darken2);

                                    objColumn.Item()
                                        .PaddingTop(3)
                                        .Text(
                                            objQuote.QuoteNumber)
                                        .FontSize(11)
                                        .SemiBold()
                                        .FontColor(
                                            Colors.Grey.Darken2);
                                });

                        objRow.ConstantItem(190)
                            .AlignRight()
                            .Column(
                                objColumn =>
                                {
                                    ComposeHeaderField(
                                        objColumn,
                                        "Created",
                                        FormatDate(
                                            objQuote.CreatedUtc));

                                });
                    });
        }

        private static void ComposeHeaderField(
            ColumnDescriptor objColumn,
            String strLabel,
            String strValue)
        {
            objColumn.Item()
                .PaddingBottom(5)
                .AlignRight()
                .Text(
                    objText =>
                    {
                        objText.Span(
                                strLabel + ": ")
                            .FontColor(
                                Colors.Grey.Medium);

                        objText.Span(
                                strValue)
                            .SemiBold()
                            .FontColor(
                                Colors.Grey.Darken3);
                    });
        }

        private static void ComposeContent(
            IContainer objContainer,
            Quote objQuote)
        {
            objContainer
                .Column(
                    objColumn =>
                    {
                        objColumn.Spacing(14);

                        objColumn.Item()
                            .Element(
                                objInfo =>
                                    ComposeQuoteInformation(
                                        objInfo,
                                        objQuote));

                        if (!String.IsNullOrWhiteSpace(objQuote.JobDescription))
                        {
                            objColumn.Item().Element(container =>
                                ComposeNotes(container, objQuote.JobDescription, "Job description"));
                        }
                        objColumn.Item()
                            .Element(
                                objLines =>
                                    ComposeLines(
                                        objLines,
                                        objQuote));

                        objColumn.Item()
                            .ShowEntire()
                            .Column(
                                objContact =>
                                {
                                    objContact.Spacing(4);

                                    objContact.Item()
                                        .Text("Contact")
                                        .FontSize(11)
                                        .SemiBold();

                                    objContact.Item()
                                        .Text("Phone: 07532774195 / 07878010886");

                                    objContact.Item()
                                        .Text("Email: hallhomemaintenance@gmail.com");
                                });

                        if (!String.IsNullOrWhiteSpace(
                            objQuote.Notes))
                        {
                            objColumn.Item()
                                .Element(
                                    objNotes =>
                                        ComposeNotes(
                                            objNotes,
                                            objQuote.Notes));
                        }
                    });
        }

        private static void ComposeQuoteInformation(
            IContainer objContainer,
            Quote objQuote)
        {
            objContainer
                .Row(
                    objRow =>
                    {
                        objRow.RelativeItem()
                            .Element(
                                objCustomer =>
                                    ComposeInfoCard(
                                        objCustomer,
                                        "Prepared for",
                                        objContent =>
                                        {
                                            objContent.Item()
                                                .Text(
                                                    DisplayValue(
                                                        objQuote.CustomerName))
                                                .SemiBold()
                                                .FontSize(10);

                                            foreach (String strAddressLine
                                                in GetAddressLines(
                                                    objQuote))
                                            {
                                                objContent.Item()
                                                    .PaddingTop(2)
                                                    .Text(
                                                        strAddressLine)
                                                    .FontColor(
                                                        Colors.Grey.Darken1);
                                            }
                                        }));

                        objRow.ConstantItem(12);

                        objRow.RelativeItem()
                            .Element(
                                objJob =>
                                    ComposeInfoCard(
                                        objJob,
                                        "Job",
                                        objContent =>
                                        {
                                            objContent.Item()
                                                .Text(
                                                    objQuote.JobName ??
                                                    "-")
                                                .SemiBold()
                                                .FontSize(10);

                                            objContent.Item()
                                                .PaddingTop(4)
                                                .Text(
                                                    objText =>
                                                    {
                                                        objText.Span(
                                                                "Reference: ")
                                                            .FontColor(
                                                                Colors.Grey.Medium);

                                                        objText.Span(
                                                                objQuote.JobId.ToString())
                                                            .FontColor(
                                                                Colors.Grey.Darken1);
                                                    });
                                        }));
                    });
        }

        private static void ComposeInfoCard(
            IContainer objContainer,
            String strTitle,
            Action<ColumnDescriptor> objContent)
        {
            objContainer
                .Border(1)
                .BorderColor(
                    Colors.Grey.Lighten2)
                .Padding(10)
                .Column(
                    objColumn =>
                    {
                        objColumn.Item()
                            .PaddingBottom(6)
                            .Text(
                                strTitle)
                            .FontSize(8)
                            .SemiBold()
                            .FontColor(
                                Colors.Grey.Medium);

                        objContent(
                            objColumn);
                    });
        }

        private static void ComposeLines(
            IContainer objContainer,
            Quote objQuote)
        {
            objContainer
                .Column(
                    objColumn =>
                    {
                        objColumn.Item()
                            .Table(
                                objTable =>
                                {
                                    objTable.ColumnsDefinition(
                                        objColumns =>
                                        {
                                            objColumns.RelativeColumn(
                                                5);

                                            objColumns.RelativeColumn(
                                                1);

                                            objColumns.RelativeColumn(
                                                1.5f);

                                            objColumns.RelativeColumn(
                                                1.5f);
                                        });

                                    objTable.Header(
                                        objHeader =>
                                        {
                                            objHeader.Cell()
                                                .Element(
                                                    TableHeaderCell)
                                                .Text(
                                                    "Description");

                                            objHeader.Cell()
                                                .Element(
                                                    TableHeaderCell)
                                                .AlignRight()
                                                .Text(
                                                    "Qty");

                                            objHeader.Cell()
                                                .Element(
                                                    TableHeaderCell)
                                                .AlignRight()
                                                .Text(
                                                    "Unit");

                                            objHeader.Cell()
                                                .Element(
                                                    TableHeaderCell)
                                                .AlignRight()
                                                .Text(
                                                    "Total");
                                        });

                                    foreach (QuoteLine objLine
                                        in objQuote.Lines
                                            .OrderBy(
                                                objLine =>
                                                    objLine.SortOrder))
                                    {
                                        objTable.Cell()
                                            .Element(
                                                TableCell)
                                            .Text(
                                                objLine.Description);

                                        objTable.Cell()
                                            .Element(
                                                TableCell)
                                            .AlignRight()
                                            .Text(
                                                FormatQuantity(
                                                    objLine.Quantity));

                                        objTable.Cell()
                                            .Element(
                                                TableCell)
                                            .AlignRight()
                                            .Text(
                                                objLine.UnitPrice.ToString(
                                                    "C"));

                                        objTable.Cell()
                                            .Element(
                                                TableCell)
                                            .AlignRight()
                                            .Text(
                                                objLine.LineTotal.ToString(
                                                    "C"))
                                            .SemiBold();
                                    }
                                });

                        objColumn.Item()
                            .PaddingTop(12)
                            .AlignRight()
                            .Width(220)
                            .Column(
                                objTotals =>
                                {
                                    ComposeTotalRow(
                                        objTotals,
                                        "Subtotal",
                                        objQuote.Subtotal,
                                        false);

                                    ComposeTotalRow(
                                        objTotals,
                                        "Total",
                                        objQuote.Total,
                                        true);
                                });
                    });
        }

        private static IContainer TableHeaderCell(
            IContainer objContainer)
        {
            return objContainer
                .Background(
                    Colors.Grey.Lighten4)
                .BorderBottom(1)
                .BorderColor(
                    Colors.Grey.Lighten2)
                .PaddingVertical(7)
                .PaddingHorizontal(6)
                .DefaultTextStyle(
                    objStyle =>
                        objStyle
                            .FontSize(8)
                            .SemiBold()
                            .FontColor(
                                Colors.Grey.Darken2));
        }

        private static IContainer TableCell(
            IContainer objContainer)
        {
            return objContainer
                .BorderBottom(1)
                .BorderColor(
                    Colors.Grey.Lighten3)
                .PaddingVertical(8)
                .PaddingHorizontal(6);
        }

        private static void ComposeTotalRow(
            ColumnDescriptor objColumn,
            String strLabel,
            Decimal dcmAmount,
            Boolean blnGrandTotal)
        {
            objColumn.Item()
                .PaddingVertical(
                    blnGrandTotal
                        ? 7
                        : 4)
                .BorderTop(
                    blnGrandTotal
                        ? 1
                        : 0)
                .BorderColor(
                    Colors.Grey.Lighten2)
                .Row(
                    objRow =>
                    {
                        objRow.RelativeItem()
                            .Text(
                                strLabel)
                            .FontSize(
                                blnGrandTotal
                                    ? 10
                                    : 9)
                            .FontColor(
                                blnGrandTotal
                                    ? Colors.Grey.Darken3
                                    : Colors.Grey.Darken1)
                            .SemiBold();

                        objRow.ConstantItem(90)
                            .AlignRight()
                            .Text(
                                dcmAmount.ToString(
                                    "C"))
                            .FontSize(
                                blnGrandTotal
                                    ? 12
                                    : 9)
                            .Bold()
                            .FontColor(
                                blnGrandTotal
                                    ? Colors.Blue.Darken2
                                    : Colors.Grey.Darken3);
                    });
        }

        private static void ComposeNotes(
            IContainer objContainer,
            String strNotes, String strTitle = "Notes")
        {
            objContainer
                .Border(1)
                .BorderColor(
                    Colors.Grey.Lighten2)
                .Padding(10)
                .Column(
                    objColumn =>
                    {
                        objColumn.Item()
                            .Text(
                                strTitle)
                            .FontSize(8)
                            .SemiBold()
                            .FontColor(
                                Colors.Grey.Medium);

                        objColumn.Item()
                            .PaddingTop(5)
                            .Text(
                                strNotes)
                            .FontSize(9)
                            .LineHeight(1.35f);
                    });
        }

        private static void ComposeFooter(
            IContainer objContainer,
            Quote objQuote)
        {
            objContainer
                .PaddingTop(10)
                .BorderTop(1)
                .BorderColor(
                    Colors.Grey.Lighten2)
                .Row(
                    objRow =>
                    {
                        objRow.RelativeItem()
                            .Text(
                                $"Quote {objQuote.QuoteNumber}")
                            .FontSize(7)
                            .FontColor(
                                Colors.Grey.Medium);

                        objRow.RelativeItem()
                            .AlignRight()
                            .Text(
                                objText =>
                                {
                                    objText
                                        .DefaultTextStyle(
                                            objStyle =>
                                                objStyle
                                                    .FontSize(7)
                                                    .FontColor(
                                                        Colors.Grey.Medium));

                                    objText.Span(
                                        "Page ");

                                    objText.CurrentPageNumber();

                                    objText.Span(
                                        " of ");

                                    objText.TotalPages();
                                });
                    });
        }

        private static IEnumerable<String> GetAddressLines(
            Quote objQuote)
        {
            String?[] colValues =
            [
                objQuote.AddressLine1,
                objQuote.AddressLine2,
                objQuote.Town,
                objQuote.County,
                objQuote.Postcode
            ];

            return colValues
                .Where(
                    strValue =>
                        !String.IsNullOrWhiteSpace(
                            strValue))
                .Select(
                    strValue =>
                        strValue!.Trim());
        }

        private static String FormatDate(
            DateTime? dteValue)
        {
            return dteValue.HasValue
                ? dteValue.Value
                    .ToLocalTime()
                    .ToString(
                        "dd MMM yyyy")
                : "-";
        }

        private static String FormatQuantity(
            Decimal dcmQuantity)
        {
            return dcmQuantity.ToString(
                dcmQuantity % 1m == 0m
                    ? "0"
                    : "0.##");
        }

        private static String DisplayValue(
            String? strValue)
        {
            return String.IsNullOrWhiteSpace(
                strValue)
                ? "-"
                : strValue.Trim();
        }
    }
}

