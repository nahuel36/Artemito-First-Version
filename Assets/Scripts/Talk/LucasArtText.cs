using UnityEngine.UI;
using System.Threading.Tasks;
using TMPro;

public class LucasArtText : IMessageTalker
{
    bool skippable;
    bool talking;
    TextMeshProUGUI text;
    bool skipped;
    TimerEfimero currentTimer;
    ITextTimeCalculator timeCalculator;

    public LucasArtText(UnityEngine.Transform transform, ITextTimeCalculator calculator)
    {
        text = transform.GetComponentInChildren<TextMeshProUGUI>();
        timeCalculator = calculator;
    }

    public async Task Talk(string message)
    {
        talking = true;
        skipped = false;
        text.text = message;

        currentTimer = new TimerEfimero();
        currentTimer.ConfigureWithoutQueue(timeCalculator.CalculateTime(message));
        await currentTimer.Execute();
        
        text.text = "";
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
            currentTimer.Cancel();
        }
    }
}
