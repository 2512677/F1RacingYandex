using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DamageFX))]
public class DamageFXEditor : Editor
{
    // Основные разделы
    SerializedProperty pOptions;
    SerializedProperty pHealth;
    SerializedProperty pDamageIgnoreLayer;
    SerializedProperty pFindReceiversOnStart;
    SerializedProperty pFindExcludedsOnStart;
    SerializedProperty pVisualizeAsDefault;
    SerializedProperty pRootTransformForReceivers;
    SerializedProperty pRootTransformForExcludeds;
    SerializedProperty pDamageReceivers;
    SerializedProperty pExcludedReceivers;
    SerializedProperty pImpactSounds;
    SerializedProperty pParticleEffects;
    SerializedProperty pTextureEffects;
    SerializedProperty pRepairSpeed;
    SerializedProperty pDontApplyDamages;
    SerializedProperty pDebugStopOnCollision;
    SerializedProperty pLastImpactValue;

    // Флаги для сворачивания секций
    SerializedProperty pDfxEdVarsImpact;
    SerializedProperty pDfxEdVarsHealth;
    SerializedProperty pDfxEdVarsReceivers;
    SerializedProperty pDfxEdVarAudio;
    SerializedProperty pDfxEdVarsParticle;
    SerializedProperty pDfxEdVarsTexture;
    SerializedProperty pDfxEdVarsDebug;

