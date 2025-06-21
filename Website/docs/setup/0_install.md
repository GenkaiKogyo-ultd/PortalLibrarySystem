# インストール

## VCC/ALCOM 経由でのインストール

記載途中

## TextMeshPro のインストールおよび設定

TextMeshPro がインストールされていない場合、本アセットを導入した後、
自動で TMP Importer が立ち上がると思います。Import TMP Essentials をクリックしてください。

何らかの原因で TMP Importer が立ち上がらなかった場合は、上のメニューから Window -> TextMeshPro ->
Import TMP Essential Resources を選択することでインストールすることもできます。

:::tip
本アセットは、ワールド容量の削減のため、文字を一文字も収録していない空のフォント (EmptyFont) を採用しています。
VRChat上では Fallback Font として NotoSans が使用されるため正常に表示されますが、Unity上では正常に表示されません。
上のメニューから Edit -> Project Settings を開き、TextMesh Pro -> Settings の Fallback Font Assets に
NotoSansJP-Regular SDF を割り当てることで正常に表示されるようになります。

![TextMeshProFallback](/img/TextMeshProFallback.png)

フォント一覧の中に NotoSansJP-Regular SDF が無い場合は、右上の目のマークを押すと出てきます。

![SelectFont](/img/SelectFont.png)
:::

## ワールドへの設置

1. PortalLibrarySystem2_* のプレハブと、PortalManager のプレハブをワールドに設置して下さい。
2. PortalManager の GameObject を、PortalLibrarySystem2_* のインスペクタの PortalManager
   の所にドラッグ＆ドロップして下さい。
   
![LinkPortalManager](/img/LinkPortalManager.png)

:::tip
1つの PortalManager に対して、複数の PortalLibrarySystem2 を関連付けることも可能です。
:::
