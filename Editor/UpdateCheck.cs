using UnityEditor;
using UnityEditor.PackageManager.Requests;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Networking;

namespace Gaze.VirtualPointer.Editor
{
    [InitializeOnLoad]
    internal class UpdateCheck
    {
        static UpdateCheck()
        {
            CheckVersion();
        }

        static private void CheckVersion()
        {
            using (UnityWebRequest www = UnityWebRequest.Get("https://raw.githubusercontent.com/RenanDresch/UnityVirtualPointer/master/README.md"))
            {
                www.SendWebRequest();

                while (www.responseCode <= 0) { }

                if (www.isNetworkError)
                {
                    Debug.Log(www.error);
                }
                else
                {
                    var result = www.downloadHandler.text;

                    if (string.IsNullOrEmpty(result))
                    {
                        CheckVersion();
                        return;
                    }

                    var listRequest = Client.List(true);
                    while (!listRequest.IsCompleted) { }

                    foreach (var package in listRequest.Result)
                    {
                        if (package.name.Contains("com.gazeinteractive.virtualpointer"))
                        {
                            if (result.Contains(package.version))
                            {
                                return;
                            }
                            else
                            {
                                if (EditorUtility.DisplayDialog("Update!", "A new version of the Virtual Pointer package is available!\nDo you wish to update it?", "Yes", "No"))
                                {
                                    var removeRequest = Client.Remove("com.gazeinteractive.virtualpointer");
                                    EditorUtility.DisplayProgressBar("Removing old package...", "", -1);
                                    while (!removeRequest.IsCompleted)
                                    {
                                        EditorUtility.DisplayProgressBar("Removing old package...", removeRequest.Status.ToString(), -1);
                                    }
                                    EditorUtility.ClearProgressBar();
                                    TryAddPackage();
                                }
                                return;
                            }
                        }
                    }
                }
            }
        }

        static private void TryAddPackage()
        {
            EditorUtility.DisplayProgressBar("Updating", "", -1);
            var addRequest = Client.Add("https://github.com/RenanDresch/UnityVirtualPointer.git#upm");
            while (!addRequest.IsCompleted) 
            {
                EditorUtility.DisplayProgressBar("Updating Virtual Pointer package...", addRequest.Status.ToString(), -1);
            }
            EditorUtility.ClearProgressBar();
            if (addRequest.Status == StatusCode.Success)
            {
                EditorUtility.DisplayDialog("Update complete!", "Virtual Pointer up to date!", "Return");
            }
            else
            {
                if (EditorUtility.DisplayDialog("Update Failed!", "Failed to update the Virtual Poitner package!\nDo you wish to try again?", "Yes", "No"))
                {
                    TryAddPackage();
                }
            }
        }
    }
}