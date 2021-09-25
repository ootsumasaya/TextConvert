using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TextConvert.Models;

namespace TextConvert.ViewModels
{
    class ConvertViewModel  : IDisposable
    {
        //Disposableの集約
        private CompositeDisposable compositeDisposable { get; } = new CompositeDisposable();

        public ObservableCollection<ConvertModel> ConvertCollection { get;}


        //ListBoxの選択時の動作
        public ReactiveCommand<ConvertModel> ConvertListSelectionCommand { get; }



        //追加ボタンの有効無効
        public ReactiveProperty<bool> CanAdd { get; }
        //追加ボタンの動作
        public ReactiveCommand AddCommand { get; }


        public ConvertViewModel(TextModel textModel)
        {
            ConvertCollection = new ObservableCollection<ConvertModel>();

            CanAdd = new ReactiveProperty<bool>(true).AddTo(compositeDisposable);
            AddCommand = CanAdd.ToReactiveCommand()
                               .AddTo(compositeDisposable);
            AddCommand.Subscribe(() =>
            {
                ConvertCollection.Add(new ConvertModel()
                {
                    BeforeConvertItem = "",
                    AfterConvertItem = ""
                });
            });

        }




        public void Dispose()
        {
            compositeDisposable.Dispose();
        }
    }


}
