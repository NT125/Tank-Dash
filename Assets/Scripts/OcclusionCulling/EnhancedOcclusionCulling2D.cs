using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class EnhancedOcclusionCulling2D : MonoBehaviour
{
    /*Guardamos los elementos que se activaan cuando estn en camara y luego se desactivaran*/
    [SerializeField] private GameObject[] objects = new GameObject[0];
    [System.Serializable]
    internal class ObjectSettings
    {
        [SerializeField, HideInInspector] internal string title;
        [SerializeField, HideInInspector] internal GameObject theGameObject;
        [SerializeField, HideInInspector] internal Renderer theRenderer;//Para calcular el tamanio y bound

        [SerializeField, HideInInspector] internal Vector2 sized, center, TopRight, TopLeft, BottomLeft, BottomRight;//Tambien guardamos la posiciones y tamanio para hacer el culling
        [SerializeField, HideInInspector] internal float right, left, top, bottom;

        [SerializeField] internal Vector2 additiveSize = Vector2.zero;//Ajustar el tamanio adicional para ajustar el area de culling 
        /*Configuraicion visual yy optimizaicoin */
        [SerializeField] internal bool showBorders = true;
        [SerializeField] internal Color drawColor = Color.white;
        [Header("'Is Static' Prevents The Calculated Bordered Positions' Values Of The Game Object From Being Updated In Run Time, So It Boosts Performance If You Have A Lot Of Game Objects")]
        [SerializeField] internal bool isStatic = true;
    }

    [SerializeField] private ObjectSettings[] objectsSettings = new ObjectSettings[1];

    [SerializeField] float updateRateInSeconds = 0.1f;//Tiempo de actualizacion del culling

    [SerializeField] private Vector2 additiveSizeAll = Vector2.zero;//Agregar tamanio extra a los objetos

    [Space, SerializeField] private bool overrideAllObjectsSettings = true;//Activar ajustes globales de todos los objetos
    [SerializeField, HideInInspector] private bool overridingShowBordersAll = true;
    [SerializeField, HideInInspector] private Color overridingDrawColorAll = Color.white;
    [Header("'Is Static' Prevents The Calculated Bordered Positions' Values Of The Game Object From Being Updated In Run Time, So It Boosts Performance If You Have A Lot Of Game Objects")]
    [SerializeField, HideInInspector] private bool overridingIsStaticAll = true;

    private float timer;
    private new Camera camera;

    [SerializeField, HideInInspector] private bool isInitialized;
    private bool hasSavedOverridingSettings = true;

    /*Hcemos que el cuadro cubra todo el render*/
    private Bounds GetCombinedBounds(GameObject parent)
    {
        Vector3 absScale = new Vector3(Mathf.Abs(parent.transform.localScale.x), Mathf.Abs(parent.transform.localScale.y), 0);
        Bounds combinedBounds = new Bounds(parent.transform.position, absScale);
        Renderer[] renderers = parent.GetComponentsInChildren<Renderer>(GetComponent<Renderer>());
        foreach (Renderer rendererChild in renderers)
        {
            if (combinedBounds.size == absScale)
            {
                combinedBounds = rendererChild.bounds;
            }
            combinedBounds.Encapsulate(rendererChild.bounds);
        }       
        return combinedBounds;
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(EnhancedOcclusionCulling2D))]
    private class EnhancedOcclusionCulling2DEditor : Editor
    {
        private EnhancedOcclusionCulling2D reference;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (reference == null)
            {
                reference = (EnhancedOcclusionCulling2D)target;
                return;
            }

            if (reference.overrideAllObjectsSettings)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("overridingShowBordersAll"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("overridingDrawColorAll"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("overridingIsStaticAll"));
                if (GUILayout.Button("Replace Objects' Settings With Overriding Settings"))
                {
                    reference.hasSavedOverridingSettings = false;
                    if (!reference.hasSavedOverridingSettings)
                    {
                        foreach (ObjectSettings o in reference.objectsSettings)
                        {
                            o.showBorders = reference.overridingShowBordersAll;
                            o.drawColor = reference.overridingDrawColorAll;
                            o.isStatic = reference.overridingIsStaticAll;

                            if (o.theGameObject == reference.objectsSettings[reference.objectsSettings.Length - 1].theGameObject)
                            {
                                reference.hasSavedOverridingSettings = true;
                            }
                        }
                    }
                }
            }
            serializedObject.ApplyModifiedProperties();
            if (!Application.isPlaying)
            {
                if (reference.objectsSettings.Length != reference.objects.Length)
                {
                    reference.isInitialized = false;
                    reference.objectsSettings = new ObjectSettings[reference.objects.Length];
                    return;
                }
                if (!reference.isInitialized)
                {
                    for (int i = 0; i < reference.objectsSettings.Length; i++)
                    {
                        reference.objectsSettings[i].theGameObject = reference.objects[i];
                        reference.objectsSettings[i].theRenderer = reference.objects[i].GetComponent<Renderer>();
                        if (i == reference.objectsSettings.Length - 1)
                            reference.isInitialized = true;
                    }
                }
            }
        }

        public void OnSceneGUI()
        {
            if (reference == null)
            {
                reference = (EnhancedOcclusionCulling2D)target;
                return;
            }
            if (reference.isInitialized)
            {
                foreach (ObjectSettings o in reference.objectsSettings)
                {
                    if (o.theGameObject)
                    {
                        o.title = o.theGameObject.name;
                        if (Selection.activeGameObject == reference.gameObject | !Application.isPlaying)
                        {

                            Vector2 addSize = reference.additiveSizeAll + o.additiveSize;
                            Bounds bounds = reference.GetCombinedBounds(o.theGameObject);
                            o.sized = (Vector2)bounds.extents + addSize;
                            o.center = bounds.center;

                            o.TopRight = new Vector2(o.center.x + o.sized.x, o.center.y + o.sized.y);
                            o.TopLeft = new Vector2(o.center.x - o.sized.x, o.center.y + o.sized.y);
                            o.BottomLeft = new Vector2(o.center.x - o.sized.x, o.center.y - o.sized.y);
                            o.BottomRight = new Vector2(o.center.x + o.sized.x, o.center.y - o.sized.y);

                            o.right = o.center.x + o.sized.x;
                            o.left = o.center.x - o.sized.x;
                            o.top = o.center.y + o.sized.y;
                            o.bottom = o.center.y - o.sized.y;

                            bool checkShowBorders = reference.overrideAllObjectsSettings ? reference.overridingShowBordersAll : o.showBorders;

                            if (checkShowBorders)
                            {
                                o.TopRight = new Vector2(o.center.x + o.sized.x, o.center.y + o.sized.y);
                                o.TopLeft = new Vector2(o.center.x - o.sized.x, o.center.y + o.sized.y);
                                o.BottomLeft = new Vector2(o.center.x - o.sized.x, o.center.y - o.sized.y);
                                o.BottomRight = new Vector2(o.center.x + o.sized.x, o.center.y - o.sized.y);
                                Handles.color = reference.overrideAllObjectsSettings ? reference.overridingDrawColorAll : o.drawColor;
                                Handles.DrawLine(o.TopRight, o.TopLeft);
                                Handles.DrawLine(o.TopLeft, o.BottomLeft);
                                Handles.DrawLine(o.BottomLeft, o.BottomRight);
                                Handles.DrawLine(o.BottomRight, o.TopRight);
                            }
                            bool checkStatic = reference.overrideAllObjectsSettings ? reference.overridingIsStaticAll : o.isStatic;
                            o.theGameObject.isStatic = checkStatic;
                        }
                    }
                }
            }
        }
    }
