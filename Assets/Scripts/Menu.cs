using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour {

    Plane _groundPlane = new Plane (Vector3.up, Vector3.zero); // Start is called before the first frame update
    public GameObject cameraTarget;
    public GameObject levelTarget;
    Vector3 worldPosition;
    GameObject prevSelected;
    public Manager manager;
    Color prevMaterial;
    // Update is called once per frame

    public Material availableMaterial;
    public Material completeMaterial;
    GameObject[] levels;
    public GameObject current;
    void Start () {
        levels = GameObject.FindGameObjectsWithTag ("Level");

        PlayerPrefs.SetString ("Available 1", "true");
        foreach (GameObject level in levels) {
            if (PlayerPrefs.GetString ("Complete " + level.name) != "") {
                level.GetComponent<Renderer> ().material.SetFloat ("Vector1_F1298B07", 1f);

            } else if (PlayerPrefs.GetString ("Available " + level.name) != "") {
                level.GetComponent<Renderer> ().material.SetFloat ("Vector1_4489BF63", 1f);
            }

        }
    }

    public void UpdateColors () {
        prevSelected = null;
        foreach (GameObject level in levels) {
            LevelState levelState = level.GetComponent<LevelState> ();

            if (levelState.recentlyUpdated) {
                if (levelState.complete) {
                    LeanTween.value (level, (value) => {
                        level.GetComponent<Renderer> ().material.SetFloat ("Vector1_F1298B07", value);
                    }, level.GetComponent<Renderer> ().material.GetFloat ("Vector1_4489BF63"), 1, 0.5f).setEaseOutQuad ();

                } else if (levelState.available) {
                    LeanTween.value (level, (value) => {
                        level.GetComponent<Renderer> ().material.SetFloat ("Vector1_4489BF63", value);
                    }, level.GetComponent<Renderer> ().material.GetFloat ("Vector1_808204E5"), 1, 0.5f).setEaseOutQuad ();

                }
                levelState.recentlyUpdated = false;
            }

        }
    }

    void Update () {

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);

        current = null;
        if (Physics.Raycast (ray, out hit)) {
            current = hit.collider.gameObject;

            if (current != prevSelected) {
                if (prevSelected != null) {
                    Unhighlight (prevSelected);
                }
                prevSelected = current;
                Highlight (current);
            }

            if (Input.GetMouseButtonDown (0)) {
                if (PlayerPrefs.GetString ("Available " + hit.collider.gameObject.name) != "") {
                    Unhighlight (current);
                    manager.PrepareLevel (hit.collider.gameObject.name);
                    LeanTween.move (cameraTarget, levelTarget.transform.position, 1.5f).setEaseInOutCubic ().setOnComplete (() => {
                        manager.StartLevel ();
                    });
                    gameObject.GetComponent<Menu> ().enabled = false;
                } else {
                    ErrorHighlight (hit.collider.gameObject);

                    print ("Not available");
                }
            }
        } else {
            if (prevSelected != null) {
                Unhighlight (prevSelected);
                prevSelected = null;
            }
        }

    }

    void Highlight (GameObject obj) {
        LeanTween.cancel (obj);
        LeanTween.value (obj, (value) => {
            obj.GetComponent<Renderer> ().material.SetFloat ("Vector1_808204E5", value);
        }, obj.GetComponent<Renderer> ().material.GetFloat ("Vector1_808204E5"), 1, 0.5f).setEaseOutQuad ();
        LeanTween.value (obj, (value) => {
            obj.GetComponent<Renderer> ().material.SetFloat ("Vector1_FBB218A6", value);
        }, obj.GetComponent<Renderer> ().material.GetFloat ("Vector1_FBB218A6"), 0, 0.5f);
    }

    void Unhighlight (GameObject obj) {
        LeanTween.cancel (obj);
        LeanTween.value (obj, (value) => {
            obj.GetComponent<Renderer> ().material.SetFloat ("Vector1_808204E5", value);
        }, obj.GetComponent<Renderer> ().material.GetFloat ("Vector1_808204E5"), 0, 0.3f).setEaseOutQuad ();

        LeanTween.value (obj, (value) => {
            obj.GetComponent<Renderer> ().material.SetFloat ("Vector1_FBB218A6", value);
        }, obj.GetComponent<Renderer> ().material.GetFloat ("Vector1_FBB218A6"), 0, 0.3f);
    }

    void ErrorHighlight (GameObject obj) {
        LeanTween.cancel (obj);
        LeanTween.value (obj, (value) => {
            obj.GetComponent<Renderer> ().material.SetFloat ("Vector1_FBB218A6", value);
        }, obj.GetComponent<Renderer> ().material.GetFloat ("Vector1_FBB218A6"), 1, 0.3f).setOnComplete ((ErrorDehighlight)).setOnCompleteParam (obj);
        LeanTween.value (obj, (value) => {
            obj.GetComponent<Renderer> ().material.SetFloat ("Vector1_808204E5", value);
        }, obj.GetComponent<Renderer> ().material.GetFloat ("Vector1_808204E5"), 1, 0.5f).setEaseOutQuad ();
    }

    void ErrorDehighlight (object param) {
        GameObject obj = param as GameObject;
        LeanTween.value (obj, (value) => {
            obj.GetComponent<Renderer> ().material.SetFloat ("Vector1_FBB218A6", value);
        }, obj.GetComponent<Renderer> ().material.GetFloat ("Vector1_FBB218A6"), 0, 0.3f);
    }

}