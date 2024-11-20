using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;

public class VideoController : MonoBehaviour {

    // PÚBLICO - SCRIPTS //
    public VideoPlayer videoPlayer;

    // PÚBLICO //
    public Button pauseButton; // Botón de pausa.
    public TMP_InputField lapseField; // Campo para modificar notas de los errores.
    public TextMeshProUGUI time_text; // Referencia al TextMeshPro.
    public float time_lapse; // Por defecto, 10 segundos de lapso: 5 antes del evento y 5 después.

    // PRIVADO //
    private RawImage[] pause_list;
    private float begin_time, init_time, end_time;
    private bool isPlaying = false;

    // Al inicio, el vídeo está detenido.
    void Start() {
        videoPlayer.Stop();
        pause_list = pauseButton.GetComponentsInChildren<RawImage>();
        pauseButton.gameObject.SetActive(false);
        lapseField.gameObject.SetActive(false);
    }

    void Update() {

        // Verificamos si el video está reproduciéndose y si se ha alcanzado el tiempo final. En ese caso, reproducimos en bucle.
        if (isPlaying && videoPlayer.time >= end_time) { PlayFromTo(begin_time); }

        if (isPlaying) { pause_list[0].enabled = false; pause_list[1].enabled = true; }
        else { pause_list[0].enabled = true; pause_list[1].enabled = false; }
        
        // En todo momento, indicamos en pantalla en qué segundo del vídeo nos encontramos.
        float currentTime = (float) videoPlayer.time;
        if (currentTime != 0.0f) {
            int minutes = Mathf.FloorToInt(currentTime / 60); 
            int seconds = Mathf.FloorToInt(currentTime % 60);
            time_text.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }

    }

    // Método para reproducir el video desde el segundo inicial hasta el final.
    public void PlayFromTo(float start_time) {
        
        begin_time = start_time;
        init_time = start_time - time_lapse/2;
        end_time = start_time + time_lapse/2;

        StopVideo();
        videoPlayer.time = init_time;
        pauseButton.gameObject.SetActive(true); lapseField.gameObject.SetActive(true);
        PlayVideo();

    }

    public void PlayNewLapse(float inputNumber) { time_lapse = inputNumber; PlayFromTo(begin_time); }

    // Método para continuar la reproducción del vídeo.
    public void PlayVideo() { videoPlayer.Play(); isPlaying = true; }

    // Método para detener la reproducción del video.
    public void StopVideo() { videoPlayer.Stop(); isPlaying = false; }

    // Método para cambiar entre pausa y reproducción pulsando un botón.
    public void TogglePause() {
        if (isPlaying) { videoPlayer.Pause(); isPlaying = false; }
        else { PlayVideo(); }
    }

}