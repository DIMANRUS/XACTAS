namespace XACTAS.WPF.ViewModels;

public class ProjectWinowViewModel : INotifyPropertyChanged
{
    #region Инициализация
    public ProjectWinowViewModel()
    {
        BackgroundBrush.Color = Settings.Default.IsDarkTheme ? Colors.Black : Colors.White;
        ForegroundBrush.Color = Settings.Default.IsDarkTheme ? Colors.White : Colors.Black;
        ImageSourceCurrentLanguage = Settings.Default.Language switch
        {
            "ar" => new BitmapImage(new Uri($"pack://application:,,,/Assets/Icons/Languages/AR.png")),
            "tr" => new BitmapImage(new Uri($"pack://application:,,,/Assets/Icons/Languages/TR.png")),
            "zh" => new BitmapImage(new Uri($"pack://application:,,,/Assets/Icons/Languages/ZH.png")),
            "en" => new BitmapImage(new Uri($"pack://application:,,,/Assets/Icons/Languages/EN.png")),
            _ => new BitmapImage(new Uri($"pack://application:,,,/Assets/Icons/Languages/RU.png"))
        };

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
            switch (Settings.Default.Language)
            {
                case "ru":
                    Settings.Default.Language = "en";
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
                    ImageSourceCurrentLanguage = new BitmapImage(new Uri("pack://application:,,,/Assets/Icons/Languages/EN.png"));
                    break;
                case "en":
                    Settings.Default.Language = "ar";
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("ar-EG");
                    ImageSourceCurrentLanguage = new BitmapImage(new Uri("pack://application:,,,/Assets/Icons/Languages/AR.png"));
                    break;
                case "ar":
                    Settings.Default.Language = "tr";
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("tr-TR");
                    ImageSourceCurrentLanguage = new BitmapImage(new Uri("pack://application:,,,/Assets/Icons/Languages/TR.png"));
                    break;
                case "tr":
                    Settings.Default.Language = "zh";
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("zh-CN");
                    ImageSourceCurrentLanguage = new BitmapImage(new Uri("pack://application:,,,/Assets/Icons/Languages/ZH.png"));
                    break;
                case "zh":
                    Settings.Default.Language = "ru";
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("ru-RU");
                    ImageSourceCurrentLanguage = new BitmapImage(new Uri("pack://application:,,,/Assets/Icons/Languages/RU.png"));
                    break;
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