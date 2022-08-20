using UnityEngine.UI;
using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;
using TMPro;

public class LucasArtText : IMessageTalker
{
    bool skippable;
    bool talking;
    TextMeshProUGUI textmesh;
    bool skipped;
    TimerEfimero currentTimer;
    ITextTimeCalculator timeCalculator;
    public string Text { get { return textmesh.text; } set { textmesh.text = value; } }


    public LucasArtText(UnityEngine.Transform transform, ITextTimeCalculator calculator)
    {
        //pasar offset y tamaño de letra por parametro

        UnityEngine.GameObject canvasGO = new UnityEngine.GameObject("canvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.worldCamera = Camera.main;
        canvas.renderMode = RenderMode.WorldSpace;
        CanvasScaler scaler = canvas.gameObject.AddComponent<CanvasScaler>();
        canvasGO.transform.parent = transform;

        UnityEngine.GameObject textGO = new UnityEngine.GameObject("text");
        textGO.transform.parent = canvasGO.transform;
        textmesh = textGO.transform.gameObject.AddComponent<TextMeshProUGUI>();
        textmesh.transform.position = transform.position + new Vector3(0,1.45f,0);
        textmesh.fontSize = 0.4f;
        textmesh.horizontalAlignment = HorizontalAlignmentOptions.Center;
        textmesh.verticalAlignment = VerticalAlignmentOptions.Middle;
        textmesh.rectTransform.sizeDelta = new Vector2(10, 5);

        timeCalculator = calculator;
    }

    public async Task Talk(string message)
    {
        talking = true;
        skipped = false;
        Text = message;

        currentTimer = new TimerEfimero();
        currentTimer.ConfigureWithoutQueue(timeCalculator.CalculateTime(message));
        await currentTimer.Execute();
        
        Text = "";
        talking = false;
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
