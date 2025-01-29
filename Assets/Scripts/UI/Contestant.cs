using System.Collections;
using TMPro;
using UnityEngine;

public class Contestant : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] private float animationDuration = 1f;
    [SerializeField] private Vector2 startOffset = Vector2.right * 100;
    [SerializeField] private AnimationCurve animationCurve = AnimationCurve.Constant(0, 1, 1);

    [Header("UI Elements")]
    [SerializeField] private RectTransform container;
    [SerializeField] private CanvasGroup containerCanvasGroup;
    [SerializeField] private TextMeshProUGUI contestantNameText;
    [SerializeField] private TextMeshProUGUI contestantPlacementText;

    public void SetContestantName(string name)
    {
        contestantNameText.text = name;
    }

    public void SetPlacement(int placement)
    {
        contestantPlacementText.text = placement.ToString();

        // Handle special cases for 11th, 12th, and 13th
        if (placement / 10 >= 1 && placement / 10 < 2)
        {
            contestantPlacementText.text += "th";
            return;
        }

        contestantPlacementText.text += (placement % 10) switch
        {
            1 => "st",
            2 => "nd",
            3 => "rd",
            _ => "th",
        };
    }

    public void SetIsYou(bool isYou)
    {
        if (isYou)
        {
            contestantNameText.fontStyle = FontStyles.Bold;
            contestantNameText.color = Color.green;

            contestantPlacementText.fontStyle = FontStyles.Bold;
            contestantPlacementText.color = Color.green;
        }
    }

    public void Animate()
    {
        // Play animation
        StartCoroutine(Animation(animationDuration));
    }

    private IEnumerator Animation(float animationDuration)
    {
        // Go from the right, to the left while fading in, the movement should follow the animation curve
        float timer = 0f;
        Vector2 startPosition = startOffset;
        Vector2 endPosition = Vector2.zero;

        while (timer < animationDuration)
        {
            float progress = timer / animationDuration;
            container.anchoredPosition = Vector2.Lerp(startPosition, endPosition, animationCurve.Evaluate(progress));
            containerCanvasGroup.alpha = progress;
            timer += Time.deltaTime;
            yield return null;
        }
        container.anchoredPosition = endPosition;

        // TODO: Make a sound and shake the camera a bit
    }
}
