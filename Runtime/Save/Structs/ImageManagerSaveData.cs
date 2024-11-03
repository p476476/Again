using System;
using System.Collections.Generic;
using System.Linq;
using Again.Runtime.Common;
using UnityEngine;

namespace Again.Runtime.Save.Structs
{
    [Serializable]
    public class ImageObjectData
    {
        public string name;
        public Vector3 position;
        public Vector3 rotation;
        public Vector3 scale;
        public Vector2 sizeDelta;
        public string spriteName;
        public Color spriteColor;
    }

    public class ImageManagerSaveData
    {
        public string BackgroundImageName;
        public List<ImageObjectData> ImageObjectDataList;
        public bool IsBackgroundImageActive;

        public static string ToJson(Dictionary<string, GameObject> imageObjectDict)
        {
            var list = new List<ImageObjectData>();
            foreach (var item in imageObjectDict)
            {
                var rt = item.Value.GetComponent<RectTransform>();
                var renderer = item.Value.GetComponentInChildren<SpriteRenderer>();
                var imageObjectData = new ImageObjectData
                {
                    name = item.Key,
                    position = rt.localPosition,
                    rotation = rt.eulerAngles,
                    scale = rt.localScale,
                    sizeDelta = rt.sizeDelta,
                    spriteName = renderer.sprite.name,
                    spriteColor = renderer.color
                };
                list.Add(imageObjectData);
            }

            var str = JsonHelper.ToJson(list);
            return str;
        }

        public static ImageManagerSaveData FromJson(string json)
        {
            var imageObjectDataList = JsonHelper.FromJson<ImageObjectData>(json).ToList();
            return new ImageManagerSaveData
            {
                ImageObjectDataList = imageObjectDataList
            };
        }
    }
}