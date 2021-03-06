using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Serialization;
using CslaGenerator.Controls;
using CslaGenerator.Data;
using CslaGenerator.Metadata;
using CslaGenerator.Util;
using CslaGenerator.Util.PropertyBags;
using DBSchemaInfo.Base;

namespace CslaGenerator
{
    public class GeneratorController : IDisposable
    {
        #region Main (application entry point)

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// <param name="args">
        /// Command line arguments.  First arg can be a filename to load.
        /// </param>
        [STAThread]
        private static void Main(string[] args)
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en");
            var controller = new GeneratorController();
            controller.MainForm.Closing += controller.GeneratorFormClosing;
            controller.CommandLineArgs = args;
            // process the command line args here so we have a UI, also, we can not process in Init without
            // modifying more code to take args[]
            controller.ProcessCommandLineArgs();
            Application.Run();
        }

        #endregion

        #region Private Fields

        private string[] _commandlineArgs;
        private CslaGeneratorUnit _currentUnit;
        private AssociativeEntity _currentAssociativeEntitiy;
        private GlobalParameters _globalParameters = new GlobalParameters();
        private string _currentFilePath = string.Empty;
        private MainForm _mainForm;
        private static ICatalog _catalog;
        private static GeneratorController _current;
        private readonly PropertyContext _propertyContext = new PropertyContext();
        private bool _confirmReplacementRememberAnswer;
        private bool _confirmReplacementDialogResult;
        private bool _reportReplacementCounterRememberAnswer;

        internal bool IsDBConnected;
        internal bool IsLoading;
        internal bool HasErrors = false;
        internal bool HasWarnings = false;
        internal Stopwatch LoadingTimer = new Stopwatch();
        internal int Tables;
        internal int Views;
        internal int Sprocs;

        #endregion

        #region Constructors/Dispose

        internal GeneratorController()
        {
            _current = this;
            Init();
            GetConfig();
        }

        private void Init()
        {
            _mainForm = new MainForm(this);
            _mainForm.ProjectPanel.SelectedItemsChanged += CslaObjectList_SelectedItemsChanged;
            _mainForm.ProjectPanel.LastItemRemoved += delegate { CurrentCslaObject = null; };
            _mainForm.ObjectRelationsBuilderPanel.SelectedItemsChanged += AssociativeEntitiesList_SelectedItemsChanged;
            _mainForm.Show();
            _mainForm.formSizePosition.RestoreFormSizePosition();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // dispose managed resources
                if (_mainForm != null)
                {
                    _mainForm.Dispose();
                    _mainForm = null;
                }
            }
            // free native resources
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Processes command line args passed to CSLA Gen.  Called after the generator is created.
        /// </summary>
        private void ProcessCommandLineArgs()
        {
            if (CommandLineArgs.Length > 0)
            {
                string filename = CommandLineArgs[0];
                if (File.Exists(filename))
                {
                    // request that the UI load the project, since it keeps track
                    // of *additional* state (isNew) that this class is unaware of.
                    _mainForm.OpenProjectFile(filename);
                }
            }
        }

        #endregion

        #region internal Properties

        internal CslaGeneratorUnit CurrentUnit
        {
            get { return _currentUnit; }
            private set
            {
                _mainForm.WebBrowserDockPanel.Show();
                _mainForm.GlobalSettingsPanel.LoadInfo();
                _mainForm.ActivateShowGlobalSettings();
                _mainForm.ObjectRelationsBuilderPanel.Show(_mainForm.DockPanel);

                _currentUnit = value;

                if (_mainForm.ProjectPropertiesPanel == null)
                    _mainForm.ProjectPropertiesPanel = new ProjectProperties();
                _mainForm.ProjectPropertiesPanel.LoadInfo();
                _mainForm.ActivateShowProjectProperties();
            }
        }

        internal CslaObjectInfo CurrentCslaObject { get; set; }

        internal GlobalParameters GlobalParameters
        {
            get { return _globalParameters; }
            set
            {
                if (value != null)
                    _globalParameters = value;
            }
        }

