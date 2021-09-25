using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
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
        public CompositeDisposable compositeDisposable { get; } = new CompositeDisposable();

        //変換指定のコレクション
        public ObservableCollection<ConvertModel> ConvertCollection { get;}

        //ListBoxの選択時の動作
        public ReactiveCommand<int> ConvertListSelectionCommand { get; }
        //ListBoxのインデックスを格納
        public int selectedIndex = -1;

        //入力の変更の通知
        public ReactiveProperty<string> BeforeTextProperty { get; }


        //追加ボタンの動作
        public ReactiveCommand AddCommand { get; }

        //削除ボタンの動作
        public ReactiveCommand DeleteCommand { get; }

        //更新ボタンの動作
        public ReactiveCommand ReloadCommand { get; }




        public ConvertViewModel(TextModel textModel)
        {
            //変換指定のコレクション
            ConvertCollection = new ObservableCollection<ConvertModel>();

            //コレクション選択時にそのインデックスを格納
            ConvertListSelectionCommand = new ReactiveCommand<int>();
            ConvertListSelectionCommand.Subscribe(o =>
            {
                selectedIndex = o;
            }).AddTo(compositeDisposable);

            //追加ボタンの動作
            AddCommand = new ReactiveCommand();
            AddCommand.Subscribe(() =>
            {
                ConvertCollection.Add(new ConvertModel()
                {
                    BeforeConvertItem = "",
                    AfterConvertItem = ""
                });
            }).AddTo(compositeDisposable);

            //削除ボタンの動作
            DeleteCommand = new ReactiveCommand();
            DeleteCommand.Subscribe(() =>
            {
                if(0 <= selectedIndex && selectedIndex < ConvertCollection.Count())
                {
                    ConvertCollection.RemoveAt(selectedIndex);
                }
            }).AddTo(compositeDisposable);

            //更新ボタンの動作
            ReloadCommand = new ReactiveCommand();
            ReloadCommand.Subscribe(() =>
            {
                textModel.AfterText = Convert(textModel.BeforeText);
            }).AddTo(compositeDisposable);

            //入力の変更の通知
            BeforeTextProperty = textModel.ObserveProperty(o => o.BeforeText)
                                                     .Select(value => value)
                                                     .ToReactiveProperty()
                                                     .AddTo(compositeDisposable);
            //入力変更時の動作
            BeforeTextProperty.Subscribe(_ =>
            {
                textModel.AfterText = Convert(textModel.BeforeText);
            });

        }


        public string Convert(string beforetext)
        {
            foreach (ConvertModel convertModel in ConvertCollection)
            {
                if(convertModel.BeforeConvertItem != "")
                {
                    beforetext = beforetext.Replace(convertModel.BeforeConvertItem, convertModel.AfterConvertItem);
                }
            }
            return beforetext;
        }

        public void Dispose()
        {
            compositeDisposable.Dispose();
        }
    }


}
