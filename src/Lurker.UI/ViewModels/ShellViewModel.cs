//-----------------------------------------------------------------------
// <copyright file="ShellViewModel.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI
{
    using Caliburn.Micro;
    using Lurker.UI.Helpers;
    using Lurker.UI.ViewModels;
    using System.Diagnostics;
    using System.Threading.Tasks;

    public class ShellViewModel : Screen, IViewAware
    {
        #region Fields

        private IWindowManager _windowManager;
        private SimpleContainer _container;
        private ClientLurker _currentLurker;

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
        }

        #endregion

        #region Methods

        /// <summary>
        /// Registers the instances.
        /// </summary>
        private void DisplayRoot(Process process)
        {
            Execute.OnUIThread(() => 
            {
                var dockingHelper = new DockingHelper(process);
                var keyboarHelper = new PoeKeyboardHelper(process);
                this._container.RegisterInstance(typeof(ClientLurker), null, this._currentLurker);
                this._container.RegisterInstance(typeof(DockingHelper), null, dockingHelper);
                this._container.RegisterInstance(typeof(PoeKeyboardHelper), null, keyboarHelper);

                var viewModel = this._container.GetInstance<TradeBarViewModel>();
                this._windowManager.ShowWindow(viewModel);
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
            this.DisplayRoot(process);
        }

        #endregion
    }
}