# Again System -- Unity AVG 對話系統

## 目標

- 成為比 Fungus 更好用對話系統
- 精簡化 Fungus，省略一堆用不到的指令和工具
- 對企劃人員友善，支援 Google Sheet 製作對話表，遊戲中可以直接讀取播放
- 對程式人員友善，一個 Prefab 隨放及用，一個一目了然的設定介面

## 需求列表

- 企劃遊戲腳本功能需求
  - [o]顯示人名與文本
  - [o]立繪移動
  - [o]立繪跳一下、放大縮小、移動、震動...等
  - [o]ShakeCamera
  - [o]播放 Spine 動畫
  - [o]切換 Spine 差分
  - [o]等待 Wait (語句講完的 break)
  - [o]進退場的黑幕淡入淡出
  - [o]運鏡 (LookAt 攝影機放大到指定部位-Wait 1.5s-回到原本立繪)
  - [o]切換背景 Background
  - [o]對話框震動
  - [o]文本文字放大縮小
  - [o]立繪明暗切換
- [o]支援 Google Sheet 製作對話表
- [o]支援多語言
- [o]選項選單
- [o]壓住 CTRL 加速對話
- [o]當執行的腳本有錯誤時，能夠在畫面中顯示錯誤
- [x]對話紀錄

## 創建對話腳本流程
- 開啟 [Google Sheet 對話表](https://docs.google.com/spreadsheets/d/1dNdvKzT7IZryEIou0suLthyALAKxjj1Tf9NcbFznWzs/edit#gid=1096039352)
- 複製`Example`分頁，並重新命名
- 到`ConfigList`分頁的腳本列表下，加上新腳本分頁的名字
- 重開遊戲，新增的腳本就會出現在選單中

## 編輯對話腳本流程
- 可以參考`Example`分頁的撰寫方式
- 指令欄選擇要加入的指令
- 角色欄選擇說話或動作的角色
- 當選擇`Say`指令時，內容欄填入對話內容
- 參數欄填入指令的參數，格式為`參數名=參數值`
  - 有哪些參數可以參考`CommandList`分頁
  - `CommandList`分頁的參數欄括號中的值代表該參數的預設值
- `SpineList`,`CommandList`,`ConfigList`分頁(腳本列表除外)，是給程式改動的，企劃無須調整，(改了也不會變出新動畫和功能(X))

## 程式碼架構
- `AgainSystem` - 主要系統，負責管理設定值和所有模組
- `Commands` 
    - `Command` - 指令的基底類別
    - `...Command` - 其他實際指令
- `Components` - 系統核心模組目錄
    - `CameraManager` - 管理攝影機的 Unity Component
    - `DialogueManager` - 管理對話欄的 Unity Component
    - `SpineManager` - 管理 Spine 的 Unity Component
- `GoogleSheet` - Google Sheet模組目錄
    - `GoogleSheetImporter` - 讀取 Google Sheet 資料
    - `GoogleSheetReader` - 解析轉換成Command
    - `SheetSelectView` - 腳本選單介面
   

## 第三方套件
- [DoozyUI](https://assetstore.unity.com/packages/tools/visual-scripting/doozy-ui-manager-249738) - 處理 UI 的進出場和互動動畫
- [DOTween](https://dotween.demigiant.com/) - 一種簡單且高效的方式，用程式碼來創建和管理物件的動畫的工具
- Spine - 2D 骨骼動畫工具