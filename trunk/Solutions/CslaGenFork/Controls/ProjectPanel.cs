using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CslaGenerator.Metadata;
using CslaGenerator.Util;
using WeifenLuo.WinFormsUI.Docking;

namespace CslaGenerator.Controls
{
    /// <summary>
    /// Summary description for ProjectPanel.
    /// </summary>
    public partial class ProjectPanel : DockContent
    {
        #region

        public ProjectPanel()
        {
            InitializeComponent();

            cboObjectType.Items.Add("<All>");
            foreach (var oType in Enum.GetNames(typeof(CslaObjectType)))
                cboObjectType.Items.Add(oType);
            cboObjectType.SelectedIndex = 0;
            lstObjects.DrawItem += lstObjects_DrawItem;
            lstObjects.DrawMode = DrawMode.OwnerDrawFixed;
        }

        #endregion

        #region Properties

        internal TextBox ProjectNameTextBox
        {
            get { return txtProjectName; }
        }

        internal Button TargetDirectoryButton
        {
            get { return textboxPlusBtn.Button; }
        }

        internal TextBox TargetDirectory
        {
            get { return textboxPlusBtn.TextBox; }
        }

        internal ListBox ListObjects
        {
            get { return lstObjects; }
        }

        internal bool FilterTypeIsActive
        {
            get { return cboObjectType.Text != "<All>"; }
        }

        internal bool OnlyFilesystem
        {
            get { return onlyfilesystem; }
            set { onlyfilesystem = value; }
        }

        internal RadioButton SortOptionsNone
        {
            get { return optNone; }
            set { optNone = value; }
        }

        bool UnitLoaded()
        {
            if (_objects != null)
                return true;

            MessageBox.Show(@"You need to create a new project first.", @"CslaGenerator", MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            return false;
        }

        internal CslaObjectInfoCollection Objects
        {
            get { return _objects; }
            set
            {
                if (_objects != null)
                    _objects.ListChanged -= objects_ListChanged;
                _objects = value;
                if (_objects != null)
                    _objects.ListChanged += objects_ListChanged;
            }
        }

        #endregion

        #region State fields

        CslaObjectInfoCollection _objects;
        List<CslaObjectInfo> _currentView;
        private bool _suspendFilter;
        private bool _restoreSelectedItems;
        private bool _suspendListUpdates;
        private List<CslaObjectInfo> _selectedItems;

        #endregion

        #region Event Handlers

        private void textboxPlusBtn_ButtonClicked(object sender, EventArgs e)
        {
            using (var fBrowser = new FolderBrowserDialog())
            {
                fBrowser.Description = @"Please choose an output folder for the generated code.";

                if (textboxPlusBtn.TextBox.Text.Trim().Length != 0)
                {
                    try
                    {
                        fBrowser.SelectedPath = textboxPlusBtn.TextBox.Text;
                    }
                    catch
                    {
                    }
                }
                fBrowser.ShowNewFolderButton = true;
                if (fBrowser.ShowDialog() == DialogResult.OK)
                {
                    textboxPlusBtn.TextBox.Text = fBrowser.SelectedPath;
                    OnTargetDirChanged(textboxPlusBtn.TextBox.Text);
                }
            }
        }

        private void objects_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (_suspendListUpdates)
                return;

            if (e.ListChangedType == ListChangedType.ItemChanged &&
                e.PropertyDescriptor.Name != "ObjectName" &&
                e.PropertyDescriptor.Name != "ObjectType" &&
                e.PropertyDescriptor.Name != "CslaBaseClass" &&
                e.PropertyDescriptor.Name != "IsGenericType" &&
                e.PropertyDescriptor.Name != "InheritedType" &&
                e.PropertyDescriptor.Name != "InheritedTypeWinForms" &&
                e.PropertyDescriptor.Name != "AuthzProvider")
            {
                lstObjects.Refresh();
                return;
            }

            ApplyFilters(false);
            if (e.ListChangedType == ListChangedType.ItemAdded)
            {
                // no need to fire lstObjects_SelectedIndexChanged 3 times
                DisableEventSelectedIndexChanged();

                // update the ListBox items
                ApplyFiltersPresenter();

                EnableEventSelectedIndexChanged();
                ListObjects_SelectedIndexChanged(this, new EventArgs());
                return;
            }

            ApplyFiltersPresenter();
        }

        private void cboObjectType_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyFilters(true);
        }

        private void txtFilter_TextChanged(object sender, EventArgs e)
        {
            ApplyFilters(true);
        }

