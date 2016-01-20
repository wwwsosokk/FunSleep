using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

[CanEditMultipleObjects]
[CustomEditor(typeof(SpriteAtlas))]
public class SpriteAtlasInspector : Editor
{
    private bool hideSprite = true;

    public override void OnInspectorGUI ()
    {
        SpriteAtlas atlas = target as SpriteAtlas;

        // sprite pair
        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField("Sprites", GUILayout.Width(40f));
        Texture2D tex = (Texture2D) EditorGUILayout.ObjectField(atlas.Texture, typeof(Texture2D), false);

        bool force = false;
        if (GUILayout.Button("Update", GUILayout.Width(80f)))
        {
            if (null != atlas.Texture)
            {
                string path = AssetDatabase.GetAssetPath(atlas.Texture.GetInstanceID());
                tex = (Texture2D) AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D));
                force = true;
            }
        }

        if (tex != atlas.Texture || force)
        {
            atlas.Texture = tex;

            if (null != tex)
            {
                string path = AssetDatabase.GetAssetPath(tex.GetInstanceID());
                Object[] assets = AssetDatabase.LoadAllAssetsAtPath(path);

                List<Sprite> ls = new List<Sprite>();
                for (int i = 0; i < assets.Length; i++)
                {
                    if (assets[i] is Texture2D)
                        continue;

                    ls.Add(assets[i] as Sprite);
                }

                string texPath = AssetDatabase.GetAssetPath(tex);
                TextureImporter ti = (TextureImporter)TextureImporter.GetAtPath(texPath);
                SpriteMetaData[] datas = ti.spritesheet;
                Dictionary<string, Vector2> allPivots = new Dictionary<string, Vector2>();
                for (int i = 0; i < datas.Length; ++i)
                {
                    allPivots[datas[i].name] = datas[i].pivot;
                }

                atlas.AddSprite(ls.ToArray(), allPivots);
            } else
                atlas.AddSprite(null, null);

            EditorUtility.SetDirty(target);
            AssetDatabase.SaveAssets();
        }

        EditorGUILayout.EndHorizontal();

        // show or hide
        EditorGUILayout.BeginHorizontal();

        if (hideSprite)
        {
            if (GUILayout.Button("Show Sprites"))
                hideSprite = false;
        } else
        {
            if (GUILayout.Button("Hide Sprites"))
                hideSprite = true;
        }

        EditorGUILayout.EndHorizontal();

        if (! hideSprite)
        {
            // 显示 sprite
            SpriteAtlas.SpritePair[] sps = atlas.GetSprites();

            if (null != sps)
            {
                for (int i = 0; i < sps.Length; i++)
                {
                    EditorGUILayout.BeginHorizontal();

                    sps[i].name = EditorGUILayout.TextField(sps[i].name);
                    sps[i].sprite = EditorGUILayout.ObjectField(sps[i].sprite, typeof(Sprite), false) as Sprite;

                    EditorGUILayout.EndHorizontal();
                }
            }
        }
    }
}
