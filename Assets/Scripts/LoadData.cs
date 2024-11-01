using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoadData : MonoBehaviour {

    // PÚBLICO - SCRIPTS //
    public VideoController videoController; // Referencia a VideoController.
    public NotesController notesController; // Referencia a NotesController.

    // PÚBLICO //
    public Transform contentPanel; // Contenido del ScrollView.
    public GameObject listItemPrefab; // Referencia al prefab del elemento de la lista.
    public string filePath; // Ruta del archivo JSON.

    // PÚBLICO - OCULTO EN EL INSPECTOR //
    [HideInInspector]
    public int[] actionCounts = new int[6]; // Array para almacenar el conteo de cada tipo de acción.

    [HideInInspector]
    public int[] correctCounts = new int[2]; // Array para almacenar el conteo de cada tipo de acción (aciertos y fallos).
    
    [HideInInspector]
    public List<Button> buttons = new List<Button>(); // Array para almacenar los botones.

    // PÚBLICO - SERIALIZABLE //
    [System.Serializable] // Clase para deserializar el JSON.
    public class ActionData { public string tipo; public string accion; public string objeto; public float tiempo; public string nota; }

    [System.Serializable] // Clase para deserializar la lista de acciones del JSON.
    public class ActionList { public ActionData[] list; }

    // PRIVADO //
    private List<Outline> outlines = new List<Outline>(); // Array para almacenar los outlines de los botones.
    private Color defaultColor = Color.black; // Color del borde del botón por defecto.
    private Color selectedColor = Color.yellow; // Color del borde cuando el botón está seleccionado.
    private int selectedButtonIndex = -1; // Índice del botón actualmente seleccionado.

    void Awake() {

        string json = File.ReadAllText(filePath); // Cargar el archivo JSON.
        ActionList actionList = JsonUtility.FromJson<ActionList>(json); // Deserializar el JSON en una lista de acciones.
        CountActions(actionList.list); // Contar las acciones de cada tipo.
        ScrollView(actionList.list); // Llenar la lista con los elementos del JSON.

        Debug.Log("Conteo de acciones: " + string.Join(", ", actionCounts)); // Debug para mostrar el conteo de acciones.
        Debug.Log("Conteo de correctos: " + string.Join(", ", correctCounts)); // Debug para mostrar el conteo de acciones.

    }

    // Método para contar las acciones de cada tipo.
    void CountActions(ActionData[] actions) {

        foreach (ActionData action in actions) {
            switch (action.tipo) {
                case "Correcto": actionCounts[0]++; correctCounts[0]++; break;
                case "Comision": actionCounts[1]++; correctCounts[1]++; break;
                case "Orden": actionCounts[2]++; correctCounts[1]++; break;
                case "Romper las normas": actionCounts[3]++; correctCounts[1]++; break;
                case "Omision": actionCounts[4]++; correctCounts[1]++; break;
                case "Repeticion": actionCounts[5]++; correctCounts[1]++; break;
                default: Debug.LogWarning("Tipo de acción no reconocido: " + action.tipo); break;
            }
        }

    }

    // Método para llenar la lista con los elementos del JSON.
    void ScrollView(ActionData[] actions) {

        int i = 0;
        int n = 0;

        foreach (ActionData action in actions) {

            if (action.tipo != "Correcto") {

                // Instanciar un nuevo elemento de la lista a partir del prefab.
                GameObject newListItem = Instantiate(listItemPrefab);
                newListItem.transform.SetParent(contentPanel, false);
                newListItem.name = "button_error_" + i;

                // Introducir el botón y su borde en sus respectivas listas.
                Button button = newListItem.GetComponent<Button>();
                Outline outline = newListItem.GetComponent<Outline>();
                buttons.Add(button);
                outlines.Add(outline);

                // Agregar eventos al seleccionar los botones.
                int index = i;
                int number = n;
                button.onClick.AddListener(() => OnButtonClick(index, number));
                if (action.tipo != "Omision") { button.onClick.AddListener(() => videoController.PlayFromTo(action.tiempo)); }
                else { button.onClick.AddListener(() => videoController.StopVideo()); }

                // Obtener referencias a los textos e imágenes del nuevo elemento.
                TextMeshProUGUI[] textFields = newListItem.GetComponentsInChildren<TextMeshProUGUI>();
                RawImage noteImage = newListItem.GetComponentInChildren<RawImage>();
                Image imageComponent = newListItem.GetComponent<Image>();

                // Mostrar la información de los botones dependiendo de las acciones del JSON.
                if (action.tipo == "Comision") { textFields[0].text = "COMISIÓN"; imageComponent.color = new Color32(238,89,131,255); }
                else if (action.tipo == "Orden") { textFields[0].text = "ORDEN"; imageComponent.color = new Color32(188,33,77,255); }
                else if (action.tipo == "Romper las normas") { textFields[0].text = "ROTURA DE NORMAS"; imageComponent.color = new Color32(166,15,45,255); }
                else if (action.tipo == "Omision") { textFields[0].text = "OMISIÓN"; imageComponent.color = new Color32(135,6,6,255); textFields[2].text = ""; }
                else if (action.tipo == "Repeticion") { textFields[0].text = "REPETICIÓN"; imageComponent.color = new Color32(89,4,25,255); }
                if (action.tipo != "Omision") { textFields[2].text = CalculateTime(action.tiempo); }
                textFields[1].text = action.accion + " " + action.objeto;

                // Desactivar imágenes donde no haya notas.
                if (action.nota == "") { noteImage.enabled = false; }

                i++;

            }

            n++;

        }

    }

    // Método para ejecutar lo que sucede al pulsar uno de los botones con errores.
    void OnButtonClick(int index, int number) {

        if (selectedButtonIndex >= 0) { outlines[selectedButtonIndex].effectColor = defaultColor; } // Revertir el color del botón previamente seleccionado.
        outlines[index].effectColor = selectedColor; // Establecer el color del botón recién seleccionado.
        selectedButtonIndex = index; // Actualizar el índice del botón seleccionado.
        notesController.DisplayCurrentNote(index, number);

    }

    // Método para convertir el tiempo en segundos en un string con el formato "0min 0s".
    public string CalculateTime(float timeElapsed) {

        string conversionText;
        int minutes, seconds;

        minutes = Mathf.FloorToInt(timeElapsed / 60); 
        seconds = Mathf.FloorToInt(timeElapsed % 60);
        conversionText = string.Format("{0:0}min {1:0}s", minutes, seconds);
        return conversionText;

    }

}
