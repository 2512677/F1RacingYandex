using UnityEngine;

[CreateAssetMenu(fileName = "Новая сложность ИИ", menuName = "MRFE/New AI Difficulty", order = 1)]
public class AiDifficulty : ScriptableObject
{
    [Range(0, 1)]public float throttleSensitivity;
    [Range(0, 1)]public float brakeSensitivity;
    [Range(0, 1)]public float steerSensitivity;
    [Range(0.85f, 1)]public float speedModifier;
}
