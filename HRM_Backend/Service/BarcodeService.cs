using SkiaSharp;
using ZXing;
using ZXing.Common;
using ZXing.SkiaSharp.Rendering;

namespace HRM_Backend.Service
{
    public interface IBarcodeService
    {
        Task<byte[]> GenerateEmployeeBarcode(int id);
    }

    public class BarcodeService : IBarcodeService
    {
        private readonly IEmployeeService _employeeService;

        public BarcodeService(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        public async Task<byte[]> GenerateEmployeeBarcode(int id)
        {
            try
            {
                var employee = await _employeeService.GetByIdAsync(id);

                if (employee == null)
                    throw new Exception("Employee not found.");

                string employeeCode = $"EMP{employee.Id:D5}";

                var writer = new BarcodeWriter<SKBitmap>
                {
                    Format = BarcodeFormat.CODE_128,

                    Options = new EncodingOptions
                    {
                        Width = 500,
                        Height = 120,
                        Margin = 20, // increased margin
                        PureBarcode = true
                    },

                    Renderer = new SKBitmapRenderer()
                };


                using SKBitmap barcodeBitmap = writer.Write(employeeCode);


                int topPadding = 20;      // Added space above barcode
                int bottomPadding = 50;   // Space for employee code text

                int canvasWidth = barcodeBitmap.Width;
                int canvasHeight = barcodeBitmap.Height + topPadding + bottomPadding;


                using SKBitmap finalBitmap = new SKBitmap(canvasWidth, canvasHeight);

                using SKCanvas canvas = new SKCanvas(finalBitmap);

                canvas.Clear(SKColors.White);


                // Draw barcode with top padding
                canvas.DrawBitmap(barcodeBitmap, 0, topPadding);


                using SKPaint textPaint = new SKPaint
                {
                    Color = SKColors.Black,
                    IsAntialias = true
                };


                using SKFont font = new SKFont
                {
                    Typeface = SKTypeface.FromFamilyName(
                        "Arial",
                        SKFontStyle.Bold
                    ),
                    Size = 16
                };


                float textWidth = font.MeasureText(employeeCode);

                float x = (canvasWidth - textWidth) / 2;

                float y = topPadding + barcodeBitmap.Height + 35;


                canvas.DrawText(employeeCode, x, y, font, textPaint);


                using SKImage image = SKImage.FromBitmap(finalBitmap);

                using SKData data = image.Encode(
                    SKEncodedImageFormat.Png,
                    100
                );

                return data.ToArray();
            }
            catch (Exception ex)
            {
                throw new Exception(
                    "An error occurred while generating the employee barcode.",
                    ex
                );
            }
        }
    }
}