using UnityEngine;

public class AudioListenerManager : MonoBehaviour
{
    public GameObject mainListener; // Assign the main listener object in the inspector

    void OnEnable()
    {
        // Disable all other AudioListeners at the start
        AudioListener[] listeners = FindObjectsOfType<AudioListener>();
        foreach (AudioListener listener in listeners)
        {
            if (listener.gameObject != mainListener)
            {
                listener.enabled = false;
            }
        }

        // Ensure the main listener is always enabled
        if (mainListener.GetComponent<AudioListener>() != null)
        {
            mainListener.GetComponent<AudioListener>().enabled = true;
        }
    }
}

