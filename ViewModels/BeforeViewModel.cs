using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.ComponentModel;
using System.Reactive.Disposables;
using TextConvert.Models;

namespace TextConvert.ViewModels
{
    class BeforeViewModel : IDisposable
    {
        //Disposableの集約
        private CompositeDisposable compositeDisposable { get; } = new CompositeDisposable();
        //変更前テキスト
        public TextModel BeforeAfterTextModel { get; }
        public ReactiveProperty<string> BeforeText { get; }


        public BeforeViewModel(TextModel BATextModel)
        {
            //モデルの格納
            BeforeAfterTextModel = BATextModel;

            //入力の変更検知
            BeforeText = BeforeAfterTextModel.ObserveProperty(o => o.BeforeText).ToReactiveProperty().AddTo(compositeDisposable);
            //BeforeTextの変更を検知してモデルにデータをを格納
            BeforeText.Subscribe(_ => BeforeAfterTextModel.BeforeText = BeforeText.Value);
        }


        public void Dispose()
        {
            compositeDisposable.Dispose();
        }
    }
}
