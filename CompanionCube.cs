using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class CompanionCube : MonoBehaviour
{

    private enum Behaviours { Following, Waiting, Sulking, Suffering};

    [SerializeField] GameObject sulkingSpot;

    NavMeshAgent agent;
    [SerializeField] GameObject player;
    [SerializeField] float AITickTime = 2.0f;
    Vector3 targetLoc;

    Behaviours myBehaviour = Behaviours.Following;

    float timeWaiting = 0.0f;


    //Sounds
    AudioSource voice;
    public AudioClip[] HelloSounds;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        voice = GetComponent<AudioSource>();
        InvokeRepeating("TickAI", 0.0f, AITickTime);

    }

    void TickAI()
    {
        switch (myBehaviour)
        {
            case Behaviours.Following:
                if (CheckForLineOfSight())
                {
                    MoveToTarget();
                } else
                {
                    myBehaviour = Behaviours.Waiting;
                }
                break;
            case Behaviours.Waiting:
                timeWaiting += AITickTime;
                if (CheckForLineOfSight())
                {
                    if(timeWaiting > 5.0f)
                    {
                        myBehaviour = Behaviours.Sulking;
                    } else
                    {
                        myBehaviour = Behaviours.Following;
                        MoveToTarget();
                        timeWaiting = 0.0f;

                        // play an I found you sound
                        voice.clip = HelloSounds[Random.Range(0, HelloSounds.Length - 1)];
                        voice.Play();
                    }
                }
                break;
            case Behaviours.Sulking:
                print("I am sulking");
                agent.SetDestination(sulkingSpot.transform.position);
                break;
            case Behaviours.Suffering:
                break;
        }
    }

    bool CheckForLineOfSight()
    {
        RaycastHit hit;
        Vector3 startPos = transform.position;
        Vector3 direction = Vector3.Normalize(player.transform.position - startPos);

        Ray ray = new Ray(startPos, direction);

        if(Physics.Raycast(ray, out hit))
        {
            if(hit.collider.gameObject != player)
            {
                print(hit.collider.name + "is in the way");
                return false;
                
            }
        }
        return true;
    }

    public void RespondToNoise()
    {
        print("I have heard you");
        if(myBehaviour == Behaviours.Waiting && timeWaiting < 5.0f)
        {
            myBehaviour = Behaviours.Following;
            MoveToTarget();
        } else
        {
            myBehaviour = Behaviours.Sulking;
        }
    }

    void MoveToTarget()
    {
        print("Moving to target");
        targetLoc = player.transform.position;
        agent.SetDestination(targetLoc);
    }
}
