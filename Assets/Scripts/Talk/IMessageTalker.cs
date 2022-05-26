using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMessageTalker
{
    // Start is called before the first frame update
    void Talk(string message, bool skippable);
    bool Talking { get; }
    void Skip();
    bool Skipped { get; }
}
