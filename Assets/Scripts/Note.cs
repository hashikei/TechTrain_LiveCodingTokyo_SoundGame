using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note : MonoBehaviour
{
    public struct Param
    {
        public float time;
        public Line line;

        public Param (float _time, Line _line) {
            time = _time;
            line = _line;
        }
    }

    public Param param { get; private set; }

    public void Initialize(Param _param) {
        param = _param;
    }
}
