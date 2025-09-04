using E_CommerceSystem.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace E_CommerceSystem.Services
{
    public class InvoiceService : IInvoiceService
    {
        //to render the invoice as PDF
        public byte[] GenerateInvoice(Order order)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(40);
                    page.Size(PageSizes.A4);

                    page.Header()
                        .Text($"Invoice - Order #{order.OID}")
                        .FontSize(20)
                        .SemiBold().FontColor(Colors.Blue.Medium);

                    page.Content()
                        .Column(col =>
                        {
                            col.Item().Text($"Order Date: {order.OrderDate}");
                            col.Item().Text($"Total Amount: {order.TotalAmount:C}").FontSize(14).Bold();

                            col.Item().Text("Items:").FontSize(16).Underline();

                            foreach (var item in order.OrderProducts)
                            {
                                col.Item().Text($"- {item.product.ProductName} x {item.Quantity} = {item.Quantity * item.product.Price:C}");
                            }
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text("Thank you for shopping with us!")
                        .FontSize(12).Italic();
                });
            });

          
            return document.GeneratePdf();
        }
    }
}
