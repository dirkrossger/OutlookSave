using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace FileExplorer
{
    /// <summary>
    /// Класс для кэша иконок и работы со списками изображений ListView
    /// </summary>
    class IconCache
    {
        private Dictionary<string, int> iconCashe = new Dictionary<string, int>();          //Словарь для хранения индексов иконок для расширений файлов (расширение => индекс в списке изображений ListView)
        private int reserveRange = 4;                                                       //Количество зарезервированных иконок в списке

        /// <summary>
        /// Возращает индекс изображения в списке изображений ListView для расширения файла
        /// </summary>
        /// <param name="extention">Расширение файла</param>
        /// <returns>Индекс изображения или -1 если изображения нет</returns>
        public int GetIconIndexByExtention(string extention)
        {
            //to-do Вернуть индекс стандартной иконки для файлов без расширения

            List<string> extentions = iconCashe.Keys.ToList<string>();
            if (extention == null || extention.Length == 0 || extention == ".exe" || extentions.IndexOf(extention) == -1)
            {
                return -1;
            }
            return iconCashe[extention];
        }

        /// <summary>
        /// Добавить иконку для типа файла в списки изображений ListView элемента и кеш иконок
        /// </summary>
        /// <param name="file">Файл для которого добавляется иконка</param>
        /// <param name="smallIconList">Список маленьких изображений</param>
        /// <param name="largeIconList">Список больших изображений</param>
        /// <returns>Индекс добавленного изображения в списках изображений</returns>
        public int AddIconForFile(FileInfo file, ImageList smallIconList, ImageList largeIconList)
        {
            string fileExtention = file.Extension.ToLower();

            //Получаем иконку

            Icon fileIcon = Icon.ExtractAssociatedIcon(file.FullName);
            if (fileIcon == null)
            {
                return 0;
            }

            //В списки

            smallIconList.Images.Add(fileIcon);
            largeIconList.Images.Add(fileIcon);

            //Кешируем иконки не для .exe файлов

            if (fileExtention != ".exe" && fileExtention.Length != 0)
            {
                //Добавляем в конец

                iconCashe.Add(fileExtention, smallIconList.Images.Count - 1);
            }

            //Вернули последний индекс

            return smallIconList.Images.Count - 1;
        }

        /// <summary>
        /// Очистить кеш и список иконок
        /// </summary>
        /// <param name="smallIconList">Список изображений маленьких иконок</param>
        /// <param name="largeIconList">Список изображений больших иконок</param>
        public void ClearIconCashAndLists(ImageList smallIconList, ImageList largeIconList)
        {
            iconCashe.Clear();
            for (int i = reserveRange; i < smallIconList.Images.Count; i++)
            {
                smallIconList.Images.RemoveAt(i);
                largeIconList.Images.RemoveAt(i);
            }
        }
    }
}