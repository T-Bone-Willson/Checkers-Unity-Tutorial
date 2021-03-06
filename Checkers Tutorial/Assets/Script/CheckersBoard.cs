﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckersBoard : MonoBehaviour {

    // Sets data 8 by 8 for the 2D array. Which tells us the amount of black and white pieces for the board
    // Also creates the class "Piece"
    public Piece[,] pieces = new Piece[8, 8];
    // Creates game objects for board. This allows me to drag and drop the relevant art asset to the relevant GameObject
    public GameObject whitePiecePrefab;
    public GameObject blackPiecePrefab;
    // Vector3 data realigns objects within "boardOffset" method
    private Vector3 boardOffset = new Vector3(-4.0f, 0.0f, -4.0f);
    // Realigns gameObjects (checker pieces) to position of board. 
    private Vector3 pieceOffset = new Vector3(0.5f, 0.0f, 0.5f);
    public bool isWhite;
    private bool isWhiteTurn;
    private bool hasKilled;
    private Piece selectedPiece;
    private List<Piece> forcedPieces;
    private Vector2 mouseOver;
    private Vector2 startDrag;
    private Vector2 endDrag;
    // Undo Button Stack
    Stack<Vector2> undoStack = new Stack<Vector2>();

    // Use this for initialization
    private void Start()
    {
        isWhite = true;    
        isWhiteTurn = true;
        forcedPieces = new List<Piece>();
        GenerateBoard();
    }

    // updates the game every frame.
    private void Update()
    {
        UpdateMouseOver();

        // Ternary operator; If it's white, return is white turn, else is not white turn.
        if((isWhite)?isWhiteTurn:!isWhiteTurn)
        {
            int x = (int)mouseOver.x;
            int y = (int)mouseOver.y;

            if (selectedPiece != null)
                UpdatePieceDrag(selectedPiece);

            // If pressed mouse button, then we select piece
            if (Input.GetMouseButtonDown(0))
                SelectPiece(x, y);
            // when release mouse button, attemps to confirm/initiate the move.
            if (Input.GetMouseButtonUp(0))
                //x and y values refer to "mouseOver" integer
                TryMove((int)startDrag.x, (int)startDrag.y, x, y);
        }
    }

    // Mouse controls
    private void UpdateMouseOver()
    {
        // My Turn
        // Check to see if main camera is aparent.
        if (!Camera.main)
        {
            Debug.Log("Unable to main Camera");
            return;
        }
        // Aligns camera to mouse click input/Point Of View
        RaycastHit pov;
        // Store value of the "mousePosition" into variable "pov"
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out pov, 25.0f, LayerMask.GetMask("Board")))
        {
            // x, y and z are axis's in Vector2
            mouseOver.x = (int)(pov.point.x - boardOffset.x);
            mouseOver.y = (int)(pov.point.z - boardOffset.z);
        }
        else
        {
            // if raycast doesnt hit, then equals -1. A non hit essentially.
            mouseOver.x = -1;
            mouseOver.y = -1;
        }
    }

    private void UpdatePieceDrag(Piece p)
    {
        
        if (!Camera.main)
        {
            Debug.Log("Unable to main Camera");
            return;
        }
        RaycastHit pov;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out pov, 25.0f, LayerMask.GetMask("Board")))
        {
            // If valid object i.e checker piece, to move. Then raise/elevate checker piece above board
            p.transform.position = pov.point + Vector3.up;
        }
       
    }

    private void SelectPiece(int x, int y)
    {
        // Determines if player is out of bounds, in relation to the range of the array
        if (x < 0 || x >= 8 || y < 0 || y >= 8)
            return;

        Piece p = pieces[x, y];
        // if null, you don't recieve debug log. Means things are working.
        if (p != null && p.isWhite == isWhite)
        {
            // Checks if there is no forced move rule that needs to be acted
            if (forcedPieces.Count == 0)
            {
                selectedPiece = p;
                startDrag = mouseOver;
            }
            else
            {
                // Look for the piece under our forced pieces list
                // Use "Lambda" expression which is an "anonymous" method
                if (forcedPieces.Find(forcedP => forcedP == p) == null)
                    return;

                selectedPiece = p;
                startDrag = mouseOver;
            }
        }
    }

    // x1 & y1 mean start position, x2 & y2 mean end position
    private void TryMove(int x1, int y1, int x2, int y2)
    {
        forcedPieces = ScanForMove();

        startDrag = new Vector2(x1, y1);
        endDrag = new Vector2(x2, y2);
        selectedPiece = pieces[x1, y1];

        // Stops players moving checker off of board by checking if it placement cordinate is below minimum or equal to array length of 8,8
        if (x2 < 0 || x2 >= 8 || y2 < 0 || y2 >= 8)
        {
            // This resets the piece to where it initially was, if placed out of bounds or an illegal move,
            if (selectedPiece != null)
                MovePiece(selectedPiece, x1, y1);

            startDrag = Vector2.zero;
            selectedPiece = null;
            return;
        }
        // Checks if there is any piece selected, if not, then nothing happens
        if (selectedPiece != null)
        {
            // If it has not moved
            if (endDrag == startDrag)
            {
                MovePiece(selectedPiece, x1, y1);
                startDrag = Vector2.zero;
                selectedPiece = null;
                return;
            }
            // Check if it's a valid move by refering to the class "Piece" via "pieces"
            if (selectedPiece.ValidMove(pieces, x1, y1, x2, y2))
            {
                // Did we kill?
                // If this is a jump
                if (Mathf.Abs(x2 - x1) == 2)
                {
                    Piece p = pieces[(x1 + x2) / 2, (y1 + y2) / 2];
                    if (p != null)
                    {
                        //Destorys piece that has been jumped over.
                        pieces[(x1 + x2) / 2, (y1 + y2) / 2] = null;
                        DestroyImmediate(p.gameObject);
                        hasKilled = true;
                    }
                }

                // Were we supposed to kill anything?
                if (forcedPieces.Count != 0 && !hasKilled)
                {
                    MovePiece(selectedPiece, x1, y1);
                    // resets startDrag
                    startDrag = Vector2.zero;
                    // resets selectedPiece back to null
                    selectedPiece = null;
                    return;
                }

                // Sets piece thats moved into new value whithin the array, which are contained at "x2" and "y2" 
                pieces[x2, y2] = selectedPiece;
                // Makes previous data stored on the array of piece that has just moved, which are contained on "x1" and "y1", null
                pieces[x1, y1] = null;
                MovePiece(selectedPiece, x2, y2);
                // Ends turn once move has been made

                // Puts destination coordinates of checker piece into a stack that takes in vector 2 daa
                undoStack.Push(endDrag);
                
                EndTurn();
            }
            // If trying to make invalid move, it will then drop the piece back into original position.
            else
            {
                MovePiece(selectedPiece, x1, y1);                
                startDrag = Vector2.zero;
                selectedPiece = null;
                return;
            }
        }
    }

    //when undo button is pressed
    public void OnUndoClick()
    {
        Debug.Log("You Pressed Undo!");
        // Takes one instance of the vector 2 data "endDrag" off the top of the stack.
        undoStack.Pop();
    }

    public void OnRedoClick()
    {
        Debug.Log("You Pressed Redo!");
    }

    private void EndTurn()
    {
        // Keep track of where we landed during our turn
        int x = (int)endDrag.x;
        int y = (int)endDrag.y;

        // Promotion to King!
        // If white piece gets to the top of the bvoard (7) then it becomes a "King" piece
        if (selectedPiece != null)
        {
            if (selectedPiece.isWhite && !selectedPiece.isKing && y == 7)
            {
                selectedPiece.isKing = true;
                selectedPiece.transform.Rotate(Vector3.right * 180);
            }
            // If black piece gets to the bottom of the board (0) then it becomes a "King" piece
            else if (!selectedPiece.isWhite && !selectedPiece.isKing && y == 0)
            {
                selectedPiece.isKing = true;
                selectedPiece.transform.Rotate(Vector3.right * 180);
            }
        }

        selectedPiece = null;
        startDrag = Vector2.zero;

        // If piece that has just moved & has killed something, but has another opertunity to kill; Allow them to do so
        if (ScanForMove(selectedPiece, x, y).Count != 0 && hasKilled)
            return;

        isWhiteTurn = !isWhiteTurn;
        // Switches from white team to black team, each time a valid move or sequence of moves are done
        isWhite = !isWhite;
        hasKilled = false;
        CheckVictory();
    }

    private void CheckVictory()
    {
        var blackWhitePiece = FindObjectsOfType<Piece>();
        bool hasWhite = false, hasBlack = false;
        for (int i = 0; i < blackWhitePiece.Length; i++)
        {
            // Checks if all white pieces are gone
            if (blackWhitePiece[i].isWhite)
                hasWhite = true;
            else
                hasBlack = true;
        }
        // No white pieces left = to you lose
        if (!hasWhite)
        
            Victory(false);
             
        // No black pieces left = to you win
        if (!hasBlack)
        
            Victory(true);       
    }

    private void Victory(bool isWhite)
    {
        if (isWhite)
            Debug.Log("White Team Has Won");
        else
            Debug.Log("Black Team Has Won");
    }

    private List<Piece> ScanForMove(Piece p, int x, int y)
    {
        forcedPieces = new List<Piece>();

        if (pieces[x, y].ForcedMove(pieces, x, y)) 
            forcedPieces.Add(pieces[x, y]);

        return forcedPieces;
    }

    private List<Piece> ScanForMove()
    {
        forcedPieces = new List<Piece>();

        //Check all the pieces, all the time
        // Left to Right of the 2D array
        for (int dimensionX = 0; dimensionX < 8; dimensionX++)
            // Up to Down of the 2D array
            for (int dimensionY = 0; dimensionY < 8; dimensionY++)
                // Checks X & Y cordinates on board (pieces), if there is a piece (!= null) and the piece is "White". Then it's your turn
                if (pieces[dimensionX, dimensionY] != null && pieces[dimensionX, dimensionY].isWhite == isWhiteTurn)
                    // Check if move has to be forced.
                    if (pieces[dimensionX, dimensionY].ForcedMove(pieces, dimensionX, dimensionY))
                        forcedPieces.Add(pieces[dimensionX, dimensionY]);

        return forcedPieces;
    }

    // Method to generate pieces on board
    private void GenerateBoard()
    {
        // Generate White team, sets at bottom three rows of the board
        for(int up2Down = 0; up2Down < 3; up2Down++)
        {
            // Determines if it's an odd row or not. This is by using a Modulu operator
            bool oddRow = (up2Down % 2 == 0);
            // Makes pieces generate from left to right, going across the bottom three rows of the board
            for (int left2Right = 0; left2Right < 8; left2Right += 2)
            {
                // Generate our Piece by using a Ternary operator
                // if our pieve is on odd row; place. If it's not; move over by 1 and place.
                GeneratePiece((oddRow)? left2Right : left2Right +1, up2Down);
            }
        }

        // Generate Black team, sets at top three rows of the board
        // int "up2Down" is set at 7 because our array is 8 by 8, and our code reads from 0 to 7
        // which means it's the top row on the board.
        for (int up2Down = 7; up2Down > 4; up2Down--)
        {
            bool oddRow = (up2Down % 2 == 0);
            for (int left2Right = 0; left2Right < 8; left2Right += 2)
            {
                GeneratePiece((oddRow) ? left2Right : left2Right + 1, up2Down);
            }
        }
    }

    // Associates int "left2Right" and "up2Down" to GameObject's
    private void GeneratePiece(int left2Right, int up2Down)
    {
        // States that if int up2Down is > 3; Then it's black team. If less: White team. Use Ternary operator
        bool isPieceWhite = (up2Down > 3) ? false : true;
        // Ternary operator, if "isPieceWhite is white, then spawn "whitePiecePreFab". Else; Spawn "blackPiecePrefab"
        GameObject go = Instantiate((isPieceWhite) ? whitePiecePrefab : blackPiecePrefab) as GameObject;
        go.transform.SetParent(transform);
        Piece p = go.GetComponent<Piece>();
        // calls back to Array "pieces" that was made on line 8 and associates it to "p"
        pieces[left2Right, up2Down] = p;
        MovePiece(p, left2Right, up2Down);
    }

    // Method to place pieces in the correct quadrant of the board
    private void MovePiece(Piece p, int left2Right, int up2Down)
    {
        // Board in centre of the world, so there is offset. There's -4 on one side and 4 on the other
        // This fixes that
        p.transform.position = (Vector3.right * left2Right) + (Vector3.forward * up2Down) + boardOffset + pieceOffset;
    }
}
