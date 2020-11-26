using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class FlingTower : MonoBehaviour
{
    public float cameraOffset = 2f;
    public float leftColumn;
    public float rightColumn;
    public float generationOffset = 7f;
    
    public float grappleAccumulation = 1f;
    public float followerAccumulation = .1f;
    public float platformAccumulation = .1f;

    float accumulatedGrapples = 2f;
    float accumulatedFollowers = .6f;
    float accumulatedPlatforms = .5f;

    public GameObject grapplePrefab;
    public GameObject followerPrefab;
    public GameObject platformPrefab;

    public float grappleReductionPerRow = 0.995f;
    public int maximumGrappleSpacing = 4;

    public TextMeshProUGUI m_heightScore;
    public TextMeshProUGUI m_heightHighScore;
    public TextMeshProUGUI m_followerScore;
    public TextMeshProUGUI m_followerHighScore;

    int lastRow = 0;
    bool exiting = false;

    int lastGrappleRow = 0;
    int followers = 0;

    private void OnEnable()
    {
        m_heightHighScore.text = "" + PlayerPrefs.GetInt("FlingTowerHeightHighScore", 0) + ":   Height";
        m_followerHighScore.text = "" + PlayerPrefs.GetInt("FlingTowerFollowerHighScore", 0) + ":Followers";
        Follower.FollowerGained.AddListener(GainedFollower);
    }
    private void OnDisable()
    {
        Follower.FollowerGained.RemoveListener(GainedFollower);
    }

    private void FixedUpdate()
    {
        //Camera movement
        var playerPos = Movement.playerTransform.position;
        if (playerPos.y - cameraOffset > transform.position.y)
            transform.position += Vector3.up * (playerPos.y - cameraOffset - transform.position.y);

        //Level generation
        var row = (int)transform.position.y;
        if(row > lastRow)
        {
            ++lastRow;
            Accumulate();
            GenerateRow();
            
            m_heightScore.text = "Height   :" + lastRow;
            if(PlayerPrefs.GetInt("FlingTowerHeightHighScore", 0) < lastRow)
            {
                m_heightHighScore.text = "" + lastRow + ":   Height";
                PlayerPrefs.SetInt("FlingTowerHeightHighScore", lastRow);
            }
        }
    }

    void GainedFollower()
    {
        ++followers;
        m_followerScore.text = "Followers:" + followers;
        if (PlayerPrefs.GetInt("FlingTowerFollowerHighScore", 0) < followers)
        {
            m_followerHighScore.text = "" + followers + ":Followers";
            PlayerPrefs.SetInt("FlingTowerFollowerHighScore", followers);
        }
    }

    public void EndLevel()
    {
        if (exiting)
            return;
        exiting = true;
        ScreenBlind.Enter();
        Invoke("ReloadLevel", 1f);
    }

    public void ReloadLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ExitToMainMenu()
    {
        if (exiting)
            return;
        exiting = true;
        ScreenBlind.Enter();
        Invoke("LoadMenu", 1f);
    }

    void LoadMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    //Level generation

    void Accumulate()
    {
        accumulatedGrapples += grappleAccumulation;
        accumulatedFollowers += followerAccumulation;
        accumulatedPlatforms += platformAccumulation;
        grappleAccumulation *= grappleReductionPerRow;
    }

    void GenerateRow()
    {
        //Create list of the available positions
        var positions = new List<Vector3>(Mathf.RoundToInt(rightColumn - leftColumn) + 1);
        for(int i = 0; i < positions.Capacity; ++i)
        {
            positions.Add(new Vector3(leftColumn + i, lastRow + generationOffset, 0f));
        }

        //Calculate number of entities to add to this row
        var grapples  = (int)Mathf.Min(Random.value * (int)accumulatedGrapples, (int) accumulatedGrapples / 2f);  //Multiply random value by number of available grapples, and round down
        var followers = (int)Mathf.Min(Random.value * (int)accumulatedFollowers, (int)accumulatedFollowers / 3f);
        var platforms = (int)Mathf.Min(Random.value * (int)accumulatedPlatforms, (int)accumulatedPlatforms / 3f);

        if (grapples > 0)
            lastGrappleRow = lastRow;
        else if (lastRow - lastGrappleRow >= maximumGrappleSpacing)
        {
            grapples = 1;
            lastGrappleRow = lastRow;
        }

        for(; grapples > 0; grapples--)
        {
            if (positions.Count == 0)
                break;

            var positionIndex = Random.Range(0, positions.Count - 1);
            var position = positions[positionIndex];
            positions.RemoveAt(positionIndex);
            Instantiate(grapplePrefab, position, Quaternion.identity);
            accumulatedGrapples -= 1f;
        }
        for(; followers > 0; followers--)
        {
            if (positions.Count == 0)
                break;

            var positionIndex = Random.Range(0, positions.Count - 1);
            var position = positions[positionIndex];
            positions.RemoveAt(positionIndex);
            Instantiate(followerPrefab, position, Quaternion.identity);
            accumulatedFollowers -= 1f;
        }
        for(; platforms > 0; platforms--)
        {
            if (positions.Count == 0)
                break;

            var positionIndex = Random.Range(0, positions.Count - 1);
            var position = positions[positionIndex];
            positions.RemoveAt(positionIndex);
            if(Physics2D.Raycast(position, Vector2.down, 1f, LayerMask.GetMask("Grapple")))
            {
                //Grapple below this position, choose another
                positionIndex = Random.Range(0, positions.Count - 1);
                position = positions[positionIndex];
                positions.RemoveAt(positionIndex);
            }
            Instantiate(platformPrefab, position, Quaternion.identity);
            accumulatedPlatforms -= 1f;
        }
    }
}
