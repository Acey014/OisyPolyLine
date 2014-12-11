OisyPolyLine
============

Unity用の、ポリラインを生成するサンプルコード。
Update毎に、指定したTransformの移動量に応じてポリラインを生成します。

○使い方
・プロジェクトの任意のフォルダにスクリプトをドラッグアンドドロップして登録
・任意のGameObjectにコンポーネントを登録し、BasePointとTipPointを指定
・実行すると、オブジェクトの移動量に応じてポリラインが張られる

○機能解説
AlphaGraduation：時間経過と共ににポリゴンが徐々に透過されていきます
Material：指定しないとSprites-Defaultになります

○仕様
生成されるmeshのUVは、四角ポリゴン一つ(三角ポリゴン二つ)につきテクスチャ全体を張るようにしています。
（mesh全体で1枚分のテクスチャを張り合わせるようにしたほうがいいかもしれない・・・）

○展望、課題など
・ポリゴンをさらに分割して綺麗に表示する機能(Subdivision)
・処理の最適化