using System.Collections;
using UnityEngine;

public class ContestantDisplay : MonoBehaviour
{
    [SerializeField] private GameObject finishedText;
    [SerializeField] private RectTransform leaderboardWindow; // Parent for contestants
    [SerializeField] private RectTransform contestantsContainer; // Parent for contestants
    [SerializeField] private Contestant contestantPrefab;

    [Header("Animation")]
    [SerializeField] private float finishedRaceDelay = 2f;
    [SerializeField] private float leaderboardShowDelay = 3f;
    [SerializeField] private float leaderboardAnimationDelay = 1f;
    [SerializeField] private float contestantAnimationDelay = 0.1f;

    [Header("Debug")]
    [SerializeField] private ContestantData[] debugContestants;

    /*private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ClearContestants();
            DisplayContestants(debugContestants);
        }
    }*/

    public void DisplayContestants(ContestantData[] contestants)
    {
        finishedText.SetActive(false);
        leaderboardWindow.gameObject.SetActive(false);
        gameObject.SetActive(true);
        StartCoroutine(SpawnContestants(contestants));
    }

    private void ClearContestants()
    {
        foreach (Transform child in contestantsContainer)
        {
            Destroy(child.gameObject);
        }
    }

    private IEnumerator SpawnContestants(ContestantData[] contestants)
    {
        yield return new WaitForSeconds(finishedRaceDelay); // Delay before showing finished text

        finishedText.SetActive(true);

        yield return new WaitForSeconds(leaderboardShowDelay); // Delay before showing leaderboard

        leaderboardWindow.gameObject.SetActive(true);

        yield return new WaitForSeconds(leaderboardAnimationDelay); // Delay before spawning contestants

        foreach (var contestant in contestants)
        {
            Contestant newContestant = Instantiate(contestantPrefab, contestantsContainer);
            newContestant.SetContestantName(contestant.Name);
            newContestant.SetPlacement(contestant.Placement);
            newContestant.SetIsYou(contestant.IsYou);
            newContestant.Animate();

            yield return new WaitForSeconds(contestantAnimationDelay); // Small delay between each spawn
        }
    }

    //public void DisplayContestants(ContestantData[] contestants)
    //{
    //    foreach (var contestant in contestants)
    //    {
    //        var newContestant = Instantiate(contestantPrefab, transform);
    //        newContestant.SetContestantName(contestant.Name);
    //        newContestant.SetPlacement(contestant.Placement);
    //    }
    //}
}
