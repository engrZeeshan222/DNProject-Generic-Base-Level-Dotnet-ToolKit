using System.Diagnostics;

namespace GenericToolKit.Domain.Utilities
{

    public static class ExceptionHandler
    {
        // Executes
        public static void Execute(Action action, string methodName, string layerName, Action<Exception>? onException = null)
        {
            try
            {
                action?.Invoke();
            }
            catch (ArgumentNullException ex)
            {
                HandleException(ex, methodName, layerName, onException, "ArgumentNullException");
            }
            catch (ArgumentException ex)
            {
                HandleException(ex, methodName, layerName, onException, "ArgumentException");
            }
            catch (InvalidOperationException ex)
            {
                HandleException(ex, methodName, layerName, onException, "InvalidOperationException");
            }
            catch (Exception ex)
            {
                HandleException(ex, methodName, layerName, onException, "Exception");
            }
        }

        public static TResult Execute<TResult>(
            Func<TResult> func,
            string methodName,
            string layerName,
            TResult defaultValue = default(TResult)!,
            Action<Exception>? onException = null)
        {
            try
            {
                return func != null ? func() : defaultValue;
            }
            catch (ArgumentNullException ex)
            {
                HandleException(ex, methodName, layerName, onException, "ArgumentNullException");
                return defaultValue;
            }
            catch (ArgumentException ex)
            {
                HandleException(ex, methodName, layerName, onException, "ArgumentException");
                return defaultValue;
            }
            catch (InvalidOperationException ex)
            {
                HandleException(ex, methodName, layerName, onException, "InvalidOperationException");
                return defaultValue;
            }
            catch (Exception ex)
            {
                HandleException(ex, methodName, layerName, onException, "Exception");
                return defaultValue;
            }
        }

        // Executes
        public static async Task ExecuteAsync(
            Func<Task> asyncAction,
            string methodName,
            string layerName,
            Action<Exception>? onException = null)
        {
            try
            {
                if (asyncAction != null)
                {
                    await asyncAction();
                }
            }
            catch (ArgumentNullException ex)
            {
                HandleException(ex, methodName, layerName, onException, "ArgumentNullException");
            }
            catch (ArgumentException ex)
            {
                HandleException(ex, methodName, layerName, onException, "ArgumentException");
            }
            catch (InvalidOperationException ex)
            {
                HandleException(ex, methodName, layerName, onException, "InvalidOperationException");
            }
            catch (Exception ex)
            {
                HandleException(ex, methodName, layerName, onException, "Exception");
            }
        }

        public static async Task<TResult> ExecuteAsync<TResult>(
            Func<Task<TResult>> asyncFunc,
            string methodName,
            string layerName,
            TResult defaultValue = default(TResult)!,
            Action<Exception>? onException = null)
        {
            try
            {
                return asyncFunc != null ? await asyncFunc() : defaultValue;
            }
            catch (ArgumentNullException ex)
            {
                HandleException(ex, methodName, layerName, onException, "ArgumentNullException");
                return defaultValue;
            }
            catch (ArgumentException ex)
            {
                HandleException(ex, methodName, layerName, onException, "ArgumentException");
                return defaultValue;
            }
            catch (InvalidOperationException ex)
            {
                HandleException(ex, methodName, layerName, onException, "InvalidOperationException");
                return defaultValue;
            }
            catch (Exception ex)
            {
                HandleException(ex, methodName, layerName, onException, "Exception");
                return defaultValue;
            }
        }

        // Handle exception
        private static void HandleException(
            Exception ex,
            string methodName,
            string layerName,
            Action<Exception>? onException = null,
            string exceptionType = "Exception")
        {
            LogException(ex, methodName, layerName, exceptionType);
            onException?.Invoke(ex);
        }

        // Logs exception
        private static void LogException(Exception ex, string methodName, string layerName, string exceptionType)
        {
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            Debug.WriteLine($"[{timestamp}] [{layerName}] [{exceptionType}] Error in {methodName}: {ex.Message}");
            Debug.WriteLine($"Stack Trace: {ex.StackTrace}");

            if (ex.InnerException != null)
            {
                Debug.WriteLine($"Inner Exception: {ex.InnerException.GetType().Name} - {ex.InnerException.Message}");
            }
        }
    }
}

