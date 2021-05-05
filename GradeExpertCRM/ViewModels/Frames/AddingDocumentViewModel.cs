using System;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Avalonia.Controls;
using GradeExpertCRM.Models;
using GradeExpertCRM.Models.Data.Repositories;
using iText.Html2pdf;
using iText.Html2pdf.Resolver.Font;
using iText.IO.Image;
using iText.IO.Source;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout.Element;
using iText.Layout.Font;
using iText.Layout.Properties;
using Microsoft.AspNetCore.Components;
using RazorLight;
using ReactiveUI;
using Splat;
using Document = iText.Layout.Document;
using Image = iText.Layout.Element.Image;
using Border = iText.Layout.Borders.Border;

namespace GradeExpertCRM.ViewModels.Frames
{
    class AddingDocumentViewModel : ViewModelBase
    {
        private DocumentsViewModel Parent;

        private async Task OpenDocumentView() => BaseWindow.Content = Parent;

        public ReactiveCommand<Unit, Unit> GoDocumentView { get; }

        public ReactiveCommand<Unit, Unit> CalculationWithPhotoCommand { get; }
        public ReactiveCommand<Unit, Unit> CalculationWithoutPhotoCommand { get; }
        public ReactiveCommand<Unit, Unit> DamagePhotosCommand { get; }
        public ReactiveCommand<Unit, Unit> OrderCommand { get; }
        public ReactiveCommand<Unit, Unit> PerformedWorkCommand { get; }
        public ReactiveCommand<Unit, Unit> PaymentCommand { get; }
        public ReactiveCommand<Unit, Unit> FranchiseCommand { get; }

        private readonly Car car_;
        private DetailsSettings detailsSettings_;
        private IDocumentRepository documentRepository_;
        private ICarRepository carRepository_;

        public AddingDocumentViewModel(IBaseWindow baseWindow, DocumentsViewModel parent)
        {
            BaseWindow = baseWindow;
            Parent = parent;
            GoDocumentView = ReactiveCommand.CreateFromTask(OpenDocumentView);
            DamagePhotosCommand = ReactiveCommand.CreateFromTask(DamagePhotos);
            OrderCommand = ReactiveCommand.CreateFromTask(Order);
            CalculationWithoutPhotoCommand = ReactiveCommand.CreateFromTask(CalculationWithoutPhoto);
            CalculationWithPhotoCommand = ReactiveCommand.CreateFromTask(CalculationWithPhoto);
            PerformedWorkCommand = ReactiveCommand.CreateFromTask(PerformedWork);
            PaymentCommand = ReactiveCommand.CreateFromTask(Payment);
            FranchiseCommand = ReactiveCommand.CreateFromTask(Franchise);

            documentRepository_ = Locator.Current.GetService<IDocumentRepository>();
            carRepository_ = Locator.Current.GetService<ICarRepository>();
            detailsSettings_ = Locator.Current.GetService<IDetailsSettingsRepository>().FirstOrDefault();
            car_ = carRepository_.GetFullCar(carRepository_.SelectedCarId);
        }

        private const string FontArial = @"..\..\..\Resources\arial.TTF";

