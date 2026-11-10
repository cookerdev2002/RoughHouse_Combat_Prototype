using System;
using System.Collections;
using UnityEngine;

// ROUGH HOUSE V6: Self-contained, playable combat proof-of-concept.
// Male humanoid geometry is authored in code; no third-party FBX is bundled.
// All motions are procedural demonstrations, not motion capture.
public sealed class RoughHousePrototype : MonoBehaviour
{
    private RHFighter soho, slick;
    private Camera fightCamera;
    private Coroutine sequence;
    private bool freePlay, paused;
    private string status = "Setting up fighters";
    private string sceneName = "CLIP 1 / SOHO VS SLICK";
    private float shake;
    private GUIStyle titleStyle, textStyle;
    private static Shader baseShader;
    private static Material matCache;

    private void Start()
    {
        Application.targetFrameRate = 60;
        Time.timeScale = 1f;
        CreateArena();
        soho = RHFighter.Create("SOHO", true, new Vector3(-1.05f, 0f, 0f));
        slick = RHFighter.Create("SLICK", false, new Vector3(1.05f, 0f, 0f));
        soho.enemy = slick; slick.enemy = soho;
        fightCamera = Camera.main;
        if (fightCamera == null) { GameObject o = new GameObject("RoughHouse Combat Camera"); o.tag="MainCamera"; fightCamera = o.AddComponent<Camera>(); }
        fightCamera.clearFlags = CameraClearFlags.SolidColor;
        fightCamera.backgroundColor = new Color(.055f,.066f,.083f);
        fightCamera.fieldOfView = 42f;
        fightCamera.nearClipPlane = .1f;
        fightCamera.farClipPlane = 80f;
        if (fightCamera.GetComponent<AudioListener>() == null) fightCamera.gameObject.AddComponent<AudioListener>();
        Debug.Log("ROUGH HOUSE V6 READY: male humanoid fighters, procedural combat and collision spacing initialized.");
        PlayDemo(1);
    }
    private void Update()
    {
        if (soho == null || slick == null) return;
        if(Input.GetKeyDown(KeyCode.Alpha1)) PlayDemo(1);
        if(Input.GetKeyDown(KeyCode.Alpha2)) PlayDemo(2);
        if(Input.GetKeyDown(KeyCode.Space)) { paused=!paused; Time.timeScale=paused?0f:1f; }
        if(Input.GetKeyDown(KeyCode.P))
        {
            if(sequence!=null) StopCoroutine(sequence);
            sequence=null; freePlay=true; Time.timeScale=1f; paused=false;
            soho.ResetPose(); slick.ResetPose();
            status="FREE PLAY: WASD move / J jab / K cross / L hook / I uppercut / U kick / O block";
            sceneName="FREE PLAY / SOHO VS SLICK";
        }
        if(freePlay)
        {
            Vector3 direction = new Vector3(Input.GetAxisRaw("Horizontal"),0,Input.GetAxisRaw("Vertical"));
            soho.Move(direction,3f*Time.deltaTime);
            if(Input.GetKeyDown(KeyCode.J)) StartCoroutine(Attack(soho,slick,"Jab",.40f,1.7f,.16f));
            if(Input.GetKeyDown(KeyCode.K)) StartCoroutine(Attack(soho,slick,"Cross",.46f,1.8f,.20f));
            if(Input.GetKeyDown(KeyCode.L)) StartCoroutine(Attack(soho,slick,"Hook",.57f,1.8f,.25f));
            if(Input.GetKeyDown(KeyCode.I)) StartCoroutine(Attack(soho,slick,"Uppercut",.70f,1.75f,.36f));
            if(Input.GetKeyDown(KeyCode.U)) StartCoroutine(Attack(slick,soho,"Kick",.67f,2.05f,.28f));
            if(Input.GetKeyDown(KeyCode.O)) soho.Play("Block",.5f);
        }
        // Character spacing is solved in the plane; feet remain on the mat (y=0).
        Vector3 separation=slick.transform.position-soho.transform.position;
        separation.y=0;
        if(separation.sqrMagnitude < .60f*.60f)
        {
            Vector3 n=separation.sqrMagnitude>.0001f?separation.normalized:Vector3.right;
            Vector3 center=(soho.transform.position+slick.transform.position)*.5f;
            soho.SetPosition(center-n*.31f); slick.SetPosition(center+n*.31f);
        }
        Vector3 midpoint=(soho.transform.position+slick.transform.position)*.5f;
        midpoint.y=1.18f;
        Vector3 target=midpoint+new Vector3(0f,1.52f,-6.9f);
        if(shake>.001f) { target+=UnityEngine.Random.insideUnitSphere*shake; shake=Mathf.MoveTowards(shake,0f,Time.deltaTime*.45f); }
        fightCamera.transform.position=Vector3.Lerp(fightCamera.transform.position,target,Mathf.Clamp01(Time.deltaTime*4.5f));
        fightCamera.transform.LookAt(midpoint + Vector3.up*.13f);
    }
    private void PlayDemo(int number)
    {
        if(sequence!=null) StopCoroutine(sequence);
        freePlay=false; paused=false; Time.timeScale=1f;
        soho.SetPosition(new Vector3(-1.04f,0,0)); slick.SetPosition(new Vector3(1.04f,0,0));
        soho.ResetPose(); slick.ResetPose();
        sceneName=number==1?"CLIP 1 / FIGHT STANCE & BOXING":"CLIP 2 / COMBO, KNOCKDOWN & KICKS";
        status=number==1?"Soho circles, approaches and trades punches":"Jab - Jab - Hook - Uppercut - Knockdown - Slick counter-kicks";
        sequence=StartCoroutine(number==1?ClipOne():ClipTwo());
    }
    private IEnumerator ClipOne()
    {
        yield return new WaitForSeconds(.65f);
        yield return Circle(.95f,1f);
        yield return Attack(soho,slick,"Jab",.43f,1.7f,.11f);
        yield return new WaitForSeconds(.22f);
        yield return Attack(soho,slick,"Cross",.50f,1.7f,.18f);
        yield return Circle(1.1f,-1f);
        yield return Attack(slick,soho,"Jab",.43f,1.7f,.12f);
        yield return Attack(soho,slick,"Hook",.59f,1.7f,.23f);
        status="Clip 1 finished. Press 1 to repeat, 2 for combo, P to play.";
    }
    private IEnumerator ClipTwo()
    {
        yield return new WaitForSeconds(.6f);
        yield return Attack(soho,slick,"Jab",.36f,1.65f,.08f);
        yield return Attack(soho,slick,"Jab",.35f,1.65f,.08f);
        yield return Attack(soho,slick,"Hook",.58f,1.75f,.18f);
        yield return Attack(soho,slick,"Uppercut",.77f,1.75f,.15f);
        slick.Play("Knockdown",1.65f);
        yield return new WaitForSeconds(1.85f);
        slick.Play("GetUp",1.3f);
        yield return new WaitForSeconds(1.45f);
        yield return Attack(slick,soho,"Kick",.65f,2.05f,.18f);
        yield return Attack(slick,soho,"Kick",.65f,2.05f,.22f);
        status="Clip 2 finished. Press 1 or 2 to replay. P for free play.";
    }
    private IEnumerator Circle(float duration,float direction)
    {
        float t=0f;
        while(t<duration)
        {
            t+=Time.deltaTime;
            Vector3 delta=soho.transform.position-slick.transform.position; delta.y=0;
            if(delta.sqrMagnitude>.01f)
            {
                Vector3 tangent=Vector3.Cross(Vector3.up,delta.normalized)*direction;
                soho.Move(tangent,1.0f*Time.deltaTime);
                slick.Move(-tangent,.43f*Time.deltaTime);
            }
            yield return null;
        }
    }
    private IEnumerator Attack(RHFighter attacker,RHFighter victim,string move,float duration,float range,float push)
    {
        if(attacker.IsAttacking) yield break;
        float elapsed=0f;
        while(Vector3.Distance(attacker.transform.position,victim.transform.position)>range*.78f && elapsed<1.15f)
        {
            elapsed+=Time.deltaTime;
            attacker.Move(victim.transform.position-attacker.transform.position,2.4f*Time.deltaTime);
            yield return null;
        }
        attacker.Play(move,duration);
        status=attacker.displayName+" / "+move.ToUpperInvariant();
        float start=Time.time;
        bool hit=false;
        while(Time.time-start<duration)
        {
            if(!hit && Time.time-start>duration*.44f)
            {
                hit=true;
                Vector3 delta=victim.transform.position-attacker.transform.position; delta.y=0;
                bool isFacing=delta.sqrMagnitude>.01f && Vector3.Dot(attacker.transform.forward,delta.normalized)>.20f;
                if(delta.magnitude<=range && isFacing)
                {
                    victim.Play(move=="Uppercut"?"HeavyHit":"Hit",.38f);
                    victim.Move(delta,.05f+push);
                    shake=.055f;
                }
            }
            yield return null;
        }
    }
    private static void CreateArena()
    {
        GameObject arena=new GameObject("RoughHouse Studio Fight Arena");
        Make(PrimitiveType.Cube,"Solid concrete floor",arena.transform,new Vector3(0,-.21f,0),new Vector3(16,.42f,11),new Color(.11f,.12f,.15f));
        Make(PrimitiveType.Cube,"Fight mat",arena.transform,new Vector3(0,-.02f,0),new Vector3(8,.045f,6),new Color(.22f,.23f,.27f));
        for(int i=-3;i<=3;i++) Make(PrimitiveType.Cube,"Mat stripes",arena.transform,new Vector3(i,.008f,0),new Vector3(.014f,.01f,5.9f),new Color(.32f,.33f,.36f));
        for(int i=-2;i<=2;i++) Make(PrimitiveType.Cube,"Mat stripes",arena.transform,new Vector3(0,.008f,i),new Vector3(7.9f,.01f,.014f),new Color(.32f,.33f,.36f));
        Make(PrimitiveType.Cube,"Back wall",arena.transform,new Vector3(0,3.3f,5.5f),new Vector3(16,6.6f,.2f),new Color(.065f,.075f,.11f));
        for(int i=-3;i<=3;i++)
        {
            Make(PrimitiveType.Cube,"Light strip",arena.transform,new Vector3(i*1.9f,3.3f,5.34f),new Vector3(.035f,3.9f,.04f),new Color(.2f,.43f,.53f));
        }
        Light main=new GameObject("Key Light").AddComponent<Light>(); main.type=LightType.Directional; main.intensity=1.65f; main.transform.rotation=Quaternion.Euler(48,-25,0);
        RenderSettings.ambientLight=new Color(.56f,.58f,.64f);
    }
    internal static GameObject Make(PrimitiveType type,string name,Transform parent,Vector3 localPosition,Vector3 localScale,Color color)
    {
        GameObject obj=GameObject.CreatePrimitive(type);
        obj.name=name; obj.transform.SetParent(parent,false);
        obj.transform.localPosition=localPosition; obj.transform.localScale=localScale;
        Shader shader=Resources.Load<Shader>("RoughHouseFlatColor");
        if(shader==null) shader=Shader.Find("Unlit/Color");
        if(shader!=null) { var material=new Material(shader); material.color=color; var renderer=obj.GetComponent<Renderer>(); renderer.sharedMaterial=material; }
        return obj;
    }
    private void OnGUI()
    {
        if(titleStyle==null)
        {
            titleStyle=new GUIStyle(GUI.skin.label); titleStyle.fontSize=22; titleStyle.fontStyle=FontStyle.Bold; titleStyle.normal.textColor=Color.white;
            textStyle=new GUIStyle(GUI.skin.label); textStyle.fontSize=14; textStyle.normal.textColor=new Color(.77f,.92f,.95f);
        }
        GUI.Box(new Rect(15,15,Mathf.Min(730,Screen.width-30),128),"");
        GUI.Label(new Rect(29,23,690,30),"ROUGH HOUSE  |  "+sceneName,titleStyle);
        GUI.Label(new Rect(29,59,690,26),status,textStyle);
        GUI.Label(new Rect(29,100,690,28),"[1] Boxing clip    [2] Combo clip    [P] Free play    [Space] Pause",textStyle);
    }
}

