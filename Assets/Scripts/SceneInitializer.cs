using System;
using System.IO;
using UnityEngine;
using UnityEngine.Video;
using Newtonsoft.Json;

public class SceneInitializer : MonoBehaviour {

    private string basePath;
    private string video1Path;
    private string video2Path;
    private string erroresJsonPath;

    public TextAsset fileNameConfigJson; // Asigna el JSON desde el Inspector
    public VideoPlayer videoPlayer1;    // Asigna un VideoPlayer desde el Inspector
    public VideoPlayer videoPlayer2;    // Asigna otro VideoPlayer desde el Inspector

    [Serializable]
    private class FileNameConfig {
        public string[] video_file_names;
        public string execution_file_name;
        public string errores_file_name;
    }

    void Awake() {

        // Asegurarse de que la configuración del archivo JSON esté asignada
        if (fileNameConfigJson == null) {
            Debug.LogError("No se ha asignado el archivo de configuración JSON.");
            return;
        }

        // Parsear el JSON para obtener los nombres de los archivos
        FileNameConfig fileNames;
        try {
            fileNames = JsonConvert.DeserializeObject<FileNameConfig>(fileNameConfigJson.text);
            if (fileNames == null || fileNames.video_file_names.Length < 2) { Debug.LogError("El JSON de configuración es inválido o incompleto."); return; }
        } catch (Exception e) { Debug.LogError($"Error al leer el JSON de configuración: {e.Message}"); return; }

        // Determinar la ruta base de los archivos
        basePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "EcogFun-VR");
        video1Path = Path.Combine(basePath, fileNames.video_file_names[0]);
        video2Path = Path.Combine(basePath, fileNames.video_file_names[1]);
        erroresJsonPath = Path.Combine(basePath, fileNames.errores_file_name);

        // Crear la carpeta si no existe
        if (!Directory.Exists(basePath)) {
            Directory.CreateDirectory(basePath);
            Debug.Log($"Directorio creado en: {basePath}");
        }

        // Verificar y cargar los archivos
        LoadResources();

    }

    private void LoadResources() {

        // Cargar el primer vídeo
        if (File.Exists(video1Path)) {
            videoPlayer1.url = video1Path;
            videoPlayer1.Prepare();
            Debug.Log($"Vídeo 1 cargado correctamente desde: {video1Path}");
        } else { Debug.LogWarning($"El archivo de vídeo 1 no existe en la ruta: {video1Path}"); }

        // Cargar el segundo vídeo
        if (File.Exists(video2Path)) {
            videoPlayer2.url = video2Path;
            videoPlayer2.Prepare();
            Debug.Log($"Vídeo 2 cargado correctamente desde: {video2Path}");
        } else { Debug.LogWarning($"El archivo de vídeo 2 no existe en la ruta: {video2Path}"); }

        // Cargar el archivo JSON de errores
        if (File.Exists(erroresJsonPath)) {
            string jsonContent = File.ReadAllText(erroresJsonPath);
            Debug.Log("Archivo JSON de errores cargado correctamente. Contenido:");
            Debug.Log(jsonContent);
        } else { Debug.LogWarning($"El archivo JSON de errores no existe en la ruta: {erroresJsonPath}"); }

    }

    // Getter para acceder al contenido del JSON de errores
    public string GetErrors() { return erroresJsonPath; }

}
