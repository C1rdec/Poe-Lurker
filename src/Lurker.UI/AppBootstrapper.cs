//-----------------------------------------------------------------------
// <copyright file="AppBootstrapper.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI
{
    using Caliburn.Micro;
    using Lurker.UI.Helpers;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class AppBootstrapper : BootstrapperBase 
    {
        #region Fields

        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private SimpleContainer _container;

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

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Logger.Error((e.ExceptionObject as Exception).Message);
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
            this._container.PerRequest<ShellViewModel, ShellViewModel>();
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
        protected override async void OnStartup(object sender, System.Windows.StartupEventArgs e) 
        {
            await this.RegisterInstances();

            DisplayRootViewFor<ShellViewModel>();
        }

        /// <summary>
        /// Registers the instances.
        /// </summary>
        private async Task RegisterInstances()
        {
            this._container.Singleton<ClientLurker, ClientLurker>();
            var lurker = this._container.GetInstance<ClientLurker>();
            var process = await lurker.WaitForPoe();

            var dockingHelper = new DockingHelper(process);
            var keyboarHelper = new PoeKeyboardHelper(process);
            this._container.RegisterInstance(typeof(DockingHelper), null, dockingHelper);
            this._container.RegisterInstance(typeof(PoeKeyboardHelper), null, keyboarHelper);
        }

        #endregion
    }
}