using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class UI : MonoBehaviour
{
    public UnityEvent Roll;
    UIDocument document;
    Label opponentScore;
    Label yourScore;
    Label result;
    IntegerField countOfDice;
    Button roll;

    private void Start()
    {
        document = GetComponent<UIDocument>();
        opponentScore = document.rootVisualElement.Q<Label>("Opponent");
        yourScore = document.rootVisualElement.Q<Label>("Your");
        result = document.rootVisualElement.Q<Label>("Result");
        countOfDice = document.rootVisualElement.Q<IntegerField>("Count");
        roll = document.rootVisualElement.Q<Button>("Roll");

        roll.clicked += Roll.Invoke;

        countOfDice.RegisterValueChangedCallback(evt =>
        {
            int count = Mathf.Clamp(evt.newValue, 1, 100);

            RollDice.Instance.DiceCount = (uint)count;
        });

        RollDice.Instance.StartRoll += UpdateUIStart;
        RollDice.Instance.EndRoll += UpdateUIEnd;
    }

    private void UpdateUIStart()
    {
        opponentScore.text = $"Opponent's Score: {WinCondition.Instance.Condition}";
        yourScore.text = $"Your Score: ...";
        result.text = $"Result: ...";
    }

    private void UpdateUIEnd()
    {
        yourScore.text = $"Your Score: {RollDice.Instance.Result}";
        result.text = $"Result: {WinCondition.Instance.Result}";
    }
}
