using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;

public class VideoController : MonoBehaviour {

    // PÚBLICO - SCRIPTS //
    public VideoPlayer videoPlayer1; // Primer VideoPlayer
    public VideoPlayer videoPlayer2; // Segundo VideoPlayer

    // PÚBLICO //
    public Button pauseButton; // Botón de pausa
    public Button closeButton; // Botón para salir del modo pantalla completa
    public TMP_InputField lapseField; // Campo para modificar notas de los errores
    public TextMeshProUGUI time_text, timefocus_text, lapsefocus_text; // Referencias a los TextMeshPro
    public RawImage background_focus; // Fondo para evitar distracciones
    public RectTransform video1Rect, video2Rect; // RectTransform de ambos vídeos
    public float time_lapse; // Por defecto, 10 segundos de lapso: 5 antes del evento y 5 después

    [Header("Fullscreen Settings")]
    public Vector2 fullscreenSize = new Vector2(1920, 1080); // Tamaño en modo fullscreen (16:9 por defecto)
    public Vector2 fullscreenVideo1Position = new Vector2(0, 200); // Posición del primer vídeo
    public Vector2 fullscreenVideo2Position = new Vector2(0, -200); // Posición del segundo vídeo

    // PRIVADO //
    private RawImage[] pause_list;
    private float begin_time, init_time, end_time;
    private int minutes, seconds;
    private bool isPlaying = false;
    private bool isFullScreen = false;
    private Vector2 originalSize1, originalPosition1; // Tamaño y posición original del primer vídeo
    private Vector2 originalSize2, originalPosition2; // Tamaño y posición original del segundo vídeo
    private float lastClickTime = 0f; // Tiempo del último clic para detectar doble clic
    private const float doubleClickThreshold = 0.3f; // Tiempo límite para doble clic (en segundos)

    void Start() {

        videoPlayer1.Stop();
        videoPlayer2.Stop();
        pause_list = pauseButton.GetComponentsInChildren<RawImage>();

        // Mantener GameObjects desactivados al inicio
        pauseButton.gameObject.SetActive(false);
        lapseField.gameObject.SetActive(false);
        closeButton.gameObject.SetActive(false);
        background_focus.gameObject.SetActive(false);
        timefocus_text.gameObject.SetActive(false);
        lapsefocus_text.gameObject.SetActive(false);

        // Guardar tamaño y posición original de los vídeos
        originalSize1 = video1Rect.sizeDelta;
        originalPosition1 = video1Rect.anchoredPosition;
        originalSize2 = video2Rect.sizeDelta;
        originalPosition2 = video2Rect.anchoredPosition;

    }

    void Update() {

        // Verificamos si los vídeos están reproduciéndose y si se ha alcanzado el tiempo final. En ese caso, reproducimos en bucle.
        if (isPlaying && (videoPlayer1.time >= end_time || videoPlayer2.time >= end_time)) { PlayFromTo(begin_time); }

        if (isPlaying) { pause_list[0].enabled = false; pause_list[1].enabled = true; }
        else { pause_list[0].enabled = true; pause_list[1].enabled = false; }

        // En todo momento, indicamos en pantalla en qué segundo del vídeo nos encontramos.
        float currentTime = (float)videoPlayer1.time; // Sincronizados ambos vídeos, tomamos el tiempo de uno.
        time_text.text = CalculateTime(currentTime);
        timefocus_text.text = time_text.text;
    
    }

    public string CalculateTime(float currentTime) {

        if (currentTime != 0.0f) {
            minutes = Mathf.FloorToInt(currentTime / 60);
            seconds = Mathf.FloorToInt(currentTime % 60);
        }

        return string.Format("{0:00}:{1:00}", minutes, seconds);

    }

    public void PlayFromTo(float start_time) {

        TogglePause();

        begin_time = start_time;
        init_time = start_time - time_lapse / 2;
        end_time = start_time + time_lapse / 2;

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

    public void PlayVideos() {
        videoPlayer1.Play();
        videoPlayer2.Play();
        isPlaying = true;
    }

    public void TogglePause() {
        if (isPlaying) {
            videoPlayer1.Pause();
            videoPlayer2.Pause();
            isPlaying = false;
        } else { PlayVideos(); }
    }

    public void OnVideoClick(RectTransform videoRect) {
        float timeSinceLastClick = Time.time - lastClickTime;
        lastClickTime = Time.time;
        if (timeSinceLastClick <= doubleClickThreshold) { ToggleFullScreen(); }
    }

    public void ToggleFullScreen() {

        if (isFullScreen) {
            // Posición original
            video1Rect.sizeDelta = originalSize1;
            video1Rect.anchoredPosition = originalPosition1;
            video2Rect.sizeDelta = originalSize2;
            video2Rect.anchoredPosition = originalPosition2;

            closeButton.gameObject.SetActive(false);
            background_focus.gameObject.SetActive(false);
            timefocus_text.gameObject.SetActive(false);
            lapsefocus_text.gameObject.SetActive(false);
        } else {
            // Fullscreen
            video1Rect.sizeDelta = fullscreenSize;
            video1Rect.anchoredPosition = fullscreenVideo1Position;
            video2Rect.sizeDelta = fullscreenSize;
            video2Rect.anchoredPosition = fullscreenVideo2Position;

            string time1 = CalculateTime(init_time);
            string time2 = CalculateTime(end_time);
            lapsefocus_text.text = "[ " + time1 + " / " + time2 + " ]";

            closeButton.gameObject.SetActive(true);
            background_focus.gameObject.SetActive(true);
            timefocus_text.gameObject.SetActive(true);
            lapsefocus_text.gameObject.SetActive(true);
        }

        isFullScreen = !isFullScreen;

    }

}