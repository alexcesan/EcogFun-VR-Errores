using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InputHandler : MonoBehaviour {

    // PÚBLICO - SCRIPTS //
    public VideoController videoController;

    // PÚBLICO //
    public TMP_InputField inputField;
    public Button submitButton;

    void Start() { submitButton.onClick.AddListener(OnSubmit); }

    void OnSubmit() {

        string inputText = inputField.text; // Get the input from the Input Field
        int inputNumber; // Convert the input text to an integer

        if (int.TryParse(inputText, out inputNumber)) { videoController.PlayNewLapse(inputNumber); }
        else { Debug.Log("Invalid input. Please enter a valid number."); }

    }

}
