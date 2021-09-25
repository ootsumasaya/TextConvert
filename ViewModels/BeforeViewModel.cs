using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using TextConvert.Models;

namespace TextConvert.ViewModels
{
    class BeforeViewModel : IDisposable
    {
        //Disposableの集約
        private CompositeDisposable compositeDisposable { get; } = new CompositeDisposable();
        //変更前テキスト
        public ReactiveProperty<string> BeforeText { get; }

        //BeforeTextのReadOnly
        public ReactiveProperty<bool> BeforeTextIsReadOnly { get; set; }


        public BeforeViewModel(TextModel textModel)
        {
            //入力の変更検知
            BeforeText = textModel.ObserveProperty(o => o.BeforeText).ToReactiveProperty().AddTo(compositeDisposable);
            //BeforeTextの変更を検知してモデルにデータをを格納
            BeforeText.Subscribe(_ => textModel.BeforeText = BeforeText.Value);

            //BeforeTextのReadOnly
            BeforeTextIsReadOnly = new ReactiveProperty<bool>(false);
        }


        public void Dispose()
        {
            compositeDisposable.Dispose();
        }
    }
}
