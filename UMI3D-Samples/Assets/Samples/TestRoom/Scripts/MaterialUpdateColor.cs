using System;
using System.Collections;
using System.Collections.Generic;
using umi3d.edk;
using UnityEngine;

public class MaterialUpdateColor : MonoBehaviour
{
    /// <summary>
    /// Color used to overide materials
    /// </summary>
    public Color color = Color.white;

    /// <summary>
    /// Model to display the current random color
    /// </summary>
    public UMI3DModel currentColorDisplayer;

    /// <summary>
    /// plot prefab to test override on specific material
    /// </summary>
    public UMI3DModel plot;

    public MaterialSO matToAdd;

    private PBRMaterial pBRMaterial;
    private Color originalColor = new Color();
    private MaterialOverrider materialOverrider = new MaterialOverrider();

    // Start is called before the first frame update
    void Start()
    {
        //originalColor = pBRMaterial.objectBaseColorFactor.GetValue();
        materialOverrider.overrideAllMaterial = true;
        pBRMaterial = ScriptableObject.CreateInstance<PBRMaterial>();
        pBRMaterial.baseColorFactor = color;
        materialOverrider.newMaterial = pBRMaterial;
        materialOverrider.newMaterial.name = "new random material";

        Transaction transaction = new Transaction();
        // add Materials send to client and and in preloaded for futur client
        transaction.AddIfNotNull(pBRMaterial.GetLoadEntity());
        transaction.AddIfNotNull(matToAdd.GetLoadEntity());
        UMI3DEnvironment.Instance.GetComponentInChildren<UMI3DScene>().PreloadedMaterials.Add(pBRMaterial);
        UMI3DEnvironment.Instance.GetComponentInChildren<UMI3DScene>().PreloadedMaterials.Add(matToAdd);


        // add overider 
        transaction.AddIfNotNull(currentColorDisplayer.objectMaterialsOverrided.SetValue(true));
        transaction.AddIfNotNull(currentColorDisplayer.objectMaterialOverriders.SetValue(new List<MaterialOverrider>() { materialOverrider }));

        transaction.Dispatch();

    }


    public void RandomColor()
    {
        color = UnityEngine.Random.ColorHSV();
        color.a = 1f;
        Transaction transaction = new Transaction();
        transaction.AddIfNotNull(pBRMaterial.objectBaseColorFactor.SetValue(color));

        transaction.Dispatch();
    }

    public void ChangeColorOnMat(PBRMaterial mat)
    {
        materialsToReset.Add(new Tuple<PBRMaterial, Color>(mat, mat.objectBaseColorFactor.GetValue()));
        Transaction transaction = new Transaction();
        transaction.AddIfNotNull(mat.objectBaseColorFactor.SetValue(color));
        transaction.Dispatch();
    }

    //Change color property
    public void ChangeOneMaterial(string materialName)
    {
        Transaction transaction = new Transaction();
        transaction.AddIfNotNull(plot.objectMaterialsOverrided.SetValue(true));
        var mat = ScriptableObject.CreateInstance<PBRMaterial>();
        mat.baseColorFactor = color;
        // add mat on resources 
        transaction.AddIfNotNull(mat.GetLoadEntity());
        UMI3DEnvironment.Instance.GetComponentInChildren<UMI3DScene>().PreloadedMaterials.Add(mat);
        var overrider = new MaterialOverrider()
        {
            addMaterialIfNotExists = false,
            overrideAllMaterial = false,
            newMaterial = mat,
            overidedMaterials = new List<string>() { materialName}
        };
        transaction.AddIfNotNull(plot.objectMaterialOverriders.Add(overrider));
        transaction.Dispatch();
    }

    public void ChangeAllMaterial()
    {
        Transaction transaction = new Transaction();
        transaction.AddIfNotNull(plot.objectMaterialsOverrided.SetValue(true));
        var mat = ScriptableObject.CreateInstance<PBRMaterial>();
        mat.baseColorFactor = color;
        // add mat on resources 
        transaction.AddIfNotNull(mat.GetLoadEntity());
        UMI3DEnvironment.Instance.GetComponentInChildren<UMI3DScene>().PreloadedMaterials.Add(mat);
        var overrider = new MaterialOverrider()
        {
            addMaterialIfNotExists = false,
            overrideAllMaterial = true,
            newMaterial = mat,
        };
        transaction.AddIfNotNull(plot.objectMaterialOverriders.Add(overrider));
        transaction.Dispatch();
    }

