﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System.IO;
using UnityEngine.UI;

[System.Serializable]
struct SaveData
{
    public _ParticleMainModule       main;
    public _ParticleEmissionModule   emission;
    public _ParticleShape            shape;
    string                           texName;
}

/// <summary>
/// パーティクル一粒の基本設定
/// </summary>
[System.Serializable]
struct _ParticleMainModule
{
    public float        duration;                       // パーティクル発生時間                 
    public bool         loop;                           // パーティクル発生を継続するか
    public bool         prewarm;                        // あらかじめパーティクルを飛ばしておくか
    public ParamMode    startDelay;                     // 遅延時間
    public ParamMode    startLifeTime;                  // 寿命
    public ParamMode    startSpeed;                     // 初速度
    public ParamMode    startSize;                      // 初期サイズ
    public ParamMode    startRotation;                  // 初期回転角度
    public float        randomizeRotationDirection;     // 反対方向にスピンさせる（ちょっとよくわからない）
    public ColorMode    startColor;                     // 初期色
    public ParamMode    gravityModifier;                // 重力値
    public float        simulationSpeed;                // パーティクル発生のスピード（1を基本とする）
    public int          maxParticles;                   // 最大パーティクルの数
    public bool         randSeed;                       // 毎回バラバラかどうか
}

/// <summary>
/// パーティクルの発生間隔の設定
/// </summary>
[System.Serializable]
struct _ParticleEmissionModule
{
    public ParamMode    rateOverTime;   // 一秒間当たりのパーティクル発生数
    public Burst[]      bursts;         // パーティクルを一気に発生させるオプション
}

/// <summary>
/// パーティクルの放出方向の設定
/// </summary>
[System.Serializable]
struct _ParticleShape
{
    public Shape        shape;              // 放射の仕方
    // shape
    public bool         emitFromShell;      // shapeの表面から出すか
    // circle
    public ShapeCircleParam arc;
    // 共通
    public float radius;             // 半径(shape,circle)
    public bool         alignToDirection;   // ２Dでは使わない
    public float        randomizeDirection; // ランダム方向に飛ばす（0から1）
    public float        spherizeDirection;  // 中心から外側に向かって飛ばす(0から1)


}


// 共用体
//[System.Runtime.InteropServices.StructLayout(LayoutKind.Explicit)]
[System.Serializable]
struct ParamMode
{
    // 構造体の先頭アドレスを表す
   // [System.Runtime.InteropServices.FieldOffset(0)]
    public float constant;
    //[System.Runtime.InteropServices.FieldOffset(0)]
    public Curve curve;
    //[System.Runtime.InteropServices.FieldOffset(0)]
    public float constantTwo;
    //[System.Runtime.InteropServices.FieldOffset(0)]
    public Curve curveTwo;
}

[System.Serializable]
struct Curve
{
    public KeyInfo[] keys;
    public int       keyLength;
}

// TODO:この情報で曲線の情報の取り方が分からない
// とりあえず後回し
[System.Serializable]
struct KeyInfo
{
    public float inTangent;   // 前のキーフレームから、このキーフレームに近づくときの接線を設定します
    public float outTangent;  // 次のキーフレームに向かって、キーフレームを離れる際の接線を設定します
    public float time;        //キーフレームの時間
    public float value;       // キーフレームでの曲線の値
}

// 共用体
//[System.Runtime.InteropServices.StructLayout(LayoutKind.Explicit)]
[System.Serializable]
struct ColorMode
{
    //[System.Runtime.InteropServices.FieldOffset(0)]
    public Color colorMin;
    public Color colorMax;
}

// カラー情報
[System.Serializable]
struct Color
{
    public float r;
    public float b;
    public float g;
    public float a;
}

// Bursts
[System.Serializable]
struct Burst
{
    public int   minCount;
    public int   maxCount;
    public float time;
}

// Shapeの種類
[System.Serializable]
struct Shape
{
    public bool sphere;
    public bool hemiSphere;
    public bool cone;
    public bool box;
    public bool mesh;
    public bool meshRenderer;
    public bool skinnedMeshRendeerer;
    public bool circle;
    public bool edge;
}

// Shape = circleのパラメータ
[System.Serializable]
struct ShapeCircleParam
{
    public float        arc;
    public bool         random;
    public bool         loop;
    public bool         pingpong;
    public bool         burstSpeed;
    public float        arcSpread;
    public ParamMode    arcSpeed;
}


