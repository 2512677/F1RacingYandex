using UnityEngine;

namespace RGSK
{
    [CreateAssetMenu(fileName = "Новые детали ИИ", menuName = "MRFE/New AI Details", order = 1)]
    public class AiDetails : ScriptableObject
    {
        public AIDetial[] aiDetials;
    }

    [System.Serializable]
    public class AIDetial
    {
        public string name;
        public Nationality nationality;
    }
}