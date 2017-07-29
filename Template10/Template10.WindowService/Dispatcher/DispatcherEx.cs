﻿using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Template10.Core;
using Windows.UI.Core;

namespace Template10.Core
{

    // DOCS: https://github.com/Windows-XAML/Template10/wiki/DispatcherWrapper
    public class DispatcherEx : IDispatcherEx
    {
        #region Debug

        static void DebugWrite(string text = null, Services.LoggingService.Severities severity = Services.LoggingService.Severities.Template10, [CallerMemberName]string caller = null) =>
            Services.LoggingService.LoggingService.WriteLine(text, severity, caller: $"DispatcherWrapper.{caller}");

        #endregion

        public static IDispatcherEx Current() => WindowEx.Current().Dispatcher;

        public static IDispatcherEx Create(CoreDispatcher dispatcher)
        {
            return new DispatcherEx(dispatcher);
        }

        private DispatcherEx(CoreDispatcher dispatcher)
        {
            DebugWrite(caller: "Constructor");
            this.dispatcher = dispatcher;
        }

        public bool HasThreadAccess() => dispatcher.HasThreadAccess;

        private readonly CoreDispatcher dispatcher;

        public async Task<T> DispatchAsync<T>(Func<Task<T>> func, int delayms = 0, CoreDispatcherPriority priority = CoreDispatcherPriority.Normal)
        {
            if (delayms > 0)
            {
                await Task.Delay(delayms).ConfigureAwait(dispatcher.HasThreadAccess);
            }

            if (dispatcher.HasThreadAccess && priority == CoreDispatcherPriority.Normal)
            {
                return await func().ConfigureAwait(false);
            }
            else
            {
                var tcs = new TaskCompletionSource<T>();
                await dispatcher.RunAsync(priority, async () =>
                {
                    try
                    {
                        var result = await func().ConfigureAwait(false);
                        tcs.TrySetResult(result);
                    }
                    catch (Exception ex)
                    {
                        tcs.TrySetException(ex);
                    }
                }).AsTask().ConfigureAwait(false);
                return await tcs.Task.ConfigureAwait(false);
            }
        }

        public async Task DispatchAsync(Action action, int delayms = 0, CoreDispatcherPriority priority = CoreDispatcherPriority.Normal)
        {
            if (delayms > 0)
                await Task.Delay(delayms).ConfigureAwait(dispatcher.HasThreadAccess);

            if (dispatcher.HasThreadAccess && priority == CoreDispatcherPriority.Normal)
            {
                action();
            }
            else
            {
                var tcs = new TaskCompletionSource<object>();
                await dispatcher.RunAsync(priority, () =>
                {
                    try
                    {
                        action();
                        tcs.TrySetResult(null);
                    }
                    catch (Exception ex)
                    {
                        tcs.TrySetException(ex);
                    }
                }).AsTask().ConfigureAwait(false);
                await tcs.Task.ConfigureAwait(false);
            }
        }

        public async Task DispatchAsync(Func<Task> func, int delayms = 0, CoreDispatcherPriority priority = CoreDispatcherPriority.Normal)
        {
            if (delayms > 0)
                await Task.Delay(delayms).ConfigureAwait(dispatcher.HasThreadAccess);

            if (dispatcher.HasThreadAccess && priority == CoreDispatcherPriority.Normal)
            {
                await func().ConfigureAwait(false);
            }
            else
            {
                var tcs = new TaskCompletionSource<object>();
                await dispatcher.RunAsync(priority, async () =>
                {
                    try
                    {
                        await func().ConfigureAwait(false);
                        tcs.TrySetResult(null);
                    }
                    catch (Exception ex)
                    {
                        tcs.TrySetException(ex);
                    }
                }).AsTask().ConfigureAwait(false);
                await tcs.Task.ConfigureAwait(false);
            }
        }

        public async Task<T> DispatchAsync<T>(Func<T> func, int delayms = 0, CoreDispatcherPriority priority = CoreDispatcherPriority.Normal)
        {
            if (delayms > 0)
                await Task.Delay(delayms).ConfigureAwait(dispatcher.HasThreadAccess);

            if (dispatcher.HasThreadAccess && priority == CoreDispatcherPriority.Normal)
            {
                return func();
            }
            else
            {
                var tcs = new TaskCompletionSource<T>();
                await dispatcher.RunAsync(priority, () =>
                {
                    try
                    {
                        tcs.TrySetResult(func());
                    }
                    catch (Exception ex)
                    {
                        tcs.TrySetException(ex);
                    }
                }).AsTask().ConfigureAwait(false);
                return await tcs.Task.ConfigureAwait(false);
            }
        }

