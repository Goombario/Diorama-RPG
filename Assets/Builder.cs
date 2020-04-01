using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Builder : MonoBehaviour
{
    [SerializeField] private Transform previewCube = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        previewCube.GetComponent<MeshRenderer>().enabled = false;
        ShowPreview();
    }

    void ShowPreview()
    {
        // Raycast tutorial: https://www.youtube.com/watch?v=_yf5vzZ2sYE

        var cast = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(cast, out hit))
        {
            var selection = hit.transform;
            var center = selection.GetComponent<Renderer>().bounds.center;
            previewCube.SetPositionAndRotation(center + hit.normal, Quaternion.identity);
            previewCube.GetComponent<MeshRenderer>().enabled = true;
            
        }
    }
}
