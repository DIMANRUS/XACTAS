using Project = Models.Project;

namespace XACTAS.WPF.Windows;

public partial class MainWindow
{
    #region Constructors
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
    private const string _fileExtensionPattern = "\\.[0-9a-z]+$";
    #endregion

    #region Watchers Visual Studio Project
    private void WathcherVisualStudioProject_Changed(object sender, FileSystemEventArgs e)
    {
        _watcherAndroidStudioProject.EnableRaisingEvents = false;
        if (e.FullPath.Last() != '~')
        {
            try
            {
                if (Regex.Match(e.Name, _fileExtensionPattern).Value == string.Empty)
                {
                    Directory.CreateDirectory(@$"ASProject\app\src\main\res\{e.Name}");
                }
                File.Copy(e.FullPath, @$"ASProject\app\src\main\res\{e.Name}", true);
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
                if (Regex.Match(e.OldName, _fileExtensionPattern).Value == string.Empty)
                {
                    DirectoryInfo oldDirectory = new(@$"ASProject\\app\\src\\main\\res\\{e.OldName}\");
                    foreach (var file in oldDirectory.GetFiles())
                        file.Delete();
                    Directory.Delete(@$"ASProject\app\src\main\res\{e.OldName}");
                    Directory.CreateDirectory(@$"ASProject\app\src\main\res\{e.Name}");
                    DirectoryInfo directoryInfo = new(e.FullPath);
                    foreach (var file in directoryInfo.GetFiles())
                        File.Copy(file.FullName, @$"ASProject\app\src\main\res\{e.Name}\{file.Name}");
                    return;
                }
                File.Delete(@$"ASProject\app\src\main\res\{e.OldName}");
                File.Copy(e.FullPath, @$"ASProject\app\src\main\res\{e.Name}");
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
                if (Regex.Match(e.Name, _fileExtensionPattern).Value == string.Empty)
                {
                    DirectoryInfo directoryInfo = new(@$"ASProject\app\src\main\res\{e.Name}");
                    foreach (var file in directoryInfo.GetFiles())
                        file.Delete();
                    directoryInfo.Delete();
                }
                File.Delete(@$"ASProject\app\src\main\res\{e.Name}");
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
                if (Regex.Match(e.Name, _fileExtensionPattern).Value == string.Empty)
                {
                    Directory.CreateDirectory(@$"ASProject\app\src\main\res\{e.Name}");
                }
                File.Copy(e.FullPath, @$"ASProject\app\src\main\res\{e.Name}");
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
        try
        {
            if (e.FullPath[^1] != '~' && e.FullPath.Split(@"\")[^1].Split(".").Length == 2)
            {
                File.Delete(_currentProject.VsProjectPath + @$"\Resources\{e.Name}");
                File.Copy(e.FullPath, _currentProject.VsProjectPath + @$"\Resources\{e.Name}");
            }
        }
        catch
        {
            // ignored
        }
        _watcherVisualStudioProject.EnableRaisingEvents = true;
    }
    #endregion

    #region  Events Realizations
    private async void Page_Loaded(object sender, RoutedEventArgs e)
    {
        await using (var fileStream = File.Open(@$"data.json", FileMode.Open))
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
            if (File.Exists(rootFolderProject + @"\Properties\AndroidManifest.xml") || File.Exists(rootFolderProject + @"\AndroidManifest.xml"))
            {
                await AddProject(Path.Combine(Path.GetDirectoryName(_openFileDialog.FileName)));
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

    private void LaunchVS_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        try
        {
            Process.Start(Settings.Default.VisualStudioPath, _currentProject.VsProjectPathSln);
        }
        catch
        {
            ShowErrorMessageBox(Properties.Resources.ErrorLaunchVs);
        }
    }

    private void LaunchAS_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        try
        {
            Process.Start(Settings.Default.AndroidStudioPath, @$"ASProject");
        }
        catch (Exception ex)
        {
            ShowErrorMessageBox(Properties.Resources.ErrorLaunchAs + $"Error: {ex.Message}");
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
        _watcherAndroidStudioProject?.Dispose();
        _watcherVisualStudioProject?.Dispose();
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
        _watcherAndroidStudioProject = new FileSystemWatcher(@$"ASProject\app\src\main\res");
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
            var directoryInfo = new DirectoryInfo(_currentProject.VsProjectPath + @"\Resources");
            foreach (var folder in directoryInfo.GetDirectories())
            {
                Directory.CreateDirectory($"ASProject\\app\\src\\main\\res\\{folder.Name}");
                foreach (var file in folder.GetFiles())
                {
                    File.Copy(file.FullName, @$"ASProject\app\src\main\res\{folder.Name}\{file.Name}", true);
                }
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
        await using (var write = File.Open(@$"data.json", FileMode.Open))
        {
            _projects?.Add(newProject);
            await JsonSerializer.SerializeAsync(write, _projects);
        }
    }

    private async Task RemoveProject(int id)
    {
        Project project = _projects.FirstOrDefault(x => x.ProjectId == id);
        _projects.Remove(project);
        await using (StreamWriter write = new(@$"data.json"))
        {
            await write.WriteAsync("");
            await JsonSerializer.SerializeAsync(write.BaseStream, _projects);
        }
        ProjectsControl.ItemsSource = null;
        ProjectsControl.ItemsSource = _projects;
    }

    private static void DeleteAllFilesFromAndroidStudioProject()
    {
        DirectoryInfo directoryInfo = new($@"ASProject\app\src\main\res");
        var directories = directoryInfo.GetDirectories();
        foreach (var folder in directories)
        {
            foreach (var file in folder.GetFiles())
                File.Delete(file.FullName);
            Directory.Delete(folder.FullName);
        }
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