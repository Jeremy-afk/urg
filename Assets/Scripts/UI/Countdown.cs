using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Countdown : MonoBehaviour
{
    [SerializeField] private bool activateOnAwake = false;

    [Header("General parameters")]
    [Range(1, 10)]
    [SerializeField] private int countdownDuration = 3;
    [Tooltip("Represents a second of this countdown.")]
    [SerializeField] private float countdownUnitMultiplier = 1f;

    [Header("Digit Animation")]
    [Tooltip("If true, displays the integer part of the remaining time instead of the integer part + 1." +
        "\nThis option is not very common tho. Does not affect the event timing." +
        "\nThe zero WILL show up, but can be replaced by stringToDisplayInsteadOfZero.")]
    [SerializeField] private bool displayIntegerPart = false;
    [Tooltip("If true, displays the zero at the end of the countdown." +
        "\nDoes not affect the event timing." +
        "\nIgnored if displayIntegerPart is true.")]
    [SerializeField] private bool displayZero = true;
    [Tooltip("Prints this at the screen instead of printing a zero." +
        "\nA zero must appear for it to be replaced by this string.")]
    [SerializeField] private string stringToDisplayInsteadOfZero;
    [Tooltip("Not affected by the countdownUnitMultiplier. Does not affect the event timing.")]
    [SerializeField] private float digitDuration = 1.2f;
    [Tooltip("Controls how much the digit animation begins in advance compared to when its actual integer part gets past on the timer" +
        "\nFor example, 0.5 is great if the digit animation has a 'peak' around its middle and you want to match that peak with the event." +
        "\nAdditionnally, 0 is approriate if the timing between the beginning of the countdown and the event must match exactly the countdown timer." +
        "\nDoes affect the event timing by adding this amount to the total countdown duration (weighted by countdownUnitMultiplier)."), Range(0f, 1f)]
    [SerializeField] private float digitOffset = 0.5f;
    [SerializeField] private AnimationCurve digitAlpha = AnimationCurve.Constant(0, 1, 1);
    [SerializeField] private AnimationCurve digitScale = AnimationCurve.Constant(0, 1, 1);
    [SerializeField] private float scaleMultiplier = 3f;

    [Header("References")]
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private UnityEvent onCountdownBegin;
    [SerializeField] private UnityEvent onCountdownFinished;

    private List<TextMeshProUGUI> digits = new();

    private bool isCountingDown = false;
    private int textIndex = 0;

    private void Awake()
    {
        digits.Clear();
        digits.Add(countdownText);

        // Hide all the digits
        image.gameObject.SetActive(false);
        HideDigits();

        if (activateOnAwake)
        {
            StartCountdown();
        }
    }

    public void SetDuration(int duration)
    {
        countdownDuration = duration;
    }

    public void StartCountdown(Action callback = null)
    {
        if (isCountingDown)
        {
            Debug.LogWarning("Countdown already in progress.");
            return;
        }
        isCountingDown = true;
        PopulateNecessaryTexts();
        StartCoroutine(CountdownCoroutine(callback));
    }

    private IEnumerator CountdownCoroutine(Action callback)
    {
        textIndex = 0;
        image.gameObject.SetActive(true);
        onCountdownBegin.Invoke();

        // Typically, begins at 3.
        int count = countdownDuration;

        while (count > 0)
        {
            // Animate the number
            AnimateDigitWithNumber(count);
            count--;
            // Wait a full second
            yield return new WaitForSeconds(countdownUnitMultiplier);

            // Zero is not displayed in this loop
        }

        if (displayZero && !displayIntegerPart)
        {
            AnimateDigitWithNumber(0);
        }
        yield return new WaitForSeconds(countdownUnitMultiplier * digitOffset);

        callback?.Invoke();
        onCountdownFinished?.Invoke();

        isCountingDown = false;
    }

    private void AnimateDigitWithNumber(int num)
    {
        if (displayIntegerPart)
        {
            num--;
        }

        string numString;
        if (num == 0 && !string.IsNullOrEmpty(stringToDisplayInsteadOfZero))
        {
            numString = stringToDisplayInsteadOfZero;
        }
        else
        {
            numString = num.ToString();
        }

        digits[textIndex].text = numString;
        digits[textIndex].transform.SetSiblingIndex(-1);
        StartCoroutine(AnimateDigit(digits[textIndex]));
        textIndex = (textIndex + 1) % digits.Count;
    }

    private IEnumerator AnimateDigit(TextMeshProUGUI digit)
    {
        float time = 0;
        while (time < digitDuration)
        {
            time += Time.deltaTime;
            float alpha = digitAlpha.Evaluate(time / digitDuration);
            float scale = digitScale.Evaluate(time / digitDuration) * scaleMultiplier + 1;
            digit.alpha = alpha;
            digit.transform.localScale = Vector3.one * scale;
            yield return null;
        }
    }

    private void PopulateNecessaryTexts()
    {
        int currentCount = digits.Count;
        int necessaryCount = Mathf.CeilToInt(digitDuration / countdownUnitMultiplier);

        if (currentCount < necessaryCount)
        {
            for (int i = currentCount; i < necessaryCount; i++)
            {
                TextMeshProUGUI newDigit = Instantiate(countdownText, countdownText.transform.parent);
                digits.Add(newDigit);
            }
        }
        else if (currentCount > necessaryCount)
        {
            for (int i = currentCount - 1; i >= necessaryCount; i--)
            {
                Destroy(digits[i].gameObject);
                digits.RemoveAt(i);
            }
        }

        HideDigits();
    }

    private void HideDigits()
    {
        foreach (TextMeshProUGUI digit in digits)
        {
            digit.text = "";
        }
    }
}
