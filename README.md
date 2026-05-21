# 政治亂鬥 - 專案設定指南

## 前置需求
1. 安裝 **Unity Hub** (https://unity.com/download)
2. 透過 Unity Hub 安裝 **Unity 2022.3.20f1** (或任何 2022 LTS 版本)
3. 安裝時勾選 **Android Build Support** (含 IL2CPP) 和 **Windows Build Support**

## 專案設定步驟

### 1. 開啟專案
- 打開 Unity Hub → Open → Add project from disk
- 選擇 `C:\Users\user\Documents\PoliticalBrawl` 資料夾

### 2. 建立場景結構

#### 主選單場景 (MainMenu)
1. 在 `Assets/Scenes/` 按右鍵 → Create → Scene → 命名 `MainMenu`
2. 建立 Canvas (UI → Canvas)
3. 在 Canvas 下建立：
   - **Text**: "政治亂鬥" (遊戲標題，Font Size 48)
   - **Button**: "開始遊戲" → 掛載 `MainMenuUI.cs`
   - **Button**: "離開遊戲" → 掛載 `MainMenuUI.cs`
4. 建立空物件 `EventSystem`
5. 建立 `CharacterSelectPanel` (Panel) → 掛載 `CharacterSelectUI.cs`
   - 內部建立 Button Template (供 instantiate)
   - Text: 角色名稱、職業說明
   - Button: "確認選擇"

#### 遊戲場景 (GameScene)
1. 在 `Assets/Scenes/` → Create → Scene → 命名 `GameScene`
2. 建立空物件 `GameManager` → 掛載 `GameManager.cs`

3. **建立 InputManager**
   - 空物件 `InputManager` → 掛載 `InputManager.cs`

4. **建立 UI Canvas**
   - 左下方：**Movement Joystick** (Joystick.cs) — 半透明圓形底 + 圓形把手
   - 右下方：**Attack Joystick** (Joystick.cs) — 同上方
   - 左上方：**Health Bar** (Image.fillAmount) + Text (血量數字)
   - 左上方：**Super Bar** (Image.fillAmount) + Text (%數)
   - 右上方：**Super Button** (按鈕，大招滿時亮起)
   - 右上方：**Gadget 1 Button** (隨身科技1)
   - 右上方：**Gadget 2 Button** (隨身科技2)
   - 計時器 Text
   - 掛載 `GameUI.cs`

5. **建立相機**
   - Main Camera → 掛載 `CameraFollow.cs`

6. **建立地圖**
   - 空物件 `Map` → 用 SpriteRenderer 畫出地板
   - 空物件 `SpawnPoints` → 建立多個子物件作為生成點

7. **建立遊戲模式**
   - 空物件 `ShowdownMode` → 掛載 `ShowdownMode.cs`
   - 設定 Map Center / Map Size

8. **建立 BotSpawner**
   - 空物件 `BotSpawner` → 掛載 `BotSpawner.cs`

### 3. 圖形/美術資源（需自行替換）
- `Assets/Art/Sprites/` 放入角色圖片
- 角色可用圓形 + 顏色區分（開發階段）
- 最終換成 Q 版政治人物頭像

### 4. 設定 Build Settings
1. File → Build Settings
2. Add Open Scenes → 加入 MainMenu (index 0) 和 GameScene (index 1)
3. Platform → Android → Switch Platform

## 控制方式

| 操作 | 手機 | 電腦 (開發用) |
|------|------|-------------|
| 移動 | 左搖桿 | WASD / 方向鍵 |
| 攻擊方向 | 右搖桿拖曳 | 滑鼠位置 |
| 攻擊 | 右搖桿啟動 | 滑鼠左鍵 |
| 大招 | Super 按鈕 | Space |
| 隨身科技1 | Gadget1 按鈕 | Q |
| 隨身科技2 | Gadget2 按鈕 | E |

## 角色稀有度與解鎖

| 角色 | 稀有度 | 預設解鎖 |
|------|--------|---------|
| 川普 | 傳奇 | ✅ |
| 習近平 | 傳奇 | ✅ |
| 賴清德 | 神話 | ✅ |
| 蔡英文 | 神話 | ✅ |

## 專案結構

```
Assets/
├─ Scenes/
│  ├─ MainMenu.unity
│  └─ GameScene.unity
├─ Scripts/
│  ├─ Core/
│  │  ├─ GameManager.cs
│  │  ├─ InputManager.cs
│  │  ├─ ProjectileManager.cs
│  │  ├─ SpawnManager.cs
│  │  ├─ CharacterController.cs
│  │  └─ CameraFollow.cs
│  ├─ Characters/
│  │  ├─ BaseCharacter.cs
│  │  ├─ TrumpCharacter.cs
│  │  ├─ XiCharacter.cs
│  │  ├─ LaiCharacter.cs
│  │  └─ TsaiCharacter.cs
│  ├─ Combat/
│  │  ├─ Projectile.cs
│  │  └─ DamageNumber.cs
│  ├─ UI/
│  │  ├─ Joystick.cs
│  │  ├─ HealthBarUI.cs
│  │  ├─ SuperBarUI.cs
│  │  ├─ GameUI.cs
│  │  ├─ CharacterSelectUI.cs
│  │  └─ MainMenuUI.cs
│  ├─ AI/
│  │  ├─ AIBrain.cs
│  │  └─ BotSpawner.cs
│  └─ GameModes/
│     ├─ ShowdownMode.cs
│     └─ PowerUp.cs
├─ Prefabs/
│  ├─ Characters/
│  └─ Projectiles/
└─ Art/
   └─ Sprites/
```
