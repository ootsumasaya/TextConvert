using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
        }


        // https://water2litter.net/rye/post/c_io_from_clipboard/
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
            InputText.DataContext = Input;
        }

        private void Button_Convert_Click(object sender, RoutedEventArgs e)
        {
            // 入力の受け取り
            string BeforeText = InputText.Text;
            // データ変換
            string tmpText = BeforeText;
            tmpText = tmpText.Replace("\r\n", "");
            tmpText = tmpText.Replace("\n", "");
            tmpText = tmpText.Replace(".", ".\n\n");
            // 出力
            string AfterText = tmpText;
            OutputText.DataContext = AfterText;
            
        }

        private void Button_Clear_Click(object sender, RoutedEventArgs e)
        {
            // 入力と出力をクリア
            InputText.DataContext = "";
            OutputText.DataContext = "";
        }

        private void Button_Copy_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(OutputText.Text);
        }
    }
}
