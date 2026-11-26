using UnityEngine;
using UnityEngine.UIElements;

public class UIHandler : MonoBehaviour
{
    public static UIHandler instance { get; private set; }

    public float displayTime = 4.0f;
    private VisualElement m_NonPlayerDialogue;
    private Label m_DialogueLabel;
    private float m_TimerDisplay;

    private VisualElement m_Healthbar;

    private VisualElement m_WinScreen;
    private VisualElement m_LoseScreen;

    // More things to try: лічильник «Полагоджено x / N»
    private Label m_RobotCounter;

    // Лабораторна 8: залишок часу перемикача
    private Label m_SwitchTimer;

    // Лабораторна 9: стартовий екран — показується лише при першому запуску
    static bool s_StartShown;
    private VisualElement m_StartScreen;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        UIDocument uiDocument = GetComponent<UIDocument>();
        m_Healthbar = uiDocument.rootVisualElement.Q<VisualElement>("HealthBar");
        SetHealthValue(1.0f);

        m_NonPlayerDialogue = uiDocument.rootVisualElement.Q<VisualElement>("NPCDialogue");
        m_DialogueLabel = m_NonPlayerDialogue.Q<Label>();
        m_NonPlayerDialogue.style.display = DisplayStyle.None;
        m_TimerDisplay = -1.0f;

        m_LoseScreen = uiDocument.rootVisualElement.Q<VisualElement>("LoseScreenContainer");
        m_WinScreen = uiDocument.rootVisualElement.Q<VisualElement>("WinScreenContainer");

        m_RobotCounter = uiDocument.rootVisualElement.Q<Label>("RobotCounter");
        UpdateRobotCounter();

        m_SwitchTimer = new Label();
        m_SwitchTimer.name = "SwitchTimer";
        m_SwitchTimer.style.position = Position.Absolute;
        m_SwitchTimer.style.top = 110;
        m_SwitchTimer.style.right = 30;
        m_SwitchTimer.style.fontSize = 36;
        m_SwitchTimer.style.color = new Color(1f, 0.85f, 0.3f);
        m_SwitchTimer.style.unityFontStyleAndWeight = FontStyle.Bold;
        m_SwitchTimer.style.backgroundColor = new Color(0, 0, 0, 0.45f);
        m_SwitchTimer.style.paddingLeft = 20;
        m_SwitchTimer.style.paddingRight = 20;
        m_SwitchTimer.style.display = DisplayStyle.None;
        uiDocument.rootVisualElement.Add(m_SwitchTimer);

        if (!s_StartShown)
        {
            ShowStartScreen(uiDocument.rootVisualElement);
        }
    }

    void ShowStartScreen(VisualElement root)
    {
        Time.timeScale = 0f;
        m_StartScreen = new VisualElement();
        m_StartScreen.style.position = Position.Absolute;
        m_StartScreen.style.top = 0; m_StartScreen.style.bottom = 0;
        m_StartScreen.style.left = 0; m_StartScreen.style.right = 0;
        m_StartScreen.style.backgroundColor = new Color(0.08f, 0.1f, 0.16f, 0.92f);
        m_StartScreen.style.alignItems = Align.Center;
        m_StartScreen.style.justifyContent = Justify.Center;

        var title = new Label("Robot Repair: Затоплений склад");
        title.style.fontSize = 72;
        title.style.color = new Color(1f, 0.85f, 0.3f);
        title.style.unityFontStyleAndWeight = FontStyle.Bold;
        m_StartScreen.Add(title);

        var goal = new Label("Полагодь усіх зламаних роботів шестернями і не втрать здоров'я.\nСтрілки — рух, C — постріл, X — розмова.");
        goal.style.fontSize = 34;
        goal.style.color = Color.white;
        goal.style.unityTextAlign = TextAnchor.MiddleCenter;
        goal.style.marginTop = 20; goal.style.marginBottom = 40;
        m_StartScreen.Add(goal);

        var play = new Button(HideStartScreen) { text = "Грати (Enter)" };
        play.style.fontSize = 40;
        play.style.paddingLeft = 40; play.style.paddingRight = 40;
        play.style.paddingTop = 10; play.style.paddingBottom = 10;
        m_StartScreen.Add(play);

        root.Add(m_StartScreen);
    }

    void HideStartScreen()
    {
        s_StartShown = true;
        Time.timeScale = 1f;
        if (m_StartScreen != null)
        {
            m_StartScreen.RemoveFromHierarchy();
            m_StartScreen = null;
        }
    }

    public void SetSwitchTimer(float secondsLeft)
    {
        if (m_SwitchTimer == null) return;
        if (secondsLeft <= 0)
        {
            m_SwitchTimer.style.display = DisplayStyle.None;
            return;
        }
        m_SwitchTimer.style.display = DisplayStyle.Flex;
        m_SwitchTimer.text = "Двері відчинені: " + secondsLeft.ToString("0.0") + " с";
    }

    public void SetHealthValue(float percentage)
    {
        m_Healthbar.style.width = Length.Percent(100 * percentage);
    }

    private void Update()
    {
        if (m_StartScreen != null && UnityEngine.InputSystem.Keyboard.current != null
            && UnityEngine.InputSystem.Keyboard.current.enterKey.wasPressedThisFrame)
        {
            HideStartScreen();
        }

        if (m_TimerDisplay > 0)
        {
            m_TimerDisplay -= Time.deltaTime;
            if (m_TimerDisplay < 0)
            {
                m_NonPlayerDialogue.style.display = DisplayStyle.None;
            }
        }
    }

    public void DisplayDialogue()
    {
        m_NonPlayerDialogue.style.display = DisplayStyle.Flex;
        m_TimerDisplay = displayTime;
    }

    // More things to try: у кожного NPC своя репліка
    public void DisplayDialogue(string text)
    {
        if (m_DialogueLabel != null && !string.IsNullOrEmpty(text))
        {
            m_DialogueLabel.text = text;
        }
        DisplayDialogue();
    }

    public void DisplayWinScreen()
    {
        m_WinScreen.style.opacity = 1.0f;
    }

    public void DisplayLoseScreen()
    {
        m_LoseScreen.style.opacity = 1.0f;
    }

    public void UpdateRobotCounter()
    {
        if (m_RobotCounter == null) return;

        EnemyController[] enemies = FindObjectsByType<EnemyController>(FindObjectsSortMode.None);
        int fixedCount = 0;
        foreach (EnemyController enemy in enemies)
        {
            if (!enemy.isBroken) fixedCount++;
        }
        m_RobotCounter.text = "Полагоджено " + fixedCount + " / " + enemies.Length;
    }
}
