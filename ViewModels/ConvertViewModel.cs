using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TextConvert.Models;

namespace TextConvert.ViewModels
{
    class ConvertViewModel  : IDisposable, INotifyPropertyChanged
    {
        //変換指定のコレクション
        public ObservableCollection<ConvertModel> _ConvertCollection = new ObservableCollection<ConvertModel>(){};
        public ObservableCollection<ConvertModel> ConvertCollection { get { return this._ConvertCollection; } }

        //リストのインデックス
        private int _CurrentIndex;
        public int CurrentIndex
        {
            get { return this._CurrentIndex; }
            set { SetProperty(ref this._CurrentIndex, value); }
        }

        //ドロップしたときの動作
        public Action<int> DropCallback { get { return OnDrop; } }

        private void OnDrop(int index)
        {
            if (index >= 0)
            {
                this.ConvertCollection.Move(this.CurrentIndex, index);
            }
        }

        //Disposableの集約
        public CompositeDisposable compositeDisposable { get; } = new CompositeDisposable();

        //ListBoxの選択時の動作
        public ReactiveCommand<int> ConvertListSelectionCommand { get; }
        //ListBoxのインデックスを格納
        public int selectedIndex = -1;

        //入力の変更の通知
        public ReactiveProperty<string> BeforeTextProperty { get; }


        //追加ボタンの動作
        public ReactiveCommand AddCommand { get; }


        //更新ボタンの動作
        public ReactiveCommand ReloadCommand { get; }




        public ConvertViewModel(TextModel textModel)
        {

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


        #region INotifyPropertyChanged のメンバ

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            var h = this.PropertyChanged;
            if (h != null) h(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool SetProperty<T>(ref T target, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(target, value)) return false;
            target = value;
            RaisePropertyChanged(propertyName);
            return true;
        }

        #endregion INotifyPropertyChanged のメンバ


        public void Dispose()
        {
            compositeDisposable.Dispose();
        }
    }


}
