namespace Smart.Reactive
{
    using System;
    using System.Collections.Generic;

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
        /// <param name="collection"></param>
        /// <returns></returns>
        public static IDisposable AddTo(this IDisposable disposable, ICollection<IDisposable> collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            collection.Add(disposable);
            return disposable;
        }
    }
}