        internal CslaGeneratorUnitLayout CurrentUnitLayout { get; set; }
        internal string TemplatesDirectory { get; set; }
        internal string ProjectsDirectory { get; set; }
        internal string ObjectsDirectory { get; set; }
        internal string RulesDirectory { get; set; }
        internal List<string> MruItems { get; set; }

        internal string[] CommandLineArgs
        {
            get { return _commandlineArgs; }
            set { _commandlineArgs = value; }
        }

        internal ProjectProperties CurrentProjectProperties
        {
            get { return _mainForm.ProjectPropertiesPanel; }
        }

        internal MainForm MainForm
        {
            get { return _mainForm; }
            set { _mainForm = value; }
        }

        internal string CurrentFilePath
        {
            get { return _currentFilePath; }
            set { _currentFilePath = value; }
        }

        internal static ICatalog Catalog
        {
            get { return _catalog; }
            set { _catalog = value; }
        }

        internal static GeneratorController Current
        {
            get { return _current; }
        }

        #endregion

        #region internal Methods

        internal void Connect()
        {
            DialogResult result;
            using (var frmConn = new ConnectionForm())
            {
                result = frmConn.ShowDialog();
            }
            if (result == DialogResult.OK)
            {
                Cursor.Current = Cursors.WaitCursor;

                BuildSchemaTree(ConnectionFactory.ConnectionString);
                Cursor.Current = Cursors.Default;
                IsDBConnected = true;
            }
        }

        internal void Load(string filePath)
        {
            bool forceSave = false;
            IsLoading = true;
            LoadingTimer.Restart();

            FileStream fs = null;
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                var deserialized = false;
                var converted = false;
                while (!deserialized)
                {
                    try
                    {
                        fs = File.Open(filePath, FileMode.Open);
                        var xml = new XmlSerializer(typeof(CslaGeneratorUnit));
                        CurrentUnit = (CslaGeneratorUnit) xml.Deserialize(fs);
                        if (VersionHelper.CurrentFileVersion != CurrentUnit.FileVersion)
                        {
                            if (fs != null)
                            {
                                fs.Close();
                                fs.Dispose();
                            }

                            forceSave = VersionHelper.SolveVersionNumberIssues(filePath, null);
                            converted = true;
                            NewCslaUnit();
                        }
                        else
                        {
                            deserialized = true;
                        }
                    }
                    catch (InvalidOperationException exception)
                    {
                        if (fs != null)
                        {
                            fs.Close();
                            fs.Dispose();
                        }

                        if (exception.InnerException == null || converted)
                            throw;

                        forceSave = VersionHelper.SolveVersionNumberIssues(filePath, exception);
                        if (FixXmlSchemaErrors(filePath, exception))
                        {
                            converted = true;
                            NewCslaUnit();
                        }
                        else
                        {
                            throw;
                        }
                    }
                }

                if (fs != null)
                {
                    fs.Close();
                    fs.Dispose();
                }

                if (forceSave)
                    Save(filePath);

                CurrentUnit.FileVersion = VersionHelper.CurrentFileVersion;

                _currentUnit.ResetParent();
                CurrentCslaObject = null;
                _currentAssociativeEntitiy = null;
                _currentFilePath = GetFilePath(filePath);

                ConnectionFactory.ConnectionString = _currentUnit.ConnectionString;

                // check if this is a valid connection, else let the user enter new connection info
                SqlConnection cn = null;
                try
                {
                    cn = ConnectionFactory.NewConnection;
                    cn.Open();
                    BuildSchemaTree(_currentUnit.ConnectionString);
                    IsDBConnected = true;
                }
                catch //(SqlException e)
                {
                    // call connect function which will allow user to enter new info
                    Connect();
                }
                finally
                {
                    if (cn != null && cn.State == ConnectionState.Open)
                    {
                        cn.Close();
                    }
                    if (cn != null)
                    {
                        cn.Dispose();
                    }
                }

                BindControls();
                _currentUnit.CslaObjects.ListChanged += CslaObjects_ListChanged;
                if (_currentUnit.CslaObjects.Count > 0)
                {
                    if (_mainForm.ProjectPanel.ListObjects.Items.Count > 0)
                    {
                        CurrentCslaObject = (CslaObjectInfo) _mainForm.ProjectPanel.ListObjects.Items[0];
                    }
                    else
                    {
                        CurrentCslaObject = null;
                    }

                    if (_mainForm.DbSchemaPanel != null)
                        _mainForm.DbSchemaPanel.CurrentCslaObject = CurrentCslaObject;
                }
                else
                {
                    CurrentCslaObject = null;
                    if (_mainForm.DbSchemaPanel != null)
                        _mainForm.DbSchemaPanel.CurrentCslaObject = null;
                }
                _mainForm.ProjectPanel.ApplyFiltersPresenter();

                _currentUnit.AssociativeEntities.ListChanged += AssociativeEntities_ListChanged;
                /*if (_currentUnit.AssociativeEntities.Count > 0)
                {
                    _currentAssociativeEntitiy = _currentUnit.AssociativeEntities[0];
                }
                else
                {
                    _currentAssociativeEntitiy = null;
                    _frmGenerator.ObjectRelationsBuilder.PropertyGrid1.SelectedObject = null;
                    _frmGenerator.ObjectRelationsBuilder.PropertyGrid2.SelectedObject = null;
                    _frmGenerator.ObjectRelationsBuilder.PropertyGrid3.SelectedObject = null;
                }*/
            }
            catch (Exception e)
            {
                var message = filePath + Environment.NewLine + Environment.NewLine + e.Message;
                if (e.InnerException != null)
                    message += Environment.NewLine + e.InnerException.Message;

                MessageBox.Show(_mainForm, message, "Loading File Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
                if (fs != null)
                {
                    fs.Close();
                    fs.Dispose();
                }
            }

            LoadProjectLayout(filePath);
            CurrentCslaObject = (CslaObjectInfo) GetSelectedItem();

            IsLoading = false;
            LoadingTimer.Stop();
        }