#endif
    void Awake()
    {
        camera = GetComponent<Camera>();
    }
    void FixedUpdate()
    {
        timer += Time.deltaTime;
        if (timer > updateRateInSeconds) timer = 0;
        else return;

        float cameraHalfWidth = camera.orthographicSize * ((float)Screen.width / (float)Screen.height);
        float cameraRight = camera.transform.position.x + cameraHalfWidth;
        float cameraLeft = camera.transform.position.x - cameraHalfWidth;
        float cameraTop = camera.transform.position.y + camera.orthographicSize;
        float cameraBottom = camera.transform.position.y - camera.orthographicSize;

        foreach (ObjectSettings o in objectsSettings)
        {
            if (o.theGameObject)
            {
                bool checkStatic = overrideAllObjectsSettings ? overridingIsStaticAll : o.isStatic;
                if (!checkStatic)
                {
                    o.center = GetCombinedBounds(o.theGameObject).center;
                    o.right = o.center.x + o.sized.x;
                    o.left = o.center.x - o.sized.x;
                    o.top = o.center.y + o.sized.y;
                    o.bottom = o.center.y - o.sized.y;
                }
                bool IsObjectVisibleInCastingCamera = o.right >= cameraLeft & o.left <= cameraRight & // verificiacion horizontal
                o.top >= cameraBottom & o.bottom <= cameraTop; // verificacion vertical
                o.theGameObject.SetActive(IsObjectVisibleInCastingCamera);
            }
        }
    }
}