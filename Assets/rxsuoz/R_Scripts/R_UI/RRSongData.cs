using UnityEngine;

[CreateAssetMenu(fileName = "SongData", menuName = "Rhythm/SongData")]
public class RRSongData : ScriptableObject
{
    public string songTitle;
    public AudioClip musicClip;

    // 난이도별 채보 파일 (TextAsset CSV)
    public TextAsset chartEasy;
    public TextAsset chartNormal;
    public TextAsset chartHard;

    // "easy", "normal", "hard" 중 하나로 설정
    public string difficulty;
}