/// <summary>
/// パーティクルシステム
/// </summary>
public class ParticleController : MonoBehaviour {

    ParticleSystem m_particle;
    [SerializeField]
    Texture        m_tex;
    [SerializeField]
    Material       m_material;
    SaveData       m_saveData;
    // Use this for initialization
    void Start () {
        m_particle = GetComponent<ParticleSystem>();
        m_material = GetComponent<Renderer>().material;
        Debug.Log(m_particle);
        Debug.Log(m_material);
        Debug.Log(m_particle.main.startLifetime.mode);
	}

    // データ保存
    public void OnSaveParticleButton()
    {
        m_saveData = new SaveData();

        SetMainModule();
        SetEmission();
        SetShape();

        string json = JsonUtility.ToJson(m_saveData,true);
        string fileName = GameObject.FindGameObjectWithTag("FileName").GetComponent<Text>().text + ".json";
        
        File.WriteAllText("../RTS_verPC\\ParticleData\\" + fileName , json);
    }

    // パーティクルメインモジュール格納
    void SetMainModule()
    {
        m_saveData.main.duration                     = m_particle.main.duration;
        m_saveData.main.loop                         = m_particle.main.loop;
        m_saveData.main.prewarm                      = m_particle.main.prewarm;
        SetParam(ref m_saveData.main.startDelay         , m_particle.main.startDelay);
        SetParam(ref m_saveData.main.startLifeTime      , m_particle.main.startLifetime);
        SetParam(ref m_saveData.main.startSpeed         , m_particle.main.startSpeed);
        SetParam(ref m_saveData.main.startSize          , m_particle.main.startSize);
        SetParam(ref m_saveData.main.startRotation      , m_particle.main.startRotation);
        m_saveData.main.randomizeRotationDirection  = m_particle.main.randomizeRotationDirection;
        SetColor(ref m_saveData.main.startColor         , m_particle.main.startColor);
        SetParam(ref m_saveData.main.gravityModifier    , m_particle.main.gravityModifier);
        m_saveData.main.simulationSpeed             = m_particle.main.simulationSpeed;
        m_saveData.main.maxParticles                = m_particle.main.maxParticles;
        m_saveData.main.randSeed                    = true;
    }

    // パラメータセット
    void SetParam(ref ParamMode _mode, ParticleSystem.MinMaxCurve _data)
    {
        _mode.constant    = _data.constant;
        _mode.constantTwo = _data.constantMax;

        if (_data.curve == null) return;
        int keyNum = _data.curve.keys.Length;
        if (keyNum == 0) return;

        _mode.curve.keys = new KeyInfo[keyNum];
        for(int i = 0; i < keyNum; ++i)
        {
            _mode.curve.keys[i].inTangent  = _data.curve.keys[i].inTangent;
            _mode.curve.keys[i].outTangent = _data.curve.keys[i].outTangent;
            _mode.curve.keys[i].time       = _data.curve.keys[i].time;
            _mode.curve.keys[i].value      = _data.curve.keys[i].value;
        }
    }

    // カラーセット
    void SetColor(ref ColorMode _mode, ParticleSystem.MinMaxGradient _data)
    {
        // RGBA(_mode.colorMin, _data.colorMin);
        //RGBA(_mode.colorMax, _data.colorMax);
        _mode.colorMin.r = _data.color.r;
        _mode.colorMin.g = _data.color.g;
        _mode.colorMin.b = _data.color.b;
        _mode.colorMin.a = _data.color.a;

        _mode.colorMax.r = _data.color.r;
        _mode.colorMax.g = _data.color.g;
        _mode.colorMax.b = _data.color.b;
        _mode.colorMax.a = _data.color.a;
    }

    void RGBA(Color _color,UnityEngine.Color _data)
    {
        _color.r = _data.r;
        _color.g = _data.g;
        _color.b = _data.b;
        _color.a = _data.a;
    }

