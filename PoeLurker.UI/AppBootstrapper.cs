//-----------------------------------------------------------------------
// <copyright file="AppBootstrapper.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace PoeLurker.UI;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Caliburn.Micro;
using PoeLurker.Core.Extensions;
using PoeLurker.Core.Helpers;
using PoeLurker.Core.Services;
using PoeLurker.Patreon.Services;
using PoeLurker.UI.Helpers;
using PoeLurker.UI.Services;
using PoeLurker.UI.ViewModels;

/// <summary>
/// Represents AppBootstrapper.
/// </summary>
/// <seealso cref="BootstrapperBase" />
public class AppBootstrapper : BootstrapperBase
{
    #region Fields

    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
    private static readonly string Dsn = "https://e92715c769194c3aa7a89d387f488136@sentry.io/2473965";
    private SimpleContainer _container;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="AppBootstrapper"/> class.
    /// </summary>
    public AppBootstrapper()
    {
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        Initialize();
    }

    #endregion

    #region Methods

    /// <summary>
    /// Override to configure the framework and setup your IoC container.
    /// </summary>
    protected override void Configure()
    {
        _container = new SimpleContainer();

        _container.Singleton<IWindowManager, WindowManager>();
        _container.Singleton<IEventAggregator, EventAggregator>();
        _container.Singleton<KeyboardHelper, KeyboardHelper>();
        _container.Singleton<PoeNinjaService, PoeNinjaService>();
        _container.Singleton<SettingsService, SettingsService>();
        _container.Singleton<HotkeyService, HotkeyService>();
        _container.Singleton<StashTabService, StashTabService>();
        _container.Singleton<BuildService, BuildService>();
        _container.Singleton<SoundService, SoundService>();
        _container.Singleton<CollaborationService, CollaborationService>();
        _container.Singleton<WinookService, WinookService>();

        _container.PerRequest<PushBulletService, PushBulletService>();
        _container.PerRequest<PushHoverService, PushHoverService>();
        _container.Singleton<GithubService, GithubService>();

        _container.Singleton<SettingsViewModel, SettingsViewModel>();
        _container.Singleton<TutorialViewModel, TutorialViewModel>();
        _container.Singleton<WelcomeViewModel, WelcomeViewModel>();
        _container.PerRequest<AfkService, AfkService>();
        _container.PerRequest<UpdateManager, UpdateManager>();
        _container.PerRequest<ShellViewModel, ShellViewModel>();
        _container.PerRequest<WikiViewModel, WikiViewModel>();
        _container.PerRequest<StashTabGridViewModel, StashTabGridViewModel>();
        _container.PerRequest<TradebarViewModel, TradebarViewModel>();
        _container.PerRequest<PopupViewModel, PopupViewModel>();
        _container.PerRequest<BuildTimelineViewModel, BuildTimelineViewModel>();
        _container.PerRequest<BuildViewModel, BuildViewModel>();
        _container.PerRequest<OutgoingbarViewModel, OutgoingbarViewModel>();
        _container.PerRequest<LifeBulbViewModel, LifeBulbViewModel>();
        _container.PerRequest<ManaBulbViewModel, ManaBulbViewModel>();
        _container.PerRequest<HideoutViewModel, HideoutViewModel>();
        _container.PerRequest<HelpViewModel, HelpViewModel>();

        _container.RegisterInstance(typeof(SimpleContainer), null, _container);
    }

    /// <summary>
    /// Override this to provide an IoC specific implementation.
    /// </summary>
    /// <param name="service">The service to locate.</param>
    /// <param name="key">The key to locate.</param>
    /// <returns>
    /// The located service.
    /// </returns>
    protected override object GetInstance(Type service, string key)
    {
        return _container.GetInstance(service, key);
    }

    /// <summary>
    /// Override this to provide an IoC specific implementation.
    /// </summary>
    /// <param name="service">The service to locate.</param>
    /// <returns>
    /// The located services.
    /// </returns>
    protected override IEnumerable<object> GetAllInstances(Type service)
    {
        return _container.GetAllInstances(service);
    }

    /// <summary>
    /// Override this to provide an IoC specific implementation.
    /// </summary>
    /// <param name="instance">The instance to perform injection on.</param>
    protected override void BuildUp(object instance)
    {
        _container.BuildUp(instance);
    }

    /// <summary>
    /// Override this to add custom behavior to execute after the application starts.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The args.</param>
    protected override void OnStartup(object sender, System.Windows.StartupEventArgs e)
    {
        var settings = _container.GetInstance<SettingsService>();
        var windowManager = _container.GetInstance<IWindowManager>();
        if (RunningInstance() != null)
        {
            System.Windows.MessageBox.Show("Another Instance Is Running");
            System.Windows.Application.Current.Shutdown();
            return;
        }

        if (settings.ShowWelcome)
        {
            settings.ShowWelcome = false;
            settings.Save();

        }

        DisplayRootViewForAsync<ShellViewModel>();
    }

    /// <summary>
    /// Runnings the instance.
    /// </summary>
    /// <returns>The other running instance.</returns>
    public static Process RunningInstance()
    {
        var currentProcess = Process.GetCurrentProcess();
        var processes = Process.GetProcessesByName(currentProcess.ProcessName);

        try
        {
            var currentFilePath = currentProcess.GetMainModuleFileName();
            foreach (var process in processes)
            {
                if (process.Id != currentProcess.Id)
                {
                    if (process.GetMainModuleFileName() == currentFilePath)
                    {
                        return process;
                    }
                }
            }
        }
        catch (System.ComponentModel.Win32Exception)
        {
            AdminRequestHelper.RequestAdmin();
        }

        return null;
    }

    /// <summary>
    /// Handles the UnhandledException event of the CurrentDomain control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="UnhandledExceptionEventArgs"/> instance containing the event data.</param>
    private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        var exception = e.ExceptionObject as Exception;
        Logger.Error(exception, exception.Message);
    }

    #endregion
}