        private bool FixXmlSchemaErrors(string filePath, Exception ex)
        {
            //ex.Message
            //"There is an error in XML document (114, 8)."
            //ex.InnerException.Message
            //"Instance validation error: 'Separator' is not a valid value for CslaObjectType."

            var point = ex.Message
                .Split(new[] {'(', ')'}, StringSplitOptions.RemoveEmptyEntries)[1]
                .Split(',');

            var line = Convert.ToInt32(point[0]);
            var position = Convert.ToInt32(point[1]);

            var originalItem = ex.InnerException.Message
                .Split(new[] {'\''}, StringSplitOptions.RemoveEmptyEntries)[1];

            var ofendedType = ex.InnerException.Message.
                Split(new[] {@"for"}, StringSplitOptions.RemoveEmptyEntries)[1]
                .Trim()
                .Trim('.');

            if (VersionHelper.FindEnum(filePath, originalItem) == 0)
                return true;

            using (var fixXmlSchema = new FixXmlSchema())
            {
                fixXmlSchema.FilePath = filePath;
                fixXmlSchema.OfendingLine = line;
                fixXmlSchema.OfendingPosition = position;
                fixXmlSchema.OfendedType = ofendedType;
                fixXmlSchema.OriginalItem = originalItem;

                var dialogResult = fixXmlSchema.ShowDialog();
                if (dialogResult == DialogResult.OK)
                {
                    var replacementItem = fixXmlSchema.ReplacementItem;

                    if (ConfirmReplacement(ofendedType, originalItem, replacementItem))
                    {
                        var counter = VersionHelper.ReplaceEnum(filePath, originalItem, replacementItem);
                        ReportReplacement(ofendedType, originalItem, replacementItem, counter);

                        return true;
                    }
                }
            }

            return false;
        }

        private bool ConfirmReplacement(string ofendedType, string originalItem, string replacementItem)
        {
            if (!_confirmReplacementRememberAnswer)
            {
                var msg = string.Format(@"Do you want to replace all occurences of '{0}' by '{1}' of '{2}' type?",
                    originalItem, replacementItem, ofendedType);
                using (var messageBox = new MessageBoxEx(msg, @"Fixing project file", MessageBoxIcon.Question))
                {
                    messageBox.SetButtons(new[] {DialogResult.No, DialogResult.Yes}, 2);
                    messageBox.SetCheckbox(@"Do not ask again if ""Yes"".");
                    messageBox.ShowDialog();

                    _confirmReplacementDialogResult = messageBox.DialogResult == DialogResult.Yes;
                    _confirmReplacementRememberAnswer = messageBox.CheckboxChecked && _confirmReplacementDialogResult;
                }
            }

            return _confirmReplacementDialogResult;
        }

