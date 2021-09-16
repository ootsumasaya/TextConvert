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

        //オートボタンの現状
        public ReactiveProperty<bool> AutoIsChecked { get; set;}
        //オートボタンの動作
        public ReactiveCommand AutoCommand { get; }

        //BeforeTextの表示切替
        public ReactiveProperty<Visibility> BeforeTextVisibility { get; }

        //ボタンのHorizontalAlignment
        public ReactiveProperty<HorizontalAlignment> GridHorizontalAlignment { get; }


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
            CopyCommand.Subscribe(() => SetClipboardText(BeforeAfterTextModel));

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
                GetClipboardText(BeforeAfterTextModel);
            });

            //オートボタンの現状
            AutoIsChecked = new ReactiveProperty<bool>(false).AddTo(compositeDisposable);
            AutoCommand = new ReactiveCommand().AddTo(compositeDisposable);

            //オートボタンがオンのときBeforeTextを非表示にする
            BeforeTextVisibility = AutoIsChecked.Select(x =>
                {
                    if (x is true)
                    {
                        return Visibility.Collapsed;
                    }
                    else
                    {
                        return Visibility.Visible;
                    }
                }).ToReactiveProperty()
                  .AddTo(compositeDisposable);

            //オートボタンがオンのとき左詰めする
            GridHorizontalAlignment = AutoIsChecked.Select(x =>
            {
                if (x is true)
                {
                    return HorizontalAlignment.Left;
                }
                else
                {
                    return HorizontalAlignment.Stretch;
                }
            }).ToReactiveProperty()
              .AddTo(compositeDisposable);

            //オートボタンの動作
            AutoCommand.Subscribe(() =>
            {
                Thread thread = new Thread(() =>
                {
                    while(AutoIsChecked.Value == true)
                    {
                        Thread STAthread = new Thread(() =>
                        {
                            GetClipboardText(BeforeAfterTextModel);
                            SetClipboardText(BeforeAfterTextModel);
                        });
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

        public void GetClipboardText(TextModel BeforeAfterTextModel)
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

        public void SetClipboardText(TextModel BeforeAfterTextModel)
        {
            Clipboard.SetText(AfterTextProperty.Value);
        }


        //Dispose関数
        public void Dispose()
        {
            compositeDisposable.Dispose();
        }
    }
}
