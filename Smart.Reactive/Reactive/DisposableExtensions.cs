namespace Smart.Reactive
{
    using System;
    using System.Collections.Generic;

    public static class DisposableExtensions
    {
        public static T AddTo<T>(this T disposable, ICollection<IDisposable> collection)
            where T : IDisposable
        {
            if (collection is null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            collection.Add(disposable);
            return disposable;
        }
    }
}
