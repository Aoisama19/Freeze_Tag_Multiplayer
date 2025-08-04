using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshTrail : MonoBehaviour
{
    [Header("Mesh Related")] 
    public float meshRefreshRate = 0.1f;
    public float meshDestroyDelay = 1f;
    public Transform positionToSpawn;

    [Header("Shader Related")] 
    public Material mat;
    public string shaderValRef;
    public float shaderVarRate = 0.1f;
    public float shaderVarRefreshRate = 0.05f;

    private bool isTrailActive;
    private SkinnedMeshRenderer[] _skinnedMeshRenderers;

    private void Start()
    {
        if (!positionToSpawn)
        {
            positionToSpawn = transform.parent.transform;
        }
    }

    public void StartTrail()
    {
        if (!isTrailActive)
        {
            isTrailActive = true;
            StartCoroutine(ActivateTrail());
        }
    }

    public void StopTrail()
    {
        isTrailActive = false;
    }

    IEnumerator ActivateTrail()
    {
        while (isTrailActive)
        {
            if (_skinnedMeshRenderers == null)
                _skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();

            for (int i = 0; i < _skinnedMeshRenderers.Length; i++)
            {
                GameObject gObj = new GameObject();
                gObj.transform.SetPositionAndRotation(positionToSpawn.position, positionToSpawn.rotation);

                MeshRenderer mr = gObj.AddComponent<MeshRenderer>();
                MeshFilter mf = gObj.AddComponent<MeshFilter>();

                Mesh mesh = new Mesh();
                _skinnedMeshRenderers[i].BakeMesh(mesh);
                
                mf.mesh = mesh;
                mr.material = mat;

                StartCoroutine(AnimateMaterialFloat(mr.material, 0, shaderVarRate, shaderVarRefreshRate));
                
                Destroy(gObj, meshDestroyDelay);
            }

            yield return new WaitForSeconds(meshRefreshRate);
        }
    }

    IEnumerator AnimateMaterialFloat(Material mat, float goal, float rate, float refreshRate)
    {
        float valueToAnimate = mat.GetFloat(shaderValRef);

        while (valueToAnimate > goal)
        {
            valueToAnimate -= rate;
            mat.SetFloat(shaderValRef, valueToAnimate);
            yield return new WaitForSeconds(refreshRate);
        }
    }
}
