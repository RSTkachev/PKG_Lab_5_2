using PKG_Lab_5.ImageFilter;
using PKG_Lab_5.Image;
using PKG_Lab_5.ImagesData;

namespace PKG_Lab_5.Storages;

/// <summary>
/// Хранилище образов
/// </summary>
public class ComponentStorage
{
    /// <summary>
    /// Массив компонент
    /// </summary>
    public List<ComponentData> _components = new List<ComponentData>();

    /// <summary>
    /// Добавить образ
    /// </summary>
    /// <param name="component">образ</param>
    public void AddComponent(ComponentData component)
    {
        _components.Add(component);
    }

    /// <summary>
    /// Добавить образ
    /// </summary>
    /// <param name="components">список образов</param>
    public void AddComponent(List<ComponentData> components)
    {
        components.AddRange(components);
    }

    /// <summary>
    /// Найти наиболее подходящий образ
    /// </summary>
    /// <param name="hash">хэш изображения, которому нужно подобрать образ</param>
    /// <returns></returns>
    public ComponentData? FindCloserComponent(ulong hash)
    {
        return _components.MinBy((component) => PerceptualHash.HammingDistances(component.Hash, hash));
    }

    /// <summary>
    /// Заполнить базу образов из файла
    /// </summary>
    /// <param name="path">путь к списку образов</param>
    public void FillComponentStorage(string path, ImagePreparer imagePreparer)
    {
        string[] ComponentNameAndImage = File.ReadAllLines(path);

        for (int i = 0; i < ComponentNameAndImage.Length; i++)
        {
            try
            {
                string[] nameAndImagePath = ComponentNameAndImage[i].Trim().Split();
                BinaryImage toAdd = imagePreparer.PrepareForAnalysis(new Bitmap(nameAndImagePath[2]), nameAndImagePath[1] == "1", 0);

                AddComponent(new MetaData(nameAndImagePath[0]), toAdd);
            }
            catch (Exception) { }
        }
    }

    /// <summary>
    /// Добавление образа - считается, что он один
    /// </summary>
    /// <param name="metaData">Методанные</param>
    /// <param name="toAdd">Изображение с образом</param>
    private void AddComponent(MetaData metaData, BinaryImage toAdd)
    {
        // нахождение карты компонент связности
        ComponentMap componentMap = ComponentMap.Create(toAdd);
        // нахождение компонент связности по карте
        var components = ComponentDeterminator.FindComponents(componentMap);

        int size = 0;
        int number = 0;

        for (int i = 0; i < components.Count; i++)
        {
            if (components[i].Item1.Width * components[i].Item1.Height > size)
            {
                size = components[i].Item1.Width * components[i].Item1.Height;
                number = i;
            }
        }

        AddComponent(new ComponentData(metaData, componentMap.ClipComponent(components[number].Item1, components[number].Item2)));
    }
}