        public void Dispatch(Action action, int delayms = 0, CoreDispatcherPriority priority = CoreDispatcherPriority.Normal)
        {
            if (delayms > 0)
                Task.Delay(delayms).ConfigureAwait(dispatcher.HasThreadAccess).GetAwaiter().GetResult();

            if (dispatcher.HasThreadAccess && priority == CoreDispatcherPriority.Normal)
            {
                action();
            }
            else
            {
                dispatcher.RunAsync(priority, () => action()).AsTask().ConfigureAwait(false).GetAwaiter().GetResult();
            }
        }

        public T Dispatch<T>(Func<T> action, int delayms = 0, CoreDispatcherPriority priority = CoreDispatcherPriority.Normal)
        {
            if (delayms > 0)
                Task.Delay(delayms).ConfigureAwait(dispatcher.HasThreadAccess).GetAwaiter().GetResult();

            if (dispatcher.HasThreadAccess && priority == CoreDispatcherPriority.Normal)
            {
                return action();
            }
            else
            {
                var tcs = new TaskCompletionSource<T>();
                dispatcher.RunAsync(priority, delegate
                {
                    try
                    {
                        tcs.TrySetResult(action());
                    }
                    catch (Exception ex)
                    {
                        tcs.TrySetException(ex);
                    }
                }).AsTask().ConfigureAwait(false).GetAwaiter().GetResult();
                return tcs.Task.ConfigureAwait(false).GetAwaiter().GetResult();
            }
        }

        public async Task DispatchIdleAsync(Action action, int delayms = 0)
        {
            if (delayms > 0)
                await Task.Delay(delayms).ConfigureAwait(false);

            var tcs = new TaskCompletionSource<object>();
            await dispatcher.RunIdleAsync(delegate
            {
                try
                {
                    action();
                    tcs.TrySetResult(null);
                }
                catch (Exception ex)
                {
                    tcs.TrySetException(ex);
                }
            }).AsTask().ConfigureAwait(false);
            await tcs.Task.ConfigureAwait(false);
        }

        public async Task DispatchIdleAsync(Func<Task> func, int delayms = 0)
        {
            if (delayms > 0)
                await Task.Delay(delayms).ConfigureAwait(false);

            var tcs = new TaskCompletionSource<object>();
            await dispatcher.RunIdleAsync(async delegate
            {
                try
                {
                    await func().ConfigureAwait(false);
                    tcs.TrySetResult(null);
                }
                catch (Exception ex)
                {
                    tcs.TrySetException(ex);
                }
            }).AsTask().ConfigureAwait(false);
            await tcs.Task.ConfigureAwait(false);
        }

        public async Task<T> DispatchIdleAsync<T>(Func<T> func, int delayms = 0)
        {
            if (delayms > 0)
                await Task.Delay(delayms).ConfigureAwait(false);

            var tcs = new TaskCompletionSource<T>();
            await dispatcher.RunIdleAsync(delegate
            {
                try
                {
                    tcs.TrySetResult(func());
                }
                catch (Exception ex)
                {
                    tcs.TrySetException(ex);
                }
            }).AsTask().ConfigureAwait(false);
            return await tcs.Task.ConfigureAwait(false);
        }

        public void DispatchIdle(Action action, int delayms = 0)
        {
            if (delayms > 0)
                Task.Delay(delayms).ConfigureAwait(false).GetAwaiter().GetResult();

            dispatcher.RunIdleAsync(args => action()).AsTask().ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public T DispatchIdle<T>(Func<T> action, int delayms = 0) where T : class
        {
            if (delayms > 0)
                Task.Delay(delayms).ConfigureAwait(false).GetAwaiter().GetResult();

            var tcs = new TaskCompletionSource<T>();
            dispatcher.RunIdleAsync(delegate
            {
                try
                {
                    tcs.TrySetResult(action());
                }
                catch (Exception ex)
                {
                    tcs.TrySetException(ex);
                }
            }).AsTask().ConfigureAwait(false).GetAwaiter().GetResult();
            return tcs.Task.ConfigureAwait(false).GetAwaiter().GetResult();
        }
    }
}