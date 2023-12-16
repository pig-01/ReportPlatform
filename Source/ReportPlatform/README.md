# 報表平台建置方案

## 方案分類：三大部分
- **ReportDataSource**：資料庫專案，包含測試資料區(Staging Data)與開發資料區(Development Data)。
    - **Staging Data**：為**客戶**提供之測試資料。
    - **Development Data**: 為**自行**建立的測試資料。
- **ReportPlatformTest**：報表平台測試平台，此部分存放報表平台客戶端測試、報表平台商業邏輯測試(Unit Test)。
- **ReportPlatformWeb**：報表平台網頁服務核心，此部分存放報表平台、報表平台商業邏輯層(Business Layer)、報表平台資料層(Data Layer)。
- **ReportProject**：報表專案核心，與報表伺服器連結與同步報表。
---

## 資料庫建置說明：
- **第一步**：在方案總管中選擇資料庫專案，點擊右鍵選擇[發行]。
- **第二步**：在發行資料庫頁面中選擇目標資料庫，點選[編輯]，並於右面中輸入連線相關訊息。
- **第三步**：點選[確定]後點擊[發行]。
- **第四步**：等候發行完成後於目標資料庫中建立資料表等內容。
- **第五步**：以SSMS中開啟資料庫連線並執行`Scripts\ScriptsIgnoredOnImport.sql`新增資料。

## 報表伺服器建置說明：
- **第一步**：於連結中[下載]{https://www.microsoft.com/zh-tw/download/details.aspx?id=104502}報表伺服器，確認版本為 `16.0.1114.11`。
- **第二步**：安裝 `開發人員(developer)`版本。
- **第三步**：安裝完成後開啟報表伺服器組態管理員[Reporting Service Configuration Manager]。
- **第四步**：服務帳戶使用本機帳戶建立，並[套用]。
- **第五步**：Web服務Url中IP設定為 `127.0.0.1`，TCP連接阜選擇 `8090`，並[套用]。
- **第六步**：建立報表伺服器所需資料庫，並[套用]。
- **第七步**：入口網站URL中點擊[套用]並進入入口網站。
