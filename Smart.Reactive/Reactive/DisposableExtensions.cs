namespace Smart.Reactive
{
    using System;
    using System.Collections.Generic;

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
        public static T AddTo<T>(this T disposable, ICollection<IDisposable> collection)
            where T : IDisposable
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