    // パーティクルエミッション格納
    void SetEmission()
    {
        SetParam(ref m_saveData.emission.rateOverTime, m_particle.emission.rateOverTime);
        int burstsNum = m_particle.emission.burstCount;
        if (burstsNum != 0)
        {
            m_saveData.emission.bursts   = new Burst[burstsNum];
            ParticleSystem.Burst[] burst = new ParticleSystem.Burst[burstsNum];
            m_particle.emission.GetBursts(burst);
            for (int i = 0; i < burstsNum; ++i)
            {
                m_saveData.emission.bursts[i].minCount = burst[i].minCount;
                m_saveData.emission.bursts[i].maxCount = burst[i].maxCount;
                m_saveData.emission.bursts[i].time     = burst[i].time;
            }
        }
    }

    // パーティクルシェイプ格納
    void SetShape()
    {
        InitShapeParam();
        switch (m_particle.shape.shapeType)
        {
            case ParticleSystemShapeType.Sphere:
                ShapeParam();
                break;
            case ParticleSystemShapeType.SphereShell:
                ShapeParam();
                break;
            case ParticleSystemShapeType.Hemisphere:
                ShapeParam();
                break;
            case ParticleSystemShapeType.HemisphereShell:
                ShapeParam();
                break;
            case ParticleSystemShapeType.Cone:
                ConeParam();
                break;
            case ParticleSystemShapeType.ConeShell:
                ConeParam();
                break;
            case ParticleSystemShapeType.ConeVolume:
                ConeParam();
                break;
            case ParticleSystemShapeType.ConeVolumeShell:
                ConeParam();
                break;
            case ParticleSystemShapeType.Box:
                BoxParam();
                break;
            case ParticleSystemShapeType.BoxShell:
                BoxParam();
                break;
            case ParticleSystemShapeType.BoxEdge:
                EdgeParam();
                break;
            case ParticleSystemShapeType.Mesh:
                MeshParam();
                break;
            case ParticleSystemShapeType.MeshRenderer:
                MeshRendererParam();
                break;
            case ParticleSystemShapeType.SkinnedMeshRenderer:
                SkinnedMeshRendererParam();
                break;
            case ParticleSystemShapeType.Circle:
                CircleParam();
                break;
            case ParticleSystemShapeType.CircleEdge:
                EdgeParam();
                break;
        }
    }
    void InitShapeParam()
    {
        m_saveData.shape.shape.sphere = false;
        m_saveData.shape.shape.hemiSphere = false;
        m_saveData.shape.shape.cone = false;
        m_saveData.shape.shape.box = false;
        m_saveData.shape.shape.mesh = false;
        m_saveData.shape.shape.meshRenderer = false;
        m_saveData.shape.shape.skinnedMeshRendeerer = false;
        m_saveData.shape.shape.circle = false;
        m_saveData.shape.shape.edge = false;
    }
    void ShapeParam()
    {
        m_saveData.shape.shape.sphere = true;
        m_saveData.shape.radius = m_particle.shape.radius;
        m_saveData.shape.emitFromShell = true;
        m_saveData.shape.alignToDirection = m_particle.shape.alignToDirection;
        m_saveData.shape.randomizeDirection = m_particle.shape.randomDirectionAmount;
        m_saveData.shape.spherizeDirection = m_particle.shape.sphericalDirectionAmount;
    }
    void ConeParam()
    {

    }
    void BoxParam()
    {

    }
    void MeshParam()
    {

    }
    void MeshRendererParam()
    {

    }
    void SkinnedMeshRendererParam()
    {

    }
    void CircleParam()
    {
        m_saveData.shape.shape.circle = true;
        m_saveData.shape.radius = m_particle.shape.radius;
        m_saveData.shape.arc.arc = m_particle.shape.arc;
        m_saveData.shape.arc.arcSpread = m_particle.shape.arcSpread;
        switch (m_particle.shape.arcMode)
        {
            case ParticleSystemShapeMultiModeValue.Random:
                m_saveData.shape.arc.random = true;
                break;
            case ParticleSystemShapeMultiModeValue.Loop:
                m_saveData.shape.arc.loop = true;
                SetParam(ref m_saveData.shape.arc.arcSpeed, m_particle.shape.arcSpeed);
                break;
            case ParticleSystemShapeMultiModeValue.PingPong:
                m_saveData.shape.arc.pingpong = true;
                SetParam(ref m_saveData.shape.arc.arcSpeed, m_particle.shape.arcSpeed);
                break;
            case ParticleSystemShapeMultiModeValue.BurstSpread:
                m_saveData.shape.arc.burstSpeed = true;
                break;
        }
    }
    void EdgeParam()
    {

    }
}
