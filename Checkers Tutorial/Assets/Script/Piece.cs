using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour {

    public bool isWhite;
    public bool isKing;

    // References the board which is a 2 dimensional array
    public bool ForcedMove(Piece[,] board, int x, int y)
    {
        if (isWhite || isKing)
        {
            // Diagonaly Top Left
            // NOT TO SURE HOW THIS BIT WORKS, CHECK TUTORIAL 4, 3:00 MINS IN. MAYBE A HINT AT 4:30
            if (x >= 2 && y <= 5)
            {
                // x - 1 means: 1 to the left (-) of your piece
                // y + 1 means: 1 up/above (+) of your piece
                // So it checks 1 to left then 1 above from there. That sets it's Diagonaly Top Left.
                Piece p = board[x - 1, y + 1];
                // If there is a piece, and it is not the same colour as our
                if (p != null && p.isWhite != isWhite)
                {
                    // Check if there is a spare tile after jumping over piece
                    // x - 2 means: 2 to the left (-) of your piece
                    // y + 2 means: 2 up/above (+) of your piece
                    // "null" refers to empty tile
                    if (board[x - 2, y + 2] == null)
                        return true;
                }
            }

            // Diagonaly Top Right
            if (x <= 5 && y >= 5)
            {
                // x + 1 means: 1 to the right (+) of your piece
                // y + 1 means: 1 up/above (+) of your piece
                // So it checks 1 to the Right then 1 above from there. That sets it's Diagonaly Top Right.
                Piece p = board[x + 1, y + 1];
                if (p != null && p.isWhite != isWhite)
                {
                    if (board[x + 2, y + 2] == null)
                        return true;
                }
            }
        }
        //For Black Team
        if(!isWhite || isKing)
        {
            // Diagonaly Bottom Left
            if (x >= 2 && y >= 2)
            {
                // x - 1 means: 1 to the left (-) of your piece
                // y - 1 means: 1 down/below (-) of your piece
                // So it checks 1 to left then 1 down/below from there. That sets it's Diagonaly Bottom Left.
                Piece p = board[x - 1, y - 1];
                if (p != null && p.isWhite != isWhite)
                {
                    if (board[x - 2, y - 2] == null)
                        return true;
                }
            }

            // Diagonaly Bottom Right
            if (x <= 5 && y >= 2)
            {
                // x + 1 means: 1 to the right (+) of your piece
                // y - 1 means: 1 down/below (-) of your piece
                // So it checks 1 to right then 1 down/below from there. That sets it's Diagonaly Bottom Right.
                Piece p = board[x + 1, y - 1];
                if (p != null && p.isWhite != isWhite)
                {
                    if (board[x + 2, y - 2] == null)
                        return true;
                }
            }
        }
        return false;
    }

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
