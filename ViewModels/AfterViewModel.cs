using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using TextConvert.Models;

namespace TextConvert.ViewModels
{
    class AfterViewModel : IDisposable
    {
        //Disposableの集約
        private CompositeDisposable compositeDisposable { get; } = new CompositeDisposable();
        public ReactiveProperty<string> AfterText { get; }

        public AfterViewModel(TextModel textModel)
        {
            //出力の変更検知
            AfterText = textModel.ObserveProperty(o => o.AfterText).ToReactiveProperty().AddTo(compositeDisposable);
            //出力の変更を検知してモデルにデータを格納
            AfterText.Subscribe(_ => textModel.AfterText = AfterText.Value);
        }


        public void Dispose()
        {
            compositeDisposable.Dispose();
        }
    }
}