        private async Task CalculationWithPhoto()
        {
            OpenFileDialog dialog = new OpenFileDialog {AllowMultiple = true};
            dialog.Filters.Add(new FileDialogFilter {Name = "Images", Extensions = {"png", "jpg", "jpeg"}});

            string[] images = await dialog.ShowAsync(new Window());

            if (images.Length <= 0) // Validation
                return;
            
            const string input = "GradeExpertCRM.Resources.Templates.template3";

            var engine = new RazorLightEngineBuilder()
                .UseEmbeddedResourcesProject(System.Reflection.Assembly.GetEntryAssembly())
                .UseMemoryCachingProvider()
                .Build();

            var documentVM = new DocumentVM
            {
                Car = car_,
                DetailsSettings = detailsSettings_
            };

            string html = await engine.CompileRenderAsync<object>(input, documentVM);

            const int pageWidth = 1000;
            const int pageHeight = 1415;

            // To save bytes
            ByteArrayOutputStream stream = new ByteArrayOutputStream();
            PdfWriter writer = new PdfWriter(stream);
            PdfDocument pdfDoc = new PdfDocument(writer);

            pdfDoc.SetDefaultPageSize(new PageSize(pageWidth, pageHeight));
            ConverterProperties converterProperties = new ConverterProperties();
            converterProperties.SetFontProvider(new DefaultFontProvider(true, true, true));
            Document document = HtmlConverter.ConvertToDocument(html, pdfDoc, converterProperties);

            // Font registraction
            FontProvider fontProvider = new FontProvider();
            fontProvider.AddFont(FontArial);
            document.SetFontProvider(fontProvider);
            PdfFontFactory.Register(FontArial);
            var collection = PdfFontFactory.GetRegisteredFonts();
            document.SetFontFamily(collection.ToList());

            document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));

