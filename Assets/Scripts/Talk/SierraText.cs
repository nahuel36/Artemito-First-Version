using UnityEngine.UI;
using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;
using TMPro;

public class SierraText : IMessageTalker
{
    bool skippable;
    bool talking;
    TextMeshProUGUI textmesh;
    bool skipped;
    TimerEfimero currentTimer;
    ITextTimeCalculator timeCalculator;
    public string Text { get { return textmesh.text; } set { textmesh.text = value; } }
    Image image;
    Image panel;
    public SierraText(UnityEngine.Transform transform, ITextTimeCalculator calculator, Sprite sprite)
    {
        //pasar offset y tamaño de letra por parametro

        UnityEngine.GameObject canvasGO = new UnityEngine.GameObject("canvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.worldCamera = Camera.main;
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        CanvasScaler scaler = canvas.gameObject.AddComponent<CanvasScaler>();
        canvasGO.transform.parent = null;



        UnityEngine.GameObject imageGO = new UnityEngine.GameObject("sprite");
        imageGO.transform.parent = canvasGO.transform;
        image = imageGO.AddComponent<Image>();
        image.sprite = sprite;
        image.rectTransform.sizeDelta = new Vector2(3f, 3f);
        image.rectTransform.anchorMax = Vector2.zero;
        image.rectTransform.anchorMin = Vector2.zero;
        image.rectTransform.pivot = Vector2.zero;
        image.enabled = false;

        UnityEngine.GameObject panelGO = new UnityEngine.GameObject("panel");
        panelGO.transform.parent = canvasGO.transform;
        panel = panelGO.AddComponent<Image>();
        panel.color = Color.gray;
        panel.rectTransform.sizeDelta = new Vector2(10f, 3f);
        panel.rectTransform.anchorMax = new Vector2(0.5f,0);
        panel.rectTransform.anchorMin = new Vector2(0.5f, 0);
        panel.rectTransform.pivot = new Vector2(0.5f, 0);
        panel.enabled = false;

        UnityEngine.GameObject textGO = new UnityEngine.GameObject("text");
        textGO.transform.parent = panelGO.transform;
        textGO.transform.localPosition = Vector3.zero;
        textmesh = textGO.transform.gameObject.AddComponent<TextMeshProUGUI>();
        textmesh.fontSize = 0.45f;
        textmesh.horizontalAlignment = HorizontalAlignmentOptions.Center;
        textmesh.verticalAlignment = VerticalAlignmentOptions.Middle;
        textmesh.rectTransform.sizeDelta = new Vector2(10, 3);


        textmesh.transform.SetAsLastSibling();

        timeCalculator = calculator;
    }

    public async Task Talk(string message)
    {
        panel.enabled = true;
        image.enabled = true;
        talking = true;
        skipped = false;
        Text = message;

        currentTimer = new TimerEfimero();
        currentTimer.ConfigureWithoutQueue(timeCalculator.CalculateTime(message));
        await currentTimer.Execute();
        
        Text = "";
        talking = false;
        image.enabled = false;
        panel.enabled = false;
        currentTimer = null;
    }

    // Start is called before the first frame update
    public void Talk(string message, bool skippable)
    {
        this.skippable = skippable;
        Talk(message);
    }

    public bool Talking { get { return talking; } }

    public bool Skipped { get { return skippable && skipped; } }

    public void Skip() {
        if (skippable && currentTimer != null)
        {
            skipped = true;
            currentTimer.Skip();
        }
    }
}