        private void Sort_CheckedChanged(object sender, EventArgs e)
        {
            var opt = (RadioButton) sender;
            if (opt.Checked)
                ApplyFilters(true);
        }

        internal void ListObjects_SelectedIndexChanged(object sender, EventArgs e)
        {
            DisableEventSelectedIndexChanged();

            if (_restoreSelectedItems)
            {
                lstObjects.SelectedItems.Clear();

                // if empty list, preload with first item
                if (_selectedItems.Count == 0 && lstObjects.Items.Count > 0)
                    _selectedItems.Add((CslaObjectInfo) lstObjects.Items[0]);

                foreach (var obj in _selectedItems)
                    lstObjects.SelectedItems.Add(obj);
            }
            else
            {
                _selectedItems = new List<CslaObjectInfo>();
            }

            OnSelectedItemsChanged();

            EnableEventSelectedIndexChanged();
        }

        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddNewObject();
        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RemoveSelected();
        }

        private void duplicateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DuplicateSelected();
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SelectAll();
        }

        private void moveUpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MoveUpSelected();
        }

        private void moveDownToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MoveDownSelected();
        }

        private void newObjectRelationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddNewObjectRelation();
        }

        private void addToObjectRelationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddToObjectRelationBuilder();
        }

        private void cslaObjectContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            addToolStripMenuItem.Enabled = (_objects != null);
            removeToolStripMenuItem.Enabled = (_objects != null && lstObjects.SelectedIndices.Count != 0);
            duplicateToolStripMenuItem.Enabled = (_objects != null && lstObjects.SelectedIndices.Count != 0);
            moveUpToolStripMenuItem.Enabled = ((_objects != null && optNone.Checked &&
                                                lstObjects.SelectedIndex > 0) ||
                                               lstObjects.SelectedIndices.Count > 1);
            moveDownToolStripMenuItem.Enabled = ((_objects != null && optNone.Checked &&
                                                  lstObjects.SelectedIndex < _currentView.Count - 1) ||
                                                 lstObjects.SelectedIndices.Count > 1);
            newObjectRelationToolStripMenuItem.Enabled = (_objects != null && lstObjects.SelectedIndices.Count != 0);
            addToObjectRelationToolStripMenuItem.Enabled = (_objects != null && lstObjects.SelectedIndices.Count != 0);
        }

        private void lstObjects_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0)
                return;

            e.DrawBackground();
            if (_currentView[e.Index].IsPlaceHolder())
            {
                e.Graphics.DrawString(_currentView[e.Index].GenericName, e.Font, Brushes.Blue, e.Bounds);
            }
            else if (!_currentView[e.Index].Generate)
            {
                e.Graphics.DrawString(_currentView[e.Index].GenericName, e.Font, Brushes.Gray, e.Bounds);
            }
            else
            {
                using (var brush = new SolidBrush(e.ForeColor))
                {
                    e.Graphics.DrawString(_currentView[e.Index].GenericName, e.Font, brush, e.Bounds);
                }
            }

            e.DrawFocusRectangle();
        }

        private void ProjectPanel_Load(object sender, EventArgs e)
        {
            textboxPlusBtn.ButtonClicked += textboxPlusBtn_ButtonClicked;
            textboxPlusBtn.TextBox.TextChanged += TextBox_TextChanged;
        }

        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            OnTargetDirChanged(textboxPlusBtn.TextBox.Text);
        }

        #endregion

        #region Events

        internal event EventHandler SelectedItemsChanged;

        void OnSelectedItemsChanged()
        {
            if (SelectedItemsChanged != null)
                SelectedItemsChanged(this, EventArgs.Empty);
        }

        internal event EventHandler LastItemRemoved;

        void OnLastItemRemoved()
        {
            if (LastItemRemoved != null)
                LastItemRemoved(this, EventArgs.Empty);
        }

        #endregion

        #region Methods

        internal void ClearSelectedItems()
        {
            _selectedItems = new List<CslaObjectInfo>();
        }

        /// <summary>
        /// Applies the filters to the CslaObjectInfoCollection.
        /// </summary>
        /// <remarks>
        /// This method should handle only the filtering of business objects.
        /// The view part - handle what is displayed in the ListBox - should be here.
        /// </remarks>
        internal void ApplyFilters(bool updatePresenter)
        {
            if (_suspendFilter)
                return;

            lstObjects.SuspendLayout();
            _currentView = new List<CslaObjectInfo>();
            if (_objects != null)
                FilterObjectsForView();

            if (optName.Checked)
                _currentView.Sort((a, b) => a.ObjectName.CompareTo(b.ObjectName));
            else if (optType.Checked)
                _currentView.Sort((a, b) =>
                {
                    var type = a.ObjectType.ToString().CompareTo(b.ObjectType.ToString());
                    if (type == 0)
                        return a.ObjectName.CompareTo(b.ObjectName);
                    return type;
                });

            if (updatePresenter)
                ApplyFiltersPresenter();
        }

        private void FilterObjectsForView()
        {
            var allTypes = cboObjectType.Text == "<All>";
            var filterList = txtFilter.Text.Split(' ');

            foreach (var obj in _objects)
            {
                if (allTypes || obj.ObjectType.ToString() == cboObjectType.Text)
                {
                    if (txtFilter.Text.Length == 0 ||
                        obj.ObjectName.IndexOf(txtFilter.Text, StringComparison.CurrentCultureIgnoreCase) >= 0)
                        _currentView.Add(obj);
                    else
                    {
                        foreach (var item in filterList)
                        {
                            if (obj.ObjectName.IndexOf(item, StringComparison.CurrentCultureIgnoreCase) >= 0)
                                _currentView.Add(obj);
                        }
                    }
                }
            }
        }

        internal void ApplyFiltersPresenter()
        {
            if (_suspendFilter)
                return;

            // store currency values for later use by lstObjects_SelectedIndexChanged
            if (_selectedItems == null)
                _selectedItems = new List<CslaObjectInfo>();
            if (_objects != null)
            {
                foreach (CslaObjectInfo obj in lstObjects.SelectedItems)
                {
                    if (_objects.Contains(obj))
                        _selectedItems.Add(obj);
                }
            }

            _restoreSelectedItems = true;
            // stupid Windows.Forms doesn't fire the event when the DataSource has elements and is assigned an empty collection
            var fireEventManually = false;
            if (lstObjects.DataSource != null)
                fireEventManually = ((ICollection) lstObjects.DataSource).Count > 0 && _currentView.Count == 0;
            lstObjects.DataSource = _currentView;// this sets the list SelectedItem to the first item
            if (fireEventManually)
                ListObjects_SelectedIndexChanged(this, new EventArgs());
            _restoreSelectedItems = false;

            lstObjects.ResumeLayout();
            if (_objects != null)
            {
                if (_objects.Count == 0)
                {
                    OnLastItemRemoved();
                }
            }
        }

        private void OnTargetDirChanged(string newDir)
        {
            if (TargetDirChanged != null)
            {
                if (newDir == null)
                    newDir = string.Empty;
                TargetDirChanged(newDir);
            }
        }

        internal void RemoveSelected()
        {
            if (!UnitLoaded())
                return;

            // Suspend filter until it's done
            _suspendFilter = true;

            // Don't restore SelectedItems
            _restoreSelectedItems = false;

            // What to delete
            /*var deleteList = new List<CslaObjectInfo>();
            foreach (CslaObjectInfo obj in lstObjects.SelectedItems)
                deleteList.Add(obj);*/
            var deleteList = lstObjects.SelectedItems.Cast<CslaObjectInfo>().ToList();

            // Select the top most selected item
            var selectedIndex = lstObjects.SelectedIndex;// -1;

            // Now we don't need SelectedIndices any more (and they don't exist anyway)
            lstObjects.SelectedIndices.Clear();

            // Do delete
            foreach (CslaObjectInfo obj in deleteList)
                _objects.Remove(obj);

            // Now filter and get ListBox updated
            _suspendFilter = false;
            ApplyFilters(true);

            // List is empty
            if (lstObjects.Items.Count == 0)
                return;

            // Can'tselect past the first element
            if (selectedIndex == -1)
                selectedIndex = 0;

            _selectedItems = new List<CslaObjectInfo>();
            lstObjects.SelectedIndices.Clear();

            // Can't select past the last element
            if (selectedIndex > lstObjects.Items.Count - 1)
                selectedIndex = lstObjects.Items.Count - 1;

            lstObjects.SelectedIndex = selectedIndex;
            var selectdObject = (CslaObjectInfo) lstObjects.SelectedItem;
            _selectedItems.Add(selectdObject);

            // Now restore SelectedItems
            _restoreSelectedItems = true;
            ApplyFiltersPresenter();

            GeneratorController.Current.MainForm.FillObjects();
        }

        internal void DuplicateSelected()
        {
            if (!UnitLoaded())
                return;

            _suspendListUpdates = true;
            DisableEventDrawItem();

            /*var duplicateList = new List<CslaObjectInfo>();
            foreach (CslaObjectInfo obj in lstObjects.SelectedItems)
                duplicateList.Add(obj.Duplicate(GeneratorController.Catalog));*/
            var duplicateList =
                (from CslaObjectInfo obj in lstObjects.SelectedItems select obj.Duplicate(GeneratorController.Catalog))
                    .ToList();
            foreach (var obj in duplicateList)
                _objects.InsertAtTop(obj, true);

            _suspendListUpdates = false;
            EnableEventDrawItem();
            ApplyFilters(true);

            lstObjects.SelectedItems.Clear();
            foreach (var obj in duplicateList)
                lstObjects.SelectedItems.Add(obj);

            GeneratorController.Current.MainForm.FillObjects();
        }

        internal void SelectAll()
        {
            DisableEventSelectedIndexChanged();

            lstObjects.SelectedItems.Clear();

            for (int index = 0; index < lstObjects.Items.Count; index++)
            {
                var obj = lstObjects.Items[index];
                lstObjects.SelectedItems.Add(obj);
            }

            OnSelectedItemsChanged();

            EnableEventSelectedIndexChanged();
        }

        internal void AddCreatedObject(CslaObjectInfoCollection objects)
        {
            if (!UnitLoaded())
                return;

            _suspendListUpdates = true;
            DisableEventDrawItem();

            foreach (var obj in objects)
                _objects.InsertAtTop(obj);

            _suspendListUpdates = false;
            EnableEventDrawItem();
            ApplyFilters(true);

            lstObjects.SelectedItems.Clear();
            foreach (var obj in objects)
                lstObjects.SelectedItems.Add(obj);

            GeneratorController.Current.MainForm.FillObjects();
        }

        internal void AddNewObject()
        {
            if (!UnitLoaded())
                return;
            var newCslaObjectInfo = new CslaObjectInfo(GeneratorController.Current.CurrentUnit);
            _objects.InsertAtTop(newCslaObjectInfo, true);
            GeneratorController.Current.MainForm.FillObjects();
        }

        internal void MoveUpSelected()
        {
            if (!UnitLoaded())
                return;

            // Suspend filter until it's done
            _suspendListUpdates = true;
            DisableEventDrawItem();

            var hitTop = false;
            var upperIdx = 0;

            for (var selIdx = 0; selIdx < lstObjects.SelectedItems.Count; selIdx++)
            {
                var item = (CslaObjectInfo) lstObjects.SelectedItems[selIdx];
                var idx = _objects.IndexOf(item);

                if (idx == 0)
                    hitTop = true;

                if (!hitTop)
                {
                    if (idx > 0)
                    {
                        _objects.RemoveAt(idx);
                        _objects.Insert(idx - 1, item);
                    }
                }
                else
                {
                    if (idx - 1 > upperIdx)
                    {
                        _objects.RemoveAt(idx);
                        _objects.Insert(idx - 1, item);
                        upperIdx = idx - 1;
                    }
                    else
                        upperIdx = idx;
                }
            }

            // Now filter and get ListBox updated
            _suspendListUpdates = false;
            EnableEventDrawItem();
            ApplyFilters(true);
        }

        internal void MoveDownSelected()
        {
            if (!UnitLoaded())
                return;

            // Suspend filter until it's done
            _suspendListUpdates = true;
            DisableEventDrawItem();

            var hitBottom = false;
            var lowerIdx = _objects.Count - 1;

            for (var selIdx = lstObjects.SelectedItems.Count - 1; selIdx > -1; selIdx--)
            {
                var item = (CslaObjectInfo) lstObjects.SelectedItems[selIdx];
                var idx = _objects.IndexOf(item);

                if (idx == _objects.Count - 1)
                    hitBottom = true;

                if (!hitBottom)
                {
                    if (idx < _objects.Count - 1)
                    {
                        _objects.RemoveAt(idx);
                        _objects.Insert(idx + 1, item);
                    }
                }
                else
                {
                    if (idx + 1 < lowerIdx)
                    {
                        _objects.RemoveAt(idx);
                        _objects.Insert(idx + 1, item);
                        lowerIdx = idx + 1;
                    }
                    else
                        lowerIdx = idx;
                }
            }

            // Now filter and get ListBox updated
            _suspendListUpdates = false;
            EnableEventDrawItem();
            ApplyFilters(true);
        }

        internal void AddNewObjectRelation()
        {
            if (!UnitLoaded())
                return;

            switch (lstObjects.SelectedItems.Count)
            {
                case 1:
                    if (TopLevelControl != null)
                        ((MainForm) TopLevelControl).ObjectRelationsBuilderPanel.Add(
                            (CslaObjectInfo) lstObjects.SelectedItems[0]);
                    break;
                case 2:
                    if (TopLevelControl != null)
                        ((MainForm) TopLevelControl).ObjectRelationsBuilderPanel.Add(
                            (CslaObjectInfo) lstObjects.SelectedItems[0],
                            (CslaObjectInfo) lstObjects.SelectedItems[1]);
                    break;
            }

            if (TopLevelControl != null)
                ((MainForm) TopLevelControl).ObjectRelationsBuilderPanel.BringToFront();
        }

        internal void AddToObjectRelationBuilder()
        {
            MessageBox.Show("1) Select the Objetc Relation from a combo box\r\n" +
                            "2) Confirm or change the relation type from a combo box or radio buttons\r\n" +
                            "3) From a combo box, select the \"AS\" member according to 1) and 2):\r\n" +
                            "\t- Primary Entity\r\n" +
                            "\t- Secondary Entity\r\n" +
                            "\t- Primary collection\r\n" +
                            "\t- Primary item", "CTP - Not implemented.");
        }

        internal IEnumerable<CslaObjectInfo> GetSelectedObjects()
        {
            /*var lst = new List<CslaObjectInfo>();
            foreach (CslaObjectInfo itm in ListObjects.SelectedItems)
                lst.Add(itm);
            return lst.ToArray();*/
            return ListObjects.SelectedItems.Cast<CslaObjectInfo>().ToArray();
        }

        private void DisableEventSelectedIndexChanged()
        {
            lstObjects.SelectedIndexChanged -= ListObjects_SelectedIndexChanged;
        }

        private void EnableEventSelectedIndexChanged()
        {
            lstObjects.SelectedIndexChanged += ListObjects_SelectedIndexChanged;
        }

        private void DisableEventDrawItem()
        {
            lstObjects.DrawItem -= lstObjects_DrawItem;
        }

        private void EnableEventDrawItem()
        {
            lstObjects.DrawItem += lstObjects_DrawItem;
        }

        #endregion

        #region Manage state

        internal void GetState()
        {
            GeneratorController.Current.CurrentUnitLayout.ProjectListFilterText = txtFilter.Text;
            GeneratorController.Current.CurrentUnitLayout.ProjectListFilterType = cboObjectType.SelectedItem.ToString();
            if (optType.Checked)
                GeneratorController.Current.CurrentUnitLayout.ProjectListSortMode = optType.Text;
            else if (optName.Checked)
                GeneratorController.Current.CurrentUnitLayout.ProjectListSortMode = optName.Text;
            else
                GeneratorController.Current.CurrentUnitLayout.ProjectListSortMode = optNone.Text;

            GeneratorController.Current.CurrentUnitLayout.ProjectListSelectedObjects.Clear();
            foreach (var item in lstObjects.SelectedItems)
            {
                var cslaObject = item as CslaObjectInfo;
                if (cslaObject != null)
                    GeneratorController.Current.CurrentUnitLayout.ProjectListSelectedObjects.Add(cslaObject.ObjectName);
            }
        }

        internal void SetState()
        {
            txtFilter.Text = GeneratorController.Current.CurrentUnitLayout.ProjectListFilterText;
            foreach (var item in cboObjectType.Items)
            {
                if (item.ToString() == GeneratorController.Current.CurrentUnitLayout.ProjectListFilterType)
                {
                    cboObjectType.SelectedItem = item;
                    break;
                }
            }

            if (GeneratorController.Current.CurrentUnitLayout.ProjectListSortMode == optType.Text)
            {
                optType.Checked = true;
                optName.Checked = false;
                optNone.Checked = false;
            }
            else if (GeneratorController.Current.CurrentUnitLayout.ProjectListSortMode == optName.Text)
            {
                optName.Checked = true;
                optType.Checked = false;
                optNone.Checked = false;
            }
            else
            {
                optNone.Checked = true;
                optType.Checked = false;
                optName.Checked = false;
            }

            if (GeneratorController.Current.CurrentUnitLayout.ProjectListSelectedObjects.Count > 0)
                lstObjects.SelectedItems.Clear();
            foreach (var selectedObject in GeneratorController.Current.CurrentUnitLayout.ProjectListSelectedObjects)
            {
                for (var index = 0; index < lstObjects.Items.Count; index++)
                {
                    var item = lstObjects.Items[index];
                    var itemLine = item as CslaObjectInfo;
                    if (itemLine != null && itemLine.ObjectName == selectedObject)
                    {
                        lstObjects.SelectedItems.Add(item);
                    }
                }
            }
        }

        #endregion
    }
}