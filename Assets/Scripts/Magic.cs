using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magic : MonoBehaviour {

    public ParticleSystem RaiseRocks_Particles;
    public LineRenderer LightningBolt;
    public Transform HeldLightningOrb;
    public float RaiseRocks_Range = 15;
    public float RaiseRocks_StrengthMultiplier = 5;
    public float RaiseRocks_ParticleStrengthMultiplier = .5f;
    public float RaiseRocks_RockAirDrag = 1.5f;
    public float RaiseRocks_FalloffOverTime = .333f;
    public float ThrowRocks_StrengthMultiplier = 2;

    List<GameObject> EffectedRocks = new List<GameObject>();
    ParticleSystem ParticleInstance;

    void Start() {

    }

    void Update() {
        List<GameObject> trashRocks = new List<GameObject>();
        foreach(GameObject rock in EffectedRocks) {
            Rigidbody reggiesBody = rock.GetComponent<Rigidbody>();
            GravityIGuess gravityScript = rock.GetComponent<GravityIGuess>();
            reggiesBody.drag = Mathf.MoveTowards(reggiesBody.drag, 0.05f, RaiseRocks_FalloffOverTime * Time.smoothDeltaTime);
            //if (gravityScript) gravityScript.GravityMultiplier = Mathf.MoveTowards(gravityScript.GravityMultiplier, 1, RaiseRocks_FalloffOverTime * Time.smoothDeltaTime);
            if (reggiesBody.drag <= 0.05f) {
                trashRocks.Add(rock);
                if(gravityScript == null) reggiesBody.useGravity = true;
                else gravityScript.UseGravity = true;
                reggiesBody.WakeUp();
            }
        }
        foreach(GameObject rock in trashRocks) {
            EffectedRocks.Remove(rock);
        }
    }

    void GetEffectedRocks(Vector3 origin, float strength) {
        GameObject[] bendableRocks = GameObject.FindGameObjectsWithTag("BendableRock");
        List<GameObject> NewEffectedRocks = new List<GameObject>();
        print("Found " + bendableRocks.Length + " rocks.");
        foreach(GameObject rock in bendableRocks) {
            float distance = (origin - rock.transform.position).magnitude;
            if (distance < RaiseRocks_Range) {
                float falloff = 1 - (distance / RaiseRocks_Range);
                Rigidbody reggiesBody = rock.GetComponent<Rigidbody>();
                GravityIGuess gravityScript = rock.GetComponent<GravityIGuess>();
                if(reggiesBody) {
                    NewEffectedRocks.Add(rock);
                    if (!EffectedRocks.Contains(rock))
                        EffectedRocks.Add(rock);

                    if(gravityScript)
                        //gravityScript.GravityMultiplier = 0;
                        gravityScript.UseGravity = false;
                    else
                        reggiesBody.useGravity = false;
                    reggiesBody.drag = RaiseRocks_RockAirDrag;
                    reggiesBody.WakeUp();
                    reggiesBody.AddForce(Vector3.up * falloff * strength, ForceMode.VelocityChange);
                    //reggiesBody.AddForce(Vector3.up * (Random.Range(95, 105) / 100) * falloff, ForceMode.VelocityChange);
                }
            }
        }
    }
    void ApplyForceToParticles(ParticleSystem particleEmitter, Vector3 velocityToApply, float RandomVariation = .2f) {
        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[particleEmitter.particleCount + 1];
        int what = particleEmitter.GetParticles(particles);
        for(int i = 0; i < what; i++) {
            float randomModifier = Random.Range((1 - RandomVariation) * 100, (1 + RandomVariation) * 100) / 100;
            particles[i].velocity += velocityToApply * randomModifier;
            //if ((particles[i].position - particleEmitter.transform.position).magnitude > RaiseRocks_Range * 2) {
            //    particles[i].remainingLifetime = 5;
            //}
            print(what);
        }
        particleEmitter.SetParticles(particles, what);
    }
    void GesturePerformed(PerformedGestureDetails details) {
        print(details.name);
        if(details.name == "RaiseRocks") {

            if(ParticleInstance) GameObject.Destroy(ParticleInstance.gameObject); ParticleInstance = null;
            if(ParticleInstance == null) {
                ParticleInstance = Instantiate(RaiseRocks_Particles);
                ParticleInstance.name = "RaiseRocks_Particles";
            }
            //ParticleInstance.Clear();
            ParticleInstance.transform.position = transform.position;
            var main = ParticleInstance.main;
            //main.gravityModifier = 0;
            //ParticleInstance.Emit(600);
            //ApplyForceToParticles(ParticleInstance, Vector3.up * details.MovedVectorMagnitude * RaiseRocks_StrengthMultiplier * RaiseRocks_ParticleStrengthMultiplier, .5f);

            GetEffectedRocks(details.CompletionLocation, details.MovedVectorMagnitude * RaiseRocks_StrengthMultiplier);
        } else if (details.name == "ThrowRocks") {
            if(EffectedRocks.Count > 0) {
                if(ParticleInstance) {
                    GameObject.Destroy(ParticleInstance.gameObject, 5);
                    Rigidbody reggiesBody = ParticleInstance.GetComponent<Rigidbody>();
                    reggiesBody.isKinematic = false;
                    reggiesBody.AddForce(details.MovedVector * ThrowRocks_StrengthMultiplier, ForceMode.VelocityChange);
                    //ApplyForceToParticles(ParticleInstance, details.MovedVector * ThrowRocks_StrengthMultiplier);
                    //var main = ParticleInstance.main;
                    //main.gravityModifier = 1;
                }
                foreach(GameObject rock in EffectedRocks) {
                    Rigidbody reggiesBody = rock.GetComponent<Rigidbody>();
                    if(reggiesBody) {
                        reggiesBody.drag = .05f;
                        reggiesBody.AddForce(details.MovedVector * ThrowRocks_StrengthMultiplier, ForceMode.VelocityChange);
                    }
                }
            }
        } else if (details.name == "SummonLightning") {
            LineRenderer bolt = Instantiate(LightningBolt);
            GestureTracker gestureTracker = GetComponent<GestureTracker>();
            Transform handTransform = gestureTracker.leftHand;
            if (details.hand == GestureTracker.Hand.Right)
                handTransform = gestureTracker.rightHand;

            bolt.transform.position = handTransform.position;
            bolt.positionCount = 2;
            bolt.SetPosition(0, handTransform.position);
            bolt.SetPosition(1, handTransform.position + (Vector3.up*10) + (gestureTracker.head.forward * 5) );
            GameObject.Destroy(bolt.gameObject, .25f);
        }
    }

}
