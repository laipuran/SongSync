namespace SongSync;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
	}

    private async void GetButton_Clicked(object sender, EventArgs e)
    {
        PermissionStatus status = await Permissions.RequestAsync<Permissions.StorageWrite>();
        if (status != PermissionStatus.Granted)
        {
            Result.Text = "Permissiont DENIED!";
            return;
        }
        List<string> filePaths = new();
#if WINDOWS
        filePaths = await GetFileNames();
#elif ANDROID
        string downloadPath = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads).AbsolutePath;
        filePaths = await GetFileNames(downloadPath);
#endif
        Result.Text = string.Join(", ", filePaths);
    }
    static readonly string[] ends = { ".wav", ".wma", ".mp3", ".flac", ".aac", ".m4a" };
    public static async Task<string> FilePicker()
    {
        PickOptions pickOptions = new()
        {
            PickerTitle = "选一个文件"
        };
        FileResult result = await Microsoft.Maui.Storage.FilePicker.Default.PickAsync(pickOptions);
        return result.FullPath;
    }

    public static async Task<List<string>> GetFileNames(string rootPath = null)
    {
        if (rootPath == null)
        {
            rootPath = await FilePicker();
            rootPath = Directory.GetParent(rootPath).FullName;
            if (string.IsNullOrEmpty(rootPath))
                return null;
        }
        List<string> filePaths = GetFiles(rootPath);
        return filePaths;
    }

    private static List<string> GetFiles(string path)
    {
        List<string> filePaths = new();
        try
        {
            foreach (string file in Directory.GetFiles(path))
            {
                bool flag = false;
                foreach (string end in ends)
                {
                    if (file.EndsWith(end))
                    {
                        flag = true;
                    }
                }
                if (!flag)
                {
                    continue;
                }
                filePaths.Add(file);
            }
            string[] directories = Directory.GetDirectories(path);
            if (directories.Length >= 0)
            {
                foreach (string directory in directories)
                {
                    filePaths.AddRange(GetFiles(directory));
                }
            }
        }
        catch (Exception ex)
        {
            List<string> errors = new()
            {
                ex.Message
            };
            return errors;
        }
        return filePaths;
    }
}

