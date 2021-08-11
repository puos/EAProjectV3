﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class BGMGroup : MonoBehaviour
{
    [Serializable]
    public class BGMSlot
    {
        public string name;
        public bool   loop;

        [Range(0, 1)]
        public float volume;

        public AudioClip audioClip;
    }

    [Header("BGM Source")]
    public BGMSlot[] audioClip;

    public BGMSlot GetBGM(string name)
    {
        for(int i = 0; i < audioClip.Length; ++i)
        {
            if (audioClip[i].name.Equals(name, StringComparison.Ordinal)) return audioClip[i];
        }
        return null;
    }
}
