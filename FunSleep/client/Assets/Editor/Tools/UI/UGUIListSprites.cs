using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

class UGUIListSprites : Editor
{
    [MenuItem("GameEditor/UGUI/ListSprites")]
    private static void ListSprites()
    {
        ListSprits(Selection.gameObjects);
    }

    private static void ListSprits(GameObject[] gameObjects)
    {
        if (null == gameObjects || gameObjects.Length <= 0)
            return;

        string basePath = Path.Combine(Application.dataPath, "../dump_sprites");
        if (!Directory.Exists(basePath))
            Directory.CreateDirectory(basePath);

        for (int i = 0; i < gameObjects.Length; ++i)
        {
            Image[] allImages = gameObjects[i].GetComponentsInChildren<Image>(true);
            string path = Path.Combine(basePath, gameObjects[i].name + "_sprites.txt");
            DumpImages(allImages, path);
        }

        Debug.Log("<color=red>List sprites ok.</color>");
    }

    private static string GetFullTransformName(Transform t)
    {
        if (null == t.parent)
            return t.name;

        return GetFullTransformName(t.parent) + "/" + t.name;
    }

    private static void DumpImages(Image[] allImages, string path)
    {
        if (null == allImages || allImages.Length <= 0)
            return;

        using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
        {
            Dictionary<string, List<string>> allImageSet = new Dictionary<string, List<string>>();
            for (int i = 0; i < allImages.Length; ++i)
            {
                Image image = allImages[i];
                if (null == image)
                    continue;

                Sprite sprite = image.sprite;
                if (null == sprite)
                    continue;

                string key = string.Format("Sprite:{0} - Atlas:{1}", sprite.name, sprite.texture.name);
                List<string> objs;
                if (!allImageSet.TryGetValue(key, out objs))
                {
                    objs = new List<string>();
                    allImageSet.Add(key, objs);
                }

                objs.Add(GetFullTransformName(image.transform));
            }

            foreach (var pair in allImageSet)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(pair.Key);
                sb.Append(" - ");
                sb.Append("Name:");
                for (int i = 0; i < pair.Value.Count; ++i)
                {
                    sb.Append(pair.Value[i]);
                    sb.Append(", ");
                }
                sb.Append("\n");
                byte[] allBytes = Encoding.UTF8.GetBytes(sb.ToString());
                fs.Write(allBytes, 0, allBytes.Length);
            }
        }

        Debug.Log("Dump sprites path:" + path); 
    }
}
