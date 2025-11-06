using UnityEngine;
using UnityEngine.InputSystem;

public class BoatMovement : MonoBehaviour
{
    string direction = "up";
    public InputAction LeftTurn;
    public InputAction ForwardMovement;
    public InputAction RightTurn;
    private int rotation;

    // Update is called once per frame
    void Update()
    {   //I am 1 Million percent sure there's a better way to handle this, but I do not have the time to reassess
        if (Keyboard.current.wKey.wasPressedThisFrame){
            NorthMovement();
        }
        if (Keyboard.current.aKey.wasPressedThisFrame){
            EastMovement();
        }
        if (Keyboard.current.dKey.wasPressedThisFrame){
            WestMovement();
        }
        if (Keyboard.current.sKey.wasPressedThisFrame){
            SouthMovement();
        }

    }
    void NorthMovement(){
        Vector2 position = transform.position;
        if(direction == "up"){
            position.y +=1.0f;
            transform.position = position;
            Debug.Log("Moved North");
        }
        if(direction == "right"){
            RotateLeft();
            direction = "up";
            position.x +=.5f;
            position.y +=.5f;
            transform.position = position;
            Debug.Log("Turned North");  
        }
        if(direction == "left"){
            RotateRight();
            direction = "up";
            position.x -=0.5f;
            position.y +=0.5f;
            transform.position = position;
            Debug.Log("Turned North");  
        }
    }
    void EastMovement(){
        Vector2 position = transform.position;
        if(direction == "left"){
            position.x -=1.0f;
            transform.position = position;
            Debug.Log("Moved East");
        }
        if(direction == "down"){
            RotateRight();
            direction = "left";
            position.x -=0.5f;
            position.y -=0.5f;
            transform.position = position;
            Debug.Log("Turned East");
        }
        if(direction == "up"){
            RotateLeft();
            direction = "left";
            position.x -=0.5f;
            position.y +=0.5f;
            transform.position = position;
            Debug.Log("Turned East 2");
        }
    }
    void WestMovement(){
        Vector2 position = transform.position;
        if(direction == "right"){
            position.x +=1.0f;
            transform.position = position;
            Debug.Log("Moved West");
        }
        if(direction == "down"){
            RotateLeft();
            direction = "right";
            position.x +=0.5f;
            position.y -=0.5f;
            transform.position = position;
            Debug.Log("Turned West");
        }
        if(direction == "up"){
            RotateRight();
            direction = "right";
            Debug.Log("Turned West 2");
        }
    }
    void SouthMovement(){
        Vector2 position = transform.position;
        if(direction == "down"){
            position.y -=1.0f;
            transform.position = position;
            Debug.Log("Moved South");
        }
        if(direction == "right"){
            RotateRight();
            direction = "down";
            position.y -=0.5f;
            position.x +=0.5f;
            transform.position = position;
            Debug.Log("Turned South");  
        }
        if(direction == "left"){
            RotateLeft();
            direction = "down";
            Debug.Log("Turned South");  
        }
    }
    void RotateLeft(){
        Vector2 position = transform.position;
        rotation += 90;
        position.y -= 0.5f;
        position.x -= 0.5f;
        transform.rotation = Quaternion.Euler(Vector3.forward * rotation);
        transform.position = position;
    }
    void RotateRight(){
        Vector2 position = transform.position;
        rotation -= 90;
        position.y += 0.5f;
        position.x += 0.5f;
        transform.rotation = Quaternion.Euler(Vector3.forward * rotation);
        transform.position = position;
    }
}
