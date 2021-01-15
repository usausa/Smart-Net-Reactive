namespace Smart.Reactive
{
    using System;
    using System.Collections.Generic;

    public static class DisposableExtensions
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "Ignore")]
        public static T AddTo<T>(this T disposable, ICollection<IDisposable> collection)
            where T : IDisposable
        {
            collection.Add(disposable);
            return disposable;
        }
    }
}
