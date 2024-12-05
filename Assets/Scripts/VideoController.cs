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
    public Button pauseButton, pausefocusButton; // Botón de pausa
    public Button closeButton; // Botón para salir del modo pantalla completa
    public TMP_InputField lapseField; // Campo para modificar notas de los errores
    public TextMeshProUGUI time_text, timefocus_text, lapsefocus_text; // Referencias a los TextMeshPro
    public RawImage background_focus; // Fondo para evitar distracciones
    public RectTransform video1Rect, video2Rect; // RectTransform de ambos vídeos
    public float time_lapse = 10f; // Por defecto, 10 segundos de lapso

    [Header("Fullscreen Settings")]
    public Vector2 fullscreenSize = new Vector2(1920, 1080); // Tamaño en modo fullscreen
    public Vector2 fullscreenVideo1Position = new Vector2(0, 200); // Posición del primer vídeo
    public Vector2 fullscreenVideo2Position = new Vector2(0, -200); // Posición del segundo vídeo

    // PRIVADO //
    private RawImage[] pause_list, pausefocus_list;
    private float begin_time, start_time, end_time;
    private int minutes, seconds;
    private bool isPlaying = false;
    private bool isFullScreen = false;
    public bool canReplay = false;
    private Vector2 originalSize1, originalPosition1; // Tamaño y posición original del primer vídeo
    private Vector2 originalSize2, originalPosition2; // Tamaño y posición original del segundo vídeo
    private float lastClickTime = 0f; // Tiempo del último clic para detectar doble clic
    private const float doubleClickThreshold = 0.3f; // Tiempo límite para doble clic (en segundos)

    public void StartVC() {
        videoPlayer1.prepareCompleted += OnVideoPrepared;
        videoPlayer2.prepareCompleted += OnVideoPrepared;

        // Preparar vídeos y dejarlos listos
        videoPlayer1.Prepare();
        videoPlayer2.Prepare();

        pause_list = pauseButton.GetComponentsInChildren<RawImage>();
        pausefocus_list = pausefocusButton.GetComponentsInChildren<RawImage>();

        // Mantener GameObjects desactivados al inicio
        pauseButton.gameObject.SetActive(false);
        lapseField.gameObject.SetActive(false);
        closeButton.gameObject.SetActive(false);
        background_focus.gameObject.SetActive(false);
        pausefocusButton.gameObject.SetActive(false);
        timefocus_text.gameObject.SetActive(false);
        lapsefocus_text.gameObject.SetActive(false);

        // Guardar tamaño y posición original de los vídeos
        originalSize1 = video1Rect.sizeDelta;
        originalPosition1 = video1Rect.anchoredPosition;
        originalSize2 = video2Rect.sizeDelta;
        originalPosition2 = video2Rect.anchoredPosition;
    }

    void Update() {
        // Reproducir en bucle entre start_time y end_time
        if (isPlaying && canReplay && (videoPlayer1.time >= end_time || videoPlayer2.time >= end_time)) { PlayFromTo(begin_time); }

        if (isPlaying && !canReplay && videoPlayer1.time < end_time && videoPlayer1.time >= start_time) { canReplay = true; }

        // Actualizar icono de pausa
        //if (isPlaying) { pause_list[0].enabled = false; pause_list[1].enabled = true; pausefocus_list[0].enabled = false; pausefocus_list[1].enabled = true; }
        //else { pause_list[0].enabled = true; pause_list[1].enabled = false; pausefocus_list[0].enabled = true; pausefocus_list[1].enabled = false; }

        // Actualizar el tiempo mostrado en pantalla
        float currentTime = (float)videoPlayer1.time;
        time_text.text = CalculateTime(currentTime);
        timefocus_text.text = time_text.text;
    }

    public void PlayFromTo(float init_time) {

        begin_time = init_time;
        start_time = init_time - (time_lapse / 2);
        end_time = init_time + (time_lapse / 2);

        if (start_time <= 0.0f) { start_time = 0.0f; } // Si marca inicial es negativa
        if (end_time >= videoPlayer1.length) { end_time = (float)videoPlayer1.length; } // Si marca final sobrepasa la duración del vídeo

        // Desactivar el bucle temporalmente si estamos fuera del rango
        if (videoPlayer1.time >= end_time || videoPlayer2.time >= end_time) { canReplay = false; }
        else { canReplay = true; }

        TogglePause();
        videoPlayer1.time = start_time;
        videoPlayer2.time = start_time;
        PlayVideos();
        
        pauseButton.gameObject.SetActive(true);
        lapseField.gameObject.SetActive(true);

    }

    public void NewLapse(float new_time_lapse) {
        time_lapse = new_time_lapse;
        PlayFromTo(begin_time);
    }

    public void PlayVideos() {
        videoPlayer1.Play();
        videoPlayer2.Play();
        isPlaying = true;
    }

    public void Pause() {
        videoPlayer1.Pause();
        videoPlayer2.Pause();
        isPlaying = false;
    }

    public void TogglePause() {
        if (isPlaying) {
            videoPlayer1.Pause();
            videoPlayer2.Pause();
            isPlaying = false;
        } else { PlayVideos(); }
    }

    public void Omision() {
        Pause();
        pauseButton.gameObject.SetActive(false);
        lapseField.gameObject.SetActive(false);
    }

    public void OnVideoClick(RectTransform videoRect) {
        float timeSinceLastClick = Time.time - lastClickTime;
        lastClickTime = Time.time;
        if (timeSinceLastClick <= doubleClickThreshold) { ToggleFullScreen(); }
    }

    public void ToggleFullScreen() {
        if (isFullScreen) {
            // Volver a la posición original
            video1Rect.sizeDelta = originalSize1;
            video1Rect.anchoredPosition = originalPosition1;
            video2Rect.sizeDelta = originalSize2;
            video2Rect.anchoredPosition = originalPosition2;

            closeButton.gameObject.SetActive(false);
            background_focus.gameObject.SetActive(false);
            pausefocusButton.gameObject.SetActive(false);
            timefocus_text.gameObject.SetActive(false);
            lapsefocus_text.gameObject.SetActive(false);
        } else {
            // Pasar a fullscreen
            video1Rect.sizeDelta = fullscreenSize;
            video1Rect.anchoredPosition = fullscreenVideo1Position;
            video2Rect.sizeDelta = fullscreenSize;
            video2Rect.anchoredPosition = fullscreenVideo2Position;

            lapsefocus_text.text = $"[ {CalculateTime(start_time)} / {CalculateTime(end_time)} ]";

            closeButton.gameObject.SetActive(true);
            background_focus.gameObject.SetActive(true);
            pausefocusButton.gameObject.SetActive(true);
            timefocus_text.gameObject.SetActive(true);
            lapsefocus_text.gameObject.SetActive(true);
        }

        isFullScreen = !isFullScreen;
    }

    private void OnVideoPrepared(VideoPlayer source) {
        if (!isPlaying) { source.Play(); source.Pause(); }
    }

    private string CalculateTime(float currentTime) {

        if (currentTime != 0.0f) {
            minutes = Mathf.FloorToInt(currentTime / 60);
            seconds = Mathf.FloorToInt(currentTime % 60);
            return string.Format("{0:00}:{1:00}", minutes, seconds);
        } else { return ""; }
    }

}