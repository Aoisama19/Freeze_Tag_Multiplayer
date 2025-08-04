using System.Collections;
using UnityEngine;

public class DissolvingController : MonoBehaviour
{
    public float dissolveRate = 0.0125f;
    public float refreshRate = 0.025f;

    [SerializeField]
    private SkinnedMeshRenderer[] _skinnedMesh;
    private Material[][] _skinnedMaterials; // Jagged Array
    private int _dissolveAmount;

    private void Start()
    {
        var child = transform.GetChild(0);
        _skinnedMesh = child.GetComponentsInChildren<SkinnedMeshRenderer>();

        _dissolveAmount = Shader.PropertyToID("_DissolveAmount");
        _skinnedMaterials = new Material[_skinnedMesh.Length][];

        if (_skinnedMesh.Length > 0)
        {
            for (int i = 0; i < _skinnedMesh.Length; i++)
            {
                _skinnedMaterials[i] = _skinnedMesh[i].materials;
            }
        }
    }

    private void OnEnable()
    {
       // Actions.GameStart += GameStart;

        
    }

    private void OnDisable()
    {
       // Actions.GameStart -= GameStart;
    }

    private void GameStart()
    {
        //var child = transform.GetChild(0);
        //_skinnedMesh = child.GetComponentsInChildren<SkinnedMeshRenderer>();

        //_dissolveAmount = Shader.PropertyToID("_DissolveAmount");
        //_skinnedMaterials = new Material[_skinnedMesh.Length][];

        //if (_skinnedMesh.Length > 0)
        //{
        //    for (int i = 0; i < _skinnedMesh.Length; i++)
        //    {
        //        _skinnedMaterials[i] = _skinnedMesh[i].materials;
        //    }
        //}
    }

    public void Dissolve()
    {
        StartCoroutine(DissolveCo());
    }

    public void UnDissolve()
    {
        StartCoroutine(UnDissolveCo());
    }

    private IEnumerator DissolveCo()
    {
        if (_skinnedMesh.Length > 0 && _skinnedMaterials.Length > 0)
        {
            var counter = 0f;

            while (_skinnedMaterials[0][0].GetFloat(_dissolveAmount) < 1f)
            {
                counter += dissolveRate;

                for (int i = 0; i < _skinnedMesh.Length; i++)
                {
                    for (int j = 0; j < _skinnedMaterials[i].Length; j++)
                    {
                        _skinnedMaterials[i][j].SetFloat(_dissolveAmount, counter);
                    }
                }

                yield return new WaitForSeconds(refreshRate);
            }
        }
    }
    
    private IEnumerator UnDissolveCo()
    {
        if (_skinnedMesh.Length > 0 && _skinnedMaterials.Length > 0)
        {
            var counter = 1f;

            while (_skinnedMaterials[0][0].GetFloat(_dissolveAmount) > 0f)
            {
                counter -= dissolveRate;

                for (int i = 0; i < _skinnedMesh.Length; i++)
                {
                    for (int j = 0; j < _skinnedMaterials[i].Length; j++)
                    {
                        _skinnedMaterials[i][j].SetFloat(_dissolveAmount, counter);
                    }
                }

                yield return new WaitForSeconds(refreshRate);
            }
        }
    }
    
}
