using System;
using System.Collections;
using UnityEngine;

// RoughHouse: entirely procedural, dependency-free combat visualization prototype.
// All graphics, rig motions, stage elements and cues are generated at runtime.
public class RoughHousePrototype : MonoBehaviour
{
    public enum Move { None, JabLeft, JabRight, Hook, Uppercut, QuickKick, SpinKick }
    public enum Reaction { None, Hit, Crumple, Knockdown, GettingUp, Airborne }

    private Fighter soho, slick;
    private CombatCamera followCamera;
    private Coroutine sequence;
    private string currentClip = "CLIP 1 / LOCK-ON MOVEMENT";
    private string actionCaption = "Setting up the fight...";
    private bool paused;
    private bool manual;
    private float clock;
    private GUIStyle titleStyle, smallStyle, bigStyle;

    private void Start()
    {
        Application.targetFrameRate = 60;
        CreateArena();
        soho = Fighter.Build("SOHO", new Vector3(-2.15f, 0, 0),
            new Color(0.17f, 0.72f, 0.78f), new Color(0.11f, 0.20f, 0.27f), false);
        slick = Fighter.Build("SLICK", new Vector3(2.15f, 0, 0),
            new Color(0.68f, 0.24f, 0.88f), new Color(0.18f, 0.10f, 0.28f), true);
        soho.FacingTarget = slick.transform;
        slick.FacingTarget = soho.transform;
        CreateCamera();
        StartClip(1);
    }

