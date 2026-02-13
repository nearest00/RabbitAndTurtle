using UnityEngine;

[CreateAssetMenu(fileName = "SongData", menuName = "Rhythm/SongData")]
public class RRSongData : ScriptableObject
{
    public string songTitle;
    public AudioClip musicClip;     // 음악 파일
    public TextAsset chartCsv;      // 채보 파일
    public string difficulty;       // "easy", "normal", "hard"
}
