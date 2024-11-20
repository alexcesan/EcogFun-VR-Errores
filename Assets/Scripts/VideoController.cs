using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;

public class VideoController : MonoBehaviour {

    // PÚBLICO - SCRIPTS //
    public VideoPlayer videoPlayer1; // Primer VideoPlayer
    public VideoPlayer videoPlayer2; // Segundo VideoPlayer

    // PÚBLICO //
    public Button pauseButton; // Botón de pausa.
    public TMP_InputField lapseField; // Campo para modificar notas de los errores.
    public TextMeshProUGUI time_text; // Referencia al TextMeshPro.
    public float time_lapse; // Por defecto, 10 segundos de lapso: 5 antes del evento y 5 después.

    // PRIVADO //
    private RawImage[] pause_list;
    private float begin_time, init_time, end_time;
    private bool isPlaying = false;

    // Al inicio, los vídeos están detenidos.
    void Start() {
        videoPlayer1.Stop();
        videoPlayer2.Stop();
        pause_list = pauseButton.GetComponentsInChildren<RawImage>();
        pauseButton.gameObject.SetActive(false);
        lapseField.gameObject.SetActive(false);
    }

    void Update() {

        // Verificamos si los vídeos están reproduciéndose y si se ha alcanzado el tiempo final. En ese caso, reproducimos en bucle.
        if (isPlaying && (videoPlayer1.time >= end_time || videoPlayer2.time >= end_time)) { PlayFromTo(begin_time); }

        if (isPlaying) { pause_list[0].enabled = false; pause_list[1].enabled = true; }
        else { pause_list[0].enabled = true; pause_list[1].enabled = false; }

        // En todo momento, indicamos en pantalla en qué segundo del vídeo nos encontramos.
        float currentTime = (float)videoPlayer1.time; // Sincronizamos ambos vídeos, tomamos el tiempo de uno.
        if (currentTime != 0.0f) {
            int minutes = Mathf.FloorToInt(currentTime / 60);
            int seconds = Mathf.FloorToInt(currentTime % 60);
            time_text.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    // Método para reproducir ambos vídeos desde el segundo inicial hasta el final.
    public void PlayFromTo(float start_time) {
        begin_time = start_time;
        init_time = start_time - time_lapse / 2;
        end_time = start_time + time_lapse / 2;

        StopVideos();
        videoPlayer1.time = init_time;
        videoPlayer2.time = init_time;
        pauseButton.gameObject.SetActive(true);
        lapseField.gameObject.SetActive(true);
        PlayVideos();
    }

    public void PlayNewLapse(float inputNumber) {
        time_lapse = inputNumber;
        PlayFromTo(begin_time);
    }

    // Método para continuar la reproducción de ambos vídeos.
    public void PlayVideos() {
        videoPlayer1.Play();
        videoPlayer2.Play();
        isPlaying = true;
    }

    // Método para detener la reproducción de ambos vídeos.
    public void StopVideos() {
        videoPlayer1.Stop();
        videoPlayer2.Stop();
        isPlaying = false;
    }

    // Método para cambiar entre pausa y reproducción pulsando un botón.
    public void TogglePause() {
        if (isPlaying) {
            videoPlayer1.Pause();
            videoPlayer2.Pause();
            isPlaying = false;
        } else { PlayVideos(); }
    }

}