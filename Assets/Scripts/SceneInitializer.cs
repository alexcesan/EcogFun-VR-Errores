using System;
using System.IO;
using UnityEngine;
using UnityEngine.Video;

public class SceneInitializer : MonoBehaviour
{
    private string basePath;
    private string video1Path;
    private string video2Path;
    private string jsonPath;

    public VideoPlayer videoPlayer1; // Asigna un VideoPlayer desde el Inspector
    public VideoPlayer videoPlayer2; // Asigna otro VideoPlayer desde el Inspector

    void Start()
    {
        // Determinar la ruta de la carpeta en el directorio de Documentos
        basePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "EcogFun-VR");
        video1Path = Path.Combine(basePath, "video_vr.mkv");
        video2Path = Path.Combine(basePath, "webcam.mkv");
        jsonPath = Path.Combine(basePath, "errores.json");

        // Crear la carpeta si no existe
        if (!Directory.Exists(basePath))
        {
            Directory.CreateDirectory(basePath);
            Debug.Log($"Directorio creado en: {basePath}");
        }

        // Verificar y cargar los archivos
        LoadResources();
    }

    private void LoadResources()
    {
        // Cargar el primer vídeo
        if (File.Exists(video1Path))
        {
            videoPlayer1.url = video1Path;
            videoPlayer1.Prepare();
            Debug.Log("Vídeo 1 cargado correctamente.");
        }
        else
        {
            Debug.LogWarning($"El archivo de vídeo 1 no existe en la ruta: {video1Path}");
        }

        // Cargar el segundo vídeo
        if (File.Exists(video2Path))
        {
            videoPlayer2.url = video2Path;
            videoPlayer2.Prepare();
            Debug.Log("Vídeo 2 cargado correctamente.");
        }
        else
        {
            Debug.LogWarning($"El archivo de vídeo 2 no existe en la ruta: {video2Path}");
        }

        // Cargar el archivo JSON
        if (File.Exists(jsonPath))
        {
            string jsonContent = File.ReadAllText(jsonPath);
            Debug.Log("JSON cargado correctamente. Contenido:");
            Debug.Log(jsonContent);
        }
        else
        {
            Debug.LogWarning($"El archivo JSON no existe en la ruta: {jsonPath}");
        }
    }
}