using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    
    private float currentTime = 0;//current time counter
    public int skulls = 0;//number skulls player has
    public int supers = 3;//number of supers left
    public GameObject pauseMenu;

    private void Start()
    {
        //DontDestroyOnLoad(GameObject.Find("Canvas"));//save text accross levels
        //DontDestroyOnLoad(this);//don't destroy gameManager
        GameObject.Find("skullsText").GetComponent<Text>().text = "Skulls: " + skulls;//text to display skulls

    }


    void Update()
    {
       
        Timer();

        if (!pauseMenu.activeInHierarchy && SceneManager.GetActiveScene().buildIndex != 4) {//if paused or last scene, stop timer
            currentTime += Time.deltaTime;//increase timer
            
        }
        if (Input.GetKeyDown(KeyCode.Escape) && SceneManager.GetActiveScene().buildIndex!=4) {//if escape key pressed (not active on last scene
            pauseMenu.SetActive(!pauseMenu.activeInHierarchy);//change to activate or deactivate if escape pressed
            GameObject.Find("Player").GetComponent<playerMovement>().gamePaused = pauseMenu.activeInHierarchy;//paused if pause menu active
        }
        if (SceneManager.GetActiveScene().buildIndex == 4) {//if last scene
            pauseMenu.SetActive(true);//pause menu always active on this scene
            GameObject.Find("Pause").GetComponent<Image>().sprite = null;//remove pause box
            GameObject.Find("Pause").GetComponent<Image>().color = Color.clear;//set pause box color to be clear
            resetSaveData();//reset save data after game won
        }

    }

    void Timer() {

        GameObject.Find("superLeft").GetComponent<Text>().text = "Supers: " + supers;//update supers number left text
        GameObject.Find("timerText").GetComponent<Text>().text = "Time: " + Math.Round((double)currentTime,2).ToString();//update timer text
    }

    public void addSkull()//function to add to skulls
    {
        skulls++;//increment skulls
        GameObject.Find("skullsText").GetComponent<Text>().text = "Skulls: " + skulls;// upldate skulls left text
    }

    public void usedSuper()//function to update when super is used
    {
        supers--;//decrement number of supers left
        GameObject.Find("skullsText").GetComponent<Text>().text = "Skulls: " + skulls;//change skull text
        GameObject.Find("superLeft").GetComponent<Text>().text = "Supers: " + supers;//change super text
    }

    public void saveState() {//save state is called at the end of each level
        PlayerPrefs.SetInt("Level", SceneManager.GetActiveScene().buildIndex +1);//set new level to saved level
        PlayerPrefs.SetFloat("CurrentTime", currentTime);//save time
        PlayerPrefs.SetInt("Skullls", skulls);//save number of skulls
        PlayerPrefs.SetInt("Supers", supers);//save number of supers left
    }

    public void setState(float time, int skulls, int supers) {//function to load saved variables
        this.skulls = skulls;//set skulls to previous saved skulls
        this.currentTime = time;//set current time to previous saved current time
        this.supers = supers;//set supers to previous saved supers count
    }

    public void resetSaveData()
    {
        PlayerPrefs.SetInt("Level", 1);//set new level to saved level
        PlayerPrefs.SetFloat("CurrentTime", 0);//save time
        PlayerPrefs.SetInt("Skullls", 0);//save number of skulls
        PlayerPrefs.SetInt("Supers", 3);//save number of supers left
    }

}
