using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Windows;
using TextConvert.Models;

namespace TextConvert.ViewModels
{
    class AfterViewModel : IDisposable
    {
        public TextModel BeforeAfterTextModel { get; }
        public ReactiveProperty<string> AfterText { get; }


        public AfterViewModel(TextModel BATextModel)
        {
            //モデルの格納
            BeforeAfterTextModel = BATextModel;
            //出力の変更検知
            AfterText = BeforeAfterTextModel.ObserveProperty(o => o.AfterText).ToReactiveProperty();
            //出力の変更を検知してモデルにデータを格納
            AfterText.Subscribe(_ => BeforeAfterTextModel.AfterText = AfterText.Value);
        }


        public void Dispose()
        {
            AfterText.Dispose();
        }
    }
}
