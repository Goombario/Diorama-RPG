using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Builder : MonoBehaviour
{
    [SerializeField] private Transform previewCube = null;
    [SerializeField] private Transform instantiateObject;
    [SerializeField] private Material addMaterial;
    [SerializeField] private Material removeMaterial;
    [SerializeField] private bool createMode = true;

    private Transform _selection;
    private Vector3 _hitNormal;
    private Vector3 _offset;

    // Start is called before the first frame update
    void Start()
    {
        previewCube.GetComponent<Renderer>().material = addMaterial;
    }

    // Update is called once per frame
    void Update()
    {
        previewCube.GetComponent<MeshRenderer>().enabled = false;

        if (createMode)
        {
            previewCube.GetComponent<Renderer>().material = addMaterial;
            ShowCreatingPreview();

            if (Input.GetMouseButtonDown(0) && _selection)
            {
                CreateBlock();
            }
        }
        else
        {
            previewCube.GetComponent<Renderer>().material = removeMaterial;
            ShowDeletingPreview();

            if (Input.GetMouseButtonDown(0) && _selection)
            {
                DeleteBlock();
            }
        }
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
            var center = selection.GetComponent<Renderer>().bounds.center;
            previewCube.SetPositionAndRotation(center, Quaternion.identity);
            previewCube.GetComponent<MeshRenderer>().enabled = true;
            _selection = selection;
            _hitNormal = hit.normal;
            _offset = selection.position - center;
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
            var center = selection.GetComponent<Renderer>().bounds.center;
            previewCube.SetPositionAndRotation(center + hit.normal, Quaternion.identity);
            previewCube.GetComponent<MeshRenderer>().enabled = true;
            _selection = selection;
            _hitNormal = hit.normal;
            _offset = selection.position - center;
        }
    }

    void CreateBlock()
    {
        Debug.Log(_offset);
        var newBlock = Instantiate(instantiateObject, previewCube.transform.position, Quaternion.identity, transform.parent);
        newBlock.Translate(_offset);
    }

    public void SwitchMode(bool newMode)
    {
        createMode = newMode;
    }
}
