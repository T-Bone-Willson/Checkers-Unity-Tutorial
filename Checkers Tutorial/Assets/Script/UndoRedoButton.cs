using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UndoRedoButton : MonoBehaviour {


    //when undo button is pressed
    public void OnUndoClick()
    {
        Debug.Log("You Pressed Undo!");

    }

    public void OnRedoClick()
    {
        Debug.Log("You Pressed Redo!");
    }
    
}