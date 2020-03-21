//-----------------------------------------------------------------------
// <copyright file="RemainingMonsterViewModel.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI.ViewModels
{
    using Lurker.Patreon.Events;

    public class RemainingMonsterViewModel : Caliburn.Micro.PropertyChangedBase
    {
        #region Fields

        private MonstersRemainEvent _monsterEvent;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RemainingMonsterViewModel"/> class.
        /// </summary>
        /// <param name="monsterEvent">The monster event.</param>
        public RemainingMonsterViewModel(MonstersRemainEvent monsterEvent)
        {
            this._monsterEvent = monsterEvent;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the monster remaining.
        /// </summary>
        public int MonsterCount => this._monsterEvent.MonsterCount;

        /// <summary>
        /// Gets a value indicating whether [less than50].
        /// </summary>
        public bool LessThan50 => this.MonsterCount < 50;

        #endregion
    }
}
