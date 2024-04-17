using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackButton : MonoBehaviour
{

    public GameObject currentPanel;
    public GameObject mainPanel;

    // Start is called before the first frame update
    void Start()
    {

        currentPanel = this.gameObject.transform.parent.gameObject;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GoBack()
    {

        currentPanel.SetActive(false);
        mainPanel.SetActive(true);

    }

}
