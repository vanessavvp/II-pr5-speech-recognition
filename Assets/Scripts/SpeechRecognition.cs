using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows.Speech;
using System.Linq;

public class SpeechRecognition : MonoBehaviour
{
    public Text displayText;
    private KeywordRecognizer keywordRecognizer;
    private ConfidenceLevel confidence = ConfidenceLevel.Low;
    private Dictionary<string, Action> keywordActions = new Dictionary<string, Action>();
    private CarController carController;
    private delegate void Action();
    void Start()
    {
        carController = FindObjectOfType<CarController>();

        keywordActions.Add("forward", carController.MoveForward);
        keywordActions.Add("back", carController.MoveBackward);
        keywordActions.Add("left", carController.SteerLeft);
        keywordActions.Add("right", carController.SteerRight);
        keywordActions.Add("all left", carController.SteerAllLeft);
        keywordActions.Add("all right", carController.SteerAllRight);
        keywordActions.Add("stop", carController.Stop);
        keywordActions.Add("straight ahead", carController.SteerStraightAhead);
        keywordActions.Add("top speed", carController.MaxSpeed);
        keywordActions.Add("reset", carController.ResetCarPosition);

        keywordRecognizer = new KeywordRecognizer(keywordActions.Keys.ToArray(), confidence);
        keywordRecognizer.OnPhraseRecognized += OnKeywordRecognized;
        keywordRecognizer.Start();
    }

    void OnDestroy()
    {
        if (keywordRecognizer != null && keywordRecognizer.IsRunning)
        {
            keywordRecognizer.Stop();
            keywordRecognizer.Dispose();
        }
        
    }

    private void OnKeywordRecognized(PhraseRecognizedEventArgs args)
    {
        displayText.text = args.text;
        keywordActions[args.text].Invoke();
    }
}
