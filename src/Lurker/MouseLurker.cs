//-----------------------------------------------------------------------
// <copyright file="MouseLurker.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker
{
    using Lurker.Extensions;
    using Lurker.Services;
    using System;
    using System.Diagnostics;

    public class MouseLurker
    {
        private const int MouseHookMessageSizeInBytes = 20;

        private SocketMessageService _socketMessageService;
        private Process _process;
        private uint _mainWindowThreadId;

        public event EventHandler<MouseMessageReceivedEventArgs> MouseMessageReceived;

        public MouseLurker(Process process)
        {
            _process = process;
            _mainWindowThreadId = process.GetWindowThreadProcessId();
            _socketMessageService = new SocketMessageService(MouseHookMessageSizeInBytes);
            _socketMessageService.SocketMessageReceived += SocketMessageService_SocketMessageReceived;
            _socketMessageService.StartListening();

            // TODO: Call exe to hook PoE
        }

        private void SocketMessageService_SocketMessageReceived(object sender, SocketMessageReceivedEventArgs e)
        {
            MouseMessageReceived(this, new MouseMessageReceivedEventArgs
            {
                MessageCode = BitConverter.ToInt32(e.MessageBytes, 0),
                X = BitConverter.ToInt32(e.MessageBytes, 4),
                Y = BitConverter.ToInt32(e.MessageBytes, 8),
                Handle = BitConverter.ToInt32(e.MessageBytes, 12),
                HitTestCode = BitConverter.ToInt32(e.MessageBytes, 16),
            });
        }
    }

    public class MouseMessageReceivedEventArgs : EventArgs
    {
        public int MessageCode;
        public int X;
        public int Y;
        public int Handle;
        public int HitTestCode;
    }
}
