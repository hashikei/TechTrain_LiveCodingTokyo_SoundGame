using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour
{
    [SerializeField] private GameObject notePrefab;
    [SerializeField] private GameObject judgeLineObj;

    [SerializeField] private AudioSource bgm;
    [SerializeField] private AudioSource se;

    [SerializeField] private Text scoreText;
    [SerializeField] private Combo comboClass;
    [SerializeField] private JudgeText[] judgeTexts;

    private static readonly List<Note.Param> MUSIC_MASTER = new List<Note.Param>() {
        { new Note.Param(1f, Line.Line1) },
        { new Note.Param(2f, Line.Line2) },
        { new Note.Param(3f, Line.Line3) },
        { new Note.Param(4f, Line.Line4) },
        { new Note.Param(5f, Line.Line1) },
        { new Note.Param(6f, Line.Line2) },
        { new Note.Param(7f, Line.Line3) },
        { new Note.Param(8f, Line.Line4) },
    };

    private static readonly Dictionary<Line, float> LINE_POSITION = new Dictionary<Line, float>() {
        { Line.Line1, -6f },
        { Line.Line2, -2f },
        { Line.Line3, 2f },
        { Line.Line4, 6f },
    };

    private static readonly Dictionary<Judge, float> JUDGE_THRESHOLD = new Dictionary<Judge, float>() {
        { Judge.Perfect, 0.1f },
        { Judge.Great, 0.2f },
        { Judge.Good, 0.3f },
        { Judge.Miss, 0.5f },
    };

    private static readonly Dictionary<Judge, int> JUDGE_SCORE = new Dictionary<Judge, int>() {
        { Judge.Perfect, 1000 },
        { Judge.Great, 500 },
        { Judge.Good, 100 },
        { Judge.Miss, 0 },
    };

    private static readonly float BASE_SPEED = 10f;

    private static readonly Dictionary<Line, List<Note>> notesLineList = new Dictionary<Line, List<Note>>();

    private static readonly Dictionary<Line, int> currentNoteList = new Dictionary<Line, int>();

    private int score;
    private int combo;

    // Start is called before the first frame update
    void Start() {
        Initialize();
    }

    // Update is called once per frame
    void Update() {
        //if (Input.GetKeyDown(KeyCode.Return)) {
        //    SceneManager.LoadScene("Result");
        //}

        foreach (var notes in notesLineList.Values) {
            foreach (var note in notes) {
                var pos = note.transform.position;
                pos.y = judgeLineObj.transform.position.y + (note.param.time - bgm.time) * BASE_SPEED;
                note.transform.position = pos;
            }
        }

        foreach (Line line in System.Enum.GetValues(typeof(Line))) {
            int index = currentNoteList[line];
            if (index < 0)
                continue;

            var note = notesLineList[line][index];
            float diff = bgm.time - note.param.time;

            if (diff > JUDGE_THRESHOLD[Judge.Miss]) {
                combo = 0;
                judgeTexts[(int)line].Draw(Judge.Miss);
                if (currentNoteList[note.param.line] + 1 < notesLineList[line].Count) {
                    ++currentNoteList[note.param.line];
                } else {
                    currentNoteList[note.param.line] = -1;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.A)) {
            JudgeNotes(Line.Line1);
        }
        if (Input.GetKeyDown(KeyCode.S)) {
            JudgeNotes(Line.Line2);
        }
        if (Input.GetKeyDown(KeyCode.D)) {
            JudgeNotes(Line.Line3);
        }
        if (Input.GetKeyDown(KeyCode.F)) {
            JudgeNotes(Line.Line4);
        }
    }

    void Initialize() {
        score = 0;
        combo = 0;

        foreach (Line line in System.Enum.GetValues(typeof(Line))) {
            notesLineList.Add(line, new List<Note>());
            currentNoteList.Add(line, -1);
        }

        foreach (var data in MUSIC_MASTER) {
            var obj = Instantiate(notePrefab, transform);
            var note = obj.GetComponent<Note>();
            note.Initialize(data);

            var pos = obj.transform.position;
            pos.x = LINE_POSITION[note.param.line];
            pos.y = judgeLineObj.transform.position.y + note.param.time * BASE_SPEED;
            note.transform.position = pos;

            notesLineList[note.param.line].Add(note);
            if (currentNoteList[note.param.line] < 0) {
                currentNoteList[note.param.line] = 0;
            }
        }

        bgm.Play();
    }

    void JudgeNotes(Line line) {
        se.Play();

        if (currentNoteList[line] < 0)
            return;

        var note = notesLineList[line][currentNoteList[line]];
        float diff = Mathf.Abs(bgm.time - note.param.time);

        if (diff > JUDGE_THRESHOLD[Judge.Miss]) {
            return;
        }

        var judge = Judge.Miss;
        if (diff < JUDGE_THRESHOLD[Judge.Perfect]) {
            judge = Judge.Perfect;
        } else if (diff < JUDGE_THRESHOLD[Judge.Great]) {
            judge = Judge.Great;
        } else if (diff < JUDGE_THRESHOLD[Judge.Good]) {
            judge = Judge.Good;
        }

        score += JUDGE_SCORE[judge];
        scoreText.text = "Score:" + score.ToString("D6");

        judgeTexts[(int)line].Draw(judge);

        if (judge != Judge.Miss) {
            ++combo;
            comboClass.Draw(combo);
        } else {
            combo = 0;
        }

        if (currentNoteList[note.param.line] + 1 < notesLineList[line].Count) {
            ++currentNoteList[note.param.line];
        } else {
            currentNoteList[note.param.line] = -1;
        }
    }
}