    public void ClearMaterial()
    {
        Transaction transaction = new Transaction();
        transaction.AddIfNotNull(plot.objectMaterialOverriders.SetValue(new List<MaterialOverrider>() {}));
        transaction.AddIfNotNull(plot.objectMaterialsOverrided.SetValue(false));
        transaction.Dispatch();

    }

    //add and remove materials
    private bool isMaterialAdded = false;
    private MaterialOverrider currentAddedOverrider;
    public void AddMaterial(UMI3DModel model)
    {
        if (!isMaterialAdded)
        {
            Transaction transaction = new Transaction();
            transaction.AddIfNotNull(model.objectMaterialsOverrided.SetValue(true));
            currentAddedOverrider = new MaterialOverrider()
            {
                addMaterialIfNotExists = true,
                overrideAllMaterial = true,
                newMaterial = matToAdd,
            };
            transaction.AddIfNotNull(model.objectMaterialOverriders.Add(currentAddedOverrider));
            transaction.Dispatch();
            isMaterialAdded = true;
        }
    }
    public void RemoveMaterial(UMI3DModel model)
    {
        if (isMaterialAdded)
        {
            Transaction transaction = new Transaction();
            transaction.AddIfNotNull(model.objectMaterialOverriders.Remove(currentAddedOverrider));
            transaction.Dispatch();
            isMaterialAdded = false;
        }
    }

    //Shader properties by dictionary
    private OriginalMaterial originalMatForEnvColoring;
    public void ActiveEnvColoringProperty(UMI3DModel model)
    {
        Transaction transaction = new Transaction();
        modelsToReset.Add(model);
        if(originalMatForEnvColoring == null)
        {
            originalMatForEnvColoring = ScriptableObject.CreateInstance<OriginalMaterial>();
            originalMatForEnvColoring.shaderProperties.Add("_ENVIRONMENT_COLORING", true);
            originalMatForEnvColoring.shaderProperties.Add("_EnvironmentColorThreshold", 2f);
            originalMatForEnvColoring.shaderProperties.Add("_EnvironmentColorIntensity", 1f);

            transaction.AddIfNotNull(originalMatForEnvColoring.GetLoadEntity());
            UMI3DEnvironment.Instance.GetComponentInChildren<UMI3DScene>().PreloadedMaterials.Add(originalMatForEnvColoring);
        }
        transaction.AddIfNotNull(model.objectMaterialsOverrided.SetValue(true));
        transaction.AddIfNotNull(model.objectMaterialOverriders.Add(new MaterialOverrider() {overrideAllMaterial = true, newMaterial = originalMatForEnvColoring}));
        transaction.Dispatch();
    }

    //Reset
    private List<Tuple<PBRMaterial, Color>> materialsToReset = new List<Tuple<PBRMaterial, Color>>();
    public List<AbstractRenderedNode> modelsToReset = new List<AbstractRenderedNode>();
    public void ResetMat()
    {
        Transaction transaction = new Transaction();
        foreach (AbstractRenderedNode item in modelsToReset)
        {
            transaction.AddIfNotNull(item.objectMaterialsOverrided.SetValue(false));
            transaction.AddIfNotNull(item.objectMaterialOverriders.SetValue(new List<MaterialOverrider>()));
        }
        foreach (Tuple<PBRMaterial, Color> item in materialsToReset)
        {
            transaction.AddIfNotNull(item.Item1.objectBaseColorFactor.SetValue(item.Item2));
        }

        color = Color.white;
        transaction.AddIfNotNull(currentColorDisplayer.objectMaterialsOverrided.SetValue(true));
        transaction.AddIfNotNull(pBRMaterial.objectBaseColorFactor.SetValue(color));
        transaction.AddIfNotNull(currentColorDisplayer.objectMaterialOverriders.SetValue(new List<MaterialOverrider>() { materialOverrider }));

        transaction.Dispatch();
    }

    //On destroy, reset existing pbr materials
    private void OnDestroy()
    {
        Transaction transaction = new Transaction();
        foreach (Tuple<PBRMaterial, Color> item in materialsToReset)
        {
            transaction.AddIfNotNull(item.Item1.objectBaseColorFactor.SetValue(item.Item2));
        }

    }


}
