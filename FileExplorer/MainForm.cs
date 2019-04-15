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
using OutlookHandle;

namespace FileExplorer
{
    public partial class Form1 : Form
    {
        #region Fields

        private const int BYTE_IN_KILOBYTE = 1000;
        private const int COLUMN_WIDTH = 120;
        private const int DRAG_DISTANCE = 10;
        
        private string topLevelName = "Computer";                                                              //The name of the top level of the file system hierarchy
        private string[] viewModes = { "Large icons", "Small icons", "List", "Table", "Tile" };                 //Display Modes

        private Dictionary<string, int> columnsFiles = new Dictionary<string, int>();                           //A set of columns for files (name, width)
        private Dictionary<string, int> columnsDrives = new Dictionary<string, int>();                          //Column set for disks (name, width)
        private string[] columnsForFiles = { "Name", "Size", "Date of creation", "Date of change" };              //Columns for files and directories
        private string[] columnsForDrives = { "Name", "Type", "File system", "Overall size", "Free" };          //Columns for disks

        private List<FileSystemInfo> fileSystemItems = new List<FileSystemInfo>();                              //File system objects on the current path

        private IconCache iconCache = new IconCache();                                                          //Class to work with icons
        private DragHelper dragHelper = new DragHelper();                                                       //Helper class for dragging in a ListView

        #endregion

        #region Initialization

