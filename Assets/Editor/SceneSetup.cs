using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.IO;

public static class SceneSetup
{
    [MenuItem("政治亂鬥/Setup All Scenes")]
    public static void SetupAllScenes()
    {
        SetupMainMenu();
        SetupGameScene();

        // Add scenes to build settings
        var buildScenes = new EditorBuildSettingsScene[2];
        buildScenes[0] = new EditorBuildSettingsScene("Assets/Scenes/MainMenu.unity", true);
        buildScenes[1] = new EditorBuildSettingsScene("Assets/Scenes/GameScene.unity", true);
        EditorBuildSettings.scenes = buildScenes;

        Debug.Log("✅ 政治亂鬥場景設定完成！");
    }

    static void SetupMainMenu()
    {
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        EditorSceneManager.SaveScene(scene, "Assets/Scenes/MainMenu.unity");

        GameObject eventSystem = new GameObject("EventSystem");
        eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
        eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();

        GameObject canvasObj = new GameObject("Canvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasObj.AddComponent<GraphicRaycaster>();

        // Title
        GameObject titleObj = CreateUIText("Title", canvasObj.transform, "政治亂鬥", 48, TextAnchor.MiddleCenter);
        RectTransform titleRt = titleObj.GetComponent<RectTransform>();
        titleRt.anchorMin = new Vector2(0.5f, 0.7f);
        titleRt.anchorMax = new Vector2(0.5f, 0.7f);
        titleRt.sizeDelta = new Vector2(400, 80);
        titleRt.anchoredPosition = Vector2.zero;

        // Start button
        GameObject startBtnObj = CreateUIButton("StartButton", canvasObj.transform, "開始遊戲", new Vector2(0, -60));
        Button startBtn = startBtnObj.GetComponent<Button>();
        Text startText = startBtnObj.GetComponentInChildren<Text>();
        if (startText != null) startText.fontSize = 28;

        // Quit button
        GameObject quitBtnObj = CreateUIButton("QuitButton", canvasObj.transform, "離開遊戲", new Vector2(0, -140));
        Button quitBtn = quitBtnObj.GetComponent<Button>();
        Text quitText = quitBtnObj.GetComponentInChildren<Text>();
        if (quitText != null) quitText.fontSize = 28;

        // CharacterSelectPanel
        GameObject selectPanel = new GameObject("CharacterSelectPanel");
        selectPanel.transform.SetParent(canvasObj.transform, false);
        RectTransform panelRt = selectPanel.AddComponent<RectTransform>();
        panelRt.anchorMin = Vector2.zero;
        panelRt.anchorMax = Vector2.one;
        panelRt.sizeDelta = Vector2.zero;
        Image panelBg = selectPanel.AddComponent<Image>();
        panelBg.color = new Color(0.1f, 0.1f, 0.1f, 0.95f);
        selectPanel.SetActive(false);

        // Character button template
        GameObject charBtnTemplate = CreateUIButton("CharBtnTemplate", selectPanel.transform, "角色", new Vector2(-200, 100));
        charBtnTemplate.SetActive(false);

        // Selected info text
        GameObject infoText = CreateUIText("InfoText", selectPanel.transform, "選擇一個角色", 24, TextAnchor.MiddleLeft);
        RectTransform infoRt = infoText.GetComponent<RectTransform>();
        infoRt.anchorMin = new Vector2(0.5f, 0.5f);
        infoRt.anchorMax = new Vector2(0.5f, 0.5f);
        infoRt.anchoredPosition = new Vector2(100, 60);
        infoRt.sizeDelta = new Vector2(300, 120);

        // Confirm button
        GameObject confirmBtn = CreateUIButton("ConfirmButton", selectPanel.transform, "確認選擇", new Vector2(100, -100));
        Button confirmBtnComp = confirmBtn.GetComponent<Button>();
        confirmBtnComp.interactable = false;

        // MainMenuUI script
        MainMenuUI mainMenu = canvasObj.AddComponent<MainMenuUI>();
        mainMenu.startButton = startBtn;
        mainMenu.quitButton = quitBtn;
        mainMenu.characterSelectPanel = selectPanel;

        // CharacterSelectUI script
        CharacterSelectUI charSelect = selectPanel.AddComponent<CharacterSelectUI>();
        charSelect.characterButtonPrefab = charBtnTemplate;
        charSelect.buttonContainer = selectPanel.transform;
        charSelect.confirmButton = confirmBtnComp;
        charSelect.selectedNameText = infoText.GetComponent<Text>();
        charSelect.selectedClassText = infoText.GetComponent<Text>();

        EditorSceneManager.SaveScene(scene, "Assets/Scenes/MainMenu.unity");
    }

    static void SetupGameScene()
    {
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        EditorSceneManager.SaveScene(scene, "Assets/Scenes/GameScene.unity");

        // Camera
        Camera cam = Camera.main;
        if (cam == null)
        {
            GameObject camObj = new GameObject("Main Camera");
            cam = camObj.AddComponent<Camera>();
            camObj.tag = "MainCamera";
        }
        cam.orthographic = true;
        cam.orthographicSize = 12f;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0.2f, 0.25f, 0.2f);
        CameraFollow camFollow = cam.gameObject.AddComponent<CameraFollow>();

        // GameManager
        GameObject gmObj = new GameObject("GameManager");
        GameManager gm = gmObj.AddComponent<GameManager>();
        gm.maxPlayers = 6;

        // InputManager
        GameObject imObj = new GameObject("InputManager");
        imObj.AddComponent<InputManager>();

        // ShowdownMode
        GameObject sdObj = new GameObject("ShowdownMode");
        ShowdownMode sd = sdObj.AddComponent<ShowdownMode>();
        sd.mapCenter = Vector2.zero;
        sd.mapSize = new Vector2(24f, 24f);

        // BotSpawner
        GameObject botObj = new GameObject("BotSpawner");
        BotSpawner bs = botObj.AddComponent<BotSpawner>();
        bs.botCount = 5;

        // SpawnManager
        GameObject smObj = new GameObject("SpawnManager");
        smObj.AddComponent<SpawnManager>();

        // SpawnPoints
        GameObject spawnPoints = new GameObject("SpawnPoints");
        spawnPoints.transform.SetParent(gmObj.transform);
        Vector3[] spawnPositions = new Vector3[]
        {
            new Vector3(-8, -8, 0), new Vector3(8, -8, 0),
            new Vector3(-8, 8, 0), new Vector3(8, 8, 0),
            new Vector3(0, -10, 0), new Vector3(0, 10, 0)
        };
        gm.spawnPoints = new Transform[spawnPositions.Length];
        for (int i = 0; i < spawnPositions.Length; i++)
        {
            GameObject sp = new GameObject($"SpawnPoint_{i}");
            sp.transform.SetParent(spawnPoints.transform);
            sp.transform.position = spawnPositions[i];
            gm.spawnPoints[i] = sp.transform;

            SpriteRenderer sr = sp.AddComponent<SpriteRenderer>();
            sr.color = Color.cyan;
        }

        // PowerUp spawn points
        GameObject puSpawnPoints = new GameObject("PowerUpSpawnPoints");
        puSpawnPoints.transform.SetParent(smObj.transform);
        smObj.GetComponent<SpawnManager>().powerUpSpawnPoints = new Transform[4];
        Vector3[] puPositions = new Vector3[]
        {
            new Vector3(-5, 0, 0), new Vector3(5, 0, 0),
            new Vector3(0, -5, 0), new Vector3(0, 5, 0)
        };
        for (int i = 0; i < puPositions.Length; i++)
        {
            GameObject sp = new GameObject($"PUSpawn_{i}");
            sp.transform.SetParent(puSpawnPoints.transform);
            sp.transform.position = puPositions[i];
            smObj.GetComponent<SpawnManager>().powerUpSpawnPoints[i] = sp.transform;
        }

        // Canvas
        GameObject canvasObj = new GameObject("Canvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080, 1920);
        canvasObj.AddComponent<GraphicRaycaster>();

        GameUI gameUI = canvasObj.AddComponent<GameUI>();

        // EventSystem
        GameObject es = new GameObject("EventSystem");
        es.AddComponent<UnityEngine.EventSystems.EventSystem>();
        es.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();

        // --- Movement Joystick (bottom-left) ---
        GameObject moveJoystick = CreateJoystick("MovementJoystick", canvasObj.transform,
            new Vector2(180, 180), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0.15f, 0.15f));
        gameUI.movementJoystick = moveJoystick.GetComponent<Joystick>();

        // --- Attack Joystick (bottom-right) ---
        GameObject attackJoystick = CreateJoystick("AttackJoystick", canvasObj.transform,
            new Vector2(180, 180), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0.85f, 0.15f));
        gameUI.attackJoystick = attackJoystick.GetComponent<Joystick>();

