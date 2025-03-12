using System;

[Serializable]
public class SaveData
{
    public bool bot_a;
    public bool bot_b;
    public bool bot_c;

    public SaveData()
    {
        bot_a = false;
        bot_b = false;
        bot_c = false;
    }

    public void Reset()
    {
        bot_a = false;
        bot_b = false;
        bot_c = false;
    }
}
