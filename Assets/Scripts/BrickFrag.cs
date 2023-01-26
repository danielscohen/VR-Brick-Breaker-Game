using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BrickFrag : MonoBehaviour
{
    class Fragment {
        public GameObject fragment;
        public FragController fragC;
        public bool isDestroyed;

        public Fragment(GameObject fragment, FragController fragc) {
            this.fragment = fragment;
            this.fragC = fragc;
            this.isDestroyed = false;
        }
    }

    class DrawPt {
        public Vector3 pt;
        public int epoch;

        public DrawPt(Vector3 pt, int epoch) {
            this.pt = pt;
            this.epoch = epoch;
        }
    }


    [SerializeField] GameObject cam;
    [SerializeField] FragSpawner fragSpawner;
    [SerializeField] GameObject lightExpPrefab;
    [SerializeField] float particleExpDelay = 1f;
    [SerializeField] int ptsPerUnit = 10;
    [SerializeField] float coneAngle = 30f;
    [SerializeField] int fracBranchDepth = 1;
    [SerializeField] float fragSizeMult = 10f;
    [SerializeField] float lineWidth = 0.005f;
    [SerializeField] Material lineMaterial;
    [SerializeField] int fracBranchProb = 2;
    [SerializeField] float craterRadMult = 1f;
    [SerializeField] int maxFragDepth = 5;
    [SerializeField] float fracLineDrawDelay = 0.1f;
    [SerializeField] float fragExpDelay = 0.1f;
    [SerializeField] float voxFadePer = 0.2f;
    [SerializeField] float voxFadeOutDur = 0.5f;
    [SerializeField] float voxFadeInDur = 0.1f;
    [SerializeField] int minFragSizeKeep = 1;
    [SerializeField] int particleProb = 4;
    [SerializeField] int PowerUpSpawnProb = 10;
    [SerializeField] AudioClip _impactAudio;
    [SerializeField] AudioClip _fracAudio;
    [SerializeField] AudioClip _explosionAudio;
    [SerializeField] GameObject _explosionPrefab;
    [SerializeField] GameObject _sparkPrefab;
    public static event System.Action<Vector3> onSpawnPowerUp;
    
    int[,,] voxMap;
    public GameObject[,,] voxels;
    List<Fragment> frags = new List<Fragment>();
    List<LineRenderer> fracRenderers = new List<LineRenderer>();
    List<Vector3> fracLinePts = new List<Vector3>();
    List<Vector3> craterFragsPos = new List<Vector3>();
    List<List<DrawPt>> fracDrawPts = new List<List<DrawPt>>();
    List<Vector3Int> deletedVoxels = new List<Vector3Int>();

    ArenaManager _arenaManager;

    AudioSource _audioSource;
    ParticleSystem _explosionPS;



    Vector3 brickSize;
    Vector3Int numVoxels;
    Vector3 ballCollDir;
    float ballCollSpeed;

    Vector3 collisionPt;

    float craterRadius;

    int numTimeFrag = 0;

    bool brickAlreadyHit = false;

    AudioSource _fracAudioSource;

    bool _isPLayingFracAudio = false;
    GameObject _fracSpark;
    bool _fracSparkIsActive = false;

    void OnEnable() {
        // GameController.onPauseGame += SetBrickInvisible;
        GameController.onPauseGame += PauseFracAudio;
        // GameController.onPauseGame += SetFracLinesInvisible;
        // GameController.onResumeGame += SetBrickVisible;
        GameController.onResumeGame += ResumeFracAudio;
        // GameController.onResumeGame += SetFracLinesVisible;
        // GameController.onGameOver += SetBrickInvisible;
        GameController.onGameOver += PauseFracAudio;
        // GameController.onGameOver += SetFracLinesInvisible;
        // GameController.onGameOver += DeleteFracSpark;
    }

    void OnDisable() {
        // GameController.onPauseGame -= SetBrickInvisible;
        GameController.onPauseGame -= PauseFracAudio;
        // GameController.onPauseGame -= SetFracLinesInvisible;
        // GameController.onResumeGame -= SetBrickVisible;
        GameController.onResumeGame -= ResumeFracAudio;
        // GameController.onResumeGame -= SetFracLinesVisible;
        // GameController.onGameOver -= SetBrickInvisible;
        GameController.onGameOver -= PauseFracAudio;
        // GameController.onGameOver -= SetFracLinesInvisible;
        // GameController.onGameOver -= DeleteFracSpark;
    }



    private void Awake() {
        _audioSource = GetComponent<AudioSource>();
        // _explosionPS = Instantiate(_explosionPrefab, transform).GetComponent<ParticleSystem>();
    }
    private void Start() {
        cam = Camera.main.gameObject;
        fragSpawner = GameObject.Find("Frag Spawner").GetComponent<FragSpawner>();
        _arenaManager = GameObject.Find("Arena").GetComponent<ArenaManager>();
    }

    void SetBrickVisible(){
        if(brickAlreadyHit) return;
        gameObject.GetComponent<Renderer>().enabled = true;
    }
    void SetBrickInvisible(){
        gameObject.GetComponent<Renderer>().enabled = false;
    }

    void PauseFracAudio(){
        if(_isPLayingFracAudio){
            _fracAudioSource.Pause();
        }
    }
    void ResumeFracAudio(){
        if(_isPLayingFracAudio){
            _fracAudioSource.Play();
        }
    }
    void DeleteFracSpark(){
        if(_fracSparkIsActive){
            Destroy(_fracSpark);
        }
    }

    void SetFracLinesInvisible(){
        foreach(LineRenderer lineRen in fracRenderers){
            lineRen.enabled = false;
        }
    }
    void SetFracLinesVisible(){
        foreach(LineRenderer lineRen in fracRenderers){
            lineRen.enabled = true;
        }
    }


    private void OnCollisionEnter(Collision other) {
        if(!other.collider.CompareTag("Ball") || brickAlreadyHit || !other.gameObject.GetComponent<BallController>().IsCollisionAllowed()) return;
        // GetComponentInParent<Animator>().speed = 0;
        var ballCollVel = other.gameObject.GetComponent<BallController>().PrevVelocity;

        _arenaManager.NumBricksRemaining--;

        brickAlreadyHit = true;

        GetComponent<Collider>().enabled = false;
        GetComponent<Renderer>().enabled = false;
        brickSize = Vector3.Scale(transform.parent.localScale, transform.localScale);
        numVoxels = new Vector3Int(voxels.GetLength(0), voxels.GetLength(1), voxels.GetLength(2));
        ballCollDir = ballCollVel.normalized;
        ballCollSpeed = ballCollVel.magnitude;
        // Debug.Log($"speed: {ballCollSpeed}");
        craterRadius = ballCollSpeed * craterRadMult;
        collisionPt = other.GetContact(0).point;

        ptsPerUnit = (int)Mathf.Floor(ballCollSpeed * 10);

        StartCoroutine(StartCollActions());
    }

    IEnumerator StartCollActions() {
        MakeVoxMap();

        AudioManager.Instance.PlayAudio(AudioTypes.BallHitBrick1, transform.position);

        MakeCrater();

        MakeVoxelsVisible();
        StartCoroutine(CreateParticleStream(craterFragsPos));
        //yield return new WaitForSeconds(craterFracDelay);

        CreateFracLines();
        // Debug.Log($"frac line pts: {fracLinePts.Count}");
        yield return StartCoroutine(FadeVoxels(voxFadePer, voxFadeOutDur));
        _fracAudioSource = AudioManager.Instance.StartFracAudio(transform.position);
        _isPLayingFracAudio = true;
        yield return StartCoroutine(DrawFracLines());
        _isPLayingFracAudio = false;
        AudioManager.Instance.StopFracAudio(_fracAudioSource);
        yield return StartCoroutine(FadeVoxels(1f, voxFadeInDur));
        DeleteFracLines();



        int maxGroupIndex = generateVoxGroups();


        GenerateFrags(maxGroupIndex);



        yield return new WaitForSeconds(particleExpDelay);


        DeleteSmallFracs();

        if(UnityEngine.Random.Range(0, PowerUpSpawnProb) == 0){
            onSpawnPowerUp?.Invoke(transform.position);
        }

        MakeFlash();

        AudioManager.Instance.PlayAudio(AudioTypes.BallHitBrick3, transform.position);

        for (int i = 0; i < frags.Count; i++) {
            frags[i].fragC.MakeFragExplode(GetWorldFracPoints());
            // if (i % 1 == 0) {
            //     yield return new WaitForSeconds(fragExpDelay);
            // }
        }

    }


    // IEnumerator CreateLightExplosion()
    // {
    //     GameObject lightExp = Instantiate(lightExpPrefab, transform, false);
    //     yield return new WaitForSeconds(0.1f);
    // }

    void MakeFlash(){
        Instantiate(_explosionPrefab, transform);
    }

    void CreateFracLines() {
        var brickBounds = GetComponent<Renderer>().bounds;
        Vector3 startPt = collisionPt;
        startPt = NudgeCollisionPt(ballCollDir);
        //Debug.DrawRay(collisionPt, ballCollDir, Color.green, 1000f);

        Vector3 dirToCenter = (transform.TransformPoint(Vector3.zero) - startPt).normalized;

        Vector3 dir = ballCollDir;
        int numRots = 0;

        while (true) {
            if (CreateFracLine(startPt, dir, brickBounds, fracBranchDepth, 0, true) > 20){
                ConvertFracPointsToLocal();
                // Debug.Log($"num rots: {numRots}");
                return;
            }
            if (Vector3.Angle(dir, dirToCenter) < 1f){
                ConvertFracPointsToLocal();
                // Debug.Log($"num rots forced to stop: {numRots}");
                return;
            } 
            dir = Vector3.RotateTowards(dir, dirToCenter, 0.08f, 0);
            numRots++;
        }
    }

    void ConvertFracPointsToLocal(){
        for(int i = 0; i < fracLinePts.Count; i++){
            fracLinePts[i] = transform.InverseTransformPoint(fracLinePts[i]);
        }
    }

    List<Vector3> GetWorldFracPoints(){
        var fracPts = new List<Vector3>();
        for(int i = 0; i < fracLinePts.Count; i++){
            fracPts.Add(transform.TransformPoint(fracLinePts[i]));
        }
        return fracPts;
    }


    bool ContainedInBrick(Vector3 pt) {
        Vector3 localPos = transform.InverseTransformPoint(pt);
        return ApproxOrGreater(localPos.x, -0.5f) && ApproxOrGreater(localPos.y, -0.5f) && ApproxOrGreater(localPos.z, -0.5f) &&
        ApproxOrGreater(0.5f, localPos.x) && ApproxOrGreater(0.5f, localPos.y) && ApproxOrGreater(0.5f, localPos.z);
    }

    bool ApproxOrGreater(float a, float b) {
        return Mathf.Abs(a - b) < 0.01 || a > b;
    }

    Vector3 NudgeCollisionPt(Vector3 dir) {
        float startDist = Mathf.Min(brickSize.x, brickSize.y, brickSize.z) / 10f;
        return collisionPt + dir * startDist;
    }

    int CreateFracLine(Vector3 branchPt, Vector3 fracDir, Bounds brickBounds, int branchDepth, int epoch, bool firstLine) {
        List<Vector3> fracPts = new List<Vector3>();
        if (!firstLine) {
            fracDir = PerturbVectorDir(fracDir).normalized;
        }
        float distToBound = GetLineBrickBoundsIntersect(branchPt, fracDir);
        //Debug.Log($"dist to bound: {distToBound}");
        if (distToBound < 0) return 0;
        Vector3 boundPt = branchPt + fracDir * distToBound;


        var numPts = (int)(ptsPerUnit * Mathf.Abs(distToBound));
        //Debug.Log($"num points: {numPts}");
        if(numPts == 0) return 0;
        fracDrawPts.Add(new List<DrawPt>());
        for (int i = 0; i <= numPts; i++) {
            var point = Vector3.Lerp(branchPt, boundPt, (float)i / numPts);
            fracPts.Add(point);
            fracDrawPts.Last().Add(new DrawPt(transform.InverseTransformPoint(point), epoch + i));
        }
        fracLinePts.AddRange(fracPts);
        for(int i = 1; i < fracPts.Count; i++) {
            if (branchDepth > 0 && UnityEngine.Random.Range(0, fracBranchProb) == 0) {
                CreateFracLine(fracPts[i], fracDir, brickBounds, branchDepth - 1, epoch, false);
            }
            epoch++;
        }
        return fracLinePts.Count;
    }

    float GetLineBrickBoundsIntersect(Vector3 pt, Vector3 dir) {
        Ray ray = new Ray(pt, dir);
        Plane[] brickFaces = new Plane[6];
        Vector3[] vecDirs = new Vector3[] {transform.up, -transform.up, transform.right, -transform.right, transform.forward, -transform.forward};
        Vector3[] planePts = new Vector3[] {Vector3.up, Vector3.down, Vector3.right, Vector3.left, Vector3.forward, Vector3.back};

        for (int i = 0; i < vecDirs.Length; i++) {
            brickFaces[i] = new Plane(vecDirs[i], transform.TransformPoint(planePts[i] / 2));
        }


        float minDist = -1f;

        foreach (Plane face in brickFaces) {
            float enter = 0.0f;
            if(face.Raycast(ray, out enter)) {
                Vector3 collPt = pt + dir * enter;
                if (ContainedInBrick(collPt)) {
                    if (minDist < 0) minDist = enter;
                    else if (enter < minDist) minDist = enter;
                }
            }
        }

        return minDist;
    }

    bool IsInCrater(Vector3 pos) {
        return Vector3.Distance(pos, collisionPt) < craterRadius;
    }
    IEnumerator DrawFracLines() {
        foreach(var l in fracDrawPts) {
            if (l.Count > 0) {
                l.RemoveAt(l.Count - 1);
                l.RemoveAll(p => IsInCrater(p.pt));
            }
            var line = new GameObject();
            line.transform.parent = transform;
            var lineR = line.AddComponent<LineRenderer>();
            fracRenderers.Add(lineR);
            lineR.useWorldSpace = false;
            lineR.material = lineMaterial;
            lineR.startWidth = lineWidth;
            lineR.endWidth = lineWidth;
            lineR.positionCount = 0;
        }

        _fracSpark = Instantiate(_sparkPrefab);
        _fracSparkIsActive = true;
        
        int max = GetMaxEpoch();
        for (int i = 0; i < max; i++) {
            for (int j = 0; j < fracDrawPts.Count; j++) {
                var index = fracDrawPts[j].FindIndex(x => x.epoch == i);
                if (index != -1) {
                    fracRenderers[j].positionCount = index + 1;
                    Vector3 loc = transform.TransformPoint(fracDrawPts[j][index].pt);
                    _fracSpark.transform.position = loc;
                    fracRenderers[j].SetPosition(index, fracRenderers[j].transform.InverseTransformPoint(loc));
                }
            }
            if (i % 1 == 0) {
                yield return new WaitForSeconds(fracLineDrawDelay);
            }  
        }

        _fracSparkIsActive = false;
        Destroy(_fracSpark);


    }

    int GetMaxEpoch() {
        if (fracDrawPts.Count == 0) return 0;
        return fracDrawPts.Max(x => x.Count);
    }


    Vector3 PerturbVectorDir(Vector3 fracDir) {
        Vector3 randVec;
        // Generate random vector that is linearly indep. from fracDir
        while (true) {
            randVec = new Vector3(Random.Range(-10f, 10f), Random.Range(-10f, 10f), Random.Range(-10f, 10f));
            if (randVec != Vector3.zero && Vector3.Cross(randVec, fracDir) != Vector3.zero) {
                break;
            }
        }
        var perpVec = Vector3.Cross(fracDir, randVec).normalized * fracDir.magnitude;
        var randTheta = Random.Range(1f, coneAngle) * Mathf.Deg2Rad;
        var perturbvec = fracDir * Mathf.Cos(randTheta) + perpVec * Mathf.Sin(randTheta);
        return perturbvec;
    }


    void MakeVoxelsVisible() {
        for (int x = 0; x < numVoxels.x; x++) {
            for (int y = 0; y < numVoxels.y; y++) {
                for (int z = 0; z < numVoxels.z; z++) {
                    if(voxMap[x,y,z] != -2) {
                        voxels[x,y,z].GetComponent<Renderer>().enabled = true;
                        voxels[x,y,z].GetComponent<Collider>().enabled = true;
                    }
                }
            }
        }
    }

    void DeleteSmallFracs() {
        foreach(var frag in frags) {
            if (frag.fragC.FragSize < minFragSizeKeep) {
                frag.fragC.DeleteFrag();
                frag.isDestroyed = true;
            }
        }
        frags.RemoveAll(frag => frag.isDestroyed);
    }


    IEnumerator FadeVoxels(float fadePer, float fadeDur) {
        List<Material> voxMats = new List<Material>();
        List<Color> startColors = new List<Color>();
        List<Color> endColors = new List<Color>();

        for (int x = 0; x < numVoxels.x; x++) {
            for (int y = 0; y < numVoxels.y; y++) {
                for (int z = 0; z < numVoxels.z; z++) {
                    if(voxMap[x,y,z] != -2) {
                        Material mat = voxels[x,y,z].GetComponent<Renderer>().material;
                        voxMats.Add(mat);
                        startColors.Add(mat.color);
                        Color endColor = mat.color;
                        endColor.a = fadePer;
                        endColors.Add(endColor);
                    }
                }
            }
        }

        float time = 0f;
       
        while(time < fadeDur) {
            for(int i = 0; i < voxMats.Count; i++) {
                voxMats[i].color = Color.Lerp(startColors[i], endColors[i], time / fadeDur);
            }
            time += Time.deltaTime;
            yield return null;
        }
    }

    void MakeCrater() {
        for (int x = 0; x < numVoxels.x; x++) {
            for (int y = 0; y < numVoxels.y; y++) {
                for (int z = 0; z < numVoxels.z; z++) {
                    if (IsInCrater(voxels[x, y, z].transform.position)) {
                        voxMap[x, y, z] = -2;
                        deletedVoxels.Add(new Vector3Int (x, y, z));
                        craterFragsPos.Add(voxels[x, y, z].transform.position);
                        voxels[x, y, z].transform.parent = null;
                        voxels[x,y,z].SetActive(false);
                    }
                }
            }
        }

    }

    void DeleteFracLines() {
        foreach (LineRenderer line in fracRenderers) {
            line.enabled = false;
        }
    }

    IEnumerator CreateParticleStream(List<Vector3> destroyedFragPos) {

        foreach(Vector3 pos in destroyedFragPos.ToList()) {
            if (Random.Range(0, particleProb) == 0) {
                int r = Random.Range(1, 3);
                for (int i = 0; i < r; i++) {
                    var particle = GameObject.Find("ParticleSpawner").GetComponent<ParticleSpawner>().GetNewParticle(pos, Random.rotation);
                    var particleC = particle.GetComponent<ParticleController>();
                    particleC.Init(cam.transform.position);
                    StartCoroutine(particleC.FadeOut());
                    yield return null;
                }
            }
        }

    }

    void MakeVoxMap() {

        voxMap = new int[numVoxels.x, numVoxels.y, numVoxels.z];

        for (int x = 0; x < numVoxels.x; x++) {
            for (int y = 0; y < numVoxels.y; y++) {
                for (int z = 0; z < numVoxels.z; z++) {
                    voxMap[x, y, z] = -1;
                }
            }
        }
    }

    int generateVoxGroups() {
        int xLen = numVoxels.x;
        int yLen = numVoxels.y;
        int zLen = numVoxels.z;
        List<int> freeIndices = new List<int>();
        for (int i = 0; i < xLen * yLen * zLen; i++) {
            freeIndices.Add(i);
        }

        foreach(Vector3Int vox in deletedVoxels) {
            freeIndices.Remove(vox.x + vox.y * voxMap.GetLength(0) + vox.z * voxMap.GetLength(0) * voxMap.GetLength(1));
        }

        int groupIndex = 0;

        while (freeIndices.Count > 0) {
            int i = Random.Range(0, freeIndices.Count);
            int r = freeIndices[i];
            int z = (int)(r / (xLen * yLen));
            int y = (int)((r % (xLen * yLen)) /  xLen);
            int x = (int)((r % (xLen * yLen)) % xLen);

            Vector3 closestFracPt = UtilFunctions.FindClosestFracPt(voxels[x, y, z].transform.position, fracLinePts);

            float distFromFrac = Vector3.Distance(closestFracPt, voxels[x, y, z].transform.position);
            int prob = (int)(UtilFunctions.CalcDistScore(distFromFrac, brickSize));
            StartMakeVoxGroup(x, y, z, prob, groupIndex, freeIndices);


            groupIndex++;
        }

        int numbad = 0;
        foreach(int i in voxMap) {
            if (i == -1) numbad++; 
        }
        numTimeFrag++;
        if (numbad > 0) Debug.Log($"num -1 in voxmap: {numbad}, numtime create: {numTimeFrag}");

        return groupIndex;
    }

    void StartMakeVoxGroup(int x, int y, int z, int prob, int groupIndex, List<int> freeIndices) {
        voxMap[x, y, z] = groupIndex;
        freeIndices.Remove(x + y * voxMap.GetLength(0) + z * voxMap.GetLength(0) * voxMap.GetLength(1));

        MakeVoxGroup(x + 1, y, z, prob, groupIndex, freeIndices, maxFragDepth);
        MakeVoxGroup(x - 1, y, z, prob, groupIndex, freeIndices, maxFragDepth);
        MakeVoxGroup(x, y + 1, z, prob, groupIndex, freeIndices, maxFragDepth);
        MakeVoxGroup(x, y - 1, z, prob, groupIndex, freeIndices, maxFragDepth);
        MakeVoxGroup(x, y, z + 1, prob, groupIndex, freeIndices, maxFragDepth);
        MakeVoxGroup(x, y, z - 1, prob, groupIndex, freeIndices, maxFragDepth);
    }

    void MakeVoxGroup(int x, int y, int z, int prob, int groupIndex, List<int> freeIndices, int depth) {
        if (depth == 0) return;
        if(x < 0 || y < 0 || z < 0) return;
        if (x >= numVoxels.x || y >= numVoxels.y || z >= numVoxels.z) return;
        if(voxMap[x, y, z] != -1) return;
        int r = Random.Range(1, (int)((1f / fragSizeMult) * (fracLinePts.Count / 10)));
        if (r > prob) return;
        voxMap[x, y, z] = groupIndex;
        freeIndices.Remove(x + y * voxMap.GetLength(0) + z * voxMap.GetLength(0) * voxMap.GetLength(1));

        prob = prob < 10 ? 0 : prob - 10;

        MakeVoxGroup(x + 1, y, z, prob, groupIndex, freeIndices, depth - 1);
        MakeVoxGroup(x - 1, y, z, prob, groupIndex, freeIndices, depth - 1);
        MakeVoxGroup(x, y + 1, z, prob, groupIndex, freeIndices, depth - 1);
        MakeVoxGroup(x, y - 1, z, prob, groupIndex, freeIndices, depth - 1);
        MakeVoxGroup(x, y, z + 1, prob, groupIndex, freeIndices, depth - 1);
        MakeVoxGroup(x, y, z - 1, prob, groupIndex, freeIndices, depth - 1);

    }



    bool IsEdgeVox(Vector3 loc) {
        return loc.x == 0 || loc.x == numVoxels.x - 1 ||
            loc.y == 0 || loc.y == numVoxels.y - 1 ||
            loc.z == 0 || loc.z == numVoxels.z - 1; 
    }

    void GenerateFrags(int MaxGroupIndex) {
        Vector3[] posSums = new Vector3[MaxGroupIndex];
        int[] groupCounts = new int[MaxGroupIndex];

        for (int x = 0; x < numVoxels.x; x++) {
            for (int y = 0; y < numVoxels.y; y++) {
                for (int z = 0; z < numVoxels.z; z++) {
                    if (voxMap[x, y, z] == -2) continue;
                    if (voxMap[x, y, z] >= MaxGroupIndex || voxMap[x,y,z] < 0) {
                        Debug.Log($"Vox Map: {voxMap[x, y, x]}, Max Index: {MaxGroupIndex}");
                    }
                    posSums[voxMap[x, y, z]] += voxels[x, y, z].transform.position;
                    groupCounts[voxMap[x, y, z]]++;
                }
            }
        }


        for (int i = 0; i < MaxGroupIndex; i++) {
            GameObject parent = fragSpawner.GetNewFrag(posSums[i] / groupCounts[i], Quaternion.identity);
            FragController fragC = parent.GetComponent<FragController>();
            fragC.Init(groupCounts[i], transform.position, brickSize, collisionPt, ballCollSpeed / 10, GetComponent<BrickFrag>());
            parent.name = "Frag_" + i;
            frags.Add(new Fragment(parent, fragC));
        }


        for (int x = 0; x < numVoxels.x; x++) {
            for (int y = 0; y < numVoxels.y; y++) {
                for (int z = 0; z < numVoxels.z; z++) {
                    if (voxMap[x, y, z] == -2) continue;
                    voxels[x, y, z].transform.SetParent(frags[voxMap[x,y,z]].fragment.transform);
                }
            }
        }

        frags.Sort((x, y) => x.fragC.nDist.CompareTo(y.fragC.nDist));

    }



}
