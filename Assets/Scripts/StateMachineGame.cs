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
                if (trialNumber >= TRIAL_COUNT)
                {
                    SceneManager.LoadScene("Debrief");
                }
                else
                {
                    gameRunning = false;
                    overlay.SetActive(true);
                    scorePopUp.text = "You scored " + score + " points.";
                }
            }
        }
    }
    public GameObject overlay;
    public Text moveCounterText;
    public Text turnTimerText;
    public Text scoreText;
    public Text scorePopUp;
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
            turnTimerText.text = "Time:" + Environment.NewLine + _turnTimer.ToString("0.0");
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
            scoreText.text = "Score:" + Environment.NewLine + _score.ToString();
        }
    }
    public Color selectedColor;
    public Color unselectedColor;
    const int TRIAL_COUNT = 32;
    int[] LARGE_PENALTIES = { -70, -100, -140 };
    public static int participantID;
    public int trialNumber = 0;
    public AudioClip[] songChoices;
    private AudioSource audioSource;
    private string logfile = "";
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
            turnTimer += Time.deltaTime;
        }
    }
    public void UPressed()
    {
        File.AppendAllText(logfile, "U," + currentState.name + "," + currentState.nextUState.name + "," + turnTimer.ToString() + "," + currentState.UReward.ToString() + Environment.NewLine);
        changeState(currentState.UReward, currentState.nextUState);
    }
    public void IPressed()
    {
        File.AppendAllText(logfile, "I," + currentState.name + "," + currentState.nextIState.name + "," + turnTimer.ToString() + "," + currentState.IReward.ToString() + Environment.NewLine);
        changeState(currentState.IReward, currentState.nextIState);
    }
    private void changeState(int reward, StateScript nextState)
    {
        score += reward;
        currentState.image.color = unselectedColor;
        currentState = nextState;
        nextState.image.color = selectedColor;
        moveCount--;
        turnTimer = 0.0f;
    }
    private int latinSquare(int size, int index)
    {
        return (index % size + index / size) % size;
    }
    private void newTrial()
    {
        if (currentState != null)
        {
            currentState.image.color = unselectedColor;
        }
        currentState = transform.Find("Background/Game Panel/" + (latinSquare(6, trialNumber + participantID) + 1).ToString()).GetComponent<StateScript>();
        currentState.image.color = selectedColor;
        moveCount = latinSquare(7, trialNumber + participantID) + 2;
        moveCounterText.text = _moveCount.ToString() + " moves";
        score = 0;
    }
    public void closeOverlay()
    {
        overlay.SetActive(false);
        newTrial();
        gameRunning = true;
    }
}
