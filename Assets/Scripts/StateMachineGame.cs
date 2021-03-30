using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
[RequireComponent(typeof(AudioSource))]
public class StateMachineGame : MonoBehaviour
{
    public bool gameRunning = true;
    public StateScript currentState;
    public StateScript targetState;
    private int _moveCount = 0;
    public int moveCount
    {
        get
        {
            return _moveCount;
        }
        set
        {
            _moveCount = value;
            if (_moveCount <= 0)
            {
                trialNumber++;
            }
            centerText.text = _moveCount.ToString() + " moves.";
        }
    }
    public GameObject overlay;
    public Text centerText;
    public Text topText;
    public Text bottomText;
    public Text popUpText;
    public Text popUpTitleText;
    private float _turnTimer = 0.0f;
    public float turnTimer
    {
        get
        {
            return _turnTimer;
        }
        set
        {
            _turnTimer = value;
            topText.text = "Time:" + Environment.NewLine + _turnTimer.ToString("0.0");
        }
    }
    private int _score = 0;
    public int score
    {
        get
        {
            return _score;
        }
        set
        {
            _score = value;
            if (trialNumber > TARGET_TEST_END)
            {
                bottomText.text = "Score:" + Environment.NewLine + _score.ToString();
            }
        }
    }
    public int targetScore;
    public Color selectedColor;
    public Color unselectedColor;
    public Color targetColor;
    int[] LARGE_PENALTIES = { -70, -100, -140 };
    public static int participantID;
    public int _trialNumber = 0;
    public int trialNumber
    {
        get
        {
            return _trialNumber;
        }
        set
        {
            _trialNumber = value;
            newTrial();
        }
    }
    public AudioClip[] songChoices;
    private AudioSource audioSource;
    private string logfile = "";
    public int successes = 0;
    const int TUTORIAL_END = 8;
    const int TARGET_PRACTICE_END = TUTORIAL_END + 81;
    const int TARGET_TEST_END = TARGET_PRACTICE_END + 21;
    const int PROFIT_PRACTICE_END = TARGET_TEST_END + 81;
    const int PROFIT_TEST_END = PROFIT_PRACTICE_END + 21;
    const int TRIAL_END = PROFIT_TEST_END + 66;
    const int MINIMUM_SCORE = 9;
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
    void Start()
    {
        foreach (StateScript state in GetComponentsInChildren<StateScript>())
        {
            if (state.IReward == 0)
            {
                state.IReward = LARGE_PENALTIES[latinSquare(LARGE_PENALTIES.Length, participantID)];
            }
        }
        audioSource.clip = songChoices[latinSquare(songChoices.Length, participantID)];
        audioSource.Play();
        Directory.CreateDirectory("Experiment Logs");
        logfile = Path.Combine("Experiment Logs", participantID + System.DateTime.Now.ToString(" yyyy MM dd hh mm ss") + ".csv");
        File.WriteAllText(logfile, "Key Pressed,From Rectangle,To Rectangle,Time Spent Before Moving,Point Change" + Environment.NewLine);
        newTrial();
    }
    void Update()
    {
        if (gameRunning)
        {
            if (Input.GetKeyDown(KeyCode.U))
            {
                UPressed();
            }
            else if (Input.GetKeyDown(KeyCode.I))
            {
                IPressed();
            }
            if (trialNumber == 7)
            {
                turnTimer -= Time.deltaTime;
                if (turnTimer <= 0)
                {
                    trialNumber++;
                }
            }
            else if (trialNumber > PROFIT_TEST_END)
            {
                turnTimer += Time.deltaTime;
                if (turnTimer <= 0)
                {
                    trialNumber++;
                }
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                closeOverlay();
            }
        }
    }
    public void UPressed()
    {
        switch (trialNumber)
        {
            case 5:
                trialNumber++;
                break;
            case 6:
                break;
            case 7:
                setState(int.Parse(currentState.nextUState.name));
                break;
            default:
                if (trialNumber > PROFIT_PRACTICE_END)
                {
                    File.AppendAllText(logfile, "U," + currentState.name + "," + currentState.nextUState.name + "," + turnTimer.ToString() + "," + currentState.UReward.ToString() + Environment.NewLine);
                    turnTimer = 0.0f;
                }
                changeState(currentState.UReward, currentState.nextUState);
                break;
        }
    }
    public void IPressed()
    {
        switch (trialNumber)
        {
            case 5:
                break;
            case 6:
                trialNumber++;
                break;
            case 7:
                setState(int.Parse(currentState.nextIState.name));
                break;
            default:
                if (trialNumber > PROFIT_PRACTICE_END)
                {
                    File.AppendAllText(logfile, "I," + currentState.name + "," + currentState.nextIState.name + "," + turnTimer.ToString() + "," + currentState.IReward.ToString() + Environment.NewLine);
                    turnTimer = 0.0f;
                }
                changeState(currentState.IReward, currentState.nextIState);
                break;
        }
    }
    private void changeState(int reward, StateScript nextState)
    {
        score += reward;
        if (currentState == targetState)
        {
            currentState.image.color = targetColor;
        }
        else
        {
            currentState.image.color = unselectedColor;
        }
        currentState = nextState;
        nextState.image.color = selectedColor;
        moveCount--;
    }
    private int latinSquare(int size, int index)
    {
        return (index % size + index / size) % size;
    }
    private void newTrial()
    {
        centerText.text = moveCount.ToString() + " moves";
        switch (trialNumber)
        {
            case 0:
                showOverlay("Instructions", "Welcome to the first part of the training!");
                break;
            case 1:
                showOverlay("Instructions", "You will see 6 rectangles on the screen. You can move back and forth between any of the rectangles by pressing the 'U' or 'I' keys.");
                break;
            case 2:
                showOverlay("Instructions", "One key goes through the middle, the other around the circle. You will have some time to try this out. Familiarize yourself with which key you have to press in order to get away from other rectangles.");
                break;
            case 3:
                showOverlay("Instructions", "Below the rectangles there will be indicators denoted by pluses and minuses that show if you will gain or lose points. Two pluses or minuses indicate you will win or lose more. The indicators are separated by a '/'  on the left corresponds to pressing 'U' to go in a circle while the right corresponds with pressing 'I' to go across.");
                break;
            case 4:
                showOverlay("Instructions", "Now it is time to practice!");
                break;
            case 5:
                setState(1);
                topText.text = "Tutorial";
                centerText.text = "Press U to move counter-clockwise around the circle.";
                break;
            case 6:
                setState(2);
                centerText.text = "Press I to move across the circle.";
                break;
            case 7:
                centerText.text = "Experiment with U and I to get used to how you can move around the circle.";
                turnTimer = 10.0f;
                break;
            case TUTORIAL_END:
                showOverlay("Practice 1", "Now we will practice reaching a target. Get to the red target in the allotted amount of moves.");
                break;
            case TARGET_PRACTICE_END:
                successes = 0;
                showOverlay("Test 1", "In order to continue, you must score at least 9 out of 10 successes.");
                break;
            case TARGET_TEST_END:
                if (successes > MINIMUM_SCORE)
                {
                    showOverlay("Test 1", "You have achieved " + successes.ToString() + " successes and passed the test.");
                }
                else
                {
                    _trialNumber = TARGET_PRACTICE_END - 1;
                    showOverlay("Test 1", "You have achieved " + successes.ToString() + " successes and failed the test. Please try again.");
                }
                break;
            case TARGET_TEST_END + 1:
                targetState.image.color = unselectedColor;
                targetState = null;
                showOverlay("Practice 2", "Welcome to the second part of the training. Your goal now is to memorize how many points each move delivers. Try to get the maximum amount of points with the amount of moves you are given.");
                break;
            case PROFIT_PRACTICE_END:
                successes = 0;
                showOverlay("Test 2", "In order to continue, you must achieve the highest score on 9 out of 10 tries.");
                break;
            case PROFIT_TEST_END:
                if (successes > MINIMUM_SCORE)
                {
                    showOverlay("Test 2", "You have achieved " + successes.ToString() + " successes and passed the test.");
                }
                else
                {
                    _trialNumber = PROFIT_PRACTICE_END - 1;
                    showOverlay("Test 2", "You have achieved " + successes.ToString() + " successes and failed the test. Please try again.");
                }
                break;
            case PROFIT_TEST_END + 1:
                showOverlay("Trial", "You have successfully passed the tutorials and practice rounds, and are now beginning the trial.");
                break;
            case TRIAL_END:
                showOverlay("End", "You have finished the trial.");
                break;
            case TRIAL_END + 1:
                Application.Quit();
                break;
            default:
                if (trialNumber < TARGET_PRACTICE_END)
                {
                    if (trialNumber % 2 == 1)
                    {
                        topText.text = "Practice 1 Review " + ((trialNumber - TUTORIAL_END) / 2 + 1);
                        if (targetState != null)
                        {
                            targetState.image.color = unselectedColor;
                        }
                        setState(UnityEngine.Random.Range(1, 7));
                        targetState = currentState;
                        moveCount = 2;
                        for (int moves = moveCount; moves > 0; moves--)
                        {
                            targetState = UnityEngine.Random.Range(0.0f, 1.0f) >= 0.5f ? targetState.nextIState : targetState.nextUState;
                        }
                        if (targetState == currentState)
                        {
                            moveCount++;
                            targetState = UnityEngine.Random.Range(0.0f, 1.0f) >= 0.5f ? targetState.nextIState : targetState.nextUState;
                        }
                        targetState.image.color = targetColor;
                    }
                    else
                    {
                        if (currentState == targetState)
                        {
                            showOverlay("Practice 1", "Success.");
                        }
                        else
                        {
                            showOverlay("Practice 1", "Failure.");
                        }
                    }
                }
                else if (trialNumber < TARGET_TEST_END)
                {
                    if (trialNumber % 2 == 0)
                    {
                        topText.text = "Test 1 Question " + ((trialNumber - TARGET_PRACTICE_END) / 2 + 1);
                        if (targetState != null)
                        {
                            targetState.image.color = unselectedColor;
                        }
                        setState(UnityEngine.Random.Range(1, 7));
                        targetState = currentState;
                        moveCount = 2;
                        for (int moves = moveCount; moves > 0; moves--)
                        {
                            targetState = UnityEngine.Random.Range(0.0f, 1.0f) >= 0.5f ? targetState.nextIState : targetState.nextUState;
                        }
                        if (targetState == currentState)
                        {
                            moveCount++;
                            targetState = UnityEngine.Random.Range(0.0f, 1.0f) >= 0.5f ? targetState.nextIState : targetState.nextUState;
                        }
                        targetState.image.color = targetColor;
                    }
                    else
                    {
                        if (currentState == targetState)
                        {
                            successes++;
                            showOverlay("Test 1", "Success.");
                        }
                        else
                        {
                            showOverlay("Test 1", "Failure.");
                        }
                    }
                }
                else if (trialNumber < PROFIT_PRACTICE_END)
                {
                    if (trialNumber % 2 == 0)
                    {
                        score = 0;
                        topText.text = "Practice 2 Review " + (trialNumber - TARGET_TEST_END) / 2;
                        setState(UnityEngine.Random.Range(1, 7));
                        moveCount = UnityEngine.Random.Range(2, 8);
                        targetScore = findHighScore();
                    }
                    else
                    {
                        if (score >= targetScore)
                        {
                            showOverlay("Practice 2", "Success.");
                        }
                        else
                        {
                            showOverlay("Practice 2", "Failure.");
                        }
                    }
                }
                else if (trialNumber < PROFIT_TEST_END)
                {
                    if (trialNumber % 2 == 0)
                    {
                        score = 0;
                        topText.text = "Test 2 Question " + ((trialNumber - PROFIT_PRACTICE_END) / 2 + 1);
                        setState(UnityEngine.Random.Range(1, 7));
                        moveCount = UnityEngine.Random.Range(2, 8);
                        targetScore = findHighScore();
                    }
                    else
                    {
                        if (score >= targetScore)
                        {
                            successes++;
                            showOverlay("Test 2", "Success.");
                        }
                        else
                        {
                            showOverlay("Test 2", "Failure.");
                        }
                    }
                }
                else if (trialNumber < TRIAL_END)
                {
                    if (trialNumber % 2 == 0)
                    {
                        turnTimer = 0;
                        score = 0;
                        setState(latinSquare(6, participantID + trialNumber) + 1);
                        moveCount = latinSquare(7, participantID + trialNumber) + 2;
                    }
                    else
                    {
                        showOverlay("Trial episode " + (trialNumber - PROFIT_TEST_END) / 2, "You scored " + score + " points.");
                    }
                }
                break;
        }
    }
    public void showOverlay(string title, string text)
    {
        gameRunning = false;
        overlay.SetActive(true);
        popUpText.text = text;
        popUpTitleText.text = title;
    }
    public void closeOverlay()
    {
        gameRunning = true;
        overlay.SetActive(false);
        trialNumber++;
    }
    public void setState(int state)
    {
        if (currentState != null)
        {
            currentState.image.color = unselectedColor;
        }
        currentState = transform.Find("Background/Game Panel/" + state.ToString()).GetComponent<StateScript>();
        currentState.image.color = selectedColor;
    }
    private int findHighScore()
    {
        int highScore = int.MinValue;
        for (int testingCombination = 0; testingCombination < Mathf.Pow(2.0f, moveCount); testingCombination++)
        {
            StateScript cursor = currentState;
            int cursorScore = 0;
            for (int moves = 0; moves < moveCount; moves++)
            {
                if (Mathf.Floor(testingCombination & (int) Mathf.Pow(2, moves)) > 0)
                {
                    cursorScore += cursor.IReward;
                    cursor = cursor.nextIState;
                }
                else
                {
                    cursorScore += cursor.UReward;
                    cursor = cursor.nextUState;
                }
            }
            if (cursorScore > highScore)
            {
                highScore = cursorScore;
            }
        }
        return highScore;
    }
}