    private void Update()
    {
        clock += Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.Alpha1)) StartClip(1);
        if (Input.GetKeyDown(KeyCode.Alpha2)) StartClip(2);
        if (Input.GetKeyDown(KeyCode.P))
        {
            manual = !manual;
            if (sequence != null) StopCoroutine(sequence);
            sequence = null;
            currentClip = manual ? "FREE PLAY / PLAYER CONTROL" : "CLIP 1 / LOCK-ON MOVEMENT";
            actionCaption = manual ? "WASD move · J/K/L/I punches · U kick · 1/2 demo" : "Auto demo";
            if (!manual) StartClip(1);
        }
        if (Input.GetKeyDown(KeyCode.Space)) paused = !paused;
        Time.timeScale = paused ? 0f : 1f;
        if (manual && !paused)
        {
            Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
            soho.Move(input.normalized, 3.0f * Time.deltaTime);
            if (Input.GetKeyDown(KeyCode.J)) StartCoroutine(Perform(soho, slick, Move.JabLeft, .48f, Reaction.Hit));
            if (Input.GetKeyDown(KeyCode.K)) StartCoroutine(Perform(soho, slick, Move.JabRight, .48f, Reaction.Hit));
            if (Input.GetKeyDown(KeyCode.L)) StartCoroutine(Perform(soho, slick, Move.Hook, .65f, Reaction.Crumple));
            if (Input.GetKeyDown(KeyCode.I)) StartCoroutine(Perform(soho, slick, Move.Uppercut, .8f, Reaction.Airborne));
            if (Input.GetKeyDown(KeyCode.U)) StartCoroutine(Perform(slick, soho, Move.QuickKick, .7f, Reaction.Hit));
        }
    }

    private void OnDestroy() { Time.timeScale = 1f; }

    private void StartClip(int clip)
    {
        if (sequence != null) StopCoroutine(sequence);
        manual = false;
        paused = false;
        Time.timeScale = 1f;
        soho.ResetFighter(new Vector3(-2.15f, 0f, 0f));
        slick.ResetFighter(new Vector3(2.15f, 0f, 0f));
        currentClip = clip == 1 ? "CLIP 1 / GAMEPLAY IS ALIVE" : "CLIP 2 / COMBAT HIGHLIGHTS";
        actionCaption = clip == 1 ? "Fight stance · lock-on camera · circular footwork" : "Jab · Jab · Hook · Uppercut · Crumple · OTG · Kicks";
        sequence = StartCoroutine(clip == 1 ? ClipOne() : ClipTwo());
    }

    private IEnumerator ClipOne()
    {
        yield return new WaitForSeconds(2.5f);
        actionCaption = "Soho approaches Slick — always facing his opponent";
        yield return Circle(soho, slick, 8.0f, -1f, .95f);
        actionCaption = "Soho's opening jab";
        yield return Perform(soho, slick, Move.JabLeft, .56f, Reaction.Hit);
        yield return new WaitForSeconds(.75f);
        yield return Perform(soho, slick, Move.JabRight, .58f, Reaction.Hit);
        actionCaption = "Both fighters reset and circle";
        yield return Circle(soho, slick, 6.2f, 1f, .68f);
        actionCaption = "Soho throws a hook";
        yield return Perform(soho, slick, Move.Hook, .7f, Reaction.Hit);
        yield return new WaitForSeconds(1.6f);
        yield return Circle(soho, slick, 5.4f, -1f, .65f);
        actionCaption = "End of Clip 1 — press 2 for the combat showcase";
    }

    private IEnumerator ClipTwo()
    {
        yield return new WaitForSeconds(1.8f);
        actionCaption = "JAB → JAB → HOOK → UPPERCUT";
        yield return Perform(soho, slick, Move.JabLeft, .49f, Reaction.Hit);
        yield return Perform(soho, slick, Move.JabRight, .47f, Reaction.Hit);
        yield return Perform(soho, slick, Move.Hook, .65f, Reaction.Hit);
        yield return Perform(soho, slick, Move.Uppercut, .86f, Reaction.Airborne);
        yield return new WaitForSeconds(1.5f);
        slick.ResetStance();
        actionCaption = "CRUMPLE — Slick slowly buckles after a heavy hit";
        yield return Perform(soho, slick, Move.Hook, .75f, Reaction.Crumple);
        yield return new WaitForSeconds(1.7f);
        actionCaption = "OTG — follow-up while Slick is on the ground";
        yield return Perform(soho, slick, Move.JabRight, .68f, Reaction.Knockdown);
        yield return new WaitForSeconds(1.0f);
        slick.GroundHit();
        yield return new WaitForSeconds(1.4f);
        actionCaption = "GET UP — Slick recovers to fighting stance";
        slick.GetUp();
        yield return new WaitForSeconds(2.1f);
        actionCaption = "SLICK COUNTERS — fast kicks and tricking style";
        yield return Perform(slick, soho, Move.QuickKick, .6f, Reaction.Hit);
        yield return new WaitForSeconds(.35f);
        yield return Perform(slick, soho, Move.SpinKick, .9f, Reaction.Hit);
        yield return new WaitForSeconds(1f);
        actionCaption = "Soho and Slick reset for the next round";
        yield return Circle(soho, slick, 6f, 1f, .6f);
        actionCaption = "End of Clip 2 — press 1 to replay Clip 1";
    }

    private IEnumerator Circle(Fighter actor, Fighter target, float duration, float direction, float speed)
    {
        float elapsed = 0;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            Vector3 radial = actor.transform.position - target.transform.position;
            radial.y = 0;
            if (radial.sqrMagnitude < .02f) radial = Vector3.left;
            Vector3 tangent = Vector3.Cross(Vector3.up, radial.normalized) * direction;
            actor.Move(tangent, speed * Time.deltaTime);
            yield return null;
        }
    }

    private IEnumerator Perform(Fighter attacker, Fighter defender, Move move, float duration, Reaction reaction)
    {
        if (attacker.IsAttacking) yield break;
        attacker.BeginAttack(move, duration);
        float elapsed = 0;
        bool impacted = false;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            if (!impacted && elapsed >= duration * .5f)
            {
                impacted = true;
                if (reaction == Reaction.Airborne) defender.Launch();
                else if (reaction == Reaction.Crumple) defender.Crumple();
                else if (reaction == Reaction.Knockdown) defender.Knockdown();
                else defender.Hit();
                followCamera.Shake(.14f);
            }
            yield return null;
        }
    }

    private void CreateCamera()
    {
        Camera cam = Camera.main;
        if (cam == null)
        {
            GameObject obj = new GameObject("Cinematic Fight Camera");
            cam = obj.AddComponent<Camera>();
            obj.tag = "MainCamera";
        }
        cam.backgroundColor = new Color(.11f, .13f, .19f);
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.fieldOfView = 46;
        followCamera = cam.gameObject.AddComponent<CombatCamera>();
        followCamera.Setup(soho.transform, slick.transform);
    }

    private static void CreateArena()
    {
        GameObject env = new GameObject("ARENA / generated geometry");
        MakeBox(env.transform, "Arena Floor", new Vector3(0, -.22f, 0), new Vector3(18, .4f, 15), ColorAt(.12f, .15f, .20f));
        MakeBox(env.transform, "Center Mat", new Vector3(0, -.008f, 0), new Vector3(12, .04f, 10), ColorAt(.19f, .23f, .30f));
        for (int n = -5; n <= 5; n++)
            MakeBox(env.transform, "Mat stripe", new Vector3(n, .018f, 0), new Vector3(.015f, .014f, 9.7f), ColorAt(.22f, .31f, .38f));
        for (int n = -4; n <= 4; n++)
            MakeBox(env.transform, "Mat stripe", new Vector3(0, .018f, n), new Vector3(11.7f, .014f, .015f), ColorAt(.22f, .31f, .38f));
        const int segments = 56;
        for (int i = 0; i < segments; i++)
        {
            float a = i * Mathf.PI * 2 / segments;
            float b = (i + 1) * Mathf.PI * 2 / segments;
            Vector3 p1 = new Vector3(Mathf.Cos(a) * 4.4f, .037f, Mathf.Sin(a) * 3.75f);
            Vector3 p2 = new Vector3(Mathf.Cos(b) * 4.4f, .037f, Mathf.Sin(b) * 3.75f);
            GameObject line = MakeBox(env.transform, "Boundary stroke", (p1+p2)/2, new Vector3(.085f, .018f, Vector3.Distance(p1,p2)+.05f), ColorAt(.95f,.67f,.29f));
            line.transform.rotation = Quaternion.LookRotation(p2-p1);
        }
        MakeBox(env.transform, "Backdrop", new Vector3(0,3.0f,6.8f), new Vector3(18,6, .25f), ColorAt(.09f,.11f,.18f));
        MakeBox(env.transform, "Left wall", new Vector3(-8.8f,2.3f,0), new Vector3(.25f,4.6f,15), ColorAt(.11f,.13f,.19f));
        MakeBox(env.transform, "Right wall", new Vector3(8.8f,2.3f,0), new Vector3(.25f,4.6f,15), ColorAt(.11f,.13f,.19f));
        for (int i=0; i<6; i++)
        {
            MakeBox(env.transform, "Arena light strip", new Vector3(-7.2f + i*2.88f,4.65f,6.55f), new Vector3(1.55f,.12f,.12f), ColorAt(.25f,.8f,.88f));
        }
        Light light = new GameObject("Arena key light").AddComponent<Light>();
        light.type = LightType.Directional;
        light.transform.rotation = Quaternion.Euler(42,-30,0);
        light.intensity = 1.7f;
        RenderSettings.ambientLight = new Color(.5f,.53f,.63f);
        foreach (var go in GameObject.FindGameObjectsWithTag("Respawn")) go.SetActive(false);
    }

    private static Color ColorAt(float r,float g,float b) { return new Color(r,g,b); }
    public static GameObject MakeBox(Transform parent, string name, Vector3 pos, Vector3 scale, Color color)
    {
        GameObject g = GameObject.CreatePrimitive(PrimitiveType.Cube);
        g.name = name;
        g.transform.SetParent(parent, false);
        g.transform.localPosition = pos;
        g.transform.localScale = scale;
        var mr = g.GetComponent<MeshRenderer>();
        mr.material = new Material(Shader.Find("Standard"));
        mr.material.color = color;
        // Collider components are unnecessary for the visual-only prototype.
        // Avoid a hard PhysicsModule dependency in Unity Cloud builds.
        return g;
    }

    private void OnGUI()
    {
        if (titleStyle == null)
        {
            titleStyle = new GUIStyle(GUI.skin.label) { fontSize = 26, fontStyle = FontStyle.Bold };
            titleStyle.normal.textColor = new Color(.96f,.95f,.91f);
            bigStyle = new GUIStyle(GUI.skin.label) { fontSize = 17, fontStyle = FontStyle.Bold };
            bigStyle.normal.textColor = new Color(.36f,.89f,.97f);
            smallStyle = new GUIStyle(GUI.skin.label) { fontSize = 14 };
            smallStyle.normal.textColor = Color.white;
        }
        GUI.Box(new Rect(20,20,Mathf.Min(640,Screen.width-40),148), "");
        GUI.Label(new Rect(38,27,650,36), "ROUGH HOUSE  |  COMBAT PROTOTYPE", titleStyle);
        GUI.Label(new Rect(38,65,600,28), currentClip, bigStyle);
        GUI.Label(new Rect(38,93,600,26), actionCaption, smallStyle);
        GUI.Label(new Rect(38,123,610,32), "[1] Clip 1  [2] Clip 2  [P] Free play  [Space] Pause", smallStyle);
        if (manual)
            GUI.Label(new Rect(30,Screen.height-50,760,30), "FREE PLAY: WASD move, J/K/L/I attack, U Slick kick", smallStyle);
        if (paused) GUI.Label(new Rect(Screen.width / 2f-55, Screen.height/2f-30, 180,60), "PAUSED", titleStyle);
        GUI.Label(new Rect(Screen.width-200,20,180,35), "SOHO  VS  SLICK", bigStyle);
    }
}

