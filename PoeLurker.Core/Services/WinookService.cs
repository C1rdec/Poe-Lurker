using System.Diagnostics;
using Winook;

namespace PoeLurker.Core.Services;

public class WinookService
{
    #region Fields

    private KeyboardHook _hook;

    #endregion

    #region Methods

    public Task InstallAsync()
    {
        var process = Process.GetCurrentProcess();
        _hook = new KeyboardHook(process.Id);

        return _hook.InstallAsync();
    }

    public Task<(KeyCode Key, Modifiers Modifier)> GetNextKeyAsync()
    {
        var taskCompletionSource = new TaskCompletionSource<(KeyCode key, Modifiers modifier)>();

        void handler(object s, KeyboardMessageEventArgs e)
        {
            if (e.Direction == KeyDirection.Up)
            {
                var keyCode = (KeyCode)e.KeyValue;
                var modifier = Modifiers.None;
                if (e.Control)
                {
                    modifier = Modifiers.Control;
                }
                else if (e.Alt)
                {
                    modifier = Modifiers.Alt;
                }
                else if (e.Shift)
                {
                    modifier = Modifiers.Shift;
                }

                _hook.MessageReceived -= handler;
                taskCompletionSource.SetResult((keyCode, modifier));
            }
        }

        _hook.MessageReceived += handler;

        return taskCompletionSource.Task;
    }

    #endregion
}
