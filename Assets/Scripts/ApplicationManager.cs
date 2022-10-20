using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ApplicationManager : MonoBehaviour
{
    [Header("Categories Attributes")]
    private int CurrentCategoryIndex;
    public Category[] Categories;

    [Header("Pages Attributes")]
    public int CurrentPageIndex;

    [Header("Projects Attributes")]
    private int CurrentProjectIndex;
    public GameObject ProjectButtonPrefab;                //button template to be instantiated.
    public GameObject ProjectButtonsVerticalLayoutPrefab; //parent to hold Project Buttons Vertical Layout.
    public Transform ProjectButtonsVerticalLayoutParent;
    public List<GameObject> Pages;                        //list of instantiated pages.
    public Transform ProjectButtonsParent;                //parent to hold buttons.
    private List<GameObject> InstantiatedProjectButtons;  //list of instantiated buttons.
    //Animation Attributes :
    public float TransitionValue;

    [Header("Managers Attributes")]
    public UIManager UIManager;

    #region Sinelton Region

    public static ApplicationManager ApplicationManagerInstance;

    private void Awake()
    {
        if (ApplicationManagerInstance == null)
        {
            ApplicationManagerInstance = this;
        }
    }

    #endregion

    #region Default Callbacks Region

    public void Start()
    {
        //Initialize.
        InstantiatedProjectButtons = new List<GameObject>();
        UIManager.PagesSelectionCanvas.SetActive(false);

        //AnimateMainMenuCategoryButtons.
        UIManager.AnimateMainMenuCategoryButtons(UIManager.MainMenuCategoryButtonsAnimationWaitingTime);
    }

    #endregion

    #region Categories Managment Region

    public void SetCurrentCayegoryIndex(int index)
    {
        CurrentCategoryIndex = index;
    }

    public int GetCurrentCayegoryIndex()
    {
        return CurrentCategoryIndex;
    }

    #endregion

    #region Pages Managment Region

    public void SetCurrentPageIndex(int index)
    {
        CurrentPageIndex = index;
    }

    public int GetCurrentPageIndex()
    {
        return CurrentPageIndex;
    }

    #endregion

    #region Projects Managment Region

    #region Project Index Managment

    public void SetCurrentProjectIndex(int index)
    {
        CurrentProjectIndex = index;
    }

    public int GetCurrentProjectIndex()
    {
        return CurrentProjectIndex;
    }

    #endregion

    #region Instantiate Projects List

    public void InstantiateProjectsList()
    {
        StartCoroutine(InstantiateProjectsListCorotinue());
    }

    private IEnumerator InstantiateProjectsListCorotinue()
    {
        //Disable Project Buttons Selection.
        UIManager.EnableProjectButtonsSelection(false);

        //Instantiate vertical layout according to number of pages.
        GameObject page;
        foreach (var item in Categories[GetCurrentCayegoryIndex()].Pages)
        {
            page = Instantiate(ProjectButtonsVerticalLayoutPrefab, ProjectButtonsVerticalLayoutParent);
            Pages.Add(page);
        }

        //Instantiate Projects for each page.
        InstaniateProjects();

        //Hide Projects.
        yield return new WaitForSeconds(0.01f);
        HideProjects();

        //Animate projects of first page Items.
        yield return new WaitForSeconds(0.01f);
        AnimatePageProjects(0);
    }

    private void InstaniateProjects()
    {
        //Instaniate projects for each page. 
        GameObject game_Object;
        for (int i = 0; i < Pages.Count; i++)
        {
            for (int j = 0; j < Categories[GetCurrentCayegoryIndex()].Pages[i].Projects.Count; j++)
            {
                //Instaniate.
                game_Object = Instantiate(ProjectButtonPrefab, Pages[i].transform);
                InstantiatedProjectButtons.Add(game_Object);
                //SetProjectButtonImage.
                game_Object.GetComponent<Image>().sprite = Categories[GetCurrentCayegoryIndex()].Pages[i].Projects[j].Button;
                game_Object.GetComponent<Image>().SetNativeSize();
                //SetIndex.
                game_Object.GetComponent<ProjectButton>().ButtonIndex = j;
            }
        }
    }

    private void HideProjects()
    {
        foreach (var item in InstantiatedProjectButtons)
        {
            item.transform.position -= new Vector3(TransitionValue, 0, 0);
        }
    }

    public void HidePageProjects(int pageIndex)
    {
        for (int i = 0; i < Pages[pageIndex].transform.childCount; i++)
        {
            Pages[pageIndex].transform.GetChild(i).gameObject.transform.position -= new Vector3(TransitionValue, 0, 0);
        }
    }

    public void AnimatePageProjects(int pageIndex)
    {
        StartCoroutine(AnimatePageProjectsCorotinue(pageIndex));
    }

    private IEnumerator AnimatePageProjectsCorotinue(int pageIndex)
    {
        //Disable Project Buttons Selection.
        UIManager.EnableProjectButtonsSelection(false);
        
        //Animate.
        for (int i = 0; i < Pages[pageIndex].transform.childCount; i++)
        {
            LeanTween.moveX(Pages[pageIndex].transform.GetChild(i).gameObject, (Pages[pageIndex].transform.GetChild(i).gameObject.transform.position.x + TransitionValue), 0.3f);
            LeanTween.alphaCanvas(Pages[pageIndex].transform.GetChild(i).gameObject.GetComponent<CanvasGroup>(), 1, 1.5f);
            yield return new WaitForSeconds(0.1f);
        }
       
        //Enable Project Buttons Selection.
        yield return new WaitForSeconds(0.5f);
        UIManager.EnableProjectButtonsSelection(true);
    }

    #endregion

    #region Destroy Projects List

    public void DestroyPreviousProjectList()
    {
        StartCoroutine(DestroyPreviousProjectListCorotinue());
    }

    private IEnumerator DestroyPreviousProjectListCorotinue()
    {
        //Destroy Pages.
        foreach (var page in Pages)
        {
            Destroy(page.gameObject);
        }

        //Clear Pages List.
        Pages.Clear();

        //Destroy Projects.
        foreach (var project in InstantiatedProjectButtons)
        {
            Destroy(project.gameObject);
        }

        //Clear Projects List.
        InstantiatedProjectButtons.Clear();

        //
        yield return null;
    }

    #endregion

    #endregion

    public void StopAllCorotinue()
    {
        StopAllCoroutines();
    }
}
