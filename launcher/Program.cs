using System.Diagnostics;
using System.Net.Http;

namespace MiMenu;

internal static class Program
{
    internal const string LoaderUrl = "https://cdn.jsdelivr.net/gh/xd290/mi-menu@main/src/loader.lua";
    internal const string RepoUrl = "https://github.com/xd290/mi-menu";
    internal static readonly HttpClient Http = new();

    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new LauncherForm());
    }
}

internal sealed class LauncherForm : Form
{
    private readonly Label _status;
    private readonly TextBox _log;

    public LauncherForm()
    {
        Text = "mi-menu launcher";
        ClientSize = new Size(470, 390);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = Color.FromArgb(18, 18, 20);
        ForeColor = Color.White;
        Font = new Font("Consolas", 9f);

        var title = new Label
        {
            Text = "mi-menu",
            Font = new Font("Consolas", 22f, FontStyle.Bold),
            ForeColor = Color.FromArgb(48, 255, 106),
            Bounds = new Rectangle(20, 15, 420, 40),
        };

        var sub = new Label
        {
            Text = "mod menu open source para Roblox",
            ForeColor = Color.FromArgb(130, 130, 140),
            Bounds = new Rectangle(22, 55, 420, 20),
        };

        _status = new Label
        {
            Text = "Roblox: no detectado",
            ForeColor = Color.FromArgb(200, 200, 200),
            Bounds = new Rectangle(22, 85, 420, 20),
        };

        _log = new TextBox
        {
            Multiline = true,
            ReadOnly = true,
            ScrollBars = ScrollBars.Vertical,
            BackColor = Color.FromArgb(28, 28, 32),
            ForeColor = Color.FromArgb(150, 220, 160),
            BorderStyle = BorderStyle.None,
            Bounds = new Rectangle(22, 115, 426, 180),
        };

        var btnDetect = MakeButton("Detectar Roblox", 22, 310);
        btnDetect.Click += (_, _) => Detect();

        var btnFetch = MakeButton("Copiar comando", 152, 310);
        btnFetch.Click += (_, _) => Copy();

        var btnOpen = MakeButton("Abrir Roblox", 282, 310);
        btnOpen.Click += (_, _) => OpenRoblox();

        var btnRepo = MakeButton("GitHub", 22, 344);
        btnRepo.Click += (_, _) => OpenUrl(Program.RepoUrl);

        var btnReload = MakeButton("Verificar loader", 152, 344);
        btnReload.Click += async (_, _) => await PingAsync();

        var btnExit = MakeButton("Salir", 282, 344);
        btnExit.Click += (_, _) => Close();

        Controls.AddRange(new Control[] { title, sub, _status, _log, btnDetect, btnFetch, btnOpen, btnRepo, btnReload, btnExit });

        var tick = new System.Windows.Forms.Timer { Interval = 3000 };
        tick.Tick += (_, _) => Detect();
        tick.Start();

        Write("1) Detecta Roblox   2) Copia comando   3) Pegalo en tu ejecutor");
        Write("rojo = roblox cerrado   verde = listo");
    }

    private Button MakeButton(string text, int x, int y) => new()
    {
        Text = text,
        Bounds = new Rectangle(x, y, 122, 28),
        FlatStyle = FlatStyle.Flat,
        BackColor = Color.FromArgb(32, 32, 38),
        ForeColor = Color.White,
        FlatAppearance = { BorderColor = Color.FromArgb(60, 60, 70) },
    };

    private void Write(string msg) => _log.AppendText(msg + Environment.NewLine);

    private void Detect()
    {
        var running = Process.GetProcessesByName("RobloxPlayerBeta").Length > 0;
        _status.Text = running ? "Roblox: detectado, listo para ejecutar" : "Roblox: no detectado";
        _status.ForeColor = running ? Color.FromArgb(48, 255, 106) : Color.FromArgb(200, 200, 200);
    }

    private void Copy()
    {
        Clipboard.SetText($"loadstring(game:HttpGet(\"{Program.LoaderUrl}\"))()");
        Write("comando copiado al portapapeles");
    }

    private async Task PingAsync()
    {
        try
        {
            var lua = await Program.Http.GetStringAsync(Program.LoaderUrl);
            Write($"loader responde OK ({lua.Length} bytes)");
        }
        catch (Exception ex)
        {
            Write("loader no responde: " + ex.Message);
        }
    }

    private void OpenRoblox()
    {
        try
        {
            Process.Start(new ProcessStartInfo("roblox://") { UseShellExecute = true });
            Write("abriendo Roblox...");
        }
        catch
        {
            Write("no pude abrir Roblox, abrelo a mano");
        }
    }

    private void OpenUrl(string url) =>
        Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
}
