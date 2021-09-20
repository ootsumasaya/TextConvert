# TextConvert
クリップボードにコピーしたテキストを.(ピリオド)でのみ改行したテキストに変換します。

論文などをコピーしたときの無駄な改行を消したいときに利用してください。

# ScreenShot
初期画面
![初期画面](https://user-images.githubusercontent.com/32339438/133966145-5bafc410-6ea1-47f8-a14d-06e2489faab0.PNG)



入力例
![入力例](https://user-images.githubusercontent.com/32339438/133966161-523b2e61-7944-43d8-8b7a-a25e881334c5.PNG)

オートモード
![オートモード](https://user-images.githubusercontent.com/32339438/133966181-5acb40ff-954c-4bf9-a233-73461b7fa56d.PNG)



# 使い方
## マニュアルモード
０．bin/ReleaseをダウンロードしてTextConvert.exeを実行。

１．変換したいテキストをクリップボードにコピーしペーストボタンを押す、または右欄に入力する。

２．コピーを押す

これで変換後のテキストがクリップボードにコピーされます。

## オートモード
０．bin/ReleaseをダウンロードしてTextConvert.exeを実行。

１．オートをONにする。

２．変換したいテキストをクリップボードにコピーすると、変換後のテキストがクリップボードに自動でコピーされます。


# 参考文献
・Windows GUIプログラミング入門17 変換ツール

https://qiita.com/Kosen-amai/items/c2542fe0d0b62d108096

・C#でクリップボードから文字列を取得する

https://water2litter.net/rye/post/c_io_from_clipboard/


# ver1.1
・オートモードを追加しました。

オートをオンにするとペーストとコピーを自動で行います。

# ver1.0
・変換ボタンを押す必要がなくなりました。

・出力欄が空欄のときコピーボタンを無効に、入力欄と出力欄が空欄のときクリアボタンを無効になります。