// Self-contained human-proportioned male mesh actor. Not mocap and not photorealistic.
public sealed class RHFighter : MonoBehaviour
{
    public RHFighter enemy;
    public string displayName;
    public bool IsAttacking { get { return Time.time < actionEnd && (action=="Jab"||action=="Cross"||action=="Hook"||action=="Uppercut"||action=="Kick"); } }
    private Transform body,head,leftArm,rightArm,leftForearm,rightForearm,leftLeg,rightLeg,leftShin,rightShin;
    private float actionEnd,actionStart,actionDuration,seed;
    private string action="Idle";
    private float speedMotion;
    private bool boxStyle;
    private Vector3 restLocalBody;
    public static RHFighter Create(string fighterName,bool isBoxer,Vector3 spawn)
    {
        GameObject root=new GameObject(fighterName+" / male fighter");
        root.transform.position=spawn;
        // Runtime colliders keep the fighter's physical volume explicit.
        CapsuleCollider capsule=root.AddComponent<CapsuleCollider>(); capsule.center=new Vector3(0,1.03f,0); capsule.height=2.05f; capsule.radius=.33f;
        RHFighter f=root.AddComponent<RHFighter>(); f.displayName=fighterName; f.boxStyle=isBoxer;
        f.seed=isBoxer?0f:1.2f;
        f.Build(isBoxer);
        return f;
    }
    private static Transform Pivot(Transform parent,string name,Vector3 local)
    {
        GameObject p=new GameObject(name); p.transform.SetParent(parent,false); p.transform.localPosition=local;
        return p.transform;
    }
    private static void Shape(Transform parent,string name,PrimitiveType shape,Vector3 local,Vector3 size,Color color)
    {
        GameObject part=RoughHousePrototype.Make(shape,name,parent,local,size,color);
        Collider c=part.GetComponent<Collider>(); if(c!=null) Destroy(c);
    }
    private void Build(bool boxer)
    {
        Color skin=boxer?new Color(.64f,.42f,.28f):new Color(.31f,.20f,.15f);
        Color top=boxer?new Color(.13f,.15f,.18f):new Color(.50f,.14f,.18f);
        Color shorts=boxer?new Color(.18f,.24f,.28f):new Color(.17f,.17f,.24f);
        Color glove=boxer?new Color(.80f,.20f,.13f):new Color(.10f,.58f,.68f);
        Color shoe=new Color(.075f,.085f,.09f); Color hair=boxer?new Color(.12f,.09f,.07f):new Color(.09f,.055f,.035f);
        body=Pivot(transform,"Hips and chest",new Vector3(0,1.14f,0));
        Shape(body,"Muscular torso",PrimitiveType.Capsule,new Vector3(0,.31f,0),new Vector3(.72f,.69f,.37f),top);
        Shape(body,"Shoulders",PrimitiveType.Capsule,new Vector3(0,.52f,0),new Vector3(.94f,.23f,.41f),top);
        Shape(body,"Waist / trunks",PrimitiveType.Cube,new Vector3(0,-.11f,0),new Vector3(.57f,.35f,.40f),shorts);
        Shape(body,"Neck",PrimitiveType.Cylinder,new Vector3(0,.72f,0),new Vector3(.18f,.20f,.18f),skin);
        head=Pivot(body,"Head",new Vector3(0,.90f,0));
        Shape(head,"Male head",PrimitiveType.Sphere,new Vector3(0,.10f,0),new Vector3(.37f,.47f,.33f),skin);
        Shape(head,"Hair",PrimitiveType.Sphere,new Vector3(0,.31f,-.012f),new Vector3(.37f,.14f,.33f),hair);
        Shape(head,"Nose",PrimitiveType.Cube,new Vector3(0,.105f,.176f),new Vector3(.08f,.12f,.11f),skin);
        for(int side=-1;side<=1;side+=2)
        {
            Shape(head,"Eye",PrimitiveType.Sphere,new Vector3(side*.09f,.17f,.159f),new Vector3(.06f,.035f,.035f),Color.white);
            Shape(head,"Pupil",PrimitiveType.Sphere,new Vector3(side*.09f,.17f,.188f),new Vector3(.026f,.027f,.018f),new Color(.06f,.07f,.08f));
            Shape(head,"Brow",PrimitiveType.Cube,new Vector3(side*.09f,.225f,.168f),new Vector3(.14f,.025f,.03f),hair);
        }
        leftArm=Pivot(body,"Left shoulder",new Vector3(-.42f,.55f,0));
        rightArm=Pivot(body,"Right shoulder",new Vector3(.42f,.55f,0));
        BuildArm(leftArm,"Left",skin,glove); BuildArm(rightArm,"Right",skin,glove);
        leftForearm=leftArm.Find("Left elbow"); rightForearm=rightArm.Find("Right elbow");
        leftLeg=Pivot(transform,"Left hip",new Vector3(-.19f,1.02f,0));
        rightLeg=Pivot(transform,"Right hip",new Vector3(.19f,1.02f,0));
        BuildLeg(leftLeg,"Left",skin,shorts,shoe); BuildLeg(rightLeg,"Right",skin,shorts,shoe);
        leftShin=leftLeg.Find("Left knee");rightShin=rightLeg.Find("Right knee");
        restLocalBody=body.localPosition;
    }
    private static void BuildArm(Transform shoulder,string name,Color skin,Color glove)
    {
        float sign=name=="Left"?-1f:1f;
        Shape(shoulder,name+" upper arm",PrimitiveType.Capsule,new Vector3(sign*.055f,-.24f,0),new Vector3(.23f,.46f,.24f),skin);
        Transform elbow=Pivot(shoulder,name+" elbow",new Vector3(sign*.08f,-.47f,0));
        Shape(elbow,name+" forearm",PrimitiveType.Capsule,new Vector3(0,-.19f,0),new Vector3(.21f,.39f,.22f),skin);
        Shape(elbow,name+" Boxing glove",PrimitiveType.Sphere,new Vector3(0,-.39f,.035f),new Vector3(.29f,.27f,.32f),glove);
    }
    private static void BuildLeg(Transform hip,string name,Color skin,Color trunks,Color shoe)
    {
        Shape(hip,name+" shorts",PrimitiveType.Capsule,new Vector3(0,-.15f,0),new Vector3(.32f,.34f,.34f),trunks);
        Shape(hip,name+" thigh",PrimitiveType.Capsule,new Vector3(0,-.36f,0),new Vector3(.27f,.50f,.29f),skin);
        Transform knee=Pivot(hip,name+" knee",new Vector3(0,-.62f,0));
        Shape(knee,name+" shin",PrimitiveType.Capsule,new Vector3(0,-.30f,0),new Vector3(.22f,.56f,.23f),skin);
        Shape(knee,name+" foot",PrimitiveType.Cube,new Vector3(0,-.54f,.10f),new Vector3(.29f,.17f,.49f),shoe);
    }
    public void SetPosition(Vector3 p) { p.y=0; p.x=Mathf.Clamp(p.x,-3.35f,3.35f);p.z=Mathf.Clamp(p.z,-2.55f,2.55f);transform.position=p; }
    public void Move(Vector3 direction,float distance)
    {
        direction.y=0;
        if(direction.sqrMagnitude<.0005f)return;
        direction.Normalize(); SetPosition(transform.position+direction*distance);
        speedMotion=1f;
    }
    public void Play(string name,float seconds)
    {
        action=name; actionStart=Time.time; actionDuration=Mathf.Max(.08f,seconds);actionEnd=actionStart+actionDuration;
    }
    public void ResetPose() { action="Idle"; actionEnd=0; speedMotion=0; body.localPosition=restLocalBody; }
    private void LateUpdate()
    {
        if(enemy!=null)
        {
            Vector3 delta=enemy.transform.position-transform.position; delta.y=0;
            if(delta.sqrMagnitude>.01f) transform.rotation=Quaternion.Slerp(transform.rotation,Quaternion.LookRotation(delta.normalized),Mathf.Clamp01(Time.deltaTime*13f));
        }
        float t=Time.time;
        float n=Mathf.Sin(t*4.2f+seed);
        // A guarded male boxing stance, foot weight transfer and breathing.
        body.localPosition=restLocalBody+new Vector3(0,.017f*n,0);
        body.localRotation=Quaternion.Euler(-3f+1.8f*n,0,Mathf.Sin(t*2.1f+seed)*2f);
        leftArm.localRotation=Quaternion.Euler(-48f,0,-22f);
        rightArm.localRotation=Quaternion.Euler(-58f,0,22f);
        leftForearm.localRotation=Quaternion.Euler(-78f,0,0);
        rightForearm.localRotation=Quaternion.Euler(-80f,0,0);
        leftLeg.localRotation=Quaternion.Euler(4f+Mathf.Sin(t*3f)*2f,0,-4f);
        rightLeg.localRotation=Quaternion.Euler(-4f-Mathf.Sin(t*3f)*2f,0,5f);
        leftShin.localRotation=Quaternion.Euler(4f,0,0);
        rightShin.localRotation=Quaternion.Euler(5f,0,0);
        head.localRotation=Quaternion.Euler(-4f,0,0);
        if(Time.time<=actionEnd)
        {
            float u=Mathf.Clamp01((t-actionStart)/actionDuration);
            float strike=Mathf.Sin(Mathf.PI*Mathf.Pow(u,.75f));
            if(action=="Jab") {leftArm.localRotation=Quaternion.Euler(-48f-93f*strike,0,-22f+16f*strike);leftForearm.localRotation=Quaternion.Euler(-78f+74f*strike,0,0);body.localRotation*=Quaternion.Euler(0,-8f*strike,0);}
            else if(action=="Cross") {rightArm.localRotation=Quaternion.Euler(-58f-92f*strike,0,22f-15f*strike);rightForearm.localRotation=Quaternion.Euler(-80f+75f*strike,0,0);body.localRotation*=Quaternion.Euler(0,18f*strike,0);}
            else if(action=="Hook") {rightArm.localRotation=Quaternion.Euler(-100f*strike-58f,60f*strike,22f-44f*strike);rightForearm.localRotation=Quaternion.Euler(-80f+50f*strike,0,0);body.localRotation*=Quaternion.Euler(0,27f*strike,0);}
            else if(action=="Uppercut") {rightArm.localRotation=Quaternion.Euler(-58f+44f*strike,0,22f);rightForearm.localRotation=Quaternion.Euler(-80f-23f*strike,0,0);body.localRotation*=Quaternion.Euler(-14f*strike,18f*strike,0);}
            else if(action=="Kick") {rightLeg.localRotation=Quaternion.Euler(-4f-95f*strike,0,5f);rightShin.localRotation=Quaternion.Euler(5f+70f*strike,0,0);body.localRotation*=Quaternion.Euler(13f*strike,-12f*strike,0);}
            else if(action=="Hit"||action=="HeavyHit") {body.localRotation*=Quaternion.Euler(17f*strike,0,10f*strike);head.localRotation=Quaternion.Euler(23f*strike,0,0);}
            else if(action=="Block") {leftArm.localRotation=Quaternion.Euler(-115f,0,-28f);rightArm.localRotation=Quaternion.Euler(-115f,0,28f);}
            else if(action=="Knockdown") { float fall=Mathf.SmoothStep(0,1,Mathf.Clamp01(u*3f));body.localRotation=Quaternion.Euler(-80f*fall,0,0);body.localPosition=restLocalBody+new Vector3(0,-.72f*fall,-.38f*fall);leftLeg.localRotation=Quaternion.Euler(42f*fall,0,0);rightLeg.localRotation=Quaternion.Euler(-45f*fall,0,0); }
            else if(action=="GetUp") {float rise=Mathf.SmoothStep(0,1,u);body.localRotation=Quaternion.Euler(-80f*(1-rise),0,0);body.localPosition=restLocalBody+new Vector3(0,-.72f*(1-rise),-.38f*(1-rise));}
        }
        else if(action=="Knockdown") {body.localRotation=Quaternion.Euler(-80f,0,0);body.localPosition=restLocalBody+new Vector3(0,-.72f,-.38f);}
        else { action="Idle"; }
        speedMotion=Mathf.MoveTowards(speedMotion,0f,Time.deltaTime*3f);
    }
}