        // --- Super Button ---
        GameObject superBtn = CreateUIButton("SuperButton", canvasObj.transform, "SUPER", new Vector2(-120, 120));
        RectTransform superRt = superBtn.GetComponent<RectTransform>();
        superRt.anchorMin = new Vector2(1, 0);
        superRt.anchorMax = new Vector2(1, 0);
        superRt.pivot = new Vector2(0.5f, 0.5f);
        superRt.sizeDelta = new Vector2(80, 80);
        Text superText = superBtn.GetComponentInChildren<Text>();
        if (superText != null) { superText.text = "SUPER"; superText.fontSize = 14; superText.color = Color.yellow; }
        Image superImage = superBtn.GetComponent<Image>();
        if (superImage != null) superImage.color = new Color(0.8f, 0.6f, 0f, 0.8f);
        gameUI.superButton = superBtn.GetComponent<Button>();

        // --- Gadget1 Button ---
        GameObject g1Btn = CreateUIButton("Gadget1Button", canvasObj.transform, "G1", new Vector2(-60, 120));
        RectTransform g1Rt = g1Btn.GetComponent<RectTransform>();
        g1Rt.anchorMin = new Vector2(1, 0);
        g1Rt.anchorMax = new Vector2(1, 0);
        g1Rt.pivot = new Vector2(0.5f, 0.5f);
        g1Rt.sizeDelta = new Vector2(60, 60);
        Text g1Text = g1Btn.GetComponentInChildren<Text>();
        if (g1Text != null) { g1Text.text = "G1"; g1Text.fontSize = 12; }
        gameUI.gadget1Button = g1Btn.GetComponent<Button>();

