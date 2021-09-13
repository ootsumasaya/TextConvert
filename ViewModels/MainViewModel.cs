using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Windows;
using TextConvert.Models;

namespace TextConvert.ViewModels
{
    class MainViewModel : IDisposable
    {
        //変更通知機能を持つ、テキストが格納されたモデル
        public TextModel BeforeAfterTextModel;

        public ReactiveProperty<string> BeforeTextProperty { get; }
        public ReactiveProperty<string> AfterTextProperty { get; }

        //変更前テキストのViewModelを持つReactiveProperty
        public BeforeViewModel BViewModel { get; }
        //変更後テキストのViewModelを持つReactiveProperty
        public AfterViewModel AViewModel { get; }

        //コピーボタンを押したときの動作
        public ReactiveCommand CopyCommand { get; }
        //コピーボタンの有効無効
        public ReactiveProperty<bool> CanCopy { get; }


        //クリアボタンの有効無効
        public ReactiveProperty<bool> ClearButtonIsEnable { get; }


        //ViewModelの定義
        public MainViewModel()
        {
            //変更通知機能を持つ、テキストが格納されたモデルの作成
            BeforeAfterTextModel = new TextModel();

            //入力の変更の通知
            BeforeTextProperty = BeforeAfterTextModel.ObserveProperty(o => o.BeforeText).Select(value => value).ToReactiveProperty();
            //出力の変更の通知
            AfterTextProperty = BeforeAfterTextModel.ObserveProperty(o => o.AfterText).Select(value => value).ToReactiveProperty();

            //出力の変更を検知してCanCopyのTrueFalseを変更
            CanCopy = AfterTextProperty.Select(x => string.IsNullOrWhiteSpace(x) == false).ToReactiveProperty();
            //Cancopyの変更を検知してコピーボタンの有効無効
            CopyCommand = CanCopy.ToReactiveCommand();
            CopyCommand.Subscribe(() => Clipboard.SetText(AfterTextProperty.Value));


            //入力と出力の変更を検知してClearの有効無効
            ClearButtonIsEnable = Observable.CombineLatest(BeforeTextProperty, AfterTextProperty,
                (x, y) => string.IsNullOrWhiteSpace(x) == false || string.IsNullOrWhiteSpace(y) == false
            ).ToReactiveProperty();


            //BeforeViewModelの作成
            BViewModel = new BeforeViewModel(BeforeAfterTextModel);
            //AfterViewModelの作成
            AViewModel = new AfterViewModel(BeforeAfterTextModel);

        }


        //Dispose関数
        public void Dispose()
        {
            BeforeTextProperty.Dispose();
            AfterTextProperty.Dispose();
            CanCopy.Dispose();
            CopyCommand.Dispose();
            ClearButtonIsEnable.Dispose();
            AViewModel.Dispose();
            BViewModel.Dispose();
        }
    }
}
