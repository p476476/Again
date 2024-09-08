# Again System For 程式開發教學

## 安裝
- 下載"XXX.unitypackage"，將其拖入Unity中，將所有檔案匯入
- submodule
- 開啟 `Assets/Again/Scenes/HelloWorld`，執行場景，確認有AVG正常執行
- 額外測試Spine
  - 下載"XXX.unitypackage"，將其匯入(此為測試資源，用完可以刪除)
  -  開啟 `Assets/Again/Scenes/HelloWorldSpine`，執行場景，確認有AVG正常執行

## 新增專案的選端腳本
- 各專案都會有一個自己的GoogleSheet，用來存放腳本
- 開啟[試算表範例腳本](https://docs.google.com/spreadsheets/d/18vCXwuMSR7K0FUEEcHy7X5NzgtyWyUgr7sbeIRW74zs/edit?gid=1492752394#gid=1492752394)，點擊左上檔案>建立副本，複製試算表
- 新的試算表網址中`https://docs.google.com/spreadsheets/d/XXXXX/edit`，`XXXXX`即為`GoogleSheetId`

## 範例
- Scenes/HelloWorld，最簡單的範例，執行HelloWorld腳本
- Scenes/SpriteEditor，可以快速測試已有的腳本(記得要設定GoogleSheetId)

## 新場景使用
- 將Prefab/AgainSystem或Prefab/AgainSystemOrtho則一放入場景中
    - AgainSystemOrtho用的相機為正交相機，沒有近大遠小，較容易理解，但運鏡拉近功能(LookAtSpine)無效
- 點開物件 AgainSystem>Main，Inspector看到Again System並設定GoogleSheetId為剛剛的`GoogleSheetId`貼上
- 開一個空物件，並加上程式碼
  ```csharp
  private void Start()
  {
      // 執行腳本
      // HelloWold為GoogleSheet上其中的一個腳本
      AgainSystem.Instance.Execute("HelloWorld");
  }
  ```
- 執行場景，確認有AVG正常執行

## 資源放置
- 資源要放在對應的資料夾下，系統才找的到圖，否則會直接跳過指令
  - Assets/Resources/Backgrounds，背景圖片
  - Assets/Resources/Images，圖片
  - Assets/Resources/Spines，Spine動畫
  - Assets/Resources/TSV，本地腳本
  
## Again System Prefab物件介紹
- MAIN
  - AgainSystem Class
    - OnScriptFinish: 一個腳本執行完畢後的回調
    - GoogleSheetId: 腳本的GoogleSheetId
  - 其他Manager，基本上不會動到
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
- AgainSystem 是一個Singleton，可以透過AgainSystem.Instance取得
- 可用屬性:
  - GoogleSheetId: 腳本的GoogleSheetId 
  - OnScriptFinished: 腳本執行完畢的回調
- 可用方法:
  - Execute(string scriptName): 執行腳本
  - RunCommands(List<Command> commands): 執行指令集
  - SetLanguage(string language): 設定語言
  
## AgainSystem底下Managers介紹
  - SpineManager: 管理Spine動畫
  - ImageManager: 管理圖片和背景
  - CameraManager: 處理相機的運鏡
  - EventManager: 處理事件
  - DialogueManager: 管理對話框和選像
  - 雖然可以直接取得使用，但基本上除了EventManager, DialogueManager不太會動到
  - `AgainSystem.Instance.DialogueManager`取得DialogueManager設定播放速度等`

## 事件處理
  - 腳本有一個Emit指令，AgainSystem會發出Event
  - 監聽事件方法
    ```csharp
    private void OnEnable()
    {
        AgainSystem.Instance.EventManager.On("EventName", OnEvent);
        AgainSystem.Instance.EventManager.On<List<string>>("EventName", OnEventWithParameters);
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
  
## 改為讀取本地腳本
- 執行`python3 download_script.py`，輸入`GoogleSheetId`下載所有腳本
- 將所有腳本放到`Assets/Resources/TSV`
- 在物件`AgainSystem/MAIN`中設定`GoogleSheetId`為空，即可讀取本地腳本
- 施工中，之後預計做成一鍵下載的執行檔

