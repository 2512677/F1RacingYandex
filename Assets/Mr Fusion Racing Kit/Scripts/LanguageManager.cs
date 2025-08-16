using I2.Loc;

public static class LanguageManager
{
    /// <summary>
    /// ѕолучает локализованную строку дл€ типа гонки.
    /// </summary>
    public static string GetLocalizedRaceType(RaceType raceType)
    {
        return LocalizationManager.GetTranslation($"RaceType/{raceType}");
    }

    /// <summary>
    /// ѕолучает локализованную строку дл€ уровн€ сложности.
    /// </summary>
    public static string GetLocalizedDifficulty(AIDifficultyLevel difficultyLevel)
    {
        return LocalizationManager.GetTranslation($"DifficultyLevel/{difficultyLevel}");
    }

    /// <summary>
    /// ”станавливает текущий €зык.
    /// </summary>
    public static void SetLanguage(string languageCode)
    {
        LocalizationManager.CurrentLanguage = languageCode;
    }
}
