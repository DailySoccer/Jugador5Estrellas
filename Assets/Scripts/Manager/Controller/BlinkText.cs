using UnityEngine;
using System.Collections;

class BlinkText : MonoBehaviour
{
    UILabel label;

    float nextChange = 0;


    void Start()
    {
        label = GetComponent<UILabel>();
        nextChange = Time.realtimeSinceStartup + 0.5f;
    }

    void Update()
    {
        if( Time.realtimeSinceStartup> nextChange) {
            nextChange = Time.realtimeSinceStartup + 0.5f;
            label.enabled = !label.enabled;
        }
    }




}
