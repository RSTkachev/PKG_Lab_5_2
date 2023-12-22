using PKG_Lab_5.Image;

namespace PKG_Lab_5.ImageFilter;

/// <summary>
/// Морфологический фильтр
/// </summary>
public class MorphologicalFilter
{
    /// <summary>
    /// Метод Нарасчивания (дилатации)
    /// </summary>
    /// <param name="image"></param>
    /// <returns></returns>
    public static BinaryImage DilatationFilter(BinaryImage image)
    {
        BinaryImage res = new BinaryImage(image.Width, image.Height);

        for (int y = 1; y < image.Height - 1; y++)
        {
            for (int x = 1; x < image.Width - 1; x++)
            {
                if (image.GetPixel(x, y) == Bit.one)
                {
                    res.SetPixel(x - 1, y - 1, Bit.one);
                    res.SetPixel(x - 1, y, Bit.one);
                    res.SetPixel(x - 1, y + 1, Bit.one);
                    res.SetPixel(x, y - 1, Bit.one);
                    res.SetPixel(x, y, Bit.one);
                    res.SetPixel(x, y + 1, Bit.one);
                    res.SetPixel(x + 1, y - 1, Bit.one);
                    res.SetPixel(x + 1, y, Bit.one);
                    res.SetPixel(x + 1, y + 1, Bit.one);
                }
            }
        }

        return res;
    }
}
