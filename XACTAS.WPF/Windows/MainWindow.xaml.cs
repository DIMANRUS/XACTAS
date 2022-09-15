using Project = Models.Project;

namespace XACTAS.WPF.Windows;

public partial class MainWindow
{
    #region Initialization
    public MainWindow()
    {
        InitializeComponent();
    }
    #endregion

    #region Private variables
    private readonly OpenFileDialog _openFileDialog = new();
    private BindingList<Project> _projects;
    private FileSystemWatcher _watcherVisualStudioProject;
    private FileSystemWatcher _watcherAndroidStudioProject;
    private Project _currentProject;
    #endregion

    #region Watchers Visual Studio Project
    private void WathcherVisualStudioProject_Changed(object sender, FileSystemEventArgs e)
    {
        _watcherAndroidStudioProject.EnableRaisingEvents = false;
        if (e.FullPath.Last() != '~')
        {
            try
            {
                File.Delete(@$"C:\Users\{Environment.UserName}\Documents\DIMANRUS\XACTAS\ASProject\app\src\main\res\{e.Name}");
                File.Copy(e.FullPath, @$"C:\Users\{Environment.UserName}\Documents\DIMANRUS\XACTAS\ASProject\app\src\main\res\{e.Name}");
            }
            catch
            {
                // ignored
            }
        }
        _watcherAndroidStudioProject.EnableRaisingEvents = true;
    }

    private void WathcherVisualStudioProject_Renamed(object sender, RenamedEventArgs e)
    {
        if (e.FullPath.Last() != '~')
        {
            try
            {
                File.Delete(@$"C:\Users\{Environment.UserName}\Documents\DIMANRUS\XACTAS\ASProject\app\src\main\res\{e.OldName}");
                File.Copy(e.FullPath, @$"C:\Users\{Environment.UserName}\Documents\DIMANRUS\XACTAS\ASProject\app\src\main\res\{e.Name}");
            }
            catch
            {
                // ignored
            }
        }
    }

    private void WathcherVisualStudioProject_Deleted(object sender, FileSystemEventArgs e)
    {
        if (e.FullPath.Last() != '~')
        {
            try
            {
                File.Delete(@$"C:\Users\{Environment.UserName}\Documents\DIMANRUS\XACTAS\ASProject\app\src\main\res\{e.Name}");
            }
            catch
            {
                // ignored
            }
        }
    }

    private void WathcherVisualStudioProject_Created(object sender, FileSystemEventArgs e)
    {
        if (e.FullPath.Last() != '~')
        {
            try
            {
                File.Copy(e.FullPath, @$"C:\Users\{Environment.UserName}\Documents\DIMANRUS\XACTAS\ASProject\app\src\main\res\{e.Name}");
            }
            catch (UnauthorizedAccessException)
            {
                Directory.CreateDirectory(@$"C:\Users\{Environment.UserName}\Documents\DIMANRUS\XACTAS\ASProject\app\src\main\res\{e.Name}");
            }
            catch
            {
                // ignored
            }
        }
    }
    #endregion

