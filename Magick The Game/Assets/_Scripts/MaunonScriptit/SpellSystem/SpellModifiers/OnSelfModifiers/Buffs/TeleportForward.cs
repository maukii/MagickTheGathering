using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportForward : OnSelfModifier
{

    public float totalDuration = 1; // Duration for entire teleport sequence.
    public float warpFovDuration = 0.25f; // Duration to warp into/out of wide FOV.
    public float warpFov = 90; // Field of view while teleporting.

    [SerializeField] private float duration = 0.5f;
    [SerializeField] private float distance = 5.0f;

    private Spellbook spellbook;

    public override void AddSpellModifier(GameObject go, Spellbook spellbook)
    {
        go.AddComponent<TeleportForward>();
    }

    private void Start()
    {
        spellbook = GetComponent<Spellbook>();
        StartCoroutine(Teleport(gameObject));
    }

    private IEnumerator Teleport(GameObject go)
    {
        // Record initial state:
        var initialTime = Time.time;
        var initialPosition = transform.position;
        var initialFov = Camera.main.fieldOfView;

        Vector3 destination = transform.position + spellbook.GetDirection2() * distance;
        go.GetComponentInChildren<MeshRenderer>().enabled = false;

        // Teleport a little bit each frame:
        while (Time.time < initialTime + duration)
        {
            spellbook.isCasting = true;

            var elapsed = Time.time - initialTime;
            transform.position = Vector3.Lerp(initialPosition, destination, elapsed / totalDuration);
            if (Time.time < initialTime + warpFovDuration)
            {
                // Ease FOV into teleport mode:
                Camera.main.fieldOfView = Mathf.Lerp(initialFov, warpFov, elapsed / warpFovDuration);
            }
            else if (Time.time > initialTime + duration - warpFovDuration)
            {
                // Ease FOV out of teleport mode:
                var easeOutElapsed = elapsed - (duration - warpFovDuration);
                Camera.main.fieldOfView = Mathf.Lerp(warpFov, initialFov, easeOutElapsed / warpFovDuration);
            }
            yield return null;
        }

        go.GetComponentInChildren<MeshRenderer>().enabled = true;
        spellbook.StopCasting();
        spellbook.SetCooldown();

        Destroy(this);
    }
}
