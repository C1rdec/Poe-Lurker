//-----------------------------------------------------------------------
// <copyright file="AppBootstrapper.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Caliburn.Micro;
    using Lurker.Extensions;
    using Lurker.Helpers;
    using Lurker.Services;
    using Lurker.UI.Helpers;
    using Lurker.UI.Services;
    using Lurker.UI.ViewModels;
    using Sentry;

    /// <summary>
    /// Represents AppBootstrapper.
    /// </summary>
    /// <seealso cref="Caliburn.Micro.BootstrapperBase" />
    public class AppBootstrapper : BootstrapperBase
    {
        #region Fields

        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private static readonly string Dsn = "https://e92715c769194c3aa7a89d387f488136@sentry.io/2473965";
        private SimpleContainer _container;
        private IDisposable _sentry;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AppBootstrapper"/> class.
        /// </summary>
        public AppBootstrapper()
        {
            AppDomain.CurrentDomain.UnhandledException += this.CurrentDomain_UnhandledException;
            this.Initialize();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Override to configure the framework and setup your IoC container.
        /// </summary>
        protected override void Configure()
        {
            this._container = new SimpleContainer();

            this._container.Singleton<IWindowManager, WindowManager>();
            this._container.Singleton<IEventAggregator, EventAggregator>();
            this._container.Singleton<KeyboardHelper, KeyboardHelper>();
            this._container.Singleton<SettingsService, SettingsService>();
            this._container.Singleton<HotkeyService, HotkeyService>();
            this._container.Singleton<BuildService, BuildService>();
            this._container.Singleton<SoundService, SoundService>();
            this._container.Singleton<CollaborationService, CollaborationService>();
            this._container.Singleton<SettingsViewModel, SettingsViewModel>();
            this._container.Singleton<DashboardViewModel, DashboardViewModel>();
            this._container.Singleton<TutorialViewModel, TutorialViewModel>();

            this._container.PerRequest<AfkService, AfkService>();
            this._container.PerRequest<UpdateManager, UpdateManager>();
            this._container.PerRequest<ShellViewModel, ShellViewModel>();
            this._container.PerRequest<TradebarViewModel, TradebarViewModel>();
            this._container.PerRequest<PopupViewModel, PopupViewModel>();
            this._container.PerRequest<BuildTimelineViewModel, BuildTimelineViewModel>();
            this._container.PerRequest<BuildViewModel, BuildViewModel>();
            this._container.PerRequest<OutgoingbarViewModel, OutgoingbarViewModel>();
            this._container.PerRequest<LifeBulbViewModel, LifeBulbViewModel>();
            this._container.PerRequest<ManaBulbViewModel, ManaBulbViewModel>();
            this._container.PerRequest<HideoutViewModel, HideoutViewModel>();
            this._container.PerRequest<HelpViewModel, HelpViewModel>();
            this._container.RegisterInstance(typeof(SimpleContainer), null, this._container);
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
            return this._container.GetInstance(service, key);
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
            return this._container.GetAllInstances(service);
        }

        /// <summary>
        /// Override this to provide an IoC specific implementation.
        /// </summary>
        /// <param name="instance">The instance to perform injection on.</param>
        protected override void BuildUp(object instance)
        {
            this._container.BuildUp(instance);
        }

        /// <summary>
        /// Override this to add custom behavior to execute after the application starts.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The args.</param>
        protected override void OnStartup(object sender, System.Windows.StartupEventArgs e)
        {
            var settings = this._container.GetInstance<SettingsService>();
            this._sentry = SentrySdk.Init(Dsn);
            SentrySdk.ConfigureScope((c) =>
            {
                c.User = new User()
                {
                    Id = settings.UserId,
                };
            });

            if (RunningInstance() != null)
            {
                System.Windows.MessageBox.Show("Another Instance Is Running");
                System.Windows.Application.Current.Shutdown();
                return;
            }

            this.DisplayRootViewFor<ShellViewModel>();
        }

        /// <summary>
        /// Override this to add custom behavior on exit.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event args.</param>
        protected override void OnExit(object sender, EventArgs e)
        {
            this._sentry.Dispose();
            base.OnExit(sender, e);
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

        #if !DEBUG
            SentrySdk.CaptureException(exception);
        #endif
            this._sentry.Dispose();
        }

        #endregion
    }
}