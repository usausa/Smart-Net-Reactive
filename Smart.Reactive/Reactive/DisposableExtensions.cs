namespace Smart.Reactive.Reactive
{
    using System;
    using System.Reactive.Disposables;

    /// <summary>
    ///
    /// </summary>
    public static class DisposableExtensions
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="disposable"></param>
        /// <param name="compositeDisposable"></param>
        public static void AddDisposableTo(this IDisposable disposable, CompositeDisposable compositeDisposable)
        {
            if (compositeDisposable == null)
            {
                throw new ArgumentNullException(nameof(compositeDisposable));
            }

            compositeDisposable.Add(disposable);
        }
    }
}
