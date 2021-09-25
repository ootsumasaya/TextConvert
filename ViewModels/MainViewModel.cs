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
        public TextModel textModel;

        public ReactiveProperty<string> BeforeTextProperty { get; }
        public ReactiveProperty<string> AfterTextProperty { get; }

        //変更前テキストのViewModel
        public BeforeViewModel beforeViewModel { get; }
        //変更後テキストのViewModel
        public AfterViewModel afterViewModel { get; }
        //変換動作のViewModel
        public ConvertViewModel convertViewModel { get; }

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

        //オートボタンの現状
        public ReactiveProperty<bool> AutoIsChecked { get;}
        //オートボタンの動作
        public ReactiveCommand AutoCommand { get; }

        //BeforeTextのReadOnly
        public ReactiveProperty<bool> BeforeTextIsReadOnly { get; }




        //ViewModelの定義
        public MainViewModel()
        {
            //変更通知機能を持つ、テキストが格納されたモデルの作成
            textModel = new TextModel();
            //BeforeViewModelの作成
            beforeViewModel = new BeforeViewModel(textModel).AddTo(compositeDisposable);
            //AfterViewModelの作成
            afterViewModel = new AfterViewModel(textModel).AddTo(compositeDisposable);
            //ConvertViewModekの作成
            convertViewModel = new ConvertViewModel(textModel).AddTo(compositeDisposable);

            //入力の変更の通知
            BeforeTextProperty = textModel.ObserveProperty(o => o.BeforeText)
                                                     .Select(value => value)
                                                     .ToReactiveProperty()
                                                     .AddTo(compositeDisposable);

            //出力の変更の通知
            AfterTextProperty = textModel.ObserveProperty(o => o.AfterText)
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
            CopyCommand.Subscribe(() => SetClipboardText(textModel));

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
                textModel.BeforeText = "";
                textModel.AfterText = "";
            });

            //ペーストボタンの有効無効
            CanPaste = new ReactiveProperty<bool>(true).AddTo(compositeDisposable);
            PasteCommand = CanPaste.ToReactiveCommand()
                                   .AddTo(compositeDisposable);

            //ペーストボタンの動作
            PasteCommand.Subscribe(() =>
            {
                GetClipboardText(textModel);
            });

            //オートボタンの現状
            AutoIsChecked = new ReactiveProperty<bool>(false).AddTo(compositeDisposable);
            AutoCommand = new ReactiveCommand().AddTo(compositeDisposable);



            //オートボタンの動作
            AutoCommand.Subscribe(() =>
            {
                Thread thread = new Thread(() =>
                {
                    while(AutoIsChecked.Value == true)
                    {
                        //クリップボード関連はメインスレッドで動作させる
                        Thread STAthread = new Thread(() =>
                        {
                            GetClipboardText(textModel);
                        });
                        STAthread.SetApartmentState(ApartmentState.STA);
                        STAthread.Start();
                        Thread.Sleep(1000);
                    }
                    return;
                });
                thread.Start();
            });


            //オート時にBeforeTextをReadOnlyにする
            AutoIsChecked.Subscribe(x =>
            {
                beforeViewModel.BeforeTextIsReadOnly.Value = x;
            });

            //オート時に変換後テキストが変更されたらそれをクリップボードに格納
            AfterTextProperty.Subscribe(x =>
            {
                if (AutoIsChecked.Value == true)
                {
                    SetClipboardText(textModel);
                }
            });

            



        }

        public void GetClipboardText(TextModel textModel)
        {
            // クリップボードからオブジェクトを取得
            IDataObject ClipboardData = Clipboard.GetDataObject();
            
            string ClipboardDataString;
            // テキストデータかどうか確認
            if (ClipboardData.GetDataPresent(DataFormats.Text))
            {
                // オブジェクトからテキストを取得
                ClipboardDataString = (string)ClipboardData.GetData(DataFormats.Text);
                //BeforeTextにデータを格納
                textModel.BeforeText = ClipboardDataString;
            }
        }

        public void SetClipboardText(TextModel textModel)
            {
                //AfterTextをクリップボード格納
                Clipboard.SetText(textModel.AfterText);
            }

      public void Dispose()
        {
            compositeDisposable.Dispose();
        }
    }
}
