using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Builder : MonoBehaviour
{
    [SerializeField] private BoxCollider previewObject = null;
    [SerializeField] private BoxCollider instantiateObject;
    [SerializeField] private Material addMaterial;
    [SerializeField] private Material removeMaterial;
    [SerializeField] private bool createMode = true;

    private Transform _selection;
    private Vector3 _hitNormal;
    private Vector3 _offset;

    // Start is called before the first frame update
    void Start()
    {
        if (!previewObject) previewObject = GetComponentInChildren<BoxCollider>();
        previewObject.GetComponentInChildren<Renderer>().material = addMaterial;
    }

    // Update is called once per frame
    void Update()
    {
        previewObject.GetComponentInChildren<MeshRenderer>().enabled = false;

        if (createMode)
        {
            previewObject.GetComponentInChildren<Renderer>().material = addMaterial;
            ShowCreatingPreview2();

            if (Input.GetMouseButtonDown(0) && _selection)
            {
                CreateBlock2();
            }
        }
        else
        {
            previewObject.GetComponentInChildren<Renderer>().material = removeMaterial;
            ShowDeletingPreview();

            if (Input.GetMouseButtonDown(0) && _selection)
            {
                DeleteBlock();
            }
        }
    }

    private void CreateBlock2()
    {
        var newBlock = Instantiate(instantiateObject, previewObject.transform.position, Quaternion.identity, transform.parent);
    }

    void ShowCreatingPreview2()
    {
        if (_selection != null)
        {
            _selection = null;
        }

        var cast = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (!Physics.Raycast(cast, out hit)) return;

        BoxCollider boxCollider = hit.collider as BoxCollider;

        // if (!boxCollider)
        // {
        //     Debug.Log("Not a box collider");
        //     return;
        // }
        

        var previewPos = 
            hit.point +     // Need to round now, maybe based on the perpendicular to the normal?
            Vector3.Scale(previewObject.size/2f, hit.normal) -
            previewObject.center;
            //boxCollider.transform.position +
            //Vector3.Scale(boxCollider.size, hit.normal) + 

        previewPos = new Vector3(Mathf.Round(previewPos.x), Mathf.Round(previewPos.y), Mathf.Round(previewPos.z));
        previewObject.transform.SetPositionAndRotation(previewPos, Quaternion.identity);
        previewObject.GetComponentInChildren<MeshRenderer>().enabled = true;
        _selection = hit.transform;
        //Debug.Log(previewPos);
    }

    void DeleteBlock()
    {
        Destroy(_selection.gameObject);
        _selection = null;
    }

    void ShowDeletingPreview()
    {
        // Raycast tutorial: https://www.youtube.com/watch?v=_yf5vzZ2sYE
        if (_selection != null)
        {
            _selection = null;
        }

        var cast = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(cast, out hit))
        {
            var selection = hit.transform;
            var selCenter = selection.GetComponentInChildren<Renderer>().bounds.center;
            previewObject.transform.SetPositionAndRotation(selCenter, Quaternion.identity);
            previewObject.GetComponentInChildren<MeshRenderer>().enabled = true;
            _selection = selection;
            _hitNormal = hit.normal;
            _offset = selection.position - selCenter;
        }
    }

    void ShowCreatingPreview()
    {
        // Raycast tutorial: https://www.youtube.com/watch?v=_yf5vzZ2sYE
        if (_selection != null)
        {
            _selection = null;
        }

        var cast = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(cast, out hit))
        {
            var selection = hit.transform;
            var selCenter = selection.GetComponentInChildren<Renderer>().bounds.center;
            var previewCenter = previewObject.GetComponentInChildren<Renderer>().bounds.center;
            var prevOffset = previewObject.transform.position - previewCenter;
            previewObject.transform.SetPositionAndRotation(FindPreviewPoint(hit), Quaternion.identity);
            previewObject.GetComponentInChildren<MeshRenderer>().enabled = true;
            _selection = selection;
            _hitNormal = hit.normal;
            _offset = selection.position - selCenter;
        }
    }

    Vector3 FindPreviewPoint(RaycastHit hit)
    {
        var selection = hit.transform;
        var selCenter = selection.GetComponentInChildren<Renderer>().bounds.center;
        var selOffset = selection.position - selCenter;
        var previewCenter = previewObject.GetComponentInChildren<Renderer>().bounds.center;
        var prevOffset = previewObject.transform.position - previewCenter;

        var hitDirection = (hit.point - selCenter).normalized;
        float maxHit = Mathf.NegativeInfinity;
        int pos = 0;
        for (int i = 0; i < 3; i++)
        {
            if (Mathf.Abs(hitDirection[i]) > maxHit)
            {
                pos = i;
                maxHit = hitDirection[i];
            }
        }
        var hitDir = new Vector3();
        hitDir[pos] = maxHit;
        hitDir = hitDir.normalized;

        var difference = selection.GetComponentInChildren<Renderer>().bounds.extents[pos] + previewObject.GetComponentInChildren<Renderer>().bounds.extents[pos];
        var newPoint = selCenter;
        newPoint[pos] += difference;
        var hitDisplacement = new Vector3(selOffset.x * hitDirection.x, selOffset.y * hitDirection.y, selOffset.z * hitDirection.z);

        var previewPoint = selCenter + hitDisplacement + prevOffset;
        return newPoint;
    }

    void CreateBlock()
    {
        Debug.Log(_offset);
        var newBlock = Instantiate(instantiateObject, previewObject.transform.position, Quaternion.identity, transform.parent);
        //newBlock.Translate(_offset);
        newBlock.transform.Translate(-(previewObject.transform.position - previewObject.GetComponentInChildren<Renderer>().bounds.center));
    }

    public void SwitchMode(bool newMode)
    {
        createMode = newMode;
    }
}
