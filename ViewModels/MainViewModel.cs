using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TextConvert.Models;

namespace TextConvert.ViewModels
{
    class MainViewModel : IDisposable
    {
        //Disposableの集約
        private CompositeDisposable compositeDisposable { get; } = new CompositeDisposable();

        //変更通知機能を持つ、テキストが格納されたモデル
        public TextModel BeforeAfterTextModel;

        public ReactiveProperty<string> BeforeTextProperty { get; }
        public ReactiveProperty<string> AfterTextProperty { get; }

        //変更前テキストのViewModelを持つReactiveProperty
        public BeforeViewModel BeforeViewModel { get; }
        //変更後テキストのViewModelを持つReactiveProperty
        public AfterViewModel AfterViewModel { get; }

        //コピーボタンの有効無効
        public ReactiveProperty<bool> CanCopy { get; }
        //コピーボタンを押したときの動作
        public ReactiveCommand CopyCommand { get; }

        //クリアボタンの有効無効
        public ReactiveProperty<bool> CanClear { get; }
        //クリアボタンを押したときの動作
        public ReactiveCommand ClearCommand { get; }

        //ペーストボタンの有効無効
        public ReactiveProperty<bool> CanPaste { get; }
        //ペーストボタンの動作
        public ReactiveCommand PasteCommand { get; }

        //オートペーストボタンの現状
        public ReactiveProperty<bool> AutoPasteIsChecked { get; set;}
        //オートペーストの動作
        public ReactiveCommand AutoPasteCommand { get; }


        //ViewModelの定義
        public MainViewModel()
        {
            //変更通知機能を持つ、テキストが格納されたモデルの作成
            BeforeAfterTextModel = new TextModel();

            //入力の変更の通知
            BeforeTextProperty = BeforeAfterTextModel.ObserveProperty(o => o.BeforeText)
                                                     .Select(value => value)
                                                     .ToReactiveProperty()
                                                     .AddTo(compositeDisposable);

            //出力の変更の通知
            AfterTextProperty = BeforeAfterTextModel.ObserveProperty(o => o.AfterText)
                                                    .Select(value => value)
                                                    .ToReactiveProperty()
                                                    .AddTo(compositeDisposable);

            //出力の変更を検知してコピーボタンの有効無効
            CanCopy = AfterTextProperty.Select(x => string.IsNullOrWhiteSpace(x) == false)
                                       .ToReactiveProperty()
                                       .AddTo(compositeDisposable);
            CopyCommand = CanCopy.ToReactiveCommand()
                                 .AddTo(compositeDisposable);

            //コピーボタンの動作
            CopyCommand.Subscribe(() => Clipboard.SetText(AfterTextProperty.Value));

            //入力と出力の変更を検知してクリアボタンの有効無効
            CanClear = Observable.CombineLatest(BeforeTextProperty,
                                                           AfterTextProperty,
                                                           (x, y) => string.IsNullOrWhiteSpace(x) == false || string.IsNullOrWhiteSpace(y) == false)
                                            .ToReactiveProperty()
                                            .AddTo(compositeDisposable);
            ClearCommand = CanClear.ToReactiveCommand()
                                   .AddTo(compositeDisposable);

            //クリアボタンの動作
            ClearCommand.Subscribe(() =>
            {
                //入力と出力をクリア
                BeforeAfterTextModel.BeforeText = "";
                BeforeAfterTextModel.AfterText = "";
            });

            //ペーストボタンの有効無効
            CanPaste = new ReactiveProperty<bool>(true).AddTo(compositeDisposable);
            PasteCommand = CanPaste.ToReactiveCommand()
                                   .AddTo(compositeDisposable);

            //ペーストボタンの動作
            PasteCommand.Subscribe(() =>
            {
                SetClipboardText(BeforeAfterTextModel);
            });

            //オートペーストボタンの現状
            AutoPasteIsChecked = new ReactiveProperty<bool>(false).AddTo(compositeDisposable);
            AutoPasteCommand = new ReactiveCommand().AddTo(compositeDisposable);

            //オートペーストボタンの動作
            AutoPasteCommand.Subscribe(() =>
            {
                Thread thread = new Thread(() =>
                {
                    while(AutoPasteIsChecked.Value == true)
                    {
                        Thread STAthread = new Thread(() => SetClipboardText(BeforeAfterTextModel));
                        STAthread.SetApartmentState(ApartmentState.STA);
                        STAthread.Start();
                        Thread.Sleep(1000);
                    }
                    return;
                });
                thread.Start();
            });

            //BeforeViewModelの作成
            BeforeViewModel = new BeforeViewModel(BeforeAfterTextModel).AddTo(compositeDisposable);
            //AfterViewModelの作成
            AfterViewModel = new AfterViewModel(BeforeAfterTextModel).AddTo(compositeDisposable);

        }

        public void SetClipboardText(TextModel BeforeAfterTextModel)
        {
            // クリップボードからオブジェクトを取得
            IDataObject ClipboardData = Clipboard.GetDataObject();
            // テキストデータかどうか確認
            if (ClipboardData.GetDataPresent(DataFormats.Text))
            {
                // オブジェクトからテキストを取得
                BeforeAfterTextModel.BeforeText = (string)ClipboardData.GetData(DataFormats.Text);
            }
            else
            {
                MessageBox.Show("コピーしたデータが文字列ではありません");
            }
            return;
        }


        //Dispose関数
        public void Dispose()
        {
            compositeDisposable.Dispose();
        }
    }
}
