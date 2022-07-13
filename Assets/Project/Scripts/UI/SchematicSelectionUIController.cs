using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction.ComprehensiveSample;
using UnityEngine;

public class SchematicSelectionUIController : MonoBehaviour
{
    [SerializeField]
    private UICanvasCurves _introAnimation;

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(5f);
        _introAnimation.Animate(true);
    }
}
