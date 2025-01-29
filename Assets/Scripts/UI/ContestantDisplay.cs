using System.Collections;
using UnityEngine;

public class ContestantDisplay : MonoBehaviour
{
    [SerializeField] private RectTransform leaderboardContainer; // Parent for contestants
    [SerializeField] private Contestant contestantPrefab;

    [Header("Animation")]
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
        gameObject.SetActive(true);
        StartCoroutine(SpawnContestants(contestants));
    }

    private void ClearContestants()
    {
        foreach (Transform child in leaderboardContainer)
        {
            Destroy(child.gameObject);
        }
    }

    private IEnumerator SpawnContestants(ContestantData[] contestants)
    {
        foreach (var contestant in contestants)
        {
            Contestant newContestant = Instantiate(contestantPrefab, leaderboardContainer);
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
