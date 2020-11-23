using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    int lastRow = 0;
    bool exiting = false;

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
        }
    }

    public void Lose()
    {

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
            Instantiate(platformPrefab, position, Quaternion.identity);
            accumulatedPlatforms -= 1f;
        }
    }
}
