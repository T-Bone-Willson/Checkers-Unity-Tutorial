using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour {

    public bool isWhite;
    public bool isKing;

    public bool ValidMove(Piece[,] board, int x1, int y1, int x2, int y2)
    {
        // If you are moving on top of another piece, then deny that move because it's illegal
        if (board[x2, y2] != null)
            return false;

        // THIS COULD BE IMPORTANT TO TRACK THE EXACT MOVES AND REPLAY THE GAME!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        // Keep track of the number of tiles that have been jumped via x axis
        int jumpedTilesX = Mathf.Abs(x1 - x2);
        // Keep track of the number of tiles that have been jumped via y axis
        // subtract value y2 to value of y1, gives new cordinate
        int jumpedTilesY = y2 - y1;
        // If jump 1 tile, it's a move. If jumped two tiles, it's a kill!
        // WHITE TEAM MOVE SET
        if (isWhite || isKing)
        {
            if (jumpedTilesX == 1)
            {
                if (jumpedTilesY == 1)
                    return true;
            }
            else if (jumpedTilesX == 2)
            {
                if (jumpedTilesY == 2)
                {
                    Piece p = board[(x1 + x2) / 2, (y1 + y2) / 2];
                    // If piece piece is jumping and it's not null AND NOT over the same colour as our piece, then allowed to jump over by two tiles.
                    if (p != null && p.isWhite != isWhite)
                        return true;
                }
            }
        }
        //BLACK TEAM MOVE SET
        if (!isWhite || isKing)
        {
            if (jumpedTilesX == 1)
            {
                if (jumpedTilesY == -1)
                    return true;
            }
            else if (jumpedTilesX == 2)
            {
                if (jumpedTilesY == -2)
                {
                    Piece p = board[(x1 + x2) / 2, (y1 + y2) / 2];
                    // If piece piece is jumping and it's not null AND NOT over the same colour as our piece, then allowed to jump over by two tiles.
                    if (p != null && p.isWhite != isWhite)
                        return true;
                }
            }
        }
        // If it's not a piece in the same team, it's not jumping two tiles in "x" OR "y", 
        // if the piece is null OR is the same colour as our piece, then deny move.
        return false;
    }




    /*
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}*/
}
