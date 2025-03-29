## 修正
-   修正Mac從Google Sheet抓資料便亂碼問題
-   更新預設字體，解決一些中文字或全形符號出現亂碼問題
-   修正ShowImage指令的淡入淡出無效功能

## 優化
-   移除doozy，都改用原生的UI
-   將可修改的物件做成prefab，改用prefab生成AVG物件，可到設定面板更換自己的prefab
-   整理Avg個畫面的顯示層級(Order)，可以參考README的層級表
-   執行腳本時Camera改用overlay的方式加到main camera上，解決需要切換開關攝影機的問題
-   ShowSpine, ScaleSpine, ShowImage, ScaleImage 的新增參數ScaleX 和 ScaleY 可以個別縮放
-   背景圖和圖片統一放在 Resources/Images 底下
-   ShowBackground 新增 Type 參數，可以做淡入效果
-   ShowImage 新增 ImageName 參數，處理顯示多個相同物件情況，讓Name表示物件名；ImageName表示圖檔名
-   ShowSpine 新增 SpineName 參數，處理顯示多個相同物件情況，讓Name表示物件名；SpineName表示圖檔名

## 新功能
-   對話新增自動撥放和skip對話功能
-   所有指令新增Join參數，可以和上一個指令同時執行
-   新增 Call 指令，可以執行另一個對話腳本
-   新增 PlaySound指令，可以撥放音效
-   新增 HideBackground 指令，可以關閉或淡出背景
-   新增設定面板，可以從Unity上方 Tools > Again > Setting 開啟

																			