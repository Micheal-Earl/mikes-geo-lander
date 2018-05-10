using UnityEngine;
using UnityEngine.SceneManagement;

public class RocketController : MonoBehaviour {

    [SerializeField] private float rotSpeed = 200f;
    [SerializeField] private float thrustSpeed = 40f;
    [SerializeField] private float levelLoadDelay = 1f;

    [SerializeField] private AudioClip mainEngine;
    [SerializeField] private AudioClip success;
    [SerializeField] private AudioClip death;

    [SerializeField] private ParticleSystem mainEngineParticle;
    [SerializeField] private ParticleSystem successParticle;
    [SerializeField] private ParticleSystem deathParticle;

    private bool collisionsAreDisabled = false;

    private Rigidbody rigidBody;
    private AudioSource audioSource;

    private enum State {
        Alive,
        Dying,
        Transcending
    };

    private State state = State.Alive;

    private void Start() {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Update() {
        if(state == State.Alive) {
            RespondToThrustInput();
            RespondToRotateInput();
        }

        if(Debug.isDebugBuild) {
            RespondToDebugInput();
        }
	}

    private void RespondToDebugInput() {
        if(Input.GetKeyDown(KeyCode.L)) {
            LoadNextScene();
        }

        if(Input.GetKeyDown(KeyCode.C)) {
            collisionsAreDisabled = !collisionsAreDisabled;
        }
    }

    private void OnCollisionEnter(Collision other) {
        if(state != State.Alive || collisionsAreDisabled) return;
        
        switch(other.gameObject.tag) {
            case "Friendly":
                // do nothing
                break;
            case "Finish":
                StartSuccessSequence();
                break;
            default:
                StartDeathSequence();
                break;
        }
    }

    private void StartSuccessSequence() {
        state = State.Transcending;
        audioSource.Stop();
        successParticle.Play();
        audioSource.PlayOneShot(success);
        Invoke("LoadNextScene", levelLoadDelay);
    }

    private void StartDeathSequence() {
        state = State.Dying;
        audioSource.Stop();
        deathParticle.Play();
        audioSource.PlayOneShot(death);
        Invoke("LoadFirstScene", levelLoadDelay - 0.5f);
    }

    private void LoadNextScene() {
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;
        if(sceneIndex+1 < SceneManager.sceneCountInBuildSettings) {
            SceneManager.LoadScene(sceneIndex + 1);
        } else {
            SceneManager.LoadScene(0);
        }
    }

    private void LoadFirstScene() {
        SceneManager.LoadScene(0);
        
    }

    private void RespondToRotateInput() {
        rigidBody.freezeRotation = true; // take control of the rotation
        
        float rotationThisFrame = rotSpeed * Time.deltaTime;

        if(Input.GetKey(KeyCode.A)) {
            transform.Rotate(Vector3.left * rotationThisFrame);
        }

        if(Input.GetKey(KeyCode.D)) {
            transform.Rotate(Vector3.right * rotationThisFrame);
        }

        rigidBody.freezeRotation = false;
    }

    private void RespondToThrustInput() {
        if(Input.GetKey(KeyCode.Space)) {
            ApplyThrust();
        } else {
            audioSource.Stop();
            mainEngineParticle.Stop();
        }
    }

    private void ApplyThrust() {
        rigidBody.AddRelativeForce(Vector3.up * thrustSpeed * Time.deltaTime);
        if(!audioSource.isPlaying) {
            audioSource.PlayOneShot(mainEngine);
        }
        mainEngineParticle.Play();
    }
}
