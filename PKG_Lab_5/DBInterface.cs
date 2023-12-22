using PKG_Lab_5.ImageAnalizer;
using PKG_Lab_5.ImagesData;
using PKG_Lab_5.Storages;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace PKG_Lab_5
{
    public partial class DBInterface : Form
    {
        public ComponentStorage storage;

        public DBInterface(ComponentStorage container)
        {
            storage = container;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string index = textBox1.Text;
            string hash = textBox2.Text;

            if (index.Length > 0)
            {
                int i_index;
                Int32.TryParse(index, out i_index);
                if (i_index < storage._components.Count)
                {
                    storage._components.RemoveAt(i_index);
                    MessageBox.Show(
                        $"Элемент с индексом {i_index} удален",
                        "Удаление завершено",
                        MessageBoxButtons.OK
                        );
                }
                else
                {
                    MessageBox.Show(
                        $"Элемент с индексом {i_index} отсутствует",
                        "Удаление невозможно",
                        MessageBoxButtons.OK
                        );
                }
            }
            else if (hash.Length > 0)
            {
                UInt64 ui_hash;
                UInt64.TryParse(hash, out ui_hash);
                bool is_deleted = false;

                for (int i = 0; i < storage._components.Count; i++)
                {
                    if (storage._components[i].Hash == ui_hash)
                    {
                        storage._components.RemoveAt(i);
                        is_deleted = true;
                    }
                }
                if (is_deleted)
                {
                    MessageBox.Show(
                        $"Элемент с хэшом {ui_hash} удален",
                        "Удаление завершено",
                        MessageBoxButtons.OK
                        );
                }
                else
                {
                    MessageBox.Show(
                        $"Элемент с хэшом {ui_hash} не найден",
                        "Удаление невозможно",
                        MessageBoxButtons.OK
                        );
                }
            }
            else
            {
                MessageBox.Show(
                    "Введите индекс или хэш для удаления элемента",
                    "Удаление невозможно",
                    MessageBoxButtons.OK
                    );
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {

            if (textBox2.Text.Length > 0 && textBox3.Text.Length > 0)
            {
                MetaData metaData = new MetaData(textBox3.Text);
                ulong ui_hash;
                UInt64.TryParse(textBox2.Text, out ui_hash);
                storage.AddComponent(new ComponentData(metaData, ui_hash));

                MessageBox.Show(
                    $"Образ с описанием {textBox3.Text} и хэшом {ui_hash} добавлен",
                    "Добавление завершено",
                    MessageBoxButtons.OK
                    );
            }
            else
            {
                MessageBox.Show(
                    "Проверьте ввод метаданых и хэша",
                    "Добавление невозможно",
                    MessageBoxButtons.OK
                    );
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length > 0)
            {
                int i_index;
                Int32.TryParse(textBox1.Text, out i_index);
                if (i_index < storage._components.Count)
                {
                    MessageBox.Show(
                        $"Элемент с индексом {i_index} имеет хэш {storage._components[i_index].Hash}",
                        "Поиск завершен",
                        MessageBoxButtons.OK
                        );
                }
                else
                {
                    MessageBox.Show(
                        $"Элемент с индексом {i_index} отсутствует",
                        "Поиск завершен",
                        MessageBoxButtons.OK
                        );
                }
            }
            else if (textBox2.Text.Length > 0)
            {
                UInt64 ui_hash;
                UInt64.TryParse(textBox2.Text, out ui_hash);
                int finded_item_index = -1;

                for (int i = 0; i < storage._components.Count; i++)
                {
                    if (storage._components[i].Hash == ui_hash)
                    {
                        finded_item_index = i;
                    }
                }
                if (finded_item_index >= 0)
                {
                    MessageBox.Show(
                        $"Элементы с хэшом {ui_hash} имеет индекс {finded_item_index}",
                        "Поиск завершен",
                        MessageBoxButtons.OK
                        );
                }
                else
                {
                    MessageBox.Show(
                        $"Элемент с хэшом {ui_hash} не найден",
                        "Поиск завершен",
                        MessageBoxButtons.OK
                        );
                }
            }
            else
            {
                MessageBox.Show(
                    "Проверьте ввод данных",
                    "Поиск невозможен",
                    MessageBoxButtons.OK
                    );
            }
        }
    }
}
