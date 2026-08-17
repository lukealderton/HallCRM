using CRM.Core.Jobs.Abstractions;
using CRM.Core.Jobs.Domain;
using CRM.Core.Users.Abstraction.Services;
using CRM.Core.Users.Domain;
using CRM.Primitives.Extensions;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace CRM.Infrastructure.Jobs.Services
{
    public sealed class JobSheetService
        : IJobSheetService
    {
        private readonly IJobService _objJobService;
        private readonly IUserService _objUserService;

        public JobSheetService(
            IJobService objJobService,
            IUserService objUserService)
        {
            _objJobService =
                objJobService;

            _objUserService =
                objUserService;
        }

        public async Task<Byte[]> GenerateJobSheetAsync(
            Guid objJobId,
            CancellationToken objToken = default)
        {
            Job? objJob =
                await _objJobService.GetJobByIdAsync(
                    objJobId,
                    objToken);

            if (objJob == null)
            {
                throw new InvalidOperationException(
                    "The requested job could not be found.");
            }

            User? objAssignedUser =
                null;

            if (objJob.AssignedUserId.HasValue)
            {
                objAssignedUser =
                    await _objUserService.GetUserAsync(
                        objJob.AssignedUserId.Value,
                        objToken);
            }

            return Document
                .Create(objDocument =>
                {
                    objDocument.Page(objPage =>
                    {
                        objPage.Size(
                            PageSizes.A4);

                        objPage.Margin(
                            32);

                        objPage.DefaultTextStyle(
                            objStyle =>
                                objStyle
                                    .FontSize(10)
                                    .FontColor(
                                        Colors.Grey.Darken3));

                        objPage.Header()
                            .Element(
                                objContainer =>
                                    ComposeHeader(
                                        objContainer,
                                        objJob));

                        objPage.Content()
                            .PaddingVertical(18)
                            .Element(
                                objContainer =>
                                    ComposeContent(
                                        objContainer,
                                        objJob,
                                        objAssignedUser));

                        objPage.Footer()
                            .Element(
                                ComposeFooter);
                    });
                })
                .GeneratePdf();
        }

        private static void ComposeHeader(
            IContainer objContainer,
            Job objJob)
        {
            objContainer
                .Row(objRow =>
                {
                    objRow.RelativeItem()
                        .Column(objColumn =>
                        {
                            objColumn.Item()
                                .Text("JOB SHEET")
                                .FontSize(11)
                                .SemiBold()
                                .FontColor(
                                    Colors.Blue.Darken2);

                            objColumn.Item()
                                .PaddingTop(3)
                                .Text(objJob.Name)
                                .FontSize(22)
                                .Bold()
                                .FontColor(
                                    Colors.Grey.Darken4);

                            objColumn.Item()
                                .PaddingTop(4)
                                .Text(
                                    objJob.Company?.Name ??
                                    "No company linked")
                                .FontSize(10)
                                .FontColor(
                                    Colors.Grey.Darken1);
                        });

                    objRow.ConstantItem(130)
                        .AlignRight()
                        .Column(objColumn =>
                        {
                            objColumn.Item()
                                .AlignRight()
                                .Text(
                                    objJob.Stage
                                        .GetDisplay()
                                        .Name)
                                .SemiBold();

                            objColumn.Item()
                                .PaddingTop(4)
                                .AlignRight()
                                .Text(
                                    $"Job ID: {objJob.Id}")
                                .FontSize(8)
                                .FontColor(
                                    Colors.Grey.Medium);
                        });
                });
        }

        private static void ComposeContent(
            IContainer objContainer,
            Job objJob,
            User? objAssignedUser)
        {
            objContainer.Column(objColumn =>
            {
                objColumn.Spacing(14);

                objColumn.Item()
                    .Element(
                        objSection =>
                            ComposeSummary(
                                objSection,
                                objJob,
                                objAssignedUser));

                objColumn.Item()
                    .Element(
                        objSection =>
                            ComposeSite(
                                objSection,
                                objJob));

                objColumn.Item()
                    .Element(
                        objSection =>
                            ComposeServices(
                                objSection,
                                objJob));

                if (!String.IsNullOrWhiteSpace(
                    objJob.Description))
                {
                    objColumn.Item()
                        .Element(
                            objSection =>
                                ComposeTextSection(
                                    objSection,
                                    "Work description",
                                    objJob.Description));
                }

                if (!String.IsNullOrWhiteSpace(
                    objJob.Notes))
                {
                    objColumn.Item()
                        .Element(
                            objSection =>
                                ComposeTextSection(
                                    objSection,
                                    "Internal notes",
                                    objJob.Notes));
                }

                objColumn.Item()
                    .Element(
                        ComposeCompletionSection);
            });
        }

        private static void ComposeSummary(
            IContainer objContainer,
            Job objJob,
            User? objAssignedUser)
        {
            objContainer
                .Border(1)
                .BorderColor(
                    Colors.Grey.Lighten2)
                .CornerRadius(8)
                .Padding(12)
                .Column(objColumn =>
                {
                    objColumn.Item()
                        .Element(
                            objHeader =>
                                ComposeSectionHeading(
                                    objHeader,
                                    "Job details"));

                    objColumn.Item()
                        .PaddingTop(10)
                        .Table(objTable =>
                        {
                            objTable.ColumnsDefinition(
                                objColumns =>
                                {
                                    objColumns.RelativeColumn();
                                    objColumns.RelativeColumn();
                                });

                            AddValueCell(
                                objTable,
                                "Assigned to",
                                objAssignedUser?.DisplayName ??
                                "Unassigned");

                            AddValueCell(
                                objTable,
                                "Stage",
                                objJob.Stage
                                    .GetDisplay()
                                    .Name);

                            AddValueCell(
                                objTable,
                                "Company",
                                objJob.Company?.Name ??
                                "-");

                            AddValueCell(
                                objTable,
                                "Contact",
                                objJob.Contact?.Entity.DisplayName ??
                                "-");

                            AddValueCell(
                                objTable,
                                "Value",
                                objJob.Value.HasValue
                                    ? objJob.Value.Value.ToString("C")
                                    : "-");

                            AddValueCell(
                                objTable,
                                "Expected close",
                                objJob.ExpectedCloseDateUtc.HasValue
                                    ? objJob.ExpectedCloseDateUtc.Value
                                        .ToLocalTime()
                                        .ToString("dd MMM yyyy")
                                    : "-");
                        });
                });
        }

        private static void ComposeSite(
            IContainer objContainer,
            Job objJob)
        {
            objContainer
                .Border(1)
                .BorderColor(
                    Colors.Grey.Lighten2)
                .CornerRadius(8)
                .Padding(12)
                .Column(objColumn =>
                {
                    objColumn.Item()
                        .Element(
                            objHeader =>
                                ComposeSectionHeading(
                                    objHeader,
                                    "Site"));

                    objColumn.Item()
                        .PaddingTop(10)
                        .Text(
                            FormatAddress(
                                objJob))
                        .SemiBold()
                        .FontSize(11);

                    if (!String.IsNullOrWhiteSpace(
                        objJob.SiteContactName) ||
                        !String.IsNullOrWhiteSpace(
                            objJob.SiteContactPhone))
                    {
                        objColumn.Item()
                            .PaddingTop(8)
                            .Text(
                                objText =>
                                {
                                    objText
                                        .Span("Site contact: ")
                                        .SemiBold();

                                    objText.Span(
                                        BuildSiteContact(
                                            objJob));
                                });
                    }

                    if (!String.IsNullOrWhiteSpace(
                        objJob.AccessNotes))
                    {
                        objColumn.Item()
                            .PaddingTop(10)
                            .Background(
                                Colors.Amber.Lighten5)
                            .Border(1)
                            .BorderColor(
                                Colors.Amber.Lighten3)
                            .CornerRadius(5)
                            .Padding(9)
                            .Column(objAccessColumn =>
                            {
                                objAccessColumn.Item()
                                    .Text("ACCESS NOTES")
                                    .FontSize(8)
                                    .Bold()
                                    .FontColor(
                                        Colors.Amber.Darken3);

                                objAccessColumn.Item()
                                    .PaddingTop(3)
                                    .Text(
                                        objJob.AccessNotes);
                            });
                    }
                });
        }

        private static void ComposeServices(
    IContainer objContainer,
    Job objJob)
        {
            objContainer
                .Border(1)
                .BorderColor(
                    Colors.Grey.Lighten2)
                .CornerRadius(8)
                .Padding(12)
                .Column(objColumn =>
                {
                    objColumn.Item()
                        .Element(
                            objHeader =>
                                ComposeSectionHeading(
                                    objHeader,
                                    "Services / work"));

                    if (objJob.ServiceLinks.Count == 0)
                    {
                        objColumn.Item()
                            .PaddingTop(10)
                            .Text(
                                "No services have been linked to this job.")
                            .FontColor(
                                Colors.Grey.Medium);

                        return;
                    }

                    objColumn.Item()
                        .PaddingTop(10)
                        .Table(objTable =>
                        {
                            objTable.ColumnsDefinition(
                                objColumns =>
                                {
                                    objColumns.RelativeColumn(4);
                                    objColumns.RelativeColumn(1);
                                    objColumns.RelativeColumn(1.5f);
                                    objColumns.RelativeColumn(1.5f);
                                });

                            objTable.Header(objHeader =>
                            {
                                objHeader.Cell()
                                    .Element(ServiceTableHeaderCell)
                                    .Text("Service");

                                objHeader.Cell()
                                    .Element(ServiceTableHeaderCell)
                                    .AlignRight()
                                    .Text("Qty");

                                objHeader.Cell()
                                    .Element(ServiceTableHeaderCell)
                                    .AlignRight()
                                    .Text("Unit");

                                objHeader.Cell()
                                    .Element(ServiceTableHeaderCell)
                                    .AlignRight()
                                    .Text("Total");
                            });

                            foreach (JobServiceLink objLink
                                in objJob.ServiceLinks
                                    .OrderBy(objLink =>
                                        objLink.Service.Name))
                            {
                                objTable.Cell()
                                    .Element(ServiceTableCell)
                                    .Column(objServiceColumn =>
                                    {
                                        objServiceColumn.Item()
                                            .Text(
                                                objLink.Service.Name)
                                            .SemiBold();

                                        if (!String.IsNullOrWhiteSpace(
                                            objLink.Service.Description))
                                        {
                                            objServiceColumn.Item()
                                                .PaddingTop(2)
                                                .Text(
                                                    objLink.Service.Description)
                                                .FontSize(8)
                                                .FontColor(
                                                    Colors.Grey.Darken1);
                                        }
                                    });

                                objTable.Cell()
                                    .Element(ServiceTableCell)
                                    .AlignRight()
                                    .Text(
                                        FormatQuantity(
                                            objLink.Quantity));

                                objTable.Cell()
                                    .Element(ServiceTableCell)
                                    .AlignRight()
                                    .Text(
                                        objLink.UnitPrice.HasValue
                                            ? objLink.UnitPrice.Value.ToString("C")
                                            : "-");

                                objTable.Cell()
                                    .Element(ServiceTableCell)
                                    .AlignRight()
                                    .Text(
                                        objLink.UnitPrice.HasValue
                                            ? CalculateLineTotal(objLink).ToString("C")
                                            : "-")
                                    .SemiBold();
                            }
                        });

                    if (objJob.ServiceLinks.Any(
                        objLink =>
                            objLink.UnitPrice.HasValue))
                    {
                        Decimal dcmServicesTotal =
                            objJob.ServiceLinks
                                .Where(objLink =>
                                    objLink.UnitPrice.HasValue)
                                .Sum(
                                    CalculateLineTotal);

                        objColumn.Item()
                            .PaddingTop(10)
                            .AlignRight()
                            .Row(objRow =>
                            {
                                objRow.AutoItem()
                                    .Text("Services total")
                                    .FontSize(9)
                                    .SemiBold()
                                    .FontColor(
                                        Colors.Grey.Darken1);

                                objRow.ConstantItem(85)
                                    .AlignRight()
                                    .Text(
                                        dcmServicesTotal.ToString("C"))
                                    .FontSize(11)
                                    .Bold()
                                    .FontColor(
                                        Colors.Grey.Darken4);
                            });
                    }
                });
        }

        private static void ComposeTextSection(
            IContainer objContainer,
            String strHeading,
            String strText)
        {
            objContainer
                .Border(1)
                .BorderColor(
                    Colors.Grey.Lighten2)
                .CornerRadius(8)
                .Padding(12)
                .Column(objColumn =>
                {
                    objColumn.Item()
                        .Element(
                            objHeader =>
                                ComposeSectionHeading(
                                    objHeader,
                                    strHeading));

                    objColumn.Item()
                        .PaddingTop(8)
                        .Text(strText)
                        .LineHeight(1.4f);
                });
        }

        private static void ComposeCompletionSection(
            IContainer objContainer)
        {
            objContainer
                .Border(1)
                .BorderColor(
                    Colors.Grey.Lighten2)
                .CornerRadius(8)
                .Padding(12)
                .Column(objColumn =>
                {
                    objColumn.Item()
                        .Element(
                            objHeader =>
                                ComposeSectionHeading(
                                    objHeader,
                                    "Completion"));

                    objColumn.Item()
                        .PaddingTop(14)
                        .Row(objRow =>
                        {
                            objRow.RelativeItem()
                                .Column(objSignature =>
                                {
                                    objSignature.Item()
                                        .Text("Completed by")
                                        .FontSize(8)
                                        .FontColor(
                                            Colors.Grey.Medium);

                                    objSignature.Item()
                                        .PaddingTop(22)
                                        .BorderBottom(1)
                                        .BorderColor(
                                            Colors.Grey.Medium);
                                });

                            objRow.ConstantItem(20);

                            objRow.RelativeItem()
                                .Column(objDate =>
                                {
                                    objDate.Item()
                                        .Text("Date")
                                        .FontSize(8)
                                        .FontColor(
                                            Colors.Grey.Medium);

                                    objDate.Item()
                                        .PaddingTop(22)
                                        .BorderBottom(1)
                                        .BorderColor(
                                            Colors.Grey.Medium);
                                });
                        });

                    objColumn.Item()
                        .PaddingTop(18)
                        .Column(objNotes =>
                        {
                            objNotes.Item()
                                .Text("Completion notes")
                                .FontSize(8)
                                .FontColor(
                                    Colors.Grey.Medium);

                            objNotes.Item()
                                .PaddingTop(28)
                                .BorderBottom(1)
                                .BorderColor(
                                    Colors.Grey.Lighten1);

                            objNotes.Item()
                                .PaddingTop(28)
                                .BorderBottom(1)
                                .BorderColor(
                                    Colors.Grey.Lighten1);
                        });
                });
        }

        private static void ComposeSectionHeading(
            IContainer objContainer,
            String strText)
        {
            objContainer
                .Text(strText)
                .FontSize(11)
                .Bold()
                .FontColor(
                    Colors.Grey.Darken4);
        }

        private static void AddValueCell(
            TableDescriptor objTable,
            String strLabel,
            String strValue)
        {
            objTable.Cell()
                .PaddingVertical(5)
                .PaddingRight(8)
                .Column(objColumn =>
                {
                    objColumn.Item()
                        .Text(strLabel)
                        .FontSize(8)
                        .FontColor(
                            Colors.Grey.Medium);

                    objColumn.Item()
                        .PaddingTop(1)
                        .Text(strValue)
                        .SemiBold();
                });
        }

        private static void ComposeFooter(
            IContainer objContainer)
        {
            objContainer
                .AlignCenter()
                .Text(objText =>
                {
                    objText
                        .DefaultTextStyle(
                            objStyle =>
                                objStyle
                                    .FontSize(8)
                                    .FontColor(
                                        Colors.Grey.Medium));

                    objText.Span(
                        "Generated ");

                    objText.Span(
                        DateTime.Now
                            .ToString(
                                "dd MMM yyyy HH:mm"));

                    objText.Span(
                        "  •  Page ");

                    objText.CurrentPageNumber();

                    objText.Span(
                        " of ");

                    objText.TotalPages();
                });
        }

        private static String FormatAddress(
            Job objJob)
        {
            List<String> colParts =
                new[]
                {
                    objJob.AddressLine1,
                    objJob.AddressLine2,
                    objJob.Town,
                    objJob.County,
                    objJob.Postcode
                }
                .Where(strValue =>
                    !String.IsNullOrWhiteSpace(
                        strValue))
                .Select(strValue =>
                    strValue!.Trim())
                .ToList();

            return colParts.Count == 0
                ? "No site address recorded."
                : String.Join(
                    Environment.NewLine,
                    colParts);
        }

        private static String BuildSiteContact(
            Job objJob)
        {
            List<String> colParts =
                [];

            if (!String.IsNullOrWhiteSpace(
                objJob.SiteContactName))
            {
                colParts.Add(
                    objJob.SiteContactName.Trim());
            }

            if (!String.IsNullOrWhiteSpace(
                objJob.SiteContactPhone))
            {
                colParts.Add(
                    objJob.SiteContactPhone.Trim());
            }

            return colParts.Count == 0
                ? "-"
                : String.Join(
                    " • ",
                    colParts);
        }

        private static IContainer ServiceTableHeaderCell(
    IContainer objContainer)
        {
            return objContainer
                .PaddingVertical(5)
                .PaddingHorizontal(4)
                .Background(
                    Colors.Grey.Lighten4)
                .DefaultTextStyle(
                    objStyle =>
                        objStyle
                            .FontSize(8)
                            .SemiBold()
                            .FontColor(
                                Colors.Grey.Darken2));
        }

        private static IContainer ServiceTableCell(
            IContainer objContainer)
        {
            return objContainer
                .BorderBottom(1)
                .BorderColor(
                    Colors.Grey.Lighten3)
                .PaddingVertical(7)
                .PaddingHorizontal(4);
        }

        private static Decimal CalculateLineTotal(
            JobServiceLink objLink)
        {
            Decimal dcmQuantity =
                objLink.Quantity <= 0m
                    ? 1m
                    : objLink.Quantity;

            return dcmQuantity *
                (objLink.UnitPrice ?? 0m);
        }

        private static String FormatQuantity(
            Decimal dcmQuantity)
        {
            return dcmQuantity.ToString(
                dcmQuantity % 1m == 0m
                    ? "0"
                    : "0.##");
        }
    }
}