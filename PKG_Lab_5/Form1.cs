using PKG_Lab_5.ImageAnalizer;
using PKG_Lab_5.ImageFilter;
using PKG_Lab_5.Storages;

namespace PKG_Lab_5;

/// <summary>
/// Основная форма
/// </summary>
public partial class Form1 : Form
{
    /// <summary>
    /// Анализатор образов
    /// </summary>
    private ImageAnalyzer _analyzer;
    /// <summary>
    /// Хранилище образов
    /// </summary>
    public ComponentStorage _storage;
    /// <summary>
    /// Предобработчик изображения
    /// </summary>
    private ImagePreparer _imagePreparer;

    /// <summary>
    /// Изображение, которое анализируется
    /// </summary>
    private Bitmap _bitmap;

    public Form1()
    {
        InitializeComponent();

        _imagePreparer = new ImagePreparer();

        _storage = new ComponentStorage();
        _storage.FillComponentStorage("..\\..\\..\\Storage\\сomponents.txt", _imagePreparer);
        _analyzer = new ImageAnalyzer(_storage);

        openFileDialog1.Filter = $"Bitmap files (*.bmp)|*.bmp";

        _imagePreparer.IsInteractive = true;
        _bitmap = new Bitmap(10, 10);
    }

    /// <summary>
    /// Открыть изображение для анализа
    /// </summary>
    private void OpenClick(object sender, EventArgs e)
    {
        // отмена
        if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
            return;

        // получаем выбранный файл
        string filename = openFileDialog1.FileName;

        // установка изображение
        _bitmap = new Bitmap(filename);
        pictureBox1.Image = _bitmap;
    }

    /// <summary>
    /// Найти образы на открытом изображении
    /// </summary>
    private void FindClick(object sender, EventArgs e)
    {
        textBox1.Clear();

        var toAnalysis = _imagePreparer.PrepareForAnalysis(_bitmap, comboBox1.SelectedIndex == 1, 0);
        var results = _analyzer.AnalyzeImage(toAnalysis);

        int[] contrast = { -40, -20, 20, 40 };

        foreach (int i in contrast)
        {
            if (results.ResultFindedItems.Count == 0 && results.ResultNewComponentItems.Count == 0 && results.is_close == true)
            {
                toAnalysis = _imagePreparer.PrepareForAnalysis(_bitmap, comboBox1.SelectedIndex == 1, i);
                // Распознавание образов, определённых и не найденных 
                results = _analyzer.AnalyzeImage(toAnalysis);
            }
        }


        if (results.ResultFindedItems.Count == 0 && results.ResultNewComponentItems.Count == 0)
        {
            // Распознавание образов, определённых и не найденных 
            results = _analyzer.AnalyzeImage(toAnalysis);
        }

        var finded = results.ResultFindedItems;

        // Если есть не расспознанные образы
        if (results.ResultNewComponentItems.Any())
        {
            // Спросить пользователя о добавлении новых образов
            var result = MessageBox.Show(
                "Программа не смогла распознать некоторые образы",
                "Вы хотите их добавить?", MessageBoxButtons.YesNo);

            if (result == DialogResult.Yes)
            {
                // Открытие формы добавления образов
                AddNewComponentForm addForm = new AddNewComponentForm(results.ResultNewComponentItems, _bitmap, _storage);
                addForm.ShowDialog();

                // добавление образов
                _storage.AddComponent(addForm.FindedComponents);

                // добавление в результат новых образов
                finded.AddRange(addForm.FindedItems);
            }
        }


        // Отсортировать слева направо и сверху вниз
        finded.Sort();

        pictureBox1.Image = new Bitmap(_bitmap);

        using Graphics graphics = Graphics.FromImage(pictureBox1.Image);

        for (int i = 0; i < finded.Count; i++)
        {
            // выделение прямоугольником
            using Pen pen = new Pen(RGBcolorCreator.GetRandomColor(), 4);
            graphics.DrawRectangle(pen, finded[i].Location);

            // Добавление значения
            string text = finded[i].MetaData.Name + ": " + finded[i].percent.ToString();
            textBox1.AppendText(text);
            textBox1.AppendText(Environment.NewLine);
        }
    }

    private void DeleteClick(object sender, EventArgs e)
    {
        DBInterface dbForm = new DBInterface(_storage);
        dbForm.ShowDialog();
    }

    /// <summary>
    /// Установить степень юинаризации
    /// </summary>
    private void trackBar1_Scroll(object sender, EventArgs e)
    {
        _imagePreparer.BinarizationMeasure = trackBar1.Value / 10.0;
    }




}

/// <summary>
/// Генератор цветов
/// </summary>
public static class RGBcolorCreator
{
    private static Random _random = new Random();

    public static Color GetRandomColor()
    {
        byte red = (byte)_random.Next(1, 255);
        byte green = (byte)_random.Next(1, 255);
        byte blue = (byte)_random.Next(1, 255);

        return Color.FromArgb(255, red, green, blue);
    }
}