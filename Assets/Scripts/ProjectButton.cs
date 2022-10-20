using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectButton : MonoBehaviour
{
    public int ButtonIndex = -1;

    public void OnclickProjectButton()
    {
        ApplicationManager.ApplicationManagerInstance.UIManager.OnclickProjectButton(this.gameObject, ButtonIndex);
    }
}
