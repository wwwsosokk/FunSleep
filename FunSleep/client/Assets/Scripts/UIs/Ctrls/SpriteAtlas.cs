using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpriteAtlas : MonoBehaviour
{
    [System.Serializable]
    public class SpritePair
    {
        public string name;
        public Sprite sprite;
        public Vector2 pivot;

        public SpritePair(string name, Sprite sp, Vector2 pivot)
        {
            this.name = name;
            this.sprite = sp;
            this.pivot = pivot;
        }
    }

    // 图集
    [SerializeField] protected SpritePair[] sprites;
    protected Dictionary<string, SpritePair> spriteDic;

    [SerializeField] private Texture2D texture;

    public Texture2D Texture
    {
        get { return texture; }
        set { texture = value;}
    }

    protected void Awake()
    {
        // 创建一个sprite的Dictionary
#if ! ENV_EDITOR
        UpdateSprite();
#endif // UNITY_EDITOR
    }

#if UNITY_EDITOR
    public virtual void AddSprite(Sprite[] sp, Dictionary<string, Vector2> allPivot)
    {
        if (null == sp)
            return;

        List<SpritePair> ls = new List<SpritePair>();
        if (null == sprites)
        {
            sprites = new SpritePair[0];
        }

        for (int i = 0; i < sp.Length; i++)
        {
            bool found = false;
            for (int j = 0; j < sprites.Length; ++j)
            {
                if (sprites[j].sprite == sp[i])
                {
                    ls.Add(new SpritePair(sprites[j].name, sp[i], GetPivot(sp[i].name, allPivot)));
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                Vector2 pivot = GetPivot(sp[i].name, allPivot);
                SpritePair pair = new SpritePair(sp[i].name, sp[i], pivot);
                ls.Add(pair);
            }
        }

        // 移除不存在的数据
        for (int i = 0; i < ls.Count; ++i)
        {
            if (null == ls[i].sprite)
                ls.Remove(sprites[i]);
        }

        sprites = ls.ToArray();
    }

    protected Vector2 GetPivot(string name, Dictionary<string, Vector2> allPivot)
    {
        Vector2 pivot;
        if (!allPivot.TryGetValue(name, out pivot))
            pivot = new Vector2(0.5f, 0.5f);

        return pivot;
    }
#endif // UNITY_EDITOR

    public void UpdateSprite()
    {
        if (null != sprites)
        {
            spriteDic = new Dictionary<string, SpritePair>();
            for (int i = 0; i < sprites.Length; i++)
                spriteDic[sprites[i].name] = sprites[i];
        }
    }

    public Sprite GetSprite(string name)
    {
        SpritePair pair = GetSpritePair(name);
        if (null != pair)
            return pair.sprite;

        return null;
    }

    public SpritePair GetSpritePair(string name)
    {
        if (string.IsNullOrEmpty(name))
            return null;

        if (null == spriteDic)
            UpdateSprite();

        SpritePair sp = null;
        spriteDic.TryGetValue(name, out sp);
        return sp;
    }

    public SpritePair[] GetSprites()
    {
        return sprites;
    }
}