public class CombatCamera : MonoBehaviour
{
    private Transform a, b;
    private Vector3 velocity;
    private float shake;
    public void Setup(Transform first, Transform second) { a=first; b=second; }
    public void Shake(float amount) { shake = amount; }
    private void LateUpdate()
    {
        if (a==null || b==null) return;
        Vector3 middle = (a.position+b.position)*.5f;
        float dist = Vector3.Distance(a.position,b.position);
        float zoom = Mathf.Clamp(dist*1.2f+7f,9f,13.8f);
        Vector3 desired = middle + new Vector3(0,3.75f, -zoom);
        transform.position = Vector3.SmoothDamp(transform.position, desired, ref velocity, .22f);
        if (shake>0)
        {
            transform.position += UnityEngine.Random.insideUnitSphere*shake;
            shake=Mathf.MoveTowards(shake,0,Time.deltaTime*.85f);
        }
        transform.LookAt(middle + Vector3.up*1.2f);
    }
}

public class Fighter : MonoBehaviour
{
    public Transform FacingTarget;
    public bool IsAttacking { get { return attackTime > 0f; } }
    private Transform modelRoot, torso, leftArm, rightArm, leftElbow, rightElbow, leftLeg, rightLeg, head;
    private bool fancy;
    private float timer, attackTime, attackDuration, reactTime, reactDuration;
    private RoughHousePrototype.Move currentAttack;
    private RoughHousePrototype.Reaction currentReaction;
    private Vector3 startPosition;

