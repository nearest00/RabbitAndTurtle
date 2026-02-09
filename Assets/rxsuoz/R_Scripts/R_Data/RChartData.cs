using UnityEngine;

// 개별 노트의 데이터를 정의하는 클래스
[System.Serializable]
public class RNoteData
{
    public int key;           // 0: 왼쪽 레인, 1: 아래쪽 레인
    public float time;        // 노트가 나타날 시간 (초 단위)
    public int type;          // 0: 탭 노트, 1: 홀드 노트, 2: 연타 노트
    public float duration;    // 홀드/연타 지속 시간 (초)
}

// 전체 채보 데이터를 정의하는 클래스
[System.Serializable]
public class RChartData
{
    public RNoteData[] notes;  // 모든 노트의 배열
}
