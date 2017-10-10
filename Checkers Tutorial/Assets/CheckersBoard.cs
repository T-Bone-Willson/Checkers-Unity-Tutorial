using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckersBoard : MonoBehaviour {

    // Sets data for the array. Which tells us the amount of black and white pieces for the board
    public Piece[,] pieces = new Piece[8, 8];
    //Creates game objects for board. This allows me to drag and drop the relevant art to the relevant GameObject
    public GameObject whitePiecePrefab;
    public GameObject blackPiecePrefab;
    //Vector3 data realigns objects within "boardOffset" method
    private Vector3 boardOffset = new Vector3(-4.0f, 0, 0 - 4.0f);
    //Same as previous but for "pieceOffset" method
    private Vector3 pieceOffset = new Vector3(0.5f, 0, 0.5f);

    // Use this for initialization
    private void Start()
    {
        GenerateBoard();
    }

    //Method to generate pieces on board
    private void GenerateBoard()
    {
        // Generate White team, sets at bottom two rows of the board
        for(int up2Down = 0; up2Down < 3; up2Down++)
        {
            //Determines if it's an odd row or not. This is by using a Modulu operator
            bool oddRow = (up2Down % 2 == 0);
            //Makes pieces generate from left to right, going across the bottom two rows of the board
            for (int left2Right = 0; left2Right < 8; left2Right += 2)
            {
                //Generate our Piece by using a Ternary operator
                // if our pieve is on odd row; place. If it's not; move over by 1 and place.
                GeneratePiece((oddRow)? left2Right : left2Right +1, up2Down);
            }
        }

        //REVIEW CODE COMMENTS ABOVE ^^^ TO UNDERSTAND PROCESS. IT'S THE SAME APART FROM SOME VARIABLE VALUES

        //Generate Black team, sets at top two rows of the board
        //int "up2Down" is set at 7 because our array is 8 by 8, and our code reads from 0 to 7
        //which means it's the top row on the board.
        for (int up2Down = 7; up2Down > 4; up2Down--)
        {
            bool oddRow = (up2Down % 2 == 0);
            for (int left2Right = 0; left2Right < 8; left2Right += 2)
            {
                GeneratePiece((oddRow) ? left2Right : left2Right + 1, up2Down);
            }
        }

    }

    //Associates int "left2Right" and "up2Down" to GameObject's
    private void GeneratePiece(int left2Right, int up2Down)
    {
        //States that if int up2Down is > 3; Then it's black team. If less: White team. Use Ternary operator
        bool isPieceWhite = (up2Down > 3) ? false : true;
        //Ternary operator, if "isPieceWhite is white, then spawn "whitePiecePreFab". Else; Spawn "blackPiecePrefab"
        GameObject go = Instantiate((isPieceWhite) ? whitePiecePrefab : blackPiecePrefab) as GameObject;
        go.transform.SetParent(transform);
        Piece p = go.GetComponent<Piece>();
        //calls back to Array "pieces" that was made on line 8 and associates it to "p"
        pieces[left2Right, up2Down] = p;
        MovePiece(p, left2Right, up2Down);
    }

    //Method to place pieces in the correct quadrant of the board
    private void MovePiece(Piece p, int left2Right, int up2Down)
    {
        //Board in centre of the world, so there is offset. There's -4 on one side and 4 on the other
        //This fixes that
        p.transform.position = (Vector3.right * left2Right) + (Vector3.forward * up2Down) + boardOffset + pieceOffset;
    }



    /*
	// Update is called once per frame
	void Update () {
		
	}*/
}