    #region Watcher Android Studio Project
    private void WathcherAndroidStudioProject_Changed(object sender, FileSystemEventArgs e)
    {
        _watcherVisualStudioProject.EnableRaisingEvents = false;
        //try
        //{
        if (e.FullPath[^1] != '~' && e.FullPath.Split(@"\")[^1].Split(".").Length == 2)
        {
            File.Delete(_currentProject.VsProjectPath + @$"\Resources\{e.Name}");
            File.Copy(e.FullPath, _currentProject.VsProjectPath + @$"\Resources\{e.Name}");
        }
        //}
        //catch
        //{
        //    // ignored
        //}
        _watcherVisualStudioProject.EnableRaisingEvents = true;
    }
    #endregion

    #region  Events Realizations
    private async void Page_Loaded(object sender, RoutedEventArgs e)
    {
        await using (var fileStream = File.Open(@$"C:\Users\{Environment.UserName}\Documents\DIMANRUS\XACTAS\data.json", FileMode.Open))
        {
            _projects = await JsonSerializer.DeserializeAsync<BindingList<Project>>(fileStream);
        }
        ProjectsControl.ItemsSource = _projects;
    }

    private async void AddProject_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        _openFileDialog.Filter = ".NET Project | *.csproj;*.fsproj";
        _openFileDialog.FileName = "";
        _openFileDialog.ShowDialog();
        string rootFolderProject = Path.GetDirectoryName(_openFileDialog.FileName);
        if (_projects.FirstOrDefault(x => x.VsProjectPath == rootFolderProject) == null)
        {
            if (File.Exists(rootFolderProject + @"\Properties\AndroidManifest.xml"))
            {
                var array = _openFileDialog.FileName.Split(@"\");
                array[^1] = "";
                await AddProject(Path.Combine(array));
            }
            else
            {
                ShowErrorMessageBox(Properties.Resources.SelectProject);
            }
        }
        else
        {
            ShowErrorMessageBox(Properties.Resources.ProjectAlreadyAdded);
        }
    }

    private void OpenFolder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        Process.Start("explorer.exe", ((FrameworkElement)sender).Tag.ToString()!);
    }
    private async void Launch_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (StopBlock.Visibility != Visibility.Collapsed || !await LaunchWatcher((Image)sender))
            return;
    }

