using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NotesController : MonoBehaviour {

    // PÚBLICO - SCRIPTS //
    public LoadData loadData;

    // PÚBLICO //
    public TMP_InputField inputField; // Campo para modificar notas de los errores.
    public TextMeshProUGUI error_index; // Título de la nota
    public TextMeshProUGUI error_save; // Título de la nota
    public Button cancelButton; // Botón de cancelar.
    public Button confirmButton; // Botón de confirmar.
    public string jsonFilePath; // Ruta al archivo JSON.

    // PÚBLICO - SERIALIZABLE //
    [System.Serializable]
    public class ActionData { public string tipo; public string accion; public string objeto; public float tiempo; public string nota; }

    [System.Serializable]
    public class ActionList { public ActionData[] list; }

    // PRIVADO //
    private ActionList actionList; // Para almacenar los datos deserializados.
    private int selectedErrorIndex, selectedActionIndex; // Posición del evento seleccionado según error o acción.

    void Start() {

        HideCurrentNote();
        ReadJson(); // Leer y deserializar el JSON al inicio.
        confirmButton.onClick.AddListener(OnConfirmButtonClicked); // Agrega el listener al botón de confirmación.
        cancelButton.onClick.AddListener(HideCurrentNote); // Agrega el listener al botón de cancelar.

    }

    void ReadJson() {

        if (File.Exists(jsonFilePath)) {
            string jsonString = File.ReadAllText(jsonFilePath);
            actionList = JsonUtility.FromJson<ActionList>(jsonString);
        } else { Debug.LogError("Archivo JSON no encontrado en la ruta " + jsonFilePath); }

    }

    public void DisplayCurrentNote(int index, int number) {

        selectedErrorIndex = index;
        selectedActionIndex = number;

        // Hacer visible la nota de la acción escogida.
        GameObject note = this.gameObject;
        note.SetActive(true);
        
        // Cargar ventana de visualización con la nota correspondiente.
        error_index.text = "Notas del error " + (index+1);
        error_save.text = ("");
        if (actionList != null && actionList.list.Length > selectedActionIndex) {
            inputField.text = actionList.list[selectedActionIndex].nota;
        } else { Debug.LogError("Índice de acción seleccionado fuera de rango o lista vacía."); }

    }

    // Método para gestionar lo que sucede cuando se pulsa el botón para guardar la nota.
    void OnConfirmButtonClicked() {

        if (actionList != null && actionList.list.Length > selectedActionIndex) {
            
            // Texto de retroalimentación de guardado, a menos que no hubiese cambios.
            if (actionList.list[selectedActionIndex].nota != inputField.text) { StartCoroutine(ShowText()); }

            // Actualizar el campo "nota" de la acción seleccionada con el texto del Input Field.
            actionList.list[selectedActionIndex].nota = inputField.text;
            SaveJson();

            // Actualizar visibilidad de la imagen de las notas, según si se almacena texto o no.
            RawImage note_image = loadData.buttons[selectedErrorIndex].GetComponentInChildren<RawImage>();
            if (inputField.text == "") { note_image.enabled = false; }
            else { note_image.enabled = true; }

        } else { Debug.LogError("Índice de acción seleccionado fuera de rango o lista vacía."); }

    }

    // Método para guardar el JSON actualizado.
    void SaveJson() {
        string updatedJsonString = JsonUtility.ToJson(actionList, true);
        File.WriteAllText(jsonFilePath, updatedJsonString);
        Debug.Log("JSON actualizado correctamente.");
    }

    // Método para ocultar el GameObject.
    void HideCurrentNote() {
        GameObject note = this.gameObject;
        note.SetActive(false);
    }

    // Mostrar el texto, esperar 3 segundos y ocultar texto.
    private IEnumerator ShowText() {

        error_save.text = ("(guardado)");
        yield return new WaitForSeconds(1.5f);
        error_save.text = ("");

    }

}
