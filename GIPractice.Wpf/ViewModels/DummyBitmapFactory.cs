using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GIPractice.Wpf.ViewModels;

public static class DummyBitmapFactory
{
    public static ImageSource MakeGrayTile(int width, int height)
    {
        var wb = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgra32, null);
        var stride = width * 4;
        var pixels = new byte[height * stride];

        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
            {
                var i = y * stride + x * 4;
                byte g = (byte)(((x / 16 + y / 16) % 2 == 0) ? 210 : 120);
                pixels[i + 0] = g; // B
                pixels[i + 1] = g; // G
                pixels[i + 2] = g; // R
                pixels[i + 3] = 255; // A
            }

        wb.WritePixels(new Int32Rect(0, 0, width, height), pixels, stride, 0);
        return wb;
    }
}
