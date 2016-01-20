// UGUIAtlasWnd
// Created by huangzw Oct/14/2014
// atlas maker wnd

using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class UGUIAtlasWnd : EditorWindow
{
    private const int TEXTURE_SIZE = 2048;
    private const int SPRITE_PADDING = 2;

    private object selectDirectory = null;
    private string packPath = "";
    private Texture2D[] selectTextures;
    private bool removeEmpty = true;

    // init
    public void Init()
    {
    }

    void OnSelectionChange()
    {
        packPath = "";
        selectTextures = null;

        Repaint();
    }

    void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Select Directory:");
        EditorGUILayout.EndHorizontal();

        // 判断selection的目录下是否有textures目录
        // 如果有，则显示，并列出所选择的textures文件
        if (null != Selection.objects
            && 1 == Selection.objects.Length
            && 1 == Selection.assetGUIDs.Length
            && selectDirectory != Selection.objects[0])
        {
            selectDirectory = Selection.objects[0];
            string path = AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs[0]);
            if (string.IsNullOrEmpty(path))
                return;

            string texturePath = path + "/Textures";
            if (Directory.Exists(texturePath))
            {
                packPath = path;
                selectTextures = GetTextures(texturePath);
            } else
                return;
        }

        if (! string.IsNullOrEmpty(packPath))
        {
            EditorGUILayout.BeginHorizontal();
            removeEmpty = EditorGUILayout.ToggleLeft("Remove empty", removeEmpty);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(packPath);

            if (GUILayout.Button("Pack"))
            {
                PackTextures(packPath + "/Data", Path.GetFileName(packPath), selectTextures, removeEmpty);
            }

            EditorGUILayout.EndHorizontal();
        }

        // 列出path / Textures 目录下所有的texture，不处理子目录
        if (null != selectTextures)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Textures:");
            EditorGUILayout.EndHorizontal();

            EditorGUI.indentLevel++;
            for (int i = 0; i < selectTextures.Length; i++)
            {
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField(selectTextures[i].name);
                EditorGUILayout.EndHorizontal();
            }
            EditorGUI.indentLevel--;
        }
    }

    Texture2D[] GetTextures(string path)
    {
        var files = AssetDatabase.FindAssets("t:Texture", new string[] { path });
        if (null == files)
            return null;

        List<Texture2D> texs = new List<Texture2D>();
        for (int i = 0; i < files.Length; i++)
        {
            string filePath = AssetDatabase.GUIDToAssetPath(files[i]);

            // texture modifty to can readable
            TextureImporter ti = (TextureImporter) TextureImporter.GetAtPath(filePath);
            if (null == ti)
                continue;

            do
            {
                bool bSet = false;

                if (! ti.isReadable)
                {
                    ti.isReadable = true;
                    bSet = true;
                }

                if (ti.npotScale != TextureImporterNPOTScale.None)
                {
                    ti.npotScale = TextureImporterNPOTScale.None;
                    bSet = true;
                }

                if (ti.textureFormat != TextureImporterFormat.ARGB32)
                {
                    ti.textureFormat = TextureImporterFormat.ARGB32;
                    bSet = true;
                }

                if (! bSet)
                    break;

                // reimport
                AssetDatabase.ImportAsset(filePath);
            } while (false);

            // load asset
            Texture2D tex = (Texture2D) AssetDatabase.LoadAssetAtPath(filePath, typeof(Texture2D));
            if (null == tex)
                continue;

            texs.Add(tex);
        }

        return texs.ToArray();
    }

    void PackTextures(string outPath, string name, Texture2D[] texs, bool removeEmpty)
    {
        if (null == texs || 0 == texs.Length)
            return;

        if (! Directory.Exists(outPath))
            Directory.CreateDirectory(outPath);

        Rect[] solidRect = null;
        Texture2D[] newTexs = removeEmpty ? CreateSprites(texs, out solidRect).ToArray() : texs;
        if (!removeEmpty)
        {
            solidRect = new Rect[newTexs.Length];
            for (int i = 0; i < solidRect.Length; ++i)
                solidRect[i] = Rect.MinMaxRect(0, 0, newTexs[i].width, newTexs[i].height);
        }

        // 生成新的texture
        Texture2D atlas = new Texture2D(TEXTURE_SIZE, TEXTURE_SIZE, TextureFormat.ARGB32, false);
        Rect[] rects = atlas.PackTextures(newTexs, SPRITE_PADDING, TEXTURE_SIZE);

        if (null == rects)
            // 打包失败
            return;

        int width = atlas.width;
        int height = atlas.height;

        // 生成spriteMetaData
        List<SpriteMetaData> lsMeta = new List<SpriteMetaData>();
        for (int i = 0; i < rects.Length; i++)
        {
            SpriteMetaData data = new SpriteMetaData();
            data.alignment = 0; // center
            data.border = Vector4.zero;
            data.name = newTexs[i].name;
            // data.pivot = Vector2.zero;
            float x = (texs[i].width / 2 - solidRect[i].xMin);
            float y = (texs[i].height / 2 - solidRect[i].yMax);
            data.alignment = (int)SpriteAlignment.Custom;
            data.pivot = new Vector2(Mathf.Abs(x) / newTexs[i].width, Mathf.Abs(y) / newTexs[i].height);
            data.rect = new Rect(rects[i].x * width,
                                 rects[i].y * height,
                                 rects[i].width * width,
                                 rects[i].height * height);


            lsMeta.Add(data);
        }

        // 写入文件
        byte[] bytes = atlas.EncodeToPNG();
        if (null == bytes)
            return;

        string texName = outPath + "/" + name + "Atlas.png";
        File.WriteAllBytes(texName, bytes);

        // 修改spritesheet
        AssetDatabase.Refresh();

        TextureImporter ti = (TextureImporter) TextureImporter.GetAtPath(texName);

        ti.maxTextureSize = TEXTURE_SIZE;
        ti.textureFormat = TextureImporterFormat.AutomaticTruecolor;
        ti.textureType = TextureImporterType.Sprite;
        ti.spriteImportMode = SpriteImportMode.Multiple;
        ti.spritesheet = ApplySpriteSheet(lsMeta.ToArray(), ti.spritesheet);
        ti.textureFormat = TextureImporterFormat.AutomaticTruecolor;

        AssetDatabase.ImportAsset(texName);
    }

    // 保存原来的配置中记录的border和pivot信息
    private SpriteMetaData[] ApplySpriteSheet(SpriteMetaData[] lsMeta, SpriteMetaData[] old)
    {
        for (int i = 0; i < lsMeta.Length; i++)
        {
            string name = lsMeta[i].name;
            for (int j = 0; j < old.Length; j++)
            {
                if (name == old[j].name)
                {
                    lsMeta[i].border = old[j].border;
                    // lsMeta[i].pivot = old[j].pivot;
                    break;
                }
            }
        }

        return lsMeta;
    }

    public List<Texture2D> CreateSprites(Texture2D[] textures, out Rect[] solidRect)
    {
        List<Texture2D> list = new List<Texture2D>();
        List<Rect> solidRectList = new List<Rect>();

        Texture2D tempTexture = null;

        for (int i = 0; i < textures.Length; i++)
        {
            Texture2D tex = textures[i];

            // If we want to trim transparent pixels, there is more work to be done
            Color32[] pixels = tex.GetPixels32();

            int xmin = tex.width;
            int xmax = 0;
            int ymin = tex.height;
            int ymax = 0;
            int oldWidth = tex.width;
            int oldHeight = tex.height;

            // Find solid pixels
            for (int y = 0, yw = oldHeight; y < yw; ++y)
            {
                for (int x = 0, xw = oldWidth; x < xw; ++x)
                {
                    Color32 c = pixels[y * xw + x];

                    if (c.a != 0)
                    {
                        if (y < ymin) ymin = y;
                        if (y > ymax) ymax = y;
                        if (x < xmin) xmin = x;
                        if (x > xmax) xmax = x;
                    }
                }
            }

            int newWidth  = (xmax - xmin) + 1;
            int newHeight = (ymax - ymin) + 1;

            if (newWidth > 0 && newHeight > 0)
            {
                // If the dimensions match, then nothing was actually trimmed
                if (newWidth == oldWidth && newHeight == oldHeight)
                {
                    tempTexture = tex;
                    tempTexture.name = tex.name;
                }
                else
                {
                    // Copy the non-trimmed texture data into a temporary buffer
                    Color32[] newPixels = new Color32[newWidth * newHeight];

                    for (int y = 0; y < newHeight; ++y)
                    {
                        for (int x = 0; x < newWidth; ++x)
                        {
                            int newIndex = y * newWidth + x;
                            int oldIndex = (ymin + y) * oldWidth + (xmin + x);

                            newPixels[newIndex] = pixels[oldIndex];
                        }
                    }

                    // Create a new texture
                    tempTexture = new Texture2D(newWidth, newHeight);
                    tempTexture.name = tex.name;
                    tempTexture.SetPixels32(newPixels);
                    tempTexture.Apply();
                }

                list.Add(tempTexture);
                solidRectList.Add(Rect.MinMaxRect(xmin, ymax, xmax, ymin));
            }
        }

        solidRect = solidRectList.ToArray();
        return list;
    }
}
