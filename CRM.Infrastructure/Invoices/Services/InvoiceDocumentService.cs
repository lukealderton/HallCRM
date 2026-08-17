using CRM.Core.Invoices.Abstractions;
using CRM.Core.Invoices.Domain;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace CRM.Infrastructure.Invoices.Services
{
    public sealed class InvoiceDocumentService
        : IInvoiceDocumentService
    {
        private readonly IInvoiceService _invoiceService;

        public InvoiceDocumentService(
            IInvoiceService objInvoiceService)
        {
            _invoiceService =
                objInvoiceService;
        }

        ///<inheritdoc/>
        public async Task<Byte[]> GenerateInvoiceAsync(
            Guid objInvoiceId,
            CancellationToken objToken = default)
        {
            if (objInvoiceId == Guid.Empty)
            {
                throw new ArgumentException(
                    "Invoice id is required.",
                    nameof(objInvoiceId));
            }

            Invoice? objInvoice =
                await _invoiceService.GetInvoiceByIdAsync(
                    objInvoiceId,
                    objToken);

            if (objInvoice == null ||
                objInvoice.Entity.DeletedUtc.HasValue)
            {
                throw new InvalidOperationException(
                    "The selected invoice could not be found.");
            }

            if (objInvoice.Status ==
                InvoiceStatus.Draft)
            {
                throw new InvalidOperationException(
                    "Draft invoices cannot be generated as customer invoices.");
            }

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
                                                objInvoice));

                                objPage.Content()
                                    .PaddingTop(14)
                                    .Element(
                                        objContent =>
                                            ComposeContent(
                                                objContent,
                                                objInvoice));

                                objPage.Footer()
                                    .Element(
                                        objFooter =>
                                            ComposeFooter(
                                                objFooter,
                                                objInvoice));
                            });
                    });

            return objDocument.GeneratePdf();
        }

        private static void ComposeHeader(
            IContainer objContainer,
            Invoice objInvoice)
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
                                        .Text("INVOICE")
                                        .FontSize(24)
                                        .Bold()
                                        .FontColor(
                                            Colors.Blue.Darken2);

                                    objColumn.Item()
                                        .PaddingTop(3)
                                        .Text(
                                            objInvoice.InvoiceNumber)
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
                                        "Issue date",
                                        FormatDate(
                                            objInvoice.IssueDateUtc));

                                    ComposeHeaderField(
                                        objColumn,
                                        "Due date",
                                        FormatDate(
                                            objInvoice.DueDateUtc));
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
            Invoice objInvoice)
        {
            objContainer
                .Column(
                    objColumn =>
                    {
                        objColumn.Spacing(14);

                        objColumn.Item()
                            .Element(
                                objInfo =>
                                    ComposeInvoiceInformation(
                                        objInfo,
                                        objInvoice));

                        objColumn.Item()
                            .Element(
                                objLines =>
                                    ComposeLines(
                                        objLines,
                                        objInvoice));

                        if (!String.IsNullOrWhiteSpace(
                            objInvoice.Notes))
                        {
                            objColumn.Item()
                                .Element(
                                    objNotes =>
                                        ComposeNotes(
                                            objNotes,
                                            objInvoice.Notes));
                        }
                    });
        }

        private static void ComposeInvoiceInformation(
            IContainer objContainer,
            Invoice objInvoice)
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
                                        "Bill to",
                                        objContent =>
                                        {
                                            objContent.Item()
                                                .Text(
                                                    DisplayValue(
                                                        objInvoice.CustomerName))
                                                .SemiBold()
                                                .FontSize(10);

                                            foreach (String strAddressLine
                                                in GetAddressLines(
                                                    objInvoice))
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
                                                    objInvoice.Job?.Name ??
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
                                                                objInvoice.JobId.ToString())
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
            Invoice objInvoice)
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

                                    foreach (InvoiceLine objLine
                                        in objInvoice.Lines
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
                                        objInvoice.Subtotal,
                                        false);

                                    ComposeTotalRow(
                                        objTotals,
                                        "Total",
                                        objInvoice.Total,
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
            String strNotes)
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
                                "Notes")
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
            Invoice objInvoice)
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
                                $"Invoice {objInvoice.InvoiceNumber}")
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
            Invoice objInvoice)
        {
            String?[] colValues =
            [
                objInvoice.AddressLine1,
                objInvoice.AddressLine2,
                objInvoice.Town,
                objInvoice.County,
                objInvoice.Postcode
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