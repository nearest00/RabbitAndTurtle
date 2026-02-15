// RRInputHandler.cs
using System;
using UnityEngine;

public class RRInputHandler : MonoBehaviour
{
    // 기존 게임 매니저(네 프로젝트에 맞는 타입)
    public RRGameManager gm;

    // 명시적으로 노트들이 부모로 들어있는 Transform을 연결하세요.
    // 예: Canvas_Play/PlayArea (Transform)
    public Transform noteParent;

    // 자동 발견용: NoteManager나 PlayArea 이름으로 찾아본다.
    public string fallbackNoteParentName = "PlayArea";

    void Start()
    {
        // 만약 inspector에서 noteParent를 연결하지 않았다면 시도해서 찾아본다.
        if (noteParent == null)
        {
            // 1) 먼저 NoteManager가 있으면 그 자식(생성된 노트)의 부모를 사용해본다.
            var nm = FindObjectOfType<R_NoteManager>();
            if (nm != null && nm.playArea != null)
            {
                noteParent = nm.playArea;
            }
            else
            {
                // 2) fallback 이름으로 Hierarchy에서 찾아본다.
                var go = GameObject.Find(fallbackNoteParentName);
                if (go != null) noteParent = go.transform;
            }
        }
    }


    void Update()
    {
        if (gm == null) return;
        double cur = gm.GetSongTime();

        // 눌렀을 때 (탭 or 롱노트 시작)
        if (Input.GetKeyDown(KeyCode.UpArrow))
            TryHitLane("up", cur);
        if (Input.GetKeyDown(KeyCode.DownArrow))
            TryHitLane("down", cur);

        // 누르고 있는 동안 (롱노트 유지)
        if (Input.GetKey(KeyCode.UpArrow))
            TryHoldLane("up", cur);
        if (Input.GetKey(KeyCode.DownArrow))
            TryHoldLane("down", cur);

        // 뗐을 때 (롱노트 종료)
        if (Input.GetKeyUp(KeyCode.UpArrow))
            TryReleaseLane("up", cur);
        if (Input.GetKeyUp(KeyCode.DownArrow))
            TryReleaseLane("down", cur);
    }

    /*void Update()
    {
        if (gm == null) return;

        double cur = gm.GetSongTime();

        *//*if (Input.GetKeyDown(KeyCode.UpArrow))
            TryHitLane("up", cur);
        if (Input.GetKeyDown(KeyCode.DownArrow))
            TryHitLane("down", cur);

        if (Input.GetKeyUp(KeyCode.UpArrow))
            TryReleaseLane("up", cur);
        if (Input.GetKeyUp(KeyCode.DownArrow))
            TryReleaseLane("down", cur);*//*

    }*/

    void TryHitLane(string lane, double time)
    {
        if (noteParent == null)
        {
            Debug.LogWarning("RRInputHandler: noteParent is not assigned and could not be found.");
            return;
        }

        // 가장 근접한 노트(시간 차 기준)를 찾는다.
        RRNote best = null;
        double bestDiff = double.MaxValue;

        foreach (Transform t in noteParent)
        {
            RRNote n = t.GetComponent<RRNote>();
            if (n == null) continue;
            if (string.IsNullOrEmpty(n.data.lane)) continue;

            // lane 비교: 소문자/대문자 허용
            if (!string.Equals(n.data.lane, lane, StringComparison.OrdinalIgnoreCase)) continue;

            double diffMs = Math.Abs((time - n.data.time) * 1000.0);
            if (diffMs < bestDiff)
            {
                bestDiff = diffMs;
                best = n;
            }
        }

        if (best != null)
        {
            best.OnHitAttempt(time);
        }
    }

    void TryReleaseLane(string lane, double time)
    {
        if (noteParent == null)
        {
            Debug.LogWarning("RRInputHandler: noteParent is not assigned and could not be found.");
            return;
        }

        foreach (Transform t in noteParent)
        {
            RRNote n = t.GetComponent<RRNote>();
            if (n == null) continue;
            if (!string.Equals(n.data.lane, lane, StringComparison.OrdinalIgnoreCase)) continue;
            if (!string.Equals(n.data.type, "long", StringComparison.OrdinalIgnoreCase)) continue;

            n.OnHoldRelease(time);
        }
    }

    void TryHoldLane(string lane, double time)
    {
        foreach (Transform t in gm.noteParent)
        {
            RRNote n = t.GetComponent<RRNote>();
            if (n == null) continue;
            if (n.data.lane != lane) continue;
            if (n.data.type != "long") continue;

            // 누르고 있는 동안 isBeingHeld를 true로 유지
            n.isBeingHeld = true;
        }
    }

}
