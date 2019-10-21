using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MenuButtons : MonoBehaviour
{

    public void StartButton() {//function for Play button
        GameObject saveObjects = Resources.Load("Collection") as GameObject;///collection is the gameobjects that persist through every level
        GameObject instance = Instantiate(saveObjects);//new collection for this playthrough
        DontDestroyOnLoad(instance);//persist through levels
        instance.GetComponentInChildren<GameManager>().saveState();//save at beginning of level
        SceneManager.LoadScene(1);//load first scene
    }

    public void LoadSave()//load last saved game
    {
        int level = PlayerPrefs.GetInt("Level");//load previously saved level
        float time=  PlayerPrefs.GetFloat("CurrentTime");//load saved time
        int  skulls= PlayerPrefs.GetInt("Skullls");//load number of skulls
        int  supers= PlayerPrefs.GetInt("Supers");//load supers left
        
        GameObject Collection = Resources.Load("Collection") as GameObject;//collection is the gameobjects that persist through every level
        GameObject instance = Instantiate(Collection);//create new set of collection
        DontDestroyOnLoad(instance);//persist through levels
        instance.GetComponentInChildren<GameManager>().setState(time, skulls, supers);//load previously saved variables
        SceneManager.LoadScene(level);//load previously saved levels
        
    }
    public void ExitGame()//close the game
    {
        Application.Quit();//closes the project (only works on build)
    }

    public void MainMenu() {//return to main menu
        GameObject Collection = GameObject.Find("Collection(Clone)");//Finds the gameobjects that are saved through the levels
        Destroy(Collection);//destroy those objects
        SceneManager.LoadScene(0);//load scene 0 (which is the main menu)

    }



}