        // --- Gadget2 Button ---
        GameObject g2Btn = CreateUIButton("Gadget2Button", canvasObj.transform, "G2", new Vector2(0, 120));
        RectTransform g2Rt = g2Btn.GetComponent<RectTransform>();
        g2Rt.anchorMin = new Vector2(1, 0);
        g2Rt.anchorMax = new Vector2(1, 0);
        g2Rt.pivot = new Vector2(0.5f, 0.5f);
        g2Rt.sizeDelta = new Vector2(60, 60);
        Text g2Text = g2Btn.GetComponentInChildren<Text>();
        if (g2Text != null) { g2Text.text = "G2"; g2Text.fontSize = 12; }
        gameUI.gadget2Button = g2Btn.GetComponent<Button>();

        // --- Health Bar (top-left) ---
        GameObject hpBar = CreateUIBar("HealthBar", canvasObj.transform, Color.red,
            new Vector2(0.02f, 0.95f), new Vector2(0, 0), new Vector2(200, 24));
        gameUI.healthBarFill = hpBar.transform.Find("Fill").GetComponent<Image>();

        // --- Super Bar (below health) ---
        GameObject spBar = CreateUIBar("SuperBar", canvasObj.transform, Color.yellow,
            new Vector2(0.02f, 0.91f), new Vector2(0, 0), new Vector2(200, 16));
        gameUI.superBarFill = spBar.transform.Find("Fill").GetComponent<Image>();

        // --- Health Text ---
        GameObject hpText = CreateUIText("HPText", canvasObj.transform, "", 16, TextAnchor.MiddleLeft);
        RectTransform hpTextRt = hpText.GetComponent<RectTransform>();
        hpTextRt.anchorMin = new Vector2(0, 1);
        hpTextRt.anchorMax = new Vector2(0, 1);
        hpTextRt.pivot = new Vector2(0, 1);
        hpTextRt.anchoredPosition = new Vector2(210, -22);
        hpTextRt.sizeDelta = new Vector2(150, 24);
        gameUI.healthText = hpText.GetComponent<Text>();

        // --- Timer (top-center) ---
        GameObject timerText = CreateUIText("TimerText", canvasObj.transform, "03:00", 36, TextAnchor.MiddleCenter);
        RectTransform timerRt = timerText.GetComponent<RectTransform>();
        timerRt.anchorMin = new Vector2(0.5f, 1);
        timerRt.anchorMax = new Vector2(0.5f, 1);
        timerRt.pivot = new Vector2(0.5f, 1);
        timerRt.anchoredPosition = new Vector2(0, -20);
        timerRt.sizeDelta = new Vector2(120, 40);
        gameUI.countdownText = timerText.GetComponent<Text>();

        // --- GameOver Panel ---
        GameObject goPanel = new GameObject("GameOverPanel");
        goPanel.transform.SetParent(canvasObj.transform, false);
        RectTransform goRt = goPanel.AddComponent<RectTransform>();
        goRt.anchorMin = Vector2.zero;
        goRt.anchorMax = Vector2.one;
        goRt.sizeDelta = Vector2.zero;
        Image goBg = goPanel.AddComponent<Image>();
        goBg.color = new Color(0, 0, 0, 0.7f);
        goPanel.SetActive(false);
        gameUI.gameOverPanel = goPanel;