    private async void LaunchVS_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        try
        {
            if (StopBlock.Visibility != Visibility.Collapsed)
                return;
            if (await LaunchWatcher((FrameworkElement)sender))
            {
                Process.Start(Settings.Default.VisualStudioPath, _currentProject.VsProjectPathSln);
            }
            else
            {
                ShowErrorMessageBox(Properties.Resources.ProjectWillRemovedOrMoved);
            }
        }
        catch
        {
            StopWatcher();
            ShowErrorMessageBox(Properties.Resources.ErrorLaunchVs);
        }
    }

    private async void LaunchAS_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        try
        {
            if (StopBlock.Visibility != Visibility.Collapsed)
                return;
            if (await LaunchWatcher((FrameworkElement)sender))
            {
                Process.Start(Settings.Default.AndroidStudioPath, @$"C:\Users\{Environment.UserName}\Documents\DIMANRUS\XACTAS\ASProject");
            }
            else
            {
                ShowErrorMessageBox(Properties.Resources.ProjectWillRemovedOrMoved);
            }
        }
        catch
        {
            StopWatcher();
            ShowErrorMessageBox(Properties.Resources.ErrorLaunchAs);
        }
    }

    private async void RemoveProject_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (StopBlock.Visibility == Visibility.Collapsed)
        {
            await RemoveProject(int.Parse(((Image)sender).Tag.ToString()!));
        }
    }

    private void SelectASPath_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        _openFileDialog.Filter = "Android Studio (*.exe) | *.exe";
        _openFileDialog.FileName = "";
        _openFileDialog.ShowDialog();
        if (_openFileDialog.FileName == "")
            return;
        Settings.Default.AndroidStudioPath = _openFileDialog.FileName;
        Settings.Default.Save();
    }

    private void SelectVSPath_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        _openFileDialog.Filter = "Visual Studio (*.exe) | *.exe";
        _openFileDialog.FileName = "";
        _openFileDialog.ShowDialog();
        if (_openFileDialog.FileName == "")
            return;
        Settings.Default.VisualStudioPath = _openFileDialog.FileName;
        Settings.Default.Save();
    }

    private void Help_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        ShowInformationMessageBox(Properties.Resources.Info);
    }

    private void StopWatcher_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        StopWatcher();
    }
    #endregion

    #region Private Methods
    private void StopWatcher()
    {
        _watcherAndroidStudioProject.Dispose();
        _watcherVisualStudioProject.Dispose();
        StopBlock.Visibility = Visibility.Collapsed;
        OptionBlock.Visibility = Visibility.Visible;
        AddProjectBlock.Visibility = Visibility.Visible;
    }

    private void CreateWatcher()
    {
        _watcherVisualStudioProject = new FileSystemWatcher(_currentProject.VsProjectPath + @"\Resources");
        _watcherVisualStudioProject.Created += WathcherVisualStudioProject_Created;
        _watcherVisualStudioProject.Deleted += WathcherVisualStudioProject_Deleted;
        _watcherVisualStudioProject.Renamed += WathcherVisualStudioProject_Renamed;
        _watcherVisualStudioProject.Changed += WathcherVisualStudioProject_Changed;
        _watcherVisualStudioProject.IncludeSubdirectories = true;
        _watcherAndroidStudioProject = new FileSystemWatcher(@$"C:\Users\{Environment.UserName}\Documents\DIMANRUS\XACTAS\ASProject\app\src\main\res");
        _watcherAndroidStudioProject.Changed += WathcherAndroidStudioProject_Changed;
        _watcherAndroidStudioProject.IncludeSubdirectories = true;
        _watcherAndroidStudioProject.EnableRaisingEvents = true;
        _watcherVisualStudioProject.EnableRaisingEvents = true;
    }

    private async Task<bool> LaunchWatcher(FrameworkElement img)
    {
        _currentProject = _projects.FirstOrDefault(x => x.ProjectId == int.Parse(img.Tag.ToString()!));
        if (Directory.Exists(_currentProject?.VsProjectPath))
        {
            DeleteAllFilesFromAndroidStudioProject();
            CreateWatcher();
            var directoryInfo = new DirectoryInfo(_currentProject.VsProjectPath + @"\Resources\layout");
            foreach (var file in directoryInfo.GetFiles())
            {
                File.Copy(file.FullName, @$"C:\Users\{Environment.UserName}\Documents\DIMANRUS\XACTAS\ASProject\app\src\main\res\layout\{file.Name}");
            }
            LaunchingProjectName.Text = Path.GetFileNameWithoutExtension(_currentProject.VsProjectPathSln);
            StopBlock.Visibility = Visibility.Visible;
            OptionBlock.Visibility = Visibility.Collapsed;
            AddProjectBlock.Visibility = Visibility.Collapsed;
            return true;
        }
        await RemoveProject(int.Parse(img.Tag.ToString()!));
        return false;
    }

    private async Task AddProject(string vsPath)
    {
        Random random = new();
        Project newProject = new()
        {
            ProjectId = random.Next(1, 10000),
            VsProjectPathSln = _openFileDialog.FileName,
            VsProjectPath = vsPath,
            Name = Path.GetFileNameWithoutExtension(_openFileDialog.FileName)
        };
        await using (var write = File.Open(@$"C:\Users\{Environment.UserName}\Documents\DIMANRUS\XACTAS\data.json", FileMode.Open))
        {
            _projects?.Add(newProject);
            await JsonSerializer.SerializeAsync(write, _projects);
        }
    }

    private async Task RemoveProject(int id)
    {
        Project project = _projects.FirstOrDefault(x => x.ProjectId == id);
        _projects.Remove(project);
        await using (StreamWriter write = new(@$"C:\Users\{Environment.UserName}\Documents\DIMANRUS\XACTAS\data.json"))
        {
            await write.WriteAsync("");
            await JsonSerializer.SerializeAsync(write.BaseStream, _projects);
        }
        ProjectsControl.ItemsSource = null;
        ProjectsControl.ItemsSource = _projects;
    }

    private static void DeleteAllFilesFromAndroidStudioProject()
    {
        DirectoryInfo directoryInfo = new(@$"C:\Users\{Environment.UserName}\Documents\DIMANRUS\XACTAS\ASProject\app\src\main\res\layout");
        foreach (var file in directoryInfo.GetFiles())
            File.Delete(file.FullName);
    }

    #region Message Boxes
    private void ShowErrorMessageBox(string text)
    {
        MessageBox.Show(text, Properties.Resources.Error, MessageBoxButton.OK, MessageBoxImage.Error);
    }

    private void ShowInformationMessageBox(string text)
    {
        MessageBox.Show(text, Properties.Resources.Information, MessageBoxButton.OK, MessageBoxImage.Information);
    }
    #endregion

    #endregion
}