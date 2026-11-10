using System;
using System.Collections;
using UnityEngine;

// RoughHouse V5: humanoid-asset based combat vertical-slice architecture.
// NO generated stick figures. Supply genuine rigged character prefabs through the editor wizard.
public sealed class RoughHousePrototype : MonoBehaviour
{
    private HumanoidFighter soho, slick;
    private FightCamera cameraRig;
    private Coroutine demo;
    private bool ready, manual, paused;
    private string title = "ROUGH HOUSE / HUMAN FIGHTERS";
    private string caption = "Loading humanoid character assets...";
    private GUIStyle heading, body, warning;

    private void Start()
    {
        Application.targetFrameRate = 60;
        MakeArena();
        var a = Resources.Load<GameObject>("Fighters/Soho");
        var b = Resources.Load<GameObject>("Fighters/Slick");
        if (a == null || b == null)
        {
            caption = "HUMAN ASSETS MISSING: Use RoughHouse > Configure Human Fighters in Unity Editor.";
            Debug.LogError("RoughHouse V5: missing Assets/Resources/Fighters/Soho.prefab or Slick.prefab. Import a rigged human and clips using the editor wizard before Cloud Build.");
            return;
        }
        soho = HumanoidFighter.Spawn("SOHO", a, new Vector3(-1.18f, .05f, 0));
        slick = HumanoidFighter.Spawn("SLICK", b, new Vector3(1.18f, .05f, 0));
        if (!soho.Valid || !slick.Valid)
        {
            caption = "HUMANOID SETUP INVALID: Check imported Animator and animation controller.";
            Debug.LogError("RoughHouse V5: character prefab must have an Animator with a generated controller, from Configure Human Fighters.");
            return;
        }
        soho.Opponent = slick;
        slick.Opponent = soho;
        Camera c = Camera.main;
        if (c == null)
        {
            GameObject obj = new GameObject("Fight camera"); obj.tag = "MainCamera"; c = obj.AddComponent<Camera>();
        }
        c.clearFlags = CameraClearFlags.SolidColor;
        c.backgroundColor = new Color(.075f, .09f, .12f);
        c.fieldOfView = 42f;
        cameraRig = c.gameObject.AddComponent<FightCamera>();
        cameraRig.SetTargets(soho.transform, slick.transform);
        ready = true;
        Debug.Log("RoughHouse V5: TWO HUMANOID fighters ready, collision controllers initialized.");
        StartDemo(1);
    }
    private void Update()
    {
        if (!ready) return;
        if (Input.GetKeyDown(KeyCode.Alpha1)) StartDemo(1);
        if (Input.GetKeyDown(KeyCode.Alpha2)) StartDemo(2);
        if (Input.GetKeyDown(KeyCode.Space)) { paused = !paused; Time.timeScale = paused ? 0f : 1f; }
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (demo != null) StopCoroutine(demo);
            demo = null; manual = true;
            caption = "FREE PLAY: WASD movement, J jab, K cross, L hook, I uppercut, U kick, O block";
            soho.Rest(); slick.Rest();
        }
        if (manual && !paused)
        {
            Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"),0f,Input.GetAxisRaw("Vertical"));
            soho.Walk(input, 3.1f);
            if (Input.GetKeyDown(KeyCode.J)) StartCoroutine(Strike(soho,slick,"Jab",.37f,1.55f, .14f));
            if (Input.GetKeyDown(KeyCode.K)) StartCoroutine(Strike(soho,slick,"Cross",.48f,1.65f,.18f));
            if (Input.GetKeyDown(KeyCode.L)) StartCoroutine(Strike(soho,slick,"Hook",.66f,1.65f,.30f));
            if (Input.GetKeyDown(KeyCode.I)) StartCoroutine(Strike(soho,slick,"Uppercut",.73f,1.5f,.38f));
            if (Input.GetKeyDown(KeyCode.U)) StartCoroutine(Strike(slick,soho,"Kick",.68f,2f,.3f));
            if (Input.GetKeyDown(KeyCode.O)) soho.Play("Block");
        }
    }
    private void OnDisable() { Time.timeScale = 1f; }
    private void StartDemo(int which)
    {
        if (!ready) return;
        if (demo != null) StopCoroutine(demo);
        manual = false; paused = false; Time.timeScale = 1f;
        soho.ResetAt(new Vector3(-1.18f,.05f,0f));
        slick.ResetAt(new Vector3(1.18f,.05f,0f));
        title = which == 1 ? "CLIP 1 / LOCK-ON BOXING" : "CLIP 2 / COMBAT SHOWCASE";
        caption = which == 1 ? "Guard, circle, close distance and exchange punches" : "Jab - Cross - Hook - Uppercut - Knockdown - Counter-kick";
        demo = StartCoroutine(which == 1 ? ClipOne() : ClipTwo());
    }
    private IEnumerator ClipOne()
    {
        yield return new WaitForSeconds(.7f);
        yield return Circle(.9f,1f);
        yield return Strike(soho,slick,"Jab",.44f,1.65f,.12f);
        yield return new WaitForSeconds(.22f);
        yield return Strike(soho,slick,"Cross",.52f,1.65f,.18f);
        yield return Circle(1.5f,-1f);
        yield return Strike(slick,soho,"Jab",.45f,1.65f,.13f);
        yield return Strike(soho,slick,"Hook",.65f,1.6f,.27f);
        yield return Circle(1.2f,1f);
        caption = "Clip 1 complete. Press 2 for the second demonstration.";
    }
    private IEnumerator ClipTwo()
    {
        yield return new WaitForSeconds(.75f);
        yield return Strike(soho,slick,"Jab",.39f,1.65f,.1f);
        yield return Strike(soho,slick,"Cross",.45f,1.65f,.16f);
        yield return Strike(soho,slick,"Hook",.62f,1.75f,.25f);
        yield return Strike(soho,slick,"Uppercut",.78f,1.65f,.33f);
        yield return new WaitForSeconds(.4f);
        slick.Play("Knockdown", 1.5f);
        yield return new WaitForSeconds(1.6f);
        slick.Play("GetUp",1.1f);
        yield return new WaitForSeconds(1.3f);
        yield return Strike(slick,soho,"Kick",.72f,2.15f,.28f);
        yield return Strike(slick,soho,"Kick",.8f,2.15f,.31f);
        caption = "Clip 2 complete. Press 1 to replay movement.";
    }
    private IEnumerator Circle(float seconds, float clockwise)
    {
        float elapsed=0;
        while (elapsed < seconds)
        {
            elapsed += Time.deltaTime;
            Vector3 separation = soho.transform.position - slick.transform.position;
            separation.y=0;
            if (separation.sqrMagnitude < .1f) separation = Vector3.left;
            Vector3 tangent = Vector3.Cross(Vector3.up,separation.normalized)*clockwise;
            soho.Walk(tangent, 1.4f);
            slick.Walk(-tangent, .7f);
            yield return null;
        }
        soho.Rest(); slick.Rest();
    }
    private IEnumerator Strike(HumanoidFighter attacker, HumanoidFighter defender, string move, float duration, float range, float knockback)
    {
        if (!ready || attacker == null || defender == null || attacker.Busy) yield break;
        // Close just enough to deliver a plausible contact rather than striking from a distance.
        float approach = 0f;
        while (Vector3.Distance(attacker.transform.position,defender.transform.position) > range*.82f && approach < 1.25f)
        {
            approach += Time.deltaTime;
            Vector3 delta = defender.transform.position-attacker.transform.position;
            delta.y=0;
            attacker.Walk(delta, 2.7f);
            yield return null;
        }
        attacker.Rest(); attacker.Play(move,duration);
        caption = attacker.DisplayName + " / " + move.ToUpperInvariant();
        float elapsed=0f; bool checkedHit=false;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            if (!checkedHit && elapsed >= duration*.43f)
            {
                checkedHit=true;
                Vector3 delta=defender.transform.position-attacker.transform.position; delta.y=0f;
                float dist=delta.magnitude;
                bool facing=dist>.01f && Vector3.Dot(attacker.transform.forward,delta.normalized)>.35f;
                if (dist <= range && facing && defender.CanBeHit)
                {
                    defender.TakeHit(move == "Uppercut" ? "HeavyHit" : "Hit",knockback,delta.normalized);
                    if (cameraRig!=null) cameraRig.Impact(.07f);
                }
                else Debug.Log("RoughHouse V5: " + move + " missed (range or facing). Distance="+dist.ToString("F2"));
            }
            yield return null;
        }
        attacker.EndAttack();
    }
    private static void MakeArena()
    {
        var world=new GameObject("RoughHouse Arena / physical collider floor");
        MakeCube(world.transform,"Ground", new Vector3(0,-.30f,0),new Vector3(16,.6f,12),new Color(.19f,.20f,.23f));
        MakeCube(world.transform,"Fight mat",new Vector3(0,-.045f,0),new Vector3(9,.09f,7),new Color(.26f,.28f,.32f));
        for (int i=-4;i<=4;i++)
            MakeCube(world.transform,"Mat lines",new Vector3(i,.008f,0),new Vector3(.025f,.018f,6.8f),new Color(.33f,.37f,.39f));
        for (int i=-3;i<=3;i++)
            MakeCube(world.transform,"Mat lines",new Vector3(0,.008f,i),new Vector3(8.8f,.018f,.025f),new Color(.33f,.37f,.39f));
        for (int i=0;i<64;i++)
        {
            float theta=i*Mathf.PI*2f/64f;
            float theta2=(i+1)*Mathf.PI*2f/64f;
            Vector3 a=new Vector3(Mathf.Cos(theta)*3.6f,.025f,Mathf.Sin(theta)*2.7f);
            Vector3 b=new Vector3(Mathf.Cos(theta2)*3.6f,.025f,Mathf.Sin(theta2)*2.7f);
            var edge=MakeCube(world.transform,"Ring",(a+b)*.5f,new Vector3(.06f,.025f,Vector3.Distance(a,b)+.02f),new Color(.92f,.58f,.23f));
            edge.transform.rotation=Quaternion.LookRotation(b-a);
        }
        MakeCube(world.transform,"Rear wall",new Vector3(0,3.4f,5.8f),new Vector3(16,7,.25f),new Color(.09f,.105f,.14f));
        Light key=new GameObject("Arena lighting").AddComponent<Light>(); key.type=LightType.Directional;
        key.transform.rotation=Quaternion.Euler(48,-35,0); key.intensity=1.6f;
        RenderSettings.ambientLight = new Color(.52f,.54f,.62f);
    }
    private static Shader stageShader;
    private static GameObject MakeCube(Transform parent,string name,Vector3 pos,Vector3 scale,Color color)
    {
        GameObject obj=GameObject.CreatePrimitive(PrimitiveType.Cube);
        obj.name=name; obj.transform.SetParent(parent,false); obj.transform.localPosition=pos; obj.transform.localScale=scale;
        if (stageShader==null)
        {
            stageShader=Resources.Load<Shader>("RoughHouseFlatColor");
            if(stageShader==null) stageShader=Shader.Find("Sprites/Default");
        }
        if(stageShader != null)
        {
            var m=new Material(stageShader); m.color=color; obj.GetComponent<MeshRenderer>().sharedMaterial=m;
        }
        return obj; // Unity's primitive BoxCollider remains enabled.
    }
    private void OnGUI()
    {
        if(heading==null)
        {
            heading=new GUIStyle(GUI.skin.label) {fontSize=25,fontStyle=FontStyle.Bold,normal={textColor=Color.white}};
            body=new GUIStyle(GUI.skin.label) {fontSize=15,normal={textColor=new Color(.72f,.91f,.96f)}};
            warning=new GUIStyle(GUI.skin.label) {fontSize=17,wordWrap=true,normal={textColor=new Color(1f,.60f,.3f)}};
        }
        GUI.Box(new Rect(18,18,Mathf.Min(750,Screen.width-36),140),"");
        GUI.Label(new Rect(32,29,700,34),title,heading);
        GUI.Label(new Rect(32,67,700,35),caption,ready?body:warning);
        GUI.Label(new Rect(32,116,730,30),"[1] Clip 1    [2] Clip 2    [P] Free play    [Space] Pause",body);
    }
}

