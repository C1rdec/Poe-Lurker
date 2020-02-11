//-----------------------------------------------------------------------
// <copyright file="ClientLurker.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker
{
    using Lurker.Events;
    using Lurker.Extensions;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines a file Watcher for the Client log file.
    /// </summary>
    public class ClientLurker : IDisposable
    {
        #region Fields

        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private static readonly List<string> PossibleProcessNames = new List<string> { "PathOfExile", "PathOfExile_x64", "PathOfExileSteam", "PathOfExile_x64Steam" };
        private static readonly string ClientLogFileName = "Client.txt";
        private static readonly string ClientLogFolderName = "logs";
        private static readonly int TenSeconds = 10000;

        private string _lastLine;
        private CancellationTokenSource _tokenSource;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientLurker" /> class.
        /// </summary>
        public ClientLurker()
        {
            this._tokenSource = new CancellationTokenSource();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the path of exile process.
        /// </summary>
        public Process PathOfExileProcess { get; private set; }

        /// <summary>
        /// Gets or sets the file path.
        /// </summary>
        private string FilePath { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// The poe ended
        /// </summary>
        public event EventHandler PoeClosed;

        /// <summary>
        /// Occurs when the player changed the location.
        /// </summary>
        public event EventHandler<LocationChangedEvent> LocationChanged;

        /// <summary>
        /// Occurs when a trade is accepted.
        /// </summary>
        public event EventHandler<TradeAcceptedEvent> TradeAccepted;

        /// <summary>
        /// Occurs when the players ask the remaining monsters count[remaining monters].
        /// </summary>
        public event EventHandler<MonstersRemainEvent> RemainingMonsters;

        /// <summary>
        /// Occurs when a player join/leave an area.
        /// </summary>
        public event EventHandler<PlayerJoinedEvent> PlayerJoined;

        /// <summary>
        /// Occurs when [player left].
        /// </summary>
        public event EventHandler<PlayerLeftEvent> PlayerLeft;

        /// <summary>
        /// Occurs when [whispered].
        /// </summary>
        public event EventHandler<WhisperEvent> Whispered;


        /// <summary>
        /// Creates new offer.
        /// </summary>
        public event EventHandler<TradeEvent> NewOffer;

        #endregion

        #region Methods

        /// <summary>
        /// Waits for poe.
        /// </summary>
        public async Task<Process> WaitForPoe()
        {
            this.PathOfExileProcess = this.GetProcess();

            while (this.PathOfExileProcess == null)
            {
                await Task.Delay(TenSeconds);
                this.PathOfExileProcess = this.GetProcess();
            }

            this.Lurk();
            this.WaitForExit();

            return this.PathOfExileProcess;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.PathOfExileProcess.Dispose();
                this._tokenSource.Cancel();
            }
        }

        /// <summary>
        /// Reads the last line from ut f8 encoded file.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        /// <exception cref="System.IO.IOException">Error reading from file at " + path</exception>
        private string GetLastLine()
        {
            using (var stream = new FileStream(this.FilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                if (stream.Length == 0)
                {
                    return null;
                }
                stream.Position = stream.Length - 1;

                return GetLine(stream);
            }
        }

        /// <summary>
        /// Gets the new lines.
        /// </summary>
        /// <returns>The all the new lines</returns>
        private IEnumerable<string> GetNewLines()
        {
            using (var stream = new FileStream(this.FilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                if (stream.Length == 0)
                {
                    return null;
                }

                stream.Position = stream.Length - 1;

                var newLines = new List<string>();
                var currentNewLine = GetLine(stream);
                while (this._lastLine != currentNewLine)
                {
                    newLines.Add(currentNewLine);
                    currentNewLine = GetLine(stream);
                }

                return newLines;
            }
        }

        /// <summary>
        /// Gets the line.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>The current line of the stream position</returns>
        private static string GetLine(Stream stream)
        {
            // while we have not yet reached start of file, read bytes backwards until '\n' byte is hit
            var lineLength = 0;
            while (stream.Position > 0)
            {
                stream.Position--;
                var byteFromFile = stream.ReadByte();

                if (byteFromFile < 0)
                {
                    // the only way this should happen is if someone truncates the file out from underneath us while we are reading backwards
                    throw new IOException("Error reading from file");
                }
                else if (byteFromFile == '\n')
                {
                    // we found the new line, break out, fs.Position is one after the '\n' char
                    break;
                }

                lineLength++;
                stream.Position--;
            }

            var oldPosition = stream.Position;
            // fs.Position will be right after the '\n' char or position 0 if no '\n' char
            var bytes = new BinaryReader(stream).ReadBytes(lineLength -1);

            // -1 is the \n
            stream.Position = oldPosition - 1;
            return Encoding.UTF8.GetString(bytes).Replace(System.Environment.NewLine, string.Empty);
        }

        /// <summary>
        /// Lurks this instance.
        /// </summary>
        private void Lurk()
        {
            this.FilePath = Path.Combine(Path.GetDirectoryName(this.PathOfExileProcess.GetMainModuleFileName()), ClientLogFolderName, ClientLogFileName);
            this._lastLine = this.GetLastLine();
            this.LurkLastLine();
        }

        /// <summary>
        /// Lurks this instance.
        /// </summary>
        private async void LurkLastWriteTime()
        {
            Logger.Trace("Lurk with last write time");
            var fileInformation = new FileInfo(this.FilePath);
            var lastWriteTime = fileInformation.LastWriteTimeUtc;

            var token = this._tokenSource.Token;
            while (true)
            {
                do
                {
                    await Task.Delay(500);
                    fileInformation.Refresh();

                    if (token.IsCancellationRequested)
                    {
                        return;
                    }
                }
                while (fileInformation.LastWriteTimeUtc == lastWriteTime);

                lastWriteTime = fileInformation.LastAccessTimeUtc;
                this.OnFileChanged(GetLastLine());
            }
        }

        /// <summary>
        /// Lurks the last line.
        /// </summary>
        private async void LurkLastLine()
        {
            Logger.Trace("Lurk with last line");

            var token = this._tokenSource.Token;
            while (true)
            {
                IEnumerable<string> newLines;
                do
                {
                    await Task.Delay(500);
                    newLines = this.GetNewLines();
                    if (token.IsCancellationRequested)
                    {
                        return;
                    }
                }
                while (newLines.Count() == 0);

                this._lastLine = newLines.First();
                foreach (var line in newLines)
                {
                    this.OnFileChanged(line);
                }
            }
        }

        /// <summary>
        /// Gets the process.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">Path of Exile is not running</exception>
        private Process GetProcess()
        {
            foreach (var processName in PossibleProcessNames)
            {
                var process = Process.GetProcessesByName(processName).FirstOrDefault();
                if (process != null)
                {
                    return process;
                }
            }

            return null;
        }
        
        /// <summary>
        /// Called when [file changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="FileSystemEventArgs"/> instance containing the event data.</param>
        private void OnFileChanged(string newline)
        {
            if (string.IsNullOrEmpty(newline))
            {
                return;
            }

            // TradeEvent need to be parse before whisper
            var tradeEvent = TradeEvent.TryParse(newline);
            if (tradeEvent != null)
            {
                Logger.Debug($"Parsed: {newline}");
                this.NewOffer?.Invoke(this, tradeEvent);
                return;
            }

            var whisperEvent = WhisperEvent.TryParse(newline);
            if (whisperEvent != null)
            {
                Logger.Debug($"Parsed: {newline}");
                this.Whispered?.Invoke(this, whisperEvent);
                return;
            }

            var locationEvent = LocationChangedEvent.TryParse(newline);
            if (locationEvent != null)
            {
                Logger.Debug($"Parsed: {newline}");
                this.LocationChanged?.Invoke(this, locationEvent);
                return;
            }

            var tradeAcceptedEvent = TradeAcceptedEvent.TryParse(newline);
            if (tradeAcceptedEvent != null)
            {
                Logger.Debug($"Parsed: {newline}");
                this.TradeAccepted?.Invoke(this, tradeAcceptedEvent);
                return;
            }

            var monsterEvent = MonstersRemainEvent.TryParse(newline);
            if (monsterEvent != null)
            {
                Logger.Debug($"Parsed: {newline}");
                this.RemainingMonsters?.Invoke(this, monsterEvent);
                return;
            }

            var playerJoinEvent = PlayerJoinedEvent.TryParse(newline);
            if (playerJoinEvent != null)
            {
                Logger.Debug($"Parsed: {newline}");
                this.PlayerJoined?.Invoke(this, playerJoinEvent);
                return;
            }

            var playerLeftEvent = PlayerLeftEvent.TryParse(newline);
            if (playerLeftEvent != null)
            {
                Logger.Debug($"Parsed: {newline}");
                this.PlayerLeft?.Invoke(this, playerLeftEvent);
                return;
            }

            Logger.Trace($"Not parsed: {newline}");
        }

        /// <summary>
        /// Waits for exit.
        /// </summary>
        private async void WaitForExit()
        {
            await Task.Run(() =>
            {
                this.PathOfExileProcess.WaitForExit();
                this.PoeClosed?.Invoke(this, EventArgs.Empty);
            });
        }

        /// <summary>
        /// Tests the file last write time.
        /// </summary>
        /// <returns>True if enable.</returns>
        private bool FileLastWriteTimeEnable()
        {
            var filePath = Path.GetTempFileName();
            var fileInformation = new FileInfo(filePath);
            var initialeWriteTime = fileInformation.LastWriteTimeUtc;

            Thread.Sleep(500);
            File.WriteAllText(filePath, "TestWriteTime");
            fileInformation.Refresh();
            File.Delete(filePath);

            return initialeWriteTime != fileInformation.LastWriteTimeUtc;
        }

        #endregion
    }
}
