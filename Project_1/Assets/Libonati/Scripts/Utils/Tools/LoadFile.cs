using UnityEngine;
using System.Collections;
using System.Text;
using System.IO;
using System;

public class LoadFile : MonoBehaviour{
    public delegate void OnTextLoaded(string text);
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void LoadTextFile(LoadFileSettings settings, OnTextLoaded onResult, Encoding encoding = null){
        if (encoding == null) {
            encoding = Encoding.Default;
        }
        StartCoroutine(_LoadTextFile(settings, onResult, encoding));
    }
    private IEnumerator _LoadTextFile(LoadFileSettings settings, OnTextLoaded onResult, Encoding encoding) {
        string result = "";
        try {
            // Create a new StreamReader
            StreamReader theReader = new StreamReader(settings.fullPath, encoding);
            using (theReader) {
                result = theReader.ReadToEnd();
                theReader.Close();
            }
        }catch (Exception e) {
            Console.WriteLine("{0}\n", e.Message);
        }

        if (onResult != null) {
            onResult(result);
        }

        yield return null;
    }
}
public class LoadFileSettings{
    public string fileName = "";
    public string fileExtension = "";
    public string folder = "";
    public Environment.SpecialFolder folderRoot = Environment.SpecialFolder.MyDocuments;

    public void setFullPath(string fileName, Environment.SpecialFolder folderRoot, string folder, string fileExtension="") {
        this.fileName = fileName;
        this.folderRoot = folderRoot;
        this.folder = folder;
        if (fileExtension != "") {
            this.fileExtension = fileExtension;
        }
    }
    public string fullPath {
        get {
            return System.Environment.GetFolderPath(folderRoot) + "/" + folder + "/" + fileName + "." + fileExtension;
        }
    }
    public string folderPath {
        get {
            return System.Environment.GetFolderPath(folderRoot) + "/" + folder;
        }
    }
}