        public Form1()
        {
            InitializeComponent();

            Application.ThreadException += Application_ThreadException;

            //====================================================
            //Filling dictionaries for ListView columns
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
            //Combo Box Setup - Display Modes
            //====================================================

            foreach (string item in viewModes)
            {
                toolStripComboBox1.Items.Add(item);
            }
            toolStripComboBox1.SelectedIndex = 0;
            toolStripComboBox1.SelectedIndexChanged += toolStripComboBox1_SelectedIndexChanged;


            //====================================================
            //Configuring a ListView
            //====================================================

            //Event handlers

            lv_files.MouseDoubleClick += lv_files_MouseDoubleClick;
            lv_files.ColumnClick += lv_files_ColumnClick;

            //Default mode

            lv_files.View = View.LargeIcon;

            //Current path

            tsl_path.Text = topLevelName;

            //====================================================
            //Configuring a TreeView
            //====================================================

            tv_files.BeforeExpand += tv_files_BeforeExpand;
            tv_files.AfterSelect += tv_files_AfterSelect;

            //Editing labels

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
            //Dragging an object in the list with LKM pressed

            if (e.Button == System.Windows.Forms.MouseButtons.Left && dragHelper.isSelected)
            {
                dragHelper.currentPositionPoint.X = e.X;
                dragHelper.currentPositionPoint.Y = e.Y;

                //If DRAG_DISTANCE pixels are dragged, we will enable dragging

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
            //Element Roll - Helper Zero

            dragHelper = new DragHelper();

            //The item on which the draggable item was dropped

            Point clPoint = lv_files.PointToClient(new Point(e.X, e.Y));
            ListViewItem destItem = lv_files.GetItemAt(clPoint.X, clPoint.Y);

            if (destItem != null)
            {
                //Check if it is a directory

                DirectoryInfo destDirectory = destItem.Tag as DirectoryInfo;

                if (destDirectory == null)
                {
                    return;
                }
                else
                {
                    //Is this the same directory?

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

                //Moving an object to the directory

                ListViewItem srcItem = (ListViewItem)e.Data.GetData(typeof(ListViewItem));
                FileSystemInfo srcFileSystemItem = srcItem.Tag as FileSystemInfo;

                //New name

                string newPath = Path.Combine(destDirectory.FullName, srcFileSystemItem.Name);
                if (MoveFileObject(srcFileSystemItem, newPath))
                {
                    //Refresh list

                    if (SetFileSystemItems(tsl_path.Text))
                    {
                        ShowFileSystemItems();
                    }

                    //Refresh tree

                    tv_files.SelectedNode.Collapse();
                    tv_files.SelectedNode.Expand();
                }
            }
        }

        //If left ListView - stop dragging

        void lv_files_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {
            if (!dragHelper.isDrag)
            {
                e.Action = DragAction.Cancel;
            }
        }

        #endregion

        #region Event handlers

        //====================================================
        //Uncaught Exception
        //====================================================

        void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            MessageBox.Show(e.Exception.Message, "Disaster, something went wrong", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        //====================================================
        //Changing the display mode (choice in combo box)
        //====================================================

        void toolStripComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (toolStripComboBox1.SelectedItem.ToString())
            {
                case "Large icons":
                    lv_files.View = View.LargeIcon;
                    break;
                case "Small icons":
                    lv_files.View = View.SmallIcon;
                    break;
                case "Table":
                    lv_files.View = View.Details;
                    lv_files.FullRowSelect = true;
                    break;
                case "List":
                    lv_files.View = View.List;
                    break;
                case "Tile":
                    lv_files.View = View.Tile;
                    break;
                default:
                    MessageBox.Show("Unknown display mode", "Mistake", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
            }
        }

        //====================================================
        //Click an item in the ListView
        //====================================================

        void lv_files_MouseDoubleClick(object sender, MouseEventArgs e)
        {

            //Get the element that clicked (by coordinates)

            ListViewItem selection = lv_files.GetItemAt(e.X, e.Y);

            //Get the object associated with it

            object tag = selection.Tag;

            //File - run

            if (tag is FileInfo)
            {
                Process.Start(((FileInfo)tag).FullName);
                return;
            }

            //We receive files / directories of a root of a disk

            string path = null;
            if (tag is DriveInfo)
            {
                path = ((DriveInfo)tag).RootDirectory.ToString();
            }
            else if (tag is DirectoryInfo)
            {
                path = ((DirectoryInfo)tag).FullName;
            }

            //Go on the way

            if (SetFileSystemItems(path))
            {
                ShowFileSystemItems();
                tsl_path.Text = path;

                //Open the path in the tree

                ShowPathInTree(path);
            }
        }

        //====================================================
        //Up level (button)
        //====================================================

        void tsb_upLevel_Click(object sender, EventArgs e)
        {
            //Current directory

            string path = tsl_path.Text;

            if (path == topLevelName)
            {
                return;
            }

            DirectoryInfo currentDirectory = new DirectoryInfo(path);

            //Parent directory

            DirectoryInfo parentDirectory = currentDirectory.Parent;

            //Change directory

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
        //Click on the column - sorting
        //====================================================

        void lv_files_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            //Disks do not sort

            if (tsl_path.Text == topLevelName)
            {
                return;
            }

            //Index of the table clicked on

            int currentColumn = e.Column;

            //We get the current 'Comparative'

            FileSystemComparer currentComparer = (FileSystemComparer)lv_files.ListViewItemSorter;

            //If not, create one (0 column, ascending)

            if (currentComparer == null)
            {
                currentComparer = new FileSystemComparer();
            }

            //If you click on the column on which the sorting is performed, change the direction and the icon

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

            //On a new column - set up a comparer and an icon

            else
            {
                lv_files.Columns[currentComparer.columnIndex].ImageIndex = -1;
                lv_files.Columns[currentComparer.columnIndex].TextAlign = HorizontalAlignment.Center;
                lv_files.Columns[currentComparer.columnIndex].TextAlign = HorizontalAlignment.Left;

                currentComparer.columnIndex = currentColumn;
                currentComparer.sortOrder = FileSystemComparer.SORTORDER.ASC;
                lv_files.Columns[currentColumn].ImageIndex = 2;
            }

            //Sorting

            lv_files.ListViewItemSorter = currentComparer;
            lv_files.Sort();
        }

        //====================================================
        //Filling a tree node when plowing
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
        //Selecting a tree node - display the contents in a ListView
        //====================================================

        void tv_files_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode selectedNode = e.Node;
            string currentPath = e.Node.FullPath;

            //Open the current node

            selectedNode.Expand();

            //Reflect the current node in the list

            if (SetFileSystemItems(currentPath))
            {
                ShowFileSystemItems();
                tsl_path.Text = currentPath;
            }

            //We will return the active node if we could not go to the directory

            else
            {
                ShowPathInTree(tsl_path.Text);
            }
        }

        //====================================================
        //Rename a folder when editing a node in the tree
        //====================================================

        void tv_files_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            //Current path and new name

            string currentPath = e.Node.FullPath;
            string newDirectoryName = e.Label;

            if (newDirectoryName == null || newDirectoryName.Trim().Length == 0)
            {
                e.CancelEdit = true;
                return;
            }

            //New full folder name

            string newFullName = Path.Combine(e.Node.Parent.FullPath, newDirectoryName);

            //Current directory

            DirectoryInfo currentDirectory = new DirectoryInfo(currentPath);

            //Rename

            try
            {
                currentDirectory.MoveTo(newFullName);

                //Update path and list contents

                if (SetFileSystemItems(newFullName))
                {
                    ShowFileSystemItems();
                    tsl_path.Text = newFullName;
                }
            }
            catch
            {
                MessageBox.Show("Cannot rename directory", "Attention", MessageBoxButtons.OK, MessageBoxIcon.Information);
                e.CancelEdit = true;
            }
        }

        #endregion

        #region Work with file system

        //====================================================
        //Get a list of files and directories for the specified path
        //====================================================

        public bool SetFileSystemItems(string path)
        {
            //Access check

            try
            {
                string[] access = Directory.GetDirectories(path);
            }
            catch 
            {
                MessageBox.Show("Unable to read directory", "Attention", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }


            //Clearing the list

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
        //Display a list of directories / files in a ListView
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

            //Setting Folder Columns

            SetColumsForFolders();

            //Clear the cache of icons and image lists

            iconCache.ClearIconCashAndLists(il_DiscFoldersFilesIcons_Small, il_DiscFoldersFilesIcons_Large);

            //Filling the list

            ListViewItem lviFile = null;
            foreach (FileSystemInfo file in fileSystemItems)
            {
                    
                lviFile = new ListViewItem();
                lviFile.Tag = file;
                lviFile.Text = file.Name;

                //Catalog

                if (file is DirectoryInfo)
                {
                    lviFile.ImageIndex = 1;
                    lviFile.SubItems.Add("Catalog");
                }

                //File

                else if (file is FileInfo)
                {
                    FileInfo currentFile = file as FileInfo;
                    if (currentFile == null) 
                    {
                        return;
                    }

                    string fileExtention = currentFile.Extension.ToLower();

                    //====================================================
                    //Assigning an icon to a file
                    //====================================================

                    //Search cache

                    int iconIndex = iconCache.GetIconIndexByExtention(fileExtention);

                    //Cached

                    if (iconIndex != -1)
                    {
                        lviFile.ImageIndex = iconIndex;
                    }

                    //Not in cache

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
        //Display a list of disks
        //====================================================

        private void ShowDrives()
        {
            tsl_path.Text = topLevelName;

            //Cleaning

            if (lv_files != null && lv_files.Items.Count != 0)
            {
                lv_files.Items.Clear();
            }
            if (tv_files != null && tv_files.Nodes.Count != 0)
            {
                tv_files.Nodes.Clear();
            }

            //Getting discs (available for reading)

            DriveInfo[] discs = DriveInfo.GetDrives();

            if (discs.Length == 0)
            {
                MessageBox.Show("Диски не обнаружены", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #region ListView

            //Setting up columns for disks

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
                    //Add drive

                    TreeNode tnDisc = new TreeNode(disc.Name, 2, 2);
                    tv_files.Nodes.Add(tnDisc);

                    //Add "+" if there is content

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
        //Select a node in the tree on the way
        //====================================================

        private void ShowPathInTree(string path)
        {
            //Directory Array

            string[] directories = path.Split('\\');

            //Root directory

            string root = Path.GetPathRoot(path);

            //Open the root directory

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

            //We are looking for catalogs and open them in the tree.

            for (int i = 1; i < directories.Length; i++)
            {
                //Skipping unnamed directories

                if (directories[i].Length == 0) 
                {
                    continue;
                }

                //Looking for a catalog

                foreach (TreeNode treeNode in currentNode.Nodes)
                {
                    if (treeNode.Text == directories[i])
                    {
                        treeNode.Expand();
                        currentNode = treeNode;
                    }
                }
            }

            //Select a node in the tree

            tv_files.SelectedNode = currentNode;
        }

        private bool MoveFileObject(FileSystemInfo fsObject, string newPath)
        {
            string message = "";
            try
            {
                if (fsObject is DirectoryInfo)
                {
                    message = "Cannot move directory";
                    ((DirectoryInfo)fsObject).MoveTo(newPath);
                }
                else
                {
                    message = "Cannot move file";
                    ((FileInfo)fsObject).MoveTo(newPath);
                }
                return true;
            }
            catch
            {
                MessageBox.Show(message, "Attention", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
        }

        #endregion

        #region Columns

        //====================================================
        //Set columns for table mode (colums - dictionary: name / width)
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

            //Default Sort

            int sortedColumnIndex = 0;
            FileSystemComparer.SORTORDER sortOrder = FileSystemComparer.SORTORDER.ASC;

            //We get the 'Comparison' for the ListView; we read the collation

            FileSystemComparer currentComparer = (FileSystemComparer)lv_files.ListViewItemSorter;
            if (currentComparer != null)
            {
                sortedColumnIndex = currentComparer.columnIndex;
                sortOrder = currentComparer.sortOrder;
            }

            //Create tables, with the icon installed

            ColumnHeader column = null;
            int currentColumnIndex = 0;
            foreach (KeyValuePair<string, int> item in columnsFiles)
            {
                column = new ColumnHeader();
                column.Text = item.Key;
                column.Width = item.Value;

                //Icon for the column sorted column

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

        private void button1_Click(object sender, EventArgs e)
        {
            string currPath = "";
            //Current path
            currPath = tsl_path.Text = topLevelName;
            Extension readEmail = new Extension();
            readEmail.SaveEmailMarked("a:\\Dropbox");
        }
    }
}