using UnityEngine.UI;
using System.Threading.Tasks;
using TMPro;

public class LucasArtText : IMessageTalker
{
    bool skippable;
    bool talking;
    TextMeshProUGUI text;
    bool skipped;

    public LucasArtText(UnityEngine.Transform transform)
    {
        text = transform.GetComponentInChildren<TextMeshProUGUI>();

    }

    public async Task Talk(string message)
    {
        talking = true;
        skipped = false;
        text.text = message;

        TimerEfimero timer = new TimerEfimero();
        timer.ConfigureWithoutQueue(1);
        await timer.Execute();
        
        text.text = "";
        talking = false;
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
        skipped = true;
    }
}
