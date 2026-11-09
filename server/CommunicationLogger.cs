using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// https://learn.microsoft.com/en-us/dotnet/api/system.io.file.appendalltextasync?view=net-10.0

public static class CommunicationLogger
{
    private static readonly SemaphoreSlim _lock = new(1, 1);

    private const string LogFile = "communication-errors.log";

    public static async Task LogErrorAsync(string message, Exception? exception = null)
    {
        string logMessage =
            $"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC] " +
            $"{message}";

        if (exception != null)
        {
            logMessage += $" | {exception.GetType().Name}: {exception.Message}";
        }

        logMessage += Environment.NewLine;

        await _lock.WaitAsync();

        try
        {
            await File.AppendAllTextAsync(LogFile, logMessage);
        }
        finally
        {
            _lock.Release();
        }
    }
}
