using UnityEngine;

public class WinCondition : MonoBehaviour
{
    public static WinCondition Instance { get; private set; }
    public int Condition { get; private set; }
    public string Result { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        RollDice.Instance.StartRoll += GetCondition;
        RollDice.Instance.EndRoll += GetResult;
    }

    void GetCondition()
    {
        var count = RollDice.Instance.DiceCount;
        Result = "";

        int opponentScore = 0;

        for (int i = 0; i < RollDice.Instance.DiceCount; i++)
        {
            opponentScore += Random.Range(1, 7);
        }

        Condition = opponentScore;
    }

    void GetResult()
    {
        if (Condition > RollDice.Instance.Result) Result = "Failed";
        else if (Condition == RollDice.Instance.Result) Result = "Draw";
        else Result = "Win";
    }
}
