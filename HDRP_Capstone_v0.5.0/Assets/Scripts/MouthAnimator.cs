using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouthAnimator : MonoBehaviour
{
    public VowelDiscriminator vowelDiscriminator; // Reference to the VowelDiscriminator
    public SkinnedMeshRenderer skinnedMeshRenderer; // Reference to the SkinnedMeshRenderer for mouth animation
    public bool lipSyncToggle = false;


    //private float targetBlendShapeValue = 100.0f; // Target value for blend shapes
    private float lerpSpeed = 5f; // Speed of linear interpolation
    private float currentBlendShapeValue = 0.0f; // Current blend shape value
    private int currentVowelIndex = -1; // Index of the current vowel

    private string lastDetectedVowel = "";
    private int stableVowelCount = 0;
    private int vowelStabilityThreshold = 1; // Number of frames for a vowel to be considered stable

    // Update is called once per frame
    void Update()
    {
        if (lipSyncToggle)
        {
            // Check if vowelDiscriminator is set
            if (vowelDiscriminator == null)
            {
                Debug.LogError("VowelDiscriminator is not assigned.");
                return;
            }

            // Retrieve the detected vowel and loudness from the VowelDiscriminator
            string currentVowel = vowelDiscriminator.dominantVowel;
            float loudness = vowelDiscriminator.magnitude;

            // If below threshold, reset mouth to neutral position
            ResetMouthShape();
            // Update mouth based on the detected vowel
            UpdateMouthShape(currentVowel, loudness);

        }
    }

    //void UpdateMouthShape(string vowel, float loudness)
    //{
    //    // Map vowels to blend shape indices
    //    int vowelIndex = GetVowelIndex(vowel);

    //    if (vowelIndex != currentVowelIndex)
    //    {
    //        // Calculate target blend shape value based on loudness
    //        float blendValue = Mathf.Clamp(loudness * targetBlendShapeValue, 0, targetBlendShapeValue);

    //        // Interpolate towards the new vowel shape
    //        currentBlendShapeValue = Mathf.Lerp(currentBlendShapeValue, blendValue, lerpSpeed * Time.deltaTime);
    //        skinnedMeshRenderer.SetBlendShapeWeight(vowelIndex, currentBlendShapeValue);

    //        // Reset the previous vowel shape if different
    //        if (currentVowelIndex >= 0)
    //        {
    //            skinnedMeshRenderer.SetBlendShapeWeight(currentVowelIndex, 0);
    //        }

    //        // Update the current vowel index
    //        currentVowelIndex = vowelIndex;
    //    }
    //}
    void UpdateMouthShape(string vowel, float loudness)
    {
        // Map vowels to blend shape indices
        int vowelIndex = GetVowelIndex(vowel);

        // Check if the detected vowel has been consistent
        if (vowel == lastDetectedVowel)
        {
            stableVowelCount++;
        }
        else
        {
            lastDetectedVowel = vowel;
            stableVowelCount = 1;
        }

        if (stableVowelCount >= vowelStabilityThreshold)
        {
            // Calculate target blend shape value
            //float blendValue = Mathf.Clamp(loudness * 100, 0, targetBlendShapeValue);
            //float blendValue = loudness * 100;
            float blendValue = 60;

            // Interpolate towards the new vowel shape
            currentBlendShapeValue = Mathf.Lerp(currentBlendShapeValue, blendValue, lerpSpeed * Time.deltaTime);

            if (vowelIndex != currentVowelIndex)
            {
                // Gradually reduce the weight of the previous vowel shape
                if (currentVowelIndex >= 0)
                {
                    float previousBlendShapeValue = skinnedMeshRenderer.GetBlendShapeWeight(currentVowelIndex);
                    //previousBlendShapeValue = Mathf.Lerp(previousBlendShapeValue, 0, lerpSpeed * Time.deltaTime);
                    previousBlendShapeValue = 0;
                    skinnedMeshRenderer.SetBlendShapeWeight(currentVowelIndex, previousBlendShapeValue);
                }

                // Update the blend shape weight for the current vowel
                skinnedMeshRenderer.SetBlendShapeWeight(vowelIndex, currentBlendShapeValue);

                // Update the current vowel index
                currentVowelIndex = vowelIndex;
            }
        }
    }

    void ResetMouthShape()
    {
        if (currentVowelIndex >= 0)
        {
            // Get the current weight of the blend shape
            float currentWeight = skinnedMeshRenderer.GetBlendShapeWeight(currentVowelIndex);

            // Gradually interpolate the blend shape weight back to 0
            float newWeight = Mathf.Lerp(currentWeight, 0.0f, lerpSpeed * Time.deltaTime);

            skinnedMeshRenderer.SetBlendShapeWeight(currentVowelIndex, newWeight);

            // Check if the blend shape weight is close enough to zero to reset
            if (Mathf.Approximately(newWeight, 0.0f))
            {
                currentVowelIndex = -1;
                currentBlendShapeValue = 0.0f;
                lastDetectedVowel = "";
                stableVowelCount = 0;
            }
        }
    }


    int GetVowelIndex(string vowel)
    {
        switch (vowel.ToLower())
        {
            case "a": return 4;
            case "e": return 3;
            case "i": return 2;
            case "o": return 1;
            case "u": return 0;
            default: return -1; // Invalid or no vowel
        }
    }
}
