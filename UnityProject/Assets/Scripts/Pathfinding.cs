using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    public GameObject tileMarker;
    public GameObject tileMarkerRed;
    public GameObject tileMarkerGreen;
    GameObject _tileMarker;

    public List<Vector3> GetMovePath(Vector3 clickedTile, Vector3 playerPos)
    {
        int[,] tiles = new int[100, 100]; 
        int h = 50;
        List<Vector3> pathToMove = new List<Vector3>();
        Queue<Vector3> tilesToCheck = new Queue<Vector3>();
        tilesToCheck.Enqueue(playerPos);
        Vector3 tile;
        //_tileMarker = tileMarkerRed;

        for (int x = 0; x < h*2; x++)
            for (int y = 0; y < h*2; y++)
            {
                tiles[x, y] = -10;
            }
        tiles[(int)playerPos.x + h, (int)playerPos.y + h] = 0;

        for (int i = 1; i < 10000; i++)
        {
            tile = tilesToCheck.Dequeue();

            if (!Physics2D.OverlapCircle(tile + Vector3.up, 0.2f) && tiles[(int)tile.x + h, (int)tile.y + h + 1] == -10)
            {
                tiles[(int)tile.x + h, (int)tile.y + h + 1] = tiles[(int)tile.x + h, (int)tile.y + h] + 1;
                tilesToCheck.Enqueue(tile + Vector3.up);

                //Debug.Log(tile + Vector3.up + " - added to queue");
                //Instantiate(_tileMarker, tile + Vector3.up, quaternion.identity);
            }
            if (!Physics2D.OverlapCircle(tile + Vector3.left, 0.2f) && tiles[(int)tile.x + h - 1, (int)tile.y + h] == -10)
            {
                tiles[(int)tile.x + h - 1, (int)tile.y + h] = tiles[(int)tile.x + h, (int)tile.y + h] + 1;
                tilesToCheck.Enqueue(tile + Vector3.left);

                //Debug.Log(tile + Vector3.left + " - added to queue");
                //Instantiate(_tileMarker, tile + Vector3.left, quaternion.identity);
            }
            if (!Physics2D.OverlapCircle(tile + Vector3.right, 0.2f) && tiles[(int)tile.x + h + 1, (int)tile.y + h] == -10)
            {
                tiles[(int)tile.x + h + 1, (int)tile.y + h] = tiles[(int)tile.x + h, (int)tile.y + h] + 1;
                tilesToCheck.Enqueue(tile + Vector3.right);

                //Debug.Log(tile + Vector3.right + " - added to queue");
                //Instantiate(_tileMarker, tile + Vector3.right, quaternion.identity);
            }
            if (!Physics2D.OverlapCircle(tile + Vector3.down, 0.2f) && tiles[(int)tile.x + h, (int)tile.y + h - 1] == -10)
            {
                tiles[(int)tile.x + h, (int)tile.y + h - 1] = tiles[(int)tile.x + h, (int)tile.y + h] + 1;
                tilesToCheck.Enqueue(tile + Vector3.down);

                //Debug.Log(tile + Vector3.down + " - added to queue");
                //Instantiate(_tileMarker, tile + Vector3.down, quaternion.identity);
            }

            if (tile == clickedTile)
            {
                //Debug.Log(tiles[(int)tile.x + h, (int)tile.y + h] + " - distance");
                //Debug.Log("Found path" + tile.ToString());
                pathToMove.Add(tile);
                Instantiate(tileMarker, tile, quaternion.identity);
                break;
            }
        }


        for (int i = 0; i <= tiles[(int)pathToMove[0].x + h, (int)pathToMove[0].y + h]; i++)
        {
            if (tiles[(int)pathToMove[i].x + h + 1, (int)pathToMove[i].y + h] == tiles[(int)pathToMove[i].x + h, (int)pathToMove[i].y + h] - 1 )
            {
                pathToMove.Add(new Vector3((pathToMove[i].x + 1), (pathToMove[i].y), 0));
            }
            else if (tiles[(int)pathToMove[i].x + h - 1, (int)pathToMove[i].y + h] == tiles[(int)pathToMove[i].x + h, (int)pathToMove[i].y + h] - 1)
            {
                pathToMove.Add(new Vector3((pathToMove[i].x - 1), (pathToMove[i].y), 0));
            }
            else if (tiles[(int)pathToMove[i].x + h, (int)pathToMove[i].y + h + 1] == tiles[(int)pathToMove[i].x + h, (int)pathToMove[i].y + h] - 1)
            {
                pathToMove.Add(new Vector3((pathToMove[i].x), (pathToMove[i].y + 1), 0));
            }
            else if (tiles[(int)pathToMove[i].x + h, (int)pathToMove[i].y + h - 1] == tiles[(int)pathToMove[i].x + h, (int)pathToMove[i].y + h] - 1)
            {
                pathToMove.Add(new Vector3((pathToMove[i].x), (pathToMove[i].y - 1), 0));
            }
            else if(pathToMove[i] == playerPos)
            {
                Debug.Log("path found");
                return pathToMove;
            }
            else
            {
                Debug.Log("No path found");
            }
        }

        return null;
    }
}