    void OnEnable()
    {
        // найденные свойства
        pOptions = serializedObject.FindProperty("Options");
        pHealth = serializedObject.FindProperty("Health");
        pDamageIgnoreLayer = serializedObject.FindProperty("DamageIgnoreLayer");
        pFindReceiversOnStart = serializedObject.FindProperty("FindReceiversOnStart");
        pFindExcludedsOnStart = serializedObject.FindProperty("FindExcludedsOnStart");
        pVisualizeAsDefault = serializedObject.FindProperty("VisualizeAsDefault");
        pRootTransformForReceivers = serializedObject.FindProperty("RootTransformForReceivers");
        pRootTransformForExcludeds = serializedObject.FindProperty("RootTransformForExcludeds");
        pDamageReceivers = serializedObject.FindProperty("DamageReceivers");
        pExcludedReceivers = serializedObject.FindProperty("ExcludedReceivers");
        pImpactSounds = serializedObject.FindProperty("ImpactSounds");
        pParticleEffects = serializedObject.FindProperty("ParticleEffects");
        pTextureEffects = serializedObject.FindProperty("TextureEffects");
        pRepairSpeed = serializedObject.FindProperty("RepairSpeed");
        pDontApplyDamages = serializedObject.FindProperty("DontApplyDamages");
        pDebugStopOnCollision = serializedObject.FindProperty("DebugStopOnCollision");
        pLastImpactValue = serializedObject.FindProperty("LastImpactValue");

        pDfxEdVarsImpact = serializedObject.FindProperty("dfxEdVarsImpact");
        pDfxEdVarsHealth = serializedObject.FindProperty("dfxEdVarsHealth");
        pDfxEdVarsReceivers = serializedObject.FindProperty("dfxEdVarsReceivers");
        pDfxEdVarAudio = serializedObject.FindProperty("dfxEdVarAudio");
        pDfxEdVarsParticle = serializedObject.FindProperty("dfxEdVarsParticle");
        pDfxEdVarsTexture = serializedObject.FindProperty("dfxEdVarsTexture");
        pDfxEdVarsDebug = serializedObject.FindProperty("dfxEdVarsDebug");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Заголовок
        GUILayout.BeginVertical("box");
        GUILayout.Label("DamageFX Manager", EditorStyles.boldLabel);
        GUILayout.Label("DamageFX v1.6", EditorStyles.miniLabel);
        GUILayout.EndVertical();

        // Impact Settings
        pDfxEdVarsImpact.boolValue = EditorGUILayout.Foldout(pDfxEdVarsImpact.boolValue, "Impact Settings");
        if (pDfxEdVarsImpact.boolValue)
        {
            EditorGUILayout.PropertyField(pDamageIgnoreLayer);
            EditorGUILayout.PropertyField(pOptions.FindPropertyRelative("MaxImpactPower"));
            EditorGUILayout.PropertyField(pOptions.FindPropertyRelative("DeformRadius"));
            EditorGUILayout.PropertyField(pOptions.FindPropertyRelative("DeformManipulator"));
            EditorGUILayout.PropertyField(pOptions.FindPropertyRelative("MaxDeform"));
            EditorGUILayout.PropertyField(pOptions.FindPropertyRelative("VerticalDeformBias"));
        }

        // Vehicle Health
        pDfxEdVarsHealth.boolValue = EditorGUILayout.Foldout(pDfxEdVarsHealth.boolValue, "Vehicle Health");
        if (pDfxEdVarsHealth.boolValue)
        {
            EditorGUILayout.PropertyField(pHealth.FindPropertyRelative("VehicleHealth"), new GUIContent("Total Health"));
            EditorGUILayout.PropertyField(pHealth.FindPropertyRelative("ImpactToHealthPercent"), new GUIContent("Impact To Health %"));
            EditorGUILayout.PropertyField(pHealth.FindPropertyRelative("EngineSmokePercent"), new GUIContent("Engine Smoke Start %"));
            EditorGUILayout.PropertyField(pRepairSpeed, new GUIContent("Repair Speed"));
        }

        // Meshes
        pDfxEdVarsReceivers.boolValue = EditorGUILayout.Foldout(pDfxEdVarsReceivers.boolValue, "Meshes");
        if (pDfxEdVarsReceivers.boolValue)
        {
            EditorGUILayout.PropertyField(pFindReceiversOnStart);
            EditorGUILayout.PropertyField(pFindExcludedsOnStart);
            EditorGUILayout.PropertyField(pVisualizeAsDefault);
            EditorGUILayout.PropertyField(pRootTransformForReceivers);
            EditorGUILayout.PropertyField(pRootTransformForExcludeds);
            EditorGUILayout.PropertyField(pDamageReceivers, true);
            EditorGUILayout.PropertyField(pExcludedReceivers, true);
        }

        // Audio Settings
        pDfxEdVarAudio.boolValue = EditorGUILayout.Foldout(pDfxEdVarAudio.boolValue, "Audio Settings");
        if (pDfxEdVarAudio.boolValue)
        {
            EditorGUILayout.PropertyField(pImpactSounds.FindPropertyRelative("EnableAudio"));
            EditorGUILayout.PropertyField(pImpactSounds.FindPropertyRelative("RolloffMode"));
            EditorGUILayout.PropertyField(pImpactSounds.FindPropertyRelative("MinDistance"));
            EditorGUILayout.PropertyField(pImpactSounds.FindPropertyRelative("MaxDistance"));
            EditorGUILayout.PropertyField(pImpactSounds.FindPropertyRelative("MaxVolume"));
            EditorGUILayout.PropertyField(pImpactSounds.FindPropertyRelative("Spatial"));
            EditorGUILayout.PropertyField(pImpactSounds.FindPropertyRelative("TapCollisionAudio"));
            EditorGUILayout.PropertyField(pImpactSounds.FindPropertyRelative("LightCollisionValue"));
            EditorGUILayout.PropertyField(pImpactSounds.FindPropertyRelative("LightCollisionAudio"));
            EditorGUILayout.PropertyField(pImpactSounds.FindPropertyRelative("MedCollisionValue"));
            EditorGUILayout.PropertyField(pImpactSounds.FindPropertyRelative("MedCollisionAudio"));
            EditorGUILayout.PropertyField(pImpactSounds.FindPropertyRelative("HeavyCollisionValue"));
            EditorGUILayout.PropertyField(pImpactSounds.FindPropertyRelative("HighCollisionAudio"));
        }

        // Particle Settings
        pDfxEdVarsParticle.boolValue = EditorGUILayout.Foldout(pDfxEdVarsParticle.boolValue, "Particle Settings");
        if (pDfxEdVarsParticle.boolValue)
        {
            EditorGUILayout.PropertyField(pParticleEffects.FindPropertyRelative("EnableParticleEffects"));
            EditorGUILayout.PropertyField(pParticleEffects.FindPropertyRelative("GetFromScene"));
            EditorGUILayout.PropertyField(pParticleEffects.FindPropertyRelative("ImpactSparks"));
            EditorGUILayout.PropertyField(pParticleEffects.FindPropertyRelative("BrokenEngineSmoke"));
        }

        // Texture Effects
        pDfxEdVarsTexture.boolValue = EditorGUILayout.Foldout(pDfxEdVarsTexture.boolValue, "Texture Effects");
        if (pDfxEdVarsTexture.boolValue)
        {
            EditorGUILayout.PropertyField(pTextureEffects.FindPropertyRelative("EnableTextureEffects"));
            EditorGUILayout.PropertyField(pTextureEffects.FindPropertyRelative("ReferenceObject"));
            EditorGUILayout.PropertyField(pTextureEffects.FindPropertyRelative("CheckEveryMaterialHaveProperty"));
            EditorGUILayout.PropertyField(pTextureEffects.FindPropertyRelative("MaterialPropertyName"));
        }

        // Debug
        pDfxEdVarsDebug.boolValue = EditorGUILayout.Foldout(pDfxEdVarsDebug.boolValue, "Debug");
        if (pDfxEdVarsDebug.boolValue)
        {
            EditorGUILayout.PropertyField(pDontApplyDamages);
            EditorGUILayout.PropertyField(pDebugStopOnCollision);
            EditorGUILayout.PropertyField(pLastImpactValue);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
