using System.Windows;
using TextConvert.ViewModels;

namespace TextConvert
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }

        //ペーストボタンクリック時の動作
        private void Button_Paste_Click(object sender, RoutedEventArgs e)
        {

            string Input = "";
            // クリップボードからオブジェクトを取得
            IDataObject ClipboardData = Clipboard.GetDataObject();
            // テキストデータかどうか確認
            if (ClipboardData.GetDataPresent(DataFormats.Text))
            {
                // オブジェクトからテキストを取得
                Input = (string)ClipboardData.GetData(DataFormats.Text);
            }
            else
            {
                MessageBox.Show("コピーしたデータが文字列ではありません");
            }
            //TextBoxに表示
            ((MainViewModel)DataContext).BeforeAfterTextModel.BeforeText = Input;
        }


        private void Button_Clear_Click(object sender, RoutedEventArgs e)
        {
            // 入力と出力をクリア
            ((MainViewModel)DataContext).BeforeAfterTextModel.BeforeText = "";
            ((MainViewModel)DataContext).BeforeAfterTextModel.AfterText = "";
        }



    }

}
