using System;
using System.Collections.Generic;
using System.Linq;
using Again.Runtime.Common;
using Spine.Unity;
using UnityEngine;

namespace Again.Runtime.Save.Structs
{
    [Serializable]
    public class SpineObjectData
    {
        public string name;
        public Vector3 position;
        public Vector3 rotation;
        public Vector3 scale;
        public Vector2 sizeDelta;
        public Color materialColor;
        public Color materialBlackColor;
        public string animationName;
        public string skinName;
        public bool isLoop;
    }

    public class SpineManagerSaveData
    {
        public List<SpineObjectData> SpineObjectDataList;

        public static string ToJson(Dictionary<string, GameObject> spineObjectDict)
        {
            var list = new List<SpineObjectData>();
            foreach (var item in spineObjectDict)
            {
                var rt = item.Value.GetComponent<RectTransform>();
                var animation = item.Value.GetComponentInChildren<SkeletonAnimation>();
                var material = animation.skeletonDataAsset.atlasAssets[0].PrimaryMaterial;
                var spineObjectData = new SpineObjectData
                {
                    name = item.Key,
                    position = rt.localPosition,
                    rotation = rt.eulerAngles,
                    scale = rt.localScale,
                    sizeDelta = rt.sizeDelta,
                    materialColor = material.GetColor("_Color"),
                    materialBlackColor = material.GetColor("_Black"),
                    isLoop = animation.AnimationState.GetCurrent(0).Loop
                };
                list.Add(spineObjectData);
            }

            var str = JsonHelper.ToJson(list);
            return str;
        }

        public static SpineManagerSaveData FromJson(string json)
        {
            var spineObjectDataList = JsonHelper.FromJson<SpineObjectData>(json).ToList();
            return new SpineManagerSaveData
            {
                SpineObjectDataList = spineObjectDataList
            };
        }
    }
}