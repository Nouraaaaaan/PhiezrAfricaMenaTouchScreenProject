using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Coffee.UIEffects;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [Header("MainMenu Attributes")]
    public GameObject MainMenuUI;
    public GameObject ApplicationUI;
    public GameObject[] MainMenuCategoryButtons;
    //Animation Atrributes
    public float MainMenuCategoryButtonsAnimationWaitingTime;
    public float MainMenuCategoryButtonsAnimationTime;
    public float InitialMainMenuCategoryButtonPosition;
    public float FinalMainMenuCategoryButtonPosition;

    [Header("Project Buttons Attributes")]
    public GameObject CurrentProjectButton;
    public bool CanClickProjectButton = false;
    

    [Header("Info Panel Attributes")]
    public GameObject InfoPanelImage;
    public float InfoPanelShowTransitionValue;
    public float InfoPanelHideTransitionValue;
    public float InfoPanelTransitionTime;

    [Header("Map Image Attributes")]
    public Image ProjectMapImage;
    public UIDissolve UIDissolveManager;
    public float MapImageFadeinDissolveEffectSpeed;
    public float MapImageFadeoutDissolveEffectSpeed;

    [Header("Map Image Attributes")]
    public Image CategoryMapImage;

    [Header("Numbers List Attributes")]
    public int NumbersListCurrentIndex = 0;
    public GameObject PagesSelectionCanvas;
    public Image[] NumbersListImages;

    [Header("Category Selection Attributes")]
    public bool CanClickCategoryButton = true;
    public Image CategorySelectionLineImage;
    public Sprite[] CategorySelectionLine;
    public Button[] CategoryButtons;

    #region Category Buttons

    public void OnclickCategoryButton(int index)
    {
        if (CanClickCategoryButton)
        {
            //Stop all Corotinues.
            StopAllCoroutines();
            ApplicationManager.ApplicationManagerInstance.StopAllCorotinue();

            //Destroy previous project list.
            ApplicationManager.ApplicationManagerInstance.DestroyPreviousProjectList();

            //Instantiate current project list.
            OnclickMainMenuCategoryButton(index);

            //Show List Selection Menu.
            if (ApplicationManager.ApplicationManagerInstance.Categories[index].Pages.Count == 1)
            {
                PagesSelectionCanvas.SetActive(false);
            }

            //Reset Numbers List Current Index.
            NumbersListCurrentIndex = 0;
        }       
    }

    private void HighLightCategoryButton(int index)
    {
        CategoryButtons[index].GetComponent<Image>().color = Color.cyan;
    }

    private void ResetCategoryButtons()
    {
        foreach (var button in CategoryButtons)
        {
            button.GetComponent<Image>().color = Color.white;
        }
    }

    public void AnimateMainMenuCategoryButtons(float waitingTime)
    {
        StartCoroutine(AnimateMainMenuCategoryButtonsCorotinue(waitingTime));
    }

    private IEnumerator AnimateMainMenuCategoryButtonsCorotinue(float waitingTime)
    {
        for (int i = 0; i < MainMenuCategoryButtons.Length; i++)
        {
            LeanTween.moveLocalY(MainMenuCategoryButtons[i], InitialMainMenuCategoryButtonPosition, 0f);
        }

        yield return new WaitForSeconds(waitingTime);

        for (int i = 0; i < MainMenuCategoryButtons.Length; i++)
        {
            LeanTween.alphaCanvas(MainMenuCategoryButtons[i].gameObject.GetComponent<CanvasGroup>(), 1f, MainMenuCategoryButtonsAnimationTime);

            LeanTween.moveLocalY(MainMenuCategoryButtons[i], FinalMainMenuCategoryButtonPosition, MainMenuCategoryButtonsAnimationTime);

            yield return new WaitForSeconds(MainMenuCategoryButtonsAnimationTime / 2);
        }
    }

    #endregion

    #region Category Image

    public void SetCategoryImage(Sprite sprite)
    {
        CategoryMapImage.sprite = sprite;
        CategoryMapImage.SetNativeSize();
    }

    public void EnableCategoryImage(bool enableCategoryImage)
    {
        CategoryMapImage.gameObject.SetActive(enableCategoryImage);
    }

    #endregion

    #region Project Buttons

    public void OnclickProjectButton(GameObject button, int buttonIndex)
    {
        if (CanClickProjectButton)
        {
            StopAllCoroutines();

            //Disable Category Image.
            EnableCategoryImage(false);
            //Disable Project Buttons Selection.
            EnableProjectButtonsSelection(false);
            //Disable Category Button Selection
            CanClickCategoryButton = false;

            if (CurrentProjectButton != null)
            {
                CurrentProjectButton.GetComponent<Image>().color = Color.white;
            }

            //Set current project index.
            ApplicationManager.ApplicationManagerInstance.SetCurrentProjectIndex(buttonIndex);
            CurrentProjectButton = button;

            //Highlight button.
            button.GetComponent<Image>().color = Color.blue;

            //Set Info Panel Image.
            int projectIndex = ApplicationManager.ApplicationManagerInstance.GetCurrentProjectIndex();
            int categoryIndex = ApplicationManager.ApplicationManagerInstance.GetCurrentCayegoryIndex();
            InfoPanelImage.GetComponent<Image>().sprite = ApplicationManager.ApplicationManagerInstance.Categories[categoryIndex].Pages[ApplicationManager.ApplicationManagerInstance.GetCurrentPageIndex()].Projects[projectIndex].InfoPanel;

            //Show Info Panel Image only if it's not null (some projects do not have info panels).
            if (ApplicationManager.ApplicationManagerInstance.Categories[categoryIndex].Pages[ApplicationManager.ApplicationManagerInstance.GetCurrentPageIndex()].Projects[projectIndex].InfoPanel != null)
            {
                ShowInfoPanelImage();
            }
            else
            {
                EnableProjectButtonsSelection(true);
                CanClickCategoryButton = true;
            }
            
            //Set map Image.
            ProjectMapImage.sprite = ApplicationManager.ApplicationManagerInstance.Categories[categoryIndex].Pages[ApplicationManager.ApplicationManagerInstance.GetCurrentPageIndex()].Projects[projectIndex].Map;
            ProjectMapImage.SetNativeSize();

            //Show map Image.
            ShowMapImage();         
        }       
    }

    public void EnableProjectButtonsSelection(bool canUserClickProjectButton)
    {
        CanClickProjectButton = canUserClickProjectButton;
    }

    #endregion

    #region Info Panel Back Button

    public void OnclickInfoPanelBackButton()
    {
        //
        HideInfoPanelImage();

        //
        HideMapImage();

        //
        if (CurrentProjectButton != null)
        {
            CurrentProjectButton.GetComponent<Image>().color = Color.white;
        }

        //
        EnableProjectButtonsSelection(true);

        CanClickCategoryButton = true;
    }

    public void ShowInfoPanelImage()
    {
        LeanTween.moveLocalX(InfoPanelImage, InfoPanelShowTransitionValue, InfoPanelTransitionTime);
    }

    public void HideInfoPanelImage()
    {
        LeanTween.moveLocalX(InfoPanelImage, InfoPanelHideTransitionValue, InfoPanelTransitionTime);
    }

    #endregion

    #region Map Image Region

    public void SetMapImage()
    { 
    }

    public void ShowMapImage()
    {
        StartCoroutine(ShowMapImageCorotinue());
    }

    private IEnumerator ShowMapImageCorotinue()
    {
        float duration = 1f;

        while (duration > 0)
        {
            duration -= MapImageFadeinDissolveEffectSpeed;
            UIDissolveManager.effectFactor = duration;
            yield return new WaitForEndOfFrame();
        }

        yield return null;
    }

    public void HideMapImage()
    {
        StartCoroutine(HideMapImageCorotinue());
    }

    private IEnumerator HideMapImageCorotinue()
    {
        //Hide Map Image.
        float duration = 0f;
        while (duration < 1)
        {
            duration += MapImageFadeoutDissolveEffectSpeed;
            UIDissolveManager.effectFactor = duration;
            yield return new WaitForEndOfFrame();
        }

        //Enable Category Image.
        yield return new WaitForSeconds(0.1f);
        EnableCategoryImage(true);
    }

    #endregion

    #region Main Menu Region

    public void OnclickMainMenuCategoryButton(int index)
    {
        //Set Current Cayegory Index.
        ApplicationManager.ApplicationManagerInstance.SetCurrentCayegoryIndex(index);

        //Set Current page Index.
        ApplicationManager.ApplicationManagerInstance.SetCurrentPageIndex(0);

        //Show List Selection Menu.
        if (ApplicationManager.ApplicationManagerInstance.Categories[index].Pages.Count > 1)
        {
            PagesSelectionCanvas.SetActive(true);
            ResetPagesListButtons();
            HighLightNumberButton(0);
        }
        
        //Instantiate Projects List.
        ApplicationManager.ApplicationManagerInstance.InstantiateProjectsList();

        //Set Category Image.
        EnableCategoryImage(true);
        SetCategoryImage(ApplicationManager.ApplicationManagerInstance.Categories[index].Pages[0].PageStaticMapSprite);

        //Disable MainMenu UI, Enable Application UI.
        MainMenuUI.SetActive(false);
        ApplicationUI.SetActive(true);

        //HighLight selected Category Button.
        ResetCategoryButtons();
        HighLightCategoryButton(index);

        //Highlight current category button.
        CategorySelectionLineImage.sprite = CategorySelectionLine[index];

        //
        UIDissolveManager.effectFactor = 1f;
    }

    #endregion

    #region Pages List Region

    private void ResetPagesListButtons()
    {
        foreach (var image in NumbersListImages)
        {
            image.color = Color.white;
        }
    }

    public void HighLightNumberButton(int index)
    {
        NumbersListImages[index].color = Color.cyan;
    }

    public void OnclickLeftButton()
    {
        if ((NumbersListCurrentIndex >= 1) && (CanClickProjectButton))
        {
            NumbersListCurrentIndex = NumbersListCurrentIndex - 1;

            //1. Reset all Numbers.
            ResetPagesListButtons();

            //2. Highlight selected button.
            HighLightNumberButton(NumbersListCurrentIndex);

            //3. Disable current page.
            ApplicationManager.ApplicationManagerInstance.HidePageProjects(NumbersListCurrentIndex + 1);

            //4. Update page index.
            ApplicationManager.ApplicationManagerInstance.SetCurrentPageIndex(NumbersListCurrentIndex);

            //5. Show new projects.
            ApplicationManager.ApplicationManagerInstance.AnimatePageProjects(NumbersListCurrentIndex);

            //6. Change static map image.
            int CategoryIndex = ApplicationManager.ApplicationManagerInstance.GetCurrentCayegoryIndex();
            SetCategoryImage(ApplicationManager.ApplicationManagerInstance.Categories[CategoryIndex].Pages[NumbersListCurrentIndex].PageStaticMapSprite);
        }
    }

    public void OnclickRightButton()
    {
        if ((NumbersListCurrentIndex <= 2) && (CanClickProjectButton))
        {
            NumbersListCurrentIndex = NumbersListCurrentIndex + 1;

            //1.Reset all Numbers.
            ResetPagesListButtons();

            //2.Highlight selected button.
            HighLightNumberButton(NumbersListCurrentIndex);

            //3.Disable current page.
            ApplicationManager.ApplicationManagerInstance.HidePageProjects(NumbersListCurrentIndex - 1);

            //4.Update page index.
            ApplicationManager.ApplicationManagerInstance.SetCurrentPageIndex(NumbersListCurrentIndex);

            //5.Show new projects.
            ApplicationManager.ApplicationManagerInstance.AnimatePageProjects(NumbersListCurrentIndex);

            //6. Change static map image.
            int CategoryIndex = ApplicationManager.ApplicationManagerInstance.GetCurrentCayegoryIndex();
            SetCategoryImage(ApplicationManager.ApplicationManagerInstance.Categories[CategoryIndex].Pages[NumbersListCurrentIndex].PageStaticMapSprite);
        }
    }

    #endregion

    #region Home Button Region

    public void OnclickHomeButton()
    {
        SceneManager.LoadScene(0);
    }

    #endregion

}
