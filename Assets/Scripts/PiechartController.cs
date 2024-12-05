using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PiechartController : MonoBehaviour {

    // PÚBLICO - SCRIPTS //
    public LoadData loadData;

    // PÚBLICO //
    public Image[] imagesPieChart;
    public TextMeshProUGUI[] textCounter;
    // piechart_1 - 0: Correctas | 1: Comisión | 2: Orden | 3: Normas | 4: Omisión | 5: Repetición
    // piechart_2 - 0: Aciertos | 1: Fallos

    // PRIVADO //
    private int[] counts = new int[6];
    private float[] listPercentage = new float[6];

    void Start() {

        GameObject pieChart = this.gameObject;

        //for(int i=0; i < counts.Length; i++) { counts[i] = Random.Range(0, 50); } // Lista aleatoria.

        // Obtenemos la cantidad de eventos de cada tipo.
        if (pieChart.name == "piechart_1") {

            counts = loadData.actionCounts; // Lectura del JSON para dividir entre seis tipos de acciones.

            // Textos que muestran el conteo de los eventos.
            textCounter[0].text = counts[0].ToString() + " aciertos";
            textCounter[1].text = counts[1].ToString() + " comisiones";
            textCounter[2].text = counts[2].ToString() + " actos desordenados";
            textCounter[3].text = counts[3].ToString() + " roturas de normas";
            textCounter[4].text = counts[4].ToString() + " omisiones";
            textCounter[5].text = counts[5].ToString() + " repeticiones";
            /*textCounter[0].text = "Correctas: " + counts[0].ToString(); textCounter[1].text = "Comisión: " + counts[1].ToString(); textCounter[2].text = "Orden: " + counts[2].ToString();
            textCounter[3].text = "Normas: " + counts[3].ToString(); textCounter[4].text = "Omisión: " + counts[4].ToString(); textCounter[5].text = "Repetición: " + counts[5].ToString();*/

        } else {
        
            counts = loadData.correctCounts; // Lectura del JSON para dividir entre aciertos y fallos.

            // Textos que muestran el conteo de los eventos.
            textCounter[0].text = counts[0].ToString() + " aciertos";
            textCounter[1].text = counts[1].ToString() + " fallos";
            /*textCounter[0].text = "Aciertos: " + counts[0].ToString(); textCounter[1].text = "Fallos: " + counts[1].ToString();*/

        }

        // Y calculamos el porcentaje que deberá rellenar cada trozo del gráfico.
        SetValues(counts);
        
    }

    void Update() {

        // Animación para rellenar el gráfico.
        for(int i = 0; i < imagesPieChart.Length; i++) {
            float currentFillAmount = imagesPieChart[i].fillAmount;
            
            //imagesPieChart[i].fillAmount = Mathf.Lerp(currentFillAmount, listPercentage[i], Time.deltaTime * 3.6f); // Versión 1.
            imagesPieChart[i].fillAmount = Mathf.Min(currentFillAmount + 0.011f, listPercentage[i]); // Versión 2.
        }

    }

    void SetValues(int[] values) {

        float totalAmount = 0; // Cantidad de eventos existentes.
        float totalPercentage = 0; // Porcentaje total entre los eventos acumulados (máximo 1.0f).

        for(int i = 0; i < imagesPieChart.Length; i++) { totalAmount += values[i]; }
        
        for(int i = 0; i < imagesPieChart.Length; i++) {
            totalPercentage += values[i] / totalAmount;
            listPercentage[i] = totalPercentage;
        }

    }

}
