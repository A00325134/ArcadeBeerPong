using UnityEngine;

public class BallBounce : MonoBehaviour
{
    public AudioSource bounceSound;

    private void OnCollisionEnter(Collision collision)
    {
        bounceSound.PlayOneShot(bounceSound.clip);
    }
}
