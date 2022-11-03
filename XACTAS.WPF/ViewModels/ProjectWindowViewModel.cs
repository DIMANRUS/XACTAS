namespace XACTAS.WPF.ViewModels;

public class ProjectWinowViewModel : INotifyPropertyChanged
{
    #region Constructors
    public ProjectWinowViewModel()
    {
        BackgroundBrush.Color = Settings.Default.IsDarkTheme ? Colors.Black : Colors.White;
        ForegroundBrush.Color = Settings.Default.IsDarkTheme ? Colors.White : Colors.Black;
        ImageSourceCurrentLanguage = new BitmapImage(new Uri($"pack://application:,,,/Assets/Icons/Languages/{Settings.Default.Language}.png"));
        Thread.CurrentThread.CurrentUICulture = new CultureInfo(Settings.Default.Language);

        #region Commands Initializations
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
            switch (Settings.Default.Language)
            {
                case "ru-RU":
                    Settings.Default.Language = "en-US";
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
                    ImageSourceCurrentLanguage = new BitmapImage(new Uri("pack://application:,,,/Assets/Icons/Languages/en-US.png"));
                    break;
                case "en-US":
                    Settings.Default.Language = "ar-EG";
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("ar-EG");
                    ImageSourceCurrentLanguage = new BitmapImage(new Uri("pack://application:,,,/Assets/Icons/Languages/ar-EG.png"));
                    break;
                case "ar-EG":
                    Settings.Default.Language = "tr-TR";
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("tr-TR");
                    ImageSourceCurrentLanguage = new BitmapImage(new Uri("pack://application:,,,/Assets/Icons/Languages/tr-TR.png"));
                    break;
                case "tr-TR":
                    Settings.Default.Language = "zh-CN";
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("zh-CN");
                    ImageSourceCurrentLanguage = new BitmapImage(new Uri("pack://application:,,,/Assets/Icons/Languages/zh-CN.png"));
                    break;
                case "zh-CN":
                    Settings.Default.Language = "ru-RU";
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("ru-RU");
                    ImageSourceCurrentLanguage = new BitmapImage(new Uri("pack://application:,,,/Assets/Icons/Languages/ru-RU.png"));
                    break;
            }
            Settings.Default.Save();
            NotifyPropertyChanged(nameof(ImageSourceCurrentLanguage));
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
    public ICommand ChangeTheme { get; private init; }
    public ICommand ChangeLanguage { get; private init; }
    #endregion

    #region INotifyPropertyChanged Realization
    public event PropertyChangedEventHandler PropertyChanged;
    protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    #endregion
}