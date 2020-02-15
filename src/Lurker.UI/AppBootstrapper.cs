//-----------------------------------------------------------------------
// <copyright file="AppBootstrapper.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI
{
    using Caliburn.Micro;
    using Lurker.Helpers;
    using Lurker.Services;
    using Lurker.UI.Helpers;
    using Lurker.UI.ViewModels;
    using Sentry;
    using System;
    using System.Collections.Generic;

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
            this._container.Singleton<UpdateManager, UpdateManager>();
            this._container.Singleton<SettingsViewModel, SettingsViewModel>();

            this._container.PerRequest<ShellViewModel, ShellViewModel>();
            this._container.PerRequest<TradebarViewModel, TradebarViewModel>();
            this._container.PerRequest<OutgoingbarViewModel, OutgoingbarViewModel>();
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
        /// Override this to provide an IoC specific implementation
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
            this._sentry = SentrySdk.Init(Dsn);
            DisplayRootViewFor<ShellViewModel>();
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
        /// Handles the UnhandledException event of the CurrentDomain control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="UnhandledExceptionEventArgs"/> instance containing the event data.</param>
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = (e.ExceptionObject as Exception);
            Logger.Error(exception, exception.Message);

#if (!DEBUG)
            SentrySdk.CaptureException(exception);
#endif
            this._sentry.Dispose();

        }

        #endregion
    }
}