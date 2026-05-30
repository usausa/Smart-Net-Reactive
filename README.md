# Smart.Reactive .NET

[![NuGet](https://img.shields.io/nuget/v/Usa.Smart.Reactive.svg)](https://www.nuget.org/packages/Usa.Smart.Reactive)

Reactive extension helpers.

## Features

* `ObservableValue<T>` — observable property wrapper implementing `INotifyPropertyChanged`
* `NotifyPropertyChangedExtensions` — convert `INotifyPropertyChanged` events to `IObservable<T>`
* `NotifyDataErrorInfoExtensions` — convert `INotifyDataErrorInfo` events to `IObservable<T>`
* Extension methods: `ThrottleFirst`, `Pairwise`, `WhereNotNull`, `ObserveOnCurrentContext`