        GameObject goText = CreateUIText("GameOverText", goPanel.transform, "遊戲結束", 48, TextAnchor.MiddleCenter);
        RectTransform goTextRt = goText.GetComponent<RectTransform>();
        goTextRt.anchorMin = Vector2.zero;
        goTextRt.anchorMax = Vector2.one;
        goTextRt.sizeDelta = Vector2.zero;
        gameUI.gameOverText = goText.GetComponent<Text>();

        EditorSceneManager.SaveScene(scene, "Assets/Scenes/GameScene.unity");
    }

    static GameObject CreateUIText(string name, Transform parent, string text, int fontSize, TextAnchor alignment)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        RectTransform rt = obj.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(200, 40);
        Text txt = obj.AddComponent<Text>();
        txt.text = text;
        txt.fontSize = fontSize;
        txt.alignment = alignment;
        txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        txt.color = Color.white;
        return obj;
    }

    static GameObject CreateUIButton(string name, Transform parent, string text, Vector2 anchoredPos)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        RectTransform rt = obj.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = new Vector2(200, 50);

        Image img = obj.AddComponent<Image>();
        img.color = new Color(0.2f, 0.3f, 0.5f);

        Button btn = obj.AddComponent<Button>();
        btn.targetGraphic = img;
        btn.colors = new ColorBlock
        {
            normalColor = new Color(0.2f, 0.3f, 0.5f),
            highlightedColor = new Color(0.3f, 0.4f, 0.6f),
            pressedColor = new Color(0.1f, 0.2f, 0.4f),
            selectedColor = new Color(0.3f, 0.4f, 0.6f),
            disabledColor = Color.gray,
            colorMultiplier = 1,
            fadeDuration = 0.1f
        };

        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(obj.transform, false);
        RectTransform textRt = textObj.AddComponent<RectTransform>();
        textRt.anchorMin = Vector2.zero;
        textRt.anchorMax = Vector2.one;
        textRt.sizeDelta = Vector2.zero;
        Text txt = textObj.AddComponent<Text>();
        txt.text = text;
        txt.fontSize = 20;
        txt.alignment = TextAnchor.MiddleCenter;
        txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        txt.color = Color.white;

        return obj;
    }

    static GameObject CreateJoystick(string name, Transform parent, Vector2 size, Vector2 anchorMin, Vector2 anchorMax, Vector2 anchorPos)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent, false);

        RectTransform rt = obj.AddComponent<RectTransform>();
        rt.anchorMin = anchorPos;
        rt.anchorMax = anchorPos;
        rt.pivot = anchorPos;
        rt.sizeDelta = size;
        rt.anchoredPosition = Vector2.zero;

        Image img = obj.AddComponent<Image>();
        img.color = new Color(1, 1, 1, 0.2f);
        img.raycastTarget = true;

        // Handle
        GameObject handle = new GameObject("Handle");
        handle.transform.SetParent(obj.transform, false);
        RectTransform handleRt = handle.AddComponent<RectTransform>();
        handleRt.sizeDelta = size * 0.4f;
        handleRt.anchoredPosition = Vector2.zero;
        Image handleImg = handle.AddComponent<Image>();
        handleImg.color = new Color(1, 1, 1, 0.5f);

        Joystick joystick = obj.AddComponent<Joystick>();
        joystick.handle = handleRt;
        joystick.handleRange = size.x * 0.4f;

        return obj;
    }

    static GameObject CreateUIBar(string name, Transform parent, Color fillColor, Vector2 anchorPos, Vector2 anchorMin, Vector2 size)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent, false);

        RectTransform rt = obj.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(anchorPos.x, anchorPos.y);
        rt.anchorMax = new Vector2(anchorPos.x + anchorMin.x, anchorPos.y + anchorMin.y);
        rt.pivot = new Vector2(0, 1);
        rt.sizeDelta = new Vector2(size.x, size.y);
        rt.anchoredPosition = new Vector2(0, 0);

        Image bg = obj.AddComponent<Image>();
        bg.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);

        GameObject fillObj = new GameObject("Fill");
        fillObj.transform.SetParent(obj.transform, false);
        RectTransform fillRt = fillObj.AddComponent<RectTransform>();
        fillRt.anchorMin = Vector2.zero;
        fillRt.anchorMax = Vector2.one;
        fillRt.sizeDelta = Vector2.zero;
        Image fillImg = fillObj.AddComponent<Image>();
        fillImg.color = fillColor;
        fillImg.type = Image.Type.Filled;
        fillImg.fillMethod = Image.FillMethod.Horizontal;
        fillImg.fillAmount = 1f;

        return obj;
    }
}
