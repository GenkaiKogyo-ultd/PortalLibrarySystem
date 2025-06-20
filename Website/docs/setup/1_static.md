# Static モードの設定

## ワールドの追加方法

1. 動作モードが Static になっていることを確認した後、「カテゴリ一覧」の＋をクリックして下さい。
   WorldData -> StaticWorldData -> Categorys の下に、Category という GameObject が追加されます。
2. Category のインスペクタを開き、「カテゴリー名」を設定して下さい。
3. Category のインスペクタの、「ワールド一覧」の＋をクリックして下さい。
   Category の下に World という GameObject が追加されます。
4. World のインスペクタを開き、ワールドIDに登録したいワールドのIDを入力して下さい。
5. World のインスペクタの下の方にある、「ワールドIDからワールドの情報を取得」のボタンを押してください。
   ワールド名やワールドに入れる人数、対応プラットフォーム、Description、サムネイル画像などを自動で取得します。

![AddWorld](/img/AddWorld.png)

:::tip
取得したサムネイル画像は、Assets/PortalLibrarySystem/Thumbnails 以下に保存されています。
:::

## ロールの設定方法

:::tip
本アセットを使用する上で、ロールの設定は必須ではありません。  
ロールの設定が不要であれば、この手順は省略できます。
:::

カテゴリーやワールドには、ロールの設定を行うことができます。
ロールの設定を行った場合、そのカテゴリー/ワールドは、設定したロールに所属している人しか見れなくなります。

1. 「ロール一覧」の＋をクリックして下さい。
   WorldData -> StaticWorldData -> Roles の下に、Role という GameObject が追加されます。
2. Role のインスペクタを開き、ロール名を設定して下さい。
3. ロールに追加したいユーザーの名前を、「ユーザー名一覧」に追加して下さい。
4. Category / World のインスペクタを開き、「カテゴリ/ワールドの表示を許可するロール」に
   ロール名を追加して下さい。

![ConfigureRole](/img/ConfigureRole.png)

:::warning
本アセットのポータルの動作は、**初期設定ではグローバル**となっています。  
権限が無い人は、ロールの設定されたカテゴリー・ワールドのボタンを見ることはできませんが、
権限を持った別の人が出したポータルは見ることができます。
:::

:::warning
本アセットのロール機能は、**ワールドIDの秘匿を保証するものではありません**。  
本機能を使用したことによる、ワールドIDの漏洩などに関する責任は負いかねます。
:::

## その他の設定

### カテゴリーの順を逆順に

初期状態では、カテゴリーの並び順は、GameObject の順番の通りになりますが、
このチェックボックスを有効にすることで、並び順が逆になります。
後から追加したものを先頭に持って行きたい時に使います。

### プライベートワールドを表示

初期状態では、公開状態が private になっているワールドのポータルは開けないように設定されています。
こののチェックボックスを有効にすることで、プライベートワールドのポータルも開けるようになります。
