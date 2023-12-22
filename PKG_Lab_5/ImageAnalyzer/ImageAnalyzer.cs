using PKG_Lab_5.Image;
using PKG_Lab_5.ImagesData;
using PKG_Lab_5.Storages;

namespace PKG_Lab_5.ImageAnalizer;

/// <summary>
/// класс - результат анализа
/// </summary>
public class AnalyzerResult
{
    /// <summary>
    /// найденные образы
    /// </summary>
    public List<ResultFindedItem> ResultFindedItems;
    /// <summary>
    /// Ненайденные образы
    /// </summary>
    public List<ResultNewComponentItem> ResultNewComponentItems;

    public bool is_close = false;

    public AnalyzerResult()
    {
        ResultFindedItems = new List<ResultFindedItem>();
        ResultNewComponentItems = new List<ResultNewComponentItem>();
    }
}

/// <summary>
/// Найденный образ
/// </summary>
public class ResultFindedItem : IComparable<ResultFindedItem>
{
    /// <summary>
    /// Методанные
    /// </summary>
    public MetaData MetaData;
    
    /// <summary>
    /// Положение
    /// </summary>
    public Rectangle Location;

    /// <summary>
    /// Процент совпадения
    /// </summary>
    public int percent;

    public ResultFindedItem(MetaData metaData, Rectangle location, int percent)
    {
        MetaData = metaData;
        Location = location;
        this.percent = percent;

    }

    /// <summary>
    /// Чтобы можно было сортировать по местуположению
    /// </summary>
    /// <param name="other">Другой объект</param>
    public int CompareTo(ResultFindedItem? other)
    {
        if (other == null)
            return 1;

        int result = Location.X.CompareTo(other.Location.X);

        if (result == 0)
        {
            result = Location.Y.CompareTo(other.Location.Y);
        }

        return result;
    }
}

/// <summary>
/// Новый образ
/// </summary>
public class ResultNewComponentItem
{
    /// <summary>
    /// Положение
    /// </summary>
    public Rectangle Location;

    /// <summary>
    /// Хэш изображения
    /// </summary>
    public ulong Hash;

    /// <summary>
    /// Конструктор
    /// </summary>
    /// <param name="location">положение</param>
    /// <param name="hash">хэш изображения</param>
    public ResultNewComponentItem(Rectangle location, ulong hash)
    {
        Location = location;
        Hash = hash;
    }
}

/// <summary>
/// Анализатор изображения
/// </summary>
public class ImageAnalyzer
{
    /// <summary>
    /// База образов
    /// </summary>
    private ComponentStorage _storage;

    /// <summary>
    /// минимальный процент совпадения (от 0 до 1)
    /// </summary>
    private const double MinimumMatchPercentage = 0.9;

    /// <summary>
    /// Конструктор
    /// </summary>
    /// <param name="storage">База образов</param>
    public ImageAnalyzer(ComponentStorage storage)
    {
        _storage = storage;
    }

    /// <summary>
    /// Получить список образов, которые известны и не известны базе образов
    /// </summary>
    /// <param name="toAnalysis">изображение для анализа</param>
    /// <returns>список характеристик образов</returns>
    public AnalyzerResult AnalyzeImage(BinaryImage toAnalysis)
    {
        // нахождение карты компонент связности
        ComponentMap componentMap = ComponentMap.Create(toAnalysis);
        // нахождение компонент связности по карте
        var components = ComponentDeterminator.FindComponents(componentMap);

        AnalyzerResult result = new AnalyzerResult();

        foreach (var component in components)
        {
            BinaryImage componentToFind = componentMap.ClipComponent(component.Item1, component.Item2);

            var hash = PerceptualHash.CalcPerceptualHash(componentToFind);

            var response = _storage.FindCloserComponent(hash);

            var matchPercentage = 1 - PerceptualHash.HammingDistances(hash, response.Hash) / 64.0;

            if (response != null && MinimumMatchPercentage <= matchPercentage)
            {
                result.ResultFindedItems.Add(new ResultFindedItem(response.Data, component.Item1, (int)Math.Round(matchPercentage * 100)));
            }
            else
            {
                result.ResultNewComponentItems.Add(new ResultNewComponentItem(component.Item1, hash));
            }
            if (response != null && matchPercentage > 0.7) 
            { 
                result.is_close = true;
            }
        }

        return result;
    }
}