        private void ReportReplacement(string ofendedType, string originalItem, string replacementItem, int counter)
        {
            if (!_reportReplacementCounterRememberAnswer)
            {
                var msg = string.Format(@"Replaced {0} occurences of '{1}' by '{2}' of '{3}' type.", counter,
                    originalItem, replacementItem, ofendedType);
                using (var messageBox = new MessageBoxEx(msg, @"Fixing project file", MessageBoxIcon.Information))
                {
                    messageBox.SetButtons(new[] {DialogResult.OK});
                    messageBox.SetCheckbox("Do not show again.");
                    messageBox.ShowDialog();

                    _reportReplacementCounterRememberAnswer = messageBox.CheckboxChecked;
                }
            }
        }

        internal void LoadProjectLayout(string filePath)
        {
            filePath = ExtractPathWithoutExtension(filePath) + ".Layout";
            if (File.Exists(filePath))
            {
                using (var fs = File.Open(filePath, FileMode.Open))
                {
                    var xml = new XmlSerializer(typeof(CslaGeneratorUnitLayout));
                    CurrentUnitLayout = (CslaGeneratorUnitLayout) xml.Deserialize(fs);
                }

                MainForm.SetProjectState();
            }
        }

        private void CslaObjects_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (e.ListChangedType == ListChangedType.ItemChanged)
            {
                if (e.PropertyDescriptor.Name == "ObjectType" && _mainForm.ProjectPanel.FilterTypeIsActive)
                    ReloadPropertyGrid();
            }
        }

        private void AssociativeEntities_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (e.ListChangedType == ListChangedType.ItemChanged)
            {
                if (e.PropertyDescriptor.Name == "ObjectName" || e.PropertyDescriptor.Name == "RelationType")
                    ReloadBuilderPropertyGrid();
            }
        }

        internal void NewCslaUnit()
        {
            IsLoading = true;
            CurrentUnit = new CslaGeneratorUnit();
            CurrentUnitLayout = new CslaGeneratorUnitLayout();
            _currentFilePath = Path.GetTempPath() + @"\" + Guid.NewGuid();
            CurrentCslaObject = null;
            _currentUnit.ConnectionString = ConnectionFactory.ConnectionString;
            BindControls();
            _mainForm.ObjectInfoGrid.SelectedObject = null;
            IsLoading = false;
        }

        internal void Save(string filePath)
        {
            if (!_mainForm.ApplyProjectProperties())
                return;

            CurrentUnit.FileVersion = VersionHelper.CurrentFileVersion;

            _mainForm.GlobalSettingsPanel.ForceSaveGlobalSettings();

            FileStream fs = null;
            var tempFile = Path.GetTempPath() + Guid.NewGuid() + ".cslagenerator";
            var success = false;
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                fs = File.Open(tempFile, FileMode.Create);
                var s = new XmlSerializer(typeof(CslaGeneratorUnit));
                s.Serialize(fs, _currentUnit);
                success = true;
            }
            catch (Exception e)
            {
                MessageBox.Show(_mainForm, @"An error occurred while trying to save: " + Environment.NewLine + e.Message,
                    "Save Error");
            }
            finally
            {
                Cursor.Current = Cursors.Default;
                if (fs != null)
                {
                    fs.Close();
                    fs.Dispose();
                }
            }

            if (success)
            {
                File.Delete(filePath);
                File.Move(tempFile, filePath);
                _currentFilePath = GetFilePath(filePath);
            }
            SaveProjectLayout(filePath);
        }

        internal void SaveProjectLayout(string filePath)
        {
            _mainForm.GetProjectState();
            filePath = ExtractPathWithoutExtension(filePath) + ".Layout";
            FileStream fs = null;
            var tempFile = Path.GetTempPath() + Guid.NewGuid().ToString() + ".cslagenerator";
            var success = false;
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                fs = File.Open(tempFile, FileMode.Create);
                var s = new XmlSerializer(typeof(CslaGeneratorUnitLayout));
                s.Serialize(fs, CurrentUnitLayout);
                success = true;
            }
            catch (Exception e)
            {
                MessageBox.Show(_mainForm, @"An error occurred while trying to save: " + Environment.NewLine + e.Message,
                    "Save Error");
            }
            finally
            {
                Cursor.Current = Cursors.Default;
                if (fs != null)
                {
                    fs.Close();
                    fs.Dispose();
                }
            }
            if (success)
            {
                File.Delete(filePath);
                File.Move(tempFile, filePath);
                File.SetAttributes(filePath, FileAttributes.Hidden);
            }
        }

        internal static string ExtractFilename(string filePath)
        {
            var slash = filePath.LastIndexOf('\\');
            if (slash > 0)
                return filePath.Substring(slash + 1);

            return filePath;
        }

        internal static string ExtractPathWithoutExtension(string filePath)
        {
            var dot = filePath.LastIndexOf('.');
            if (dot > 0)
                return filePath.Substring(0, dot);

            return filePath;
        }

        #endregion

        #region private Methods

        private string GetFilePath(string fileName)
        {
            var fi = new FileInfo(fileName);
            return fi.Directory.FullName;
        }

        private void BindControls()
        {
            if (_currentUnit != null)
            {
                _mainForm.ProjectNameTextBox.Enabled = true;
                _mainForm.ProjectNameTextBox.DataBindings.Clear();
                _mainForm.ProjectNameTextBox.DataBindings.Add("Text", _currentUnit, "ProjectName");

                _mainForm.TargetDirectoryButton.Enabled = true;
                _mainForm.TargetDirectoryTextBox.Enabled = true;
                _mainForm.TargetDirectoryTextBox.DataBindings.Clear();
                _mainForm.TargetDirectoryTextBox.DataBindings.Add("Text", _currentUnit, "TargetDirectory");

                BindCslaList();
                BindRelationsList();
            }
        }

        private void BindCslaList()
        {
            if (_currentUnit != null)
            {
                _mainForm.ProjectPanel.Objects = _currentUnit.CslaObjects;
                _mainForm.ProjectPanel.ApplyFilters(true);
                _mainForm.ProjectPanel.ListObjects.ClearSelected();
                if (_mainForm.ProjectPanel.ListObjects.Items.Count > 0)
                    _mainForm.ProjectPanel.ListObjects.SelectedIndex = 0;

                // make sure the previous stored selection is cleared
                _mainForm.ProjectPanel.ClearSelectedItems();
            }
        }

        private void BindRelationsList()
        {
            if (_currentUnit != null)
            {
                _mainForm.ObjectRelationsBuilderPanel.AssociativeEntities = _currentUnit.AssociativeEntities;
                _mainForm.ObjectRelationsBuilderPanel.FillViews(true);
                _mainForm.ObjectRelationsBuilderPanel.GetCurrentListBox().ClearSelected();
                if (_mainForm.ObjectRelationsBuilderPanel.GetCurrentListBox().Items.Count > 0)
                    _mainForm.ObjectRelationsBuilderPanel.GetCurrentListBox().SelectedIndex = 0;

                // make sure the previous stored selection is cleared
                _mainForm.ObjectRelationsBuilderPanel.ClearSelectedItems();
            }
        }

        private void BuildSchemaTree(string connectionString)
        {
            try
            {
                if (_mainForm.DbSchemaPanel != null && _mainForm.DbSchemaPanel.Visible)
                    _mainForm.DbSchemaPanel.Hide();

                if (_mainForm.DbSchemaPanel != null)
                {
                    _mainForm.DbSchemaPanel.Close();
                    _mainForm.DbSchemaPanel.Dispose();
                }
                _mainForm.DbSchemaPanel = new DbSchemaPanel(_currentUnit, connectionString);
                _mainForm.DbSchemaPanel.BuildSchemaTree();
                _mainForm.ActivateShowSchema();
                _mainForm.DbSchemaPanel.SetDbColumnsPctHeight(73);
                _mainForm.DbSchemaPanel.SetDbTreeViewPctHeight(73);
            }
            catch (Exception e)
            {
                throw (e);
            }
        }

        /*private string GetFileNameWithoutExtension(string fileName)
        {
            var index = fileName.LastIndexOf(".");
            if (index >= 0)
            {
                return fileName.Substring(0, index);
            }
            return fileName;
        }

        private string GetFileExtension(string fileName)
        {
            var index = fileName.LastIndexOf(".");
            if (index >= 0)
            {
                return fileName.Substring(index + 1);
            }
            return ".cs";
        }

        private string GetTemplateName(CslaObjectInfo info)
        {
            switch (info.ObjectType)
            {
                case CslaObjectType.EditableRoot:
                    return ConfigurationManager.AppSettings["EditableRootTemplate"];
                case CslaObjectType.EditableChild:
                    return ConfigurationManager.AppSettings["EditableChildTemplate"];
                case CslaObjectType.EditableRootCollection:
                    return ConfigurationManager.AppSettings["EditableRootCollectionTemplate"];
                case CslaObjectType.EditableChildCollection:
                    return ConfigurationManager.AppSettings["EditableChildCollectionTemplate"];
                case CslaObjectType.EditableSwitchable:
                    return ConfigurationManager.AppSettings["EditableSwitchableTemplate"];
                case CslaObjectType.DynamicEditableRoot:
                    return ConfigurationManager.AppSettings["DynamicEditableRootTemplate"];
                case CslaObjectType.DynamicEditableRootCollection:
                    return ConfigurationManager.AppSettings["DynamicEditableRootCollectionTemplate"];
                case CslaObjectType.ReadOnlyObject:
                    return ConfigurationManager.AppSettings["ReadOnlyObjectTemplate"];
                case CslaObjectType.ReadOnlyCollection:
                    return ConfigurationManager.AppSettings["ReadOnlyCollectionTemplate"];
                default:
                    return String.Empty;
            }
        }*/

        #endregion

        #region Event Handlers

        private void GeneratorFormClosing(object sender, CancelEventArgs e)
        {
            Application.Exit();
        }

        private void CslaObjectList_SelectedItemsChanged(object sender, EventArgs e)
        {
            // fired on "ObjectName" changed for the following scenario:
            //     Suppose we have a filter on the name,
            //     and we change the object name in such way that
            //     the object isn't visible any longer,
            //     and it must not be shown on the PropertyGrid
            //     THEN we need to reload the PropertyGrid
            ReloadPropertyGrid();
        }

        private void AssociativeEntitiesList_SelectedItemsChanged(object sender, EventArgs e)
        {
            ReloadBuilderPropertyGrid();
        }

        internal void ReloadPropertyGrid()
        {
            if (_mainForm.DbSchemaPanel != null)
                _mainForm.DbSchemaPanel.CurrentCslaObject = null;

            var selectedItems = new List<CslaObjectInfo>();
            foreach (CslaObjectInfo obj in _mainForm.ProjectPanel.ListObjects.SelectedItems)
            {
                selectedItems.Add(obj);
                if (!IsLoading && _mainForm.DbSchemaPanel != null)
                {
                    CurrentCslaObject = obj;
                    _mainForm.DbSchemaPanel.CurrentCslaObject = obj;
                }
            }

            if (_mainForm.DbSchemaPanel != null && selectedItems.Count != 1)
            {
                CurrentCslaObject = null;
                _mainForm.DbSchemaPanel.CurrentCslaObject = null;
            }

            if (selectedItems.Count == 0)
                _mainForm.ObjectInfoGrid.SelectedObject = null;
            else
                _mainForm.ObjectInfoGrid.SelectedObject = new PropertyBag(selectedItems.ToArray(), _propertyContext);
        }

        void ReloadBuilderPropertyGrid()
        {
            var selectedItems = new List<AssociativeEntity>();
            var listBoxSelectedItems = _mainForm.ObjectRelationsBuilderPanel.GetCurrentListBox().SelectedItems;

            foreach (AssociativeEntity obj in listBoxSelectedItems)
            {
                selectedItems.Add(obj);
                if (!IsLoading)
                {
                    _currentAssociativeEntitiy = obj;
                }
            }

            if (selectedItems.Count == 0)
            {
                _currentAssociativeEntitiy = null;
            }
            else
            {
                if (_currentAssociativeEntitiy == null)
                    _currentAssociativeEntitiy = selectedItems[0];
            }
            _mainForm.ObjectRelationsBuilderPanel.SetAllPropertyGridSelectedObject(_currentAssociativeEntitiy);
        }

        #endregion

        private void GetConfig()
        {
            GetConfigTemplatesFolder();
            GetConfigProjectsFolder();
            GetConfigObjectsFolder();
            GetConfigRulesFolder();
        }

        private void GetConfigTemplatesFolder()
        {
            var tDir = ConfigTools.SharedAppConfigGet("TemplatesDirectory");
            if (string.IsNullOrEmpty(tDir))
            {
                tDir = ConfigTools.AppConfigGet("TemplatesDirectory");

                while (tDir.LastIndexOf(@"\\") == tDir.Length - 2)
                {
                    tDir = tDir.Substring(0, tDir.Length - 1);
                }
            }

            if (string.IsNullOrEmpty(tDir))
            {
                TemplatesDirectory = Environment.SpecialFolder.Desktop.ToString();
            }
            else
            {
                TemplatesDirectory = tDir;
            }
        }

        private void GetConfigProjectsFolder()
        {
            var tDir = ConfigTools.SharedAppConfigGet("ProjectsDirectory");
            if (string.IsNullOrEmpty(tDir))
            {
                tDir = ConfigTools.AppConfigGet("ProjectsDirectory");

                while (tDir.LastIndexOf(@"\\") == tDir.Length - 2)
                {
                    tDir = tDir.Substring(0, tDir.Length - 1);
                }
            }

            if (string.IsNullOrEmpty(tDir))
            {
                ProjectsDirectory = Environment.SpecialFolder.Desktop.ToString();
            }
            else
            {
                ProjectsDirectory = tDir;
            }
        }

        internal void GetConfigObjectsFolder()
        {
            var tDir = ConfigTools.SharedAppConfigGet("ObjectsDirectory");
            if (string.IsNullOrEmpty(tDir))
            {
                tDir = ConfigTools.AppConfigGet("ObjectsDirectory");

                while (tDir.LastIndexOf(@"\\") == tDir.Length - 2)
                {
                    tDir = tDir.Substring(0, tDir.Length - 1);
                }
            }

            if (string.IsNullOrEmpty(tDir))
            {
                ObjectsDirectory = Environment.SpecialFolder.Desktop.ToString();
            }
            else
            {
                ObjectsDirectory = tDir;
            }
        }

        private void GetConfigRulesFolder()
        {
            var tDir = ConfigTools.SharedAppConfigGet("RulesDirectory");
            if (string.IsNullOrEmpty(tDir))
            {
                tDir = ConfigTools.AppConfigGet("RulesDirectory");

                while (tDir.LastIndexOf(@"\\") == tDir.Length - 2)
                {
                    tDir = tDir.Substring(0, tDir.Length - 1);
                }
            }

            if (string.IsNullOrEmpty(tDir))
            {
                RulesDirectory = Environment.SpecialFolder.Desktop.ToString();
            }
            else
            {
                RulesDirectory = tDir;
            }
        }

        internal void ReSortMruItems()
        {
            var original = MruItems.ToArray();
            MruItems.Clear();
            foreach (var item in original)
            {
                if (!MruItems.Contains(item))
                    MruItems.Add(item);
            }

            ConfigTools.SharedAppConfigChangeMru(MruItems);
        }

        internal object GetSelectedItem()
        {
            object selectedItem;
            if (Current.MainForm.ProjectPanel.ListObjects.InvokeRequired)
            {
                selectedItem = Current.MainForm.ProjectPanel.ListObjects.Invoke((Action) delegate { GetSelectedItem(); });
                return selectedItem;
            }
            selectedItem = Current.MainForm.ProjectPanel.ListObjects.SelectedItem;

            return selectedItem;
        }

        #region Nested CslaObjectInfoComparer

        private class CslaObjectInfoComparer : IComparer<CslaObjectInfo>
        {
            #region IComparer<CslaObjectInfo> Members

            public int Compare(CslaObjectInfo x, CslaObjectInfo y)
            {
                return x.ObjectName.CompareTo(y.ObjectName);
            }

            #endregion
        }

        #endregion
    }
}