using System;
using UnityEngine;
using UnityEngine.UI;

public class Element : MonoBehaviour
{
    [SerializeField] private Text position;
    [SerializeField] private Text description;
    [SerializeField] private Image image;

    [SerializeField] private Color[] teamColors;

    public void Propagate(Warrior elem, Warrior valid, int number, int round, bool debug)
    {
        string letter;
        switch (elem.team)
        {
            case Force.Red:
                image.color = teamColors[0];
                letter = "К";
                break;

            case Force.Blue:
                image.color = teamColors[1];
                letter = "С";
                break;

            default:
                throw new Exception("bug");
        }

        position.text = $"# {number + 1}";
        description.text = $"Существо {letter}{elem.number}:\n" +
                           $"Инициатива - {elem.initiative}, Скорость - {elem.speed}";

        {
            // Показываем, насколько корректно мы отрисовали:
            if (!debug) return;

            description.text += $", Коэфициент - {elem.Coefficient(round)}";

            var isSame = elem.IsSame(valid);
            description.text += $" | {isSame}";

            if (!isSame)
            {
                var text = JsonUtility.ToJson(valid);
                description.text += $" | {text}";
            }
        }
    }
}