public sealed class HumanoidFighter : MonoBehaviour
{
    public HumanoidFighter Opponent;
    public string DisplayName { get; private set; }
    public bool Valid { get; private set; }
    public bool Busy { get { return Time.time < busyUntil; } }
    public bool CanBeHit { get { return Time.time > groundedUntil; } }
    private CharacterController controller;
    private Animator animator;
    private Transform visual;
    private float busyUntil,groundedUntil;
    private Vector3 home;
    private bool moved;
    private string lastState;
    public static HumanoidFighter Spawn(string name,GameObject prefab,Vector3 position)
    {
        var root=new GameObject(name+" / human fighter / collision root");
        root.transform.position=position;
        var cc=root.AddComponent<CharacterController>(); cc.center=new Vector3(0, .93f,0); cc.height=1.86f; cc.radius=.32f; cc.skinWidth=.045f; cc.stepOffset=.23f;
        cc.slopeLimit=45f;
        var fighter=root.AddComponent<HumanoidFighter>(); fighter.DisplayName=name; fighter.controller=cc; fighter.home=position;
        GameObject model=Instantiate(prefab,root.transform); model.name=name+" / skinned human model";
        fighter.visual=model.transform;
        fighter.animator=model.GetComponentInChildren<Animator>();
        if(fighter.animator != null)
        {
            fighter.animator.applyRootMotion=false;
            fighter.Valid=fighter.animator.runtimeAnimatorController!=null;
        }
        // Scale feet to the mat without altering colliders or the world y coordinate.
        var renderers=model.GetComponentsInChildren<Renderer>();
        if(renderers.Length>0)
        {
            float feet=float.PositiveInfinity;
            foreach(var renderer in renderers) feet=Mathf.Min(feet,renderer.bounds.min.y);
            if(!float.IsInfinity(feet)) model.transform.position += Vector3.up*(position.y-feet+.015f);
        }
        fighter.Play("Idle");
        return fighter;
    }
    private void Update()
    {
        if(Opponent!=null)
        {
            Vector3 delta=Opponent.transform.position-transform.position; delta.y=0;
            if(delta.sqrMagnitude>.025f)
                transform.rotation=Quaternion.RotateTowards(transform.rotation,Quaternion.LookRotation(delta),720f*Time.deltaTime);
        }
        if(controller!=null && controller.enabled)
        {
            if(!controller.isGrounded) controller.Move(Vector3.down * 5f * Time.deltaTime);
            // CharacterControllers do not automatically collide with one another. Resolve separation explicitly.
            if(Opponent != null)
            {
                Vector3 d=transform.position-Opponent.transform.position; d.y=0;
                if(d.sqrMagnitude>.0001f && d.magnitude<.78f)
                    controller.Move(d.normalized*(.78f-d.magnitude)*.5f);
            }
        }
        if(moved) { moved=false; }
    }
    public void Walk(Vector3 direction,float speed)
    {
        if(Busy || controller==null || !controller.enabled) return;
        direction.y=0;
        if(direction.sqrMagnitude>.001f)
        {
            direction.Normalize();
            Vector3 candidate=transform.position+direction*speed*Time.deltaTime;
            if(Mathf.Abs(candidate.x)>4f || Mathf.Abs(candidate.z)>3f) return;
            controller.Move(direction*speed*Time.deltaTime);
            Play("Walk"); moved=true;
        }
        else Rest();
    }
    public void Rest() { if(!Busy) Play("Idle"); }
    public void EndAttack() { busyUntil=0f; lastState=null; Play("Idle"); }
    public void Play(string state,float lockSeconds=0f)
    {
        if(animator==null || animator.runtimeAnimatorController==null) return;
        if(!animator.HasState(0,Animator.StringToHash("Base Layer."+state)) && !animator.HasState(0,Animator.StringToHash(state)))
        {
            Debug.LogWarning("RoughHouse V5: missing humanoid animation state '"+state+"' for "+DisplayName);
            return;
        }
        if(lastState != state) animator.CrossFadeInFixedTime(state,.10f,0,0f);
        lastState=state;
        busyUntil=Mathf.Max(busyUntil,Time.time+lockSeconds);
    }
    public void TakeHit(string state,float push,Vector3 away)
    {
        Play(state,state=="HeavyHit"?.62f:.3f);
        if(controller!=null) controller.Move(away*push);
        if(state=="HeavyHit") groundedUntil=Time.time+.4f;
    }
    public void ResetAt(Vector3 pos)
    {
        controller.enabled=false; transform.position=pos; controller.enabled=true;
        busyUntil=groundedUntil=0f; lastState=null; Play("Idle");
    }
}

public sealed class FightCamera : MonoBehaviour
{
    private Transform first,second;
    private Vector3 velocity;
    private float impulse;
    public void SetTargets(Transform a,Transform b) {first=a;second=b;}
    public void Impact(float amount) {impulse=Mathf.Max(impulse,amount);}
    private void LateUpdate()
    {
        if(first==null || second==null) return;
        Vector3 mid=(first.position+second.position)*.5f;
        float gap=Vector3.Distance(first.position,second.position);
        Vector3 desired=mid+new Vector3(0,2.65f,-Mathf.Clamp(5.7f+gap*.45f,6.2f,8.5f));
        transform.position=Vector3.SmoothDamp(transform.position,desired,ref velocity,.16f);
        if(impulse>0f) { transform.position+=UnityEngine.Random.insideUnitSphere*impulse; impulse=Mathf.MoveTowards(impulse,0,Time.deltaTime*.5f); }
        transform.LookAt(mid+Vector3.up*1.1f);
    }
}