    public static Fighter Build(string displayName, Vector3 position, Color jacket, Color pants, bool slick)
    {
        GameObject root = new GameObject(displayName + " / procedural fighter");
        root.transform.position = position;
        Fighter f = root.AddComponent<Fighter>();
        f.fancy = slick;
        f.BuildRig(displayName, jacket, pants);
        f.startPosition = position;
        return f;
    }

    private void BuildRig(string fighterName, Color jacket, Color pants)
    {
        modelRoot = NewJoint(transform,"Model Pivot",Vector3.zero);
        Transform pelvis = NewJoint(modelRoot,"Pelvis",new Vector3(0,1.08f,0));
        Ball(pelvis,"Waist",new Vector3(0,-.02f,0),new Vector3(.37f,.24f,.22f),pants);
        torso = NewJoint(pelvis,"Torso Pivot",new Vector3(0,.1f,0));
        Ball(torso,"Jacket torso",new Vector3(0,.32f,0),new Vector3(.46f,.65f,.29f),jacket);
        Ball(torso,"Collar",new Vector3(0,.66f,0),new Vector3(.23f,.15f,.23f),new Color(.06f,.09f,.13f));
        head = NewJoint(torso,"Head Pivot",new Vector3(0,.79f,0));
        Ball(head,"Face",new Vector3(0,.18f,0),new Vector3(.24f,.31f,.24f),fancy? new Color(.69f,.40f,.28f) : new Color(.56f,.32f,.23f));
        Ball(head,"Hair",new Vector3(0,.39f,-.025f),new Vector3(.26f,.105f,.25f),fancy ? new Color(.16f,.09f,.07f) : new Color(.07f,.07f,.09f));
        // Faces are oriented toward +Z.
        Ball(head,"Left eye",new Vector3(-.095f,.20f,.215f),new Vector3(.028f,.025f,.012f),Color.black);
        Ball(head,"Right eye",new Vector3(.095f,.20f,.215f),new Vector3(.028f,.025f,.012f),Color.black);
        Ball(head,"Nose",new Vector3(0,.125f,.245f),new Vector3(.045f,.05f,.05f),new Color(.45f,.27f,.21f));
        if (fancy) // Flashier silhouette, swept hair and shoulder accents.
        {
            Box(torso,"Right jacket stripe",new Vector3(.29f,.29f,.10f),new Vector3(.06f,.47f,.04f),new Color(.92f,.73f,.30f));
            Box(torso,"Left jacket stripe",new Vector3(-.29f,.29f,.10f),new Vector3(.06f,.47f,.04f),new Color(.92f,.73f,.30f));
            Ball(head,"Top hair",new Vector3(.10f,.46f,-.05f),new Vector3(.18f,.10f,.20f),new Color(.09f,.06f,.06f));
        }
        leftArm = NewJoint(torso,"Left Shoulder",new Vector3(-.47f,.53f,0));
        rightArm = NewJoint(torso,"Right Shoulder",new Vector3(.47f,.53f,0));
        BuildArm(leftArm, jacket, fancy, "Left");
        BuildArm(rightArm,jacket,fancy,"Right");
        leftElbow = leftArm.Find("Left Elbow");
        rightElbow = rightArm.Find("Right Elbow");
        leftLeg = NewJoint(pelvis,"Left Hip",new Vector3(-.21f,-.10f,0));
        rightLeg = NewJoint(pelvis,"Right Hip",new Vector3(.21f,-.10f,0));
        BuildLeg(leftLeg,pants,"Left",fancy);
        BuildLeg(rightLeg,pants,"Right",fancy);
        // Name badge is an independent 3D text object for simple identification.
        GameObject badge=new GameObject("Label");
        badge.transform.SetParent(modelRoot,false);
        badge.transform.localPosition=new Vector3(0,2.34f,0);
        TextMesh text=badge.AddComponent<TextMesh>();
        text.text=fighterName;
        text.anchor=TextAnchor.MiddleCenter;
        text.characterSize=.13f;
        text.fontSize=50;
        text.color=fancy ? new Color(.94f,.69f,1f) : new Color(.55f,.95f,1f);
        badge.transform.localRotation=Quaternion.Euler(0,180,0); // labels face camera from rear camera view
    }
    private static Transform NewJoint(Transform parent, string name, Vector3 localPosition)
    {
        GameObject obj=new GameObject(name);
        obj.transform.SetParent(parent,false);
        obj.transform.localPosition=localPosition;
        return obj.transform;
    }
    private static GameObject Ball(Transform p,string name,Vector3 pos,Vector3 scale,Color color)
    {
        GameObject g=GameObject.CreatePrimitive(PrimitiveType.Sphere);
        g.name=name;
        g.transform.SetParent(p,false);
        g.transform.localPosition=pos;
        g.transform.localScale=scale;
        MeshRenderer renderer=g.GetComponent<MeshRenderer>();
        renderer.material=new Material(Shader.Find("Standard"));
        renderer.material.color=color;
        // Collider components are unnecessary for the visual-only prototype.
        // Avoid a hard PhysicsModule dependency in Unity Cloud builds.
        return g;
    }
    private static void Box(Transform p,string name,Vector3 pos,Vector3 scale,Color color)
    { RoughHousePrototype.MakeBox(p,name,pos,scale,color); }
    private static void BuildArm(Transform shoulder,Color jacket,bool slick,string side)
    {
        Ball(shoulder,side+" upper arm",new Vector3(0,-.23f,.06f),new Vector3(.2f,.46f,.21f),jacket);
        Transform elbow=NewJoint(shoulder,side+" Elbow",new Vector3(0,-.47f,.04f));
        Ball(elbow,side+" forearm",new Vector3(0,-.20f,0),new Vector3(.18f,.37f,.19f),new Color(.26f,.26f,.29f));
        Ball(elbow,side+" boxing glove",new Vector3(0,-.42f,.02f),new Vector3(.25f,.23f,.26f),slick? new Color(.92f,.67f,.22f) : new Color(.94f,.23f,.25f));
    }
    private static void BuildLeg(Transform hip,Color pants,string side,bool slick)
    {
        Ball(hip,side+" thigh",new Vector3(0,-.24f,0),new Vector3(.24f,.48f,.26f),pants);
        Transform knee=NewJoint(hip,side+" Knee",new Vector3(0,-.52f,0));
        Ball(knee,side+" shin",new Vector3(0,-.25f,0),new Vector3(.21f,.49f,.21f),pants);
        Ball(knee,side+" sneaker",new Vector3(0,-.52f,.16f),new Vector3(.24f,.17f,.42f),slick? new Color(.94f,.79f,.28f) : Color.white);
    }
    public void ResetFighter(Vector3 position)
    {
        transform.position=position;
        modelRoot.localPosition=Vector3.zero;
        modelRoot.localRotation=Quaternion.identity;
        currentReaction=RoughHousePrototype.Reaction.None;
        reactTime=attackTime=0;
        ResetStance();
    }
    public void ResetStance()
    {
        currentReaction=RoughHousePrototype.Reaction.None;
        reactTime=0;
    }
    public void BeginAttack(RoughHousePrototype.Move move,float duration)
    {
        currentAttack=move;
        attackTime=duration;
        attackDuration=duration;
    }
    public void Hit() { React(RoughHousePrototype.Reaction.Hit,.55f); }
    public void Crumple() { React(RoughHousePrototype.Reaction.Crumple,3.5f); }
    public void Knockdown() { React(RoughHousePrototype.Reaction.Knockdown,5f); }
    public void Launch() { React(RoughHousePrototype.Reaction.Airborne,2.2f); }
    public void GroundHit() { React(RoughHousePrototype.Reaction.Knockdown,2f); }
    public void GetUp() { React(RoughHousePrototype.Reaction.GettingUp,1.8f); }
    private void React(RoughHousePrototype.Reaction reaction,float duration)
    {
        currentReaction=reaction;
        reactTime=duration;
        reactDuration=duration;
    }
    public void Move(Vector3 deltaDirection, float distance)
    {
        if (currentReaction==RoughHousePrototype.Reaction.Knockdown || currentReaction==RoughHousePrototype.Reaction.Crumple) return;
        Vector3 next=transform.position + deltaDirection*distance;
        next.x=Mathf.Clamp(next.x,-5.2f,5.2f);
        next.z=Mathf.Clamp(next.z,-4.1f,4.1f);
        transform.position=next;
    }
    private void Update()
    {
        timer+=Time.deltaTime;
        if (FacingTarget!=null)
        {
            Vector3 delta=FacingTarget.position-transform.position;
            delta.y=0;
            if (delta.sqrMagnitude>.01f)
                transform.rotation=Quaternion.RotateTowards(transform.rotation,Quaternion.LookRotation(delta),Time.deltaTime*680f);
        }
        if (attackTime>0) attackTime=Mathf.Max(0,attackTime-Time.deltaTime);
        if (reactTime>0) reactTime=Mathf.Max(0,reactTime-Time.deltaTime);
    }
    private void LateUpdate()
    {
        float idle=Mathf.Sin(timer*4.6f);
        float jab = attackDuration>0 ? Mathf.Sin(Mathf.PI*(1f-attackTime/attackDuration)) : 0;
        if (attackTime<=0f) jab=0;
        leftArm.localRotation=Quaternion.Euler(-65+idle*3,-12,-12);
        rightArm.localRotation=Quaternion.Euler(-65-idle*3,12,12);
        leftElbow.localRotation=Quaternion.Euler(-65,0,0);
        rightElbow.localRotation=Quaternion.Euler(-65,0,0);
        leftLeg.localRotation=Quaternion.Euler(idle*3,0,-3);
        rightLeg.localRotation=Quaternion.Euler(-idle*3,0,3);
        torso.localRotation=Quaternion.Euler(idle*1.5f,0,0);
        head.localRotation=Quaternion.Euler(-idle*1.2f,0,0);
        if (attackTime>0)
        {
            switch(currentAttack)
            {
                case RoughHousePrototype.Move.JabLeft:
                    leftArm.localRotation=Quaternion.Euler(-65-95*jab,0,-10);
                    leftElbow.localRotation=Quaternion.Euler(-65+65*jab,0,0);
                    torso.localRotation=Quaternion.Euler(0,-15*jab,0);
                    break;
                case RoughHousePrototype.Move.JabRight:
                    rightArm.localRotation=Quaternion.Euler(-65-95*jab,0,10);
                    rightElbow.localRotation=Quaternion.Euler(-65+65*jab,0,0);
                    torso.localRotation=Quaternion.Euler(0,15*jab,0);
                    break;
                case RoughHousePrototype.Move.Hook:
                    rightArm.localRotation=Quaternion.Euler(-105,30+85*jab,22);
                    torso.localRotation=Quaternion.Euler(0,48*jab,0);
                    break;
                case RoughHousePrototype.Move.Uppercut:
                    rightArm.localRotation=Quaternion.Euler(-50+125*jab,0,20);
                    torso.localRotation=Quaternion.Euler(-20*jab,14*jab,0);
                    break;
                case RoughHousePrototype.Move.QuickKick:
                    rightLeg.localRotation=Quaternion.Euler(85*jab,0,-12);
                    torso.localRotation=Quaternion.Euler(-18*jab,0,0);
                    break;
                case RoughHousePrototype.Move.SpinKick:
                    rightLeg.localRotation=Quaternion.Euler(100*jab,0,-10);
                    torso.localRotation=Quaternion.Euler(0,270*jab,0);
                    break;
            }
        }
        float total=reactDuration>0 ? 1f-reactTime/reactDuration : 1f;
        float tilt=0, height=0;
        switch(currentReaction)
        {
            case RoughHousePrototype.Reaction.Hit:
                tilt= -18*Mathf.Sin(Mathf.PI*total);
                if (reactTime<=0) currentReaction=RoughHousePrototype.Reaction.None;
                break;
            case RoughHousePrototype.Reaction.Crumple:
                tilt=65*Mathf.SmoothStep(0,1,total);
                height=-.63f*Mathf.SmoothStep(0,1,total);
                break;
            case RoughHousePrototype.Reaction.Knockdown:
                tilt=80; height=-.72f;
                break;
            case RoughHousePrototype.Reaction.Airborne:
                tilt=28*Mathf.Sin(Mathf.PI*total);
                height=1.55f*Mathf.Sin(Mathf.PI*total);
                if(reactTime<=0) { currentReaction=RoughHousePrototype.Reaction.Knockdown; reactTime=2; reactDuration=2; }
                break;
            case RoughHousePrototype.Reaction.GettingUp:
                tilt=80*(1-Mathf.SmoothStep(0,1,total));
                height=-.72f*(1-Mathf.SmoothStep(0,1,total));
                if(reactTime<=0) currentReaction=RoughHousePrototype.Reaction.None;
                break;
        }
        modelRoot.localRotation=Quaternion.Euler(tilt,0,0);
        modelRoot.localPosition=new Vector3(0,height+Mathf.Sin(timer*6)*.018f,0);
    }
}
