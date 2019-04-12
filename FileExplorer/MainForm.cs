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
    public partial class Form1 : Form
    {
        #region Поля

        private const int BYTE_IN_KILOBYTE = 1000;
        private const int COLUMN_WIDTH = 120;
        private const int DRAG_DISTANCE = 10;
        
        private string topLevelName = "Компьютер";                                                              //Имя верхего уровня иерархии файловой системы
        private string[] viewModes = { "Крупные значки", "Мелкие значки", "Список", "Таблица", "Плитка" };      //Режимы отображеня
        
        private Dictionary<string, int> columnsFiles = new Dictionary<string, int>();                           //Набор столбцов для файлов (имя, ширина)
        private Dictionary<string, int> columnsDrives = new Dictionary<string, int>();                          //Набор столбцов для дисков (имя, ширина)
        private string[] columnsForFiles = { "Имя", "Размер", "Дата создания", "Дата изменения" };              //Столбцы для файлов и каталогов
        private string[] columnsForDrives = { "Имя", "Тип", "Файловая система", "Общий размер", "Свободно" };   //Столбцы для дисков
        
        private List<FileSystemInfo> fileSystemItems = new List<FileSystemInfo>();                              //Объекты файловой системы по текущему пути

        private IconCache iconCache = new IconCache();                                                          //Класс для работы с иконками
        private DragHelper dragHelper = new DragHelper();                                                       //Класс хелпер для перетаскивания в ListView

        #endregion

        #region Инициализация
        
        public Form1()
        {
            InitializeComponent();

            Application.ThreadException += Application_ThreadException;

            //====================================================
            //Заполнение словарей для столбцов ListView
            //====================================================

            foreach (string column in columnsForFiles)
            {
                columnsFiles.Add(column, COLUMN_WIDTH);
            }

            foreach (string column in columnsForDrives)
            {
                columnsDrives.Add(column, COLUMN_WIDTH);
            }

            //====================================================
            //Настройка комбобокса - режимы отображения
            //====================================================

            foreach (string item in viewModes)
            {
                toolStripComboBox1.Items.Add(item);
            }
            toolStripComboBox1.SelectedIndex = 0;
            toolStripComboBox1.SelectedIndexChanged += toolStripComboBox1_SelectedIndexChanged;
            

            //====================================================
            //Конфигурирование ListView
            //====================================================

            //Обработчики событий

            lv_files.MouseDoubleClick += lv_files_MouseDoubleClick;
            lv_files.ColumnClick += lv_files_ColumnClick;

            //Режим по-умолчанию

            lv_files.View = View.LargeIcon;

            //Текущий путь

            tsl_path.Text = topLevelName;

            //====================================================
            //Конфигурирование TreeView
            //====================================================

            tv_files.BeforeExpand += tv_files_BeforeExpand;
            tv_files.AfterSelect += tv_files_AfterSelect;
            
            //Редактирвание надписи

            tv_files.AfterLabelEdit += tv_files_AfterLabelEdit;

            //====================================================
            //Drag-n-Drop Oo ListView
            //====================================================

            lv_files.MouseDown += lv_files_MouseDown;
            lv_files.MouseMove += lv_files_MouseMove;
            lv_files.DragEnter += lv_files_DragEnter;
            lv_files.DragLeave += lv_files_DragLeave;
            lv_files.DragDrop += lv_files_DragDrop;
            lv_files.QueryContinueDrag += lv_files_QueryContinueDrag;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ShowDrives();
        }
        
        #endregion

        #region Drag-n-Drop

        void lv_files_MouseDown(object sender, MouseEventArgs e)
        {
            ListViewItem currentItem = lv_files.GetItemAt(e.X, e.Y);
            if (currentItem != null)
            {
                currentItem.Selected = true;
                dragHelper.isSelected = true;
                dragHelper.startPositionPoint.X = e.X;
                dragHelper.startPositionPoint.Y = e.Y;
            }
        }

        void lv_files_MouseMove(object sender, MouseEventArgs e)
        {
            //Перетаскиваем объект в листвью списке с нажатой лкм

            if (e.Button == System.Windows.Forms.MouseButtons.Left && dragHelper.isSelected)
            {
                dragHelper.currentPositionPoint.X = e.X;
                dragHelper.currentPositionPoint.Y = e.Y;
                
                //Если протащили DRAG_DISTANCE пикселов - включим перетаскивание

                if (dragHelper.GetDistance() >= DRAG_DISTANCE)
                {
                    ListViewItem currentLvi = lv_files.GetItemAt(e.X, e.Y);
                    if (currentLvi == null)
                    {
                        return;
                    }
                    lv_files.DoDragDrop(currentLvi, DragDropEffects.Move);
                    dragHelper.isDrag = true;
                }
            }
        }

        void lv_files_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        void lv_files_DragLeave(object sender, EventArgs e)
        {
            dragHelper.isDrag = false;
        }

        void lv_files_DragDrop(object sender, DragEventArgs e)
        {
            //Бросок элемента - обнуление хелпера

            dragHelper = new DragHelper();

            //Элемент на котором бросили перетаскиваемый элемент

            Point clPoint = lv_files.PointToClient(new Point(e.X, e.Y));
            ListViewItem destItem = lv_files.GetItemAt(clPoint.X, clPoint.Y);

            if (destItem != null)
            {
                //Проверка, каталог ли это

                DirectoryInfo destDirectory = destItem.Tag as DirectoryInfo;

                if (destDirectory == null)
                {
                    return;
                }
                else
                {
                    //Не тот же ли это каталог

                    ListViewItem tempLvi = (ListViewItem)e.Data.GetData(typeof(ListViewItem));
                    FileSystemInfo tempFsi = (FileSystemInfo)tempLvi.Tag;
                    DirectoryInfo tempDi = tempFsi as DirectoryInfo;
                    if (tempDi != null)
                    {
                        if (tempDi == destDirectory)
                        {
                            return;
                        }
                    }
                }

                //Перемещаем объект в каталог

                ListViewItem srcItem = (ListViewItem)e.Data.GetData(typeof(ListViewItem));
                FileSystemInfo srcFileSystemItem = srcItem.Tag as FileSystemInfo;

                //Новый имя

                string newPath = Path.Combine(destDirectory.FullName, srcFileSystemItem.Name);
                if (MoveFileObject(srcFileSystemItem, newPath))
                {
                    //Обновить список

                    if (SetFileSystemItems(tsl_path.Text))
                    {
                        ShowFileSystemItems();
                    }

                    //Обновить дерево

                    tv_files.SelectedNode.Collapse();
                    tv_files.SelectedNode.Expand();
                }
            }
        }

        //Если покинули ListView - прекратим перетаскивание

        void lv_files_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {
            if (!dragHelper.isDrag)
            {
                e.Action = DragAction.Cancel;
            }
        }

        #endregion

        #region Обработчики событий

        //====================================================
        //Неперехваченное исключение
        //====================================================

        void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            MessageBox.Show(e.Exception.Message, "Катастрофа, что-то пошло не так", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        //====================================================
        //Изменение режима отображения (выбор в комбобоксе)
        //====================================================

        void toolStripComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (toolStripComboBox1.SelectedItem.ToString())
            {
                case "Крупные значки":
                    lv_files.View = View.LargeIcon;
                    break;
                case "Мелкие значки":
                    lv_files.View = View.SmallIcon;
                    break;
                case "Таблица":
                    lv_files.View = View.Details;
                    lv_files.FullRowSelect = true;
                    break;
                case "Список":
                    lv_files.View = View.List;
                    break;
                case "Плитка":
                    lv_files.View = View.Tile;
                    break;
                default:
                    MessageBox.Show("Неизвестный режим отображения", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
            }
        }

        //====================================================
        //Клик по элементу в ListView
        //====================================================

        void lv_files_MouseDoubleClick(object sender, MouseEventArgs e)
        {

            //Получаем элемент, по которому совершён клик (по координатам)

            ListViewItem selection = lv_files.GetItemAt(e.X, e.Y);

            //Получаем связанный с ним объект

            object tag = selection.Tag;

            //Файл - запускаем

            if (tag is FileInfo)
            {
                Process.Start(((FileInfo)tag).FullName);
                return;
            }

            //Получаем файлы/каталоги корня диска

            string path = null;
            if (tag is DriveInfo)
            {
                path = ((DriveInfo)tag).RootDirectory.ToString();
            }
            else if (tag is DirectoryInfo)
            {
                path = ((DirectoryInfo)tag).FullName;
            }

            //Переход по пути

            if (SetFileSystemItems(path))
            {
                ShowFileSystemItems();
                tsl_path.Text = path;

                //Открыть путь в дереве

                ShowPathInTree(path);
            }
        }

        //====================================================
        //Выход на уровень вверх (кнопка)
        //====================================================

        void tsb_upLevel_Click(object sender, EventArgs e)
        {
            //Текущий каталог

            string path = tsl_path.Text;

            if (path == topLevelName)
            {
                return;
            }

            DirectoryInfo currentDirectory = new DirectoryInfo(path);

            //Родительский каталог

            DirectoryInfo parentDirectory = currentDirectory.Parent;

            //Смена каталога

            if (parentDirectory != null)
            {
                SetFileSystemItems(parentDirectory.FullName);
                ShowFileSystemItems();
                tsl_path.Text = parentDirectory.FullName;
                ShowPathInTree(tsl_path.Text);
            }
            else
            {
                ShowDrives();
            }
        }

        //====================================================
        //Клик по столбцу - сортировка
        //====================================================

        void lv_files_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            //Диски не сортируем

            if (tsl_path.Text == topLevelName)
            {
                return;
            }

            //Индекс столца по которому кликнули

            int currentColumn = e.Column;

            //Получаем текущий 'Сравнитель'

            FileSystemComparer currentComparer = (FileSystemComparer)lv_files.ListViewItemSorter;

            //Если его нет - создадим (0 столбец, возрастание)

            if (currentComparer == null)
            {
                currentComparer = new FileSystemComparer();
            }
            
            //Если клик по столбцу по которому выполнена сортировка - меняем направление и иконку

            if (currentColumn == currentComparer.columnIndex)
            {
                if (currentComparer.sortOrder == FileSystemComparer.SORTORDER.ASC)
                {
                    currentComparer.sortOrder = FileSystemComparer.SORTORDER.DESC;
                    lv_files.Columns[currentColumn].ImageIndex = 3;
                }
                else
                {
                    currentComparer.sortOrder = FileSystemComparer.SORTORDER.ASC;
                    lv_files.Columns[currentColumn].ImageIndex = 2;
                }
            }

            //По новому столбцу - настраиваем компарер и иконку

            else
            {
                lv_files.Columns[currentComparer.columnIndex].ImageIndex = -1;
                lv_files.Columns[currentComparer.columnIndex].TextAlign = HorizontalAlignment.Center;
                lv_files.Columns[currentComparer.columnIndex].TextAlign = HorizontalAlignment.Left;

                currentComparer.columnIndex = currentColumn;
                currentComparer.sortOrder = FileSystemComparer.SORTORDER.ASC;
                lv_files.Columns[currentColumn].ImageIndex = 2;
            }

            //Сортируем

            lv_files.ListViewItemSorter = currentComparer;
            lv_files.Sort();
        }
        
        //====================================================
        //Заполнение узла дерева при распахивании
        //====================================================

        void tv_files_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            TreeNode currentNode = e.Node;
            
            currentNode.Nodes.Clear();

            string[] directories = Directory.GetDirectories(currentNode.FullPath);
            foreach (string directory in directories)
            {
                TreeNode t = new TreeNode(Path.GetFileName(directory), 0, 1);
                currentNode.Nodes.Add(t);

                try
                {
                    string[] a = Directory.GetDirectories(directory);
                    if (a.Length > 0)
                    {
                        t.Nodes.Add("?");
                    }
                }
                catch { }
            }
        }

        //====================================================
        //Выбор узла дерева - выводим содержимое в ListView
        //====================================================

        void tv_files_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode selectedNode = e.Node;
            string currentPath = e.Node.FullPath;

            //Откроем текущий узел

            selectedNode.Expand();

            //Отразим в списке текущий узел

            if (SetFileSystemItems(currentPath))
            {
                ShowFileSystemItems();
                tsl_path.Text = currentPath;
            }

            //Вернём активный узел если не удалось перейти в каталог

            else
            {
                ShowPathInTree(tsl_path.Text);
            }
        }

        //====================================================
        //Переименование папки при редактировании узла в дереве
        //====================================================

        void tv_files_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            //Текущий путь и новое имя

            string currentPath = e.Node.FullPath;
            string newDirectoryName = e.Label;

            if (newDirectoryName == null || newDirectoryName.Trim().Length == 0)
            {
                e.CancelEdit = true;
                return;
            }

            //Новое полное имя папки

            string newFullName = Path.Combine(e.Node.Parent.FullPath, newDirectoryName);

            //Текущий каталог

            DirectoryInfo currentDirectory = new DirectoryInfo(currentPath);

            //Переименование

            try
            {
                currentDirectory.MoveTo(newFullName);

                //Обновить путь и содержимое списка

                if (SetFileSystemItems(newFullName))
                {
                    ShowFileSystemItems();
                    tsl_path.Text = newFullName;
                }
            }
            catch
            {
                MessageBox.Show("Невозможно переименовать каталог", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Information);
                e.CancelEdit = true;
            }
        }
        
        #endregion

        #region Работа с файловой системой

        //====================================================
        //Получить список файлов и каталогов для указанного пути
        //====================================================

        public bool SetFileSystemItems(string path)
        {
            //Проверка доступа
            
            try
            {
                string[] access = Directory.GetDirectories(path);
            }
            catch 
            {
                MessageBox.Show("Невозможно прочитать каталог", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }


            //Очистка списка

            if (fileSystemItems != null && fileSystemItems.Count != 0)
            {
                fileSystemItems.Clear();
            }

            //Список каталогов

            string[] directories = Directory.GetDirectories(path);
            foreach (string directory in directories)
            {
                DirectoryInfo di = new DirectoryInfo(directory);
                fileSystemItems.Add(di);
            }
            
            //Список файлов

            string[] files = Directory.GetFiles(path);
            foreach (string file in files)
            {
                FileInfo fi = new FileInfo(file);
                fileSystemItems.Add(fi);
            }

            return true;
        }

        //====================================================
        //Отобразить список каталогов/файлов в ListView
        //====================================================

        private void ShowFileSystemItems()
        {
            lv_files.BeginUpdate();

            //Очистим ListView

            lv_files.Items.Clear();

            if (fileSystemItems == null || fileSystemItems.Count == 0)
            {
                return;
            }

            //Установка столбцов для папок

            SetColumsForFolders();                          

            //Очистим кэш иконок и списки изображений

            iconCache.ClearIconCashAndLists(il_DiscFoldersFilesIcons_Small, il_DiscFoldersFilesIcons_Large);

            //Заполнение списка

            ListViewItem lviFile = null;
            foreach (FileSystemInfo file in fileSystemItems)
            {
                    
                lviFile = new ListViewItem();
                lviFile.Tag = file;
                lviFile.Text = file.Name;
                
                //Каталог
                
                if (file is DirectoryInfo)
                {
                    lviFile.ImageIndex = 1;
                    lviFile.SubItems.Add("Каталог");
                }

                //Файл

                else if (file is FileInfo)
                {
                    FileInfo currentFile = file as FileInfo;
                    if (currentFile == null) 
                    {
                        return;
                    }

                    string fileExtention = currentFile.Extension.ToLower();

                    //====================================================
                    //Назначение иконку файлу
                    //====================================================

                    //Поиск в кеше

                    int iconIndex = iconCache.GetIconIndexByExtention(fileExtention);
                    
                    //Есть в кэше
                    
                    if (iconIndex != -1)
                    {
                        lviFile.ImageIndex = iconIndex;
                    }

                    //Нет в кэше

                    else
                    {
                        lviFile.ImageIndex = iconCache.AddIconForFile((FileInfo)file, il_DiscFoldersFilesIcons_Small, il_DiscFoldersFilesIcons_Large);
                    }
                    lviFile.SubItems.Add(((FileInfo)file).Length.ToString());
                }
                lviFile.SubItems.Add(file.CreationTime.ToString());
                lviFile.SubItems.Add(file.LastWriteTime.ToString());

                lv_files.Items.Add(lviFile);

                lv_files.EndUpdate();
            }
        }

        //====================================================
        //Отобразить список дисков
        //====================================================

        private void ShowDrives()
        {
            tsl_path.Text = topLevelName;

            //Очистка

            if (lv_files != null && lv_files.Items.Count != 0)
            {
                lv_files.Items.Clear();
            }
            if (tv_files != null && tv_files.Nodes.Count != 0)
            {
                tv_files.Nodes.Clear();
            }

            //Получение дисков (доступных для чтения)

            DriveInfo[] discs = DriveInfo.GetDrives();

            if (discs.Length == 0)
            {
                MessageBox.Show("Диски не обнаружены", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #region ListView

            //Настройка столбцов для дисков

            SetColumsForDrives();

            ListViewItem lviDisc;
            foreach (DriveInfo disc in discs)
            {
                if (disc.IsReady)
                {
                    string totalSize = String.Format("{0:F2} Гб", (double)disc.TotalSize / (BYTE_IN_KILOBYTE * BYTE_IN_KILOBYTE * BYTE_IN_KILOBYTE));             //в Гб
                    string freeSpace = String.Format("{0:F2} Гб", (double)disc.TotalFreeSpace / (BYTE_IN_KILOBYTE * BYTE_IN_KILOBYTE * BYTE_IN_KILOBYTE));        //в Гб

                    lviDisc = new ListViewItem(disc.Name, 0);
                    lviDisc.SubItems.Add(disc.DriveType.ToString());
                    lviDisc.SubItems.Add(disc.DriveFormat.ToString());
                    lviDisc.SubItems.Add(totalSize);
                    lviDisc.SubItems.Add(freeSpace);

                    lviDisc.Tag = disc;

                    lv_files.Items.Add(lviDisc);
                }
            }
            #endregion

            #region TreeView

            foreach (DriveInfo disc in discs)
            {
                if(disc.IsReady)
                {
                    //Добавить диск

                    TreeNode tnDisc = new TreeNode(disc.Name, 2, 2);
                    tv_files.Nodes.Add(tnDisc);
                
                    //Добавить "+", если есть содержимое

                    try
                    {
                        string[] directoriesInDisc = Directory.GetDirectories(disc.RootDirectory.ToString());
                        if (directoriesInDisc.Length > 0)
                        {
                            TreeNode tempNode = new TreeNode("?");
                            tnDisc.Nodes.Add(tempNode);
                        }
                    }
                    catch { }
                }
            }

            #endregion
        }

        //====================================================
        //Выделить в дереве узел по пути
        //====================================================

        private void ShowPathInTree(string path)
        {
            //Массив директорий

            string[] directories = path.Split('\\');

            //Корневой каталог
            
            string root = Path.GetPathRoot(path);

            //Откроем корневой катлог

            TreeNode currentNode = null;
            foreach (TreeNode treeNode in tv_files.Nodes)
            {
                if (treeNode.Text == root)
                {
                    treeNode.Expand();
                    currentNode = treeNode;
                    break;
                }
            }

            //Ищем каталоги и открываем их в дереве

            for (int i = 1; i < directories.Length; i++)
            {
                //Пропускаем директории без имени

                if (directories[i].Length == 0) 
                {
                    continue;
                }

                //Ищем каталог

                foreach (TreeNode treeNode in currentNode.Nodes)
                {
                    if (treeNode.Text == directories[i])
                    {
                        treeNode.Expand();
                        currentNode = treeNode;
                    }
                }
            }

            //Выберем узел в дереве

            tv_files.SelectedNode = currentNode;
        }

        private bool MoveFileObject(FileSystemInfo fsObject, string newPath)
        {
            string message = "";
            try
            {
                if (fsObject is DirectoryInfo)
                {
                    message = "Не возможно переместить каталог";
                    ((DirectoryInfo)fsObject).MoveTo(newPath);
                }
                else
                {
                    message = "Не возможно переместить файл";
                    ((FileInfo)fsObject).MoveTo(newPath);
                }
                return true;
            }
            catch
            {
                MessageBox.Show(message, "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
        }
        
        #endregion

        #region Столбцы

        //====================================================
        //Установить столбцы для режима таблицы(colums - словарь: имя / ширина)
        //====================================================

        private void SetColumsForDrives()
        {
            if (lv_files.Columns.Count != 0)
            {
                lv_files.Columns.Clear();
            }

            ColumnHeader column = null;
            foreach (KeyValuePair<string, int> item in columnsDrives)
            {
                column = new ColumnHeader();
                column.Text = item.Key;
                column.Width = item.Value;
                column.TextAlign = HorizontalAlignment.Left;
                lv_files.Columns.Add(column);
            }
        }

        private void SetColumsForFolders()
        {
            if (lv_files.Columns.Count != 0)
            {
                lv_files.Columns.Clear();
            }
            
            //Сортировка по умолчанию

            int sortedColumnIndex = 0;
            FileSystemComparer.SORTORDER sortOrder = FileSystemComparer.SORTORDER.ASC;

            //Получаем 'Сравнитель' для ListView читаем параметры сортировки

            FileSystemComparer currentComparer = (FileSystemComparer)lv_files.ListViewItemSorter;
            if (currentComparer != null)
            {
                sortedColumnIndex = currentComparer.columnIndex;
                sortOrder = currentComparer.sortOrder;
            }

            //Создаём столцы, с установленной иконкой

            ColumnHeader column = null;
            int currentColumnIndex = 0;
            foreach (KeyValuePair<string, int> item in columnsFiles)
            {
                column = new ColumnHeader();
                column.Text = item.Key;
                column.Width = item.Value;
                
                //Иконка для столца сортированного столбца
                
                if (sortedColumnIndex == currentColumnIndex)
                {
                    if (sortOrder == FileSystemComparer.SORTORDER.ASC)
                    {
                        column.ImageIndex = 2;
                    }
                    else
                    {
                        column.ImageIndex = 3;
                    }
                }

                lv_files.Columns.Add(column);
                currentColumnIndex++;
            }
        }

        #endregion
    }
}