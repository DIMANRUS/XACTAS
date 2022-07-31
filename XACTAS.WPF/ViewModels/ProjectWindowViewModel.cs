namespace XACTAS.WPF.ViewModels;

public class ProjectWinowViewModel : INotifyPropertyChanged
{
    #region Инициализация
    public ProjectWinowViewModel()
    {
        BackgroundBrush.Color = Settings.Default.IsDarkTheme ? Colors.Black : Colors.White;
        ForegroundBrush.Color = Settings.Default.IsDarkTheme ? Colors.White : Colors.Black;
        ImageSourceCurrentLanguage = new BitmapImage(new Uri($"pack://application:,,,/Assets/Icons/Languages/{(Settings.Default.Language == "ru" ? "RU" : "EN")}.png"));

        #region Инициализация комманд
        ChangeTheme = new RelayCommand(() =>
        {
            Settings.Default.IsDarkTheme = !Settings.Default.IsDarkTheme;
            Settings.Default.Save();
            BackgroundBrush.Color = Settings.Default.IsDarkTheme ? Colors.Black : Colors.White;
            ForegroundBrush.Color = Settings.Default.IsDarkTheme ? Colors.White : Colors.Black;
            NotifyPropertyChanged(nameof(ForegroundColor));
            NotifyPropertyChanged(nameof(BackgroundBrush));
            NotifyPropertyChanged(nameof(ForegroundBrush));
        });
        ChangeLanguage = new RelayCommand(() =>
        {
            if (Settings.Default.Language == "ru")
            {
                Settings.Default.Language = "en-US";
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
                ImageSourceCurrentLanguage = new BitmapImage(new Uri("pack://application:,,,/Assets/Icons/Languages/EN.png"));
            }
            else
            {
                Settings.Default.Language = "ru";
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("ru");
                ImageSourceCurrentLanguage = new BitmapImage(new Uri("pack://application:,,,/Assets/Icons/Languages/RU.png"));
            }
            NotifyPropertyChanged(nameof(ImageSourceCurrentLanguage));
            Settings.Default.Save();
        });
        #endregion
    }
    #endregion

    #region Свойства
    public Color ForegroundColor => Settings.Default.IsDarkTheme ? Colors.White : Colors.Black;
    public SolidColorBrush BackgroundBrush { get; } = new();
    public SolidColorBrush ForegroundBrush { get; } = new();
    public ImageSource ImageSourceCurrentLanguage { get; private set; }
    #endregion

    #region  Команды
    public ICommand ChangeTheme { get; }
    public ICommand ChangeLanguage { get; }
    #endregion

    #region Реализация INotifyPropertyChanged
    public event PropertyChangedEventHandler PropertyChanged;
    protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    #endregion
}