            for (int i = 0; i < images.Length; i += 2)
            {
                document.Add(new Paragraph("ФОТО К КАЛЬКУЛЯЦИИ СТОИМОСТИ РАБОТ").SetBold());
                document.Add(new Paragraph($"№ {car_.Id} ОТ {DateTime.Now:d/M/yyyy} Г"));

                #region Table

                Table table = new Table(4, false).UseAllAvailableWidth();
                Cell cell11 = new Cell().Add(new Paragraph("ВЛАДЕЛЕЦ").SetBold());
                Cell cell21 = new Cell().Add(new Paragraph("НОМЕР ПОЛИСА ").SetBold());
                Cell cell31 = new Cell().Add(new Paragraph("НОМЕР УБЫТКА").SetBold());
                
                Cell cell12 = new Cell().Add(new Paragraph(car_.Client.Name?.ToUpper() ?? ""));
                Cell cell22 = new Cell().Add(new Paragraph(car_.PolicyNumber?.ToUpper() ?? ""));
                Cell cell32 = new Cell().Add(new Paragraph(car_.Loss?.ToUpper() ?? ""));

                Cell cell13 = new Cell().Add(new Paragraph("АВТОМОБИЛЬ").SetBold());
                Cell cell23 = new Cell().Add(new Paragraph("ГОС. НОМЕР").SetBold());
                Cell cell33 = new Cell().Add(new Paragraph("VIN").SetBold());

                Cell cell14 = new Cell().Add(new Paragraph($"{car_.Brand?.ToUpper() ?? ""} {car_.Model?.ToUpper() ?? ""}"));
                Cell cell24 = new Cell().Add(new Paragraph(car_.Number?.ToUpper() ?? ""));
                Cell cell34 = new Cell().Add(new Paragraph(car_.VIN?.ToUpper() ?? ""));

                table.AddCell(cell11).AddCell(cell12).AddCell(cell13).AddCell(cell14);
                table.AddCell(cell21).AddCell(cell22).AddCell(cell23).AddCell(cell24);
                table.AddCell(cell31).AddCell(cell32).AddCell(cell33).AddCell(cell34);
                document.Add(table);

                #endregion

                #region Photos

                document.Add(new Paragraph($"ВСЕГО {images.Length} ФОТО"));

                Table table2 = new Table(1, false).SetMarginTop(20);

                Image img = new Image(ImageDataFactory.Create(images[i])) //todo remove Directory from filename
                    .SetMaxWidth(pageWidth - document.GetLeftMargin() - document.GetRightMargin())
                    .SetMaxHeight(600).SetMargins(10, 0, 10, 0);
                table2.AddCell(new Cell().Add(new Paragraph($"ФОТО {i + 1}.")).Add(img).SetBorder(Border.NO_BORDER));

                if (i + 1 != images.Length)
                {
                    Image img2 = new Image(ImageDataFactory.Create(images[i + 1])) //todo remove Directory from filename
                        .SetMaxWidth(pageWidth - document.GetLeftMargin() - document.GetRightMargin())
                        .SetMaxHeight(600)
                        .SetMargins(10, 0, 10, 0);
                    table2.AddCell(
                        new Cell().Add(new Paragraph($"ФОТО {i + 2}.")).Add(img2).SetBorder(Border.NO_BORDER));
                }


                document.Add(table2);

                #endregion

                #region Footer

                Table footer = new Table(2, false);
                footer.AddCell(new Cell()
                    .Add(new Paragraph(
                        "ООО ГРАД-ЭКСПЕРТ 620016 Г. ЕКАТЕРИНБУРГ, УЛ. МОСТОВАЯ 14, INFO@GRAD-EXPERT.RU, ТЕЛ. (343) 268-58-18")) //TODO get values from "DetailSettings"
                    .SetBorder(iText.Layout.Borders.Border.NO_BORDER));

                int currentPage = i / 2 + 1;
                int pageCount = (int) Math.Round(images.Length / 2.0, MidpointRounding.AwayFromZero);
                footer.AddCell(new Cell().Add(new Paragraph($"СТР. {currentPage} ИЗ {pageCount}"))
                    .SetBorder(iText.Layout.Borders.Border.NO_BORDER));
                footer.SetFixedPosition(document.GetLeftMargin(), document.GetBottomMargin(),
                    pageWidth - document.GetLeftMargin() - document.GetRightMargin());
                document.Add(footer);

                #endregion

                if (currentPage != pageCount)
                    document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));
            }

            // To save bytes
            document.Close();
            byte[] pdfBytes = stream.ToArray();

            Models.Document documentModel = new Models.Document
            {
                CarId = car_.Id,
                Title = "КАЛЬКУЛЯЦИЯ СТОИМОСТИ РАБОТ (С ФОТО)",
                CreationDate = DateTime.Now,
                Content = pdfBytes
            };

            await documentRepository_.AddAsync(documentModel);
            ReturnBack(pdfBytes);
        }

        private async Task CalculationWithoutPhoto()
        {
            const string input = "GradeExpertCRM.Resources.Templates.template3";

            var engine = new RazorLightEngineBuilder()
                .UseEmbeddedResourcesProject(System.Reflection.Assembly.GetEntryAssembly())
                .UseMemoryCachingProvider()
                .Build();

            var documentVM = new DocumentVM
            {
                Car = car_,
                DetailsSettings = detailsSettings_
            };
            
            string html = await engine.CompileRenderAsync<object>(input, documentVM);

            const int pageWidth = 1000;
            const int pageHeight = 1415;

            // To save bytes
            ByteArrayOutputStream stream = new ByteArrayOutputStream();
            PdfWriter writer = new PdfWriter(stream);
            PdfDocument pdfDoc = new PdfDocument(writer);

            pdfDoc.SetDefaultPageSize(new PageSize(pageWidth, pageHeight));
            ConverterProperties converterProperties = new ConverterProperties();
            converterProperties.SetFontProvider(new DefaultFontProvider(true, true, true));
            HtmlConverter.ConvertToPdf(html, pdfDoc, converterProperties);

            pdfDoc.Close();
            byte[] pdfBytes = stream.ToArray();

            Models.Document documentModel = new Models.Document
            {
                CarId = car_.Id,
                Title = "КАЛЬКУЛЯЦИЯ СТОИМОСТИ РАБОТ (БЕЗ ФОТО)",
                CreationDate = DateTime.Now,
                Content = pdfBytes
            };

            await documentRepository_.AddAsync(documentModel);

            ReturnBack(pdfBytes);
        }

        private async Task DamagePhotos()
        {
            OpenFileDialog dialog = new OpenFileDialog {AllowMultiple = true};
            dialog.Filters.Add(new FileDialogFilter {Name = "Images", Extensions = {"png", "jpg", "jpeg"}});

            string[] images = await dialog.ShowAsync(new Window());

            if (images.Length <= 0) // Validation
                return;

            const int pageWidth = 1000;
            const int pageHeight = 1415;

            // To save bytes
            ByteArrayOutputStream stream = new ByteArrayOutputStream();
            PdfWriter writer = new PdfWriter(stream);
            PdfDocument pdf = new PdfDocument(writer);
            pdf.SetDefaultPageSize(new PageSize(pageWidth, pageHeight));
            Document document = new Document(pdf);

            // Font registraction
            FontProvider fontProvider = new FontProvider();
            fontProvider.AddFont(FontArial);
            document.SetFontProvider(fontProvider);
            PdfFontFactory.Register(FontArial);
            var collection = PdfFontFactory.GetRegisteredFonts();
            document.SetFontFamily(collection.ToList());

            for (int i = 0; i < images.Length; i += 2)
            {
                document.Add(new Paragraph("ФОТО К КАЛЬКУЛЯЦИИ СТОИМОСТИ РАБОТ").SetBold());
                document.Add(new Paragraph($"№ {car_.Id} ОТ {DateTime.Now:d/M/yyyy} Г"));

                #region Table

                Table table = new Table(4, false).UseAllAvailableWidth();
                Cell cell11 = new Cell().Add(new Paragraph("ВЛАДЕЛЕЦ").SetBold());
                Cell cell21 = new Cell().Add(new Paragraph("НОМЕР ПОЛИСА ").SetBold());
                Cell cell31 = new Cell().Add(new Paragraph("НОМЕР УБЫТКА").SetBold());
                
                Cell cell12 = new Cell().Add(new Paragraph(car_.Client.Name?.ToUpper() ?? ""));
                Cell cell22 = new Cell().Add(new Paragraph(car_.PolicyNumber?.ToUpper() ?? ""));
                Cell cell32 = new Cell().Add(new Paragraph(car_.Loss?.ToUpper() ?? ""));

                Cell cell13 = new Cell().Add(new Paragraph("АВТОМОБИЛЬ").SetBold());
                Cell cell23 = new Cell().Add(new Paragraph("ГОС. НОМЕР").SetBold());
                Cell cell33 = new Cell().Add(new Paragraph("VIN").SetBold());

                Cell cell14 = new Cell().Add(new Paragraph($"{car_.Brand?.ToUpper() ?? ""} {car_.Model?.ToUpper() ?? ""}"));
                Cell cell24 = new Cell().Add(new Paragraph(car_.Number?.ToUpper() ?? ""));
                Cell cell34 = new Cell().Add(new Paragraph(car_.VIN?.ToUpper() ?? ""));

                table.AddCell(cell11).AddCell(cell12).AddCell(cell13).AddCell(cell14);
                table.AddCell(cell21).AddCell(cell22).AddCell(cell23).AddCell(cell24);
                table.AddCell(cell31).AddCell(cell32).AddCell(cell33).AddCell(cell34);
                document.Add(table);

                #endregion

                #region Photos

                document.Add(new Paragraph($"ВСЕГО {images.Length} ФОТО"));

                Table table2 = new Table(1, false).SetMarginTop(20);

                Image img = new Image(ImageDataFactory.Create(images[i])) //todo remove Directory from filename
                    .SetMaxWidth(pageWidth - document.GetLeftMargin() - document.GetRightMargin())
                    .SetMaxHeight(600).SetMargins(10, 0, 10, 0);
                table2.AddCell(new Cell().Add(new Paragraph($"ФОТО {i + 1}.")).Add(img).SetBorder(Border.NO_BORDER));

                if (i + 1 != images.Length)
                {
                    Image img2 = new Image(ImageDataFactory.Create(images[i + 1])) //todo remove Directory from filename
                        .SetMaxWidth(pageWidth - document.GetLeftMargin() - document.GetRightMargin())
                        .SetMaxHeight(600)
                        .SetMargins(10, 0, 10, 0);
                    table2.AddCell(
                        new Cell().Add(new Paragraph($"ФОТО {i + 2}.")).Add(img2).SetBorder(Border.NO_BORDER));
                }


                document.Add(table2);

                #endregion

                #region Footer

                Table footer = new Table(2, false);
                footer.AddCell(new Cell()
                    .Add(new Paragraph(
                        "ООО ГРАД-ЭКСПЕРТ 620016 Г. ЕКАТЕРИНБУРГ, УЛ. МОСТОВАЯ 14, INFO@GRAD-EXPERT.RU, ТЕЛ. (343) 268-58-18")) //TODO get values from "DetailSettings"
                    .SetBorder(iText.Layout.Borders.Border.NO_BORDER));

                int currentPage = i / 2 + 1;
                int pageCount = (int) Math.Round(images.Length / 2.0, MidpointRounding.AwayFromZero);
                footer.AddCell(new Cell().Add(new Paragraph($"СТР. {currentPage} ИЗ {pageCount}"))
                    .SetBorder(iText.Layout.Borders.Border.NO_BORDER));
                footer.SetFixedPosition(document.GetLeftMargin(), document.GetBottomMargin(),
                    pageWidth - document.GetLeftMargin() - document.GetRightMargin());
                document.Add(footer);

                #endregion

                if (currentPage != pageCount)
                    document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));
            }

            // To save bytes
            document.Close();
            byte[] pdfBytes = stream.ToArray();

            Models.Document documentModel = new Models.Document
            {
                CarId = car_.Id,
                Title = "ФОТО ПОВРЕЖДЕНИЙ",
                CreationDate = DateTime.Now,
                Content = pdfBytes
            };

            await documentRepository_.AddAsync(documentModel);

            ReturnBack(pdfBytes);
        }

        private async Task Order()
        {
            const string input = "GradeExpertCRM.Resources.Templates.template1";

            var engine = new RazorLightEngineBuilder()
                .UseEmbeddedResourcesProject(System.Reflection.Assembly.GetEntryAssembly())
                .UseMemoryCachingProvider()
                .Build();

            var documentVM = new DocumentVM
            {
                Car = car_,
                DetailsSettings = detailsSettings_
            };

            string html = await engine.CompileRenderAsync<object>(input, documentVM);

            const int pageWidth = 1000;
            const int pageHeight = 1415;

            // To save bytes
            ByteArrayOutputStream stream = new ByteArrayOutputStream();
            PdfWriter writer = new PdfWriter(stream);
            PdfDocument pdfDoc = new PdfDocument(writer);

            pdfDoc.SetDefaultPageSize(new PageSize(pageWidth, pageHeight));
            ConverterProperties converterProperties = new ConverterProperties();
            converterProperties.SetFontProvider(new DefaultFontProvider(true, true, true));
            HtmlConverter.ConvertToPdf(html, pdfDoc, converterProperties);

            pdfDoc.Close();
            byte[] pdfBytes = stream.ToArray();

            Models.Document documentModel = new Models.Document
            {
                CarId = car_.Id,
                Title = "ЗАКАЗ-НАРЯД",
                CreationDate = DateTime.Now,
                Content = pdfBytes
            };

            await documentRepository_.AddAsync(documentModel);

            ReturnBack(pdfBytes);
        }

        private async Task PerformedWork()
        {
            const string input = "GradeExpertCRM.Resources.Templates.template2";

            var engine = new RazorLightEngineBuilder()
                .UseEmbeddedResourcesProject(System.Reflection.Assembly.GetEntryAssembly())
                .UseMemoryCachingProvider()
                .Build();

            var documentVM = new DocumentVM
            {
                Car = car_,
                DetailsSettings = detailsSettings_
            };

            string html = await engine.CompileRenderAsync<object>(input, documentVM);

            const int pageWidth = 1000;
            const int pageHeight = 1415;

            // To save bytes
            ByteArrayOutputStream stream = new ByteArrayOutputStream();
            PdfWriter writer = new PdfWriter(stream);
            PdfDocument pdfDoc = new PdfDocument(writer);

            pdfDoc.SetDefaultPageSize(new PageSize(pageWidth, pageHeight));
            ConverterProperties converterProperties = new ConverterProperties();
            converterProperties.SetFontProvider(new DefaultFontProvider(true, true, true));
            HtmlConverter.ConvertToPdf(html, pdfDoc, converterProperties);

            pdfDoc.Close();
            byte[] pdfBytes = stream.ToArray();

            Models.Document documentModel = new Models.Document
            {
                CarId = car_.Id,
                Title = "АКТ ВЫПОЛНЕННЫХ РАБОТ",
                CreationDate = DateTime.Now,
                Content = pdfBytes
            };

            await documentRepository_.AddAsync(documentModel);

            ReturnBack(pdfBytes);
        }

        private async Task Payment()
        {
             const string input = "GradeExpertCRM.Resources.Templates.template5";

            var engine = new RazorLightEngineBuilder()
                .UseEmbeddedResourcesProject(System.Reflection.Assembly.GetEntryAssembly())
                .UseMemoryCachingProvider()
                .Build();

            var documentVM = new DocumentVM
            {
                Car = car_,
                DetailsSettings = detailsSettings_
            };

            string html = await engine.CompileRenderAsync<object>(input, documentVM);

            const int pageWidth = 1000;
            const int pageHeight = 1415;

            // To save bytes
            ByteArrayOutputStream stream = new ByteArrayOutputStream();
            PdfWriter writer = new PdfWriter(stream);
            PdfDocument pdfDoc = new PdfDocument(writer);

            pdfDoc.SetDefaultPageSize(new PageSize(pageWidth, pageHeight));
            ConverterProperties converterProperties = new ConverterProperties();
            converterProperties.SetFontProvider(new DefaultFontProvider(true, true, true));
            HtmlConverter.ConvertToPdf(html, pdfDoc, converterProperties);

            pdfDoc.Close();
            byte[] pdfBytes = stream.ToArray();

            Models.Document documentModel = new Models.Document
            {
                CarId = car_.Id,
                Title = "СЧЕТ НА ОПЛАТУ",
                CreationDate = DateTime.Now,
                Content = pdfBytes
            };

            await documentRepository_.AddAsync(documentModel);

            ReturnBack(pdfBytes);
        }

        private async Task Franchise()
        {
            const string input = "GradeExpertCRM.Resources.Templates.template5";

            var engine = new RazorLightEngineBuilder()
                .UseEmbeddedResourcesProject(System.Reflection.Assembly.GetEntryAssembly())
                .UseMemoryCachingProvider()
                .Build();

            var documentVM = new DocumentVM
            {
                Car = car_,
                DetailsSettings = detailsSettings_
            };

            string html = await engine.CompileRenderAsync<object>(input, documentVM);

            const int pageWidth = 1000;
            const int pageHeight = 1415;

            // To save bytes
            ByteArrayOutputStream stream = new ByteArrayOutputStream();
            PdfWriter writer = new PdfWriter(stream);
            PdfDocument pdfDoc = new PdfDocument(writer);

            pdfDoc.SetDefaultPageSize(new PageSize(pageWidth, pageHeight));
            ConverterProperties converterProperties = new ConverterProperties();
            converterProperties.SetFontProvider(new DefaultFontProvider(true, true, true));
            HtmlConverter.ConvertToPdf(html, pdfDoc, converterProperties);

            pdfDoc.Close();
            byte[] pdfBytes = stream.ToArray();

            Models.Document documentModel = new Models.Document
            {
                CarId = car_.Id,
                Title = "СЧЕТ НА ОПЛАТУ ФРАНШИЗЫ",
                CreationDate = DateTime.Now,
                Content = pdfBytes
            };

            await documentRepository_.AddAsync(documentModel);

            ReturnBack(pdfBytes);
        }

        private void ReturnBack(byte[] pdfBytes)
        {
            BaseWindow.Content = new DocumentsViewModel(BaseWindow);
            /*
            const string Output = @"C:\Users\Admin\Desktop\demo.pdf";
            MemoryStream memory = new MemoryStream(pdfBytes);
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(memory), new PdfWriter(Output));
            pdfDoc.Close();
            */
        }
    }
}