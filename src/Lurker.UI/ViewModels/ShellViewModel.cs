//-----------------------------------------------------------------------
// <copyright file="ShellViewModel.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI
{
    using Caliburn.Micro;
    using Lurker.UI.Helpers;
    using Lurker.UI.Models;
    using Lurker.UI.ViewModels;
    using Squirrel;
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Runtime.InteropServices.ComTypes;
    using System.Text;
    using System.Threading.Tasks;

    public class ShellViewModel : Screen, IViewAware
    {
        #region Fields

        private static readonly string PoeLukerGithubUrl = "https://github.com/C1rdec/Poe-Lurker";
        private IWindowManager _windowManager;
        private SimpleContainer _container;
        private ClientLurker _currentLurker;
        private TradebarViewModel _tradeBarOverlay;
        private bool _startWithWindows;
        private bool _needUpdate;
        private bool _showInTaskBar;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ShellViewModel"/> class.
        /// </summary>
        /// <param name="windowManager">The window manager.</param>
        /// <param name="container">The container.</param>
        public ShellViewModel(IWindowManager windowManager, SimpleContainer container)
        {
            this._windowManager = windowManager;
            this._container = container;
            this.WaitForPoe();
            this.StartWithWindows = File.Exists(this.ShortcutFilePath);
            this.ShowInTaskBar = true;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the command.
        /// </summary>
        public DoubleClickCommand ShowSettingsCommand => new DoubleClickCommand(this.ShowSettings);

        /// <summary>
        /// Gets or sets a value indicating whether [show in task bar].
        /// </summary>
        public bool ShowInTaskBar
        {
            get
            {
                return this._showInTaskBar;
            }

            set
            {
                this._showInTaskBar = value;
                this.NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Gets a value indicating whether [start with windows].
        /// </summary>
        public bool StartWithWindows
        {
            get
            {
                return this._startWithWindows;
            }

            set
            {
                this._startWithWindows = value;
                this.NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [need update].
        /// </summary>
        public bool NeedUpdate
        {
            get
            {
                return this._needUpdate;
            }

            set
            {
                this._needUpdate = value;
                this.NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Gets the name of the shortcut.
        /// </summary>
        public string ShortcutName => "PoeLurker.lnk";

        /// <summary>
        /// Gets the application data folder path.
        /// </summary>
        public string ApplicationDataFolderPath => Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        /// <summary>
        /// Gets the startup folder path.
        /// </summary>
        public string StartupFolderPath => Path.Combine(this.ApplicationDataFolderPath, @"Microsoft\Windows\Start Menu\Programs\Startup");

        /// <summary>
        /// Gets the shortcut file path.
        /// </summary>
        public string ShortcutFilePath => Path.Combine(this.StartupFolderPath, this.ShortcutName);

        /// <summary>
        /// Gets the version.
        /// </summary>
        public string Version => GetAssemblyVersion();

        #endregion

        #region Methods

        /// <summary>
        /// Closes this instance.
        /// </summary>
        public void Close()
        {
            this._tradeBarOverlay?.TryClose();
            this.TryClose();
        }

        /// <summary>
        /// Creates the short cut.
        /// </summary>
        public void CreateShortCut()
        {
            if (File.Exists(this.ShortcutFilePath))
            {
                File.Delete(this.ShortcutFilePath);
            }
            else
            {
                var link = (IShellLink)new ShellLink();
                link.SetDescription("PoeLurker");
                link.SetPath(System.Reflection.Assembly.GetExecutingAssembly().Location);
                var file = (IPersistFile)link;
                file.Save(this.ShortcutFilePath, false);
            }

            this.StartWithWindows = !this.StartWithWindows;
        }
        /// <summary>
        /// Gets the assembly version.
        /// </summary>
        /// <returns>The assembly version</returns>
        private static string GetAssemblyVersion()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var information = FileVersionInfo.GetVersionInfo(assembly.Location);
            var version = information.FileVersion.Remove(information.FileVersion.Length - 2);
            return version;
        }

        /// <summary>
        /// Updates this instance.
        /// </summary>
        public async void Update()
        {
            this.ShowInTaskBar = false;
            using (var updateManager = await UpdateManager.GitHubUpdateManager(PoeLukerGithubUrl))
            {
                await updateManager.UpdateApp();
                UpdateManager.RestartApp();
            }
        }

        /// <summary>
        /// Shows the settings.
        /// </summary>
        public void ShowSettings()
        {
            var viewModel = new SettingsViewModel();
            this._windowManager.ShowWindow(viewModel);
        }

        /// <summary>
        /// Updates this instance.
        /// </summary>
        private async Task CheckForUpdate()
        {
            using (var updateManager = await UpdateManager.GitHubUpdateManager(PoeLukerGithubUrl))
            {
                var information = await updateManager.CheckForUpdate();
                this.NeedUpdate = information.ReleasesToApply.Any();
            }
        }

        /// <summary>
        /// Registers the instances.
        /// </summary>
        private void ShowTradebar(Process process)
        {
            Execute.OnUIThread(() => 
            {
                var dockingHelper = new DockingHelper(process);
                var keyboarHelper = new PoeKeyboardHelper(process);
                this._container.RegisterInstance(typeof(ClientLurker), null, this._currentLurker);
                this._container.RegisterInstance(typeof(DockingHelper), null, dockingHelper);
                this._container.RegisterInstance(typeof(PoeKeyboardHelper), null, keyboarHelper);

                this._tradeBarOverlay = this._container.GetInstance<TradebarViewModel>();
                this._windowManager.ShowWindow(this._tradeBarOverlay);
            });
        }

        /// <summary>
        /// Handles the PoeClosed event of the CurrentLurker control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void CurrentLurker_PoeClosed(object sender, System.EventArgs e)
        {
            this._container.UnregisterHandler<ClientLurker>();
            this._container.UnregisterHandler<DockingHelper>();
            this._container.UnregisterHandler<PoeKeyboardHelper>();
            this._currentLurker.PoeClosed -= this.CurrentLurker_PoeClosed;
            this._currentLurker.Dispose();
            this._currentLurker = null;

            this.WaitForPoe();
        }

        /// <summary>
        /// Waits for poe.
        /// </summary>
        private async void WaitForPoe()
        {
            this._currentLurker = new ClientLurker();
            this._currentLurker.PoeClosed += CurrentLurker_PoeClosed;
            var process = await this._currentLurker.WaitForPoe();
            this.ShowTradebar(process);
            #if (!DEBUG)
                await this.CheckForUpdate();
            #endif
        }

        #endregion
    }

    [ComImport]
    [Guid("00021401-0000-0000-C000-000000000046")]
    internal class ShellLink
    {
    }

    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("000214F9-0000-0000-C000-000000000046")]
    internal interface IShellLink
    {
        void GetPath([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszFile, int cchMaxPath, out IntPtr pfd, int fFlags);
        void GetIDList(out IntPtr ppidl);
        void SetIDList(IntPtr pidl);
        void GetDescription([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszName, int cchMaxName);
        void SetDescription([MarshalAs(UnmanagedType.LPWStr)] string pszName);
        void GetWorkingDirectory([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszDir, int cchMaxPath);
        void SetWorkingDirectory([MarshalAs(UnmanagedType.LPWStr)] string pszDir);
        void GetArguments([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszArgs, int cchMaxPath);
        void SetArguments([MarshalAs(UnmanagedType.LPWStr)] string pszArgs);
        void GetHotkey(out short pwHotkey);
        void SetHotkey(short wHotkey);
        void GetShowCmd(out int piShowCmd);
        void SetShowCmd(int iShowCmd);
        void GetIconLocation([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszIconPath, int cchIconPath, out int piIcon);
        void SetIconLocation([MarshalAs(UnmanagedType.LPWStr)] string pszIconPath, int iIcon);
        void SetRelativePath([MarshalAs(UnmanagedType.LPWStr)] string pszPathRel, int dwReserved);
        void Resolve(IntPtr hwnd, int fFlags);
        void SetPath([MarshalAs(UnmanagedType.LPWStr)] string pszFile);
    }
}