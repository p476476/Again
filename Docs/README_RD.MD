# Again System For 程式開發教學

## 安裝
- 到PackageManager安裝[InputSystem](https://docs.unity3d.com/Packages/com.unity.inputsystem@1.7/manual/Installation.html)套件，裝完好像會跳出彈窗問要不要切到新的InputSystem，選是並自動重開Unity
- 下載`AgainSystem.unitypackage`，將檔案匯入Unity
- 開啟 `Assets/Again/Scenes/HelloWorld`，執行場景，確認有AVG正常執行
- 額外測試Spine
  - 下載`Resources.unitypackage`並匯入，此為測試資源，用完可以刪除
  -  開啟 `Assets/Again/Scenes/HelloWorldSpine`，執行場景，確認AVG正常執行

## 範例
- Scenes/HelloWorld
  - 最簡單的範例，執行HelloWorld腳本
- Scenes/HelloWorldSpine
  - 執行含有Spine動畫的腳本
  - 記得要匯入`Resources.unitypackage`才有資源
- Scenes/SpriteEditor
  - 可以快速測試已有的腳本
  - 記得要設定GoogleSheetId
- Scenes/SpriteEditorOrtho，同上但相機為正交相機
  
## 新增專案的Gooshe Sheet遠端腳本
- 各專案都會有一個自己的GoogleSheet，用來編輯腳本
- 開啟[範例腳本](https://docs.google.com/spreadsheets/d/18vCXwuMSR7K0FUEEcHy7X5NzgtyWyUgr7sbeIRW74zs/edit?gid=1492752394#gid=1492752394)，點擊左上檔案>建立副本，複製試算表
- 新的試算表網址中`https://docs.google.com/spreadsheets/d/XXXXX/edit`，`XXXXX`即為`GoogleSheetId`
- 之後企劃可以複製範例編輯，並在Config分頁的腳本列表中新增該腳本，就能顯示在遊戲中


## 新場景使用
- 將`Prefab/AgainSystem`或`Prefab/AgainSystemOrtho`擇一放入場景中
- 點開物件 AgainSystem>Main，Inspector看到Again System並設定GoogleSheetId為剛剛的`GoogleSheetId`貼上
- 場景中新增一個EventSystem
- 場景中新增一個空物件，並加上程式碼
  ```csharp
  private void Start()
  {
      // 執行腳本
      // HelloWold為GoogleSheet上其中的一個腳本
      AgainSystem.Instance.Execute("HelloWorld");
  }
  ```
- 執行場景，確認AVG正常執行

## 資源放置
- 資源要放在對應的資料夾下，系統才找的到圖和Spine，否則會直接跳過指令
  - Assets/Resources/Backgrounds，背景圖片
  - Assets/Resources/Images，圖片
  - Assets/Resources/Spines，Spine動畫
  - Assets/Resources/TSV，本地腳本

## Prefabs
- AgainSystem: 主要的AVG系統Prefab
- AgainSystemOrtho: 一樣是AVG系統Prefab，但相機為正交相機，沒有近大遠小，較容易理解，但運鏡拉近功能(LookAtSpine)無效
- ImageObject: 圖片的Prefab，無須理會
- SpineObject: Spine的Prefab，無須理會
  
## Again System Prefab 中物件介紹
- MAIN
  - AgainSystem Class
    - OnScriptFinish: 一個腳本執行完畢後的回調
    - GoogleSheetId: 腳本的GoogleSheetId
  - 其他Managers，基本上不會動到
- `目前系統開發中，架構可能會小調整，因此暫時不建議修改以下物件，建議等穩定了再改樣式` 
- Stage Canvas
  - 放置場上背景, 圖片, Spine的地方，基本上不會動到
- UI Canvas/Dialogue View
  - AVG的對話框
  - Visible Container底下的物件可以自行調整樣式
  - DialogueView Class
    - TextSize: 對話框文字大小
    - TextSpeed: 文字播放速度
    - WaitSprite: 文字播放中的等待圖示
    - NextSprite: 文字播放完成的圖示
  - UIContainer UIAnimator Class 可以調整Show/Hide的動畫
- UI Canvas/Option Menu View
  - AVG的選項框
  - Button Container底下的按鈕可以自行調整樣式
- UI Canvas/Transfer View
  - 轉場畫面
  - UIContainer UIAnimator Class 可以調整Show/Hide的動畫
- Screen Mask View
  - 黑幕，用來遮蔽左右超過1920的畫面

## AgainSystem Class 
- AgainSystem 是最上層的物件，是一個Singleton，可以透過AgainSystem.Instance取得
- 可用屬性:
  - GoogleSheetId: 腳本的GoogleSheetId 
  - OnScriptFinished: 腳本執行完畢的回調
- 執行腳本
  - ```csharp
      AgainSystem.Instance.Execute(scriptName);
    ```
- 執行指令集
  - ```csharp
      AgainSystem.Instance.RunCommands(commands);
    ```
- 設定語言
  - ```csharp
      AgainSystem.Instance.SetLanguage(language);
    ```
  
## AgainSystem底下Managers介紹
  - 雖然可以直接取得使用，但基本上除了EventManager不太會動到
  - EventManager: 事件處理
  - SpineManager: 管理Spine動畫
  - ImageManager: 管理圖片和背景
  - CameraManager: 處理相機的運鏡
  - DialogueManager: 管理對話框和選像

## 事件處理
  - 腳本有一個Emit指令，執行到時AgainSystem會發出Event
  - 監聽事件方法
    ```csharp
    private void OnEnable()
    {
        AgainSystem.Instance.EventManager.On("EventName", OnEvent);
        AgainSystem.Instance.EventManager.On<List<string>>("EventName", OnEventWithParameters);
    }

    private void OnDisable()
    {
        AgainSystem.Instance.EventManager.Off("EventName", OnEvent);
        AgainSystem.Instance.EventManager.Off<List<string>>("EventName", OnEventWithParameters);
    }

    private void OnEvent()
    {
        // 處理事件
    }

    private void OnEventWithParameters(List<string> parameters)
    {
        // 處理事件
    }
    ```

## 多語言
- AVG多語言的翻譯會放在`Translation`分頁中
- 腳本頁會Say指令會有個參數`Key`，對應到`Translation`中的`Key`
- 可用`AgainSystem.Instance.SetLanguage(Language.Chinese)`切換中文等語言
  
## 改為讀取本地腳本
- 執行`python3 download_script.py`，輸入`GoogleSheetId`下載所有腳本
- 將所有腳本放到`Assets/Resources/TSV`
- 在物件`AgainSystem/MAIN`中設定`GoogleSheetId`為空，即可讀取本地腳本
- 施工中，之後預計做成一鍵下載的執行檔

