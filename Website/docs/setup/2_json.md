# Json モードの設定

## JSON データの作成

以下のような形式で JSON データを作成して下さい。

- ReverseCategorys, ShowPrivateWorld, Roles は無くても動作します。
- ReverseCategorys, ShowPrivateWorld を省略した場合は、false として扱われます。

```
{
    "ReverseCategorys": false,
    "ShowPrivateWorld": false,
    "Categorys": []
    "Roles": []
}
```

Categorys の中身は、以下のような形式になります。

- PermittedRoles は省略可能です。

```
{
    "Category": "カテゴリ1",
    "Worlds": []
    "PermittedRoles": ["ロール1", "ロール2"]
},
{
    "Category": "カテゴリ2",
    "Worlds": []
    "PermittedRoles": ["ロール2", "ロール3"]
}
```

Worlds の中身は、以下のような形式になります。

- ID が無い、あるいは空文字列の場合は、押せないボタンとして扱われます。
- Capacity を省略あるいは0にした場合、Capacity の情報が非表示になります。
- Platform の各要素を省略した場合は、false として扱われます。
- Platform 自体を省略した場合や、Platformの全ての要素が false の場合は、Platfrom の情報が非表示になります。
- ReleaseStatus を省略した場合は、public として扱われます。
- PermittedRoles は省略可能です。

```
{
    "ID": "wrld_00001111-2222-3333-4444-555566667777",
    "Name": "ワールド名1",
    "RecommendedCapacity": 30,
    "Capacity": 60,
    "Description": "ワールドの説明文1",
    "Platform": {
        "PC": true,
        "Android": true,
        "iOS": true
    },
    "ReleaseStatus": "public",
    "PermittedRoles": ["ロール1", "ロール2"]
},
{
    "ID": "wrld_01234567-0123-4567-89ab-0123456789ab",
    "Name": "ワールド名2",
    "RecommendedCapacity": 20,
    "Capacity": 40,
    "Description": "ワールドの説明文2",
    "Platform": {
        "PC": true,
        "Android": true,
        "iOS": false
    },
    "ReleaseStatus": "private",
    "PermittedRoles": ["ロール2", "ロール3"]
}
```

Roles の中身は、以下のような形式になります。

```
{
    "RoleName": "ロール1",
    "DisplayNames" ["ユーザー1", "ユーザー2"]
},
{
    "RoleName": "ロール2",
    "DisplayNames" ["ユーザー1", "ユーザー3"]
},
{
    "RoleName": "ロール3",
    "DisplayNames" ["ユーザー1", "ユーザー4"]
}
```

## サムネイルの動画データの作成

:::tip
サムネイルの動画データは、Jsonモードを使用する上で必須ではありません。  
動画データの作成が難しければ、一旦、サムネイル無しで使用することをお勧めします。
:::

JSON データの中でn番目に記載されているワールドのサムネイルが、n～n+1秒で表示されるように、1FPS
の動画を作成して下さい。また、動画データの最初の1秒と、最後の1秒に何らかのデータを入れるようにして下さい。

[幻会ポータルワールド][1]のサムネイルの動画データは、以下のようなシンボリックを作成した後、ffmpeg
を使用して作成されています。

```
00000.png -> ../black.png
00001.png -> ../img/wrld_f8c1d36d-1ba5-40f4-ba7c-7b2f953b7b16.png
00002.png -> ../img/wrld_1611ec65-15ed-4d25-bb40-9304e6dbbd3b.png
...
01376.png -> ../img/wrld_78d032b6-9408-409d-a383-3a029bfbbaa9.png
01377.png -> ../img/wrld_38ee8893-e65b-4ff5-aae3-86f02abbdfb6.png
01378.png -> ../black.png
```

```
ffmpeg -r 1 -i %05d.png \
       -vcodec libx264 -profile:v baseline -pix_fmt yuv420p -movflags +faststart \
       thumbnail.mp4
```

[1]: https://vrchat.com/home/world/wrld_bb6b76c2-d67e-43c8-b268-3b17ff3f79a7/info

## PortalLibrarySystem への URL の登録

PortalLibrarySystem の動作モードを Json に変更した後、上記で作成した JSON データ、
サムネイルの動画データの URL を書き込んで下さい。
サムネイルの動画データを作成していない場合は、空欄のままで大丈夫です。

JSON データの読み込みには VRCStringDownloader を、サムネイルの動画データの読み込みには VRCUnityVideoPlayer
を使用しています。 Udon の仕様上、それぞれ、5秒に1度しかデータを読み込むことができないという制約があり、
他のアセットでも同じ機能を利用していた場合、ロード待ちが発生することがあります。

本アセットでは、JSON データの読み込み、サムネイルの動画データの読み込みに、
それぞれ別にロード遅延（秒）を設定できるようになっています。別のアセットのデータの読み込み順を制御したい場合は、
ここに適切な秒数を設定して下さい。

![JsonMode](/img/JsonMode.png)
