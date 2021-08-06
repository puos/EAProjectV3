using System.Collections;
using System.Collections.Generic;


public class MazeGen 
{
    public enum DIR
    {
        TOP = 0,
        BOTTOM,
        LEFT,
        RIGHT,
    }
    
    public class CTileAttrib
    {
        public int mX;
        public int mY;

        public bool left = false;
        public bool right = false;
        public bool top = false;
        public bool bottom = false;

        public bool IsVisited
        {
            get { return left || right || top || bottom; }
        }

        public CCube cubeObject = null;
    }

    Stack<CTileAttrib> tracks = new Stack<CTileAttrib>();

    CTileAttrib[] maze;

    private int mazeWidth = 0;
    private int mazeHeight = 0;

    public CTileAttrib[] CreateMap(int mazeWidth,int mazeHeight)
    {
        this.mazeWidth = mazeWidth;
        this.mazeHeight = mazeHeight;

        maze = new CTileAttrib[mazeWidth * mazeHeight];

        int iObj = 0;

        for(int i = 0; i < mazeHeight; ++i)
            for(int j = 0; j < mazeWidth; ++j)
            {
                CTileAttrib tile = new CTileAttrib();
                tile.mX = j;
                tile.mY = i;
                maze[iObj++] = tile;
            }

        tracks.Push(maze[0]);
        
        while(tracks.Count > 0)
        {
            CTileAttrib currentTile = tracks.Peek();
            CTileAttrib nextTile = CheckNeighborTiles(currentTile);

            if (nextTile == null)
            {
                tracks.Pop();
                continue;
            }

            VisitTile(currentTile, nextTile);

            tracks.Push(nextTile);
        }  
        
        return maze;
    }

    CTileAttrib GetTile(int x,int y) 
    {
        if (x < 0 || y < 0) return null;
        if (x >= mazeWidth || y >= mazeHeight) return null;

        return maze[mazeWidth * y + x];
    }

    /*
    private void RecursiveBackTracking()
    {
        if (tracks.Count <= 0) return;

        CTileAttrib currentTile = tracks.Peek();
        CTileAttrib nextTile = CheckNeighborTiles(currentTile);

        if(nextTile == null)
        {
            tracks.Pop();
            RecursiveBackTracking();
            return;
        }

        VisitTile(currentTile, nextTile);

        tracks.Push(nextTile);
        RecursiveBackTracking();
    }
    */

    private CTileAttrib CheckNeighborTiles(CTileAttrib tile)
    {
        CTileAttrib upTile    = GetTile(tile.mX, tile.mY + 1);
        CTileAttrib downTile  = GetTile(tile.mX, tile.mY - 1);
        CTileAttrib leftTile  = GetTile(tile.mX - 1, tile.mY);
        CTileAttrib rightTile = GetTile(tile.mX + 1, tile.mY);

        List<CTileAttrib> nbTiles = new List<CTileAttrib>();

        if(upTile != null)
        {
            if (!upTile.IsVisited)
                nbTiles.Add(upTile);
        }

        if(downTile != null)
        {
            if (!downTile.IsVisited)
                nbTiles.Add(downTile);
        }

        if(leftTile != null)
        {
            if (!leftTile.IsVisited)
                nbTiles.Add(leftTile);
        }

        if(rightTile != null)
        {
            if (!rightTile.IsVisited)
                nbTiles.Add(rightTile);
        }

        if (nbTiles.Count <= 0) return null;

        int r = UnityEngine.Random.Range(0, nbTiles.Count);

        return nbTiles[r];
    }

    private void OpenWall(DIR dir,CTileAttrib curTile,CTileAttrib nextTile)
    {
        switch(dir)
        {
            case DIR.BOTTOM: curTile.bottom = true; nextTile.top = true; break;
            case DIR.TOP:    curTile.top = true; nextTile.bottom = true; break;
            case DIR.LEFT:   curTile.left = true; nextTile.right = true; break;
            case DIR.RIGHT:  curTile.right = true; nextTile.left = true; break; 
        }
    }

    private void VisitTile(CTileAttrib curTile,CTileAttrib nextTile)
    {
        //UnityEngine.Debug.Log($"[{curTile.mX},{curTile.mY}] ==> [{nextTile.mX},{nextTile.mY}]");
        int checkDirection = 0;

        if(curTile.mX == nextTile.mX)
        {
            checkDirection = curTile.mY - nextTile.mY;
            DIR dir = (checkDirection < 0) ? DIR.TOP : DIR.BOTTOM;
            OpenWall(dir, curTile , nextTile);
        }
        else
        {
            checkDirection = curTile.mX - nextTile.mX;
            DIR dir = (checkDirection < 0) ? DIR.RIGHT : DIR.LEFT;
            OpenWall(dir, curTile, nextTile);
        }
    }
}
