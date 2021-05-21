namespace Smart.Reactive
{
    using System;
    using System.Reactive.Disposables;

    public static class DisposableExtensions
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "Ignore")]
        public static T AddTo<T>(this T disposable, CompositeDisposable disposables)
            where T : IDisposable
        {
            disposables.Add(disposable);
            return disposable;
        }
    }
}
