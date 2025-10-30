using UnityEngine;

public enum AnswerType
{
    None,
    SpeakEnd,
    Quest,
    Barter,
    Aggressive,
    Sex,
    Doctor,
    Unarmed,
    Steal
}

public class Dialogue : MonoBehaviour
{
    public DialogueNode[] nodes;
    public int currentNode;
}

[System.Serializable]
public class DialogueNode
{
    public string maleNPCText;
    public string engMaleNPCText;
    public string indonesianMaleNPCText;
    public string femaleNPCText;
    public string engFemaleNPCText;
    public string indonesianFemaleNPCText;
    public Answer[] PlayerAnswer;
}


[System.Serializable]
public class Answer
{
    public AnswerType answerType;
    public string Text;
    public string EngText;
    public string IndonesianText;
    public string FemaleText;
    public string EngFemaleText;
    public string IndonesianFemaleText;
    public int ToNode;
    public int Quest;
    public int QuestPart;
    public bool NeedComplete;
    public bool DontNeedComplete;
    public bool NeedMan;

    public Fog Fog;
    public bool openFullMap;
    public Item Item;
    public bool destroyItem;
    public Door Door;
    public Container Container;
    public RepairObject RepairObject;
    public int NextRepairObjectIndex;
    public int Money;
    public int XP;
    public bool kill;
    public bool hasLeft;
    public string sceneName;

    public string attributeName;
    public bool addAttribute;

    public Skills Skill;
    public int SkillCheck;
    public int FailNode;
}