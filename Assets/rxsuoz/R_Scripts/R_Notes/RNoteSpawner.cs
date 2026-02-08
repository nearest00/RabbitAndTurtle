using UnityEngine;

// 채보 데이터를 읽어서 노트를 생성하는 클래스
public class RNoteSpawner : MonoBehaviour
{
    // 각 노트 타입별 Prefab (0: TapNote, 1: HoldNote, 2: RapidNote)
    public GameObject[] notePrefabs = new GameObject[3];

    // 각 레인의 부모 Transform (0: LeftLane, 1: DownLane)
    public Transform[] spawnParents = new Transform[2];

    // 각 레인의 스폰 위치 (0: LeftLane, 1: DownLane)
    public Vector2[] spawnPositions = new Vector2[2];

    // 로드된 채보 데이터
    private RChartData chartData;

    // 현재 생성할 노트의 인덱스
    private int noteIndex = 0;

    // 게임 시작 시간
    private float startTime = 0;

    // 노트 이동 속도
    private float noteSpeed = 500f;

    // 씬 전환 시에도 유지되도록 설정
    void Awake()
    {
        // 부모에서 분리한 뒤 DontDestroyOnLoad 호출 (경고 방지)
        transform.SetParent(null);
        DontDestroyOnLoad(gameObject);
    }

    // 채보 로드 및 노트 생성 시작
    public void StartSpawning(string chartFileName, float speed)
    {
        LoadChart(chartFileName);
        noteSpeed = speed;
        startTime = Time.time;
        noteIndex = 0;
    }

    void Update()
    {
        // 아직 생성할 노트가 남아있는지 확인
        if (chartData != null && chartData.notes != null && noteIndex < chartData.notes.Length)
        {
            // 현재 경과 시간
            float currentTime = Time.time - startTime;

            // 다음 노트를 스폰해야 하는 시간
            float spawnTime = chartData.notes[noteIndex].time - (500f / noteSpeed);

            // 스폰 시간이 되었으면 노트 생성
            if (currentTime >= spawnTime)
            {
                SpawnNote(chartData.notes[noteIndex]);
                noteIndex++;
            }
        }
    }

    // JSON 파일에서 채보 데이터 로드
    void LoadChart(string chartFileName)
    {
        // Resources/Charts 폴더에서 JSON 파일 로드
        TextAsset textAsset = Resources.Load<TextAsset>("Charts/" + chartFileName);

        if (textAsset != null)
        {
            // JSON을 ChartData 객체로 변환
            chartData = JsonUtility.FromJson<RChartData>(textAsset.text);
            Debug.Log($"Chart loaded: {chartFileName}, Notes: {chartData.notes.Length}");
        }
        else
        {
            Debug.LogError($"Chart file not found: {chartFileName}\n" +
                           $"확인하세요: Assets/Resources/Charts/{chartFileName}.json");
        }
    }

    // 노트 생성 메소드
    void SpawnNote(RNoteData noteData)
    {
        int noteType = noteData.type;
        int key = noteData.key;

        // Prefab을 인스턴스화하여 노트 생성
        GameObject noteObject = Instantiate(notePrefabs[noteType], spawnParents[key]);

        // RectTransform 설정
        RectTransform noteRect = noteObject.GetComponent<RectTransform>();
        noteRect.anchoredPosition = spawnPositions[key];

        // Note 컴포넌트 설정
        RNote note = noteObject.GetComponent<RNote>();
        note.exactTime = startTime + noteData.time;
        note.noteSpeed = noteSpeed;
        note.SetNoteType(key);

        // 홀드 노트인 경우 지속 시간 설정
        if (noteType == 1)  // HoldNote
        {
            RHoldNote holdNote = noteObject.GetComponent<RHoldNote>();
            holdNote.holdDuration = noteData.duration;
        }

        // 연타 노트인 경우 지속 시간 설정
        if (noteType == 2)  // RapidNote
        {
            RRapidNote rapidNote = noteObject.GetComponent<RRapidNote>();
            rapidNote.duration = noteData.duration;
        }
    }
}
